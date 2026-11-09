
using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using AcaBiblioteca.Datos;
using AcaBiblioteca.Modelos;

namespace AcaBiblioteca
{
    public partial class FrmLibros : Form
    {
        private string? isbnSeleccionado = null;

        public FrmLibros()
        {
            InitializeComponent();

            ConectarBotones(this);

            dgvLibros.CellDoubleClick -= dgvLibros_CellDoubleClick;
            dgvLibros.CellDoubleClick += dgvLibros_CellDoubleClick;

            CargarAutores();
            CargarEditoriales();
            CargarLibros();
            LimpiarCampos();
        }

        private void ConectarBotones(Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (control is ButtonBase boton)
                {
                    string texto = boton.Text.Trim().ToLowerInvariant();

                    if (texto.Contains("guardar"))
                    {
                        boton.Click -= btnGuardar_Click;
                        boton.Click += btnGuardar_Click;
                    }
                    else if (texto.Contains("editar"))
                    {
                        boton.Click -= btnEditar_Click;
                        boton.Click += btnEditar_Click;
                    }
                    else if (texto.Contains("eliminar"))
                    {
                        boton.Click -= btnEliminar_Click;
                        boton.Click += btnEliminar_Click;
                    }
                    else if (texto.Contains("nuevo"))
                    {
                        boton.Click -= btnNuevo_Click;
                        boton.Click += btnNuevo_Click;
                    }
                    else if (texto.Contains("cancelar"))
                    {
                        boton.Click -= btnCancelar_Click;
                        boton.Click += btnCancelar_Click;
                    }
                    else if (texto.Contains("buscar"))
                    {
                        boton.Click -= btnBuscar_Click;
                        boton.Click += btnBuscar_Click;
                    }
                }

                if (control.HasChildren)
                    ConectarBotones(control);
            }
        }

        private void CargarLibros()
        {
            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(
                        @"SELECT ISBN, Titulo, IdAutor,
                                 IdEditorial, Categoria,
                                 Anio, Existencias
                          FROM dbo.Libros
                          ORDER BY Titulo", cn))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    dgvLibros.DataSource = tabla;
                    dgvLibros.ClearSelection();
                    dgvLibros.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MostrarError("cargar los libros", ex);
            }
        }

        private void CargarAutores()
        {
            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(
                        @"SELECT IdAutor,
                                 CONCAT(Nombre, ' ', Apellido)
                                 AS NombreCompleto
                          FROM dbo.Autores
                          ORDER BY Nombre, Apellido", cn))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cmbAutor.DataSource = tabla;
                    cmbAutor.DisplayMember = "NombreCompleto";
                    cmbAutor.ValueMember = "IdAutor";
                    cmbAutor.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MostrarError("cargar los autores", ex);
            }
        }

        private void CargarEditoriales()
        {
            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(
                        @"SELECT IdEditorial, Nombre
                          FROM dbo.Editoriales
                          ORDER BY Nombre", cn))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    cmbEditorial.DataSource = tabla;
                    cmbEditorial.DisplayMember = "Nombre";
                    cmbEditorial.ValueMember = "IdEditorial";
                    cmbEditorial.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MostrarError("cargar las editoriales", ex);
            }
        }

        private bool ValidarCampos(
            out int? anio,
            out int existencias)
        {
            anio = null;
            existencias = 0;

            if (string.IsNullOrWhiteSpace(txtISBN.Text) ||
                string.IsNullOrWhiteSpace(txtTitulo.Text) ||
                cmbAutor.SelectedIndex < 0 ||
                cmbEditorial.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Complete ISBN, título, autor y editorial.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (txtISBN.Text.Trim().Length > 20 ||
                txtTitulo.Text.Trim().Length > 200 ||
                txtCategoria.Text.Trim().Length > 100)
            {
                MessageBox.Show(
                    "ISBN, título o categoría supera la longitud permitida.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtAnio.Text))
            {
                if (!int.TryParse(txtAnio.Text.Trim(), out int valor)
                    || valor < 0 || valor > 9999)
                {
                    MessageBox.Show(
                        "Ingrese un año válido entre 0 y 9999.",
                        "Biblioteca",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }

                anio = valor;
            }

            if (!int.TryParse(txtExistencias.Text.Trim(),
                              out existencias) ||
                existencias < 0)
            {
                MessageBox.Show(
                    "Las existencias deben ser un número entero mayor o igual a cero.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private Libro CrearLibroDesdeFormulario(int? anio, int existencias)
        {
            return new Libro
            {
                ISBN = txtISBN.Text.Trim(),
                Titulo = txtTitulo.Text.Trim(),
                IdAutor = Convert.ToInt32(cmbAutor.SelectedValue),
                IdEditorial = Convert.ToInt32(cmbEditorial.SelectedValue),
                Categoria = string.IsNullOrWhiteSpace(txtCategoria.Text)
                    ? null : txtCategoria.Text.Trim(),
                Anio = anio,
                Existencias = existencias
            };
        }

        private void AgregarParametros(SqlCommand comando, Libro libro)
        {
            comando.Parameters.Add("@ISBN", SqlDbType.VarChar, 20).Value = libro.ISBN;
            comando.Parameters.Add("@Titulo", SqlDbType.VarChar, 200).Value = libro.Titulo;
            comando.Parameters.Add("@IdAutor", SqlDbType.Int).Value = libro.IdAutor;
            comando.Parameters.Add("@IdEditorial", SqlDbType.Int).Value = libro.IdEditorial;
            comando.Parameters.Add("@Categoria", SqlDbType.VarChar, 100).Value =
                (object?)libro.Categoria ?? DBNull.Value;
            comando.Parameters.Add("@Anio", SqlDbType.Int).Value =
                (object?)libro.Anio ?? DBNull.Value;
            comando.Parameters.Add("@Existencias", SqlDbType.Int).Value = libro.Existencias;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos(out int? anio,
                              out int existencias))
                return;

            try
            {
                Libro libro = CrearLibroDesdeFormulario(anio, existencias);
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        INSERT INTO dbo.Libros
                        (ISBN, Titulo, IdAutor, IdEditorial,
                         Categoria, Anio, Existencias)
                        VALUES
                        (@ISBN, @Titulo, @IdAutor, @IdEditorial,
                         @Categoria, @Anio, @Existencias)";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        AgregarParametros(comando, libro);

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Libro guardado correctamente.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarLibros();
                LimpiarCampos();
            }
            catch (SqlException ex)
                when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show(
                    "Ya existe un libro con ese ISBN.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarError("guardar el libro", ex);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (isbnSeleccionado == null)
            {
                MessageBox.Show(
                    "Primero haga doble clic sobre el libro que desea editar.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos(out int? anio,
                              out int existencias))
                return;

            if (txtISBN.Text.Trim() != isbnSeleccionado)
            {
                MessageBox.Show(
                    "No puede cambiar el ISBN durante la edición. " +
                    "Puede modificar los demás campos.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Libro libro = CrearLibroDesdeFormulario(anio, existencias);
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        UPDATE dbo.Libros
                        SET Titulo = @Titulo,
                            IdAutor = @IdAutor,
                            IdEditorial = @IdEditorial,
                            Categoria = @Categoria,
                            Anio = @Anio,
                            Existencias = @Existencias
                        WHERE ISBN = @ISBN";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        AgregarParametros(comando, libro);

                        int filas = comando.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            MessageBox.Show(
                                "No se encontró el libro.",
                                "Biblioteca");
                            return;
                        }
                    }
                }

                MessageBox.Show(
                    "Libro actualizado correctamente.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarLibros();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError("editar el libro", ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (isbnSeleccionado == null)
            {
                MessageBox.Show(
                    "Primero haga doble clic sobre el libro que desea eliminar.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar el libro seleccionado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        DELETE FROM dbo.Libros
                        WHERE ISBN = @ISBN";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        comando.Parameters.Add(
                            "@ISBN", SqlDbType.VarChar, 20
                        ).Value = isbnSeleccionado;

                        int filas = comando.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            MessageBox.Show(
                                "No se encontró el libro.",
                                "Biblioteca");
                            return;
                        }
                    }
                }

                MessageBox.Show(
                    "Libro eliminado correctamente.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarLibros();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "No se puede eliminar este libro porque tiene préstamos relacionados.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarError("eliminar el libro", ex);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string isbn = txtBuscarISBN.Text.Trim();

            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show(
                    "Ingrese un ISBN para buscar.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        SELECT ISBN, Titulo, IdAutor,
                               IdEditorial, Categoria,
                               Anio, Existencias
                        FROM dbo.Libros
                        WHERE ISBN = @ISBN";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        comando.Parameters.Add(
                            "@ISBN", SqlDbType.VarChar, 20
                        ).Value = isbn;

                        using (SqlDataReader lector =
                            comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                CargarLibroEnCampos(
                                    lector["ISBN"],
                                    lector["Titulo"],
                                    lector["IdAutor"],
                                    lector["IdEditorial"],
                                    lector["Categoria"],
                                    lector["Anio"],
                                    lector["Existencias"]);
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No se encontró un libro con ese ISBN.",
                                    "Biblioteca",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("buscar el libro", ex);
            }
        }

        private void dgvLibros_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow fila =
                dgvLibros.Rows[e.RowIndex];

            if (fila.IsNewRow)
                return;

            CargarLibroEnCampos(
                fila.Cells["ISBN"].Value,
                fila.Cells["Titulo"].Value,
                fila.Cells["IdAutor"].Value,
                fila.Cells["IdEditorial"].Value,
                fila.Cells["Categoria"].Value,
                fila.Cells["Anio"].Value,
                fila.Cells["Existencias"].Value);
        }

        private void CargarLibroEnCampos(
            object? isbn,
            object? titulo,
            object? idAutor,
            object? idEditorial,
            object? categoria,
            object? anio,
            object? existencias)
        {
            isbnSeleccionado = Convert.ToString(isbn)?.Trim();

            txtISBN.Text = isbnSeleccionado ?? "";
            txtTitulo.Text = Convert.ToString(titulo) ?? "";

            cmbAutor.SelectedValue =
                Convert.ToInt32(idAutor);

            cmbEditorial.SelectedValue =
                Convert.ToInt32(idEditorial);

            txtCategoria.Text =
                Convert.ToString(categoria) ?? "";

            txtAnio.Text =
                Convert.ToString(anio) ?? "";

            txtExistencias.Text =
                Convert.ToString(existencias) ?? "";

            txtISBN.ReadOnly = true;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            isbnSeleccionado = null;

            txtISBN.ReadOnly = false;
            txtISBN.Clear();
            txtTitulo.Clear();
            txtCategoria.Clear();
            txtAnio.Clear();
            txtExistencias.Clear();

            cmbAutor.SelectedIndex = -1;
            cmbEditorial.SelectedIndex = -1;

            dgvLibros.ClearSelection();
            dgvLibros.CurrentCell = null;

            txtISBN.Focus();
        }

        private void MostrarError(
            string operacion,
            Exception ex)
        {
            MessageBox.Show(
                "Error al " + operacion + ":\n" + ex.Message,
                "Biblioteca",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}

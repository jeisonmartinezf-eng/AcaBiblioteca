using AcaBiblioteca.Datos;
using AcaBiblioteca.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace AcaBiblioteca
{
    public partial class FrmAutores : Form
    {
        private int? idAutorSeleccionado = null;

        public FrmAutores()
        {
            InitializeComponent();
        }

        private void CargarAutores()
        {
            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                using (SqlDataAdapter adaptador = new SqlDataAdapter(
                    "SELECT IdAutor, Nombre, Apellido FROM dbo.Autores ORDER BY IdAutor DESC", cn))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dgvAutores.DataSource = tabla;
                    dgvAutores.ClearSelection();
                    dgvAutores.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MostrarError("cargar los autores", ex);
            }
        }

        private void FrmAutores_Load(object sender, EventArgs e)
        {
            CargarAutores();
        }

        private bool IntentarCrearAutor(out Autor autor)
        {
            autor = new Autor();
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Complete nombre y apellido.", "Biblioteca",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            autor = new Autor
            {
                IdAutor = idAutorSeleccionado ?? 0,
                Nombre = nombre,
                Apellido = apellido
            };
            return true;
        }

        private static void AgregarParametros(SqlCommand comando, Autor autor)
        {
            comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = autor.Nombre;
            comando.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = autor.Apellido;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!IntentarCrearAutor(out Autor autor)) return;

            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                {
                    cn.Open();
                    using (SqlCommand comando = new SqlCommand(
                        "INSERT INTO dbo.Autores (Nombre, Apellido) VALUES (@Nombre, @Apellido)", cn))
                    {
                        AgregarParametros(comando, autor);
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Autor guardado correctamente.", "Biblioteca",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarAutores();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError("guardar el autor", ex);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void dgvAutores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow fila = dgvAutores.Rows[e.RowIndex];
            if (fila.IsNewRow) return;

            Autor autor = new Autor
            {
                IdAutor = Convert.ToInt32(fila.Cells["IdAutor"].Value),
                Nombre = Convert.ToString(fila.Cells["Nombre"].Value) ?? string.Empty,
                Apellido = Convert.ToString(fila.Cells["Apellido"].Value) ?? string.Empty
            };

            idAutorSeleccionado = autor.IdAutor;
            txtNombre.Text = autor.Nombre;
            txtApellido.Text = autor.Apellido;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idAutorSeleccionado == null)
            {
                MessageBox.Show("Haga doble clic sobre el autor que desea editar.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IntentarCrearAutor(out Autor autor)) return;

            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                {
                    cn.Open();
                    using (SqlCommand comando = new SqlCommand(
                        "UPDATE dbo.Autores SET Nombre = @Nombre, Apellido = @Apellido WHERE IdAutor = @IdAutor", cn))
                    {
                        AgregarParametros(comando, autor);
                        comando.Parameters.Add("@IdAutor", SqlDbType.Int).Value = autor.IdAutor;
                        if (comando.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("No se encontró el autor seleccionado.", "Biblioteca",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CargarAutores();
                            LimpiarCampos();
                            return;
                        }
                    }
                }

                MessageBox.Show("Autor actualizado correctamente.", "Biblioteca",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarAutores();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError("actualizar el autor", ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idAutorSeleccionado == null)
            {
                MessageBox.Show("Haga doble clic sobre el autor que desea eliminar.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este autor?", "Biblioteca",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                {
                    cn.Open();
                    using (SqlCommand comando = new SqlCommand(
                        "DELETE FROM dbo.Autores WHERE IdAutor = @IdAutor", cn))
                    {
                        comando.Parameters.Add("@IdAutor", SqlDbType.Int).Value = idAutorSeleccionado.Value;
                        if (comando.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("No se encontró el autor seleccionado.", "Biblioteca",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CargarAutores();
                            LimpiarCampos();
                            return;
                        }
                    }
                }

                MessageBox.Show("Autor eliminado correctamente.", "Biblioteca",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarAutores();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("No puede eliminar este autor porque tiene libros relacionados.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarError("eliminar el autor", ex);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            idAutorSeleccionado = null;
            txtNombre.Clear();
            txtApellido.Clear();
            dgvAutores.ClearSelection();
            dgvAutores.CurrentCell = null;
            txtNombre.Focus();
        }

        private static void MostrarError(string operacion, Exception ex)
        {
            MessageBox.Show("Error al " + operacion + ":\n" + ex.Message,
                "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

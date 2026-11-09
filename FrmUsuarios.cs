
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using AcaBiblioteca.Datos;
using AcaBiblioteca.Modelos;

namespace AcaBiblioteca
{
    public partial class FrmUsuarios : Form
    {
        private int? idUsuarioSeleccionado = null;

        public FrmUsuarios()
        {
            InitializeComponent();

            ConectarBotones(this);

            dgvUsuarios.CellDoubleClick -= dgvUsuarios_CellDoubleClick;
            dgvUsuarios.CellDoubleClick += dgvUsuarios_CellDoubleClick;

            CargarUsuarios();
            LimpiarCampos();
        }

        // Busca los botones existentes en el diseñador
        // y conecta sus eventos automáticamente.
        private void ConectarBotones(Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (control is Button boton)
                {
                    string nombre = boton.Text
                        .Trim()
                        .ToLowerInvariant();

                    if (nombre.Contains("guardar"))
                    {
                        boton.Click -= btnGuardar_Click;
                        boton.Click += btnGuardar_Click;
                    }
                    else if (nombre.Contains("editar"))
                    {
                        boton.Click -= btnEditar_Click;
                        boton.Click += btnEditar_Click;
                    }
                    else if (nombre.Contains("eliminar"))
                    {
                        boton.Click -= btnEliminar_Click;
                        boton.Click += btnEliminar_Click;
                    }
                    else if (nombre.Contains("nuevo"))
                    {
                        boton.Click -= btnNuevo_Click;
                        boton.Click += btnNuevo_Click;
                    }
                    else if (nombre.Contains("cancelar"))
                    {
                        boton.Click -= btnCancelar_Click;
                        boton.Click += btnCancelar_Click;
                    }
                }

                if (control.HasChildren)
                {
                    ConectarBotones(control);
                }
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    string consulta = @"
                        SELECT
                            IdUsuario,
                            Nombre,
                            Apellido,
                            Documento,
                            Telefono,
                            Correo
                        FROM dbo.Usuarios
                        ORDER BY IdUsuario DESC";

                    using (SqlDataAdapter adaptador =
                        new SqlDataAdapter(consulta, cn))
                    {
                        DataTable tabla = new DataTable();
                        adaptador.Fill(tabla);

                        dgvUsuarios.DataSource = tabla;

                        dgvUsuarios.ClearSelection();
                        dgvUsuarios.CurrentCell = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show(
                    "Debe completar nombre, apellido y documento.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (txtNombre.Text.Trim().Length > 100 ||
                txtApellido.Text.Trim().Length > 100 ||
                txtDocumento.Text.Trim().Length > 30 ||
                txtTelefono.Text.Trim().Length > 30 ||
                txtCorreo.Text.Trim().Length > 150)
            {
                MessageBox.Show(
                    "Uno de los campos supera la longitud permitida.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        // Convierte los datos del formulario en un objeto del modelo Usuario.
        private Usuario ObtenerUsuarioDesdeCampos()
        {
            return new Usuario
            {
                IdUsuario = idUsuarioSeleccionado ?? 0,
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Documento = txtDocumento.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text)
                    ? null : txtTelefono.Text.Trim(),
                Correo = string.IsNullOrWhiteSpace(txtCorreo.Text)
                    ? null : txtCorreo.Text.Trim()
            };
        }

        // Los parametros SQL se obtienen del objeto, no directamente de los controles.
        private void AgregarParametros(SqlCommand comando, Usuario usuario)
        {
            comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
            comando.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = usuario.Apellido;
            comando.Parameters.Add("@Documento", SqlDbType.VarChar, 30).Value = usuario.Documento;
            comando.Parameters.Add("@Telefono", SqlDbType.VarChar, 30).Value =
                (object?)usuario.Telefono ?? DBNull.Value;
            comando.Parameters.Add("@Correo", SqlDbType.VarChar, 150).Value =
                (object?)usuario.Correo ?? DBNull.Value;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            Usuario usuario = ObtenerUsuarioDesdeCampos();

            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        INSERT INTO dbo.Usuarios
                        (
                            Nombre,
                            Apellido,
                            Documento,
                            Telefono,
                            Correo
                        )
                        VALUES
                        (
                            @Nombre,
                            @Apellido,
                            @Documento,
                            @Telefono,
                            @Correo
                        )";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        AgregarParametros(comando, usuario);
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Usuario guardado correctamente.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarUsuarios();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idUsuarioSeleccionado == null)
            {
                MessageBox.Show(
                    "Haga doble clic sobre el usuario que desea editar.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!ValidarCampos())
                return;

            Usuario usuario = ObtenerUsuarioDesdeCampos();

            try
            {
                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                {
                    cn.Open();

                    string consulta = @"
                        UPDATE dbo.Usuarios
                        SET
                            Nombre = @Nombre,
                            Apellido = @Apellido,
                            Documento = @Documento,
                            Telefono = @Telefono,
                            Correo = @Correo
                        WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        AgregarParametros(comando, usuario);

                        comando.Parameters.Add(
                            "@IdUsuario", SqlDbType.Int
                        ).Value = usuario.IdUsuario;

                        int filas = comando.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            MessageBox.Show(
                                "No se encontró el usuario seleccionado.",
                                "Biblioteca");

                            return;
                        }
                    }
                }

                MessageBox.Show(
                    "Usuario actualizado correctamente.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarUsuarios();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idUsuarioSeleccionado == null)
            {
                MessageBox.Show(
                    "Haga doble clic sobre el usuario que desea eliminar.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este usuario?",
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
                        DELETE FROM dbo.Usuarios
                        WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, cn))
                    {
                        comando.Parameters.Add(
                            "@IdUsuario", SqlDbType.Int
                        ).Value = idUsuarioSeleccionado.Value;

                        int filas = comando.ExecuteNonQuery();

                        if (filas == 0)
                        {
                            MessageBox.Show(
                                "No se encontró el usuario seleccionado.",
                                "Biblioteca");

                            return;
                        }
                    }
                }

                MessageBox.Show(
                    "Usuario eliminado correctamente.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarUsuarios();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "No puede eliminar este usuario porque tiene préstamos relacionados.",
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void dgvUsuarios_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow fila =
                dgvUsuarios.Rows[e.RowIndex];

            if (fila.IsNewRow)
                return;

            idUsuarioSeleccionado =
                Convert.ToInt32(
                    fila.Cells["IdUsuario"].Value);

            txtNombre.Text =
                Convert.ToString(
                    fila.Cells["Nombre"].Value) ?? "";

            txtApellido.Text =
                Convert.ToString(
                    fila.Cells["Apellido"].Value) ?? "";

            txtDocumento.Text =
                Convert.ToString(
                    fila.Cells["Documento"].Value) ?? "";

            txtTelefono.Text =
                Convert.ToString(
                    fila.Cells["Telefono"].Value) ?? "";

            txtCorreo.Text =
                Convert.ToString(
                    fila.Cells["Correo"].Value) ?? "";
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
            idUsuarioSeleccionado = null;

            txtNombre.Clear();
            txtApellido.Clear();
            txtDocumento.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();

            dgvUsuarios.ClearSelection();
            dgvUsuarios.CurrentCell = null;

            txtNombre.Focus();
        }

        private void MostrarError(Exception ex)
        {
            MessageBox.Show(
                "Ocurrió un error:\n" + ex.Message,
                "Biblioteca",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
        }
    }
}

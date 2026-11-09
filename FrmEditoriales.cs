using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using AcaBiblioteca.Datos;
using AcaBiblioteca.Modelos;

namespace AcaBiblioteca
{
    public partial class FrmEditoriales : Form
    {
        private int? idEditorialSeleccionada = null;

        public FrmEditoriales()
        {
            InitializeComponent();
            dgvEditoriales.CellDoubleClick -= dgvEditoriales_CellDoubleClick;
            dgvEditoriales.CellDoubleClick += dgvEditoriales_CellDoubleClick;
        }

        private void CargarEditoriales()
        {
            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                using (SqlDataAdapter adaptador = new SqlDataAdapter(
                    "SELECT IdEditorial, Nombre FROM dbo.Editoriales ORDER BY IdEditorial DESC", cn))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dgvEditoriales.DataSource = tabla;
                    dgvEditoriales.ClearSelection();
                    dgvEditoriales.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MostrarError("cargar las editoriales", ex);
            }
        }

        private void FrmEditoriales_Load(object sender, EventArgs e)
        {
            CargarEditoriales();
            LimpiarCampos();
        }

        private Editorial? ObtenerEditorialDelFormulario()
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre de la editorial.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new Editorial
            {
                IdEditorial = idEditorialSeleccionada ?? 0,
                Nombre = nombre
            };
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Editorial? editorial = ObtenerEditorialDelFormulario();
            if (editorial == null) return;

            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                {
                    cn.Open();
                    const string consulta =
                        "INSERT INTO dbo.Editoriales (Nombre) VALUES (@Nombre)";
                    using (SqlCommand comando = new SqlCommand(consulta, cn))
                    {
                        comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 200)
                            .Value = editorial.Nombre;
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Editorial guardada correctamente.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarEditoriales();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError("guardar la editorial", ex);
            }
        }

        private void dgvEditoriales_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow fila = dgvEditoriales.Rows[e.RowIndex];
            if (fila.IsNewRow) return;

            idEditorialSeleccionada = Convert.ToInt32(fila.Cells["IdEditorial"].Value);
            txtNombre.Text = Convert.ToString(fila.Cells["Nombre"].Value) ?? "";
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idEditorialSeleccionada == null)
            {
                MessageBox.Show("Haga doble clic sobre la editorial que desea editar.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Editorial? editorial = ObtenerEditorialDelFormulario();
            if (editorial == null) return;

            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                {
                    cn.Open();
                    const string consulta =
                        "UPDATE dbo.Editoriales SET Nombre = @Nombre WHERE IdEditorial = @IdEditorial";
                    using (SqlCommand comando = new SqlCommand(consulta, cn))
                    {
                        comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 200)
                            .Value = editorial.Nombre;
                        comando.Parameters.Add("@IdEditorial", SqlDbType.Int)
                            .Value = editorial.IdEditorial;
                        if (comando.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("No se encontró la editorial seleccionada.", "Biblioteca");
                            return;
                        }
                    }
                }

                MessageBox.Show("Editorial actualizada correctamente.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarEditoriales();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarError("actualizar la editorial", ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idEditorialSeleccionada == null)
            {
                MessageBox.Show("Haga doble clic sobre la editorial que desea eliminar.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar esta editorial?",
                    "Biblioteca", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                using (SqlConnection cn = new Conexion().ObtenerConexion())
                {
                    cn.Open();
                    const string consulta =
                        "DELETE FROM dbo.Editoriales WHERE IdEditorial = @IdEditorial";
                    using (SqlCommand comando = new SqlCommand(consulta, cn))
                    {
                        comando.Parameters.Add("@IdEditorial", SqlDbType.Int)
                            .Value = idEditorialSeleccionada.Value;
                        if (comando.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("No se encontró la editorial seleccionada.", "Biblioteca");
                            return;
                        }
                    }
                }

                MessageBox.Show("Editorial eliminada correctamente.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarEditoriales();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show("No se puede eliminar esta editorial porque tiene libros relacionados.",
                    "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MostrarError("eliminar la editorial", ex);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e) => LimpiarCampos();

        private void btnCancelar_Click(object sender, EventArgs e) => LimpiarCampos();

        private void LimpiarCampos()
        {
            idEditorialSeleccionada = null;
            txtNombre.Clear();
            dgvEditoriales.ClearSelection();
            dgvEditoriales.CurrentCell = null;
            txtNombre.Focus();
        }

        private void MostrarError(string operacion, Exception ex)
        {
            MessageBox.Show("Error al " + operacion + ":\n" + ex.Message,
                "Biblioteca", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

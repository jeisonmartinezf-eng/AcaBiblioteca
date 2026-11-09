
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using AcaBiblioteca.Datos;

namespace AcaBiblioteca
{
    public partial class FrmReportes : Form
    {
        private ComboBox cmbReporte = null!;
        private DataGridView dgvReportes = null!;
        private Label lblTotal = null!;
        private Button btnConsultar = null!;
        private Button btnActualizar = null!;

        public FrmReportes()
        {
            InitializeComponent();
            CrearInterfaz();
            CargarOpciones();
            ConsultarReporte();
        }

        private void CrearInterfaz()
        {
            Text = "Reportes - Biblioteca";
            Size = new Size(1000, 650);
            MinimumSize = new Size(850, 500);
            StartPosition = FormStartPosition.CenterScreen;

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            Controls.Add(panel);

            Label titulo = new Label
            {
                Text = "REPORTES DE LA BIBLIOTECA",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };
            panel.Controls.Add(titulo);

            Label etiqueta = new Label
            {
                Text = "Seleccione el reporte:",
                Location = new Point(30, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 11)
            };
            panel.Controls.Add(etiqueta);

            cmbReporte = new ComboBox
            {
                Location = new Point(30, 125),
                Size = new Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panel.Controls.Add(cmbReporte);

            btnConsultar = new Button
            {
                Text = "Consultar",
                Location = new Point(400, 120),
                Size = new Size(140, 40),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(btnConsultar);

            btnActualizar = new Button
            {
                Text = "Actualizar",
                Location = new Point(560, 120),
                Size = new Size(140, 40),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(btnActualizar);

            Label listado = new Label
            {
                Text = "RESULTADOS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(30, 195),
                AutoSize = true
            };
            panel.Controls.Add(listado);

            dgvReportes = new DataGridView
            {
                Location = new Point(30, 235),
                Size = new Size(890, 280),
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill
            };
            panel.Controls.Add(dgvReportes);

            lblTotal = new Label
            {
                Text = "Total de registros: 0",
                Location = new Point(30, 535),
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            panel.Controls.Add(lblTotal);

            btnConsultar.Click += (s, e) => ConsultarReporte();
            btnActualizar.Click += (s, e) => ConsultarReporte();
        }

        private void CargarOpciones()
        {
            cmbReporte.Items.Add("Libros registrados");
            cmbReporte.Items.Add("Autores registrados");
            cmbReporte.Items.Add("Editoriales registradas");
            cmbReporte.Items.Add("Usuarios registrados");
            cmbReporte.Items.Add("Todos los préstamos");
            cmbReporte.Items.Add("Préstamos activos");
            cmbReporte.Items.Add("Préstamos devueltos");

            cmbReporte.SelectedIndex = 0;
        }

        private void ConsultarReporte()
        {
            if (cmbReporte.SelectedIndex < 0)
                return;

            try
            {
                string consulta = ObtenerConsulta();

                using (SqlConnection cn =
                    new Conexion().ObtenerConexion())
                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(consulta, cn))
                {
                    DataTable tabla = new DataTable();

                    adaptador.Fill(tabla);

                    dgvReportes.DataSource = tabla;

                    lblTotal.Text =
                        "Total de registros: " +
                        tabla.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el reporte:\n" + ex.Message,
                    "Biblioteca",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string ObtenerConsulta()
        {
            switch (cmbReporte.SelectedIndex)
            {
                case 0:
                    return @"
                        SELECT ISBN, Titulo, Categoria,
                               Anio, Existencias
                        FROM dbo.Libros
                        ORDER BY Titulo";

                case 1:
                    return @"
                        SELECT *
                        FROM dbo.Autores";

                case 2:
                    return @"
                        SELECT *
                        FROM dbo.Editoriales";

                case 3:
                    return @"
                        SELECT IdUsuario, Nombre, Apellido,
                               Documento, Telefono, Correo
                        FROM dbo.Usuarios
                        ORDER BY Nombre, Apellido";

                case 4:
                    return ConsultaPrestamos("");

                case 5:
                    return ConsultaPrestamos(
                        "WHERE p.Estado = 'Prestado'");

                case 6:
                    return ConsultaPrestamos(
                        "WHERE p.Estado = 'Devuelto'");

                default:
                    return @"
                        SELECT ISBN, Titulo
                        FROM dbo.Libros";
            }
        }

        private string ConsultaPrestamos(string filtro)
        {
            return @"
                SELECT
                    p.IdPrestamo,
                    CONCAT(u.Nombre, ' ', u.Apellido) AS Usuario,
                    l.Titulo AS Libro,
                    p.FechaPrestamo,
                    p.FechaDevolucion,
                    p.Estado
                FROM dbo.Prestamos p
                INNER JOIN dbo.Usuarios u
                    ON p.IdUsuario = u.IdUsuario
                INNER JOIN dbo.Libros l
                    ON p.ISBN = l.ISBN
                " + filtro + @"
                ORDER BY p.IdPrestamo DESC";
        }
    }
}

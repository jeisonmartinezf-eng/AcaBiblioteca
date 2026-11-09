using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using AcaBiblioteca.Datos;
using ModeloPrestamo = AcaBiblioteca.Modelos.Prestamo;

namespace AcaBiblioteca
{
    public partial class FrmPrestamos : Form
    {
        private ComboBox cmbUsuario;
        private ComboBox cmbLibro;
        private DateTimePicker dtpFechaPrestamo;
        private DateTimePicker dtpFechaDevolucion;
        private DataGridView dgvPrestamos;
        private Button btnGuardar;
        private Button btnDevolver;
        private Button btnActualizar;
        private Button btnEliminar;

        private int idSeleccionado = 0;
        private bool cargando = false;

        public FrmPrestamos()
        {
            InitializeComponent();
            CrearInterfaz();
            ConectarEventos();
            CargarTodo();
        }

        private SqlConnection NuevaConexion()
        {
            return new Conexion().ObtenerConexion();
        }

        private void CrearInterfaz()
        {
            Text = "Gestión de préstamos - Biblioteca";
            Size = new Size(1000, 650);
            MinimumSize = new Size(850, 550);
            StartPosition = FormStartPosition.CenterScreen;

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };
            Controls.Add(panel);

            Label titulo = new Label
            {
                Text = "GESTIÓN DE PRÉSTAMOS",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(25, 20),
                AutoSize = true
            };
            panel.Controls.Add(titulo);

            AgregarEtiqueta(panel, "Usuario:", 30, 90);
            cmbUsuario = CrearCombo(panel, 190, 85);

            AgregarEtiqueta(panel, "Libro:", 30, 140);
            cmbLibro = CrearCombo(panel, 190, 135);

            AgregarEtiqueta(panel, "Fecha préstamo:", 30, 190);
            dtpFechaPrestamo = CrearFecha(panel, 190, 185);

            AgregarEtiqueta(panel, "Fecha devolución:", 30, 240);
            dtpFechaDevolucion = CrearFecha(panel, 190, 235);
            dtpFechaDevolucion.ShowCheckBox = true;
            dtpFechaDevolucion.Checked = false;

            btnGuardar = CrearBoton(panel, "Guardar", 30, 305);
            btnDevolver = CrearBoton(panel, "Devolver", 190, 305);
            btnActualizar = CrearBoton(panel, "Actualizar", 350, 305);
            btnEliminar = CrearBoton(panel, "Eliminar", 510, 305);

            Label listado = new Label
            {
                Text = "LISTADO DE PRÉSTAMOS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(30, 375),
                AutoSize = true
            };
            panel.Controls.Add(listado);

            dgvPrestamos = new DataGridView
            {
                Location = new Point(30, 415),
                Size = new Size(890, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left |
                         AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            panel.Controls.Add(dgvPrestamos);
        }

        private void AgregarEtiqueta(
            Control padre, string texto, int x, int y)
        {
            Label etiqueta = new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10)
            };
            padre.Controls.Add(etiqueta);
        }

        private ComboBox CrearCombo(Control padre, int x, int y)
        {
            ComboBox combo = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(300, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            padre.Controls.Add(combo);
            return combo;
        }

        private DateTimePicker CrearFecha(
            Control padre, int x, int y)
        {
            DateTimePicker fecha = new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(300, 30),
                Format = DateTimePickerFormat.Short
            };
            padre.Controls.Add(fecha);
            return fecha;
        }

        private Button CrearBoton(
            Control padre, string texto, int x, int y)
        {
            Button boton = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(140, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            padre.Controls.Add(boton);
            return boton;
        }

        private void ConectarEventos()
        {
            btnGuardar.Click += (s, e) => Guardar();
            btnDevolver.Click += (s, e) => Devolver();
            btnActualizar.Click += (s, e) => Actualizar();
            btnEliminar.Click += (s, e) => Eliminar();

            dgvPrestamos.SelectionChanged +=
                (s, e) => SeleccionarPrestamo();
        }

        private void EjecutarSeguro(Action accion)
        {
            try
            {
                accion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarTodo()
        {
            EjecutarSeguro(() =>
            {
                cargando = true;
                try
                {
                    CargarUsuarios();
                    CargarLibros();
                    CargarPrestamos();
                    Limpiar();
                }
                finally
                {
                    cargando = false;
                }
            });
        }

        private void CargarUsuarios()
        {
            using (SqlConnection cn = NuevaConexion())
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT IdUsuario,
                         CONCAT(Nombre, ' ', Apellido) AS NombreCompleto
                  FROM dbo.Usuarios
                  ORDER BY Nombre, Apellido", cn))
            {
                DataTable tabla = new DataTable();
                da.Fill(tabla);
                cmbUsuario.DataSource = tabla;
                cmbUsuario.DisplayMember = "NombreCompleto";
                cmbUsuario.ValueMember = "IdUsuario";
            }
        }

        private void CargarLibros()
        {
            using (SqlConnection cn = NuevaConexion())
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT ISBN,
                         CONCAT(Titulo, ' (Disponibles: ',
                                Existencias, ')') AS Descripcion
                  FROM dbo.Libros
                  ORDER BY Titulo", cn))
            {
                DataTable tabla = new DataTable();
                da.Fill(tabla);
                cmbLibro.DataSource = tabla;
                cmbLibro.DisplayMember = "Descripcion";
                cmbLibro.ValueMember = "ISBN";
            }
        }

        private void CargarPrestamos()
        {
            using (SqlConnection cn = NuevaConexion())
            using (SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT p.IdPrestamo, p.IdUsuario, p.ISBN,
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
                  ORDER BY p.IdPrestamo DESC", cn))
            {
                DataTable tabla = new DataTable();
                da.Fill(tabla);
                dgvPrestamos.DataSource = tabla;

                if (dgvPrestamos.Columns.Contains("IdUsuario"))
                    dgvPrestamos.Columns["IdUsuario"].Visible = false;

                if (dgvPrestamos.Columns.Contains("ISBN"))
                    dgvPrestamos.Columns["ISBN"].Visible = false;

                dgvPrestamos.ClearSelection();
            }
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            cmbUsuario.SelectedIndex = -1;
            cmbLibro.SelectedIndex = -1;
            dtpFechaPrestamo.Value = DateTime.Today;
            dtpFechaDevolucion.Value = DateTime.Today;
            dtpFechaDevolucion.Checked = false;
            dgvPrestamos.ClearSelection();
        }

        private bool ValidarDatos()
        {
            if (cmbUsuario.SelectedValue == null ||
                cmbLibro.SelectedValue == null ||
                cmbUsuario.SelectedIndex < 0 ||
                cmbLibro.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un usuario y un libro.");
                return false;
            }

            if (dtpFechaPrestamo.Value.Date > DateTime.Today)
            {
                MessageBox.Show(
                    "La fecha del préstamo no puede ser futura.");
                return false;
            }

            if (dtpFechaDevolucion.Checked &&
                dtpFechaDevolucion.Value.Date <
                dtpFechaPrestamo.Value.Date)
            {
                MessageBox.Show(
                    "La devolución no puede ser anterior al préstamo.");
                return false;
            }

            return true;
        }

        // Construye un objeto del modelo sin alterar las transacciones SQL.
        private ModeloPrestamo LeerPrestamoDelFormulario()
        {
            return new ModeloPrestamo
            {
                IdPrestamo = idSeleccionado,
                IdUsuario = Convert.ToInt32(cmbUsuario.SelectedValue),
                ISBN = Convert.ToString(cmbLibro.SelectedValue)?.Trim() ?? string.Empty,
                FechaPrestamo = dtpFechaPrestamo.Value.Date,
                FechaDevolucion = dtpFechaDevolucion.Checked
                    ? dtpFechaDevolucion.Value.Date
                    : (DateTime?)null,
                Estado = "Prestado"
            };
        }

        private void Guardar()
        {
            EjecutarSeguro(() =>
            {
                if (!ValidarDatos()) return;

                if (dtpFechaDevolucion.Checked)
                {
                    MessageBox.Show(
                        "Para registrar un préstamo nuevo, deje " +
                        "sin marcar la fecha de devolución.");
                    return;
                }

                ModeloPrestamo prestamo = LeerPrestamoDelFormulario();

                using (SqlConnection cn = NuevaConexion())
                {
                    cn.Open();

                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE dbo.Libros
                                  SET Existencias = Existencias - 1
                                  WHERE ISBN = @isbn
                                    AND Existencias > 0", cn, tx))
                            {
                                cmd.Parameters.Add("@isbn",
                                    SqlDbType.VarChar, 20).Value = prestamo.ISBN;

                                if (cmd.ExecuteNonQuery() != 1)
                                {
                                    throw new InvalidOperationException(
                                        "No hay existencias disponibles " +
                                        "para este libro.");
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"INSERT INTO dbo.Prestamos
                                  (IdUsuario, ISBN, FechaPrestamo,
                                   FechaDevolucion, Estado)
                                  VALUES
                                  (@usuario, @isbn, @fecha,
                                   NULL, 'Prestado')", cn, tx))
                            {
                                cmd.Parameters.Add("@usuario",
                                    SqlDbType.Int).Value = prestamo.IdUsuario;
                                cmd.Parameters.Add("@isbn",
                                    SqlDbType.VarChar, 20).Value = prestamo.ISBN;
                                cmd.Parameters.Add("@fecha",
                                    SqlDbType.Date).Value =
                                    prestamo.FechaPrestamo;

                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show("Préstamo registrado correctamente.");
                CargarTodo();
            });
        }

        private void SeleccionarPrestamo()
        {
            if (cargando ||
                dgvPrestamos.SelectedRows.Count == 0)
                return;

            DataGridViewRow fila = dgvPrestamos.SelectedRows[0];

            if (fila.Cells["IdPrestamo"].Value == null)
                return;

            idSeleccionado =
                Convert.ToInt32(fila.Cells["IdPrestamo"].Value);

            cmbUsuario.SelectedValue =
                fila.Cells["IdUsuario"].Value;

            cmbLibro.SelectedValue =
                fila.Cells["ISBN"].Value.ToString();

            dtpFechaPrestamo.Value =
                Convert.ToDateTime(fila.Cells["FechaPrestamo"].Value);

            object devolucion =
                fila.Cells["FechaDevolucion"].Value;

            if (devolucion == null || devolucion == DBNull.Value)
            {
                dtpFechaDevolucion.Value = DateTime.Today;
                dtpFechaDevolucion.Checked = false;
            }
            else
            {
                dtpFechaDevolucion.Value =
                    Convert.ToDateTime(devolucion);
                dtpFechaDevolucion.Checked = true;
            }
        }

        private void Devolver()
        {
            EjecutarSeguro(() =>
            {
                if (idSeleccionado == 0)
                {
                    MessageBox.Show(
                        "Seleccione un préstamo de la tabla.");
                    return;
                }

                DateTime fecha = dtpFechaDevolucion.Checked
                    ? dtpFechaDevolucion.Value.Date
                    : DateTime.Today;

                using (SqlConnection cn = NuevaConexion())
                {
                    cn.Open();

                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            string isbn;
                            DateTime fechaPrestamo;

                            using (SqlCommand cmd = new SqlCommand(
                                @"SELECT ISBN, FechaPrestamo
                                  FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
                                  WHERE IdPrestamo = @id
                                    AND Estado = 'Prestado'", cn, tx))
                            {
                                cmd.Parameters.Add("@id",
                                    SqlDbType.Int).Value = idSeleccionado;

                                using (SqlDataReader lector =
                                    cmd.ExecuteReader())
                                {
                                    if (!lector.Read())
                                    {
                                        throw new InvalidOperationException(
                                            "El préstamo no existe " +
                                            "o ya fue devuelto.");
                                    }

                                    isbn = lector.GetString(0);
                                    fechaPrestamo = lector.GetDateTime(1);
                                }
                            }

                            if (fecha < fechaPrestamo.Date ||
                                fecha > DateTime.Today)
                            {
                                throw new InvalidOperationException(
                                    "La fecha de devolución debe estar " +
                                    "entre la fecha del préstamo y hoy.");
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE dbo.Prestamos
                                  SET FechaDevolucion = @fecha,
                                      Estado = 'Devuelto'
                                  WHERE IdPrestamo = @id
                                    AND Estado = 'Prestado'", cn, tx))
                            {
                                cmd.Parameters.Add("@fecha",
                                    SqlDbType.Date).Value = fecha;
                                cmd.Parameters.Add("@id",
                                    SqlDbType.Int).Value = idSeleccionado;

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "No se pudo registrar la devolución.");
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE dbo.Libros
                                  SET Existencias = Existencias + 1
                                  WHERE ISBN = @isbn", cn, tx))
                            {
                                cmd.Parameters.Add("@isbn",
                                    SqlDbType.VarChar, 20).Value = isbn;

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "No se pudo actualizar el libro.");
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show("Libro devuelto correctamente.");
                CargarTodo();
            });
        }

        private void Actualizar()
        {
            EjecutarSeguro(() =>
            {
                if (idSeleccionado == 0)
                {
                    MessageBox.Show(
                        "Seleccione un préstamo para actualizar.");
                    return;
                }

                if (!ValidarDatos()) return;

                ModeloPrestamo prestamo = LeerPrestamoDelFormulario();
                string isbnNuevo = prestamo.ISBN;

                using (SqlConnection cn = NuevaConexion())
                {
                    cn.Open();

                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            string isbnAnterior;
                            string estado;

                            using (SqlCommand cmd = new SqlCommand(
                                @"SELECT ISBN, Estado
                                  FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
                                  WHERE IdPrestamo = @id", cn, tx))
                            {
                                cmd.Parameters.Add("@id",
                                    SqlDbType.Int).Value = idSeleccionado;

                                using (SqlDataReader lector =
                                    cmd.ExecuteReader())
                                {
                                    if (!lector.Read())
                                        throw new Exception(
                                            "El préstamo ya no existe.");

                                    isbnAnterior = lector.GetString(0);
                                    estado = lector.GetString(1);
                                }
                            }

                            if (estado != "Prestado")
                                throw new InvalidOperationException(
                                    "Solo se pueden actualizar " +
                                    "préstamos activos.");

                            if (dtpFechaDevolucion.Checked)
                                throw new InvalidOperationException(
                                    "Utilice el botón Devolver " +
                                    "para registrar la devolución.");

                            if (isbnAnterior.Trim() != isbnNuevo.Trim())
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    @"UPDATE dbo.Libros
                                      SET Existencias = Existencias - 1
                                      WHERE ISBN = @isbn
                                        AND Existencias > 0", cn, tx))
                                {
                                    cmd.Parameters.Add("@isbn",
                                        SqlDbType.VarChar, 20).Value =
                                        isbnNuevo;

                                    if (cmd.ExecuteNonQuery() != 1)
                                        throw new Exception(
                                            "El nuevo libro no tiene " +
                                            "existencias disponibles.");
                                }

                                using (SqlCommand cmd = new SqlCommand(
                                    @"UPDATE dbo.Libros
                                      SET Existencias = Existencias + 1
                                      WHERE ISBN = @isbn", cn, tx))
                                {
                                    cmd.Parameters.Add("@isbn",
                                        SqlDbType.VarChar, 20).Value =
                                        isbnAnterior;

                                    if (cmd.ExecuteNonQuery() != 1)
                                        throw new Exception(
                                            "No se pudo devolver " +
                                            "el ejemplar anterior.");
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"UPDATE dbo.Prestamos
                                  SET IdUsuario = @usuario,
                                      ISBN = @isbn,
                                      FechaPrestamo = @fecha
                                  WHERE IdPrestamo = @id", cn, tx))
                            {
                                cmd.Parameters.Add("@usuario",
                                    SqlDbType.Int).Value = prestamo.IdUsuario;
                                cmd.Parameters.Add("@isbn",
                                    SqlDbType.VarChar, 20).Value = isbnNuevo;
                                cmd.Parameters.Add("@fecha",
                                    SqlDbType.Date).Value =
                                    prestamo.FechaPrestamo;
                                cmd.Parameters.Add("@id",
                                    SqlDbType.Int).Value = idSeleccionado;

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "No se pudo actualizar el préstamo.");
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show("Préstamo actualizado correctamente.");
                CargarTodo();
            });
        }

        private void Eliminar()
        {
            EjecutarSeguro(() =>
            {
                if (idSeleccionado == 0)
                {
                    MessageBox.Show(
                        "Seleccione un préstamo para eliminar.");
                    return;
                }

                if (MessageBox.Show(
                    "¿Está seguro de eliminar este préstamo?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                using (SqlConnection cn = NuevaConexion())
                {
                    cn.Open();

                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            string isbn;
                            string estado;

                            using (SqlCommand cmd = new SqlCommand(
                                @"SELECT ISBN, Estado
                                  FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
                                  WHERE IdPrestamo = @id", cn, tx))
                            {
                                cmd.Parameters.Add("@id",
                                    SqlDbType.Int).Value = idSeleccionado;

                                using (SqlDataReader lector =
                                    cmd.ExecuteReader())
                                {
                                    if (!lector.Read())
                                        throw new Exception(
                                            "El préstamo ya no existe.");

                                    isbn = lector.GetString(0);
                                    estado = lector.GetString(1);
                                }
                            }

                            if (estado != "Prestado" &&
                                estado != "Devuelto")
                                throw new InvalidOperationException(
                                    "Estado de préstamo no reconocido.");

                            if (estado == "Prestado")
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    @"UPDATE dbo.Libros
                                      SET Existencias = Existencias + 1
                                      WHERE ISBN = @isbn", cn, tx))
                                {
                                    cmd.Parameters.Add("@isbn",
                                        SqlDbType.VarChar, 20).Value = isbn;

                                    if (cmd.ExecuteNonQuery() != 1)
                                        throw new Exception(
                                            "No se pudo restituir " +
                                            "la existencia del libro.");
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                @"DELETE FROM dbo.Prestamos
                                  WHERE IdPrestamo = @id", cn, tx))
                            {
                                cmd.Parameters.Add("@id",
                                    SqlDbType.Int).Value = idSeleccionado;

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "No se pudo eliminar el préstamo.");
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show("Préstamo eliminado correctamente.");
                CargarTodo();
            });
        }
    }
}

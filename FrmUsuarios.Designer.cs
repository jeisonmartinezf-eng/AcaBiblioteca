namespace AcaBiblioteca
{
    partial class FrmUsuarios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlTitulo = new Panel();
            lblTitulo = new Label();
            pnlDatos = new Panel();
            iconButton4 = new FontAwesome.Sharp.IconButton();
            iconButton3 = new FontAwesome.Sharp.IconButton();
            iconButton2 = new FontAwesome.Sharp.IconButton();
            iconButton1 = new FontAwesome.Sharp.IconButton();
            btnNuevo = new FontAwesome.Sharp.IconButton();
            txtCorreo = new TextBox();
            txtTelefono = new TextBox();
            txtDocumento = new TextBox();
            txtApellido = new TextBox();
            txtNombre = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            lblNombre = new Label();
            dgvUsuarios = new DataGridView();
            panel1 = new Panel();
            pnlTitulo.SuspendLayout();
            pnlDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTitulo
            // 
            pnlTitulo.BackColor = Color.RoyalBlue;
            pnlTitulo.Controls.Add(lblTitulo);
            pnlTitulo.Dock = DockStyle.Top;
            pnlTitulo.Location = new Point(0, 0);
            pnlTitulo.Name = "pnlTitulo";
            pnlTitulo.Size = new Size(1401, 70);
            pnlTitulo.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(507, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(416, 48);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "GESTIÓN DE USUARIOS";
            // 
            // pnlDatos
            // 
            pnlDatos.Controls.Add(panel1);
            pnlDatos.Controls.Add(txtCorreo);
            pnlDatos.Controls.Add(txtTelefono);
            pnlDatos.Controls.Add(txtDocumento);
            pnlDatos.Controls.Add(txtApellido);
            pnlDatos.Controls.Add(txtNombre);
            pnlDatos.Controls.Add(label4);
            pnlDatos.Controls.Add(label3);
            pnlDatos.Controls.Add(label2);
            pnlDatos.Controls.Add(label1);
            pnlDatos.Controls.Add(lblNombre);
            pnlDatos.Dock = DockStyle.Top;
            pnlDatos.Location = new Point(0, 70);
            pnlDatos.Name = "pnlDatos";
            pnlDatos.Size = new Size(1401, 417);
            pnlDatos.TabIndex = 1;
            // 
            // iconButton4
            // 
            iconButton4.BackColor = Color.Transparent;
            iconButton4.Font = new Font("Segoe Fluent Icons", 12F);
            iconButton4.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            iconButton4.IconColor = Color.Black;
            iconButton4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton4.IconSize = 25;
            iconButton4.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton4.Location = new Point(695, 9);
            iconButton4.Name = "iconButton4";
            iconButton4.Size = new Size(146, 42);
            iconButton4.TabIndex = 14;
            iconButton4.Text = "Cancelar";
            iconButton4.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButton4.UseVisualStyleBackColor = false;
            // 
            // iconButton3
            // 
            iconButton3.BackColor = Color.Transparent;
            iconButton3.Font = new Font("Segoe Fluent Icons", 12F);
            iconButton3.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            iconButton3.IconColor = Color.Black;
            iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton3.IconSize = 25;
            iconButton3.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton3.Location = new Point(537, 9);
            iconButton3.Name = "iconButton3";
            iconButton3.Size = new Size(146, 42);
            iconButton3.TabIndex = 13;
            iconButton3.Text = "Eliminar";
            iconButton3.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButton3.UseVisualStyleBackColor = false;
            // 
            // iconButton2
            // 
            iconButton2.BackColor = Color.Transparent;
            iconButton2.Font = new Font("Segoe Fluent Icons", 12F);
            iconButton2.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            iconButton2.IconColor = Color.Black;
            iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton2.IconSize = 25;
            iconButton2.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton2.Location = new Point(368, 9);
            iconButton2.Name = "iconButton2";
            iconButton2.Size = new Size(146, 42);
            iconButton2.TabIndex = 12;
            iconButton2.Text = "Editar";
            iconButton2.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButton2.UseVisualStyleBackColor = false;
            // 
            // iconButton1
            // 
            iconButton1.BackColor = Color.Transparent;
            iconButton1.Font = new Font("Segoe Fluent Icons", 12F);
            iconButton1.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            iconButton1.IconColor = Color.Black;
            iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButton1.IconSize = 25;
            iconButton1.ImageAlign = ContentAlignment.MiddleLeft;
            iconButton1.Location = new Point(200, 9);
            iconButton1.Name = "iconButton1";
            iconButton1.Size = new Size(146, 42);
            iconButton1.TabIndex = 11;
            iconButton1.Text = "Guardar";
            iconButton1.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButton1.UseVisualStyleBackColor = false;
            // 
            // btnNuevo
            // 
            btnNuevo.BackColor = Color.Transparent;
            btnNuevo.Font = new Font("Segoe Fluent Icons", 12F);
            btnNuevo.IconChar = FontAwesome.Sharp.IconChar.Newspaper;
            btnNuevo.IconColor = Color.Black;
            btnNuevo.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnNuevo.IconSize = 25;
            btnNuevo.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevo.Location = new Point(27, 9);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(146, 42);
            btnNuevo.TabIndex = 10;
            btnNuevo.Text = "Nuevo";
            btnNuevo.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNuevo.UseVisualStyleBackColor = false;
            // 
            // txtCorreo
            // 
            txtCorreo.Location = new Point(663, 272);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Size = new Size(342, 31);
            txtCorreo.TabIndex = 9;
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(663, 218);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(342, 31);
            txtTelefono.TabIndex = 8;
            // 
            // txtDocumento
            // 
            txtDocumento.Location = new Point(663, 164);
            txtDocumento.Name = "txtDocumento";
            txtDocumento.Size = new Size(342, 31);
            txtDocumento.TabIndex = 7;
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(663, 107);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(342, 31);
            txtApellido.TabIndex = 6;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(663, 54);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(342, 31);
            txtNombre.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(475, 278);
            label4.Name = "label4";
            label4.Size = new Size(75, 25);
            label4.TabIndex = 4;
            label4.Text = "Correo: ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(475, 224);
            label3.Name = "label3";
            label3.Size = new Size(88, 25);
            label3.TabIndex = 3;
            label3.Text = "Teléfono: ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(475, 170);
            label2.Name = "label2";
            label2.Size = new Size(110, 25);
            label2.TabIndex = 2;
            label2.Text = "Documento:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(475, 113);
            label1.Name = "label1";
            label1.Size = new Size(87, 25);
            label1.TabIndex = 1;
            label1.Text = "Apellido: ";
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(475, 60);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(82, 25);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre:";
            // 
            // dgvUsuarios
            // 
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsuarios.Location = new Point(87, 513);
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.RowHeadersWidth = 62;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.Size = new Size(1398, 406);
            dgvUsuarios.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnNuevo);
            panel1.Controls.Add(iconButton4);
            panel1.Controls.Add(iconButton1);
            panel1.Controls.Add(iconButton3);
            panel1.Controls.Add(iconButton2);
            panel1.Location = new Point(325, 334);
            panel1.Name = "panel1";
            panel1.Size = new Size(855, 60);
            panel1.TabIndex = 15;
            // 
            // FrmUsuarios
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1401, 950);
            Controls.Add(dgvUsuarios);
            Controls.Add(pnlDatos);
            Controls.Add(pnlTitulo);
            Name = "FrmUsuarios";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrmUsuarios";
            WindowState = FormWindowState.Maximized;
            Load += FrmUsuarios_Load;
            pnlTitulo.ResumeLayout(false);
            pnlTitulo.PerformLayout();
            pnlDatos.ResumeLayout(false);
            pnlDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTitulo;
        private Panel pnlDatos;
        private Label lblTitulo;
        private Label lblNombre;
        private FontAwesome.Sharp.IconButton btnNuevo;
        private TextBox txtCorreo;
        private TextBox txtTelefono;
        private TextBox txtDocumento;
        private TextBox txtApellido;
        private TextBox txtNombre;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private FontAwesome.Sharp.IconButton iconButton4;
        private FontAwesome.Sharp.IconButton iconButton3;
        private FontAwesome.Sharp.IconButton iconButton2;
        private FontAwesome.Sharp.IconButton iconButton1;
        private DataGridView dgvUsuarios;
        private Panel panel1;
    }
}
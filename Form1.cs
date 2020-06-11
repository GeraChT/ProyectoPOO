using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Net.Mail;

namespace Proyecto_Final_POO
{
    public partial class Form1 : Form
    {
        string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=user";
        const string CORREO = "gerardo010yt@hotmail.com";
        const string CONTRASEÑA = "F3d3r1c0";
        const string SERVIDOR = "smtp.office365.com";
        const int PUERTO = 587;
        Datos misDatos = new Datos();
        public Form1()
        {
            InitializeComponent();
        }
        public void LimpiarTextBox()
        {
            foreach (Control x in grbDatos.Controls)
                if (x is TextBox) x.Text = "";
        }
        public string Tipo()
        {
            if (radPagado.Checked) return "Pagado";
            else return "Pendiente";
        }
        public void AgregarSQL()
        {
            string query = $"INSERT INTO `data` (`id`, `factura`, `cliente`, `costo`, `estado`, `fecha`) VALUES (NULL, '{txtNFactura.Text}', '{txtCliente.Text}', '{txtCosto.Text}', '{Tipo()}', '{DateTime.Now}')";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDataBase = new MySqlCommand(query, databaseConnection);
            commandDataBase.CommandTimeout = 60;
            misDatos.NFactura = txtNFactura.Text;
            misDatos.Cliente = txtCliente.Text;
            misDatos.Costo = double.Parse(txtCosto.Text);
            misDatos.Estado = Tipo();
            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDataBase.ExecuteReader();
                MessageBox.Show("Guardado");
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ListadoSQL()
        {
            dgDatos.Rows.Clear();
            string query = "SELECT * FROM data";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDataBase = new MySqlCommand(query, databaseConnection);
            commandDataBase.CommandTimeout = 60;
            MySqlDataReader reader;
            try
            {
                databaseConnection.Open();
                reader = commandDataBase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dgDatos.Rows.Add(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDouble(3), reader.GetString(4), reader.GetString(5));
                    }
                }
                else MessageBox.Show("No se encontraron registros");
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtNFactura.Focus();
        }
        public void ModificarSQL()
        {
            if (MessageBox.Show($"¿Esta seguro que desea pagarlo?\nNúmero de Factura: {txtPagar_Eliminar.Text}", "PAGO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            string fecha = DateTime.Now.ToString();
            string query = $"UPDATE data SET `estado` = 'Pagado', `fecha` = '{fecha}' WHERE `factura` = '{txtPagar_Eliminar.Text}'";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDataBase = new MySqlCommand(query, databaseConnection);
            commandDataBase.CommandTimeout = 60;
            MySqlDataReader reader;
            try
            {
                foreach (DataGridViewRow row in dgDatos.Rows)
                {
                    if (Convert.ToString(row.Cells["Factura"].Value) == txtPagar_Eliminar.Text)
                    {
                        misDatos.NFactura = txtPagar_Eliminar.Text;
                        misDatos.Cliente = Convert.ToString(row.Cells["Cliente"].Value);
                        misDatos.Costo = double.Parse(row.Cells["Costo"].Value.ToString());
                        misDatos.Estado = "Pagado";
                    }
                }
                databaseConnection.Open();
                reader = commandDataBase.ExecuteReader();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtNFactura.Focus();
            ListadoSQL();
        }
        public void EliminarSQL()
        {
            if (MessageBox.Show($"¿Esta seguro que desea eliminarlo?\nNúmero de Factura: {txtPagar_Eliminar.Text}", "PAGO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            string query = $" DELETE FROM `data` WHERE `factura` = '{txtPagar_Eliminar.Text}'";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDataBase = new MySqlCommand(query, databaseConnection);
            commandDataBase.CommandTimeout = 60;
            MySqlDataReader reader;
            try
            {
                databaseConnection.Open();
                reader = commandDataBase.ExecuteReader();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtPagar_Eliminar.Text = "";
            ListadoSQL();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            misDatos.EnviarCorreo += MetodoGestor;
            ListadoSQL();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarSQL();
            ListadoSQL();
            LimpiarTextBox();
        }
        private void btnTotales_Click(object sender, EventArgs e)
        {
            Total();
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            ModificarSQL();
            txtPagar_Eliminar.Text = "";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            EliminarSQL();
        }

        private void dgDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtPagar_Eliminar.Text = dgDatos.CurrentRow.Cells[1].Value.ToString();
        }

        public void Total()
        {
            double pagado = 0;
            double pendiente = 0;
            foreach (DataGridViewRow row in dgDatos.Rows)
            {
                if (Convert.ToString(row.Cells["Estado"].Value) == "Pagado")
                {
                    pagado += double.Parse(row.Cells["Costo"].Value.ToString());
                }
                if (Convert.ToString(row.Cells["Estado"].Value) == "Pendiente")
                {
                    pendiente += double.Parse(row.Cells["Costo"].Value.ToString());
                }
            }
            MessageBox.Show($"Pagado: {pagado:C}\n\nPendiente: {pendiente:C}\n\nTotal: {(pagado + pendiente):C}", "TOTAL", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void MetodoGestor(string Cliente, string Factura, double Cantidad)
        {
            MessageBox.Show($"Cliente: {Cliente}\nNúmero de Factura: {Factura}\nMonto: {Cantidad:C})", "PAGADO", MessageBoxButtons.OK);
            MailMessage miMensaje = new MailMessage();
            miMensaje.Subject = "Nuevo Pago";
            miMensaje.To.Add(new MailAddress("geracht1@gmail.com"));
            miMensaje.From = new MailAddress(CORREO, "Gerardo Chavira");
            miMensaje.Body = $"{misDatos.Cliente} a pagado la factura con el número {misDatos.NFactura}\nMonto: {misDatos.Costo:C}\nFecha y Hora: {DateTime.Now}";
            CorreoElectronico miCorreoElectronico = new CorreoElectronico(SERVIDOR, PUERTO, CORREO, CONTRASEÑA);
            miCorreoElectronico.Enviar(miMensaje);
        }
    }
}
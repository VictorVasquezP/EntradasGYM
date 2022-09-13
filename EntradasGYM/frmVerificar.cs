using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntradasGYM
{
    public partial class frmVerificar : CaptureForm
    {
        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        //private UsuariosDBEntities contexto;
        private MySqlConnection connection;

        public void Verify(DPFP.Template template)
        {
            Template = template;
            ShowDialog();
        }

        protected override void Init()
        {
            base.Init();
            base.Text = "Verificación de Huella Digital";
            Verificator = new DPFP.Verification.Verification();     // Create a fingerprint template verificator
            UpdateStatus(0);
        }

        private void UpdateStatus(int FAR)
        {
            // Show "False accept rate" value
            SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR));
        }

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            // Check quality of the sample and start verification if it's good
            // TODO: move to a separate task
            if (features != null)
            {
                // Compare the feature set with our template
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                DPFP.Template template = new DPFP.Template();
                Stream stream;
                //foreach (var emp in contexto.Empleado)
                //{
                //    stream = new MemoryStream(emp.Huella);
                //    template = new DPFP.Template(stream);

                //    Verificator.Verify(features, template, ref result);
                //    UpdateStatus(result.FARAchieved);
                //    if (result.Verified)
                //    {
                //        MakeReport("La huella fue verificada. " + emp.Nombre);
                //        break;
                //    }
                //}

                //recuperamos la lista de clientes
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT id, fecha_inicio, fecha_fin, foto, nombre, apellidos, huella FROM clientes", connection);
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(6))
                        {
                            stream = reader.GetStream(6);
                            template = new DPFP.Template(stream);
                            Console.WriteLine(stream.Length);
                            Console.WriteLine($"{reader.GetInt32(0)} \t {reader.GetDateTime(1)} \t {reader.GetString(4)}");
                            Verificator.Verify(features, template, ref result);
                            UpdateStatus(result.FARAchieved);

                            if (result.Verified)
                            {
                                MakeReport("La huella fue verificada. " + reader.GetString(4));
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                reader.Close();

                connection.Close();
            }
        }

        public frmVerificar()
        {
            //contexto = new UsuariosDBEntities();
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "localhost";
            builder.Database = "gym_control";
            builder.UserID = "root";
            builder.Password = "victor";
            builder.Port = 3306;
            connection = new MySqlConnection(builder.ToString());
            //connection.Open();
            //MySqlCommand command = new MySqlCommand("SELECT id, fecha_inicio, fecha_fin, foto, nombre, apellidos, huella FROM clientes", connection);
            //var reader = command.ExecuteReader();
            //Stream stream;
            //while (reader.Read())
            //{
            //    if (!reader.IsDBNull(6))
            //    {
            //        //long len = reader.GetBytes(6, 0, null, 0, 0);
            //        //byte[] buffer = new byte[len];
            //        //reader.GetBytes(6, 0, buffer, 0, (int)len);
            //        stream = reader.GetStream(6);
            //        Bitmap imagen = new Bitmap(Image.FromStream(stream));
            //        SetPicture(imagen);
            //        Console.WriteLine($"{reader.GetInt32(0)} \t {reader.GetDateTime(1)} \t {reader.GetString(4)}");
            //        //Console.WriteLine(stream.Length);
            //    }
            //}
            //reader.Close();
            //connection.Close();
            InitializeComponent();
        }
    }
}

using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EntradasGYM
{
    public partial class frmVerificar : CaptureForm
    {
        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;

        public void Verify(DPFP.Template template)
        {
            Template = template;
            ShowDialog();
        }

        protected override void Init()
        {
            base.Init();
            base.Text = "BERETTA TEAM";
            Verificator = new DPFP.Verification.Verification();     // Create a fingerprint template verificator
            UpdateStatus(0);
        }

        private void UpdateStatus(int FAR)
        {
            // Show "False accept rate" value
            //SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR));
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
                Console.WriteLine("Evento Canon");
                //recuperamos la lista de clientes
                //List<Empleados> empleados;
                try
                {
                    /*
                    stream = reader.GetStream(6);
                    template = new DPFP.Template(stream);
                    Verificator.Verify(features, template, ref result);
                    UpdateStatus(result.FARAchieved);

                    if (result.Verified)
                    {
                        if (!reader.IsDBNull(3))
                        {
                            //Image image = Image.FromStream(new MemoryStream(Convert.FromBase64String(reader.GetString(3)))); // convertir base64 a Image
                            Image image = Image.FromFile("C:\\xampp\\htdocs\\gym_control\\public\\clientesfile\\" + reader.GetString(3));
                            SetPicture(image);
                        }
                        DateTime fecha = DateTime.Now;
                        DateTime fecha_fin = reader.GetDateTime(2);
                        TimeSpan difFechas = fecha_fin - fecha;
                        SetNombre(reader.GetString(4) + " "+ reader.GetString(5));
                        SetInicio(reader.GetDateTime(1).ToLongDateString());
                        if (difFechas.Days >= 0)
                        {
                            //insert table entradas
                            MySqlConnection conn = new MySqlConnection(builder.ToString());
                            conn.Open();
                            int retorno = 0;
                            DateTime date = DateTime.Now;
                            MySqlCommand comando = new MySqlCommand("INSERT INTO entradas (id_cliente, created_at, updated_at) VALUES (@a, @b, @c)", conn);
                            comando.Parameters.AddWithValue("@a",reader.GetInt32(0));
                            comando.Parameters.AddWithValue("@b",date);
                            comando.Parameters.AddWithValue("@c",date);
                            retorno = comando.ExecuteNonQuery();
                            conn.Close();
                        }
                        Thread.Sleep(6000);
                        SetReset();
                        break;
                    }
                    Console.WriteLine(result.Verified);
                    */
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public frmVerificar()
        {
            InitializeComponent();
        }

    }
}

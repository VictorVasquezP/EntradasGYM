using MySqlConnector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EntradasGYM
{
    public partial class frmVerificar : CaptureForm
    {
        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        private MySqlConnection connection;
        MySqlConnectionStringBuilder builder;

        public void Verify(DPFP.Template template)
        {
            Template = template;
            ShowDialog();
        }

        protected override void Init()
        {
            base.Init();
            base.Text = "EL REY CONDOY";
            Verificator = new DPFP.Verification.Verification();     // Create a fingerprint template verificator
            UpdateStatus(0);
        }

        private void UpdateStatus(int FAR)
        {
            // Show "False accept rate" value
            //SetStatus(String.Format("False Accept Rate (FAR) = {0}", FAR));
        }

        protected override async void Process(DPFP.Sample Sample)
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
                //recuperamos la lista de clientes
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT id, nombre, huella FROM empleados", connection);
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(2))
                        {
                            stream = reader.GetStream(2);
                            template = new DPFP.Template(stream);
                            Verificator.Verify(features, template, ref result);
                            UpdateStatus(result.FARAchieved);
                            Console.WriteLine("if");
                            if (result.Verified)
                            {
                                Console.WriteLine("Entre");
                                //POST ASISTENCIA
                                using (HttpClient client = new HttpClient())
                                {
                                    string url = "http://127.0.0.1:8000/api/asistencia"; // URL de la API

                                    // Crear un objeto con los datos que deseas enviar
                                    var datos = new { id = reader.GetInt32(0) };

                                    // Serializar los datos a formato JSON
                                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(datos);

                                    // Crear el contenido de la solicitud con el JSON
                                    var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                                    // Realizar la solicitud POST y obtener la respuesta
                                    HttpResponseMessage response = await client.PostAsync(url, contenido);

                                    // Verificar si la solicitud fue exitosa
                                    if (response.IsSuccessStatusCode)
                                    {
                                        // La solicitud fue exitosa, puedes realizar alguna acción con la respuesta
                                        string contenidoRespuesta = await response.Content.ReadAsStringAsync();
                                        Console.WriteLine(contenidoRespuesta);
                                        JObject jsonEmpleado = JObject.Parse(contenidoRespuesta);
                                        string status = (string)jsonEmpleado["status"];
                                        bool message = (bool)jsonEmpleado["message"];
                                        Console.WriteLine(message);
                                        Console.WriteLine(status);
                                        if (status == "200" && message)
                                        {
                                            string base64 = (string)jsonEmpleado["data"];
                                            string nombre = (string)jsonEmpleado["nombre"];
                                            string info = (string)jsonEmpleado["info"];
                                            Image image = Image.FromStream(new MemoryStream(Convert.FromBase64String(base64))); // convertir base64 a Image
                                            SetNombre(nombre);
                                            SetInicio(info);
                                            SetPicture(image);
                                        }
                                    }
                                    else
                                    {
                                        // La solicitud no fue exitosa, puedes manejar el error de acuerdo a tus necesidades
                                        Console.WriteLine("La solicitud no fue exitosa. Código de estado: " + response.StatusCode);
                                    }
                                }
                                Thread.Sleep(6000);
                                SetReset();
                                break;
                            }
                            Console.WriteLine(result.Verified);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                reader.Close();
                connection.Close();
            }
        }

        public frmVerificar()
        {
            builder = new MySqlConnectionStringBuilder();
            builder.Server = "localhost";
            builder.Database = "ca_db";
            builder.UserID = "root";
            builder.Password = "";
            builder.Port = 3306;
            connection = new MySqlConnection(builder.ToString());
            InitializeComponent();
        }

        private void frmVerificar_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EntradasGYM
{
    /* NOTE: This form is a base for the EnrollmentForm and the VerificationForm,
		All changes in the CaptureForm will be reflected in all its derived forms.
	*/
    delegate void Function();

    public partial class CaptureForm : Form, DPFP.Capture.EventHandler
	{
		public CaptureForm()
		{
			InitializeComponent();
		}

		protected virtual void Init()
		{
            try
            {
                Capturer = new DPFP.Capture.Capture();				// Create a capture operation.

                if ( null != Capturer )
                    Capturer.EventHandler = this;					// Subscribe for capturing events.
            }
            catch
            {               
                MessageBox.Show("No se pudo iniciar la operación de captura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }
            
		}

		protected virtual void Process(DPFP.Sample Sample)
		{
			// Draw fingerprint sample image.
			//DrawPicture(ConvertSampleToBitmap(Sample));
		}

		protected void Start()
		{
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    //SetPrompt("Escanea tu huella usando el lector");
                }
                catch
                {
                    //SetPrompt("No se puede iniciar la captura");
                }
            }
		}

		protected void Stop()
		{
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    //SetPrompt("No se puede terminar la captura");
                }
            }
		}
		
	#region Form Event Handlers:

		private void CaptureForm_Load(object sender, EventArgs e)
		{
			Init();
			Start();												// Start capture operation.
		}

		private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Stop();
		}
	#endregion

	#region EventHandler Members:

		public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
		{
			Process(Sample);
		}

		public void OnFingerGone(object Capture, string ReaderSerialNumber)
		{
			//MakeReport("La huella fue removida del lector");
		}

		public void OnFingerTouch(object Capture, string ReaderSerialNumber)
		{
			//MakeReport("El lector fue tocado");
		}

		public void OnReaderConnect(object Capture, string ReaderSerialNumber)
		{
			SetPrompt("Escanea tu huella para ingresar");
		}

		public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
		{
			SetPrompt("El lector de huellas está desconectado");
		}

		public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
		{
			//if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
			//	MakeReport("La calidad de la muestra es BUENA");
			//else
			//	MakeReport("La calidad de la muestra es MALA");
		}
	#endregion

		protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
		{
			DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();	// Create a sample convertor.
			Bitmap bitmap = null;												            // TODO: the size doesn't matter
			Convertor.ConvertToPicture(Sample, ref bitmap);									// TODO: return bitmap as a result
			return bitmap;
		}

		protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
		{
			DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();	// Create a feature extractor
			DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
			DPFP.FeatureSet features = new DPFP.FeatureSet();
			Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);			// TODO: return features as a result?
			if (feedback == DPFP.Capture.CaptureFeedback.Good)
				return features;
			else
				return null;
		}

		protected void SetPrompt(string prompt)
		{
			this.Invoke(new Function(delegate() {
				Prompt.Text = prompt;
			}));
		}

		protected void SetInicio(string message)
		{
			this.Invoke(new Function(delegate () {
				fecha_inicio.Text = message;
			}));
		}

		protected void SetNombre(string message)
		{
			this.Invoke(new Function(delegate() {
				nombre.Text = message;
			}));
		}

		protected void SetFin(string message)
		{
			this.Invoke(new Function(delegate () {
				fecha_fin.Text = message;
			}));
		}

		protected void SetDias(int num)
		{
			this.Invoke(new Function(delegate () {
                if (num > 0)
                {
					dias.Text = num + " DÍAS";
					dias.BackColor = Color.DarkGreen;
					StatusLine.BackColor = Color.DarkGreen;
					StatusLine.Text = "ENTRADA REGISTRADA";
				}
                else if(num == 0)
                {
					dias.Text = "HOY TERMINA";
					dias.BackColor = Color.DarkGreen;
					StatusLine.BackColor = Color.DarkGreen;
					StatusLine.Text = "ENTRADA REGISTRADA";
                }
                else
                {
					dias.Text = "SIN DÍAS";
					dias.BackColor = Color.DarkRed;
					StatusLine.BackColor = Color.DarkRed;
					StatusLine.Text = "SUSCRIPCIÓN VENCIDA";
				}
				
			}));
		}

		protected void SetReset()
        {
			this.Invoke(new Function(delegate () {
				nombre.Text = "";
				fecha_fin.Text = "";
				fecha_inicio.Text = "";
				dias.Text = "";
				dias.BackColor = SystemColors.Control;
				StatusLine.Text = "";
				StatusLine.BackColor = SystemColors.Control;
				Picture.Image = null;
			}));
			
		}

		private void DrawPicture(Bitmap bitmap)
		{
			this.Invoke(new Function(delegate() {
                Picture.Image = new Bitmap(bitmap, Picture.Size);	// fit the image into the picture box
            }));
		}

		public void SetPicture(Image image)
        {
			Picture.Image = new Bitmap(image, Picture.Size);
		}

		private DPFP.Capture.Capture Capturer;

        private void CloseButton_Click(object sender, EventArgs e)
        {

        }
    }
}
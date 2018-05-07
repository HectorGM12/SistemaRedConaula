using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinSockets
{
    [Activity(Label = "Cliente")]
    public class Cliente : Activity
    {
        private EditText editTextIP;
        private EditText editTextPort;
        private EditText editTextChat;
        private Button buttonConectar;
        private Button buttonEnviar;
        private ConexionTcpCliente conexion;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Cliente);
            //Texviews
            this.editTextIP = FindViewById<EditText>(Resource.Id.editTextIP);
            this.editTextPort = FindViewById<EditText>(Resource.Id.editTextPort);
            this.editTextChat = FindViewById<EditText>(Resource.Id.editTextChat);
            //Botones

            this.buttonConectar = FindViewById<Button>(Resource.Id.buttonConectar);
            this.buttonEnviar = FindViewById<Button>(Resource.Id.buttonEnviarMSj);

            //eventos

            this.buttonConectar.Click += ButtonConectar_Click;
            this.buttonEnviar.Click += ButtonEnviar_Click;

            //Instanciar conexion cliente

            this.conexion = new ConexionTcpCliente();

            //Eventos de la clase cliente

            //Evento cuando llegue un msj
            this.conexion.OnDataRecieved += Conexion_OnDataRecieved;
            //Evento cuando te desconectas
            this.conexion.OnDisconnect += Conexion_OnDisconnect;
            //Evento en caso de error
            this.conexion.OnError += Conexion_OnError;


            
           
        }
        /*
          
            Cuando ocurre un error se llama el evento onError(e), al cual se le pasa una excepcion
         */
        private void Conexion_OnError(Exception e)
        {  
            //prepara el hilo
            Looper.Prepare();
            //El RunOnUiThread es necesario para utlizar los objetos de la gui desde otro hilo
            RunOnUiThread(() => {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            });
            //quita el bloquo del hilo
            Looper.Loop();
        }
        
        private void Conexion_OnDisconnect()
        {
            Looper.Prepare();
            RunOnUiThread(() => {
                Toast.MakeText(this,"Te desconectaste", ToastLength.Long).Show();
            });
            Looper.Loop();
        }

        /*
         * Ocurre este evento cuando te llega un mensja todos los mensaje heredan de la clase 
         * Message entonce cuando llegues tendras que aplicar un conversion unboxing
         */
        private void Conexion_OnDataRecieved(Mensajes.Message mensaje)
        {
            Looper.Prepare();
            RunOnUiThread(() => {
                Toast.MakeText(this, "Se recibio el msj: "+mensaje.message, ToastLength.Long).Show();
            });
            Looper.Loop();
        }

        private void ButtonEnviar_Click(object sender, EventArgs e)
        {
            conexion.EscribirMensaje(new Mensajes.Message(Types.MESSAGECHAT,this.editTextChat.Text));
        }

        private void ButtonConectar_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(Cargar);
            thread.Start();
        }
        private void Cargar()
        {

            Looper.Prepare();
           
            bool s = conexion.Conectar(this.editTextIP.Text, int.Parse(this.editTextPort.Text));
            RunOnUiThread(() => {
                if (s)
                {
                    Toast.MakeText(this, "Se conecto", ToastLength.Short).Show();
                    this.editTextChat.Enabled = true;
                    this.buttonEnviar.Enabled = true;
                    this.buttonConectar.Enabled = false;

                }
                else
                {
                    Toast.MakeText(this, "No se conecto", ToastLength.Short).Show();
                }
            });
            Looper.Loop();

        }
    }
}
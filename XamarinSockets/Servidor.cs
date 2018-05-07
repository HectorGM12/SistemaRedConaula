using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinSockets
{
    [Activity(Label = "Servidor")]
    public class Servidor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string hostName = Dns.GetHostName();
            string ip="vacia";
            IPHostEntry iPHost = Dns.GetHostEntry(hostName);
            if (iPHost.AddressList.Length > 0)
            {

                int index = 0;
                foreach (IPAddress item in iPHost.AddressList)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork) //Filtra el protocolo IPV4


                    {
                        index = iPHost.AddressList.ToList().IndexOf(item);
                    }
                }
                 ip = iPHost.AddressList[index].ToString();
                ServerSocket server = new ServerSocket(ip,9001);
                server.OnClientConnected += Server_OnClientConnected;
                server.OnClientDisconnected += Server_OnClientDisconnected;
                server.OnDataRecieved += Server_OnDataRecieved;
                server.onServerError += Server_onServerError;
                server.listenClients();

            }
            SetContentView(Resource.Layout.Servidor);
            Toast.MakeText(this, "Escuchando en el puerto: "+ip, ToastLength.Long).Show();
        }

        private void Server_onServerError(Exception exception)
        {
            Looper.Prepare();
            RunOnUiThread(() => {
                Toast.MakeText(this, exception.Message, ToastLength.Long).Show();
            });
            Looper.Loop();           
        }

        private void Server_OnDataRecieved(ConexionTcpServidor conexionTcp, Mensajes.Message data)
        {
            Looper.Prepare();
            conexionTcp.EscribirMensaje(new Mensajes.Message(Types.MESSAGECHAT,"Se recivio tu msj"));
            RunOnUiThread(() => {
                Toast.MakeText(this, data.message, ToastLength.Long).Show();
            });
            Looper.Loop();
        }

        private void Server_OnClientDisconnected(ConexionTcpServidor conexionTcp)
        {
            Looper.Prepare();        
            RunOnUiThread(() => {
                Toast.MakeText(this, "Se desconecto un cliente", ToastLength.Long).Show();
            });
            Looper.Loop();           
        }

        private void Server_OnClientConnected(ConexionTcpServidor conexionTcp)
        {            
                Looper.Prepare();

                RunOnUiThread(() => {
                    Toast.MakeText(this, "Se conecto un cliente", ToastLength.Long).Show();
                });
                Looper.Loop();               
        }
    }
}
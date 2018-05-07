using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Threading;
using System.IO;
using Mensajes;

namespace XamarinSockets
{
    class ConexionTcpServidor
    {
        #region Variables de instancia
        public TcpClient tcpClient;
        public NetworkStream stream;
        public Thread thread;
        #endregion
        #region Eventos
        public delegate void DataCarrier(Message msj);
        public event DataCarrier OnDataRecived;

        public delegate void DisconnectNotify();
        public event DisconnectNotify OnDisconnect;

        public delegate void ErrorCarrier(Exception e);
        public event ErrorCarrier OnError;

        #endregion
        public ConexionTcpServidor(TcpClient client)
        {
            stream = client.GetStream();
            tcpClient = client;
        }
        public void EscribirMensaje(Message mensaje)
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, mensaje);
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    OnError(e);
                }
            }
        }
    }
}
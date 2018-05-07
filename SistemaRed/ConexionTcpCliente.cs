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
namespace SistemaRed
{
    class ConexionTcpCliente
    {
        #region Propiedades
        public TcpClient tcpClient { get; private set; }
        public NetworkStream stream { get; private set; }
        public Thread thread { get; private set; }
        public BinaryFormatter binaryFormatter { get; private set; }
        #endregion
        #region Eventos
        public delegate void DataCarrier(Message mensaje);
        public event DataCarrier OnDataRecieved;
        public delegate void DisconnectNotify();
        public event DisconnectNotify OnDisconnect;
        public delegate void ErrorCarrier(Exception e);
        public event ErrorCarrier OnError;
        #endregion
        #region Metodos 
        public bool Conectar(string ip, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(ip), port);
                stream = tcpClient.GetStream();
                thread = new Thread(escuchar);
                binaryFormatter = new BinaryFormatter();
                //EscribirMensaje
                thread.Start();
                return true;
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    OnError(e);
                }
                return false;
            }
        }
        private void escuchar() {
            do
            {
                try
                {
                    if (OnDataRecieved != null)
                    {
                       Message mensaje = (Message)binaryFormatter.Deserialize(stream);
                        OnDataRecieved(mensaje);
                    }
                }
                catch (Exception e)
                {
                    if (OnError != null)
                    {
                        OnError(e);
                    }
                    break;
                }
            } while (true);
            if (OnDisconnect != null)
            {
                OnDisconnect();
            }
        }
        public void EscribirMensaje(Message mensaje){
            try
            {

                binaryFormatter.Serialize(stream,mensaje);             
            }
            catch (Exception e)
            {

                if (OnError!=null)
                {
                    OnError(e);
                }
            }
        }
        #endregion


    }
}

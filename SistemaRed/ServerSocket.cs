using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mensajes;
namespace SistemaRed
{
    class ServerSocket
    {
        public delegate void ClientCarrier(ConexionTcpServidor conexionTcp);
        public delegate void ServerError(Exception exception);
        public event ServerError onServerError;
        public event ClientCarrier OnClientConnected;
        public event ClientCarrier OnClientDisconnected;
        public delegate void DataRecieved(ConexionTcpServidor conexionTcp, Message data);
        public event DataRecieved OnDataRecieved;
        private TcpListener _tcpListener;
        private Thread _acceptThread;
        private string ipServer;
        private int port;
        public List<Message> messages;
        public List<ConexionTcpServidor> usersConnected;
        public ServerSocket(string ipServer, int port)
        {
            messages = new List<Message>();
            usersConnected = new List<ConexionTcpServidor>();

            this.ipServer = ipServer;
            this.port = port;
        }

        public void listenClients() {


            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(this.ipServer), this.port);
                _tcpListener.Start();
                _acceptThread = new Thread(acceptClients);
                _acceptThread.Start();
            }
            catch (Exception e)
            {
                if (onServerError != null)
                {
                    onServerError(e);
                }
            }
        }
        private void acceptClients() {
            do
            {
                try
                {
                    var conexion = _tcpListener.AcceptTcpClient();
                    var srvClient = new ConexionTcpServidor(conexion)
                    {
                        thread = new Thread(readData)
                    };
                    srvClient.thread.Start(srvClient);
                    lock (this.usersConnected)
                    {
                        if (OnClientConnected != null)
                            if (!usersConnected.Contains(srvClient))
                            {
                                usersConnected.Add(srvClient);
                            }
                        OnClientConnected(srvClient);
                    }
                }
                catch (Exception e)
                {
                    if (onServerError != null)
                    {
                        onServerError(e);
                    }
                }

            } while (true);
        }
        private void readData(object client) {
            var cli = client as ConexionTcpServidor;

            do
            {
                try
                {

                    if (OnDataRecieved != null)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        Message message = (Message)bf.Deserialize(cli.stream);
                        //Invoke our event
                        lock (messages)
                        {
                            messages.Add(message);
                        }
                        OnDataRecieved(cli, message);
                    }
                }           
                catch (Exception e)
                {

                  
                    break;
                }
            } while (true);

            if (OnClientDisconnected != null)
                lock (usersConnected)
                {
                    if (usersConnected.Contains(cli))
                    {

                        usersConnected.RemoveAt(usersConnected.IndexOf(cli));
                    }
                    OnClientDisconnected(cli);
                }
        }
      
    }
}

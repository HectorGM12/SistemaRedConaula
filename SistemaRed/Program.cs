using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SistemaRed
{
    class Program
    {
        static void Main(string[] args)
        {  
            Console.Write("Escribe la ip del servidor: ");
            
            string ip = Console.ReadLine();
            Console.Write("Escribe el puerto: ");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Te vas a conectar o vas escuchar: ");
            string msj = Console.ReadLine();
            if (msj == "c")
            {
                ConexionTcpCliente conexion = new ConexionTcpCliente();
                conexion.OnDataRecieved += Conexion_OnDataRecieved;
                conexion.OnDisconnect += Conexion_OnDisconnect;
                conexion.OnError += Conexion_OnError;
                bool vedadero = conexion.Conectar(ip, port);
                if (vedadero)
                {
                    Console.WriteLine("Conectado----");
                    while (true) {
                       string msja= Console.ReadLine();
                        if (msja=="salir") {
                            break;
                        }
                        Console.WriteLine("Escribe un msj");
                        conexion.EscribirMensaje(new Mensajes.Message(Types.MESSAGECHAT,msja));
                    }
                }
            }

            else {
                ServerSocket server = new ServerSocket(ip, port);
                server.OnClientConnected += Server_OnClientConnected;
                server.OnClientDisconnected += Server_OnClientDisconnected;
                server.OnDataRecieved += Server_OnDataRecieved;
                server.onServerError += Server_onServerError;
                server.listenClients();
                Console.WriteLine("*********Sistema de red iniciado*******");
            }
           
            
        }

        private static void Conexion_OnError(Exception e)
        {
            Console.WriteLine("********Error detectado*******");
            Console.WriteLine(e.Message);
        }

        private static void Conexion_OnDisconnect()
        {
            Console.WriteLine("Desconectado");
        }

        private static void Conexion_OnDataRecieved(Mensajes.Message mensaje)
        {
            Console.WriteLine("Mesaje detectado: "+mensaje.message);
        }

        private static void Server_onServerError(Exception exception)
        {
            Console.WriteLine("********Error detectado*******");
            Console.WriteLine(exception.Message);
        }

        private static void Server_OnDataRecieved(ConexionTcpServidor conexionTcp, Mensajes.Message data)
        {
            Console.WriteLine("*********Nuevo mensaje********");
            Console.WriteLine(data.message);
            Mensajes.Message message = new Mensajes.Message(Types.MESSAGECHAT,string.Format("Mensaje recibido {0}",data.message));
            conexionTcp.EscribirMensaje(message);

        }

        private static void Server_OnClientDisconnected(ConexionTcpServidor conexionTcp)
        {
            Console.WriteLine("************Usuario desconectado***********");
        }

        private static void Server_OnClientConnected(ConexionTcpServidor conexionTcp)
        {
            Console.WriteLine("************Usuario conectado***********");
        }
    }
}

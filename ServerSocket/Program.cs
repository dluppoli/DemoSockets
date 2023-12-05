using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Creazione della socket
            var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Binding della socket(ip address e porta)
            socket.Bind(endpoint);
            socket.Listen(128);

            //Accettazione della connessione(solo per TCP)
            Console.WriteLine("Server is running");
            while (true)
            {
                using (Socket clientSocket = await socket.AcceptAsync())
                {

                    //Generazione dello stream di comunicazione(solo per TCP)
                    using (NetworkStream networkStream = new NetworkStream(clientSocket))
                    {
                        while (true)
                        {
                            try
                            {
                                //Ricezione e Invio dei dati
                                byte[] clientMessageBytes = new byte[10000];
                                await networkStream.ReadAsync(clientMessageBytes, 0, clientMessageBytes.Length);
                                string clientMessage = System.Text.Encoding.ASCII.GetString(clientMessageBytes).TrimEnd('\0');

                                byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes("Echo di " + clientMessage);
                                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                            }
                            catch (Exception e)
                            {
                                break;
                            }
                        }
                        //Chiusura socket
                        //networkStream.Close();
                        //clientSocket.Dispose();
                    }
                }
            }
            //socket.Dispose();
        }
    }
}

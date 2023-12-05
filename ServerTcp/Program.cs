using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerTcp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Creazione della socket
            var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
            TcpListener socket = new TcpListener(endpoint);
            socket.Start();
            //Binding della socket(ip address e porta)
            //socket.Bind(endpoint);
            //socket.Listen(128);

            //Accettazione della connessione(solo per TCP)
            Console.WriteLine("Server is running");
            while (true)
            {
                using (TcpClient clientSocket = await socket.AcceptTcpClientAsync())
                {

                    //Generazione dello stream di comunicazione(solo per TCP)
                    using (NetworkStream networkStream = clientSocket.GetStream())
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

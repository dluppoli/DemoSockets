using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerUdp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Creazione della socket
            var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
            UdpClient socket = new UdpClient(endpoint);

            //Accettazione della connessione(solo per TCP)
            Console.WriteLine("Server is running");
            while (true)
            {
                try
                {
                    //Ricezione e Invio dei dati
                    var receiveResult = await socket.ReceiveAsync();
                    byte[] clientMessageBytes = receiveResult.Buffer;
                    string clientMessage = System.Text.Encoding.ASCII.GetString(clientMessageBytes).TrimEnd('\0');

                    byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes("Echo di " + clientMessage);
                    await socket.SendAsync(messageBytes, messageBytes.Length);
                }
                catch (Exception e)
                {
                    break;
                }
            }
            //socket.Dispose();
        }
    }
}

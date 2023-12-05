using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSocket
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Premi un tasto per iniare...");
            Console.ReadLine();

            //Creare Socket (ip + porta)
            var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
            Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Collegarsi al server (ip + porta)
            await socket.ConnectAsync(endpoint);

            //Generazione Stream di comunicazione
            NetworkStream networkStream = new NetworkStream(socket,true);

            //Invio dei dati
            Console.WriteLine("Inserisci il messaggio da inviare:");
            string message = Console.ReadLine();
            byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
            await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);

            //Ricezione della risposta
            byte[] responseBytes = new byte[10000];
            await networkStream.ReadAsync(responseBytes, 0, responseBytes.Length);
            string response = System.Text.Encoding.ASCII.GetString(responseBytes).TrimEnd('\0');
            Console.WriteLine(response);
            Console.ReadLine();

            //Chiusura Socket
            networkStream.Close();
            socket.Dispose();
        }
    }
}

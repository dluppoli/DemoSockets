using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientUdp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Premi un tasto per iniare...");
            Console.ReadLine();

            //Creare Socket (ip + porta)
            //var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
            UdpClient socket = new UdpClient();

            //Collegarsi al server (ip + porta)
            socket.Connect(IPAddress.Loopback, 8888);

            //Invio dei dati
            Console.WriteLine("Inserisci il messaggio da inviare:");
            string message = Console.ReadLine();
            byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
            await socket.SendAsync(messageBytes, messageBytes.Length);

            //Ricezione della risposta
            //byte[] responseBytes = new byte[10000];
            var receiveResult = await socket.ReceiveAsync();
            byte[] responseBytes = receiveResult.Buffer;
            string response = System.Text.Encoding.ASCII.GetString(responseBytes).TrimEnd('\0');
            Console.WriteLine(response);
            Console.ReadLine();

            //Chiusura Socket
            socket.Dispose();
        }
    }
}

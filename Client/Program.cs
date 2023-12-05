using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Client
{
    public class CustomMessage
    {
        public string StringMessage { get; set; }
        public int IntMessage { get; set; }
    }

    public class NetworkPacket
    {
        public List<byte> header { get; set; }
        public List<byte> body { get; set; }

        public byte[] GetPacketBytes
        {
            get
            {
                return header.Concat(body).ToArray();
            }
        }
    }
    internal class Program
    {
        static NetworkPacket Encode(CustomMessage message)
        {
            var xs = new XmlSerializer(typeof(CustomMessage));
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            xs.Serialize(sw,message);

            var bodyBytes = System.Text.Encoding.ASCII.GetBytes(sb.ToString());
            var headerBytes = BitConverter.GetBytes( IPAddress.HostToNetworkOrder(bodyBytes.Length));

            return new NetworkPacket
            {
                body = new List<byte>(bodyBytes),
                header = new List<byte>(headerBytes)
            };
        }

        static CustomMessage Decode(byte[] body)
        {
            var xml = System.Text.Encoding.ASCII.GetString(body);
            var xs = new XmlSerializer (typeof(CustomMessage));
            var sr = new StringReader(xml);

            return (CustomMessage)xs.Deserialize(sr);
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Premi un tasto per iniare...");
            Console.ReadLine();

            //Creare Socket (ip + porta)
            TcpClient socket = new TcpClient();

            //Collegarsi al server (ip + porta)
            await socket.ConnectAsync(IPAddress.Loopback, 8888);

            //Generazione Stream di comunicazione
            NetworkStream networkStream = socket.GetStream();

            //Invio dei dati
            Console.WriteLine("Inserisci il messaggio da inviare:");
            string message = Console.ReadLine();

            CustomMessage cm = new CustomMessage
            {
                StringMessage = message,
                IntMessage = 1234
            };


            byte[] messageBytes = Encode(cm).GetPacketBytes;
            await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);


            //Ricezione della risposta
            byte[] headerBytes = new byte[4];
            await networkStream.ReadAsync(headerBytes, 0, headerBytes.Length);
            int bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes, 0));

            byte[] bodyBytes = new byte[bodyLength];
            await networkStream.ReadAsync(bodyBytes,0, bodyBytes.Length);
            CustomMessage risultato = Decode(bodyBytes);

            Console.WriteLine($"Il server ha risposto con: {risultato.StringMessage} - {risultato.IntMessage}");
            
            Console.ReadLine();

            //Chiusura Socket
            networkStream.Close();
            socket.Dispose();
        }
    }
}

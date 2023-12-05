using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server
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
            xs.Serialize(sw, message);

            var bodyBytes = System.Text.Encoding.ASCII.GetBytes(sb.ToString());
            var headerBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bodyBytes.Length));

            return new NetworkPacket
            {
                body = new List<byte>(bodyBytes),
                header = new List<byte>(headerBytes)
            };
        }

        static CustomMessage Decode(byte[] body)
        {
            var xml = System.Text.Encoding.ASCII.GetString(body);
            var xs = new XmlSerializer(typeof(CustomMessage));
            var sr = new StringReader(xml);

            return (CustomMessage)xs.Deserialize(sr);
        }

        static async Task Main(string[] args)
        {
            //Creazione della socket
            var endpoint = new IPEndPoint(IPAddress.Loopback, 8888);
            TcpListener socket = new TcpListener(endpoint);
            socket.Start();

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
                                byte[] headerBytes = new byte[4];
                                await networkStream.ReadAsync(headerBytes, 0, headerBytes.Length);
                                int bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes, 0));

                                byte[] bodyBytes = new byte[bodyLength];
                                await networkStream.ReadAsync(bodyBytes, 0, bodyBytes.Length);
                                CustomMessage risultato = Decode(bodyBytes);

                                risultato.StringMessage = "Echo di " + risultato.StringMessage;
                                risultato.IntMessage *= 2;

                                byte[] messageBytes = Encode(risultato).GetPacketBytes;
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

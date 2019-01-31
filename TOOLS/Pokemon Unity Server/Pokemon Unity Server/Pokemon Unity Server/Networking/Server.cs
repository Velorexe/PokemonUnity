using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using PokemonUnity.Networking.Packets;
using System.IO;
using PokemonUnity.Networking.Packets.Incoming;
using PokemonUnity.Networking.Server.Classes;

namespace PokemonUnity.Networking.Server
{
    static class Server
    {
        public static readonly int udpPort = 4568;
        public static bool IsRunning = false;

        private static UdpClient udpSocket;
        private static Thread udpListener;

        public static void Start()
        {
            if (!IsRunning)
            {
                udpListener = new Thread(new ThreadStart(StartListening))
                {
                    IsBackground = true
                };
                udpListener.Start();
                IsRunning = true;
            }
        }

        public static void Stop()
        {
            if (IsRunning)
            {
                udpListener.Abort();
                udpListener = new Thread(new ThreadStart(StartListening))
                {
                    IsBackground = true
                };
                udpSocket.Close();
                IsRunning = false;
            }
        }

        private static void StartListening()
        {
            IPHostEntry localHostEntry;
            try
            {
                //udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket = new UdpClient(udpPort);
                try
                {
                    if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    {
                        localHostEntry = Dns.GetHostEntry(GetLocalIPAddress());
                        udpSocket.BeginReceive(new AsyncCallback(PacketHandler), udpSocket);
                    }
                }
                catch (SocketException e)
                {

                }
            }
            catch (SocketException e)
            {

            }
        }

        private static void PacketHandler(IAsyncResult result)
        {
            try
            {
                UdpClient socket = result.AsyncState as UdpClient;
                IPEndPoint source = new IPEndPoint(0, 0);

                byte[] message = socket.EndReceive(result, ref source);
                AsyncIncomingPacketHandler(message, source);

                socket.BeginReceive(new AsyncCallback(PacketHandler), socket);
            }
            catch (ObjectDisposedException)
            {

            }
        }

        private static async void AsyncIncomingPacketHandler(byte[] message, IPEndPoint endPoint)
        {
            OutgoingPacket incomingPacket;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(message, 0, message.Length);
                memoryStream.Position = 0;

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new CustomizedBinder();

                incomingPacket = (OutgoingPacket)formatter.Deserialize(memoryStream);
            }
            await Task.Run(() => ReadIncomingPacket(incomingPacket, endPoint));
        }

        private static void ReadIncomingPacket(OutgoingPacket incomingPacket, IPEndPoint endPoint)
        {
            if (incomingPacket.Type == OutgoingPacketType.CONNECTION)
            {
                ///Here we can log in the user
                ///Then we send a confirmation back
                IncomingPacket outgoingPacket = new IncomingPacket
                {
                    Type = IncomingPacketType.CONNECTION,
                    PacketContainer = new IReceivedConnection() { IsAccepted = true }
                };
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, outgoingPacket);

                    byte[] serializedData = memoryStream.ToArray();
                    udpSocket.Send(serializedData, serializedData.Length, endPoint);
                }

            }
            else
            {
                GameServer.HandlePacket(incomingPacket, endPoint);
            }
        }

        sealed class CustomizedBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type returntype = null;
                string sharedAssemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                typeName = typeName.Replace(sharedAssemblyName, assemblyName);
                returntype =
                        Type.GetType(string.Format("{0}, {1}",
                        typeName, assemblyName));

                return returntype;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                base.BindToName(serializedType, out assemblyName, out typeName);
                assemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using PokemonUnity.Networking.Packets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;

namespace PokemonUnity.Server.Networking.Trading
{
    public class TradeRoom : IDisposable
    {
        private UdpClient host;
        private UdpClient player2;

        public TradeRoom(UdpClient hostClient)
        {
            host = hostClient;
            host.BeginReceive(new AsyncCallback(PacketHandler), host);
        }

        public void ConnectPlayer2(UdpClient player2Client)
        {
            player2 = player2Client;
            player2.BeginReceive(new AsyncCallback(PacketHandler), host);
        }

        public void Dispose()
        {
            host.Close();
            player2.Close();
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
    }
}

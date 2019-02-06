using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PokemonUnity.Networking.Packets.PacketContainers;

namespace PokemonUnity.Networking.Packets
{
    [Serializable]
    public class Packet
    {
        public PacketTypes PacketType;
        public DateTime TimeCreated = DateTime.Now;

        public object Message;

        #region Constructors

        private Packet() { }

        #region Trade

        /// <summary>
        /// INCOMING/OUTGOING
        /// </summary>
        /// <param name="packetType">CONFIRM/INITIATE</param>
        public Packet(TradePacketType tradePacketType)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(tradePacketType);
        }

        /// <summary>
        /// OUTGOING
        /// </summary>
        /// <param name="tradeRoomID">The ID of the TradeRoom</param>
        public Packet(int tradeRoomID)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(tradeRoomID);
        }

        /// <summary>
        /// INCOMING/OUTGOING
        /// </summary>
        /// <param name="serializedPokemon">The Pokemon that needs to be send</param>
        public Packet(object serializedPokemon)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(serializedPokemon);
        }
        #endregion

        #region Authentication
        /// <summary>
        /// OUTGOING
        /// </summary>
        /// <param name="token"></param>
        public Packet(string token)
        {
            PacketType = PacketTypes.AUTH;
            Message = new AuthenticationPacket(token);
        }
        #endregion

        #endregion

        public static implicit operator Packet(byte[] packet)
        {
            Packet newPacket = new Packet();
            using(MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(packet, 0, packet.Length);
                memoryStream.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                newPacket = (Packet)formatter.Deserialize(memoryStream);
            }
            return newPacket;
        }

        public static implicit operator byte[](Packet packet)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, packet);
                return memoryStream.GetBuffer();
            }
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
}

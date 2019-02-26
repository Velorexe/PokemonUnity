﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PokemonUnity.Networking.Client.Packets.PacketContainers;

namespace PokemonUnity.Networking.Server.Packets
{
    [Serializable]
    public class Packet
    {
        public PacketTypes PacketType;
        public string Token;
        public DateTime TimeCreated = DateTime.Now;

        public object Message;

        public void SetToken(string token)
        {
            Token = token;
        }

        #region Constructors

        private Packet() { }

        #region Trade

        /// <summary>
        /// SERVER: INCOMING, CLIENT: OUTGOING
        /// </summary>
        /// <param name="token"></param>
        public Packet(TradePacketType tradePacketType)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket();
        }

        /// <summary>
        /// SERVER: INCOMING/OUTGOING CLIENT: OUTGOING
        /// </summary>
        /// <param name="packetType">CONFIRM/INITIATE</param>
        public Packet(TradePacketType tradePacketType, int tradeRoomID)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(tradePacketType, tradeRoomID);
        }

        /// <summary>
        /// SERVER: OUTGOING, CLIENT: INCOMING
        /// </summary>
        /// <param name="tradeRoomID">The ID of the TradeRoom</param>
        public Packet(int tradeRoomID)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(tradeRoomID);
        }

        /// <summary>
        /// SERVER: INCOMING/OUTGOING, CLIENT: INCOMING/OUTGOING
        /// </summary>
        /// <param name="serializedPokemon">The Pokemon that needs to be send</param>
        public Packet(object serializedPokemon, int tradeRoomID)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(serializedPokemon, tradeRoomID);
        }
        #endregion

        #region Authentication
        /// <summary>
        /// SERVER: OUTGOING
        /// </summary>
        /// <param name="token"></param>
        public Packet(string token)
        {
            PacketType = PacketTypes.AUTH;
            Message = new AuthenticationPacket(token);
        }

        /// <summary>
        /// SERVER: INCOMING, CLIENT: OUTGOING
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Packet(string username, string password)
        {
            PacketType = PacketTypes.LOGIN;
            Message = new LoginPacket(username, password);
        }
        #endregion

        #region Battle
        /// <summary>
        /// SERVER: INCOMING, CLIENT: OUTGOING
        /// </summary>
        /// <param name="token"></param>
        public Packet(BattlePacketType battlePacketType)
        {
            PacketType = PacketTypes.BATTLE;
            Message = new TradePacket();
        }

        /// <summary>
        /// SERVER: INCOMING/OUTGOING CLIENT: OUTGOING
        /// </summary>
        /// <param name="packetType">CONFIRM/INITIATE</param>
        public Packet(BattlePacketType tradePacketType, int tradeRoomID)
        {
            PacketType = PacketTypes.BATTLE;
            Message = new TradePacket(tradePacketType, tradeRoomID);
        }

        /// <summary>
        /// SERVER: INCOMING/OUTGOING, CLIENT: INCOMING/OUTGOING
        /// </summary>
        /// <param name="serializedPokemon">The Pokemon that needs to be send</param>
        public Packet(object serializedPokemon, int tradeRoomID)
        {
            PacketType = PacketTypes.TRADE;
            Message = new TradePacket(serializedPokemon, tradeRoomID);
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
                formatter.Binder = new CustomizedBinder();
                newPacket = (Packet)formatter.Deserialize(memoryStream);
            }
            return newPacket;
        }

        public static implicit operator byte[](Packet packet)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new CustomizedBinder();
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
    }
}

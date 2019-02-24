using System;

namespace PokemonUnity.Networking.Client.Packets.PacketContainers
{
    [Serializable]
    public class TradePacket
    {
        public TradePacketType Type;
        public int TradeRoomID;
        //Should become SeriPokemon in the future
        public object Object;

        public TradePacket()
        {
            Type = TradePacketType.INITIATE;
        }

        public TradePacket(TradePacketType type, int tradeRoomID)
        {
            Type = type;
            TradeRoomID = tradeRoomID;
        }

        public TradePacket(object serializedPokemon, int tradeRoomID)
        {
            Type = TradePacketType.SETPOKEMON;
            Object = serializedPokemon;
            TradeRoomID = tradeRoomID;
        }
    }

    [Serializable]
    public enum TradePacketType
    {
        INITIATE,
        SETPOKEMON,
        CONFIRM
    }
}

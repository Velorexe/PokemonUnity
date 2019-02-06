using System;

namespace PokemonUnity.Networking.Packets.PacketContainers
{
    [Serializable]
    class TradePacket
    {
        public TradePacketType Type;
        //Should become SeriPokemon in the future
        public object Object;

        public TradePacket(TradePacketType type)
        {
            Type = type;
        }

        public TradePacket(object serializedPokemon)
        {
            Type = TradePacketType.SETPOKEMON;
            Object = serializedPokemon;
        }

        public TradePacket(int tradeRoomID)
        {
            Type = TradePacketType.INITIATE;
            Object = tradeRoomID;
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

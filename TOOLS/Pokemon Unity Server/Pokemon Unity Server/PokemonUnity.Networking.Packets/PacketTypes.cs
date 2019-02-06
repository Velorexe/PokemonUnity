using System;

namespace PokemonUnity.Networking.Packets
{
    [Serializable]
    public enum PacketTypes
    {
        TRADE,
        BATTLE,
        AUTH,
        LOGIN
    }
}

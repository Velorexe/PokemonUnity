using System;

namespace PokemonUnity.Networking.Server.Packets
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

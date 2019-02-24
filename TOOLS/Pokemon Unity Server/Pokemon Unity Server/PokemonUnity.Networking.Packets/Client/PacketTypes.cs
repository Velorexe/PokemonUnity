using System;

namespace PokemonUnity.Networking.Client.Packets
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

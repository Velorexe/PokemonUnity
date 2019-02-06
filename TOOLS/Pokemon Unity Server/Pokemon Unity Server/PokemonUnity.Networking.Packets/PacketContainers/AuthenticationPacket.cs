using System;

namespace PokemonUnity.Networking.Packets.PacketContainers
{
    [Serializable]
    class AuthenticationPacket
    {
        public string Token;

        public AuthenticationPacket(string token)
        {
            Token = token;
        }
    }
}

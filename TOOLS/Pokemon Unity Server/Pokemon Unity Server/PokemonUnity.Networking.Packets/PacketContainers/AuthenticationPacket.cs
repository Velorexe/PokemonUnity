using System;

namespace PokemonUnity.Networking.Packets.PacketContainers
{
    [Serializable]
    public class AuthenticationPacket
    {
        public string Token;

        public AuthenticationPacket(string token)
        {
            Token = token;
        }
    }
}

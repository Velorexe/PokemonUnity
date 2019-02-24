using System;

namespace PokemonUnity.Networking.Client.Packets.PacketContainers
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

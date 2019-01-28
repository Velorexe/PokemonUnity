namespace PokemonUnity.Networking.Packets.Outgoing
{
    [System.Serializable]
    public class ONetworkProfile : IOutPacket
    {
        public string IPAdress;
        public int Port;

        public ONetworkProfile(string ipAddress, int port)
        {
            IPAdress = ipAddress;
            Port = port;
        }
    }
}

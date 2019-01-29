namespace PokemonUnity.Networking.Packets.Outgoing
{
    [System.Serializable]
    public class ONetworkProfile : IOutPacket
    {
        public int Username;
        public int Password;

        public ONetworkProfile(int username, int password)
        {
            Username = username;
            Password = password;
        }
    }
}

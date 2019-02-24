namespace PokemonUnity.Networking.Server.Packets.PacketContainers
{
    [System.Serializable]
    public class LoginPacket
    {
        public string Username;
        public string Password;

        public LoginPacket(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

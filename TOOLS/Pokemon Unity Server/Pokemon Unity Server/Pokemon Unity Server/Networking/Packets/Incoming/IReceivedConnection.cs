namespace PokemonUnity.Networking.Packets.Incoming
{
    [System.Serializable]
    public class IReceivedConnection : IInPacket
    {
        public bool IsAccepted;
    }
}

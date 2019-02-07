using PokemonUnity.Saving.SerializableClasses;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.PacketContainers;

namespace PokemonUnity.Networking
{
    public static class TradeManager
    {
        public static void ReceivePacket(TradePacket incomingPacket)
        {
            //Perform action with data received
        }

        public static void InitiateTrade()
        {
            Packet iniateTrade = new Packet(NetworkManager.RequestToken(), TradePacketType.INITIATE);
            NetworkManager.Send(iniateTrade);
        }

        public static void SetPokemon(SeriPokemon pokemonToSet)
        {
            Packet setPokemon = new Packet(NetworkManager.RequestToken(), pokemonToSet);
            NetworkManager.Send(setPokemon);
        }
    }
}

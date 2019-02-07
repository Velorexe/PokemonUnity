using PokemonUnity.Saving.SerializableClasses;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.PacketContainers;

namespace PokemonUnity.Networking
{
    public static class TradeManager
    {
        public static int tradeRoomId;

        public static void ReceivePacket(TradePacket incomingPacket)
        {
            if(incomingPacket.Type == TradePacketType.INITIATE)
            {
                tradeRoomId = (int)incomingPacket.Object;
            }
            else if(incomingPacket.Type == TradePacketType.SETPOKEMON)
            {
                //The other player has set a Pokemon
                //There should be a method to call to display it on screen
            }
            else if(incomingPacket.Type == TradePacketType.CONFIRM)
            {
                //Swap the Pokemon that this player has set to the one that's just been send
            }
        }

        public static void InitiateTrade()
        {
            Packet iniateTrade = new Packet(TradePacketType.INITIATE);
            NetworkManager.Send(iniateTrade);
        }

        public static void SetPokemon(SeriPokemon pokemonToSet)
        {
            Packet setPokemon = new Packet(pokemonToSet, tradeRoomId);
            NetworkManager.Send(setPokemon);
        }
    }
}

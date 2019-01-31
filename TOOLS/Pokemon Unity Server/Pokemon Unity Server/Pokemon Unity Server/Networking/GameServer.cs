using PokemonUnity.Networking.Server.Classes;
using System.Collections.Generic;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Server.Trading;
using System.Net;

namespace PokemonUnity.Networking.Server
{
    static class GameServer
    {
        public static List<Player> Players = new List<Player>();
        public static List<TradeRoom> TradeRooms = new List<TradeRoom>();

        public static Player AddPlayer(/*NetworkProfile profile*/ string userName)
        {
            Player newPlayer = new Player(userName);
            Players.Add(newPlayer);
            return newPlayer;
        }

        public static void HandlePacket(OutgoingPacket packet, IPEndPoint endPoint)
        {

        }

        private static void InitiateTrade(IPEndPoint endPoint)
        {
            TradeRoom tradeRoom = new TradeRoom(endPoint);
            TradeRooms.Add(tradeRoom);
        }
    }
}
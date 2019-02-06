using PokemonUnity.Networking.Server.Classes;
using System.Collections.Generic;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.PacketContainers;
using PokemonUnity.Networking.Server.Trading;
//using PokemonUnity.Networking.Server.Battling;
using System.Net;
using System.Linq;

namespace PokemonUnity.Networking.Server
{
    static class GameServer
    {
        public static List<Player> Players = new List<Player>();
        public static List<TradeRoom> TradeRooms = new List<TradeRoom>();
        //public static List<BattleRoom> BattleRooms = new List<BattleRoom>();

        public static Player AddPlayer(/*NetworkProfile profile*/ string userName)
        {
            Player newPlayer = new Player(userName);
            Players.Add(newPlayer);
            return newPlayer;
        }

        public static void HandlePacket(Packet packet, IPEndPoint endPoint)
        {
            switch (packet.PacketType)
            {
                case PacketTypes.TRADE:
                    TradePacket tradePacket = (TradePacket)packet.Message;
                    if(tradePacket.Type == TradePacketType.INITIATE)
                    {
                        InitiateTrade(endPoint);
                    }
                    else
                    {
                        TradeRooms.First(x => x.roomId == (int)tradePacket.Object).HandleTradePacket(tradePacket, endPoint);
                    }
                    break;
                case PacketTypes.BATTLE:
                    //Handle incoming Battle packets by casting the packet.Message to a (BattlePacket)
                    break;
            }
        }

        private static void InitiateTrade(IPEndPoint endPoint)
        {
            TradeRoom tradeRoom = new TradeRoom(endPoint);
            TradeRooms.Add(tradeRoom);
        }
    }
}
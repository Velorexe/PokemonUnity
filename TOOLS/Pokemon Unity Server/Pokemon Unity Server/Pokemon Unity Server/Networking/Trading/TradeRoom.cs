using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using PokemonUnity.Networking.Packets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using PokemonUnity.Networking.Server.Classes;
using System.Linq;
using PokemonUnity.Networking.Packets.Outgoing;

namespace PokemonUnity.Networking.Server.Trading
{
    public class TradeRoom : IDisposable
    {
        public int roomId;

        private IPEndPoint host;
        private IPEndPoint player2;

        public TradeRoom(IPEndPoint hostEndPoint)
        {
            host = hostEndPoint;
            roomId = GameServer.TradeRooms.Max(x => x.roomId) + 1;
        }

        private void ConnectPlayer2(IPEndPoint player)
        {
            player2 = player;
        }

        public void HandleTradePacket(OTradePacket incomingPacket, IPEndPoint endPoint)
        {

        }

        public void Dispose()
        {

        }
    }
}

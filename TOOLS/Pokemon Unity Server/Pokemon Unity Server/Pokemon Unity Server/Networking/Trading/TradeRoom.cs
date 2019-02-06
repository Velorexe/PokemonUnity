using System;
using System.Net;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.PacketContainers;
using System.Linq;

namespace PokemonUnity.Networking.Server.Trading
{
    public class TradeRoom : IDisposable
    {
        public int roomId;

        private IPEndPoint host;
        private IPEndPoint player2;

        private bool player1Confirmed = false;
        private bool player2Confirmed = false;

        public TradeRoom(IPEndPoint hostEndPoint)
        {
            host = hostEndPoint;
            roomId = GameServer.TradeRooms.Max(x => x.roomId) + 1;
        }

        private void ConnectPlayer2(IPEndPoint player)
        {
            player2 = player;
        }

        public void HandleTradePacket(TradePacket incomingPacket, IPEndPoint endPoint)
        {

        }

        public void Dispose()
        {

        }
    }
}

using PokemonUnity.Networking.Packets.Outgoing;

namespace PokemonUnity.Networking.Packets
{
    [System.Serializable]
    public class OutgoingPacket
    {
        public OutgoingPacketType Type;
        public IOutPacket PacketContainer;
        public System.DateTime Time = System.DateTime.Now;

        #region Constructors

        #region NetworkProfile
        public OutgoingPacket(string ipAdress, int port)
        {
            Type = OutgoingPacketType.CONNECTION;
            PacketContainer = new ONetworkProfile(ipAdress, port);
        }
        #endregion

        #region Trading
        /// <summary>
        /// Creates a new empty Outgoing Trade Packet with the set TradeCommand
        /// </summary>
        /// <param name="tradeCommand"></param>
        public OutgoingPacket(TradeCommand tradeCommand)
        {
            Type = OutgoingPacketType.TRADE;
            PacketContainer = new OTradePacket(tradeCommand, "");
        }
        #endregion

        #endregion
    }

    public interface IOutPacket { };

    [System.Serializable]
    public enum OutgoingPacketType
    {
        TRADE,
        BATTLE,
        AUTH,
        CONNECTION
    }
}
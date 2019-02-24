using System;

namespace PokemonUnity.Networking.Server.Packets.PacketContainers
{
    [Serializable]
    public class BattlePacket
    {
        public BattlePacketType Type;
        public int BattleRoomID;
        //Should become SeriPokemon in the future
        public object Object;

        public BattlePacket()
        {
            Type = BattlePacketType.INITIATE;
        }

        public BattlePacket(BattlePacketType type, int battleRoomID)
        {
            Type = type;
            BattleRoomID = battleRoomID;
        }

        public BattlePacket(object serializedMove, int battleRoomID)
        {
            Type = BattlePacketType.MOVE;
            Object = serializedMove;
            BattleRoomID = battleRoomID;
        }
    }

    [Serializable]
    public enum BattlePacketType
    {
        INITIATE,
        MOVE,
        CONFIRM
    }
}

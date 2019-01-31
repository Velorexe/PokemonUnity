using PokemonUnity.Server.Networking.Classes;
using System.Collections.Generic;

namespace PokemonUnity.Server.Networking
{
    static class GameServer
    {
        public static List<Player> Players = new List<Player>();

        public static Player AddPlayer(/*NetworkProfile profile*/ string userName)
        {
            Player newPlayer = new Player(userName);
            Players.Add(newPlayer);
            return newPlayer;
        }
    }
}

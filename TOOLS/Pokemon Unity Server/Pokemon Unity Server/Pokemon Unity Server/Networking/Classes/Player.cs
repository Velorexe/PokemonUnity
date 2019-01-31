using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonUnity.Networking.Server.Classes
{
    class Player
    {
        public Token Token = new Token();
        public string Name;

        //public PlayerProfile Profile;
        public Player(string name)
        {
            Name = name;
        }
    }
}

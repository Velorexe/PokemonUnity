using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonUnity.Server.Networking.Classes
{
    class Token
    {
        private string playerToken;

        public Token()
        {
            GenerateUniqueToken(GameServer.Players);
        }

        public void GenerateUniqueToken(List<Player> players)
        {
            NewToken();
            while(playerToken != players.Select(x => x.Token.GetToken()).First())
            {
                NewToken();
            }
        }

        private void NewToken()
        {
            char[] token = new char[20];

            Random letter = new Random();
            Random capitalLetter = new Random();

            for (int i = 0; i < token.Length; i++)
            {
                token[i] = (char)letter.Next(97, 123);
                if(capitalLetter.Next(0,2) == 0)
                {
                    token[i] = char.ToUpper(token[i]);
                }
            }

            playerToken = token.ToString();
        }

        public string GetToken()
        {
            return playerToken;
        }
    }
}

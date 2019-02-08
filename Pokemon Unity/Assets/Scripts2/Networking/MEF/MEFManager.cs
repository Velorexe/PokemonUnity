using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using PokemonUnity.Saving.SerializableClasses;
using PokemonUnity.Networking.MEF.Handlers;
using System.Reflection;

namespace PokemonUnity.Networking.MEF
{
    public static class MEFManager
    {
        private static IEnumerable<Lazy<SeriPokemon>> importPokemon;
        private static IEnumerable<Lazy<SeriMove>> importMoves;

        //The location where the dll's are saved
        //I'd like to keep this in a certain way so that Pokemon and Moves are seperated
        /*
           PokemonUnity/
           ├── Imported/
           │   ├── Moves
           │   │   ├── FireBlastonian.dll
           │   │   ├── TidalWave.dll
           │   ├── Pokemon
           │   │   ├── Pritonia.dll
           │   │   ├── Starling.dll
        */
        private const string folderLocation = "";

        public static void Import()
        {
            Import mefImporter = new Import(folderLocation);

            mefImporter.ImportPokemon();
            mefImporter.ImportMoves();
        }
    }
}

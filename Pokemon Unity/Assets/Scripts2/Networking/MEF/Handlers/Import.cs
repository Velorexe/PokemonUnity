using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using PokemonUnity.Saving.SerializableClasses;

namespace PokemonUnity.Networking.MEF.Handlers
{
    public class Import
    {
        [ImportMany(typeof(SeriPokemon))]
        private IEnumerable<Lazy<SeriPokemon>> PokemonOperations { get; set; }
        [ImportMany(typeof(SeriMove))]
        private IEnumerable<Lazy<SeriMove>> MoveOperations { get; set; }


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
        private string FolderLocation { get; set; }

        public Import(string folderLocation)
        {
            FolderLocation = folderLocation;
        }

        public void ImportPokemon()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new DirectoryCatalog(FolderLocation + @"/Pokemon"));
            CompositionContainer container = new CompositionContainer(catalog);

            container.ComposeParts(this);
        }

        public void ImportMoves()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new DirectoryCatalog(FolderLocation + @"/Moves"));
            CompositionContainer container = new CompositionContainer(catalog);

            container.ComposeParts(this);
        }

        public int AvaiableNumberOfPokemon { get { return PokemonOperations != null ? PokemonOperations.Count() : 0; } }
        public int AvaiableNumberOfMoves { get { return MoveOperations != null ? MoveOperations.Count() : 0; } }
    }
}

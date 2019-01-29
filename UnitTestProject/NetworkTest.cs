using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using PokemonUnity.Networking;

namespace Tests
{
    [TestClass]
    public class NetworkTest
    {
        [TestMethod]
        public void Start_NetworkManager()
        {
            NetworkManager.Start();
        }

        [TestMethod]
        public void NetworkManager_Is_Running()
        {
            NetworkManager.Start();
            Assert.IsTrue(NetworkManager.IsRunning);
        }

        [TestMethod]
        public void NetworkManager_Has_Connection()
        {
            NetworkManager.Start();
            Thread.Sleep(5000);
            Assert.IsTrue(NetworkManager.IsConnected());
        }
    }
}

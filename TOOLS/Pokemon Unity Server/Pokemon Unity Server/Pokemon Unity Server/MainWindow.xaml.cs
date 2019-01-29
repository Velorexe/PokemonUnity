using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.Incoming;
using PokemonUnity.Networking.Packets.Outgoing;
using System.Reflection;
using Pokemon_Unity_Server.Networking;

namespace Pokemon_Unity_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConsoleText.Document.Blocks.Clear();

            StartServer.Click += new RoutedEventHandler(StartServer_Click);
        }

        private void StartServer_Click(object sender, EventArgs e)
        {
            if (Server.IsRunning)
            {
                Server.Stop();
                StartServer.Content = "Start Server";
            }
            else
            {
                Server.Start();
                StartServer.Content = "Stop Server";
            }
        }
    }
}

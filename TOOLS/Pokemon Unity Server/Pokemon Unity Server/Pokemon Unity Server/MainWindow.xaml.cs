using System;
using System.Windows;
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

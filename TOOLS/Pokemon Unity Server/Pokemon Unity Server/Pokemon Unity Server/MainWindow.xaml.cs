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

namespace Pokemon_Unity_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int udpPort = 4568;
        private bool isRunning = false;

        private UdpClient udpSocket;
        private Thread udpListener;

        public MainWindow()
        {
            InitializeComponent();
            ConsoleText.Document.Blocks.Clear();

            StartServer.Click += new RoutedEventHandler(StartServer_Click);
            udpListener = new Thread(new ThreadStart(StartListening))
            {
                IsBackground = true
            };
        }

        private void StartServer_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                StartServer.Content = "Start Server";
                udpListener.Abort();
                udpListener = new Thread(new ThreadStart(StartListening))
                {
                    IsBackground = true
                };
                udpSocket.Close();
                isRunning = false;
                AddConsoleText("Stopping server");
            }
            else
            {
                StartServer.Content = "Close Server";
                udpListener.Start();
                isRunning = true;
            }
        }

        /// <summary>
        /// Adds a new line to the Console and automatically creates a new line.
        /// </summary>
        /// <param name="data">The data that needs to be written.</param>
        private void AddConsoleText(string data)
        {
            ConsoleText.AppendText(data);
            ConsoleText.AppendText(Environment.NewLine);

            ConsoleText.ScrollToEnd();
        }

        private void StartListening()
        {
            IPHostEntry localHostEntry;
            try
            {
                //udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket = new UdpClient(udpPort);
                try
                {
                    if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    {
                        localHostEntry = Dns.GetHostEntry(GetLocalIPAddress());

                        Application.Current.Dispatcher.Invoke((() =>
                        {
                            AddConsoleText("Attempting to start the server on " + localHostEntry.HostName);
                        }));
                        udpSocket.BeginReceive(new AsyncCallback(PacketHandler), udpSocket);

                        IPEndPoint localIpEndPoint = udpSocket.Client.LocalEndPoint as IPEndPoint;
                        Application.Current.Dispatcher.Invoke((() =>
                        {
                            AddConsoleText("Started the server on " + GetLocalIPAddress() + " : " + localIpEndPoint.Port);
                        }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((() =>
                        {
                            AddConsoleText("There's no internet connection avaiable, please try again later.");
                        }));
                    }

                }
                catch (SocketException e)
                {
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        AddConsoleText("Couldn't start Socket, please check if your internet drivers are correctly installed.");
                        AddConsoleText(e.ToString());
                    }));
                }
            }
            catch (SocketException e)
            {
                Application.Current.Dispatcher.Invoke((() =>
                {
                    AddConsoleText(e.ToString());
                }));
            }
        }

        private void PacketHandler(IAsyncResult result)
        {
            try
            {
                UdpClient socket = result.AsyncState as UdpClient;
                IPEndPoint source = new IPEndPoint(0, 0);

                byte[] message = socket.EndReceive(result, ref source);
                AsyncIncomingPacketHandler(message, source);

                Application.Current.Dispatcher.Invoke((() =>
                {
                    AddConsoleText("Got " + message.Length + " bytes from " + source);
                }));

                socket.BeginReceive(new AsyncCallback(PacketHandler), socket);
            }
            catch(ObjectDisposedException)
            {
                Application.Current.Dispatcher.Invoke((() =>
                {
                    AddConsoleText("Server stopped");
                }));
            }
        }

        private async void AsyncIncomingPacketHandler(byte[] message, IPEndPoint endPoint)
        {
            OutgoingPacket incomingPacket;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(message, 0, message.Length);
                memoryStream.Position = 0;

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Binder = new CustomizedBinder();

                incomingPacket = (OutgoingPacket)formatter.Deserialize(memoryStream);
            }
            await Task.Run(() => ReadIncomingPacket(incomingPacket, endPoint));
        }

        private void ReadIncomingPacket(OutgoingPacket incomingPacket, IPEndPoint endPoint)
        {
            if (incomingPacket.Type == OutgoingPacketType.CONNECTION)
            {
                ///Here we can log in the user
                ///Then we send a confirmation back
                IncomingPacket outgoingPacket = new IncomingPacket
                {
                    Type = IncomingPacketType.CONNECTION,
                    PacketContainer = new IReceivedConnection() { IsAccepted = true }
                };
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, outgoingPacket);

                    byte[] serializedData = memoryStream.ToArray();
                    udpSocket.Send(serializedData, serializedData.Length, endPoint);
                }
                Application.Current.Dispatcher.Invoke((() =>
                {
                    AddConsoleText("Received Conection call, returned: " + ((IReceivedConnection)outgoingPacket.PacketContainer).IsAccepted);
                }));
            }
        }

        sealed class CustomizedBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type returntype = null;
                string sharedAssemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                typeName = typeName.Replace(sharedAssemblyName, assemblyName);
                returntype =
                        Type.GetType(string.Format("{0}, {1}",
                        typeName, assemblyName));

                return returntype;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                base.BindToName(serializedType, out assemblyName, out typeName);
                assemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            }
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            AddConsoleText("No network adapters with an IPv4 address in the system.");
            return string.Empty;
        }
    }
}

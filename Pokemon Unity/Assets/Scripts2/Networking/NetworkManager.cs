using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using PokemonUnity.Saving;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.Incoming;
using System.Collections.Generic;
using System.Linq;

namespace PokemonUnity.Networking
{
    /// <summary>
    /// Manages the Network part of Pokemon Unity. Regulates incoming and outgoing packets.
    /// </summary>
    public static class NetworkManager
    {
        private static string authToken;
        private static bool isAuth;

        private const string encryptionKey = "pku123";
        private const string ipAdress = "192.168.0.46";
        private const int port = 4568;

        private const int maxByteBuffer = 1024;

        private static UdpClient client;
        private static IPEndPoint ipEndPoint;

        public static bool IsRunning = false;
        private static bool hasConnection = false;

        private static Queue<OutgoingPacket> packetStack = new Queue<OutgoingPacket>();

        /// <summary>
        /// Starts the server and sends a ping to the server to authenticate this user.
        /// </summary>
        public static void Start()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(ipAdress);

            /// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            /// an exception that occurs when the host IP Address is not compatible with the address family
            /// (typical in the IPv6 case).
            foreach (IPAddress adress in hostEntry.AddressList)
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);
                UdpClient tempSocket = new UdpClient(endPoint.AddressFamily);

                tempSocket.Connect(endPoint);
                if (tempSocket.Client.Connected)
                {
                    client = tempSocket;
                    ipEndPoint = endPoint;
                    IsRunning = true;
                    break;
                }
                else
                {
                    continue;
                }
            }

            RequestConnection();

            ///Here we create a new Thread
            ///That way it can stay on the background on a new Thread
            Thread listeningThread = new Thread(BackgroundListener)
            {
                IsBackground = true
            };
            listeningThread.Start();
        }

        /// <summary>
        /// Listens to the incoming packets. Should be ran on a new thread.
        /// </summary>
        private static void BackgroundListener()
        {
            bool isListening = false;
            if (client.Client.Connected)
            {
                isListening = true;
            }

            try
            {
                while (isListening)
                {
                    byte[] collectedBytes = client.Receive(ref ipEndPoint);
                    IncomingPacket collectedPacket;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.Write(collectedBytes, 0, collectedBytes.Length);
                        memoryStream.Position = 0;
                        BinaryFormatter formatter = new BinaryFormatter();
                        collectedPacket = (IncomingPacket)formatter.Deserialize(memoryStream);
                    }

                    if (hasConnection)
                    {
                        if (!isAuth)
                        {
                            if (collectedPacket.Type == IncomingPacketType.AUTH)
                            {
                                IAuthenticatePacket token = (IAuthenticatePacket)collectedPacket.PacketContainer;
                                switch (token.Authenticated)
                                {
                                    case IAuthenticatePacket.AuthOptions.SUCCES:
                                        isAuth = true;
                                        authToken = token.AuthenticationKey;
                                        EmptyOutgoingStack();
                                        break;
                                    case IAuthenticatePacket.AuthOptions.FAILED:
                                        isAuth = false;
                                        authToken = string.Empty;
                                        //Failed Authentication
                                        break;
                                    case IAuthenticatePacket.AuthOptions.ERROR:
                                        isAuth = false;
                                        authToken = string.Empty;
                                        //Error Handling
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                //Server is sending useless data
                                //Should be discarded
                                //Possible cheater or server is not doing it's job correctly
                            }
                        }
                        else
                        {
                            if (collectedPacket.Type == IncomingPacketType.TRADE)
                            {
                                //Pass data through to TradeManager
                            }
                            else if (collectedPacket.Type == IncomingPacketType.BATTLE)
                            {
                                //Pass data through to OnlineBattleManager
                            }
                        }
                    }
                    else
                    {
                        IReceivedConnection receivedConnection = (IReceivedConnection)collectedPacket.PacketContainer;
                        hasConnection = receivedConnection.IsAccepted;

                        if (hasConnection)
                        {
                            Authenticate();
                        }
                    }
                }
            }
            catch (Exception)
            {
                Disconnect();
                IsRunning = false;
                isAuth = false;
            }
        }

        private static void RequestConnection()
        {
            IPEndPoint localEndPoint = client.Client.LocalEndPoint as IPEndPoint;
            OutgoingPacket networkProfile = new OutgoingPacket(localEndPoint.Address.ToString(), localEndPoint.Port);

            using(MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, networkProfile);

                byte[] serializedData = memoryStream.ToArray();
                while (serializedData.Length != 0)
                {
                    byte[] bufferBytes;
                    if (serializedData.Length >= maxByteBuffer)
                    {
                        bufferBytes = serializedData.Take(maxByteBuffer).ToArray();
                        serializedData = RemoveAt(serializedData, 0, maxByteBuffer);
                    }
                    else
                    {
                        bufferBytes = serializedData.Take(serializedData.Length).ToArray();
                        serializedData = RemoveAt(serializedData, 0, serializedData.Length);
                    }
                    client.Send(bufferBytes, bufferBytes.Length);
                }
            }
        }

        private static void Authenticate()
        {
            //SaveData authData = SaveManager.GetActiveSave();
            SaveData authData = SaveManager.GetSave(0);
            OutgoingPacket authPacket = new OutgoingPacket(authData); 

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, authPacket);

                byte[] serializedData = memoryStream.ToArray();
                while(serializedData.Length != 0)
                {
                    byte[] bufferBytes;
                    if (serializedData.Length >= 1024)
                    {
                        bufferBytes  = serializedData.Take(1024).ToArray();
                        serializedData = RemoveAt(serializedData, 0, 1024);
                    }
                    else
                    {
                        bufferBytes = serializedData.Take(serializedData.Length).ToArray();
                        serializedData = RemoveAt(serializedData, 0, serializedData.Length);
                    }
                    client.Send(bufferBytes, bufferBytes.Length);
                }
            }
        }

        public static T[] RemoveAt<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (length < 0)
            {
                startIndex += 1 + length;
                length = -length;
            }

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException("length");

            T[] newArray = new T[array.Length - length];

            Array.Copy(array, 0, newArray, 0, startIndex);
            Array.Copy(array, startIndex + length, newArray, startIndex, array.Length - startIndex - length);

            return newArray;
        }

        /// <summary>
        /// Sends an OutgoingPacket to the designated Server
        /// </summary>
        /// <param name="outgoingPacket">The OutgoingPacket that contains the data that needs to be send</param>
        public static void Send(OutgoingPacket outgoingPacket)
        {
            if (isAuth)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, outgoingPacket);

                    byte[] serializedData = memoryStream.ToArray();
                    client.Send(serializedData, serializedData.Length);
                }
            }
            else
            {
                if (!IsRunning)
                {
                    Start();
                }
                else
                {
                    ///Wait until the authentication is complete
                }
                packetStack.Enqueue(outgoingPacket);
            }
        }

        private static void EmptyOutgoingStack()
        {
            while(packetStack.Count != 0)
            {
                OutgoingPacket outgoingPacket = packetStack.Dequeue();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, outgoingPacket);

                    byte[] serializedData = memoryStream.ToArray();
                    client.Send(serializedData, serializedData.Length);
                }
            }
        }

        /// <summary>
        /// Disconnects the UDP Client from the Server Socket (if the connection was made)
        /// </summary>
        public static void Disconnect()
        {
            if (client.Client.Connected)
            {
                client.Client.Disconnect(true);
                client.Close();
                client = null;
            }
        }

        /// <summary>
        /// Returns a bool that indicates if the Client is Authenticated to send data to the server.
        /// </summary>
        /// <returns>true if the user is authenticated, false if it's pending or rejected</returns>
        public static bool IsAuthenticated()
        {
            return isAuth;
        }
    }
}
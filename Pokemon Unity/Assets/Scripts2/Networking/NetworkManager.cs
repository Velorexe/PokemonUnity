using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PokemonUnity.Saving;
using PokemonUnity.Networking.Packets;
using PokemonUnity.Networking.Packets.PacketContainers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        private const string address = "herbertmilhomme.com";
        private const int port = 4568;

        private const int maxByteBuffer = 1024;

        private static UdpClient client;
        private static IPEndPoint ipEndPoint;

        public static bool IsRunning = false;

        private static Queue<Packet> packetStack = new Queue<Packet>();

        /// <summary>
        /// Starts the server and sends a ping to the server to authenticate this user.
        /// </summary>
        public static void Start()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(address);

            /// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            /// an exception that occurs when the host IP Address is not compatible with the address family
            /// (typical in the IPv6 case).
            foreach (IPAddress adress in hostEntry.AddressList)
            {
                IPEndPoint endPoint = new IPEndPoint(adress, port);
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
                    Packet receivedPacket = collectedBytes;

                    if (!isAuth)
                    {
                        if (receivedPacket.PacketType == PacketTypes.AUTH)
                        {
                            AuthenticationPacket authPacket = (AuthenticationPacket)receivedPacket.Message;
                            if (string.IsNullOrEmpty(authPacket.Token))
                            {
                                isAuth = true;
                                authToken = authPacket.Token;
                            }
                            else
                            {
                                isAuth = false;
                                authToken = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        if (receivedPacket.PacketType == PacketTypes.TRADE)
                        {
                            //Pass data through to TradeManager
                            TradeManager.ReceivePacket((TradePacket)receivedPacket.Message);
                        }
                        else if (receivedPacket.PacketType == PacketTypes.BATTLE)
                        {
                            //Pass data through to OnlineBattleManager
                        }
                    }
                }
            }
            catch (SocketException)
            {
                Disconnect();
                IsRunning = false;
                isAuth = false;
            }
        }

        private static void RequestConnection()
        {
            int playerTrainerID = 50;
            Packet loginPacket = new Packet(playerTrainerID.ToString(), "test-password");

            byte[] serializedData = loginPacket;
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

        private static void UploadSaveData()
        {
            //SaveData authData = SaveManager.GetActiveSave();
            SaveData authData = SaveManager.GetSave(0);
            Packet authPacket = new Packet(authData);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, authPacket);

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
        /// Sends an Packet to the designated Server
        /// </summary>
        /// <param name="Packet">The Packet that contains the data that needs to be send</param>
        public static void Send(Packet Packet)
        {
            if (isAuth)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, Packet);

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
                packetStack.Enqueue(Packet);
            }
        }

        private static void EmptyOutgoingStack()
        {
            while (packetStack.Count != 0)
            {
                Packet Packet = packetStack.Dequeue();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, Packet);

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
    }
}
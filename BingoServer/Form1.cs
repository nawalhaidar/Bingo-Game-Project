using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace BingoServer
{
    public partial class Form1 : Form
    {
        private Socket serverSocket;
        private List<Thread> clientThreads = new List<Thread>();
        private List<Socket> clients = new List<Socket>();
        private List<NetworkStream> streams = new List<NetworkStream>();
        private bool gameEnded = false;
        private int currentTurn = 0;
        private static Mutex mutex = new Mutex();
        private int numberOfPlayers;

        public Form1()
        {
            GetNumberOfPlayers();
            InitializeComponent(numberOfPlayers);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            StartServer();
        }

        private void GetNumberOfPlayers()
        {
            while (true)
            {
                string userInput = Interaction.InputBox("Enter the number of players:", "Input Dialog", "");
                if (!string.IsNullOrEmpty(userInput))
                {
                    try
                    {
                        numberOfPlayers = int.Parse(userInput);
                        break; // Exit the loop if input is valid
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Handle case where user cancels input box
                    break;
                }
            }
        }

        private async void StartServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5001));
            serverSocket.Listen(10);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                var clientThread = new Thread(HandleClient);
                clientThread.Start();
                clientThreads.Add(clientThread);
            }
            await Task.Run(() => WaitForPlayers());

            // Notify players that the game is starting
            SendMessageToStream(0, "START");
        }

        private void WaitForPlayers()
        {
            while (clients.Count < numberOfPlayers) ;
        }

        private void HandleClient()
        {
            try
            {
                Socket client = serverSocket.Accept();
                mutex.WaitOne();
                clients.Add(client);
                streams.Add(new NetworkStream(client));
                mutex.ReleaseMutex();

                string message = "PLAYER:" + (clients.Count);
                SendMessageToStream(clients.Count - 1, message);

                ListenForClientMessages(clients.Count - 1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ListenForClientMessages(int index)
        {
            while (!gameEnded)
            {
                try
                {
                    byte[] buffer = new byte[256];
                    int bytesRead = await streams[index].ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        HandleClientMessage(index, message);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void HandleClientMessage(int index, string message)
        {
            if (message.StartsWith("CHAT:"))
            {
                BroadcastMessage(message);
            }
            else if (message.StartsWith("RESULT:"))
            {
                string result = message.Split(':')[1];
                // Update UI or handle result
            }
            else
            {
                BroadcastMessageExceptStream(index, message);
                SwitchTurns();
            }
        }

        private void SendMessageToStream(int index, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            streams[index].Write(data, 0, data.Length);
        }

        private void BroadcastMessage(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            foreach (var stream in streams)
            {
                stream.Write(data, 0, data.Length);
            }
        }

        private void BroadcastMessageExceptStream(int index, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            for (int i = 0; i < streams.Count; i++)
            {
                if (i != index)
                {
                    streams[i].Write(data, 0, data.Length);
                }
            }
        }

        private void SwitchTurns()
        {
            currentTurn = (currentTurn + 1) % numberOfPlayers;
            BroadcastTurn();
        }

        private void BroadcastTurn()
        {
            string turnMessage = "TURN:" + (currentTurn + 1);
            byte[] data = Encoding.ASCII.GetBytes(turnMessage);
            foreach (var stream in streams)
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }
}

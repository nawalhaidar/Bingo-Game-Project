using System;
using System.Collections.Generic;
using System.Drawing;
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
        private TcpListener server;
        private List<Thread> clientThreads = new List<Thread>();
        private List<TcpClient> clients = new List<TcpClient>();
        private List<NetworkStream> streams = new List<NetworkStream>();
        private bool gameEnded = false;
        private int currentTurn = 0;
        private static Mutex mutex = new Mutex();
        private int numberOfPlayers;

        public Form1()
        {
            GetNumberOfPlayers();
            InitializeComponent(numberOfPlayers);
            StartServer();
        }

        private void GetNumberOfPlayers()
        {
            string userInput = Interaction.InputBox("Enter the number of players:", "Input Dialog", "");
            if (!string.IsNullOrEmpty(userInput))
            {
                try
                {
                    numberOfPlayers = int.Parse(userInput);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GetNumberOfPlayers();
                }
            }
            else
            {
                GetNumberOfPlayers();
            }
        }

        private async void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5001);
            server.Start();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                var clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(i);
                clientThreads.Add(clientThread);
            }
            await Task.Run(() => WaitForPlayers());

            // Notify players that the game is starting
            sendMessageToStream(0,"START");
        }

        private void WaitForPlayers()
        {
            while (clients.Count() < numberOfPlayers) ;
        }

        private void HandleClient(object indexObj)
        {
            // try
            // {
                int index = (int)indexObj;
                var client = server.AcceptTcpClient();
                mutex.WaitOne();
                clients.Add(client);
                streams.Add(client.GetStream());
                mutex.ReleaseMutex();

                string message = "PLAYER:" + (index + 1);
                sendMessageToStream(index, message);

                ListenForClientMessages(index);
            // }
            // catch (Exception e)
            // {
            //     MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // }
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
                string[] messageParts = message.Split(':');
                string displayMessage = messageParts[1] + ": " + messageParts[2];
                chatLabel.Text += "\n" + displayMessage;
            }
            else if (message.StartsWith("RESULT:"))
            {
                this.scoreLabels[index].Text = message.Split(':')[1];
            }
            else
            {
                BroadcastMessage(message);
                SwitchTurns();
            }
        }

        private void sendMessageToStream(int index, string message)
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

        private void SwitchTurns()
        {
            currentTurn = (currentTurn + 1) % numberOfPlayers;
            foreach (Button button in buttons)
            {
                button.Enabled = (currentTurn == 0) && (button.BackColor != Color.Red);
            }
            BroadcastTurn();
        }

        private void BroadcastTurn()
        {
            string turnMessage = "TURN:" + (currentTurn+1);
            MessageBox.Show(turnMessage,"turn",MessageBoxButtons.OKCancel,MessageBoxIcon.None);
            byte[] data = Encoding.ASCII.GetBytes(turnMessage);
            foreach (var stream in streams)
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }
}

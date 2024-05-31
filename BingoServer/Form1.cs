using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingoServer
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>();
        private List<NetworkStream> streams = new List<NetworkStream>();
        // private Button[,] buttons = new Button[5, 5];
        private int[] numbers = { 23, 5, 17, 1, 11, 19, 4, 21, 14, 2, 24, 8, 13, 7, 20, 3, 10, 18, 6, 22, 15, 9, 25, 12, 16 };
        private bool gameEnded = false;
        private int prevCount = 0, newCount = 0;
        private int currentTurn = 0; // 0: server, 1: client1, 2: client2

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid();
            StartServer();
        }
        
        private void InitializeGrid()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    buttons[i, j].Text = numbers[i * 5 + j].ToString();
                    buttons[i, j].Enabled = true;
                    buttons[i, j].Font = new Font(this.buttons[i, j].Font, FontStyle.Bold);
                }
            }
        }


        private async void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            for (int i = 0; i < 2; i++)
            {
                var client = await server.AcceptTcpClientAsync();
                clients.Add(client);
                streams.Add(client.GetStream());

                // Send the player number to the client
                string playerNumberMessage = "PLAYER:" + (i + 1);
                byte[] data = Encoding.ASCII.GetBytes(playerNumberMessage);
                streams[i].Write(data, 0, data.Length);
            }

            await Task.Run(() => ListenForMessages());
        }

        private async Task ListenForMessages()
        {
            byte[] buffer = new byte[256];
            while (!gameEnded)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    int bytesRead = await streams[i].ReadAsync(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Ignore messages from clients that are not their turn
                    if (i + 1 != currentTurn)
                    {
                        continue;
                    }

                    MarkNumberOnGrid(message);
                    BroadcastMessage(message);

                    prevCount = newCount;
                    newCount = CheckForWin();

                    if (newCount > prevCount)
                    {
                        bingoLabel.Text = newCount switch
                        {
                            1 => "B",
                            2 => "BI",
                            3 => "BIN",
                            4 => "BING",
                            5 => "BINGO",
                            _ => bingoLabel.Text
                        };
                    }
                    if (newCount >= 5)
                    {
                        gameEnded = true;
                        MessageBox.Show("Congratulations! You won!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BroadcastMessage("You lost");
                        break;
                    }
                    SwitchTurns();
                }
            }
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
            currentTurn = (currentTurn + 1) % 3; // Rotate turns among server (0), client1 (1), and client2 (2)
            foreach (Button button in buttons)
            {
                button.Enabled = (currentTurn == 0) && (button.BackColor != Color.Red);
            }
            BroadcastTurn(); // Notify clients whose turn it is
        }

        private void BroadcastTurn()
        {
            string turnMessage = "TURN:" + currentTurn;
            byte[] data = Encoding.ASCII.GetBytes(turnMessage);
            foreach (var stream in streams)
            {
                stream.Write(data, 0, data.Length);
            }
        }

        private void MarkNumberOnGrid(string number)
        {
            this.Invoke(new Action(() =>
            {
                foreach (Button button in buttons)
                {
                    if (button.Text == number)
                    {
                        button.BackColor = Color.Red;
                        button.Enabled = false;
                        break;
                    }
                }
            }));
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (gameEnded || currentTurn != 0) return;

            Button button = sender as Button;
            if (button != null)
            {
                button.BackColor = Color.Red;
                button.Enabled = false;

                string message = button.Text;
                byte[] data = Encoding.ASCII.GetBytes(message);
                BroadcastMessage(message);

                prevCount = newCount;
                newCount = CheckForWin();

                if (newCount > prevCount)
                {
                    bingoLabel.Text = newCount switch
                    {
                        1 => "B",
                        2 => "BI",
                        3 => "BIN",
                        4 => "BING",
                        5 => "BINGO",
                        _ => bingoLabel.Text
                    };
                }
                if (newCount >= 5)
                {
                    MessageBox.Show("Congratulations! You won!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BroadcastMessage("You lost");
                    gameEnded = true;
                    foreach (var btn in buttons)
                    {
                        btn.Enabled = false;
                    }
                }
                SwitchTurns();
            }
        }

        private int CheckForWin()
        {
            int totalCount = CheckRowsForWin() + CheckColumnsForWin() + CheckDiagonalsForWin();
            return totalCount;
        }

        private int CheckRowsForWin()
        {
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                bool rowComplete = true;
                for (int j = 0; j < 5; j++)
                {
                    if (buttons[i, j].BackColor != Color.Red)
                    {
                        rowComplete = false;
                        break;
                    }
                }
                if (rowComplete)
                {
                    count++;
                }
            }
            return count;
        }

        private int CheckColumnsForWin()
        {
            int count = 0;
            for (int j = 0; j < 5; j++)
            {
                bool columnComplete = true;
                for (int i = 0; i < 5; i++)
                {
                    if (buttons[i, j].BackColor != Color.Red)
                    {
                        columnComplete = false;
                        break;
                    }
                }
                if (columnComplete)
                {
                    count++;
                }
            }
            return count;
        }

        private int CheckDiagonalsForWin()
        {
            int diagonal1Complete = 1;
            int diagonal2Complete = 1;
            for (int i = 0; i < 5; i++)
            {
                if (buttons[i, i].BackColor != Color.Red)
                {
                    diagonal1Complete = 0;
                }
                if (buttons[i, 4 - i].BackColor != Color.Red)
                {
                    diagonal2Complete = 0;
                }
            }
            return diagonal1Complete + diagonal2Complete;
        }
    }
}

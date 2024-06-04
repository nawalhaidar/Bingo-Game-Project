using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingoClient
{
    public partial class Form1 : Form
    {
        private Socket clientSocket;
        private NetworkStream stream;
        private int[] numbers = GenerateRandomOrder(25);
        private bool isClientTurn = false;
        private bool gameEnded = false;
        private int playerNumber; // This client’s player number

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid();
            await ConnectToServer();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    buttons[i, j].Text = numbers[i * 5 + j].ToString();
                    buttons[i, j].Enabled = false;
                    buttons[i, j].Font = new Font(buttons[i, j].Font, FontStyle.Bold);
                }
            }
        }

         private async Task ConnectToServer()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                await clientSocket.ConnectAsync("127.0.0.1", 5001);
                stream = new NetworkStream(clientSocket);
                await Task.Run(() => ListenForMessages());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ListenForMessages()
        {
            byte[] buffer = new byte[256];
            while (!gameEnded)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                HandleServerMessage(message);
            }
        }

        private void HandleServerMessage(string message)
        {
            if (message.StartsWith("PLAYER:"))
            {
                playerNumber = int.Parse(message.Split(':')[1]);
                this.Text = "Player " + playerNumber;
            }
            else if (message == "START" && playerNumber == 1)
            {
                EnableButtons(true);
            }
            else if (message == "LOST")
            {
                MessageBox.Show("Oops :( You lost!", "You lost", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                gameEnded = true;
            }
            else if (message.StartsWith("TURN:"))
            {
                int turn = int.Parse(message.Split(':')[1]);
                isClientTurn = (turn == playerNumber);
                EnableButtons(isClientTurn);
            }
            else if (message.StartsWith("CHAT:"))
            {
                string[] messageParts = message.Split(':');
                string displayMessage = messageParts[1] + ": " + messageParts[2];
                this.Invoke(new Action(() =>
                {
                    chatLabel.Text += "\n" + displayMessage;
                }));
            }
            else
            {
                MarkNumberOnGrid(message);
                UpdateBingoLabel();
                if (bingoLabel.Text == "BINGO" && !gameEnded)
                {
                    MessageBox.Show("Congratulations! You won!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    gameEnded = true;
                }
                SendMessageToServer("RESULT:" + bingoLabel.Text);
            }
        }

         private void SendMessageToServer(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string chatMessage = chatTextBox.Text;
            string message = "CHAT:PLAYER" + playerNumber + ":" + chatMessage;
            SendMessageToServer(message);
            chatTextBox.Text = "";
        }

        private void MarkNumberOnGrid(string number)
        {
            this.Invoke(new Action(() =>
            {
                foreach (Button button in buttons)
                {
                    if (button.Text == number)
                    {
                        button.BackColor = Color.HotPink;
                        button.Enabled = false;
                        break;
                    }
                }
            }));
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                button.BackColor = Color.HotPink;
                button.Enabled = false;

                string message = button.Text;
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                UpdateBingoLabel();

                if (bingoLabel.Text == "BINGO" && !gameEnded)
                {
                    MessageBox.Show("Congratulations! You won!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    byte[] dataa = Encoding.ASCII.GetBytes("WON:" + playerNumber);
                    stream.Write(dataa, 0, dataa.Length);
                    gameEnded = true;
                    foreach (var btn in buttons)
                    {
                        btn.Enabled = false;
                    }
                    gameEnded = true;
                }
                message = "RESULT:" + bingoLabel.Text;
                // MessageBox.Show(message + " sent", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Thread.Sleep(100);
                SendMessageToServer(message);
                
            }
        }

        private void EnableButtons(bool enable)
        {
            foreach (Button button in buttons)
            {
                if (button.BackColor != Color.HotPink)
                {
                    button.Enabled = enable;
                }
            }
        }

        private void UpdateBingoLabel()
        {
            int totalCount = CheckRowsForWin() + CheckColumnsForWin() + CheckDiagonalsForWin();
            bingoLabel.Text = totalCount switch
            {
                1 => "B",
                2 => "BI",
                3 => "BIN",
                4 => "BING",
                5 => "BINGO",
                _ => bingoLabel.Text
            };
        }

        private int CheckRowsForWin()
        {
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                bool rowComplete = true;
                for (int j = 0; j < 5; j++)
                {
                    if (buttons[i, j].BackColor != Color.HotPink)
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
                    if (buttons[i, j].BackColor != Color.HotPink)
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
                if (buttons[i, i].BackColor != Color.HotPink)
                {
                    diagonal1Complete = 0;
                }
                if (buttons[i, 4 - i].BackColor != Color.HotPink)
                {
                    diagonal2Complete = 0;
                }
            }
            return diagonal1Complete + diagonal2Complete;
        }

        static int[] GenerateRandomOrder(int n)
        {
            Random rand = new Random();
            int[] numbers = new int[n];
            for (int i = 0; i < n; i++)
            {
                numbers[i] = i + 1;
            }
            for (int i = n - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                int temp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = temp;
            }
            return numbers;
        }
    }
}

using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingoClient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        // private Button[,] buttons = new Button[5, 5];
        private int[] numbers = { 23, 5, 17, 1, 11, 19, 4, 21, 14, 2, 24, 8, 13, 7, 20, 3, 10, 18, 6, 22, 15, 9, 25, 12, 16 };
        private bool isClientTurn = false;
        private bool gameEnded = false;
        private int prevCount = 0, newCount = 0;
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
                    buttons[i, j].Font = new Font(this.buttons[i, j].Font, FontStyle.Bold);
                }
            }
        }

        private async Task ConnectToServer()
        {
            client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 5000);
            stream = client.GetStream();

            await Task.Run(() => ListenForMessages());
        }

        private async Task ListenForMessages()
        {
            byte[] buffer = new byte[256];
            while (!gameEnded)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (message.StartsWith("PLAYER:"))
                {
                    playerNumber = int.Parse(message.Split(':')[1]);
                }
                else if (message == "You lost")
                {
                    MessageBox.Show("Oops :( You lost!", "You lost", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    gameEnded = true;
                    break;
                }
                else if (message.StartsWith("TURN:"))
                {
                    int turn = int.Parse(message.Split(':')[1]);
                    isClientTurn = (turn == playerNumber);
                    EnableButtons(isClientTurn);
                }
                else
                {
                    MarkNumberOnGrid(message);
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
                        byte[] data = Encoding.ASCII.GetBytes("You lost");
                        stream.Write(data, 0, data.Length);
                        gameEnded = true;
                        break;
                    }
                }
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
            if (gameEnded || !isClientTurn) return;

            Button button = sender as Button;
            if (button != null)
            {
                button.BackColor = Color.Red;
                button.Enabled = false;

                string message = button.Text;
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

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
                    byte[] dataa = Encoding.ASCII.GetBytes("You lost");
                    stream.Write(dataa, 0, dataa.Length);
                    gameEnded = true;
                    foreach (var btn in buttons)
                    {
                        btn.Enabled = false;
                    }
                }
                SwitchTurns();
            }
        }

        private void EnableButtons(bool enable)
        {
            foreach (Button button in buttons)
            {
                if (button.BackColor != Color.Red)
                {
                    button.Enabled = enable;
                }
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

        private void SwitchTurns()
        {
            byte[] data = Encoding.ASCII.GetBytes("TURN:" + ((playerNumber % 2) + 1));
            stream.Write(data, 0, data.Length);
        }
    }
}

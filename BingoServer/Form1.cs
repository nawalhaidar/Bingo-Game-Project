using System;
using System.Drawing;
using System.Linq;
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
        private TcpClient client;
        private NetworkStream stream;
        private Random random = new Random();
        // private int[] numbers = Enumerable.Range(1, 25).OrderBy(x => random.Next()).ToArray();
        // private int[] numbers = { 23, 5, 17, 1, 11, 19, 4, 21, 14, 2, 24, 8, 13, 7, 20, 3, 10, 18, 6, 22, 15, 9, 25, 12, 16 };
        private int[] numbers = GenerateRandomOrder(25);
        private bool isServerTurn = true;

        private int prevCount = 0, newCount = 0;
    
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid();
            await StartServer();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    buttons[i, j].Text = numbers[i * 5 + j].ToString();
                    buttons[i, j].Font = new System.Drawing.Font(this.buttons[i, j].Font, FontStyle.Bold);
                }
            }
        }

        private async Task StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            client = await server.AcceptTcpClientAsync();
            stream = client.GetStream();
            await Task.Run(() => ListenForMessages());
        }

        // private void Button_Click(object sender, EventArgs e)
        // {
        //     Button button = sender as Button;
        //     if (button != null)
        //     {
        //         button.BackColor = Color.Red;
        //         textBox1.Text = button.Text;
        //     }
        // }
        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                button.BackColor = Color.Red;
                button.Enabled = false;
                textBox1.Text = button.Text;

                // Send the selected number to the client
                string message = textBox1.Text;
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                // if (CheckForWin())
                // {
                //     MessageBox.Show("Congratulations! You won!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //     // Reset the game or perform any other actions as needed
                // }

                
                prevCount = newCount;
                newCount = CheckForWin();
                
                if(newCount>prevCount){
                   bingoLabel.Text = newCount switch
                    {
                        1 => "B",
                        2 => "BI",
                        3 => "BIN",
                        4 => "BING",
                        5 => "BINGO",
                        _ => bingoLabel.Text // Keep the current text for other cases
                    };
                    // MessageBox.Show("new line", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (newCount>=5 && message!="You lost")
                {
                    MessageBox.Show("Congratulations! You won(Server1)!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    byte[] dataa = Encoding.ASCII.GetBytes("You lost");
                    stream.Write(dataa, 0, dataa.Length);
                   
                    // Reset the game or perform any other actions as needed
                }
                SwitchTurns();

            }
        }

        // private async void SendButton_Click(object sender, EventArgs e)
        // {
        //     string message = textBox1.Text;
        //     byte[] data = Encoding.ASCII.GetBytes(message);
        //     await stream.WriteAsync(data, 0, data.Length);
        // }

        private async Task ListenForMessages()
        {
            byte[] buffer = new byte[256];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if(message == "You lost"){
                    MessageBox.Show("Oops :( You lost! ", "You lost", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                }
                MarkNumberOnGrid(message);

                prevCount = newCount;
                newCount = CheckForWin();
                
                if(newCount>prevCount){
                   bingoLabel.Text = newCount switch
                    {
                        1 => "B",
                        2 => "BI",
                        3 => "BIN",
                        4 => "BING",
                        5 => "BINGO",
                        _ => bingoLabel.Text // Keep the current text for other cases
                    };
                    // MessageBox.Show("new line", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (newCount>=5)
                {
                    MessageBox.Show("Congratulations! You(Server2) won!", "Winner", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    byte[] data = Encoding.ASCII.GetBytes("You lost");
                    stream.Write(data, 0, data.Length);
                    break;
                    // Reset the game or perform any other actions as needed
                }
                SwitchTurns();
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

        private void SwitchTurns()
        {
            isServerTurn = !isServerTurn;
            // Disable or enable buttons based on whose turn it is
            foreach (Button button in buttons)
            {
                if(button.BackColor != Color.Red){
                    button.Enabled = isServerTurn;
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

        static int[] GenerateRandomOrder(int n)
        {
            Random rand = new Random();
            int[] numbers = new int[n];

            // Fill the array with numbers from 1 to n
            for (int i = 0; i < n; i++)
            {
                numbers[i] = i + 1;
            }

            // Shuffle the array using Fisher-Yates algorithm
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

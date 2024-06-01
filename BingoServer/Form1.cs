using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        // private Button[,] buttons = new Button[5, 5];
        private int[] numbers = GenerateRandomOrder(25);
        private bool gameEnded = false;
        private int prevCount = 0, newCount = 0;
        private int currentTurn = 0; // 0: server, 1: client1, 2: client2
         private static Mutex mutex = new Mutex();

        private int numberOfPlayers;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGrid();
            GetNumberOfPlayers();
            StartServer();
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

        private void GetNumberOfPlayers(){
            string userInput = Interaction.InputBox("Enter the number of players:", "Input Dialog", "");
            if (!string.IsNullOrEmpty(userInput))
            {
                try{
                    numberOfPlayers = int.Parse(userInput);
                }
                catch(Exception e){
                    MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GetNumberOfPlayers();
                    return;
                }
                
            }
            else
            {
                GetNumberOfPlayers();
            }
        }

        private async void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            for (int i = 0; i < numberOfPlayers-1; i++)
            {
                var clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(i);
                clientThreads.Add(clientThread);
            }
            await Task.Run(() => WaitForPlayers());
            EnableButtons(true);
            await Task.Run(() => ListenForMessages());
        }
        private void WaitForPlayers(){
            while(clients.Count()<numberOfPlayers-1);
        }
        private void HandleClient(object indexObj)
        {
            try{
                int index = (int)indexObj;
                // Thread.Sleep(200);
                var client = server.AcceptTcpClient();
                mutex.WaitOne();
                clients.Add(client);
                streams.Add(client.GetStream());
                mutex.ReleaseMutex();

                string playerNumberMessage = "PLAYER:" + (index + 1);
                // Thread.Sleep(200);
                // sendMessageToStream(index, playerNumberMessage);

                string numberOfPlayersMessage = "NUMBER OF PLAYERS:" + numberOfPlayers;
                // sendMessageToStream(index, numberOfPlayersMessage);
                string message = playerNumberMessage + "," + numberOfPlayersMessage;
                sendMessageToStream(index, message);

                ListenForClientMessages(index);// Listen for messages from this client
            }
            catch(Exception e){
                MessageBox.Show(e.Message, "error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void ListenForClientMessages(int index){
            byte[] buffer = new byte[256];
            MessageBox.Show("listening"+index.ToString(), "index", MessageBoxButtons.OK, MessageBoxIcon.Information);
            int bytesRead = await streams[index].ReadAsync(buffer, 0, buffer.Length);
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            if(message.StartsWith("CHAT:")){
                BroadcastMessage(message);
                string[] messageParts = message.Split(':');
                string displayMessage = messageParts[1]+messageParts[2];
                chatLabel.Text+="\n"+displayMessage;
            }
        }

        private void sendMessageToStream(int i,string message){
            byte[] data = Encoding.ASCII.GetBytes(message);
            streams[i].Write(data, 0, data.Length);
        }

        private async Task ListenForMessages()
        {
            byte[] buffer = new byte[256];
            // int bytesRead1 = await streams[0].ReadAsync(buffer, 0, buffer.Length);
            // string message1 = Encoding.ASCII.GetString(buffer, 0, bytesRead1);
            // MessageBox.Show(message1,"messaeg", MessageBoxButtons.OK, MessageBoxIcon.Information);
            while (!gameEnded)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    // MessageBox.Show(i.ToString(),"i", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int bytesRead = await streams[i].ReadAsync(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // MessageBox.Show(message,"messaeg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // // Ignore messages from clients that are not their turn
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
                    // MessageBox.Show("switching turns","turns", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void SwitchTurns()
        {
            currentTurn = (currentTurn + 1) % numberOfPlayers; // Rotate turns among server (0), client1 (1), and client2 (2)
            // MessageBox.Show("currentTurn: "+currentTurn, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            foreach (Button button in buttons)
            {
                button.Enabled = (currentTurn == 0) && (button.BackColor != Color.Red);
            }
            BroadcastTurn(); // Notify clients whose turn it is
        }

        private void BroadcastTurn()
        {
            string turnMessage = "TURN:" + currentTurn;
            // MessageBox.Show("Sending "+turnMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                BroadcastMessage(message);
                SwitchTurns();
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string chatMessage = this.chatTextBox.Text;
            string message = "CHAT:SERVER: " + chatMessage;
            chatLabel.Text +="\n"+ "SERVER: " + chatMessage;
            BroadcastMessage(message);
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

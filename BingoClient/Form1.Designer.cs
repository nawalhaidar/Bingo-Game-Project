namespace BingoClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button[,] buttons;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label chatLabel;
        private System.Windows.Forms.TextBox chatTextBox;
        private System.Windows.Forms.Label bingoLabel;
        private System.Windows.Forms.Label chatNameLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttons = new System.Windows.Forms.Button[5, 5];
            this.label1 = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.bingoLabel = new System.Windows.Forms.Label();
            this.chatLabel = new System.Windows.Forms.Label();
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.chatNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // Set the size and position of the bingoLabel
            this.bingoLabel.AutoSize = false;
            this.bingoLabel.Font = new System.Drawing.Font("Times New Roman", 35, System.Drawing.FontStyle.Bold);
            this.bingoLabel.ForeColor = System.Drawing.Color.Black;
            this.bingoLabel.Location = new System.Drawing.Point(80, 20);
            this.bingoLabel.Name = "bingoLabel";
            this.bingoLabel.Size = new System.Drawing.Size(270, 70);
            this.bingoLabel.Text = "";
            this.bingoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Calculate the starting position of the grid to center it horizontally
            int buttonSize = 60;
            int buttonSpacing = 5;
            int gridWidth = 5 * buttonSize + 4 * buttonSpacing;
            int gridStartX = 50; // Adjusted to fit within the new window size
            int gridStartY = 200; // Adjust vertical position as needed

            // Set the size and position of the buttons grid
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    this.buttons[i, j] = new System.Windows.Forms.Button();
                    this.buttons[i, j].Location = new System.Drawing.Point(gridStartX + j * (buttonSize + buttonSpacing), gridStartY + i * (buttonSize + buttonSpacing));
                    this.buttons[i, j].Name = $"button{i}{j}";
                    this.buttons[i, j].Size = new System.Drawing.Size(buttonSize, buttonSize);
                    this.buttons[i, j].Font = new System.Drawing.Font("Times New Roman", 14, System.Drawing.FontStyle.Bold);
                    this.buttons[i, j].BackColor = System.Drawing.Color.MistyRose;
                    this.buttons[i, j].UseVisualStyleBackColor = true;
                    this.buttons[i, j].Click += new System.EventHandler(this.Button_Click);
                    this.Controls.Add(this.buttons[i, j]);
                }
            }

            int chatStartX = 400; 
            int chatWidth = 300;
            int chatHeight = 50;
            int chatBoxY = this.ClientSize.Height - chatHeight+250; 
            int sendButtonY = chatBoxY + chatHeight + 10; 

            // Position and configure chat components
            this.chatNameLabel.AutoSize = true;
            this.chatNameLabel.Location = new System.Drawing.Point(2*gridStartX + 5 * (buttonSize + buttonSpacing), 50);
            this.chatNameLabel.Name = "chatNameLabel";
            this.chatNameLabel.Size = new System.Drawing.Size(29, 13);
            this.chatNameLabel.Font = new System.Drawing.Font("Segoe UI", 15, System.Drawing.FontStyle.Bold);
            this.chatNameLabel.Text = "Chat:";
            this.chatNameLabel.TabIndex = 1;

            // Position and configure chat components
            this.chatLabel.AutoSize = true;
            this.chatLabel.Location = new System.Drawing.Point(2*gridStartX + 5 * (buttonSize + buttonSpacing), 100);
            this.chatLabel.Name = "chatLabel";
            this.chatLabel.Size = new System.Drawing.Size(29, 13);
            this.chatLabel.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular);
            this.chatLabel.TabIndex = 1;

            this.chatTextBox.Location = new System.Drawing.Point(chatStartX, chatBoxY);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(chatWidth, chatHeight);
            this.chatTextBox.Multiline = true;
            this.chatTextBox.TabIndex = 2;
            this.chatTextBox.ScrollBars = ScrollBars.Vertical; // Add vertical scrollbar
            this.chatTextBox.Font = new System.Drawing.Font("Times New Roman", 12, System.Drawing.FontStyle.Regular);
            this.chatTextBox.BackColor = System.Drawing.Color.White;
            this.chatTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            this.sendButton.Location = new System.Drawing.Point(chatStartX + chatWidth - 100, sendButtonY);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(100, 30);
            this.sendButton.TabIndex = 3;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Font = new System.Drawing.Font("Times New Roman", 10, System.Drawing.FontStyle.Bold);
            this.sendButton.BackColor = System.Drawing.Color.Pink; // Pink color for the send button
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);

            // Add chat components to the form
            this.Controls.Add(this.chatLabel);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatNameLabel);


            // Set the size and position of other controls if needed
            this.ClientSize = new System.Drawing.Size(750, 600);
            this.BackColor = System.Drawing.Color.Linen;
            this.Controls.Add(this.bingoLabel);
            this.Name = "Form1";
            this.Text = "Bingo Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

       
    }
}

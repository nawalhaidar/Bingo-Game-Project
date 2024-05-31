namespace BingoServer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button[,] buttons;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button sendButton;

        private System.Windows.Forms.Label bingoLabel;

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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.bingoLabel = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Set the size and position of the bingoLabel
            this.bingoLabel.AutoSize = false;
            this.bingoLabel.Font = new System.Drawing.Font("Arial", 35, System.Drawing.FontStyle.Bold);
            this.bingoLabel.ForeColor = System.Drawing.Color.Black;
            this.bingoLabel.Location = new System.Drawing.Point(200, 50);
            this.bingoLabel.Name = "bingoLabel";
            this.bingoLabel.Size = new System.Drawing.Size(300, 100);
            this.bingoLabel.Text = "";
            this.bingoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Calculate the starting position of the grid to center it horizontally
            int buttonSize = 100;
            int buttonSpacing = 10;
            int gridWidth = 5 * buttonSize + 4 * buttonSpacing;
            // int gridStartX = (this.ClientSize.Width - gridWidth) / 2;
            int gridStartX = (700 - gridWidth) / 2;

            Console.WriteLine(gridStartX);
            int gridStartY = 200; // Adjust vertical position as needed

            // Set the size and position of the buttons grid
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    this.buttons[i, j] = new System.Windows.Forms.Button();
                    this.buttons[i, j].Location = new System.Drawing.Point(gridStartX + j * (buttonSize + buttonSpacing), gridStartY + i * (buttonSize + buttonSpacing));
                    // this.buttons[i, j].Location = new System.Drawing.Point(0, gridStartY + i * (buttonSize + buttonSpacing));
                    this.buttons[i, j].Name = $"button{i}{j}";
                    this.buttons[i, j].Size = new System.Drawing.Size(buttonSize, buttonSize);
                    // this.buttons[i, j].TabIndex = i * 5 + j;
                    this.buttons[i, j].UseVisualStyleBackColor = true;
                    this.buttons[i, j].Click += new System.EventHandler(this.Button_Click);
                    this.Controls.Add(this.buttons[i, j]);
                }
            }

            // Set the size and position of other controls if needed

            this.ClientSize = new System.Drawing.Size(700, 800);
            this.Controls.Add(this.bingoLabel);
            // Add other controls as needed

            this.Name = "Form1";
            this.Text = "Bingo Server";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }
    }
}

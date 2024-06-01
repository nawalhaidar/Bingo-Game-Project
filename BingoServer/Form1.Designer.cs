namespace BingoServer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button[,] buttons;
        private System.Windows.Forms.Label chatLabel;
        private System.Windows.Forms.TextBox chatTextBox;
        private System.Windows.Forms.Button sendButton;

        private System.Windows.Forms.Label bingoLabel;
        private List<Label> nameLabels = new List<Label>();
        private List<Label> scoreLabels = new List<Label>();

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent(int numberOfPlayers)
        {
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "Dynamic Labels Form";
            this.ResumeLayout(false);

            int labelWidth = 100;
            int labelHeight = 20;
            int spacing = 30;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                // Create Name Label
                Label nameLabel = new Label();
                nameLabel.Text = "Player " + (i + 1);
                nameLabel.Location = new System.Drawing.Point(10, 10 + i * spacing);
                nameLabel.Size = new System.Drawing.Size(labelWidth, labelHeight);
                this.Controls.Add(nameLabel);
                nameLabels.Add(nameLabel); 

                // Create Score Label
                Label scoreLabel = new Label();
                scoreLabel.Text = "Result: ";
                scoreLabel.Location = new System.Drawing.Point(120, 10 + i * spacing);
                scoreLabel.Size = new System.Drawing.Size(labelWidth, labelHeight);
                this.Controls.Add(scoreLabel);
                scoreLabels.Add(scoreLabel); 
            }
        }
    }
}

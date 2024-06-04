namespace BingoServer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label[] nameLabels;
        private System.Windows.Forms.Label[] scoreLabels;
        private System.Windows.Forms.Button[] buttons;

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
            this.components = new System.ComponentModel.Container();
            this.nameLabels = new System.Windows.Forms.Label[numberOfPlayers];
            this.scoreLabels = new System.Windows.Forms.Label[numberOfPlayers];
            this.buttons = new System.Windows.Forms.Button[numberOfPlayers];

            // Set the size and background color of the form
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Name = "MainForm";
            this.Text = "Results";
            this.ResumeLayout(false);

            int labelWidth = 150;
            int labelHeight = 20;
            int spacing = 30;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                nameLabels[i] = new System.Windows.Forms.Label();
                nameLabels[i].Text = "Player " + (i + 1);
                nameLabels[i].Location = new System.Drawing.Point(10, 10 + i * spacing);
                nameLabels[i].Size = new System.Drawing.Size(labelWidth, labelHeight);
                nameLabels[i].Font = new System.Drawing.Font("Times New Roman", 12, System.Drawing.FontStyle.Bold);
                nameLabels[i].ForeColor = System.Drawing.Color.Black;
                this.Controls.Add(nameLabels[i]);

                scoreLabels[i] = new System.Windows.Forms.Label();
                scoreLabels[i].Text = "";
                scoreLabels[i].Location = new System.Drawing.Point(170, 10 + i * spacing);
                scoreLabels[i].Size = new System.Drawing.Size(labelWidth, labelHeight);
                scoreLabels[i].Font = new System.Drawing.Font("Times New Roman", 12, System.Drawing.FontStyle.Bold);
                scoreLabels[i].ForeColor = System.Drawing.Color.Black;
                this.Controls.Add(scoreLabels[i]);


            }
            this.Load += new System.EventHandler(this.Form1_Load);
        }

    
    }
}

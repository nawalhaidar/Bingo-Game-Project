namespace BingoServer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label[] nameLabels;
        private System.Windows.Forms.Label[] scoreLabels;

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
            this.nameLabels = new System.Windows.Forms.Label[numberOfPlayers];
            this.scoreLabels = new System.Windows.Forms.Label[numberOfPlayers];
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "Dynamic Labels Form";
            this.ResumeLayout(false);

            int labelWidth = 100;
            int labelHeight = 20;
            int spacing = 30;
            MessageBox.Show("here","here",MessageBoxButtons.OK,MessageBoxIcon.None);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                nameLabels[i] = new System.Windows.Forms.Label();
                nameLabels[i].Text = "Player " + (i + 1);
                nameLabels[i].Location = new System.Drawing.Point(10, 10 + i * spacing);
                nameLabels[i].Size = new System.Drawing.Size(labelWidth, labelHeight);
                this.Controls.Add(nameLabels[i]);
       
                scoreLabels[i] = new System.Windows.Forms.Label();
                scoreLabels[i].Text = "Result: ";
                scoreLabels[i].Location = new System.Drawing.Point(120, 10 + i * spacing);
                scoreLabels[i].Size = new System.Drawing.Size(labelWidth, labelHeight);
                this.Controls.Add(scoreLabels[i]);

            }
            this.Load += new System.EventHandler(this.Form1_Load);
        }
    }
}

using System;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string userInput = txtInput.Text;

            if (userInput == "1")
            {
                Form1 form1 = new Form1();
                form1.Show();
            }
            else
            {
                Form2 form2 = new Form2();
                form2.Show();
            }
        }
    }
}

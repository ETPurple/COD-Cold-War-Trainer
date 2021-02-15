using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ___.util;

namespace ___
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (util.Auth.API.Login(textBox1.Text, textBox2.Text))
            {
                // hide the login form and show the main form.
                this.Hide();

                //Display main form
                Main main = new Main();
                main.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Register reg = new Register();
            reg.Show();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}

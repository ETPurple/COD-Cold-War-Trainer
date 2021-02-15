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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(util.Auth.API.Register(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text))
            {
                //send messagebox that the registration was successfull
                MessageBox.Show("You have successfully registered!");

                //hide this window so we can display the login window again for the user to login.
                this.Hide();

                //Show login form.
                Login login = new Login();
                login.Show();
            }
            else
            {
                //notify user that the registration was un-successful
                MessageBox.Show("Registration Un-Successful");
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}

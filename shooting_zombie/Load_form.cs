using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shooting_zombie
{
    public partial class Load_form : Form
    {
        public Load_form()
        {
            InitializeComponent();
        }
        int  staring = 0;
        private void Load_form_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            staring += 1;
            progressBar1.Value = staring;
            label2.Text = staring + "%";

            if (progressBar1.Value == 100)
            {
                // timer1.Enabled = false;
                progressBar1.Value = 0;
                timer1.Stop();

                Form1 form1 = new Form1();
                this.Hide();
                form1.Show();
             
            }

        }
    }
}

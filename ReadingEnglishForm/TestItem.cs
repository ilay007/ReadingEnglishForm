using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ReadingEnglishForm.Properties;

namespace ReadingEnglishForm
{
    public partial class TestItem : Form
    {
        public TestItem()
        {
            InitializeComponent();
           
        }

        private void TestItem_KeyDown(object sender, KeyEventArgs e)
        {
            int a = 2;
        }

        private void TestItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            int al = 2;
        }


    private void pictureBox1_Click(object sender, MouseEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var wordsCount = Convert.ToInt16("2");
        }
    }

}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingEnglishForm
{
    public partial class ViewVocabulary : Form
    {
        public ViewVocabulary()
        {
            InitializeComponent();
            var count = 0;
            foreach (var word in Word.Vocabulary)
            {
                count++;
                textBox1.AppendText(count+") "+word.Key+" - " +word.Value.RusTranslation[0].ToString());
                textBox1.AppendText("\n");
            }
            
        }
    }
}

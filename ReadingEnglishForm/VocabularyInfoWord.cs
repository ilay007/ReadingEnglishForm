using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingEnglishForm
{
    public class VocabularyInfoWord
    {
        public VocabularyInfoWord(string rustransl, string time,Color color)
        {
            RusTranslation = rustransl.Split(new char[] {';'},StringSplitOptions.RemoveEmptyEntries).ToList();
            Color = color;
            if (time != "") SecsesfulTime = DateTime.Parse(time);
            
        }
        public List<string> RusTranslation;
        public DateTime SecsesfulTime;
        public Color Color;
        public Mode Mode = Mode.DirectTranslation;
        public string EnglTranslation;

    }
   public  enum Mode { DirectTranslation, BackTranslation, Writing};
}

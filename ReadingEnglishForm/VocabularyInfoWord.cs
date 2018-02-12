using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingEnglishForm
{
    public class VocabularyInfoWord
    {
        public VocabularyInfoWord(string rustransl, string time )
        {
            RusTranslation = rustransl.Split(new char[] {';'},StringSplitOptions.RemoveEmptyEntries).ToList();
            if (time != "") SecsesfulTime = DateTime.Parse(time);

        }
        public List<string> RusTranslation;
        public DateTime SecsesfulTime;
    }
}

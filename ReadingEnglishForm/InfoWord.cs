using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingEnglishForm
{
    public class InfoWord
    {
        public List<string> EnglishWords { get; private set; }
        public List<string> RussianWords { get; private set; }
        public string FirstTranslation;

        public void AddValue(string englvalue, string russianvalue)
        {
            if (FirstTranslation == null)
            {
                FirstTranslation = russianvalue;
                EnglishWords= new List<string>();
                RussianWords = new List<string>();
            }
            EnglishWords.Add(englvalue);
            RussianWords.Add(russianvalue);
        }

        public List<string> GetData()
        {
            var list = new List<string>();
            for (int i = 0; i < EnglishWords.Count; i++)
            {
                list.Add(EnglishWords[i]+" - "+RussianWords[i]);
            }
            return list;
        } 
    }
}

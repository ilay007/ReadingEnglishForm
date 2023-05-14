using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEnglishForm
{
    public class Word
    {
        public static int size;
        public static Dictionary<string, InfoWord> EDictionary;//=new Dictionary<string, List<string>>();
        public static Dictionary<string, VocabularyInfoWord> Vocabulary;
        public static List<string> LearnWords = new List<string>();
        public static int LimitX = 0;
        public static int LimitY = 0;
        public static string NameVocabulary = "";

        public string Value { get; private set;}
        public string RelVal { get; private set; }
        public string TranslatedValue { get; private set; }

        public int StartX;
        public int StartY;
        public bool IsUpper = false;

        public Rectangle PlaceWord;
        public int SucssesCount;

        public Word(string value, int x, int y, int width, int hight)
        {
            var sw = value.Split(new char[] {'.', '!', '?', ',', ':', ';','$','-','"'});
            var max = 0;
            for (int i = 0; i <sw.Length ; i++)
            {
                if (sw[i].Length > sw[0].Length)
                {
                    max = i;
                }
            }
            Value = sw[max];
            RelVal = value;
            CheckInEDictionary();
            StartX = x;
            StartY = y;
            PlaceWord = new Rectangle(StartX,StartY,width,hight);
            CheckInEDictionary();

        }

        public void Draw(Graphics g, Font drawFont, SolidBrush brush,bool drawRectangle)
        {
            var lenghs = (int) g.MeasureString(Value.ToString(), drawFont).Width - 1;
            g.FillRectangle(new SolidBrush(Color.Coral), new Rectangle(StartX, StartY, lenghs, 2 * size));
            g.DrawString(Value, drawFont, brush, new PointF(StartX, StartY));
          
          
        }

        public bool IsInVacabulary = false;
        private int count = 0;

        private void CheckInEDictionary()
        {
            if (EDictionary.ContainsKey(Value.ToLower()))
            {
                IsInVacabulary = true;
                TranslatedValue = EDictionary[Value.ToLower()].FirstTranslation;
            }
        }

        public void Click()
        {
            count++;
        }

    }
}

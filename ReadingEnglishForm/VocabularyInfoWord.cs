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
            if (time != " "&&time!=String.Empty) SecsesfulTime = DateTime.Parse(time);
            
        }
        public List<string> RusTranslation;
        public DateTime SecsesfulTime;
        public DateTime WrongTime;//=DateTime.Now;
        public Color Color;
        public int SecsesfulCount;
        public Mode Mode = Mode.DirectTranslation;
        public string EnglTranslation;
        private PointF curPoint;

        public bool WithoutMistakes = true;

        public void SetWrongTime(string time)
        {
            if (time != " " && time != String.Empty) WrongTime = DateTime.Parse(time);
            else
            {
                WrongTime = DateTime.Now;
            }
        }

        public void SetCurPoint(PointF point)
        {
            curPoint = new PointF();
            curPoint.X = point.X;
            curPoint.Y = point.Y;

        }

        public PointF GetGraphPoint()
        {
            return curPoint;
        }

    }
   public  enum Mode { DirectTranslation, BackTranslation, WritingSpelilng,Writing,End};
}

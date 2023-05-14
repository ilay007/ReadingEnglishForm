using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadingEnglishForm
{
    public partial class PictureText : System.Windows.Forms.PictureBox
    {
        private Bitmap CurMap;
        private Bitmap TranslatedCurMap;
        private Graphics g;

        public List<Word> Words= new List<Word>();
        public int Width1 = 800;
        public int Height1 = 25400;
        public bool drawRectangl = true;
        public int CountWords = 0;
        public List<Color> Colors=new List<Color>(); 


        public PictureText()
        {
            InitializeComponent();

            CurMap = new Bitmap(this.Width1, this.Height1);
            g= Graphics.FromImage(CurMap);
        }

        public void MouseMoved()
        {
            if (Words.Count == 0) return;
            Draw();
        }

        public void Draw()
        {
            g.Clear(Color.White);
            PointF drawPoint = new PointF(10.0F, 0.0F);
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            for (int i = 0; i < Words.Count; i++)
            {
                var word = Words[i];
                if(Colors.Count>i) drawBrush = new SolidBrush(Colors[i]);
                drawPoint.X = word.StartX;
                drawPoint.Y = word.StartY;
                if (word.IsUpper) drawFont = new Font("Arial", 12, FontStyle.Bold);
                if (!word.IsUpper && drawFont.Bold) drawFont = new Font("Arial", 12);
                g.DrawString(word.RelVal, drawFont, drawBrush, drawPoint);
            }
           
        }


       



        private void addToCount(List<string> words)
        {
            foreach (var word in words)
            {
                if (word.Count() > 0 && word != "the")
                {
                    CountWords ++;
                }
               
            }
           
        }

        public void InsertWordsInMemory(List<string> strings,bool Clear)
        {
            g.Clear(Color.White);
            if(Clear)Words.Clear();
            Font drawFont = new Font("Arial", 12);
         
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Word.size = 10;
          
            Word.LimitX = this.Width1;
            Word.LimitY = this.Height1;

            // Create point for upper-left corner of drawing.
            PointF drawPoint = new PointF(10.0F, 10.0F);
            var countX = 0;
            var countY = 0;
            var stepw = (int)g.MeasureString("f", drawFont).Width+1;
            var steph = (int)g.MeasureString("A", drawFont).Height;
            var county = 0;
            var max = 0;
            var l = 0;
            foreach (var str in strings)
            {
                 l+=  (int)g.MeasureString(str, drawFont).Width;
               
            }
            var height1=l*12*steph/(Width1);
            if (height1 == 0 || Width1 == 0) return;
            CurMap = new Bitmap(Width1, height1);
            g = Graphics.FromImage(CurMap);

            foreach (var str in strings)
            {
                bool isUpper = false;
               
                var words = str.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
                addToCount(words);
                var last = 0;
               // g.DrawString(str, drawFont, drawBrush, drawPoint);

                drawPoint.X = 0;
                var delta = 0;
                if (!words.Contains("."))
                {
                    int count = 0;
                    foreach (var word in words)
                    {
                        if (char.IsUpper(word[0])) count++;
                    }
                    if (count >= words.Count-2)
                    {
                        isUpper = true;
                    }
                }
                
                foreach (var word in words)
                {
                    var start = str.IndexOf(word, last);
                    last = start;
                    drawPoint.X = (start)*stepw-delta;
                   
                    var length = (int)g.MeasureString(word.ToString(), drawFont).Width - 1;

                    if (drawPoint.X+length > Width1)
                    {
                        
                        county++;
                        drawPoint.Y = county * 3 * steph / 2;
                        delta+= (int)drawPoint.X;
                        drawPoint.X = 0;
                    }

                    Words.Add(new Word(word, (int)drawPoint.X, (int)drawPoint.Y, length,steph));
                    Words.Last().IsUpper = isUpper;
                }
                county++;
                drawPoint.Y = county *3* steph/2;
             
            }
         //   TranslatedCurMap= new Bitmap((Bitmap)CurMap.Clone());
            this.Image = CurMap;
        }

        public void CheckWord(Point point)
        {
            var x = point.X;
            var y = point.Y;
            var res= Words.FirstOrDefault(s => s.PlaceWord.Contains(x,y));
            if (res != null)
            {
                g.DrawRectangle(new Pen(Color.DarkBlue), res.StartX, res.StartY, res.PlaceWord.Width,res.PlaceWord.Height);
                if (!res.IsInVacabulary)return;
                Font drawFont = new Font("Arial", 12);

                SolidBrush drawBrush = new SolidBrush(Color.Green);Word.size = 10;
              
                Word.LimitX = this.Width1;
                Word.LimitY = this.Height1;

                // Create point for upper-left corner of drawing.
                PointF drawPoint = new PointF(x, y+Word.size/2);
                var countX = 0;


                var listlatters=res.TranslatedValue.ToCharArray().ToList();
                foreach (var latter in listlatters)
                {
                    g.DrawString(latter.ToString(), drawFont, drawBrush, drawPoint);
                    drawPoint.X += (int)g.MeasureString(latter.ToString(), drawFont).Width - 1;
                }
                this.Image = CurMap;

            }
        }


        public Word GetTranslation(int x, int y)
        {
            var res = Words.FirstOrDefault(s => s.PlaceWord.Contains(x,y));
            if (res != null)
            {
               
                if (!res.IsInVacabulary) return null;
                return res;
                
               // this.Image = CurMap;

            }
            return null;
        }


    }
}

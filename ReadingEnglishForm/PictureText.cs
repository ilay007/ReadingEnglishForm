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
        public int Width1 = 600;
        public int Height1 = 5400;
        public bool drawRectangl = true;


        public PictureText()
        {
            InitializeComponent();

            CurMap = new Bitmap(this.Width1, this.Height1);
            g= Graphics.FromImage(CurMap);
        }

        public void MouseMoved()
        {
            if(Words.Count==0)return;
            g.Clear(Color.White);
            PointF drawPoint = new PointF(10.0F, 0.0F);
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            var countX = 0;
            var countY = 0;
            foreach (var word in Words)
            {
               

                drawPoint.X = word.StartX;
                drawPoint.Y = word.StartY;
                //var listlatters = word.ToCharArray().ToList();
                //countY = Words.Last().StartY;
                //foreach (var latter in listlatters)
                //{
                //    g.DrawString(latter.ToString(), drawFont, drawBrush, drawPoint);
                //    drawPoint.X +=(int)g.MeasureString(latter.ToString(),drawFont).Width-1;
                //}
                //foreach (var latter in listlatters)
                //{
                g.DrawString(word.RelVal, drawFont, drawBrush, drawPoint);
            //    drawPoint.X += (int)g.MeasureString(word.Value, drawFont).Width - 1;
              
              

              //  g.DrawRectangle(new Pen(Brushes.Red), Words.Last().StartX, Words.Last().StartY, lenthX, lenthY);

                //}

                //countX = (int)drawPoint.X + 2 * Word.size;


            }
        

    }

        public void WriteListString(List<string> strings,bool Clear)
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

            foreach (var str in strings)
            {
                var l=  (int)g.MeasureString(str, drawFont).Width;
                if (l > max)
                {
                    max = l;
                }
            }


            CurMap = new Bitmap(max, strings.Count*2*steph);
            g = Graphics.FromImage(CurMap);

            foreach (var str in strings)
            {
                var words = str.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var last = 0;
                g.DrawString(str, drawFont, drawBrush, drawPoint);

                drawPoint.X = 0;
                foreach (var word in words)
                {
                    var start = str.IndexOf(word, last);
                    last = start;
                    drawPoint.X = start*stepw;
                   
                    var length = (int)g.MeasureString(word.ToString(), drawFont).Width - 1;
                    Words.Add(new Word(word, (int)drawPoint.X, (int)drawPoint.Y, length,steph));
                    
                }
                county++;
                drawPoint.Y = county *3* steph/2;
             
                //foreach (var word in words)
                //{
                //    Words.Add(new Word(word,countX,countY));

                //    drawPoint.X = Words.Last().StartX;
                //    drawPoint.Y = Words.Last().StartY;

                //    //var listlatters = word.ToCharArray().ToList();

                //    countY = Words.Last().StartY;
                //    //foreach (var latter in listlatters)
                //    //{
                //    //    g.DrawString(latter.ToString(), drawFont, drawBrush, drawPoint);
                //    //    drawPoint.X +=(int)g.MeasureString(latter.ToString(),drawFont).Width-1;
                //    //}
                //    //foreach (var latter in listlatters)
                //    //{
                //    g.DrawString(word, drawFont, drawBrush, drawPoint);
                //    drawPoint.X += (int)g.MeasureString(word.ToString(), drawFont).Width - 1;
                //    Words.Last().EndX = (int)drawPoint.X;
                //    var lenthX = Words.Last().EndX - Words.Last().StartX;
                //    var lenthY = Words.Last().EndY - Words.Last().StartY;

                //    g.DrawRectangle(new Pen(Brushes.Red), Words.Last().StartX, Words.Last().StartY, lenthX, lenthY);

                //    //}

                //    countX = (int)drawPoint.X + 2 * Word.size;


                //}
            }
            TranslatedCurMap= new Bitmap((Bitmap)CurMap.Clone());
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

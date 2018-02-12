using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ReadingEnglishForm.Properties;

namespace ReadingEnglishForm
{
    public partial class OpenWord : Form
    {
        private Bitmap CurMap;
        private Bitmap TranslatedCurMap;
        private Graphics g;
        public Word Button;
        public int shift = 25;
        public Word CurrentWord;
        public string VocabularName = Settings.Default.EDictionary;
        public List<WordInRectangle> CurWordList=new List<WordInRectangle>();
        public Dictionary<string,List<string>> WordsForEDictionary=new Dictionary<string, List<string>>();
        public string ButtonName = "Добавить в словарь";
        public OpenWord(Word data )
        {
            InitializeComponent();
            WriteListString(data);
        }

        public void WriteListString(Word translatedword)
        {
            CurrentWord = translatedword;
            var hight = pictureBox1.Height;
            if (Word.EDictionary[translatedword.Value.ToLower()].EnglishWords.Count*3*Word.size > hight)
                hight = Word.EDictionary[translatedword.Value.ToLower()].EnglishWords.Count*3*Word.size;
           
            var maxwidth = 0;
            CurMap = new Bitmap(pictureBox1.Width, hight);
            g = Graphics.FromImage(CurMap);
          
            Font drawFont = new Font("Arial", 12);

            SolidBrush drawBrushWord = new SolidBrush(Color.Blue);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Word.size = 10;

            var listdata = Word.EDictionary[translatedword.Value.ToLower()].GetData();
        
            foreach (var onestr in listdata)
            {
              var width= (int)g.MeasureString(onestr, drawFont).Width + 3*shift;
                if (maxwidth < width)
                {
                    maxwidth = width;
                }
            }
            if (pictureBox1.Width > maxwidth) maxwidth = pictureBox1.Width;

            CurMap = new Bitmap(maxwidth, hight);
            g = Graphics.FromImage(CurMap);
            g.Clear(Color.White);

            // Create point for upper-left corner of drawing.
            PointF drawPoint = new PointF(10.0F, 0.0F);
            var countX = 0;
            var countY = 0;
            g.DrawString(translatedword.Value, drawFont, drawBrushWord, drawPoint);
            drawPoint.X += (int)g.MeasureString(translatedword.Value.ToString(), drawFont).Width +shift;

            Button = new Word(ButtonName,(int)drawPoint.X,(int)drawPoint.Y,(int)g.MeasureString(ButtonName,drawFont).Width, (int)g.MeasureString(ButtonName, drawFont).Height);
            Button.Draw(g,drawFont,drawBrush,true);
            
            drawPoint.X -= 10;
            drawPoint.Y += 30;
           // var res=translatedword.TranslatedValue.OrderBy(s => s.Length);
            foreach (var word in listdata.OrderBy(s => s.Length))
            {
                g.DrawString(word, drawFont, drawBrush, drawPoint);
                CurWordList.Add(new WordInRectangle(word, new Rectangle((int)drawPoint.X, (int)drawPoint.Y, (int)g.MeasureString(word.ToString(), drawFont).Width - 1, 2 * Word.size)));
                drawPoint.Y = (int)drawPoint.Y + 2 * Word.size;
               


            }
            pictureBox1.Image = CurMap;
        }

        private void SaveToDictionary_MouseClick(object sender, MouseEventArgs e)
        {
            if (Button.PlaceWord.Contains(e.X,e.Y))
            {

                //using (var wr = new StreamWriter(VocabularName))
                //{
                //    foreach (var val in WordsForEDictionary)
                //    {
                //        var str = val.Key+" - ";
                //        for (int i = 0; i < val.Value.Count(); i++)
                //        {
                //            str += val.Value[i]+";";
                //        }
                //        wr.WriteLine(str);
                //    }
                //    wr.Close();
                //}
                //return;
               
                AddToEDictionary();
                CurWordList.Clear();
            }
            foreach (var word in CurWordList)
            {
                if (!word.Rectangle.Contains(new Point(e.X, e.Y)))continue;

                // g.FillEllipse(new Pen(Color.Red),word.Rectangle.X-20,word.Rectangle.Y+5,8,8);
                SolidBrush drawBrushWord = new SolidBrush(Color.Red);
                g.FillEllipse(drawBrushWord, word.Rectangle.X - 20, word.Rectangle.Y + 5, 8, 8);
                pictureBox1.Image = CurMap;
                var data = word.WordValue.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Count() > 1)
                {
                    if (!WordsForEDictionary.ContainsKey(data[0]))
                    {
                        WordsForEDictionary.Add(data[0], new List<string>());
                    }
                    WordsForEDictionary[data[0]].Add(data[1]);
                }
            }

        }

        public void AddToEDictionary()
        {
           
            var namefile = "vocabulary.xml";
            XDocument doc1 = XDocument.Load(namefile);
            XElement school = doc1.Element("root");
            var count = 0;
            XElement oneword = new XElement("world"); 
            foreach (var word in WordsForEDictionary)
            {
                if (Word.EDictionary.ContainsKey(word.Key)) continue;
                if (count == 0)
                {
                  
                    oneword.Add(new XAttribute("word", word.Key));
                  //  continue;
                }

                var time = new XElement("TimeCreation");
                time.Add(new XAttribute("time", DateTime.Now.ToString()));

                var timegoodanswer= new XElement("TimeGoodAnswer");
                timegoodanswer.Add(new XAttribute("time", " "));


                var rusword = "";
                foreach (var val in word.Value)
                {
                    rusword += val + ";";
                }

                var Item = new XElement("Item");
                Item.Add(new XAttribute("engword", word.Key));
                Item.Add(new XAttribute("russword", rusword));
                Item.Add(time);
                Item.Add(timegoodanswer);

                oneword.Add(Item);
               
                count++;
            }
            school.Add(oneword);
            doc1.Save(namefile);

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

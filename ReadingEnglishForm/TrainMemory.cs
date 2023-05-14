using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ReadingEnglishForm.Properties;

namespace ReadingEnglishForm
{
    public partial class TrainMemory : Form
    {

        public TrainMemory()
        {
            InitializeComponent();
            Init();
        }

        private Bitmap CurMap;
        private Bitmap TranslatedCurMap;
        private int NumCruTranslation;
        private Graphics g;
        public Word Button;
        public int shift = 25;
        public KeyValuePair<string, VocabularyInfoWord> CurrentWord;
        public List<Word> CurWords= new List<Word>(); 
        public string VocabularName = Settings.Default.EDictionary;
        public List<WordInRectangle> CurWordList = new List<WordInRectangle>();
        public Dictionary<string, List<string>> WordsForEDictionary = new Dictionary<string, List<string>>();
        public string ButtonName = "";
        public Queue<KeyValuePair<string, VocabularyInfoWord>> TrainQueue = new Queue<KeyValuePair<string, VocabularyInfoWord>>();
        public PrintedBuffer PrintedBuffer = new PrintedBuffer();
        public static int NumberWords=3;
        public int DyesToRepeat = Settings.Default.DaysRepeat;
        public static bool TrainWas = false;
        


        private void Init()
        {
            if (Word.Vocabulary == null) return;
            var info = XmlHandler.GetInstanse().GetInfo();
            InitQueue(NumberWords);
        }

        public void InitQueue( int countWords)
        {
          
           
            var words=XmlHandler.GetInstanse().ForChecking.GetRange(0, countWords);
            foreach (var word in words)
            {
                word.Value.Mode = Mode.DirectTranslation;
                word.Value.WithoutMistakes = true;
                TrainQueue.Enqueue(word);
            }

            CurMap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(CurMap);
            CurrentWord = TrainQueue.Dequeue();
            SwitchOnNextSampleRus(CurrentWord);
           
        }

        public void SwitchOnNextSampleRus(KeyValuePair<string,VocabularyInfoWord> word)
        {
              var lstr = new Dictionary<string, PointF>();
              g.Clear(Color.White);
                var rnd = new Random().Next(0, 4);
           
                Font drawFont = new Font("Arial", 20);

                SolidBrush drawBrushWord = new SolidBrush(word.Value.Color);
               
                PointF drawPoint = new PointF(pictureBox1.Width / 3 - ((int)g.MeasureString(word.Key, drawFont).Width/2), 50.0F);
                g.DrawString(word.Key, drawFont, drawBrushWord, drawPoint);
                NumCruTranslation = new Random().Next(0, word.Value.RusTranslation.Count - 1);
                drawBrushWord = new SolidBrush(Color.DarkGreen);

                 var del = 0;
                for (int i = 0; i < 4; i++)
                {
                    var print = "";
                    if (rnd == i) print = word.Value.RusTranslation[NumCruTranslation];
                    else print = GetRandom();
                    //TODO
                    var y = 150;
                    var x = 50;
                    if (i>1)
                    {
                        y += 100;
                    }
                    if (i%2 != 0)
                    {
                        x += (del+20);
                    }
                    drawPoint = new PointF(x, y);
                    while (lstr.ContainsKey(print))
                    {
                        print = GetRandom();
                    }
                  lstr.Add(print,drawPoint);
              del =  (int)g.MeasureString(print, drawFont).Width;
            }
                CurWords.Clear();
            foreach (var sign in lstr)
            {
                g.DrawString(sign.Key, drawFont, drawBrushWord, sign.Value);
                var mesure = g.MeasureString(sign.Key, drawFont);
                CurWords.Add(new Word(sign.Key, (int) sign.Value.X, (int) sign.Value.Y, (int) mesure.Width,
                    (int) mesure.Height));
            }



            pictureBox1.Image = CurMap;
        }

        public void SwitchOnNextSampleEngl(KeyValuePair<string, VocabularyInfoWord> word)
        {
            var lstr = new Dictionary<string, PointF>();
            g.Clear(Color.White);
            var rnd = new Random().Next(0, 4);
            word.Value.Mode = Mode.BackTranslation;
            Font drawFont = new Font("Arial", 20);

            SolidBrush drawBrushWord = new SolidBrush(word.Value.Color);

            PointF drawPoint = new PointF(pictureBox1.Width / 3 - ((int)g.MeasureString(word.Key, drawFont).Width / 2), 50.0F);

            NumCruTranslation = new Random().Next(0, word.Value.RusTranslation.Count-1);
            word.Value.SetCurPoint(drawPoint);
            g.DrawString(word.Value.RusTranslation[NumCruTranslation], drawFont, drawBrushWord, drawPoint);
            word.Value.EnglTranslation = word.Key;
            drawBrushWord = new SolidBrush(Color.DarkGreen);
            for (int i = 0; i < 4; i++)
            {
                var print = "";
                if (rnd == i) print = word.Key;
                else print = GetRandomEngl();
                var y = 150;
                var x = 450;
                if (i > 1)
                {
                    y += 100;
                }
                if (i % 2 == 0)
                {
                    x = 50;
                }
                drawPoint = new PointF(x, y);
                while (lstr.ContainsKey(print))
                {
                    print = GetRandomEngl();
                }
                 
                lstr.Add(print, drawPoint);
            }
            CurWords.Clear();
            foreach (var sign in lstr)
            {
                g.DrawString(sign.Key, drawFont, drawBrushWord, sign.Value);
                var mesure = g.MeasureString(sign.Key, drawFont);
                CurWords.Add(new Word(sign.Key, (int)sign.Value.X, (int)sign.Value.Y, (int)mesure.Width, (int)mesure.Height));


            }

            pictureBox1.Image = CurMap;
        }


        public string GetRandom()
        {
            var rnd = new Random().Next(0, Word.Vocabulary.Count);
            var count = 0;
            foreach (var word in Word.Vocabulary)
            {

                if (rnd == count)
                {
                    return word.Value.RusTranslation[0];
                }
                    count++;
            }
            return "";
        }

        public string GetRandomEngl()
        {
            var rnd = new Random().Next(0, Word.Vocabulary.Count);
            var count = 0;
            foreach (var word in Word.Vocabulary)
            {

                if (rnd == count)
                {
                    return word.Key;
                }
                count++;
            }
            return "";
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
                if (!word.Rectangle.Contains(new Point(e.X, e.Y))) continue;

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
                if (count == 0)
                {

                    oneword.Add(new XAttribute("word", word.Key));
                    //  continue;
                }
                var time = new XElement("TimeCreation");
                time.Add(new XAttribute("time", DateTime.Now.ToString()));

                var timegoodanswer = new XElement("TimeGoodAnswer");
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


        public void SwitchAndInsertInQueue(KeyValuePair<string, VocabularyInfoWord> word)
        {
            switch (word.Value.Mode)
            {
                case Mode.DirectTranslation:
                    word.Value.Mode = Mode.BackTranslation;
                    break;
                case Mode.BackTranslation:
                    word.Value.Mode = Mode.WritingSpelilng;
                    break;
                case Mode.WritingSpelilng:
                    word.Value.Mode = Mode.Writing;
                    break;
                case Mode.Writing:
                    word.Value.Mode = Mode.DirectTranslation;
                    word.Value.WithoutMistakes = true;
                    break;
                 
            }
            TrainQueue.Enqueue(word);
        }


        public void SwitchOnNextSample(KeyValuePair<string, VocabularyInfoWord> word)
        {
            if (TrainQueue.Count > 0)
            {
                CurrentWord = TrainQueue.Dequeue();
                             
                switch (CurrentWord.Value.Mode)
                {
                    case Mode.DirectTranslation:
                        SwitchOnNextSampleRus(CurrentWord);
                        break;

                    case Mode.BackTranslation:
                        SwitchOnNextSampleEngl(CurrentWord);
                        break;

                    case Mode.WritingSpelilng:
                        DrawWriting(CurrentWord);
                        break;

                    case Mode.Writing:
                        DrawWriting(CurrentWord);
                        break;
                }
            }
        }

        public ListCharPoints Points; 

        public void DrawWriting(KeyValuePair<string, VocabularyInfoWord> word)
        {
          
            g.Clear(Color.White);
            
            Font drawFont = new Font("Arial", 40);

            SolidBrush drawBrushWord = new SolidBrush(word.Value.Color);

            PointF drawPoint = new PointF(pictureBox1.Width / 2 - ((int)g.MeasureString(word.Key, drawFont).Width / 2), 50.0F);

            Points = new ListCharPoints(drawPoint,pictureBox1.Width,pictureBox1.Height);

            NumCruTranslation = new Random().Next(0, word.Value.RusTranslation.Count - 1);
            g.DrawString(word.Value.RusTranslation[NumCruTranslation], drawFont, drawBrushWord, drawPoint);
            var engword=MixWord(word.Value.EnglTranslation);
            Points.FillCharPoints(engword);
            Points.DrawPointsOnConv(ref CurMap);
            pictureBox1.Image = CurMap;
        }


        public void Rewrite(char inputchar)
        {
            g.Clear(Color.White);

            Font drawFont = new Font("Arial", 30);

            SolidBrush drawBrushWord = new SolidBrush(CurrentWord.Value.Color);
            g.DrawString(CurrentWord.Value.RusTranslation[NumCruTranslation], drawFont, drawBrushWord,
                CurrentWord.Value.GetGraphPoint());
            if (CurrentWord.Value.Mode != Mode.Writing)
            {
                Points.DeleteCharFromCollection(inputchar);
                Points.DrawPointsOnConv(ref CurMap);
            }
        
            
            foreach (var latter in PrintedBuffer.PrinttedBuffer)
            {
                g.DrawString(latter.Char.ToString(), drawFont, drawBrushWord, latter.Point);
            }
            pictureBox1.Image = CurMap;
        }





        private Dictionary<int,char> MixWord(string word)
        {
            var rnd = new Random();
            var arrword = word.ToCharArray();
            var dic = new Dictionary<int,char>();
            while (true)
            {
                var val = rnd.Next(0, word.Length);
                if (!dic.ContainsKey(val))
                {
                    dic.Add(val,arrword[val]);
                }
                if(dic.Count==word.Length)break;
            }
            return dic;
        }
    
 

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
        
           if(TrainQueue.Count==0)this.Close();
         
            if (CurrentWord.Value.Mode == Mode.DirectTranslation)
            {
                if (
                    CurWords.Any(
                        curword =>
                            curword.PlaceWord.Contains(e.X, e.Y) &&
                             CurrentWord.Value.RusTranslation[NumCruTranslation].Contains(curword.Value)))
                {
                    SwitchAndInsertInQueue(CurrentWord);
                    SwitchOnNextSample(CurrentWord);
                    return;
                }
                    
            }
            else if(CurWords.Any(curword => curword.PlaceWord.Contains(e.X, e.Y) && curword.RelVal == CurrentWord.Value.EnglTranslation))
            {
                SwitchAndInsertInQueue(CurrentWord);
                SwitchOnNextSample(CurrentWord);
                return;
                //SwitchOnNextSample(CurrentWord);
            }
            CurrentWord.Value.WithoutMistakes = false;
        }

        PointF drawPoint = new PointF(50.0F, 250.0F);

       

      

        private void button1_Click(object sender, EventArgs e)
        {
            var wordsCount = Convert.ToInt16("2");
            InitQueue(wordsCount);
        }


       

        private void TrainMemory_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*
            if (CurrentWord.Value.Mode != Mode.Writing)
            {
                if (CurrentWord.Value.Mode != Mode.WritingSpelilng) return;
            }
            */

         
            Font drawFont = new Font("Arial", 40);
            bool flagend = false;

            if (e.KeyChar == '1')
            {
                SwitchOnNextSample(CurrentWord);
                XmlHandler.GetInstanse().SaveWordAsSuccesfull(CurrentWord.Key);
                return;
            }


            if (e.KeyChar=='\b')
            {
                PrintedBuffer.RemoveLast();
                drawPoint.X -= g.MeasureString(e.KeyChar.ToString(), drawFont).Width - 1;
                Rewrite(e.KeyChar);

                return;
            }
            else if(e.KeyChar=='\r')
            {
                SwitchOnNextSample(CurrentWord);
                PrintedBuffer.Clear();
                drawPoint.X = 20;
                drawPoint.Y -= 50;
                flagend = true;


            }
            else
            {
             if (CurrentWord.Value.Mode == Mode.WritingSpelilng) Rewrite(e.KeyChar);
            }
         
            SolidBrush drawBrushWord = new SolidBrush(Color.ForestGreen);
            
            PrintedBuffer.PrinttedBuffer.Add(new CharPoint(e.KeyChar, new PointF(drawPoint.X,drawPoint.Y)));
            g.DrawString(e.KeyChar.ToString(), drawFont, drawBrushWord, drawPoint);
            drawPoint.X += g.MeasureString(e.KeyChar.ToString(), drawFont).Width - 1;
            //  if (e.KeyChar != (char)32)
            if(!flagend)PrintedBuffer.StrBuilder.Append(e.KeyChar.ToString());
                        
            var liststr = CurrentWord.Key.ToCharArray();
            var data ="";
            for(int i=0;i<liststr.Length;i++)
            {
                //if (i == liststr.Length - 1 && liststr[i] ==' ') continue;
                    data += liststr[i];
            }
            if (PrintedBuffer.StrBuilder.Length>=data.Length)
            {
                drawPoint.Y += 50;

                if (!PrintedBuffer.StrBuilder.ToString().ToLower().Contains(data.ToLower()))
                {
                    TrainQueue.Enqueue(CurrentWord);
                    drawBrushWord = new SolidBrush(Color.Red);
                    CurrentWord.Value.Mode = Mode.WritingSpelilng;
                    CurrentWord.Value.WithoutMistakes = false;
                }
                else
                {

                    if (CurrentWord.Value.Mode == Mode.Writing && CurrentWord.Value.WithoutMistakes)
                    {
                        CurrentWord.Value.SecsesfulTime = DateTime.Now;
                        XmlHandler.GetInstanse().SaveWordAsSuccesfull(CurrentWord.Key);
                        CurrentWord.Value.SecsesfulTime = DateTime.Now;
                        Word.LearnWords.Add(CurrentWord.Value.EnglTranslation+" "+CurrentWord.Value.RusTranslation[0]);
                        Word.Vocabulary.Remove(CurrentWord.Key);
                        // return;
                    }
                    else if (CurrentWord.Value.Mode == Mode.Writing && !CurrentWord.Value.WithoutMistakes)
                    {
                        CurrentWord.Value.WithoutMistakes = true;
                        CurrentWord.Value.Mode = Mode.BackTranslation;
                        XmlHandler.GetInstanse().SaveWordAsWronAnswer(CurrentWord.Key);
                    }
                    else
                    {
                        SwitchAndInsertInQueue(CurrentWord);
                    }
                   
                    //    if (CurrentWord.Value.Mode == Mode.WritingSpelilng) TrainQueue.Enqueue(CurrentWord);

                }
                drawPoint.X = 20;
                g.DrawString(CurrentWord.Key, drawFont, drawBrushWord, drawPoint);
                
               if (TrainQueue.Count() > 0)
                {
                   PrintedBuffer.Clear();
                  
                }
            }
            pictureBox1.Image = CurMap;
        }

        private void TrainMemory_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TrainMemory_FormClosing(object sender, FormClosingEventArgs e)
        {
            TrainQueue.Clear();
        }

        private void TrainMemory_FormClosed(object sender, FormClosedEventArgs e)
        {
            TrainWas = true;
            while (TrainQueue.Count>0)
            {
                var word=TrainQueue.Dequeue();
                word.Value.Mode = Mode.DirectTranslation;
              
            }
            
            TrainQueue.Clear();
        }

        private void TrainMemory_Load(object sender, EventArgs e)
        {

        }

        private void TrainMemory_Load_1(object sender, EventArgs e)
        {

        }
    }
}

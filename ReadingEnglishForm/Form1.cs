using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ReadingEnglishForm.Properties;
using Path = System.IO.Path;

namespace ReadingEnglishForm
{
    public partial class Form1 : Form
    {
        public List<string> TextBook;
        public int DyesToRepeat = Settings.Default.DaysRepeat;
        List<KeyValuePair<string, VocabularyInfoWord>> listLearned = new List<KeyValuePair<string, VocabularyInfoWord>>();

        public Form1()
        {          
            InitializeComponent();           
            LoadDictionary();          
            timer1.Start();
            timer2.Start();
        }

        

        private void InitInfo()
        {
            if(Word.Vocabulary==null)return;
            listBox1.Items.Clear();
            listLearned.Clear();
            var info = XmlHandler.GetInstanse().GetInfo();
            foreach (var strinf in info)
            {
                this.listBox1.Items.Add(strinf);
            }
            DrawInfo();
        }

        public bool InitlatestWoardWas = false;

        public void DrawInfo()
        {

            var listwords = new List<string>();
            var builder = new StringBuilder();
            foreach (var word in listLearned)
            {
                builder.Append(word.Key);
                builder.Append("-");
                builder.Append(word.Value.RusTranslation[0]);
                builder.Append(" ");
                pictureText1.Colors.Add(word.Value.Color);
            }
            listwords.Add(builder.ToString());
            pictureText1.InsertWordsInMemory(listwords, true);
            InitlatestWoardWas = true;
            pictureText1.Draw();
            foreach (var learnword in Word.LearnWords)
            {
                listBox2.Items.Add(learnword);
            }
        }







        public Dictionary<string,VocabularyInfoWord> LoadVacabulary(string path)
        {

          var vocabulary = new Dictionary<string, VocabularyInfoWord>();

            XmlHandler.GetInstanse().LoadXml(path);
            // cycle through each child noed 
            foreach (XmlNode node in XmlHandler.GetInstanse().xDoc.DocumentElement.ChildNodes)
            {
                // first node is the url ... have to go to nexted loc node 
                foreach (XmlNode locNode in node)
                {
                    // thereare a couple child nodes here so only take data from node named loc 
                    if (locNode.Name == "Item")
                    {
                        var time = "";
                        var timewronganswer= "";
                        var rusword = "";
                        var englword = "";
                        var color = Color.Blue;
                        var count = 0;

                        var atributs = locNode.Attributes;

                        foreach (var tnode in locNode)
                        {
                            var d = (XmlNode) tnode;
                            if (d.Name == "TimeGoodAnswer")
                            {
                                foreach (XmlAttribute anode in d.Attributes)
                                {
                                    time = anode.Value;
                                }
                            }
                            if (d.Name == "TimeWrongAnswer")
                            {
                                foreach (XmlAttribute anode in d.Attributes)
                                {
                                     timewronganswer= anode.Value;
                                }
                            }
                            if (d.Name == "Color")
                            {
                                foreach (XmlAttribute anode in d.Attributes)
                                {
                                    // anode.Value = DateTime.Now.ToString();
                                    var c=anode.Value.Split(new char[] { ']','['}, StringSplitOptions.RemoveEmptyEntries);
                                    color = Color.FromName(c[1]);
                                }
                            }
                            if (d.Name == "SuccessCount")
                            {
                                foreach (XmlAttribute anode in d.Attributes)
                                {
                                    // anode.Value = DateTime.Now.ToString();
                                    var c = anode.Value.Split(new char[] { ']', '[' }, StringSplitOptions.RemoveEmptyEntries);
                                    count = Convert.ToInt32(c[0]);
                                }
                            }
                        }

                        foreach (var atribut in atributs)
                        {
                            var sf = (XmlNode) atribut;
                            if (sf.LocalName == "engword") englword = sf.InnerText.ToLower();
                            if (sf.LocalName == "russword") rusword = sf.InnerText.ToLower();
                        }

                        if (!vocabulary.ContainsKey(englword))
                        {
                            vocabulary.Add(englword, new VocabularyInfoWord(rusword, time, color));
                            vocabulary[englword].SecsesfulCount = count;
                            vocabulary[englword].SetWrongTime(timewronganswer);
                        }
                           

                    }
                }
            }
            return vocabulary;
        }

        public void LoadDictionary()
        {
            Encoding utf8 = Encoding.GetEncoding("utf-8");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "slovnyk.csv");
            var listwords = File.ReadAllLines(path, utf8).ToList();
            if (Word.EDictionary == null) Word.EDictionary = new Dictionary<string, InfoWord>();
            for (int i = 88; i < listwords.Count; i++)
            {
                var word = listwords[i];
                var eng = word.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (eng.Length < 4) continue;
                eng[0] = eng[2].Split(new char[] { '\\', '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var transl = eng[0].Split(new char[] { ' ' });
                var verb = eng[0];
                if (transl.Count() > 1 && transl[0].ToLower() == "to")
                {
                    verb = "";
                    eng[0] = transl[1];
                    for (int j = 0; j < transl.Count(); j++)
                    {
                        verb += transl[j] + " ";
                    }
                }
                eng[1] = eng[3].Split(new char[] { '\\', '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (!Word.EDictionary.ContainsKey(eng[0].ToLower()))
                {
                    Word.EDictionary.Add(eng[0].ToLower(), new InfoWord());
                    Word.EDictionary[eng[0].ToLower()].AddValue(verb, eng[1]);
                }
                else
                {
                    Word.EDictionary[eng[0].ToLower()].AddValue(verb, eng[1]);
                }
            }
        }
     

        public string initalDir = "C:\\Users\\ASUS\\Desktop\\IELTS";
        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = initalDir;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Encoding win1251 = Encoding.GetEncoding("windows-1251");
                TextBook = File.ReadAllLines(ofd.FileName, win1251).ToList();
                pictureBox1.InsertWordsInMemory(TextBook, true);
            }
            this.label5.Text = pictureBox1.CountWords.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TextBook != null)
            {
                pictureBox1.MouseMoved();
                pictureBox1.CheckWord(pictureBox1.PointToClient(Cursor.Position));
               
            }
        }


        public List<OpenWord> ListWindows = new List<OpenWord>();

        private void MouseClick(object sender, MouseEventArgs e)
        {
            var res = pictureBox1.GetTranslation(e.X, e.Y);
            if (res == null || !res.IsInVacabulary)
            {
                return;
            }
            OpenWord f = new OpenWord(res);
            ListWindows.Add(f);
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TrainMemory f = new TrainMemory();
            f.Show();
        }

        public List<string> PDFBook = new List<string>();
        public int CurrentPDFPage = 0;

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "C:\\Users\\ASUS\\Downloads";


            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var reader = new PdfReader(ofd.FileName);

                // не забываем, что нумерация страниц в PDF начинается с единицы.
                int pagecount = reader.NumberOfPages;
                PDFBook.Clear();
                PDFBook.Add("0");

                for (int i = 1; i <= pagecount; ++i)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    String text = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                    PDFBook.Add(text);
                }
                reader.Close();
                DrawPDFPage(10);

            }
        }

        public void DrawPDFPage(int numPage)
        {

            TextBook = PDFBook[numPage].Split(new char[] {'\n'}).ToList();

            pictureBox1.InsertWordsInMemory(TextBook, true);
            label1.Text = CurrentPDFPage.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CurrentPDFPage--;
            DrawPDFPage(CurrentPDFPage);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CurrentPDFPage++;
            DrawPDFPage(CurrentPDFPage);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentPDFPage = Convert.ToInt32(textBox2.Text);
            }
            catch (Exception)
            {

            }
            DrawPDFPage(CurrentPDFPage);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                TrainMemory.NumberWords = Convert.ToInt16(this.textBox1.Text.ToString());
            }
            catch (Exception ex)
            {
            }


        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (TrainMemory.TrainWas)
            {
                InitInfo();
                TrainMemory.TrainWas = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
            Word.Vocabulary= LoadVacabulary(Settings.Default.IELTSVocabulary);
            Word.NameVocabulary = Settings.Default.IELTSVocabulary;
            InitInfo();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
           
            Word.Vocabulary = LoadVacabulary(Settings.Default.Vocabulary);
            Word.NameVocabulary = Settings.Default.EDictionary;
            InitInfo();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
             ViewVocabulary f = new ViewVocabulary();
             f.Show();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            var corect = new SimpleFrom();
            //corect.MdiParent = this;
            corect.Show();
        }
    }

}

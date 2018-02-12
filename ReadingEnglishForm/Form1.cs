using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using ReadingEnglishForm.Properties;

namespace ReadingEnglishForm
{
    public partial class Form1 : Form
    {
        public List<string> TextBook; 

        public Form1()
        {
            try
            {
                // new xdoc instance 
                XmlDocument xDoc = new XmlDocument();

                //load up the xml from the location 
                xDoc.Load("books5.xml");

                // cycle through each child noed 
                foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
                {
                    // first node is the url ... have to go to nexted loc node 
                    foreach (XmlNode locNode in node)
                    {
                        // thereare a couple child nodes here so only take data from node named loc 
                        if (locNode.Name == "Item")
                        {
                            // get the content of the loc node 

                            var atributs = locNode.Attributes;
                            foreach (var atribut in atributs)
                            {
                                var sf=(XmlNode)atribut;
                               var iner= sf.InnerText;
                               var name = sf.LocalName;

                            }

                        }
                    }
                }
            }
            catch
            {
            }






            // Create a writer to write XML to the console.
            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Indent = true;
            //settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create("booksy.xml");

            // Write the book element.
            writer.WriteStartElement("root");

            writer.WriteStartElement("world");
            writer.WriteAttributeString("name", "make");

            // Write the title element.
            writer.WriteStartElement("Item");
            writer.WriteAttributeString("engword","to_make_up");
            writer.WriteAttributeString("russword","красится");

            writer.WriteStartElement("TimeSecsess");
            writer.WriteAttributeString("time", DateTime.Now.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("Item");
            writer.WriteAttributeString("engword","to_a_war", "воевать");
            writer.WriteAttributeString("russword", "красится");
            writer.WriteEndElement();

            writer.WriteStartElement("TimeCreation");
            writer.WriteAttributeString("time", DateTime.Now.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("TimeSecsess");
            writer.WriteAttributeString("time", DateTime.Now.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();



            writer.WriteStartElement("world");
            writer.WriteAttributeString("name", "map");

            // Write the title element.
            writer.WriteStartElement("Item");
            writer.WriteAttributeString("to_map_up", "красится");
            writer.WriteEndElement();

            writer.WriteStartElement("Item");
            writer.WriteAttributeString("to_map", "воевать");
            writer.WriteEndElement();

            writer.WriteStartElement("TimeCreation");
            writer.WriteAttributeString("time", DateTime.Now.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("TimeSecsess");
            writer.WriteAttributeString("time", DateTime.Now.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            // Write the XML and close the writer.
            writer.Close();


            InitializeComponent();
            LoadVacabulary();
            LoadFileEDictionary();
           
            LoadDictionary();
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void LoadVacabulary()
        {
            XmlDocument xDoc = new XmlDocument();
            if (Word.Vocabulary == null) Word.Vocabulary = new Dictionary<string, VocabularyInfoWord>();
            //load up the xml from the location 
            xDoc.Load(Settings.Default.Vocabulary);

            // cycle through each child noed 
            foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
            {
                // first node is the url ... have to go to nexted loc node 
                foreach (XmlNode locNode in node)
                {
                    // thereare a couple child nodes here so only take data from node named loc 
                    if (locNode.Name == "Item")
                    {
                        var time = "";
                        var rusword = "";
                        var englword = "";

                        var atributs = locNode.Attributes;

                        foreach (var tnode in locNode)
                        {
                            var d = (XmlNode) tnode;
                            if (d.Name == "TimeGoodAnswer")
                            {
                                time = d.InnerText;
                            }
                        }

                        foreach (var atribut in atributs)
                        {
                            var sf = (XmlNode)atribut;
                            if (sf.LocalName == "engword") englword = sf.InnerText;
                            if (sf.LocalName == "russword") rusword = sf.InnerText;
                        }

                        if(!Word.Vocabulary.ContainsKey(englword))Word.Vocabulary.Add(englword,new VocabularyInfoWord(rusword,time));

                    }
                }
            }
        }

        public void LoadDictionary()
        {
            Encoding utf8= Encoding.GetEncoding("utf-8");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "slovnyk.csv");
            var listwords = File.ReadAllLines(path, utf8).ToList();
            if (Word.EDictionary == null) Word.EDictionary = new Dictionary<string, InfoWord>();
            for (int i = 88; i < listwords.Count; i++)
               // for (int i=98422;i<listwords.Count;i++)
            {
                var word = listwords[i];
                var eng = word.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (eng.Length < 4) continue;
                eng[0] = eng[2].Split(new char[] { '\\','"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                var transl=eng[0].Split(new char[] {' '});
                var verb = eng[0];
                if (transl.Count() > 1&& transl[0].ToLower() == "to")
                {
                    verb = "";
                    eng[0]= transl[1];
                    for (int j = 0; j < transl.Count(); j++)
                    {
                        verb += transl[j] + " ";
                    }
                }
                eng[1] = eng[3].Split(new char[] { '\\', '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (!Word.EDictionary.ContainsKey(eng[0].ToLower()))
                {
                    Word.EDictionary.Add(eng[0].ToLower(), new InfoWord());
                    Word.EDictionary[eng[0].ToLower()].AddValue(verb,eng[1]);
                }
                else
                {
                    Word.EDictionary[eng[0].ToLower()].AddValue(verb,eng[1]);
                }
            }
        }

        public void LoadFileEDictionary()
        {
            //Encoding win1251 = Encoding.GetEncoding("windows-1251");
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "EDictionary.txt");
            //var listwords = File.ReadAllLines(path, win1251).ToList();
            //if(Word.EDictionary==null)Word.EDictionary= new Dictionary<string, InfoWord>();
            //foreach (var word in listwords)
            //{
            //    var eng= word.Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
            //    if (eng.Length <= 1) continue;
            //    eng[0] = eng[0].Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)[0];
            //    if (!Word.EDictionary.ContainsKey(eng[0].ToLower()))
            //    {
            //        Word.EDictionary.Add(eng[0].ToLower(),new List<string>() { eng[1]});
            //    }
            //    else
            //    {
            //        Word.EDictionary[eng[0]].Add(eng[1]);
            //    }
            //}

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Encoding win1251 = Encoding.GetEncoding("windows-1251");
                TextBook= File.ReadAllLines(ofd.FileName, win1251).ToList();
                pictureBox1.WriteListString(TextBook,true);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TextBook != null)
            {
               // pictureBox1.WriteListString(TextBook,false);
                pictureBox1.MouseMoved();
                pictureBox1.CheckWord(pictureBox1.PointToClient(Cursor.Position));
            }
        }

        //private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (TextBook != null)
        //    {
        //        // pictureBox1.WriteListString(TextBook);
               
        //    }
        //}

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
          
        }
        public List<OpenWord> ListWindows=new List<OpenWord>();

        private void MouseClick(object sender, MouseEventArgs e)
        {
            var res=pictureBox1.GetTranslation(e.X, e.Y);
            if (res==null||!res.IsInVacabulary)
            {
                return;
            }
            OpenWord f = new OpenWord(res);
            ListWindows.Add(f);
         //   f.MdiParent = this;
            f.Show();


        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
        
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            TrainMemory f = new TrainMemory();
            f.Show();
        }
    }
}

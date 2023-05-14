using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReadingEnglishForm
{
    public class XmlHandler
    {
        public XmlDocument xDoc = new XmlDocument();
        public XDocument Document;
        public string path;
        private static XmlHandler _curHandler;
        

        private XmlHandler()
        {
            
        }

        public static XmlHandler GetInstanse()
        {
            if(_curHandler==null)_curHandler= new XmlHandler();
            return _curHandler;
        }


        public List<string> GetColors()
        {
            var lstring = new List<string>();
            var count = 0;
            foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
            {
                var color=Color.FromName(kc.ToString());
                var  hsb=color.GetBrightness();
                var c1=(color.R + color.B + color.G)/3;
                if (c1<181)
                {
                    lstring.Add(kc.ToString());
                }
               
                count++;
            }
            return lstring;
        }

        public void LoadXml(string path)
        {
            this.path = path;
            xDoc.Load(path);
            Document=XDocument.Load(path);
        }

        public List<KeyValuePair<string,VocabularyInfoWord>> ForChecking= new List<KeyValuePair<string, VocabularyInfoWord>>(); 

        public List<string> GetInfo()
        {
            var list = new List<string>();
            if (Word.Vocabulary == null) return null;
            var str = "Всего слов: " + Word.Vocabulary.Count.ToString();
            //  this.listBox1.Items.Add(str);
            ForChecking.Clear();
            var time = DateTime.Now;
            var count = 0;
            var count2 = 0;
            foreach (var word in Word.Vocabulary)
            {
                var days = Math.Round((time - word.Value.SecsesfulTime).TotalDays,0);
                var dayssincemistake = Math.Round((time - word.Value.WrongTime).TotalDays, 0);
                var num = word.Value.SecsesfulCount;
                var year = word.Value.SecsesfulTime.Year;
                if (year > 2017)
                {
                    count++;
                }
                if (dayssincemistake >= Math.Pow(2.5,num)-1&&days>3)
                {
                    ForChecking.Add(word);
                    count2++;
                }
              
            }
            var str2 = "Выучино слов: " + count.ToString();
            var str3 = "Надо повторить: " + count2.ToString();
            list.Add(str);
            list.Add(str2);
            list.Add(str3);
            return list;

        } 

        public void SaveWordAsSuccesfull(string curword)
        {

            var word=Word.Vocabulary[curword];
            foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
            {
                // first node is the url ... have to go to nexted loc node 
                bool was = false;
                foreach (XmlNode locNode in node)
                {
                    // thereare a couple child nodes here so only take data from node named loc 
                    if (locNode.Name == "Item")
                    {

                        var time = "";
                        var rusword = "";
                        var englword = "";
                        
                       
                        var atributs = locNode.Attributes;
                        foreach (var atribut in atributs)
                        {
                            var sf = (XmlNode)atribut;
                            if (sf.LocalName == "engword")
                            {
                                englword = sf.InnerText.ToLower();
                                if (englword == curword)
                                {
                                    was = true;
                                    break;
                                }
                               
                            }
                        }
                        if(!was)continue;
                        }
                    foreach (var tnode in locNode)
                    {
                        var d = (XmlNode) tnode;
                        if (d.Name == "TimeGoodAnswer")
                        {

                            foreach (XmlAttribute anode in d.Attributes)
                            {
                                anode.Value = DateTime.Now.ToString();
                            }
                        }
                        if (d.Name == "SuccessCount")
                        {

                            foreach (XmlAttribute anode in d.Attributes)
                            {
                                var cnum0 = word.SecsesfulCount;
                                cnum0++;
                                anode.Value = cnum0.ToString();
                            }
                        }


                    }
                    if (was)
                    {
                        xDoc.Save(path);
                        return;
                    }
                }
                
            }
          
        }

        public void CreateNewWord(Dictionary<string, List<string>> WordsForEDictionary)
        {
            var Colors = GetColors();
            var namefile = Word.NameVocabulary;
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

                var timegoodanswer = new XElement("TimeGoodAnswer");
                timegoodanswer.Add(new XAttribute("time", " "));

                var timeWrongAnswer = new XElement("TimeWrongAnswer");
                timeWrongAnswer.Add(new XAttribute("time", " "));



                var n = new Random().Next(0, Colors.Count - 1);
                var color = new XElement("Color");
                color.Add(new XAttribute("color", Color.FromName(Colors[n])));
              
                var succsesscount = new XElement("SuccessCount");
                succsesscount.Add(new XAttribute("count", 0));


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
                Item.Add(timeWrongAnswer);
                Item.Add(color);
                Item.Add(succsesscount);


                oneword.Add(Item);

                count++;
            }
            school.Add(oneword);
            doc1.Save(namefile);
        }

        public void SaveWordAsWronAnswer(string curword)
        {

             foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
            {
                // first node is the url ... have to go to nexted loc node 
                bool was = false;
                foreach (XmlNode locNode in node)
                {
                    // thereare a couple child nodes here so only take data from node named loc 
                    if (locNode.Name == "Item")
                    {

                        var time = "";
                        var rusword = "";
                        var englword = "";


                        var atributs = locNode.Attributes;
                        foreach (var atribut in atributs)
                        {
                            var sf = (XmlNode)atribut;
                            if (sf.LocalName == "engword")
                            {
                                englword = sf.InnerText.ToLower();
                                if (englword == curword)
                                {
                                    was = true;
                                    break;
                                }

                            }
                        }
                        if (!was) continue;
                    }
                    foreach (var tnode in locNode)
                    {
                        var d = (XmlNode)tnode;
                        if (d.Name == "TimeWrongAnswer")
                        {

                            foreach (XmlAttribute anode in d.Attributes)
                            {
                                anode.Value = DateTime.Now.ToString();
                            }
                        }

                        if (d.Name == "SuccessCount")
                        {

                            foreach (XmlAttribute anode in d.Attributes)
                            {
                                var count = Convert.ToInt16(anode.Value);
                                count -=1;
                                anode.Value =count.ToString();
                            }
                        }



                    }
                    if (was)
                    {
                        xDoc.Save(path);
                        return;
                    }
                }

            }

        }

        public void AddToEDictionary(Dictionary<string, List<string>> WordsForEDictionary)
        {
            var Colors = GetColors();

            XDocument doc1 = Document;
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

                var timegoodanswer = new XElement("TimeGoodAnswer");
                timegoodanswer.Add(new XAttribute("time", " "));

                var timeWrongAnswer = new XElement("TimeWrongAnswer");
                timeWrongAnswer.Add(new XAttribute("time", " "));



                var n = new Random().Next(0, Colors.Count - 1);
                var color = new XElement("Color");
                color.Add(new XAttribute("color", Color.FromName(Colors[n])));


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
                Item.Add(timeWrongAnswer);
                Item.Add(color);

                oneword.Add(Item);

                count++;
            }
            school.Add(oneword);
         //   doc1.Save(namefile);

        }
    }
}

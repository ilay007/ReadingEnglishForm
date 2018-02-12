using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEnglishForm
{
    public class WordInRectangle
    {
        public string WordValue;
        public Rectangle Rectangle;

        public WordInRectangle(string wordValue, Rectangle rec)
        {
            WordValue= wordValue;
            Rectangle = rec;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEnglishForm
{
    public class CharPoint
    {
        public PointF Point;
        public char Char;

        public CharPoint(char onechar, PointF point)
        {
            Point = point;
            Char = onechar;
        }

    }
}

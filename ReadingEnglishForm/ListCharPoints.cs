using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEnglishForm
{
    public class ListCharPoints
    {
        public List<CharPoint> CharPoints=new List<CharPoint>();
        private PointF _startPoint;
        private float delx;
        private float dely;
        private Font drawFont = new Font("Arial", 40);
        private Graphics g;
        private float _maxX;
        private float _maxY;
        SolidBrush drawBrushWord = new SolidBrush(Color.BlueViolet);

        public ListCharPoints(PointF startPoint, float maxX,float maxY)
        {
            _startPoint = new PointF();
            _startPoint.X = 40;
            var _curMap = new Bitmap(Convert.ToInt16(maxX), Convert.ToInt16(maxY));
            g = Graphics.FromImage(_curMap);
            delx = (float)g.MeasureString("A", drawFont).Width;
            dely = (float)g.MeasureString("A", drawFont).Height;
          
            _startPoint.Y =startPoint.Y + dely;
            _maxX = maxX;
            _maxY = maxY;
        }

        public void DrawPointsOnConv(ref Bitmap bmp)
        {
            g = Graphics.FromImage(bmp);
         
            foreach (var point in CharPoints)
            {
                g.DrawString(point.Char.ToString(), drawFont, drawBrushWord, point.Point);
                
            }
        }

        public void FillCharPoints(Dictionary<int, char> dic)
        {
            int count = 0;
            foreach (var val in dic)
            {
              var xst= _startPoint.X + count*delx;
              var yst = _startPoint.Y;
                if (xst < _maxX)
                {
                    CharPoints.Add(new CharPoint(val.Value, new PointF(xst, yst)));
                }
                count++;

            }
        }

        public void DeleteCharFromCollection(char inputchar)
        {
           var res= CharPoints.FirstOrDefault(x => x.Char == inputchar);
            if (res != null)
            {
                CharPoints.Remove(res);
            }
        }
    }
}

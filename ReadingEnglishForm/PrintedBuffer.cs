
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEnglishForm
{
    public class PrintedBuffer
    {

        public List<CharPoint> PrinttedBuffer = new List<CharPoint>();
        public StringBuilder StrBuilder = new StringBuilder();


        public void Clear()
        {
            PrinttedBuffer.Clear();
            StrBuilder.Clear();
        }

        public void RemoveLast()
        {
            if (PrinttedBuffer.Count() > 1)
            {
                PrinttedBuffer.RemoveAt(PrinttedBuffer.Count-1);
                 StrBuilder.Remove(StrBuilder.Length - 1, 1);
            }
        }
    }
}

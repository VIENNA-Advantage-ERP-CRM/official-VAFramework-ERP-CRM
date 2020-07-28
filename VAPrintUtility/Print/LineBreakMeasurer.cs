using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace VAdvantage.Print
{
    /// <summary>
    /// 
    /// </summary>
    public class LineBreakMeasurer
    {
        private int start;
        private int pos;
        private int tempPos;
        private int limit;
        private int characterCount;
        private string title;
        //private Graphics g;
        public static char DONE = '\uFFFF';

        // characters in source text
        private char[] fChars;
        private Color stringColor;

        public LineBreakMeasurer(AttributedCharacterIterator text)
        {
            //GetGraphics();
            if (text.GetEndIndex() - text.GetBeginIndex() < 1)
            {
                throw new ArgumentException("Text must contain at least one character.");
            }

            this.limit = text.GetEndIndex();
            this.pos = this.start = text.GetBeginIndex();

            // extract chars
            fChars = new char[text.GetEndIndex() - text.GetBeginIndex()];
            int n = 0;
            for (char c = text.First(); c != DONE; c = text.Next())
            {
                fChars[n++] = c;
            }
            text.First();

            stringFont = (Font)text.GetAttribute(TextAttribute.FONT);
            stringColor = (Color)text.GetAttribute(TextAttribute.FOREGROUND);
            characterCount = fChars.Length;
        }

        public TextLayout NextLayout(float wrappingWidth)
        {
            return NextLayout(wrappingWidth, limit);
        }

        /// <summary>
        /// Returns the next layout, and updates the current position.
        /// </summary>
        /// <param name="wrappingWidth"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public TextLayout NextLayout(float wrappingWidth, float limit)
        {
            int layoutLimit = NextOffset(wrappingWidth, (int)limit);
            if (layoutLimit == pos)
            {
                return null;
            }
            TextLayout result = this.GetLayout(tempPos, layoutLimit);
            tempPos = layoutLimit;
            pos = layoutLimit;
            return result;

        }

        public TextLayout GetLayout(int start, int limit)
        {
            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= fChars.Length - 1; i++)
                sb.Append(fChars[i].ToString());
            title = sb.ToString();
            return new TextLayout(sb.ToString(), start, limit, stringFont, stringColor);
        }



        private Font stringFont;
        public System.Drawing.Font GetFont()
        {
            return stringFont;
        }

        public int NextOffset(float wrappingWidth, int offsetLimit)
        {

            int nextOffset = tempPos;
            if (tempPos < limit)
            {
                if (offsetLimit <= tempPos)
                {
                    throw new ArgumentException("offsetLimit must be after current position");
                }
            }

            bool blStatus = false;
            int counter = tempPos;
            StringBuilder newStr = new StringBuilder();
            for (int i = tempPos; i <= fChars.Length - 1; i++)
            {
                
                newStr.Append(fChars[i].ToString());
                SizeF extent = TextRenderer.MeasureText(newStr.ToString(), stringFont);
                if (!wrappingWidth.Equals(0.0f))
                {
                    float wRatio = (wrappingWidth) / (extent.Width);
                    counter = i;
                    if (wRatio < 1f)
                    {
                        //Issue (Major) : If the colwidth is fixed and less than the 
                        //actual required, Divide by Zero error will orccur.

                        //for that we have manually set the values for various positions

                        blStatus = true;
                        if (i == 0) //we will always return offset value always > 0 (so to avoid divide by zero exception)
                            if (i == tempPos)
                                nextOffset = 1 + tempPos;
                            else
                                nextOffset = 1;
                        else
                        {
                            if (i == tempPos)
                                nextOffset = 1 + tempPos;
                            else
                                nextOffset = i;
                        }
                        break;
                    }
                }
            }

            if (!blStatus)
                nextOffset = counter + 1;

            //g.Dispose();
            return nextOffset;
        }

        public int GetPosition()
        {
            return pos;
        }


    }
}

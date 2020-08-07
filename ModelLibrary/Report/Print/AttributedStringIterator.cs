using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    /// <summary>
    /// Attribute String Iterator
    /// </summary>
    public sealed class AttributedStringIterator : AttributedCharacterIterator
    {
        // note on synchronization:
        // we don't synchronize on the iterator, assuming that an iterator is only used in one thread.
        // we do synchronize access to the AttributedString however, since it's more likely to be shared between threads.

        // start and end index for our iteration
        private int beginIndex;
        private int endIndex;

        // attributes that our client is interested in
        private Attributes[] relevantAttributes;

        // the current index for our iteration
        // invariant: beginIndex <= currentIndex <= endIndex
        private int currentIndex;

        // information about the run that includes currentIndex
        private int currentRunIndex;
        private int currentRunStart;
        private int currentRunLimit;
        private AttributedString attString;
        public static char DONE = '\uFFFF';

        // constructor
        public AttributedStringIterator(Attributes[] attributes, int beginIndex, int endIndex, AttributedString aString)
        {
            attString = aString;

            if (beginIndex < 0 || beginIndex > endIndex || endIndex > attString.Length())
            {
                throw new ArgumentException("Invalid substring range");
            }

            this.beginIndex = beginIndex;
            this.endIndex = endIndex;
            this.currentIndex = beginIndex;
            UpdateRunInfo();
            if (attributes != null)
            {
                relevantAttributes = (Attributes[])attributes.Clone();
            }
        }


        //public override bool Equals(object obj)
        //{
        //    return base.Equals(obj);
        //}

        public string GetText()
        {
            return attString.GetText();
        }

        public System.Drawing.Font GetFont()
        {
            return (System.Drawing.Font)attString.GetAttribute(TextAttribute.FONT);
        }

        public System.Drawing.Color GetColor()
        {
            return (System.Drawing.Color)attString.GetAttribute(TextAttribute.FOREGROUND);
        }


        private void UpdateRunInfo()
        {
            if (currentIndex == endIndex)
            {
                currentRunStart = currentRunLimit = endIndex;
                currentRunIndex = -1;
            }
            else
            {
                lock (this)
                {
                    int runIndex = -1;
                    while (runIndex < attString.runCount - 1 && attString.runStarts[runIndex + 1] <= currentIndex)
                        runIndex++;
                    currentRunIndex = runIndex;
                    if (runIndex >= 0)
                    {
                        currentRunStart = attString.runStarts[runIndex];
                        if (currentRunStart < beginIndex)
                            currentRunStart = beginIndex;
                    }
                    else
                    {
                        currentRunStart = beginIndex;
                    }
                    if (runIndex < attString.runCount - 1)
                    {
                        currentRunLimit = attString.runStarts[runIndex + 1];
                        if (currentRunLimit > endIndex)
                            currentRunLimit = endIndex;
                    }
                    else
                    {
                        currentRunLimit = endIndex;
                    }
                }
            }
        }


        public char First()
        {
            return InternalSetIndex(beginIndex);
        }

        private char InternalSetIndex(int position)
        {
            currentIndex = position;
            if (position < currentRunStart || position >= currentRunLimit)
            {
                UpdateRunInfo();
            }
            if (currentIndex == endIndex)
            {
                return DONE;
            }
            else
            {
                return attString.CharAt(position);
                //return charAt(position);
            }
        }

        public char Last()
        {
            if (endIndex == beginIndex)
            {
                return InternalSetIndex(endIndex);
            }
            else
            {
                return InternalSetIndex(endIndex - 1);
            }
        }

        public char Current()
        {
            if (currentIndex == endIndex)
            {
                return DONE;
            }
            else
            {
                return attString.CharAt(currentIndex);
            }
        }

        public char Next()
        {
            if (currentIndex < endIndex)
            {
                return InternalSetIndex(currentIndex + 1);
            }
            else
            {
                return DONE;
            }
        }

        public char Previous()
        {
            if (currentIndex > beginIndex)
            {
                return InternalSetIndex(currentIndex - 1);
            }
            else
            {
                return DONE;
            }
        }

        public char SetIndex(int position)
        {
            if (position < beginIndex || position > endIndex)
                throw new ArgumentException("Invalid index");
            return InternalSetIndex(position);
        }

        public int GetBeginIndex()
        {
            return beginIndex;
        }

        public int GetEndIndex()
        {
            return endIndex;
        }

        public int GetIndex()
        {
            return currentIndex;
        }

        // AttributedCharacterIterator methods. See documentation in that interface.

        public int GetRunStart()
        {
            return currentRunStart;
        }

        public int GetRunLimit()
        {
            return currentRunLimit;
        }

        public object Clone()
        {
            try
            {
                AttributedStringIterator other = (AttributedStringIterator)base.MemberwiseClone();
                return other;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Object GetAttribute(TextAttribute textAtt)
        {
            return attString.GetAttribute(textAtt);
        }
    }
}

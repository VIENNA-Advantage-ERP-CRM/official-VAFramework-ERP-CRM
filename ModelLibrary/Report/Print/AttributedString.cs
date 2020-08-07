using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;

namespace VAdvantage.Print
{
    /// <summary>
    /// An AttributedString holds text and related attribute information. 
    /// It may be used as the actual data storage in some cases where a text reader 
    /// wants to access attributed text through the AttributedCharacterIterator interface. 
    /// </summary>
    public class AttributedString
    {
        // field holding the text
        String text;

        public int runArraySize;               // current size of the arrays
        public int runCount;                   // actual number of runs, <= runArraySize
        public int[] runStarts;                // start index for each run

        public ArrayList[] runAttributes;         // ArrayList of attribute keys for each run
        public ArrayList[] runAttributeValues;    // parallel ArrayList of attribute values for each run

        /// <summary>
        ///  Constructs an AttributedString instance with the given text.
        /// </summary>
        /// <param name="text">The text for this attributed string.</param>
        public AttributedString(String text)
        {
            if (text == null)
            {
                throw new NullReferenceException();
            }
            this.text = text;
        }

        public string GetText()
        {
            return this.text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void AddAttribute(Attributes attribute, Object value)
        {
            int len = Length();
            if (len == 0)
            {
                throw new ArgumentException("Can't add attribute to 0-length text");
            }

            AddAttributeImpl(attribute, value, 0, len);

        }

        public void AddAttribute(Attributes attribute, Object value, int beginIndex, int endIndex)
        {
            if (attribute == null)
            {
                throw new NullReferenceException();
            }

            if (beginIndex < 0 || endIndex > Length() || beginIndex >= endIndex)
            {
                throw new ArgumentException("Invalid substring range");
            }

            AddAttributeImpl(attribute, value, beginIndex, endIndex);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AddAttributeImpl(Attributes attribute, Object value, int beginIndex, int endIndex)
        {
            // make sure we have run attribute data vectors
            if (runCount == 0)
            {
                CreateRunAttributeDataVectors();
            }

            // break up runs if necessary
            int beginRunIndex = EnsureRunBreak(beginIndex);
            int endRunIndex = EnsureRunBreak(endIndex);

            AddAttributeRunData(attribute, value, beginRunIndex, endRunIndex);
        }

        // add the attribute attribute/value to all runs where beginRunIndex <= runIndex < endRunIndex
        private void AddAttributeRunData(Attributes attribute, Object value, int beginRunIndex, int endRunIndex)
        {

            for (int i = beginRunIndex; i < endRunIndex; i++)
            {
                int keyValueIndex = -1; // index of key and value in our vectors; assume we don't have an entry yet
                if (runAttributes[i] == null)
                {
                    ArrayList newRunAttributes = new ArrayList();
                    ArrayList newRunAttributeValues = new ArrayList();
                    runAttributes[i] = newRunAttributes;
                    runAttributeValues[i] = newRunAttributeValues;
                }
                else
                {
                    // check whether we have an entry already
                    keyValueIndex = runAttributes[i].IndexOf(attribute);
                }

                if (keyValueIndex == -1)
                {
                    // create new entry
                    int oldSize = runAttributes[i].Count;
                    runAttributes[i].Add(attribute);
                    try
                    {
                        runAttributeValues[i].Add(value);
                    }
                    catch 
                    {
                        runAttributes[i].Capacity = oldSize;
                        runAttributeValues[i].Capacity = oldSize;
                    }
                }
                else
                {
                    // update existing entry
                    runAttributeValues[i][keyValueIndex] = value;
                }
            }
        }

        public int Length()
        {
            return text.Length;
        }

        // since there are no vectors of int, we have to use arrays.
        // We allocate them in chunks of 10 elements so we don't have to allocate all the time.
        private static int ARRAY_SIZE_INCREMENT = 10;

        private void CreateRunAttributeDataVectors()
        {
            // use temporary variables so things remain consistent in case of an exception
            int[] newRunStarts = new int[ARRAY_SIZE_INCREMENT];
            ArrayList[] newRunAttributes = new ArrayList[ARRAY_SIZE_INCREMENT];
            ArrayList[] newRunAttributeValues = new ArrayList[ARRAY_SIZE_INCREMENT];
            runStarts = newRunStarts;
            runAttributes = newRunAttributes;
            runAttributeValues = newRunAttributeValues;
            runArraySize = ARRAY_SIZE_INCREMENT;
            runCount = 1; // assume initial run starting at index 0
        }

        public char CharAt(int index)
        {
            return text[index];
        }

        private int EnsureRunBreak(int offset)
        {
            return EnsureRunBreak(offset, true);
        }

        /// <summary>
        /// Ensures there is a run break at offset, returning the index of
        /// the run. If this results in splitting a run, two things can happen:
        /// <para>If copyAttrs is true, the attributes from the existing run  will be placed in both of the newly created runs.</para>
        /// <para>If copyAttrs is false, the attributes from the existing run will NOT be copied to the run to the right (>= offset) of the break, but will exist on the run to the left (&lt; offset).</para>
        /// </summary>
        /// <param name="offset">offset point</param>
        /// <param name="copyAttrs">copy attributes or not : default true</param>
        /// <returns>Break Point</returns>
        private int EnsureRunBreak(int offset, bool copyAttrs)
        {
            if (offset == Length())
            {
                return runCount;
            }

            // search for the run index where this offset should be
            int runIndex = 0;
            while (runIndex < runCount && runStarts[runIndex] < offset)
            {
                runIndex++;
            }

            // if the offset is at a run start already, we're done
            if (runIndex < runCount && runStarts[runIndex] == offset)
            {
                return runIndex;
            }

            // we'll have to break up a run
            // first, make sure we have enough space in our arrays
            if (runCount == runArraySize)
            {
                int newArraySize = runArraySize + ARRAY_SIZE_INCREMENT;
                int[] newRunStarts = new int[newArraySize];
                ArrayList[] newRunAttributes_ = new ArrayList[newArraySize];
                ArrayList[] newRunAttributeValues_ = new ArrayList[newArraySize];
                for (int i = 0; i < runArraySize; i++)
                {
                    newRunStarts[i] = runStarts[i];
                    newRunAttributes_[i] = runAttributes[i];
                    newRunAttributeValues_[i] = runAttributeValues[i];
                }
                runStarts = newRunStarts;
                runAttributes = newRunAttributes_;
                runAttributeValues = newRunAttributeValues_;
                runArraySize = newArraySize;
            }

            // make copies of the attribute information of the old run that the new one used to be part of
            // use temporary variables so things remain consistent in case of an exception
            ArrayList newRunAttributes = null;
            ArrayList newRunAttributeValues = null;

            if (copyAttrs)
            {
                ArrayList oldRunAttributes = runAttributes[runIndex - 1];
                ArrayList oldRunAttributeValues = runAttributeValues[runIndex - 1];
                if (oldRunAttributes != null)
                {
                    newRunAttributes = (ArrayList)oldRunAttributes.Clone();
                }
                if (oldRunAttributeValues != null)
                {
                    newRunAttributeValues = (ArrayList)oldRunAttributeValues.Clone();
                }
            }

            // now actually break up the run
            runCount++;
            for (int i = runCount - 1; i > runIndex; i--)
            {
                runStarts[i] = runStarts[i - 1];
                runAttributes[i] = runAttributes[i - 1];
                runAttributeValues[i] = runAttributeValues[i - 1];
            }
            runStarts[runIndex] = offset;
            runAttributes[runIndex] = newRunAttributes;
            runAttributeValues[runIndex] = newRunAttributeValues;

            return runIndex;
        }

        // returns whether the two objects are either both null or equal
        private static bool ValuesMatch(Object value1, Object value2)
        {
            if (value1 == null)
            {
                return value2 == null;
            }
            else
            {
                return value1.Equals(value2);
            }
        }

        private Object GetAttribute(Attributes attribute, int runIndex)
        {
            ArrayList currentRunAttributes = runAttributes[runIndex];
            ArrayList currentRunAttributeValues = runAttributeValues[runIndex];
            if (currentRunAttributes == null)
            {
                return null;
            }
            int attributeIndex = currentRunAttributes.IndexOf(attribute);
            if (attributeIndex != -1)
            {
                return currentRunAttributeValues[attributeIndex];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the added attributes
        /// </summary>
        /// <param name="textAtt">TextAttribute</param>
        /// <returns>Attribute as object</returns>
        public Object GetAttribute(TextAttribute textAtt)
        {
            for (int i = 0; i <= runAttributes[0].Count - 1; i++)
            {
                if (runAttributes[0][i] == textAtt)
                {
                    return runAttributeValues[0][i];
                }
            }
            return System.Drawing.Color.Empty;
        }

        public AttributedCharacterIterator GetIterator()
        {
            return GetIterator(null, 0, Length());
        }


        public AttributedCharacterIterator GetIterator(Attributes[] attributes, int beginIndex, int endIndex)
        {
            return new AttributedStringIterator(attributes, beginIndex, endIndex, this);
        }
    }


}

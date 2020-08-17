/********************************************************
 // Module Name    : Readonly-display logic
 // Purpose        : The string tokenizer class allows an application to break a 
                     string into tokens. this class return string token in two ways ..
                     either with set delims flag or without delim flag
 // Class Used     : -----------
 // Created By     : Harwinder 
 // Date           : -----
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Classes
{
    public class StringTokenizer
    {
        private int currentPosition;
        private int newPosition;
        private int maxPosition;
        private String str;
        private String delimiters;
        private bool retDelims;
        private bool delimsChanged;

        /**
      * maxDelimCodePoint stores the value of the delimiter character with the
      * highest value. It is used to optimize the detection of delimiter
      * characters.
      *
      * It is unlikely to provide any optimization benefit in the
      * hasSurrogates case because most string characters will be
      * smaller than the limit, but we keep it so that the two code
      * paths remain similar.
      */
        private int maxDelimCodePoint;

        /**
         * If delimiters include any surrogates (including surrogate
         * pairs), hasSurrogates is true and the tokenizer uses the
         * different code path. This is because String.indexOf(int)
         * doesn't handle unpaired surrogates as a single character.
         */
        private bool hasSurrogates = false;

        /**
         * When hasSurrogates is true, delimiters are converted to code
         * points and isDelimiter(int) is used to determine if the given
         * codepoint is a delimiter.
         */
        private int[] delimiterCodePoints;

        /**
         * Set maxDelimCodePoint to the highest char in the delimiter set.
         */

        private void SetMaxDelimCodePoint()
        {
            if (delimiters == null)
            {
                maxDelimCodePoint = 0;
                return;
            }

            int m = 0;
            int c;
            int count = 0;
            foreach (char chr in delimiters)
            {
                c = char.ConvertToUtf32(delimiters, count);
                if (char.IsSurrogate(chr))
                {

                    //    // c =     delimiters.code .codePointAt(i);
                    hasSurrogates = true;
                }
                if (m < c)
                    m = c;
                count++;
            }

            maxDelimCodePoint = m;

            if (hasSurrogates)
            {
                delimiterCodePoints = new int[0];
                //delimiterCodePoints = new int[count];
                //for (int i = 0, j = 0; i < count; i++, j++)
                //{
                //    c= char.ConvertToUtf32(delimiters,j);
                //    //c = Char.GetNumericValue( delimiters.codePointAt(j);
                //    delimiterCodePoints[i] =  c;
                //}
            }
        }


        /**
      * Constructs a string tokenizer for the specified string. All  
      * characters in the <code>delim</code> argument are the delimiters 
      * for separating tokens. 
      * <p>
      * If the <code>returnDelims</code> flag is <code>true</code>, then 
      * the delimiter characters are also returned as tokens. Each 
      * delimiter is returned as a string of length one. If the flag is 
      * <code>false</code>, the delimiter characters are skipped and only 
      * serve as separators between tokens. 
      * <p>
      * Note that if <tt>delim</tt> is <tt>null</tt>, this constructor does
      * not throw an exception. However, trying to invoke other methods on the
      * resulting <tt>StringTokenizer</tt> may result in a 
      * <tt>NullPointerException</tt>.
      *
      * @param   str            a string to be parsed.
      * @param   delim          the delimiters.
      * @param   returnDelims   flag indicating whether to return the delimiters
      *                         as tokens.
      * @exception NullPointerException if str is <CODE>null</CODE>
      */
        public StringTokenizer(String str, String delim, bool returnDelims)
        {
            currentPosition = 0;
            newPosition = -1;
            delimsChanged = false;
            this.str = str;
            maxPosition = str.Length;
            delimiters = delim;
            retDelims = returnDelims;
            SetMaxDelimCodePoint();
        }

        /// <summary>
        /// 
        /* Constructs a string tokenizer for the specified string. The 
         * characters in the <code>delim</code> argument are the delimiters 
         * for separating tokens. Delimiter characters themselves will not 
         * be treated as tokens.
         * <p>
         * Note that if <tt>delim</tt> is <tt>null</tt>, this constructor does
         * not throw an exception. However, trying to invoke other methods on the
         * resulting <tt>StringTokenizer</tt> may result in a
         * <tt>NullPointerException</tt>.
         */
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delim"></param>
        public StringTokenizer(String str, String delim)
            : this(str, delim, false)
        {
        }

        /**
        * Constructs a string tokenizer for the specified string. The 
        * tokenizer uses the default delimiter set, which is 
        * <code>"&nbsp;&#92;t&#92;n&#92;r&#92;f"</code>: the space character, 
        * the tab character, the newline character, the carriage-return character,
        * and the form-feed character. Delimiter characters themselves will 
        * not be treated as tokens.
        *
        * @param   str   a string to be parsed.
        * @exception NullPointerException if str is <CODE>null</CODE> 
        */
        public StringTokenizer(String str)
            : this(str, " \t\n\r\f", false)
        {

        }


        /// <summary>
        ///
        /*
       Calculates the number of times that this tokenizer's 
       <code>nextToken</code> method can be called before it generates an 
       exception. The current position is not advanced.
       the number of tokens remaining in the string using the current
       delimiter set.
      */
        /// </summary>
        /// <returns>the number of tokens remaining in the string using the current
        ///         delimiter set</returns>

        public int CountTokens()
        {
            int count = 0;
            int currpos = currentPosition;
            while (currpos < maxPosition)
            {
                currpos = SkipDelimiters(currpos);
                if (currpos >= maxPosition)
                    break;
                currpos = ScanToken(currpos);
                count++;
            }
            return count;
        }

        /// <summary>
        ///Skips delimiters starting from the specified position. If retDelims
        /// is false, returns the index of the first non-delimiter character at or
        /// after startPos. If retDelims is true, startPos is returned.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int SkipDelimiters(int startPos)
        {
            if (delimiters == null)
                throw new NullReferenceException();

            int position = startPos;
            while (!retDelims && position < maxPosition)
            {
                if (!hasSurrogates)
                {
                    char c = str[position];
                    if ((c > maxDelimCodePoint) || (delimiters.IndexOf(c) < 0))
                        break;
                    position++;
                }
                else
                {
                    //int c = str.codePointAt(position);
                    //if ((c > maxDelimCodePoint) || !isDelimiter(c))
                    //{
                    //    break;
                    //}
                    //position += Character.charCount(c);
                }
            }
            return position;
        }

        /// <summary>
        ///Skips ahead from startPos and returns the index of the next delimiter
        /// character encountered, or maxPosition if no such delimiter is found.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int ScanToken(int startPos)
        {
            int position = startPos;
            while (position < maxPosition)
            {
                if (!hasSurrogates)
                {
                    char c = str[position];
                    if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                        break;
                    position++;
                }
                else
                {
                    //int c = str.codePointAt(position);
                    //if ((c <= maxDelimCodePoint) && isDelimiter(c))
                    //    break;
                    //position += Character.charCount(c);
                }
            }
            if (retDelims && (startPos == position))
            {
                if (!hasSurrogates)
                {
                    char c = str[position];
                    if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                        position++;
                }
                else
                {
                    //int c = str.codePointAt(position);
                    //if ((c <= maxDelimCodePoint) && isDelimiter(c))
                    //    position += Character.charCount(c);
                }
            }
            return position;
        }

        /// <summary>
        /// Is char is delimiter char
        /// </summary>
        /// <param name="codePoint"></param>
        /// <returns>true if match</returns>
        private bool IsDelimiter(int codePoint)
        {
            for (int i = 0; i < delimiterCodePoints.Length; i++)
            {
                if (delimiterCodePoints[i] == codePoint)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the next token from this string tokenizer.
        /// </summary>
        /// <returns>the next token from this string tokenizer.</returns>
        public String NextToken()
        {
            /* 
             * If next position already computed in hasMoreElements() and
             * delimiters have changed between the computation and this invocation,
             * then use the computed value.
             */
            currentPosition = (newPosition >= 0 && !delimsChanged) ?
             newPosition : SkipDelimiters(currentPosition);

            /* Reset these anyway */
            delimsChanged = false;
            newPosition = -1;

            if (currentPosition >= maxPosition)
                throw new Exception("No Such Element Found");// NoSuchElementException();
            int start = currentPosition;
            currentPosition = ScanToken(currentPosition);
            return str.Substring(start, (currentPosition - start));
        }

        /// <summary>
        ///
        /*
      * Returns the same value as the <code>hasMoreTokens</code>
      * method. It exists so that this class can implement the
      * <code>Enumeration</code> interface. 
      *
      */
        /// </summary>
        /// <returns>true if there are more tokens; </returns>
        public bool HasMoreElements()
        {
            return HasMoreTokens();
        }

        /// <summary>
        ///
        /* Tests if there are more tokens available from this tokenizer's string. 
      * If this method returns <tt>true</tt>, then a subsequent call to 
      * <tt>nextToken</tt> with no argument will successfully return a token.
      *
      */
        /// </summary>
        /// <returns>true if and only if there is at least one token 
        public bool HasMoreTokens()
        {
            /*
             * Temporarily store this position and use it in the following
             * nextToken() method only if the delimiters haven't been changed in
             * that nextToken() invocation.
             */
            newPosition = SkipDelimiters(currentPosition);
            return (newPosition < maxPosition);
        }
    }
}

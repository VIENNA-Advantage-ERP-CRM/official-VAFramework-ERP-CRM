using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;
using VAdvantage.Print;

namespace VAdvantage.Utility
{
    public class Properties : HashMap<object, object>
    {
        
        //A property list that contains default values for any keys not
        //found inn this property list.
        protected Properties defaults;

        /// <summary>
        /// Creates an empty property list with no default values.
        /// </summary>
        public Properties()
            : this(null)
        {
        }

        /// <summary>
        /// Creates an empty property list with the specified defaults.
        /// </summary>
        /// <param name="defaults">the defaults.</param>
        public Properties(Properties defaults)
        {
            this.defaults = defaults;
        }

        /// <summary>
        /// Calls the Dictionary method
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Object SetProperty(String key, String value)
        {
            return Put(key, value);
        }


        /// <summary>
        /// Maps the specified key to the specified
        /// value inn this Dictionary. Neither the key nor the
        /// value can be null.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Object Put(string key, string value)
        {
            //if (string.IsNullOrEmpty(value))
            //    throw new NullReferenceException();

            string old = "";
            if (ContainsKey(key))
            {
                old = base[key].ToString();
                base[key] = value;
                return old;
            }
            else
            {
                Add(key, value);
                return value;
            }
        }


        /// <summary>
        /// Searches for the property with the specified key inn this property list.
        /// If the key is not found inn this property list, the default property list,
        /// and its defaults, recursively, are then checked. The method returns
        /// NULL if the property is not found.
        /// </summary>
        /// <param name="key">Key to be searched</param>
        /// <returns>the value inn this property list with the specified key value.</returns>
        public String GetProperty(String key)
        {
            Object oval = null;
            if (ContainsKey(key))
                oval = base[key];
            String sval = (oval is String) ? (String)oval : null;
            return ((sval == null) && (defaults != null)) ? defaults.GetProperty(key) : sval;
        }

        /// <summary>
        /// Searches for the property with the specified key inn this property list.
        /// If the key is not found inn this property list, the default property list,
        /// and its defaults, recursively, are then checked. The method returns
        /// defaultValue if the property is not found.
        /// </summary>
        /// <param name="key">Key to be searched</param>
        /// <param name="defaultValue">a default value.</param>
        /// <returns>the value inn this property list with the specified key value.</returns>
        public String GetProperty(String key, String defaultValue)
        {
            String val = GetProperty(key);
            return (val == null) ? defaultValue : val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outt"></param>
        /// <param name="comments"></param>
        public void Store(StreamWriter outt, String comments)
        {
            //StreamWriter sw = new StreamWriter(outt, Encoding.UTF8);
            Store(outt, comments, true);
        }

        /// <summary>
        /// Writes this property list (key and element pairs) in this
        /// Properties table to the output stream in a format suitable
        /// for loading into a Properties table using the StreamWriter
        /// <para>
        /// The stream is written using the ISO 8859-1 character encoding.
        /// </para>
        /// </summary>
        /// <param name="bw">StreamWriter</param>
        /// <param name="comments">Comments to be written</param>
        /// <param name="escUnicode">Escape Unicode or not</param>
        private void Store(StreamWriter bw, String comments, bool escUnicode)
        {
            if (comments == "DateAppend")
            {
                if (comments != null)
                {
                    WriteComments(bw, comments);
                }
            }
            else
            {
                if (comments != null)
                {
                    WriteComments(bw, comments);
                }
                bw.Write("#" + DateTime.Now);
            }
            bw.WriteLine();

            lock (this)
            {
                foreach (KeyValuePair<object, object> e in this)
                {
                    string key = (string)e.Key;
                    string val = (string)e.Value;
                    key = SaveConvert(key, true, escUnicode);
                    //No need to escape embedded and trailing spaces for value, hence
                    //pass false to flag.
                    val = SaveConvert(val, false, escUnicode);
                    bw.Write(key + "=" + val);
                    bw.WriteLine();                    
                }
            }
            bw.Flush();
        }





        /// <summary>
        /// Write Comments
        /// </summary>
        /// <param name="bw">Stream Object</param>
        /// <param name="comments">Comments to be written</param>
        private static void WriteComments(StreamWriter bw, String comments)
        {
            bw.Write("#");
            int len = comments.Length;
            int current = 0;
            int last = 0;
            char[] uu = new char[6];
            uu[0] = '\\';
            uu[1] = 'u';
            while (current < len)
            {
                char c = comments[current];
                if (c > '\u00ff' || c == '\n' || c == '\r')
                {
                    if (last != current)
                        bw.Write(comments.Substring(last, (current - last)));
                    if (c > '\u00ff')
                    {
                        uu[2] = ToHex((c >> 12) & 0xf);
                        uu[3] = ToHex((c >> 8) & 0xf);
                        uu[4] = ToHex((c >> 4) & 0xf);
                        uu[5] = ToHex(c & 0xf);
                        bw.Write(uu.ToString());
                    }
                    else
                    {
                        bw.WriteLine();
                        if (c == '\r' &&
                current != len - 1 &&
                comments[current + 1] == '\n')
                        {
                            current++;
                        }
                        if (current == len - 1 ||
                            (comments[current + 1] != '#' &&
                comments[current + 1] != '!'))
                            bw.Write("#");
                    }
                    last = current + 1;
                }
                current++;
            }
            if (last != current)
                bw.Write(comments.Substring(last, (current - last)));
            bw.WriteLine();
        }


        /// <summary>
        /// Converts encoded &#92;uxxxx to unicode chars
        /// and changes special saved chars to their original forms
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="off"></param>
        /// <param name="len"></param>
        /// <param name="convtBuf"></param>
        /// <returns></returns>
        private String LoadConvert(char[] inn, int off, int len, char[] convtBuf)
        {
            if (convtBuf.Length < len)
            {
                int newLen = len * 2;
                if (newLen < 0)
                {
                    newLen = int.MaxValue;
                }
                convtBuf = new char[newLen];
            }
            char aChar;
            char[] outt = convtBuf;
            int outLen = 0;
            int end = off + len;

            while (off < end)
            {
                aChar = inn[off++];
                if (aChar == '\\')
                {
                    aChar = inn[off++];
                    if (aChar == 'u')
                    {
                        // Read the xxxx
                        int value = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            aChar = inn[off++];
                            switch (aChar)
                            {
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    value = (value << 4) + aChar - '0';
                                    break;
                                case 'a':
                                case 'b':
                                case 'c':
                                case 'd':
                                case 'e':
                                case 'f':
                                    value = (value << 4) + 10 + aChar - 'a';
                                    break;
                                case 'A':
                                case 'B':
                                case 'C':
                                case 'D':
                                case 'E':
                                case 'F':
                                    value = (value << 4) + 10 + aChar - 'A';
                                    break;
                                default:
                                    throw new Exception("Malformed \\uxxxx encoding.");
                            }
                        }
                        outt[outLen++] = (char)value;
                    }
                    else
                    {
                        if (aChar == 't') aChar = '\t';
                        else if (aChar == 'r') aChar = '\r';
                        else if (aChar == 'n') aChar = '\n';
                        else if (aChar == 'f') aChar = '\f';
                        outt[outLen++] = aChar;
                    }
                }
                else
                {
                    outt[outLen++] = (char)aChar;
                }
            }
            return new String(outt, 0, outLen);
        }

        /// <summary>
        /// Converts unicodes to encoded &#92;uxxxx and escapes
        /// special characters with a preceding slash
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="escapeSpace"></param>
        /// <param name="escapeUnicode"></param>
        /// <returns></returns>
        private String SaveConvert(String theString, bool escapeSpace, bool escapeUnicode)
        {
            int len = theString.Length;
            int bufLen = len * 2;
            if (bufLen < 0)
            {
                bufLen = int.MaxValue;
            }
            StringBuilder outBuffer = new StringBuilder(bufLen);

            for (int x = 0; x < len; x++)
            {
                char aChar = theString[x];
                // Handle common case first, selecting largest block that
                // avoids the specials below
                if ((aChar > 61) && (aChar < 127))
                {
                    if (aChar == '\\')
                    {
                        outBuffer.Append('\\'); outBuffer.Append('\\');
                        continue;
                    }
                    outBuffer.Append(aChar);
                    continue;
                }
                switch (aChar)
                {
                    case ' ':
                        if (x == 0 || escapeSpace)
                            outBuffer.Append('\\');
                        outBuffer.Append(' ');
                        break;
                    case '\t': outBuffer.Append('\\'); outBuffer.Append('t');
                        break;
                    case '\n': outBuffer.Append('\\'); outBuffer.Append('n');
                        break;
                    case '\r': outBuffer.Append('\\'); outBuffer.Append('r');
                        break;
                    case '\f': outBuffer.Append('\\'); outBuffer.Append('f');
                        break;
                    case '=': // Fall through
                    case ':': // Fall through
                    case '#': // Fall through
                    case '!':
                        outBuffer.Append('\\'); outBuffer.Append(aChar);
                        break;
                    default:
                        if (((aChar < 0x0020) || (aChar > 0x007e)) & escapeUnicode)
                        {
                            outBuffer.Append('\\');
                            outBuffer.Append('u');
                            outBuffer.Append(ToHex((aChar >> 12) & 0xF));
                            outBuffer.Append(ToHex((aChar >> 8) & 0xF));
                            outBuffer.Append(ToHex((aChar >> 4) & 0xF));
                            outBuffer.Append(ToHex(aChar & 0xF));
                        }
                        else
                            outBuffer.Append(aChar);
                        break;
                }
            }
            return outBuffer.ToString();
        }

        /// <summary>
        /// Convert a nibble to a hex character
        /// </summary>
        /// <param name="nibble">the nibble to convert.</param>
        /// <returns>Converted Char</returns>
        private static char ToHex(int nibble)
        {
            return hexDigit[(nibble & 0xF)];
        }

        // A table of hex digits 
        private static char[] hexDigit = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// Enumerates all key/value pairs in the specified dictionary 
        /// and omits the property if the key or value is not a string.
        /// </summary>
        /// <param name="h">Dictionary</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void EnumerateStringProperties(Dictionary<String, String> h)
        {
            if (defaults != null)
                defaults.EnumerateStringProperties(h);

            foreach (KeyValuePair<string, string> kv in h)
            {
                Object k = kv.Key;
                Object v = kv.Value;
                if (k is String && v is String)
                {
                    h[(string)k] = (string)v;
                }
            }
        }

        /// <summary>
        /// Reads a property list (key and element pairs) from the input
        /// character stream in a simple line-oriented format.
        /// </summary>
        /// <param name="inStream"></param>
        public void Load(FileStream inStream)
        {
            LineReader line = new LineReader(new StreamReader(inStream));
            Load(line);
        }

        private void Load(LineReader lr)
        {
            char[] convtBuf = new char[1024];
            int limit;
            int keyLen;
            int valueStart;
            char c;
            bool hasSep;
            bool precedingBackslash;

            while ((limit = lr.ReadLine()) >= 0)
            {
                c = '0';
                keyLen = 0;
                valueStart = limit;
                hasSep = false;

                //System.out.println("line=<" + new String(lineBuf, 0, limit) + ">");
                precedingBackslash = false;
                while (keyLen < limit)
                {
                    c = lr.lineBuf[keyLen];
                    //need check if escaped.
                    if ((c == '=' || c == ':') && !precedingBackslash)
                    {
                        valueStart = keyLen + 1;
                        hasSep = true;
                        break;
                    }
                    else if ((c == ' ' || c == '\t' || c == '\f') && !precedingBackslash)
                    {
                        valueStart = keyLen + 1;
                        break;
                    }
                    if (c == '\\')
                    {
                        precedingBackslash = !precedingBackslash;
                    }
                    else
                    {
                        precedingBackslash = false;
                    }
                    keyLen++;
                }
                while (valueStart < limit)
                {
                    c = lr.lineBuf[valueStart];
                    if (c != ' ' && c != '\t' && c != '\f')
                    {
                        if (!hasSep && (c == '=' || c == ':'))
                        {
                            hasSep = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                    valueStart++;
                }
                String key = LoadConvert(lr.lineBuf, 0, keyLen, convtBuf);
                String value = LoadConvert(lr.lineBuf, valueStart, limit - valueStart, convtBuf);
                Put(key, value);
            }
        }

        /// <summary>
        /// Read in a "logical line" from an InputStream/Reader, skip all commen
        /// and blank lines and filter out those leading whitespace characters
        /// (\u0020, \u0009 and \u000c) from the beginning of a "natural line".
        /// </summary>
        public class LineReader
        {
            /// <summary>
            /// Constructor of the InnerClass
            /// </summary>
            /// <param name="reader"></param>
            public LineReader(StreamReader reader)
            {
                this.reader = reader;
                inCharBuf = new char[8192];
            }

            public char[] inCharBuf;
            public char[] lineBuf = new char[1024];
            int inLimit = 0;
            int inOff = 0;

            StreamReader reader = null;

            public int ReadLine()
            {
                int len = 0;
                char c = '0';

                bool skipWhiteSpace = true;
                bool isCommentLine = false;
                bool isNewLine = true;
                bool appendedLineBegin = false;
                bool precedingBackslash = false;
                bool skipLF = false;

                while (true)
                {
                    if (inOff >= inLimit)
                    {
                        inLimit = reader.Read(inCharBuf, 0, inCharBuf.Length);

                        inOff = 0;
                        if (inLimit <= 0)
                        {
                            if (len == 0 || isCommentLine)
                            {
                                return -1;
                            }
                            return len;
                        }
                    }


                    c = inCharBuf[inOff++];
                    if (skipLF)
                    {
                        skipLF = false;
                        if (c == '\n')
                        {
                            continue;
                        }
                    }
                    if (skipWhiteSpace)
                    {
                        if (c == ' ' || c == '\t' || c == '\f')
                        {
                            continue;
                        }
                        if (!appendedLineBegin && (c == '\r' || c == '\n'))
                        {
                            continue;
                        }
                        skipWhiteSpace = false;
                        appendedLineBegin = false;
                    }
                    if (isNewLine)
                    {
                        isNewLine = false;
                        if (c == '#' || c == '!')
                        {
                            isCommentLine = true;
                            continue;
                        }
                    }

                    if (c != '\n' && c != '\r')
                    {
                        lineBuf[len++] = c;
                        if (len == lineBuf.Length)
                        {
                            int newLength = lineBuf.Length * 2;
                            if (newLength < 0)
                            {
                                newLength = int.MaxValue;
                            }
                            char[] buf = new char[newLength];
                            Array.Copy(lineBuf, 0, buf, 0, lineBuf.Length);
                            lineBuf = buf;
                        }
                        //flip the preceding backslash flag
                        if (c == '\\')
                        {
                            precedingBackslash = !precedingBackslash;
                        }
                        else
                        {
                            precedingBackslash = false;
                        }
                    }
                    else
                    {
                        // reached EOL
                        if (isCommentLine || len == 0)
                        {
                            isCommentLine = false;
                            isNewLine = true;
                            skipWhiteSpace = true;
                            len = 0;
                            continue;
                        }
                        if (inOff >= inLimit)
                        {
                            inLimit = reader.Read(inCharBuf, 0, inCharBuf.Length);
                            inOff = 0;
                            if (inLimit <= 0)
                            {
                                return len;
                            }
                        }
                        if (precedingBackslash)
                        {
                            len -= 1;
                            //skip the leading whitespace characters in following line
                            skipWhiteSpace = true;
                            appendedLineBegin = true;
                            precedingBackslash = false;
                            if (c == '\r')
                            {
                                skipLF = true;
                            }
                        }
                        else
                        {
                            return len;
                        }
                    }
                }
            }

        }

    }
}

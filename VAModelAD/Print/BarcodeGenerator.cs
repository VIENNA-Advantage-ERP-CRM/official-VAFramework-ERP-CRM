using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VAdvantage.Print
{
    public class BarcodeGenerator
    {
        public BarcodeGenerator(MPrintFormatItem item)
        {
            width = item.GetMaxWidth();
            height = item.GetMaxHeight();
            humanReadable = true;
            fontSize = 12;
            fontName = "Courier New";
            centered = false;
        }


        private int height;
        private bool humanReadable;
        private int width;
        private string fontName;
        private int fontSize;
        private bool centered;

        private static string[] left_UPCA = new string[] {"0001101", "0011001", "0010011", "0111101", "0100011", 
                                        "0110001", "0101111", "0111011" , "0110111", "0001011"};
        private static string[] right_UPCA = new string[] {"1110010", "1100110", "1101100", "1000010", "1011100",
                                        "1001110", "1010000", "1000100", "1001000", "1110100"}; //1s compliment of left odd

        private static string[] both_2of5 = new string[] { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN",
                                        "NWWNN", "NNNWW", "WNNWN", "NWNWN" };

        private static char[] Code128ComboAB = new char[] { 
            ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*',
            '+', ',', '-', '.', '/', '0',  '1', '2', '3', '4', '5',
            '6', '7', '8', '9', ':', ';',  '<', '=', '>', '?', '@',
            'A', 'B', 'C', 'D', 'E', 'F',  'G', 'H', 'I', 'J', 'K',
            'L', 'M', 'N', 'O', 'P', 'Q',  'R', 'S', 'T', 'U', 'V',
            'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_'
        };

        private static char[] Code128B = new char[] { 
            '`', 'a', 'b',  'c', 'd', 'e', 'f',  'g', 'h', 'i', 'j',
            'k', 'l', 'm',  'n', 'o', 'p', 'q',  'r', 's', 't', 'u',
            'v', 'w', 'x',  'y', 'z', '{', '|',  '}', '~'
        };

        private static string[] Code128Encoding = new string[] {
            "11011001100", "11001101100", "11001100110", "10010011000", "10010001100", "10001001100", "10011001000", 
            "10011000100", "10001100100", "11001001000", "11001000100", "11000100100", "10110011100", "10011011100",
            "10011001110", "10111001100", "10011101100", "10011100110", "11001110010", "11001011100", "11001001110",
            "11011100100", "11001110100", "11101101110", "11101001100", "11100101100", "11100100110", "11101100100",
            "11100110100", "11100110010", "11011011000", "11011000110", "11000110110", "10100011000", "10001011000",
            "10001000110", "10110001000", "10001101000", "10001100010", "11010001000", "11000101000", "11000100010",
            "10110111000", "10110001110", "10001101110", "10111011000", "10111000110", "10001110110", "11101110110",
            "11010001110", "11000101110", "11011101000", "11011100010", "11011101110", "11101011000", "11101000110",
            "11100010110", "11101101000", "11101100010", "11100011010", "11101111010", "11001000010", "11110001010",
            "10100110000", "10100001100", "10010110000", "10010000110", "10000101100", "10000100110", "10110010000",
            "10110000100", "10011010000", "10011000010", "10000110100", "10000110010", "11000010010", "11001010000",
            "11110111010", "11000010100", "10001111010", "10100111100", "10010111100", "10010011110", "10111100100",
            "10011110100", "10011110010", "11110100100", "11110010100", "11110010010", "11011011110", "11011110110",
            "11110110110", "10101111000", "10100011110", "10001011110", "10111101000", "10111100010", "11110101000",
            "11110100010", "10111011110", "10111101110", "11101011110", "11110101110", "11010000100", "11010010000",
            "11010011100"
        };

        object[][] chars = new object[][] 
       {
          new object[] {'0', "n n n w w n w n n"},
          new object[] {'1', "w n n w n n n n w"},
          new object[] {'2', "n n w w n n n n w"},
          new object[] {'3', "w n w w n n n n n"},
          new object[] {'4', "n n n w w n n n w"},
          new object[] {'5', "w n n w w n n n n"},
          new object[] {'6', "n n w w w n n n n"},
          new object[] {'7', "n n n w n n w n w"},
          new object[] {'8', "w n n w n n w n n"},
          new object[] {'9', "n n w w n n w n n"},
          new object[] {'A', "w n n n n w n n w"},
          new object[] {'B', "n n w n n w n n w"},
          new object[] {'C', "w n w n n w n n n"},
          new object[] {'D', "n n n n w w n n w"},
          new object[] {'E', "w n n n w w n n n"},
          new object[] {'F', "n n w n w w n n n"},
          new object[] {'G', "n n n n n w w n w"},
          new object[] {'H', "w n n n n w w n n"},
          new object[] {'I', "n n w n n w w n n"},
          new object[] {'J', "n n n n w w w n n"},
          new object[] {'K', "w n n n n n n w w"},
          new object[] {'L', "n n w n n n n w w"},
          new object[] {'M', "w n w n n n n w n"},
          new object[] {'N', "n n n n w n n w w"},
          new object[] {'O', "w n n n w n n w n"},
          new object[] {'P', "n n w n w n n w n"},
          new object[] {'Q', "n n n n n n w w w"},
          new object[] {'R', "w n n n n n w w n"},
          new object[] {'S', "n n w n n n w w n"},
          new object[] {'T', "n n n n w n w w n"},
          new object[] {'U', "w w n n n n n n w"},
          new object[] {'V', "n w w n n n n n w"},
          new object[] {'W', "w w w n n n n n n"},
          new object[] {'X', "n w n n w n n n w"},
          new object[] {'Y', "w w n n w n n n n"},
          new object[] {'Z', "n w w n w n n n n"},
          new object[] {'-', "n w n n n n w n w"},
          new object[] {'.', "w w n n n n w n n"},
          new object[] {' ', "n w w n n n w n n"},
          new object[] {'*', "n w n n w n w n n"},
          new object[] {'$', "n w n w n w n n n"},
          new object[] {'/', "n w n w n n n w n"},
          new object[] {'+', "n w n n n w n w n"},
          new object[] {'%', "n n n w n w n w n"}
       };

        private static string Code128Stop = "11000111010";
        private enum Code128ChangeModes { CodeA = 101, CodeB = 100, CodeC = 99 };
        private enum Code128StartModes { CodeUnset = 0, CodeA = 103, CodeB = 104, CodeC = 105 };
        private enum Code128Modes { CodeUnset = 0, CodeA = 1, CodeB = 2, CodeC = 3 };

        #region Public Properties
        public bool Centered
        {
            get { return centered; }
            set { centered = value; }
        }

        public string FontName
        {
            get { return fontName; }
            set { fontName = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public bool HumanReadable
        {
            get { return humanReadable; }
            set { humanReadable = value; }
        }
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion

        public string DrawUPCA(Graphics g, string code, int x, int y)
        {
            code = code.Trim();
            try
            {
                Int64.Parse(code); // this ensures that the string is a number
            }
            catch
            {
                return "Code is not valid for a UPCA barcode: " + code;
            }
            while (code.Length < 11) // 11 is the length for upc-a barcodes;
                code = "0" + code;
            code = code.Substring(0, 11);
            code = code.Trim() + CheckDigitUPCA(code);

            string barbit = "101"; //START
            for (int i = 0; i < 6; i++) // first 6 Digits
            {
                int digit = Int32.Parse(code.Substring(i, 1));
                barbit += left_UPCA[digit];
            }
            barbit += "01010"; //CENTER
            for (int i = 6; i < 12; i++) // last 5 Digits + Check Digit
            {
                int digit = Int32.Parse(code.Substring(i, 1));
                barbit += right_UPCA[digit];
            }
            barbit += "101"; //END

            Font font = new Font(fontName, fontSize, FontStyle.Bold);
            SizeF size = g.MeasureString(code.Substring(0, 1), font);

            int padding = 0;
            if (humanReadable)
                padding = (int)size.Width + 2;

            int barwidth = (int)Math.Floor((double)(width - 2 * padding) / barbit.Length);
            if (barwidth <= 0)
                barwidth = 1;
            if (centered)
            {
                x = (int)x - (((barwidth * 95) + 2 * padding) / 2);
            }
            int start = x + padding;
            for (int i = 1; i <= barbit.Length; i++)
            {
                string bit = barbit.Substring(i - 1, 1);
                if (bit == "0")
                {
                    g.FillRectangle(Brushes.White, start, y, barwidth, height);
                }
                else // bit == "1"
                {
                    g.FillRectangle(Brushes.Black, start, y, barwidth, height);
                }
                start += barwidth;
            }
            g.FillRectangle(Brushes.White, x, y + height - 8, width, 8);
            if (humanReadable)
            {
                g.FillRectangle(Brushes.White, x + padding + ((barwidth) * 10), y + height - 20, barwidth * 36, 20);
                g.FillRectangle(Brushes.White, x + padding + ((barwidth) * 49), y + height - 20, barwidth * 36, 20);

                g.DrawString(code.Substring(0, 1), font, Brushes.Black, x + 2, y + height - font.Height);
                int spacing = (int)((barwidth * 36) / 5);

                for (int i = 1; i < 6; i++)
                {
                    g.DrawString(code.Substring(i, 1), font, Brushes.Black, x + (barwidth * 10) + padding + (spacing * (i - 1)), y + height - font.Height);
                }
                for (int i = 6; i < 11; i++)
                {
                    g.DrawString(code.Substring(i, 1), font, Brushes.Black, x + (barwidth * 49) + padding + (spacing * (i - 6)), y + height - font.Height);
                }
                g.DrawString(code.Substring(11, 1), font, Brushes.Black, x + ((barwidth) * 95) + padding, y + height - font.Height);
            }
            return "";
        }

        private string CheckDigitUPCA(string code)
        {
            int odd = 0;
            int even = 0;

            for (int i = 0; i < code.Length; i += 2)
                odd += Int32.Parse(code.Substring(i, 1));

            for (int i = 1; i < code.Length; i += 2)
                even += Int32.Parse(code.Substring(i, 1));

            int check = (10 - ((odd * 3) + even) % 10) % 10;
            return check.ToString().Trim();
        }

        public string DrawInterleaved2of5(Graphics g, string code, int x, int y)
        {
            return DrawInterleaved2of5(g, code, x, y, false);
        }

        public string DrawInterleaved2of5(Graphics g, string code, int x, int y, bool checksum)
        {
            code = code.Trim();
            try
            {
                Int64.Parse(code); // this ensures that the string is a number
            }
            catch
            {
                return "Code is not valid for an Interleaved 2 of 5 barcode: " + code;
            }

            if ((checksum && IsEven(code.Length)) || (!checksum && IsOdd(code.Length))) // check to make sure that the number of digits is even
                code = "0" + code;

            if (checksum)
                code = code + CheckDigitInterleaved(code);

            string barbit = "1010"; //START

            for (int i = 0; i < code.Length; i++)
            {
                int digitb = Int32.Parse(code.Substring(i, 1));
                string black = both_2of5[digitb];
                i++;
                int digitw = Int32.Parse(code.Substring(i, 1));
                string white = both_2of5[digitw];
                for (int j = 0; j < 5; j++) // 5 is the width of all interleaved symbologies
                {
                    if (black[j] == 'W')
                        barbit += "11";
                    else
                        barbit += "1";
                    if (white[j] == 'W')
                        barbit += "00";
                    else
                        barbit += "0";
                }
            }
            barbit += "1101"; //END

            Font font = new Font(fontName, fontSize, FontStyle.Bold);
            SizeF size = g.MeasureString(code.Substring(0, 1), font);

            int padding = 0;
            if (humanReadable)
                padding = (int)size.Width + 2;

            int barwidth = (int)Math.Floor((double)(width - 2 * padding) / barbit.Length);
            if (barwidth <= 0)
                barwidth = 1;
            if (centered)
            {
                x = (int)x - (((barwidth * barbit.Length) + 2 * padding) / 2);
            }
            int start = x + padding;
            for (int i = 1; i <= barbit.Length; i++)
            {
                string bit = barbit.Substring(i - 1, 1);
                if (bit == "0")
                {
                    g.FillRectangle(Brushes.White, start, y, barwidth, height);
                }
                else // bit == "1"
                {
                    g.FillRectangle(Brushes.Black, start, y, barwidth, height);
                }
                start += barwidth;
            }

            if (humanReadable)
            {
                int spacing = (int)((barwidth * 36) / 5);
                for (int i = 0; i < code.Length; i++)
                {
                    g.DrawString(code.Substring(i, 1), font, Brushes.Black, x + (barwidth * 10) + padding + (spacing * (i - 1)), y + height + 4);
                }
            }
            return "";
        }

        private string CheckDigitInterleaved(string code)
        {
            int odd = 0;
            int even = 0;

            for (int i = 0; i < code.Length; i += 2)
                even += Int32.Parse(code.Substring(i, 1));

            for (int i = 1; i < code.Length; i += 2)
                odd += Int32.Parse(code.Substring(i, 1));

            int check = (10 - ((odd * 3) + even) % 10) % 10;
            return check.ToString().Trim();
        }

        public string DrawCode128(Graphics g, string code, int x, int y)
        {
            if (code.Length == 0)
                return "Invalid code for Code128 barcode";

            List<int> encoded = new List<int>();
            Code128Modes currentMode = Code128Modes.CodeUnset;

            for (int i = 0; i < code.Length; i++)
            {
                if (IsNumber(code[i]) && i + 1 < code.Length && IsNumber(code[i + 1]))
                {
                    if (currentMode != Code128Modes.CodeC)
                    {
                        if (currentMode == Code128Modes.CodeUnset)
                            encoded.Add((int)Code128StartModes.CodeC);
                        else
                            encoded.Add((int)Code128ChangeModes.CodeC);
                        currentMode = Code128Modes.CodeC;
                    }
                    encoded.Add(Int32.Parse(code.Substring(i, 2)));
                    i++;
                }
                else
                {
                    if (currentMode != Code128Modes.CodeB)
                    {
                        if (currentMode == Code128Modes.CodeUnset)
                            encoded.Add((int)Code128StartModes.CodeB);
                        else
                            encoded.Add((int)Code128ChangeModes.CodeB);
                        currentMode = Code128Modes.CodeB;
                    }
                    encoded.Add(EncodeCodeB(code[i]));
                }
            }
            encoded.Add(CheckDigitCode128(encoded));

            string barbit = "";
            for (int i = 0; i < encoded.Count; i++)
            {
                barbit += Code128Encoding[encoded[i]];
            }
            barbit += Code128Stop;
            barbit += "11"; // end code



            int barwidth = (int)Math.Floor((double)(width - 2) / (barbit.Length + 20)); // add 20 for padding
            if (barwidth <= 0)
                barwidth = 1;

            int padding = barwidth * 10;
            if (centered)
            {
                x = (int)x - (((barwidth * barbit.Length) + (padding * 2)) / 2);
            }

            int start = x + padding;
            for (int i = 1; i <= barbit.Length; i++)
            {
                string bit = barbit.Substring(i - 1, 1);
                if (bit == "0")
                {
                    g.FillRectangle(Brushes.White, start, y, barwidth, height);
                }
                else // bit == "1"
                {
                    g.FillRectangle(Brushes.Black, start, y, barwidth, height);
                }
                start += barwidth;
            }

            if (humanReadable)
            {
                Font font = new Font(fontName, fontSize, FontStyle.Bold);
                SizeF size = g.MeasureString(code, font);
                x = x + (int)((barwidth * barbit.Length) + (padding * 2)) / 2;
                x -= (int)size.Width / 2;
                g.DrawString(code, font, Brushes.Black, x, y + height + 4);

            }
            return "";
        }


        private int CheckDigitCode128(List<int> codes)
        {
            int check = codes[0];
            for (int i = 1; i < codes.Count; i++)
            {
                check = check + (codes[i] * i);
            }
            return check % 103;
        }


        private bool IsNumber(char value)
        {
            return '0' == value || '1' == value || '2' == value || '3' == value ||
                   '4' == value || '5' == value || '6' == value || '7' == value ||
                   '8' == value || '9' == value;
        }

        private bool IsEven(int value)
        {
            return ((value & 1) == 0);
        }

        private bool IsOdd(int value)
        {
            return ((value & 1) == 1);
        }

        private int EncodeCodeB(char value)
        {
            for (int i = 0; i < Code128ComboAB.Length; i++)
            {
                if (Code128ComboAB[i] == value)
                    return i;
            }
            for (int i = 0; i < Code128B.Length; i++)
            {
                if (Code128B[i] == value)
                    return i + Code128ComboAB.Length;
            }
            throw new Exception("Invalid Character");
        }
    }
}

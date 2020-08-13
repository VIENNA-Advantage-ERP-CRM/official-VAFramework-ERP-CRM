using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace VAdvantage.Utility
{
    public class NativeDigitConverter
    {
        public static String ConvertToNativeNumerals(object oIn, Ctx ctx, CultureInfo cultureInfo = null)
        {
            return ConvertToNativeNumerals(oIn, (ctx.GetContext("#PrintNativeDigits") == "Y"), cultureInfo);
        }

        public static String ConvertToNativeNumerals(object oIn, bool ShowNative, CultureInfo cultureInfo = null)
        {
            if (oIn == null)
            {
                return null;
            }
            string strIn = oIn.ToString();

            if (string.IsNullOrEmpty(strIn))
            {
                return strIn;
            }

            if (!ShowNative)
            {
                return strIn;
            }

            if (cultureInfo == null)
            {
                cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            }



            return ConvertToNativeNumerals(strIn, cultureInfo.NumberFormat.NativeDigits);
        }

        private static string ConvertToNativeNumerals(string input, string[] nativedigits)
        {
            if (nativedigits == null || nativedigits.Length < 10)
            {
                return input;
            }

            //UTF8Encoding utf8Encoder = new UTF8Encoding();

            //Decoder utf8Decoder = utf8Encoder.GetDecoder();

            StringBuilder convertedChars = new System.Text.StringBuilder();

            //char[] convertedChar = new char[1];

            //byte[] bytes = new byte[] { 217, 160 };

            char[] inputCharArray = input.ToCharArray();

            foreach (char c in inputCharArray)
            {
                if (char.IsDigit(c))
                {
                    //bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                    //utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                    double index = char.GetNumericValue(c);
                    convertedChars.Append(nativedigits.GetValue(Convert.ToInt32(index)).ToString());
                }
                else
                {
                    convertedChars.Append(c);
                }
            }
            return convertedChars.ToString();
        }
    }
}

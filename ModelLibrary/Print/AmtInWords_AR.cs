/********************************************************
 * Module Name    : Reports
 * Purpose        : Amount in Words for English
 * ClassName      : AmtInWords_EN                 
 * Chronological Development
 * Deepak Saini
  ******************************************************/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.SqlExec;
using VAdvantage.Login;
using System.Data.SqlClient;
using VAdvantage.Logging;


namespace VAdvantage.Print
{
    public class AmtInWords_AR : AmtInWords
    {
        /// <summary>
        /// AmtInWords_EN
        /// </summary>
        public AmtInWords_AR()
            : base()
        {
            //super ();
        }	//	AmtInWords_EN

        private string RetStrNumber(long number)
        {
            if (number >= 3000 && number <= 10000)
            {
                return "آلآف";
            }

            else if (number >= 10001 && number <= 999999)
            {
                return "ألــف";
            }
            
            else if (number >= 10000000 && number <= 999999999)
            {
                return "مليون";
            }
            else if (number >= 3000000 && number <= 10000000)
            {
                return "ملايين";
            }
            else if (number >= 10000000001 && number <= 999999999999)
            {
                return "مليار";
            }
            else if (number >= 3000000000 && number <= 10000000000)
            {
                return "مليارات";
            }

            string ret = "";

            switch (number)
            {
                case 1:
                    ret = "واحـد";
                    break;

                case 2:
                    ret = "أثنان";
                    break;

                case 3:
                    ret = "ثلاثـه";
                    break;
                case 4:
                    ret = "اربعـه";
                    break;
                case 5:
                    ret = "خمســه";
                    break;
                case 6:
                    ret = "ستــه";
                    break;
                case 7:
                    ret = "سبعــه";
                    break;
                case 8:
                    ret = "ثمانيه";
                    break;
                case 9:
                    ret = "تسعـه";
                    break;
                case 10:
                    ret = "عشره";
                    break;
                case 11:
                    ret = "احد عشر";
                    break;
                case 12:
                    ret = "اثنا عشر";
                    break;
                case 20:
                    ret = "عشرون";
                    break;
                case 100:
                    ret = "مائـه";
                    break;
                case 200:
                    ret = "مائتين";
                    break;
                case 1000:
                    ret = "ألــف";
                    break;
                case 2000:
                    ret = "ألفـين";
                    break;
                case 1000000:
                    ret = "مليون";
                    break;
                case 2000000:
                    ret = "مليونين";
                    break;
                case 1000000000:
                    ret = "مليار";
                    break;
                case 2000000000:
                    ret = "مليارين";
                    break;
                default:
                    ret = "";
                    break;
            }
            return ret;
        }


        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="number">Number</param>
        /// <returns>amt</returns>
        private String Convert(long number)
        {
            /* special case */
            if (number == 0)
            {
                return "Zero";
            }
            //String prefix = "";
            if (number < 0)
            {
                number = -number;
               // prefix = "Negative ";
            }
           // String soFar = "";
           // int place = 0;

            return ConvertNumber(number.ToString());
            //do
            //{
            //    long n = number % 1000;
            //    if (n != 0)
            //    {
            //        String s = ConvertLessThanOneThousand((int)n);
            //        soFar = s + majorNames[place] + soFar;
            //    }
            //    place++;
            //    number /= 1000;
            //}
            //while (number > 0);
            //return (prefix + soFar).Trim();
        }	//	convert


        private string ConvertNumber(string number)
        {
            string num = number, result = "", str1, Rght_Dgt;
            int Lenght = 0;
            long Base;

            while (num.Length != 0)
            {
                Base = 0;
                str1 = "";
                Rght_Dgt = NextNumber(num, Lenght);

                Lenght = Lenght + Rght_Dgt.Length;
                num = num.Substring(0, num.Length - Rght_Dgt.Length);

                if (long.Parse(Rght_Dgt) >= 1 && long.Parse(Rght_Dgt) <= 99)
                {

                    if (Lenght <= 3)
                    {
                        if (Left(Rght_Dgt, 1) == "0")
                        {
                            Rght_Dgt = Right(Rght_Dgt, 2);
                        }

                        str1 = RetTenth(Rght_Dgt);
                    }
                    else
                    {
                        Base = RetBaseNumber(Rght_Dgt, Lenght);

                        if (long.Parse(Rght_Dgt) == 1 || long.Parse(Rght_Dgt) == 2)
                        {
                            str1 = RetStrNumber(long.Parse(Rght_Dgt) * Base);
                        }
                        else
                        {

                            str1 = RetTenth(Rght_Dgt);
                            str1 = str1 + ' ';
                            if (long.Parse(Rght_Dgt) >= 3 && long.Parse(Rght_Dgt) <= 10)
                            {
                                if (long.Parse(Rght_Dgt) == 10)
                                {
                                    Base = Base / 10;
                                }
                                str1 = str1 + RetStrNumber(Base * long.Parse(Rght_Dgt));
                            }
                            else
                            {
                                str1 = str1 + RetStrNumber(Base * long.Parse(Rght_Dgt));
                            }
                        }
                    }
                }

                else if (long.Parse(Rght_Dgt) >= 100)
                {
                    if (Lenght == 3)
                    {
                        str1 = RetHundred(Rght_Dgt);
                    }
                    else
                    {

                        Base = RetBaseNumber(Rght_Dgt, Lenght);
                        str1 = RetHundred(Rght_Dgt);
                        str1 = str1 + ' ';
                        str1 = str1 + RetStrNumber(long.Parse(Rght_Dgt[0].ToString()) * @Base);
                    }
                }
                if (long.Parse(Rght_Dgt) != 0)
                {
                    result = str1 + result;
                    if (num.Length != 0 && str1.Length != 0)
                        result = " و" + result;
                }
            }
            return result;
        }

        private string RetHundred(string TxtNumber)
        {
            string Result ;
            long Number;
            Number = long.Parse(TxtNumber);
            Result = RetStrNumber(Number);

            if (Result.Length == 0)
            {
                if (Left(TxtNumber, 1) == "1" || Left(TxtNumber, 1) == "2")
                {
                    Result = RetStrNumber(long.Parse(Left(TxtNumber, 1)) * 100);
                }
                else
                {
                    Result = RetStrNumber(long.Parse(Left(TxtNumber, 1)));
                    Result = Left(Result, Result.Length - 2) + "مائـه ";
                }
                if (Right(TxtNumber,2) != "00")
                {
                    Result = Result + " و";
                    Result = Result + RetTenth(Right(TxtNumber, 2));
                }
            }
            return Result;
        }

        private long RetBaseNumber(string TxtNumber, int Lenght)
        {
            long Result = 1;
            int BaseIndex = 1;
            int i;

            if (Left(TxtNumber, 1) == "0")
            {
                if (Left(TxtNumber, 2) == "00")
                {
                    BaseIndex = BaseIndex + 1;
                }
                BaseIndex = BaseIndex + 1;
            }
            i = 1;

            while (i <= (Lenght - BaseIndex))
            {
                Result = Result * 10;
                i = i + 1;
            }
            return Result;
        }

        private string RetTenth(string TxtNumber)
        {
            if (TxtNumber.Length == 3)
            {
                TxtNumber = TxtNumber.Substring(1);
            }
            string Result = RetStrNumber(long.Parse(TxtNumber));

            if (Result.Length == 0)
            {
                if (long.Parse(TxtNumber) > 12 && long.Parse(TxtNumber) < 20)
                {
                    Result = RetStrNumber(long.Parse(TxtNumber.Substring(TxtNumber.Length - 1)));
                    Result = Result.Substring(0, Result.Length - 1) + "ة ";
                    Result = @Result + "عشر";
                }
                else
                {
                    if (Right(TxtNumber, 1) != "0")
                    {
                        Result = RetStrNumber(long.Parse(Right(TxtNumber, 1)));
                        Result = Result + " و";
                    }
                    if (Left(TxtNumber, 1) == "2")
                    {
                        Result = Result + RetStrNumber(20);
                    }
                    else
                    {
                        Result = Result + RetStrNumber(long.Parse(Left(TxtNumber, 1)));
                        Result = Left(Result, Result.Length - 2) + "ين";
                    }
                }
            }
            return Result;
        }

        private string NextNumber(string num, int Lenght)
        {
            return Right(num, 3);

        }

        private string Left(string val, int lenght)
        {
            if (val.Length <= lenght)
            {
                return val;
            }
            return val.Substring(0, lenght);
        }

        private string Right(string val, int lenght)
        {
            if (val.Length <= lenght)
            {
                return val;
            }
            return val.Substring(val.Length - lenght);

        }

        /// <summary>
        /// Get Amount in Words
        /// </summary>
        /// <param name="amount">numeric amount (352.80)</param>
        /// <returns>Amount</returns>
        public String GetAmtInWords(string amount)
        {

            //long afterDecimal;
            if (amount == null)
                return amount;
            //
            StringBuilder sb = new StringBuilder();
            int pos = amount.LastIndexOf('.');
            int pos2 = amount.LastIndexOf(',');
            if (pos2 > pos)
                pos = pos2;
            String oldamt = amount;
            amount = amount.Replace(",", "");
            int newpos = amount.LastIndexOf('.');
            long amt = long.Parse(amount.Substring(0, newpos));



            string currencyName = "دينار";
            string coinsName = "ابن";

           string scale = amount.Substring(newpos + 1);
            
            sb.Append(ConvertNumber(amt.ToString()) + ' ' + currencyName);
            
            if (long.Parse(scale) > 0)
            {
                sb.Append(" و ").Append(ConvertNumber(scale)).Append(' ').Append(coinsName);
            }
            return sb.ToString();
        }
    }
}

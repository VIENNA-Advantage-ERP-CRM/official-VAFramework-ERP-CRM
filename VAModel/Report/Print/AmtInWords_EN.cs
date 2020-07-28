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
    public class AmtInWords_EN : AmtInWords
    {
        /// <summary>
        /// AmtInWords_EN
        /// </summary>
        public AmtInWords_EN()
            : base()
        {
            //super ();
        }	//	AmtInWords_EN

        /** Thousands plus				*/
        private static String[] majorNames = {
		" ", 
		" Thousand", 
		" Million",
		" Billion", 
		" Trillion", 
		" Quadrillion",
		" Quintillion"
	};

        /** Ten to Ninety				*/
        private static String[] tensNames = { 
		" ", 
		" Ten", 
		" Twenty",
		" Thirty", 
		" Fourty", 
		" Fifty", 
		" Sixty", 
		" Seventy",
		" Eighty", 
		" Ninety"
	};

        /** numbers to 19				*/
        private static String[] numNames = { 
		"", 
		" One", 
		" Two",
		" Three", 
		" Four", 
		" Five", 
		" Six", 
		" Seven", 
		" Eight", 
		" Nine",
		" Ten", 
		" Eleven", 
		" Twelve", 
		" Thirteen", 
		" Fourteen", 
		" Fifteen",
		" Sixteen", 
		" Seventeen", 
		" Eighteen", 
		" Nineteen"
	};


        private static string[] decimalMajorNames = { 
                                                    "Tenths",
                                                    "Hundredths",
                                                    "Thousandths",
                                                    "Ten Thousandths",
                                                    "hundred thousandths"
                                                    };

        /// <summary>
        /// Convert Less Than One Thousand
        /// </summary>
        /// <param name="number">number</param>
        /// <returns>amt</returns>
        private String ConvertLessThanOneThousand(int number)
        {
            String soFar;
            //	Below 20
            if (number % 100 < 20)
            {
                soFar = numNames[number % 100];
                number /= 100;
            }
            else
            {
                soFar = numNames[number % 10];
                number /= 10;
                soFar = tensNames[number % 10] + soFar;
                number /= 10;
            }
            if (number == 0)
                return soFar;
            return numNames[number] + " Hundred" + soFar;
        }	//	convertLessThanOneThousand

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
            String prefix = "";
            if (number < 0)
            {
                number = -number;
                prefix = "Negative ";
            }
            String soFar = "";
            int place = 0;
            do
            {
                long n = number % 1000;
                if (n != 0)
                {
                    String s = ConvertLessThanOneThousand((int)n);
                    soFar = s + majorNames[place] + soFar;
                }
                place++;
                number /= 1000;
            }
            while (number > 0);
            return (prefix + soFar).Trim();
        }	//	convert


        /// <summary>
        /// Get Amount in Words
        /// </summary>
        /// <param name="amount">numeric amount (352.80)</param>
        /// <returns>Amount</returns>
        public String GetAmtInWords(String amount)
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
            if (newpos > -1)
            {
                long dollars = long.Parse(amount.Substring(0, newpos));
                sb.Append(Convert(dollars));
            }
            else if (long.Parse(amount) > 0)
            {
                sb.Append(Convert(long.Parse(amount)));
            }
            for (int i = 0; i < oldamt.Length; i++)
            {
                if (pos == i) //	we are done
                {
                    String cents = oldamt.Substring(i + 1);
                    int rem = amount.Length - (newpos + 1);

                    string result = GetAmtInWords(cents);

                    if (!String.IsNullOrEmpty(result))
                    {

                        sb.Append(" And ").Append(result);
                        if (rem == 1)
                        {
                            sb.Append(' ').Append(decimalMajorNames[0]);
                        }
                        else if (rem == 2)
                        {
                            sb.Append(' ').Append(decimalMajorNames[1]);
                        }
                        else if (rem == 3)
                        {
                            sb.Append(' ').Append(decimalMajorNames[2]);
                        }
                        else if (rem == 4)
                        {
                            sb.Append(' ').Append(decimalMajorNames[3]);
                        }
                        else if (rem == 5)
                        {
                            sb.Append(' ').Append(decimalMajorNames[4]);
                        }
                        else if (rem == 6)
                        {
                            sb.Append(' ').Append(decimalMajorNames[5]);
                        }
                    }




                    //String cents = oldamt.Substring(i + 1);
                    //int rem = amount.Length - (newpos + 1);
                    //if (rem == 2)
                    //{
                    //    sb.Append(' ').Append(cents).Append("/100");
                    //}
                    //else if (rem == 3)
                    //{
                    //    sb.Append(' ').Append(cents).Append("/1000");
                    //}
                    break;
                }
            }
            return sb.ToString();
        }

        //    long afterDecimal;
        //    long dollars;
        //    if (amount == null)
        //        return amount;
        //    //
        //    StringBuilder sb = new StringBuilder();

        //    int po = amount.LastIndexOf('.');
        //    int pos = 0;
        //    if (po > 0)
        //    {
        //        pos = po;
        //    }
        //    else
        //    {
        //        pos = amount.Length;// amount.LastIndexOf('.');
        //    }
        //    // int pos2 = amount.LastIndexOf(',');
        //    //  if (pos2 > pos)
        //    //    pos = pos2;
        //    String oldamt = amount;
        //    //amount = amount.Replace(",", "");
        //    if (po > 0)
        //    {
        //        //int newpos = amount.LastIndexOf('.');
        //        dollars = long.Parse(amount.Substring(0, po));// long.Parse(amount.Substring(0, newpos));
        //    }
        //    else
        //    {
        //        dollars = long.Parse(amount);// long.Parse(amount.Substring(0, newpos));
        //    }

        //    sb.Append(" Rs ").Append(Convert(dollars));
        //    for (int i = 0; i <= oldamt.Length; i++)
        //    {
        //        if (pos == i) //	we are done
        //        {



        //            String cents = oldamt.Substring(i);
        //            if (cents == "")
        //            {
        //                cents = "00";
        //            }
        //            else
        //            {
        //                cents = oldamt.Substring(i + 1);
        //            }
        //            afterDecimal = long.Parse(cents);
        //            cents = Convert(afterDecimal);



        //            if (cents.Equals("Zero"))
        //            {
        //                sb.Append(" Only");
        //            }
        //            else
        //            {
        //                sb.Append(" & Paise ").Append(cents).Append(" Only");
        //            }
        //            break;
        //        }
        //    }
        //    return sb.ToString();
        //}	//	getAmtInWords

        /**
         * 	Test Print
         *	@param amt amount
         */
        //private void Print (String amt)
        //{
        //    try
        //    {
        //        //System.out.println(amt + " = " + getAmtInWords(amt));
        //    }
        //    catch (Exception e)
        //    {
        //        //e.printStackTrace();
        //    }
        //}	//	print

        ///**
        // * 	Test
        // *	@param args ignored
        // */
        //public static void main (String[] args)
        //{
        //    AmtInWords_EN aiw = new AmtInWords_EN();
        ////	aiw.print (".23");	Error
        //    aiw.print ("0.23");
        //    aiw.print ("1.23");
        //    aiw.print ("12.345");
        //    aiw.print ("123.45");
        //    aiw.print ("1234.56");
        //    aiw.print ("12345.78");
        //    aiw.print ("123457.89");
        //    aiw.print ("1,234,578.90");
        //}	//	main

    }



}

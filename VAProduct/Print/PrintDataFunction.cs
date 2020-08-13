/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     4-jul-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;


namespace VAdvantage.Print
{
    public class PrintDataFunction
    {

        /** The Sum				*/
        private Decimal m_sum = Env.ZERO;
        /** The Count			*/
        private int m_count = 0;
        /** Total Count			*/
        private int m_totalCount = 0;
        /** Minimum				*/
        private Decimal? m_min = null;
        /** Maximum				*/
        private Decimal? m_max = null;
        /** Sum of Squares		*/
        private Decimal m_sumSquare = Env.ZERO;

        /** Sum			*/
        static public char F_SUM = 'S';
        /** Mean		*/
        static public char F_MEAN = 'A';		//	Average mu
        /** Count		*/
        static public char F_COUNT = 'C';
        /** Min			*/
        static public char F_MIN = 'm';
        /** Max			*/
        static public char F_MAX = 'M';
        /** Variance	*/
        static public char F_VARIANCE = 'V';	//	sigma square
        /** Deviation	*/
        static public char F_DEVIATION = 'D';	//	sigma



        /** Function Keys							*/
        static private char[] FUNCTIONS = new char[] { F_SUM, F_MEAN, F_COUNT, F_MIN, F_MAX, F_VARIANCE, F_DEVIATION };
        /** Symbols									*/
        static private String[] FUNCTION_SYMBOLS = new String[] { " \u03A3", " \u03BC", " \u2116", " \u2193", " \u2191", " \u03C3\u00B2", " \u03C3" };
        /**	AD_Message Names of Functions			*/
        static private String[] FUNCTION_NAMES = new String[] { "Sum", "Mean", "Count", "Min", "Max", "Variance", "Deviation" };


        public void AddValue(Decimal? bd)
        {
            if (bd != null)
            {
                //	Sum

                m_sum = Decimal.Add((Decimal)m_sum, (Decimal)bd);
                //	Count
                m_count++;
                //	Min
                if (m_min == null)
                    m_min = bd;
                m_min = Math.Min((Decimal)m_min, (Decimal)bd);
                //	Max
                if (m_max == null)
                    m_max = bd;
                m_max = Math.Max((Decimal)m_max, (Decimal)bd);
                //	Sum of Squares
                m_sumSquare = Decimal.Add(m_sumSquare, (Decimal.Multiply((Decimal)bd, (Decimal)bd)));
            }
            m_totalCount++;
        }	//	addValue


        public Decimal GetValue(char function)
        {
            //	Sum
            if (function == F_SUM)
                return m_sum;
            //	Min/Max
            if (function == F_MIN)
                return (Decimal)m_min;
            if (function == F_MAX)
                return (Decimal)m_max;
            //	Count
            Decimal count = new Decimal(m_count);
            if (function == F_COUNT)
                return count;

            //	All other functions require count > 0
            if (m_count == 0)
                return Env.ZERO;

            //	Mean = sum/count - round to 4 digits
            if (function == F_MEAN)
            {
                Decimal mean = Decimal.Divide(m_sum, count);
                return mean;
            }
            //	Variance = sum of squares - (square of sum / count)
            Decimal ss = Decimal.Multiply(m_sum, m_sum);
            ss = Decimal.Divide(ss, count);
            Decimal variance = Decimal.Subtract(m_sumSquare, ss);
            if (function == F_VARIANCE)
            {
                return variance;
            }
            //	Standard Deviation
            Decimal deviation = new Decimal(Math.Sqrt(double.Parse(variance.ToString())));
            return deviation;
        }	//	getValue

        public void Reset()
        {
            m_count = 0;
            m_totalCount = 0;
            m_sum = Env.ZERO;
            m_sumSquare = Env.ZERO;
            m_min = null;
            m_max = null;
        }	//	reset

        /// <summary>
        /// Get Function Symbol of function
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        static public String GetFunctionSymbol(char function)
        {
            for (int i = 0; i < FUNCTIONS.Length; i++)
            {
                if (FUNCTIONS[i] == function)
                {
                    return FUNCTION_SYMBOLS[i];
                }
            }
            return "UnknownFunction=" + function;
        }	//	getFunctionSymbol

        /// <summary>
        /// Get Function Name of function   
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        static public String GetFunctionName(char function)
        {
            for (int i = 0; i < FUNCTIONS.Length; i++)
            {
                if (FUNCTIONS[i] == function)
                    return FUNCTION_NAMES[i];
            }
            return "UnknownFunction=" + function;
        }	//	getFunctionName


        /// <summary>
        /// Get Funcuion Name of function
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        static public int GetFunctionDisplayType(char function)
        {
            if (function == F_SUM || function == F_MIN || function == F_MAX)
                return DisplayType.Amount;
            if (function == F_COUNT)
                return DisplayType.Integer;
            //	Mean, Variance, Std. Deviation 
            return DisplayType.Number;
        }	//	getFunctionName

    }
}

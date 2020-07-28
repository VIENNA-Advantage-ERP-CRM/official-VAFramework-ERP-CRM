using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public interface AmtInWords
    {
        /// <summary>
        ///  Get Amount in Words
        /// </summary>
        /// <param name="amount">numeric amount (352.80)</param>
        /// <returns>amount in words (three*five*two 80/100)</returns>
        String GetAmtInWords(String amount);
    }	 
}

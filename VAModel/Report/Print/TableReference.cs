/********************************************************
 * Module Name    :     Report
 * Purpose        :     Contains Table refernece variable
 * Author         :     Jagmohan Bhatt
 * Date           :     1-july-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{

    public class TableReference
    {
        /** Table Name		*/
        public String TableName;
        /** Key Column		*/
        public String KeyColumn;
        /** Display Column	*/
        public String DisplayColumn;
        /** Displayed		*/
        public bool IsValueDisplayed = false;
        /** Translated		*/
        public bool IsTranslated = false;
    }	//	TableReference
}

//===================================================================================
//  File Name       :   commonFunctions.cs
//  Purpose         :   provides common functions to all the pages for common tasks.
//  Class Used      :   SqlHelper.cs
//==================================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Windows.Forms;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;

namespace VAdvantage.Classes
{
   /// <summary>
   /// Define common function for commonm task 
   /// </summary>
    public static class CommonFunction
    {
     
       
        /// <summary>
       ///contain Controls Type 
       /// </summary>
       public enum enmDataType
       {
           Button,
           CheckBox,
           ComboBox ,
           DateTimePicker,
           Label,
           TextBox,
       }
       
        /// <summary>
        /// Return Full qualified class name of Controls
        /// </summary>
        /// <param name="enm"></param>
        /// <returns></returns>
       public static string CheckControlType(enmDataType enm)
       {
           string strReturn = "";
           switch (enm)
           {
               case enmDataType.TextBox:
                   strReturn= "System.Windows.Forms.TextBox";
                   break;
               case enmDataType.ComboBox:
                   strReturn = "System.Windows.Forms.ComboBox";
                   break;
               case enmDataType.CheckBox:
                   strReturn = "System.Windows.Forms.CheckBox";
                   break;
               case enmDataType.DateTimePicker:
                   strReturn = "System.Windows.Forms.DateTimePicker";
                   break;
               case enmDataType.Button:
                   strReturn = "System.Windows.Forms.Button";
                   break;
               case enmDataType.Label:
                   strReturn = "System.Windows.Forms.Label";
                   break;
               default :
                   break;
           }
           return strReturn;
       }

       
        /// <summary>
       /// Fill Data in ComboBox control
        /// </summary>
        /// <param name="cmbObject"></param>
        /// <param name="strQry"></param>
        /// <param name="strText"></param>
        /// <param name="strValue"></param>
        /// <param name="strShowtext"></param>
        /// <param name="blnShowOther"></param>
      
      
        
    }
}

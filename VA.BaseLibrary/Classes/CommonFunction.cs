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
       public const char YES = '1';
       public const char NO = '0';
       
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
       /// Containing Validate Options
       /// </summary>
       public  enum enmValidation
       {
           Number = 0,
           String =1,
           AlphaNumeric= 2
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
        /// Validate text according to passed pattren
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="strVal"></param>
        /// <returns></returns>
       public static bool ValidateInput(string txt, string strVal)
       {
           Regex objRegx;
           if (txt == null)
               txt = "";
           switch (strVal)
           {
               case "AlphaNumeric": 
                   objRegx = new Regex("^[0-9a-zA-z]");
                   break;
               case "String":
                   objRegx = new Regex("^[a-zA-Z]");
                   break;
               default:
                   objRegx = new Regex("^[0-9]");
                   break;
           }
           return !objRegx.IsMatch(txt);
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
      
      
        /// <summary>
        /// Return database server's Datatype 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="strLength"></param>
        /// <returns></returns>
       public static string CheckDataType(string strVal, string strLength)
       {
           string strDataTyp = "";
           switch (strVal)
           {
               case "string":
                   strDataTyp = "nvarchar (" + ((strLength =="0")?"1":strLength).ToString() + ")";
                   break;
               case "button":
                   strDataTyp = "char (2)";
                   break;
               case "id":
                   strDataTyp = "int";
                   break;
               case "tabledirect":
                   strDataTyp = "int";
                   break;
               case "yes-no":
                   strDataTyp = "tinyint";
                   break;
               default:
                   strDataTyp = strVal;
                   break;
           }
           return strDataTyp;
       }
    }
}

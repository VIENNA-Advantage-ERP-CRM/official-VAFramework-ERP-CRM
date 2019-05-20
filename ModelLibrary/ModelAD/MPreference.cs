/********************************************************
 * Module Name    : Context Menu (Value Preference)
 * Purpose        : Contains constructors to set values in PO context and functions 
 *                  to delete preference from database of an attribute(column)
 * Class Used     : X_AD_Preference.cs
 * Chronological Development
 * Veena Pandey     21-Apr-2009
 ******************************************************/

using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    /// <summary>
    /// Contains constructors to set values in PO context and functions to 
    /// delete preference from database of an attribute(column)
    /// </summary>
    public class MPreference : X_AD_Preference
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Preference_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPreference(Context ctx, int AD_Preference_ID, Trx trxName)
            : base(ctx, AD_Preference_ID, trxName)
        {
            if (AD_Preference_ID == 0)
            {
                //	SetAttribute (null);
                //	SetValue (null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MPreference(Context ctx, System.Data.DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="attribute">attribute</param>
        /// <param name="value">value</param>
        /// <param name="trxName">transaction</param>
        public MPreference(Context ctx, String attribute, String value, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetAttribute(attribute);
            SetValue(value);
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Preference_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPreference(Ctx ctx, int AD_Preference_ID, Trx trxName)
            : base(ctx, AD_Preference_ID, trxName)
        {
            if (AD_Preference_ID == 0)
            {
                //	SetAttribute (null);
                //	SetValue (null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MPreference(Ctx ctx, System.Data.DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="attribute">attribute</param>
        /// <param name="value">value</param>
        /// <param name="trxName">transaction</param>
        public MPreference(Ctx ctx, String attribute, String value, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetAttribute(attribute);
            SetValue(value);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>bool type true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            String value = GetValue();
            //	NULL
            if (value == null)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Value"));
                return false;
            }
            //	Don't allow variables in Preferences
            else if (value.IndexOf('@') != -1)
            {
                log.SaveError("Error", "Invalid Value: @");
                return false;
            }
            //	if (value.equals("-1"))
            //		setValue("");
            return true;
        }

        /// <summary>
        /// Delete Preferences with Attribute & Value
        /// </summary>
        /// <param name="Attribute">attribute</param>
        /// <param name="Value">value</param>
        /// <returns>number of records deleted</returns>
        public static int Delete(String Attribute, String Value)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM AD_Preference WHERE Attribute='")
                .Append(Attribute).Append("' AND Value='").Append(Value).Append("'");
            return DataBase.DB.ExecuteQuery(sql.ToString(), null, null);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPreference[");
            sb.Append(Get_ID()).Append("-")
                .Append(GetAttribute()).Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }
    }
}
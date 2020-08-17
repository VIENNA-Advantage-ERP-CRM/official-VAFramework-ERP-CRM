/********************************************************
 * Module Name    : Search/HighVolume
 * Purpose        : Contains functions used to set query lines of user query of 
 *                  Advanced tab of "Find" window.
 * Class Used     : Inherited from class X_AD_UserQueryLine.cs
 * Chronological Development
 * Veena Pandey     10-Feb-2009
  ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Contains functions used to set query lines of user query of 
    /// Advanced tab of "Find" window in context.
    /// </summary>
    public class MUserQueryLine : X_AD_UserQueryLine
    {
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_UserQueryLine_ID">AD_UserQueryLine_ID</param>
        /// <param name="trxName">transaction name</param>
        public MUserQueryLine(Context ctx, int AD_UserQueryLine_ID, Trx trxName)
            : base(ctx, AD_UserQueryLine_ID, trxName)
        {
            if (AD_UserQueryLine_ID == 0)
            {
                SetIsAnd(true);	// Y
            }
        }

        /// <summary>
        /// Parameterized Contstructor
        /// Sets value in context
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="seqNo">seq no</param>
        /// <param name="keyName">column Name(Key name)</param>
        /// <param name="keyValue">column Value(Key Value)</param>
        /// <param name="optr">operator</param>
        /// <param name="value1Name">query value text</param>
        /// <param name="value1Value">query value's value</param>
        /// <param name="value2Name">"to query value" text</param>
        /// <param name="value2Value">"to query value's" value</param>
        public MUserQueryLine(MUserQuery parent, int seqNo, string keyName, string keyValue, string optr,
            string value1Name, string value1Value, string value2Name, string value2Value, bool fullDay = false)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetAD_UserQuery_ID(parent.GetAD_UserQuery_ID());
            SetSeqNo(seqNo);
            //
            SetKeyValue(keyValue);
            SetKeyName(keyName);
            SetOperator(optr);
            //
            SetValue1Value(value1Value);
            SetValue1Name(value1Name);
            if (value2Name != null && value2Name.Length != 0)
            {
                SetValue2Value(value2Value);
                SetValue2Name(value2Name);
            }
            SetIsFullDay(fullDay);
        }

        /// <summary>
        /// Load Constrctor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MUserQueryLine(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        //public MUserQueryLine(MUserQuery parent, int seqNo, ValueNamePair column, string optr, 
        //    ValueNamePair value, ValueNamePair value2)
        //    : base(parent.GetCtx(), 0, parent.Get_TrxName())
        //{
        //    setClientOrg(parent);
        //    SetAD_UserQuery_ID(parent.GetAD_UserQuery_ID());
        //    SetSeqNo(seqNo);
        //    //
        //    SetKeyValue(column.getValue());
        //    SetKeyName(column.getName());
        //    SetOperator(optr);
        //    //
        //    SetValue1Value(value.getValue());
        //    SetValue1Name(value.getName());
        //    if (value2 != null)
        //    {
        //        SetValue2Value(value2.getValue());
        //        SetValue2Name(value2.getName());
        //    }
        //}

        /// <summary>
        /// This function is called before final saving of records
        /// </summary>
        /// <returns>bool type true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetSeqNo() == 0)
            {
                int no = DataBase.DB.GetSQLValue(null,
                    "SELECT COALESCE(MAX(SeqNo),0)+10 FROM AD_UserQueryLine WHERE AD_UserQuery_ID=" + GetAD_UserQuery_ID());
                SetSeqNo(no);
            }
            return true;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MUserQueryLine[");
            sb.Append(Get_ID()).Append("-").Append(GetSeqNo())
                .Append("]");
            return sb.ToString();
        }

    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MElementValue
 * Purpose        : Natural Account
 * Class Used     : X_C_ElementValue
 * Chronological    Development
 * Raghunandan     11-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MElementValue : X_C_ElementValue

    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ElementValue_ID">ID or 0 for new</param>
        /// <param name="trxName">transaction</param>
        public MElementValue(Ctx ctx, int C_ElementValue_ID, Trx trxName)
            : base(ctx, C_ElementValue_ID, trxName)
        {
            if (C_ElementValue_ID == 0)
            {
                //	setC_Element_ID (0);	//	Parent
                //	setName (null);
                //	setValue (null);
                SetIsSummary(false);
                SetAccountSign(ACCOUNTSIGN_Natural);
                SetAccountType(ACCOUNTTYPE_Expense);
                SetIsDocControlled(false);
                SetIsForeignCurrency(false);
                SetIsBankAccount(false);
                //
                SetPostActual(true);
                SetPostBudget(true);
                SetPostEncumbrance(true);
                SetPostStatistical(true);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MElementValue(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">value</param>
        /// <param name="Name">name</param>
        /// <param name="Description">description</param>
        /// <param name="AccountType">account type</param>
        /// <param name="AccountSign">account sign</param>
        /// <param name="IsDocControlled">doc controlled</param>
        /// <param name="IsSummary">summary</param>
        /// <param name="trxName">transaction</param>
        public MElementValue(Ctx ctx, String Value, String Name, String Description,
            String AccountType, String AccountSign, bool IsDocControlled, bool IsSummary, Trx trxName)
            : this(ctx, 0, trxName)
        {

            SetValue(Value);
            SetName(Name);
            SetDescription(Description);
            SetAccountType(AccountType);
            SetAccountSign(AccountSign);
            SetIsDocControlled(IsDocControlled);
            SetIsSummary(IsSummary);
        }

        /// <summary>
        /// Import Constructor
        /// </summary>
        /// <param name="imp">imp import</param>
        public MElementValue(X_I_ElementValue imp)
            : this(imp.GetCtx(), 0, imp.Get_TrxName())
        {
            SetClientOrg(imp);
            Set(imp);
        }

        /// <summary>
        /// Set/Update Settings from import
        /// </summary>
        /// <param name="imp">import</param>
        public void Set(X_I_ElementValue imp)
        {
            SetValue(imp.GetValue());
            SetName(imp.GetName());
            SetDescription(imp.GetDescription());
            SetAccountType(imp.GetAccountType());
            SetAccountSign(imp.GetAccountSign());
            SetIsSummary(imp.IsSummary());
            SetIsDocControlled(imp.IsDocControlled());
            SetC_Element_ID(imp.GetC_Element_ID());
            SetPostActual(imp.IsPostActual());
            SetPostBudget(imp.IsPostBudget());
            SetPostEncumbrance(imp.IsPostEncumbrance());
            SetPostStatistical(imp.IsPostStatistical());
            //
            //	setC_BankAccount_ID(imp.getC_BankAccount_ID());
            //	setIsForeignCurrency(imp.isForeignCurrency());
            //	setC_Currency_ID(imp.getC_Currency_ID());
            //	setIsBankAccount(imp.isIsBankAccount());
            //	setValidFrom(null);
            //	setValidTo(null);
        }

        /// <summary>
        /// Is this a Balance Sheet Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsBalanceSheet()
        {
            String accountType = GetAccountType();
            return (ACCOUNTTYPE_Asset.Equals(accountType)
                || ACCOUNTTYPE_Liability.Equals(accountType)
                || ACCOUNTTYPE_OwnerSEquity.Equals(accountType));
        }

        /// <summary>
        /// Is this an Activa Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsActiva()
        {
            return ACCOUNTTYPE_Asset.Equals(GetAccountType());
        }

        /// <summary>
        /// Is this a Passiva Account
        /// </summary>
        /// <returns>bool</returns>
        public bool IsPassiva()
        {
            String accountType = GetAccountType();
            return (ACCOUNTTYPE_Liability.Equals(accountType)
                || ACCOUNTTYPE_OwnerSEquity.Equals(accountType));
        }	//	isPassiva

        /// <summary>
        /// User String Representation
        /// </summary>
        /// <returns>info value - name</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetValue()).Append(" - ").Append(GetName());
            return sb.ToString();
        }

        /// <summary>
        /// Extended String Representation
        /// </summary>
        /// <returns>info</returns>
        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder("MElementValue[");
            sb.Append(Get_ID()).Append(",").Append(GetValue()).Append(" - ").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true if ir can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
            //
            if (!newRecord && IsSummary()
                && Is_ValueChanged("IsSummary"))
            {
                String sql = "SELECT COUNT(*) FROM Fact_Acct WHERE Account_ID=" + GetC_ElementValue_ID();
                int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (no != 0)
                {
                    log.SaveError("Error", "Already posted to");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Value/Name change
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
            {
                MAccount.UpdateValueDescription(GetCtx(), "Account_ID=" + GetC_ElementValue_ID(), Get_TrxName());
                if ("Y".Equals(GetCtx().GetContext("$Element_U1")))
                    MAccount.UpdateValueDescription(GetCtx(), "User1_ID=" + GetC_ElementValue_ID(), Get_TrxName());
                if ("Y".Equals(GetCtx().GetContext("$Element_U2")))
                    MAccount.UpdateValueDescription(GetCtx(), "User2_ID=" + GetC_ElementValue_ID(), Get_TrxName());
            }
            return success;
        }
    }
}
/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABAcctElement
 * Purpose        : Natural Account
 * Class Used     : X_VAB_Acct_Element
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABAcctElement : X_VAB_Acct_Element

    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Acct_Element_ID">ID or 0 for new</param>
        /// <param name="trxName">transaction</param>
        public MVABAcctElement(Ctx ctx, int VAB_Acct_Element_ID, Trx trxName)
            : base(ctx, VAB_Acct_Element_ID, trxName)
        {
            if (VAB_Acct_Element_ID == 0)
            {
                //	setVAB_Element_ID (0);	//	Parent
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
        public MVABAcctElement(Ctx ctx, DataRow dr, Trx trxName)
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
        public MVABAcctElement(Ctx ctx, String Value, String Name, String Description,
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
        public MVABAcctElement(X_VAI_Acct imp)
            : this(imp.GetCtx(), 0, imp.Get_TrxName())
        {
            SetClientOrg(imp);
            Set(imp);
        }

        /// <summary>
        /// Set/Update Settings from import
        /// </summary>
        /// <param name="imp">import</param>
        public void Set(X_VAI_Acct imp)
        {
            SetValue(imp.GetValue());
            SetName(imp.GetName());
            SetDescription(imp.GetDescription());
            SetAccountType(imp.GetAccountType());
            SetAccountSign(imp.GetAccountSign());
            SetIsSummary(imp.IsSummary());
            SetIsDocControlled(imp.IsDocControlled());
            SetVAB_Element_ID(imp.GetVAB_Element_ID());
            SetPostActual(imp.IsPostActual());
            SetPostBudget(imp.IsPostBudget());
            SetPostEncumbrance(imp.IsPostEncumbrance());
            SetPostStatistical(imp.IsPostStatistical());
            //
            //	setVAB_Bank_Acct_ID(imp.getVAB_Bank_Acct_ID());
            //	setIsForeignCurrency(imp.isForeignCurrency());
            //	setVAB_Currency_ID(imp.getVAB_Currency_ID());
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
            StringBuilder sb = new StringBuilder("MVABAcctElement[");
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
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            //
            if (!newRecord && IsSummary()
                && Is_ValueChanged("IsSummary"))
            {
                String sql = "SELECT COUNT(*) FROM Actual_Acct_Detail WHERE Account_ID=" + GetVAB_Acct_Element_ID();
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
                MVABAccount.UpdateValueDescription(GetCtx(), "Account_ID=" + GetVAB_Acct_Element_ID(), Get_TrxName());
                if ("Y".Equals(GetCtx().GetContext("$Element_U1")))
                    MVABAccount.UpdateValueDescription(GetCtx(), "User1_ID=" + GetVAB_Acct_Element_ID(), Get_TrxName());
                if ("Y".Equals(GetCtx().GetContext("$Element_U2")))
                    MVABAccount.UpdateValueDescription(GetCtx(), "User2_ID=" + GetVAB_Acct_Element_ID(), Get_TrxName());
            }
            return success;
        }
    }
}
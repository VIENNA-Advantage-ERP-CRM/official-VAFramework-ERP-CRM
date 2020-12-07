/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_BankAccount
 * Chronological Development
 * Veena Pandey     24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;

namespace VAdvantage.Model
{
    public class MBankAccount : X_C_BankAccount
    {
        /**	Cache						*/
        private static CCache<int, MBankAccount> _cache
            = new CCache<int, MBankAccount>("C_BankAccount", 5);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BankAccount_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MBankAccount(Ctx ctx, int C_BankAccount_ID, Trx trxName)
            : base(ctx, C_BankAccount_ID, trxName)
        {
            if (C_BankAccount_ID == 0)
            {
                SetIsDefault(false);
                SetBankAccountType(BANKACCOUNTTYPE_Checking);
                SetCurrentBalance(Env.ZERO);
                SetUnMatchedBalance(Env.ZERO);
                //	SetC_Currency_ID (0);
                SetCreditLimit(Env.ZERO);
                //	SetC_BankAccount_ID (0);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MBankAccount(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * 	Get BankAccount from Cache
	     *	@param ctx context
	     *	@param C_BankAccount_ID id
	     *	@return MBankAccount
	     */
        public static MBankAccount Get(Ctx ctx, int C_BankAccount_ID)
        {
            int key = C_BankAccount_ID;
            MBankAccount retValue = (MBankAccount)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MBankAccount(ctx, C_BankAccount_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
	     * 	Get Bank
	     *	@return bank parent
	     */
        public MBank GetBank()
        {
            return MBank.Get(GetCtx(), GetC_Bank_ID());
        }

        /**
         * 	Get Bank Name and Account No
         *	@return Bank/Account
         */
        public String GetName()
        {
            return GetBank().GetName() + " " + GetAccountNo();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true if valid
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            MBank bank = GetBank();
            BankVerificationInterface verify = bank.GetVerificationClass();
            if (verify != null)
            {
                String errorMsg = verify.VerifyAccountNo(bank, GetAccountNo());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @AccountNo@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifyBBAN(bank, GetBBAN());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @BBAN@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifyIBAN(bank, GetIBAN());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @IBAN@ " + errorMsg);
                    return false;
                }
            }

            //Issue ID- SI_0613 in Google Sheet Standard Issues.. On Bank account currency should not allow to change if any transcation made against bank.
            if (!newRecord && Is_ValueChanged("C_Currency_ID"))
            {
                string sql = "SELECT SUM(total) AS TOTAL FROM (SELECT COUNT(*) AS TOTAL FROM C_PAYMENT WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL SELECT COUNT(*) AS TOTAL FROM C_ELEMENTVALUE WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM C_BP_BANKACCOUNT WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL  FROM C_BANKACCOUNT_ACCT  WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM C_BANKSTATEMENT WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL  FROM C_PAYMENTPROCESSOR  WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL  FROM C_CASHLINE WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM C_PAYSELECTION WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS TOTAL FROM C_BANKACCOUNTDOC WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID()
                               + " UNION ALL  SELECT COUNT(*) AS Total FROM C_BankAccountLine WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID();

                if (Env.IsModuleInstalled("VA026_")) // if Letter Of Credit Module is Installed..
                    sql += " UNION ALL SELECT count(*) as Total FROM VA026_LCDETAIL WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID();
                if (Env.IsModuleInstalled("VA027_")) // If Post dated Check Module is Installed..
                    sql += " UNION ALL SELECT count(*) as total FROM VA027_POSTDATEDCHECK WHERE AD_Client_ID= " + GetAD_Client_ID() + " AND C_BANKACCOUNT_ID =" + GetC_BankAccount_ID();

                sql += ")";

                if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CouldNotChnageCurrency"));
                    return false;
                }

                // JID_1583: system will update the Opening balance value in Current Balance field
                if (newRecord && Get_ColumnIndex("OpenBalance") > 0)
                {
                    SetCurrentBalance(Util.GetValueOfDecimal(Get_Value("OpenBalance")));
                }
            }
            //End

            // JID_1583: system will update the Opening balance value in Current Balance field
            if (newRecord && Get_ColumnIndex("OpenBalance") > 0)
            {
                SetCurrentBalance(Util.GetValueOfDecimal(Get_Value("OpenBalance")));
            }
            return true;
        }

        /**
        * 	After Save
        *	@param newRecord new record
        *	@param success success
        *	@return success
        */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (newRecord & success)
            {
                int _client_ID = 0;
                StringBuilder _sql = new StringBuilder();
                //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_BankAccount_Acct'");
                //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_BankAccount_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
                _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_BankAccount_Acct"));
                int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
                if (count > 0)
                {
                    string sql = " SELECT L.VALUE FROM AD_REF_LIST L" +
                                                     " INNER JOIN AD_Reference r ON R.AD_REFERENCE_ID=L.AD_REFERENCE_ID" +
                                                     " WHERE r.name='FRPT_RelatedTo' AND l.name='BankAccount'";

                    var relatedto = Convert.ToString(DB.ExecuteScalar(sql));

                    PO bnkact = null;
                    //X_FRPT_BankAccount_Acct bnkact = null;
                    //MAcctSchema acschma = new MAcctSchema(GetCtx(), 0, null);
                    _client_ID = GetAD_Client_ID();
                    _sql.Clear();
                    _sql.Append("select C_AcctSchema_ID from C_AcctSchema where AD_CLIENT_ID=" + _client_ID);
                    DataSet ds3 = new DataSet();
                    ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                    if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                    {
                        for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                        {
                            int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["C_AcctSchema_ID"]);
                            _sql.Clear();
                            _sql.Append("Select Frpt_Acctdefault_Id,C_Validcombination_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND AD_CLIENT_ID=" + _client_ID + "AND C_Acctschema_Id=" + _AcctSchema_ID);
                            DataSet ds = new DataSet();
                            ds = DB.ExecuteDataset(_sql.ToString(), null);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    //DataSet ds2 = new DataSet();
                                    string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                    if (_relatedTo != "")
                                    {

                                        // if (_relatedTo == X_FRPT_AcctSchema_Default.FRPT_RELATEDTO_BankAccount.ToString())
                                        if (_relatedTo == relatedto)
                                        {
                                            _sql.Clear();
                                            _sql.Append("Select COUNT(*) From C_BankAccount Ba Left Join FRPT_BankAccount_Acct ca On Ba.C_BANKACCOUNT_ID=ca.C_BANKACCOUNT_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Ba.IsActive='Y' AND Ba.AD_Client_ID=" + _client_ID + " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) + " AND Ba.C_BankAccount_ID = " + GetC_BankAccount_ID());
                                            int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                            //ds2 = DB.ExecuteDataset(_sql.ToString(), null);
                                            //if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                                            //{
                                            //    for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                                            //    {
                                            //        int value = Util.GetValueOfInt(ds2.Tables[0].Rows[j]["Frpt_Acctdefault_Id"]);
                                            //        if (value == 0)
                                            //        {
                                            //bnkact = new X_FRPT_BankAccount_Acct(GetCtx(), 0, null);
                                            if (recordFound == 0)
                                            {
                                                bnkact = MTable.GetPO(GetCtx(), "FRPT_BankAccount_Acct", 0, null);
                                                bnkact.Set_ValueNoCheck("C_BankAccount_ID", Util.GetValueOfInt(GetC_BankAccount_ID()));
                                                bnkact.Set_ValueNoCheck("AD_Org_ID", 0);
                                                bnkact.Set_Value("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                                bnkact.Set_Value("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                                bnkact.Set_Value("C_AcctSchema_ID", _AcctSchema_ID);
                                                if (!bnkact.Save())
                                                { }
                                            }
                                            //        }
                                            //    }
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y")
                {
                    bool sucs = Insert_Accounting("C_BankAccount_Acct", "C_AcctSchema_Default", null);

                    //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                    // Before this, data was being saved but giving message "record not saved".
                    if (!sucs)
                    {
                        log.SaveWarning("AcctNotSaved", "");
                    }

                }
            }
            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_BankAccount_Acct");
        }

        /**
	     * 	String representation
	     *	@return info
	     */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBankAccount[")
                .Append(Get_ID())
                .Append("-").Append(GetAccountNo())
                .Append("]");
            return sb.ToString();
        }

    }
}

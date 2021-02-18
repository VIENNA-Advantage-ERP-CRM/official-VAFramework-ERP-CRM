using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVABWithholding : X_VAB_Withholding
    {
        #region private variables
        //	Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABWithholding).FullName);
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Withholding_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABWithholding(Ctx ctx, int VAB_Withholding_ID, Trx trxName)
            : base(ctx, VAB_Withholding_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVABWithholding(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            // Applicable on Invoice = false, then clear values
            if (!IsApplicableonInv())
            {
                SetInvCalculation(null);
                SetInvPercentage(0);
                //JID_1857
                SetVAB_WithholdingCategory_ID(0);
            }

            // Applicable on Payment = false, then clear values
            if (!IsApplicableonPay())
            {
                SetPayCalculation(null);
                SetPayPercentage(0);
            }

            // validate unique record on the basis of this filteration of parameters
            string sql = @"SELECT COUNT(VAB_Withholding_ID) FROM VAB_Withholding WHERE TransactionType='" + GetTransactionType() + "'AND NVL(VAB_WithholdingCategory_ID , 0) = " +
                GetVAB_WithholdingCategory_ID() + " AND NVL(c_country_ID  ,0) = " + GetVAB_Country_ID() + " AND NVL(VAB_RegionState_ID , 0) = " + GetVAB_RegionState_ID();
            if (!newRecord)
            {
                sql += " AND VAB_Withholding_ID != " + GetVAB_Withholding_ID();
            }
            if (IsApplicableonPay())
            {
                sql += " AND IsApplicableonPay = 'Y' AND PayPercentage= " + GetPayPercentage();
            }
            if (IsApplicableonInv())
            {
                sql += " AND IsApplicableonInv = 'Y' AND invpercentage= " + GetInvPercentage();
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "WithholdingAlreadyExist"));
                return false;
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {

            #region create default Account
            if (Env.IsModuleInstalled("FRPT_"))
            {
                StringBuilder _sql = new StringBuilder();
                PO withholdingAcct = null;
                DataSet dsAcctSchema = null;
                DataSet dsDefaultAcct = null;

                // get value of "Withholding" -- "related to"
                _sql.Clear();
                _sql.Append(@"SELECT L.Value FROM VAF_CtrlRef_List L INNER JOIN VAF_Control_Ref r ON R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID 
                                WHERE r.name='FRPT_RelatedTo' AND l.name='Witholding'");
                var relatedtoProduct = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));

                // Get Accounting Schema
                _sql.Clear();
                _sql.Append("SELECT VAB_AccountBook_ID FROM VAB_AccountBook WHERE IsActive = 'Y' AND VAF_CLIENT_ID=" + GetVAF_Client_ID());
                dsAcctSchema = DB.ExecuteDataset(_sql.ToString(), null);
                if (dsAcctSchema != null && dsAcctSchema.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < dsAcctSchema.Tables[0].Rows.Count; k++)
                    {
                        // get accounting schema ID
                        int _AcctSchema_ID = Util.GetValueOfInt(dsAcctSchema.Tables[0].Rows[k]["VAB_AccountBook_ID"]);

                        // Get Accounting default and combination from "Default Accounting" tab of Accounting schema based on "Related To" (withholding)
                        _sql.Clear();
                        _sql.Append(@"SELECT Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id FROM Frpt_Acctschema_Default
                                        WHERE ISACTIVE='Y' AND VAF_CLIENT_ID=" + GetVAF_Client_ID() + "AND VAB_AccountBook_Id=" + _AcctSchema_ID +
                                        " AND Frpt_Relatedto = " + relatedtoProduct);
                        dsDefaultAcct = DB.ExecuteDataset(_sql.ToString(), null, Get_Trx());
                        if (dsDefaultAcct != null && dsDefaultAcct.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsDefaultAcct.Tables[0].Rows.Count; i++)
                            {
                                // check record exist or not, if not then create it
                                _sql.Clear();
                                _sql.Append(@"Select COUNT(Bp.VAB_Withholding_ID) From VAB_Withholding Bp
                                                       Left Join FRPT_Withholding_Acct  ca On Bp.VAB_Withholding_ID=ca.VAB_Withholding_ID 
                                                        And ca.Frpt_Acctdefault_Id=" + dsDefaultAcct.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]
                                               + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + GetVAF_Client_ID() +
                                               " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(dsDefaultAcct.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) +
                                               " AND Bp.VAB_Withholding_ID = " + GetVAB_Withholding_ID());
                                int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                if (recordFound == 0)
                                {
                                    withholdingAcct = MVAFTableView.GetPO(GetCtx(), "FRPT_Withholding_Acct", 0, null);
                                    withholdingAcct.Set_ValueNoCheck("VAF_Org_ID", 0);
                                    withholdingAcct.Set_ValueNoCheck("VAB_Withholding_ID", Util.GetValueOfInt(GetVAB_Withholding_ID()));
                                    withholdingAcct.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(dsDefaultAcct.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                    withholdingAcct.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(dsDefaultAcct.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                    withholdingAcct.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);
                                    if (!withholdingAcct.Save())
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        log.Log(Level.SEVERE, "Could Not create FRPT_Asset_Groip_Acct. ERRor Value : " + pp.GetValue() + "ERROR NAME : " + pp.GetName());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return true;
        }
    }
}

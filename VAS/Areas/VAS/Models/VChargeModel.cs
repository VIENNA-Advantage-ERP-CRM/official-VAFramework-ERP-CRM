using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Classes;

namespace VIS.Controllers
{
    public class VChargeModel
    {
        public int ID { get; set; }
        public string Msg { get; set; }
        public string listCreatedP { get; set; }
        public string listRejectedP { get; set; }

        /// <summary>
        /// Create element for the new account
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="m_VAF_Org_ID"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <param name="isExpenseType"></param>
        /// <param name="m_VAB_Element_ID"></param>
        /// <returns></returns>
        public int CreateElementValue(Ctx ctx, int m_VAF_Org_ID, String value, String name, Boolean isExpenseType, int m_VAB_Element_ID)
        {
            MElementValue ev = new MElementValue(ctx, value, name, null,
                isExpenseType ? X_VAB_Acct_Element.ACCOUNTTYPE_Expense : X_VAB_Acct_Element.ACCOUNTTYPE_Revenue,
                    X_VAB_Acct_Element.ACCOUNTSIGN_Natural,
                    false, false, null);
            ev.SetVAB_Element_ID(m_VAB_Element_ID);
            ev.SetVAF_Org_ID(m_VAF_Org_ID);
            if (!ev.Save())
            {
                //log.Log(Level.WARNING, "VAB_Acct_Element_ID not created");
                Msg = "VAB_Acct_Element_ID not created";
            }
            ID = ev.GetVAB_Acct_Element_ID();
            return ev.GetVAB_Acct_Element_ID();
        }

        /// <summary>
        /// Create new Charge based on the parameters passed
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="m_VAB_AccountBook_ID"></param>
        /// <param name="m_C_TaxCategory_ID"></param>
        /// <param name="name"></param>
        /// <param name="primaryVAB_Acct_Element_ID"></param>
        /// <param name="expense"></param>
        /// <returns></returns>
        public int CreateCharge(Ctx ctx, int m_VAB_AccountBook_ID, int m_C_TaxCategory_ID, String name, int primaryVAB_Acct_Element_ID, Boolean expense)
        {
            MCharge charge = new MCharge(ctx, 0, null);
            charge.SetName(name);
            charge.SetC_TaxCategory_ID(m_C_TaxCategory_ID);
            if (!charge.Save())
            {
                // log.Log(Level.SEVERE, name + " not created");
                Msg = name + " not created";
                ID = 0;
                return 0;
            }

            MAcctSchema m_acctSchema = null;
            //  Get Primary AcctSchama
            if (m_acctSchema == null)
            {
                m_acctSchema = new MAcctSchema(ctx, m_VAB_AccountBook_ID, null);
            }
            if (m_acctSchema == null || m_acctSchema.GetVAB_AccountBook_ID() == 0)
            {
                ID = 0;
                return 0;
            }
            MAcctSchemaElement primary_ase = m_acctSchema.GetAcctSchemaElement(X_VAB_AccountBook_Element.ELEMENTTYPE_Account);

            //	Get All
            MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(ctx, charge.GetVAF_Client_ID());
            foreach (MAcctSchema ac in ass)
            {
                //	Target Account
                MAccount defaultAcct = MAccount.GetDefault(ac, true);	//	optional null
                //	Natural Account
                int VAB_Acct_Element_ID = primaryVAB_Acct_Element_ID;
                MAcctSchemaElement ase = ac.GetAcctSchemaElement(X_VAB_AccountBook_Element.ELEMENTTYPE_Account);
                if (primary_ase.GetVAB_Element_ID() != ase.GetVAB_Element_ID())
                {
                    MAcctSchemaDefault defAccts = MAcctSchemaDefault.Get(ctx, ac.GetVAB_AccountBook_ID());
                    int C_ValidCombination_ID = defAccts.GetCh_Expense_Acct();
                    if (!expense)
                    {
                        C_ValidCombination_ID = defAccts.GetCh_Revenue_Acct();
                    }
                    MAccount chargeAcct = MAccount.Get(ctx, C_ValidCombination_ID);
                    VAB_Acct_Element_ID = chargeAcct.GetAccount_ID();
                    //	Fallback
                    if (VAB_Acct_Element_ID == 0)
                    {
                        VAB_Acct_Element_ID = defaultAcct.GetAccount_ID();
                        if (VAB_Acct_Element_ID == 0)
                        {
                            VAB_Acct_Element_ID = ase.GetVAB_Acct_Element_ID();
                        }
                        if (VAB_Acct_Element_ID == 0)
                        {
                            // log.Log(Level.WARNING, "No Default ElementValue for " + ac);
                            Msg = "No Default ElementValue for " + ac;
                            continue;
                        }
                    }
                }

                MAccount acct = MAccount.Get(ctx,
                    charge.GetVAF_Client_ID(), charge.GetVAF_Org_ID(),
                    ac.GetVAB_AccountBook_ID(),
                    VAB_Acct_Element_ID, defaultAcct.GetC_SubAcct_ID(),
                    defaultAcct.GetM_Product_ID(), defaultAcct.GetVAB_BusinessPartner_ID(), defaultAcct.GetVAF_OrgTrx_ID(),
                    defaultAcct.GetC_LocFrom_ID(), defaultAcct.GetC_LocTo_ID(), defaultAcct.GetC_SalesRegion_ID(),
                    defaultAcct.GetC_Project_ID(), defaultAcct.GetVAB_Promotion_ID(), defaultAcct.GetVAB_BillingCode_ID(),
                    defaultAcct.GetUser1_ID(), defaultAcct.GetUser2_ID(),
                    defaultAcct.GetUserElement1_ID(), defaultAcct.GetUserElement2_ID());
                if (acct == null)
                {
                    //log.Log(Level.WARNING, "No Default Account for " + ac);
                    Msg = "No Default Account for " + ac;
                    continue;
                }

                //  Update Accounts
                StringBuilder sql = new StringBuilder("UPDATE VAB_Charge_Acct ");
                sql.Append("SET CH_Expense_Acct=").Append(acct.GetC_ValidCombination_ID());
                sql.Append(", CH_Revenue_Acct=").Append(acct.GetC_ValidCombination_ID());
                sql.Append(" WHERE VAB_Charge_ID=").Append(charge.GetVAB_Charge_ID());
                sql.Append(" AND VAB_AccountBook_ID=").Append(ac.GetVAB_AccountBook_ID());
                //
                int no = VAdvantage.DataBase.DB.ExecuteQuery(sql.ToString(), null, null);
                if (no != 1)
                {
                    //log.Log(Level.WARNING, "Update #" + no + "\n" + sql.ToString());
                    Msg = "Update #" + no + "\n" + sql.ToString();
                }
            }
            ID = charge.GetVAB_Charge_ID();
            return charge.GetVAB_Charge_ID();
        }

        public void CreateChargeByList(Ctx ctx, int m_VAB_AccountBook_ID, int m_C_TaxCategory_ID, List<string> eleName, List<string> eleValue_ID, List<bool> expense)
        {
            StringBuilder listCreated = new StringBuilder();
            StringBuilder listRejected = new StringBuilder();

            for (int j = 0; j < eleValue_ID.Count; j++)
            {
                int VAB_Charge_ID = CreateCharge(ctx, m_VAB_AccountBook_ID, m_C_TaxCategory_ID, eleName[j], Convert.ToInt32(eleValue_ID[j]), Convert.ToBoolean(expense[j]));
                if (VAB_Charge_ID == 0)
                {
                    if (listRejected.Length > 0)
                    {
                        listRejected.Append(", ");
                    }
                    listRejected.Append(eleName[j]);
                }
                else
                {
                    if (listCreated.Length > 0)
                    {
                        listCreated.Append(", ");
                    }
                    listCreated.Append(eleName[j]);
                }
            }
            listCreatedP = listCreated.ToString();
            listRejectedP = listRejected.ToString();
        }

        /// <summary>
        /// get data from VAB_Acct_Element for load grid
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="mcAcctSchemaID"></param>
        /// <param name="mADClientId"></param>
        /// <returns></returns>
        public VChargeLodGrideData VChargeLodGrideData(Ctx ctx, int mcAcctSchemaID, int mADClientId)
        {
            VChargeLodGrideData obj = new VChargeLodGrideData();
            obj.meID = 1;

            // Get VAB_Element_id
            int MCElement_ID = GetMCElement_ID(mcAcctSchemaID, mADClientId);
            if (MCElement_ID == 0)
            {                
                obj.meID = MCElement_ID;
                return obj;
            }
            obj.VchargeMCElemTaxCatID = VchargeMCElemTaxCatID(MCElement_ID, mADClientId);
            obj.VchargeCElementValue = VchargeCElementValue(MCElement_ID);

            return obj;
        }

        /// <summary>
        /// Get C_TaxCategory_ID FROM C_TaxCategory
        /// </summary>
        /// <param name="MCElement_ID"></param>
        /// <param name="mADClientId"></param>
        /// <returns></returns>
        private List<VchargeMCElementTaxCategoryID> VchargeMCElemTaxCatID(int MCElement_ID, int mADClientId)
        {
            List<VchargeMCElementTaxCategoryID> obj = new List<VchargeMCElementTaxCategoryID>();
            string sql = "SELECT C_TaxCategory_ID FROM C_TaxCategory WHERE IsDefault='Y' AND VAF_Client_ID=" + mADClientId;
            int mCTaxCategoryID = 0;

            object mtaxcatid = DB.ExecuteScalar(sql);
            if (mtaxcatid != null && mtaxcatid != DBNull.Value)
            {
                mCTaxCategoryID = Convert.ToInt32(mtaxcatid);
            }

            // creating this object because need this IDs at client side.

            VchargeMCElementTaxCategoryID etc = new VchargeMCElementTaxCategoryID();
            etc.mCTaxCategoryID = mCTaxCategoryID;
            etc.mCElementID = MCElement_ID;

            obj.Add(etc);
            return obj;
        }


        /// <summary>
        /// get data from VAB_Acct_Element_id
        /// </summary>
        /// <param name="MCElement_ID"></param>
        /// <returns></returns>
        private List<VchargeCElementValue> VchargeCElementValue(int MCElement_ID)
        {
            List<VchargeCElementValue> obj = new List<VchargeCElementValue>();

            string sql = "SELECT 'false' as Select1,VAB_Acct_Element_ID,Value, Name, case when AccountType = 'E' THEN 'true' else 'false' end as Expense"
                  + " FROM VAB_Acct_Element "
                  + " WHERE AccountType IN ('R','E')"
                  + " AND IsSummary='N'"
                  + " AND VAB_Element_ID= " + MCElement_ID
                  + " ORDER BY 2 desc";
            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    obj.Add(new VchargeCElementValue()
                    {
                        VAB_Acct_Element_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_Element_ID"]),
                        Value = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]),
                        Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]),
                        Expense = Util.GetValueOfString(ds.Tables[0].Rows[i]["Expense"])
                    });
                }
            }
            return obj;
        }

        /// <summary>
        /// get VAB_Element_id
        /// </summary>
        /// <param name="mcAcctSchemaID"></param>
        /// <param name="mADClientId"></param>
        /// <returns></returns>
        private int GetMCElement_ID(int mcAcctSchemaID, int mADClientId)
        {
            int i = 0;
            var sql = "SELECT VAB_Element_ID FROM VAB_AccountBook_Element "
                          + "WHERE ElementType='AC' AND VAB_AccountBook_ID= " + mcAcctSchemaID;

            object defaultAcct = DB.ExecuteScalar(sql);
            if (defaultAcct != null && defaultAcct != DBNull.Value)
            {
                i = Convert.ToInt32(defaultAcct);
            }

            return i;
        }

    }
}
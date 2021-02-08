using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class AccountSchema
    {
        public bool IsHasAlies { get; set; }
        public List<AccountingElements> Elements { get; set; }
        public string Description { get; set; }
    }

    public class AccountingElements
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public bool IsMandatory { get; set; }
        public string Name { get; set; }
        public int DefaultValue { get; set; }
        public int SeqNo { get; set; }
        public int VAF_Column_ID { get; set; }
        public bool IsHeavyData { get; set; }
    }

    public class AccountingObjects
    {
        public string ErrorMsg { get; set; }
        public int VAB_Acct_ValidParameter_ID { get; set; }
        public int VAB_AccountBook_ID { get; set; }
    }

    public class AccountFormModel
    {
        /// <summary>
        /// Load controls lookup etc.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="windowNo"></param>
        /// <param name="VAB_AccountBook_ID"></param>
        /// <returns>class lookups</returns>
        public AccountSchema AccountSchemaLoad(Ctx ctx, int windowNo, int VAB_AccountBook_ID)
        {
            AccountSchema objSchema = new AccountSchema();
            MVABAccountBookElement[] elements = null;

            var _AcctSchema = new MVABAccountBook(ctx, VAB_AccountBook_ID, null);
            ctx.GetCtx(windowNo).SetContext(windowNo, "VAB_AccountBook_ID", VAB_AccountBook_ID);
            elements = _AcctSchema.GetAcctSchemaElements();

            objSchema.IsHasAlies = _AcctSchema.IsHasAlias();
            objSchema.Elements = new List<AccountingElements>();
            for (int i = 0; i < elements.Length; i++)
            {
                AccountingElements obj = new AccountingElements();
                MVABAccountBookElement ase = elements[i];
                obj.Type = ase.GetElementType();
                obj.IsMandatory = ase.IsMandatory();
                obj.ID = ase.Get_ID();
                obj.Name = ase.GetName();
                obj.DefaultValue = ase.GetDefaultValue();
                obj.SeqNo = ase.GetSeqNo();
                obj.VAF_Column_ID = ase.GetVAF_Column_ID();
                obj.IsHeavyData = Util.GetValueOfBool(ase.Get_Value("IsHeavyData"));
                objSchema.Elements.Add(obj);                
            }
            objSchema.Description = _AcctSchema.ToString();
            return objSchema;
        }

        /// <summary>
        /// null check function 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsNull(object value)
        {
            if (value == null || value.Equals(DBNull.Value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save account
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAF_Client_ID"></param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="VAB_AccountBook_ID"></param>
        /// <param name="AD_Account_ID"></param>
        /// <param name="VAB_SubAcct_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAB_BusinessPartner_ID"></param>
        /// <param name="VAF_OrgTrx_ID"></param>
        /// <param name="C_LocFrom_ID"></param>
        /// <param name="C_LocTo_ID"></param>
        /// <param name="C_SRegion_ID"></param>
        /// <param name="VAB_Project_ID"></param>
        /// <param name="VAB_Promotion_ID"></param>
        /// <param name="VAB_BillingCode_ID"></param>
        /// <param name="User1_ID"></param>
        /// <param name="User2_ID"></param>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public AccountingObjects SaveAccount(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, int VAB_AccountBook_ID, int AD_Account_ID, int VAB_SubAcct_ID, int VAM_Product_ID,
            int VAB_BusinessPartner_ID, int VAF_OrgTrx_ID, int C_LocFrom_ID, int C_LocTo_ID, int C_SRegion_ID, int VAB_Project_ID, int VAB_Promotion_ID,
            int VAB_BillingCode_ID, int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID, int UserElement3_ID, int UserElement4_ID,
            int UserElement5_ID, int UserElement6_ID, int UserElement7_ID, int UserElement8_ID, int UserElement9_ID, string Alias)
        {
            AccountingObjects obj = new AccountingObjects();
            MVABAccount acct = null;
            string qry = "SELECT Count(*) FROM VAF_Column WHERE ColumnName = 'UserElement3_ID' AND VAF_TableView_ID = 176";
            if (Util.GetValueOfInt(DBase.DB.ExecuteScalar(qry, null, null)) > 0)
            {
                acct = MVABAccount.Get(ctx, VAF_Client_ID, VAF_Org_ID, VAB_AccountBook_ID, AD_Account_ID, VAB_SubAcct_ID, VAM_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID,
                   C_LocFrom_ID, C_LocTo_ID, C_SRegion_ID, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID, User1_ID, User2_ID, UserElement1_ID, UserElement2_ID,
                   UserElement3_ID, UserElement4_ID, UserElement5_ID, UserElement6_ID, UserElement7_ID, UserElement8_ID, UserElement9_ID);
            }
            else
            {
                acct = MVABAccount.Get(ctx, VAF_Client_ID, VAF_Org_ID, VAB_AccountBook_ID, AD_Account_ID, VAB_SubAcct_ID, VAM_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID,
                    C_LocFrom_ID, C_LocTo_ID, C_SRegion_ID, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID, User1_ID, User2_ID, UserElement1_ID, UserElement2_ID);
            }

            if (acct != null && acct.Get_ID() == 0)
            {
                acct.Save();
            }

            //  Show Info
            if (acct == null || acct.Get_ID() == 0)
            {
                obj = LoadInfo(0, 0);
            }
            else
            {
                //	Update Account with optional Alias
                bool found = false;
                if (Alias.Length > 0)
                {
                    String sql1 = "SELECT COUNT(*) FROM VAB_Acct_ValidParameter WHERE Alias='" + Alias + "'";
                    int ii = DB.GetSQLValue(null, sql1);

                    if (ii != 0)
                    {
                        found = true;
                    }
                    else
                    {
                        acct.SetAlias(Alias);
                        acct.Save();
                    }
                }

                if (found)
                {
                    obj.ErrorMsg = "DuplicateAlias";
                }
                else
                {
                    //obj = LoadInfo(acct.Get_ID(), VAB_AccountBook_ID);
                    obj.VAB_Acct_ValidParameter_ID = acct.Get_ID();
                    obj.VAB_AccountBook_ID = VAB_AccountBook_ID;
                }
            }
            return obj;
        }

        /// <summary>
        /// Load info for control sttting to return into form
        /// </summary>
        /// <param name="VAB_Acct_ValidParameter_ID"></param>
        /// <param name="VAB_AccountBook_ID"></param>
        /// <returns></returns>
        private AccountingObjects LoadInfo(int VAB_Acct_ValidParameter_ID, int VAB_AccountBook_ID)
        {
            AccountingObjects obj = new AccountingObjects();
            //log.Fine("VAB_Acct_ValidParameter_ID=" + VAB_Acct_ValidParameter_ID);
            String sql = "SELECT * FROM VAB_Acct_ValidParameter WHERE VAB_Acct_ValidParameter_ID=" + VAB_Acct_ValidParameter_ID + " AND VAB_AccountBook_ID=" + VAB_AccountBook_ID;
            IDataReader dr = null;
            dr = DB.ExecuteReader(sql);
            try
            {
                if (dr.Read())
                {
                    //obj.Alias = Convert.ToString(dr["Alias"]);
                    //obj.Combination = Convert.ToString(dr["Combination"]);
                    //obj.VAF_Org_ID = Convert.ToInt32(dr["VAF_Org_ID"]);
                    //obj.Account_ID = Convert.ToInt32(dr["Account_ID"]);
                    //obj.VAB_SubAcct_ID = Convert.ToInt32(dr["VAB_SubAcct_ID"]);
                    //obj.VAM_Product_ID = Convert.ToInt32(dr["VAM_Product_ID"]);
                    //obj.VAB_BusinessPartner_ID = Convert.ToInt32(dr["VAB_BusinessPartner_ID"]);
                    //obj.VAB_Promotion_ID = Convert.ToInt32(dr["VAB_Promotion_ID"]);
                    //obj.C_LocFrom_ID = Convert.ToInt32(dr["C_LocFrom_ID"]);
                    //obj.C_LocTo_ID = Convert.ToInt32(dr["C_LocTo_ID"]);
                    //obj.VAB_Project_ID = Convert.ToInt32(dr["VAB_Project_ID"]);
                    //obj.VAB_SalesRegionState_ID = Convert.ToInt32(dr["VAB_SalesRegionState_ID"]);
                    //obj.VAF_OrgTrx_ID = Convert.ToInt32(dr["VAF_OrgTrx_ID"]);
                    //obj.VAB_BillingCode_ID = Convert.ToInt32(dr["VAB_BillingCode_ID"]);
                    //obj.User1_ID = Convert.ToInt32(dr["User1_ID"]);
                    //obj.User2_ID = Convert.ToInt32(dr["User2_ID"]);
                    //obj.Description = Convert.ToString(dr["Description"]);
                }
                dr.Close();
                dr = null;
            }
            catch (Exception)
            {
                // log.Log(Level.SEVERE, sql, e);
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }
            return obj;
        }

    }
}
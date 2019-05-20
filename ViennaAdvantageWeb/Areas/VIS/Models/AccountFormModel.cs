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
        public int AD_Column_ID { get; set; }
        public bool IsHeavyData { get; set; }
    }

    public class AccountingObjects
    {
        public string ErrorMsg { get; set; }
        public int C_ValidCombination_ID { get; set; }
        public int C_AcctSchema_ID { get; set; }
    }

    public class AccountFormModel
    {
        /// <summary>
        /// Load controls lookup etc.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="windowNo"></param>
        /// <param name="C_AcctSchema_ID"></param>
        /// <returns>class lookups</returns>
        public AccountSchema AccountSchemaLoad(Ctx ctx, int windowNo, int C_AcctSchema_ID)
        {
            AccountSchema objSchema = new AccountSchema();
            MAcctSchemaElement[] elements = null;

            var _AcctSchema = new MAcctSchema(ctx, C_AcctSchema_ID, null);
            ctx.GetCtx(windowNo).SetContext(windowNo, "C_AcctSchema_ID", C_AcctSchema_ID);
            elements = _AcctSchema.GetAcctSchemaElements();

            objSchema.IsHasAlies = _AcctSchema.IsHasAlias();
            objSchema.Elements = new List<AccountingElements>();
            for (int i = 0; i < elements.Length; i++)
            {
                AccountingElements obj = new AccountingElements();
                MAcctSchemaElement ase = elements[i];
                obj.Type = ase.GetElementType();
                obj.IsMandatory = ase.IsMandatory();
                obj.ID = ase.Get_ID();
                obj.Name = ase.GetName();
                obj.DefaultValue = ase.GetDefaultValue();
                obj.SeqNo = ase.GetSeqNo();
                obj.AD_Column_ID = ase.GetAD_Column_ID();
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
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="C_AcctSchema_ID"></param>
        /// <param name="AD_Account_ID"></param>
        /// <param name="C_SubAcct_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="C_BPartner_ID"></param>
        /// <param name="AD_OrgTrx_ID"></param>
        /// <param name="C_LocFrom_ID"></param>
        /// <param name="C_LocTo_ID"></param>
        /// <param name="C_SRegion_ID"></param>
        /// <param name="C_Project_ID"></param>
        /// <param name="C_Campaign_ID"></param>
        /// <param name="C_Activity_ID"></param>
        /// <param name="User1_ID"></param>
        /// <param name="User2_ID"></param>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public AccountingObjects SaveAccount(Ctx ctx, int AD_Client_ID, int AD_Org_ID, int C_AcctSchema_ID, int AD_Account_ID, int C_SubAcct_ID, int M_Product_ID,
            int C_BPartner_ID, int AD_OrgTrx_ID, int C_LocFrom_ID, int C_LocTo_ID, int C_SRegion_ID, int C_Project_ID, int C_Campaign_ID,
            int C_Activity_ID, int User1_ID, int User2_ID, int UserElement1_ID, int UserElement2_ID, int UserElement3_ID, int UserElement4_ID,
            int UserElement5_ID, int UserElement6_ID, int UserElement7_ID, int UserElement8_ID, int UserElement9_ID, string Alias)
        {
            AccountingObjects obj = new AccountingObjects();
            MAccount acct = null;
            string qry = "SELECT Count(*) FROM AD_Column WHERE ColumnName = 'UserElement3_ID' AND AD_Table_ID = 176";
            if (Util.GetValueOfInt(DBase.DB.ExecuteScalar(qry, null, null)) > 0)
            {
                acct = MAccount.Get(ctx, AD_Client_ID, AD_Org_ID, C_AcctSchema_ID, AD_Account_ID, C_SubAcct_ID, M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID,
                   C_LocFrom_ID, C_LocTo_ID, C_SRegion_ID, C_Project_ID, C_Campaign_ID, C_Activity_ID, User1_ID, User2_ID, UserElement1_ID, UserElement2_ID,
                   UserElement3_ID, UserElement4_ID, UserElement5_ID, UserElement6_ID, UserElement7_ID, UserElement8_ID, UserElement9_ID);
            }
            else
            {
                acct = MAccount.Get(ctx, AD_Client_ID, AD_Org_ID, C_AcctSchema_ID, AD_Account_ID, C_SubAcct_ID, M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID,
                    C_LocFrom_ID, C_LocTo_ID, C_SRegion_ID, C_Project_ID, C_Campaign_ID, C_Activity_ID, User1_ID, User2_ID, UserElement1_ID, UserElement2_ID);
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
                    String sql1 = "SELECT COUNT(*) FROM C_ValidCombination WHERE Alias='" + Alias + "'";
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
                    //obj = LoadInfo(acct.Get_ID(), C_AcctSchema_ID);
                    obj.C_ValidCombination_ID = acct.Get_ID();
                    obj.C_AcctSchema_ID = C_AcctSchema_ID;
                }
            }
            return obj;
        }

        /// <summary>
        /// Load info for control sttting to return into form
        /// </summary>
        /// <param name="C_ValidCombination_ID"></param>
        /// <param name="C_AcctSchema_ID"></param>
        /// <returns></returns>
        private AccountingObjects LoadInfo(int C_ValidCombination_ID, int C_AcctSchema_ID)
        {
            AccountingObjects obj = new AccountingObjects();
            //log.Fine("C_ValidCombination_ID=" + C_ValidCombination_ID);
            String sql = "SELECT * FROM C_ValidCombination WHERE C_ValidCombination_ID=" + C_ValidCombination_ID + " AND C_AcctSchema_ID=" + C_AcctSchema_ID;
            IDataReader dr = null;
            dr = DB.ExecuteReader(sql);
            try
            {
                if (dr.Read())
                {
                    //obj.Alias = Convert.ToString(dr["Alias"]);
                    //obj.Combination = Convert.ToString(dr["Combination"]);
                    //obj.AD_Org_ID = Convert.ToInt32(dr["AD_Org_ID"]);
                    //obj.Account_ID = Convert.ToInt32(dr["Account_ID"]);
                    //obj.C_SubAcct_ID = Convert.ToInt32(dr["C_SubAcct_ID"]);
                    //obj.M_Product_ID = Convert.ToInt32(dr["M_Product_ID"]);
                    //obj.C_BPartner_ID = Convert.ToInt32(dr["C_BPartner_ID"]);
                    //obj.C_Campaign_ID = Convert.ToInt32(dr["C_Campaign_ID"]);
                    //obj.C_LocFrom_ID = Convert.ToInt32(dr["C_LocFrom_ID"]);
                    //obj.C_LocTo_ID = Convert.ToInt32(dr["C_LocTo_ID"]);
                    //obj.C_Project_ID = Convert.ToInt32(dr["C_Project_ID"]);
                    //obj.C_SalesRegion_ID = Convert.ToInt32(dr["C_SalesRegion_ID"]);
                    //obj.AD_OrgTrx_ID = Convert.ToInt32(dr["AD_OrgTrx_ID"]);
                    //obj.C_Activity_ID = Convert.ToInt32(dr["C_Activity_ID"]);
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
/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : OrgOwnership
 * Purpose        : Org Ownership Process
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     28-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class OrgOwnership : ProcessEngine.SvrProcess
    {
        //Organization Parameter		
        private int _VAF_Org_ID = -1;
        private int _M_Warehouse_ID = -1;
        private int _M_Product_Category_ID = -1;
        private int _M_Product_ID = -1;
        private int _C_BP_Group_ID = -1;
        private int _C_BPartner_ID = -1;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAF_Org_ID"))
                {
                    _VAF_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("M_Warehouse_ID"))
                {
                    _M_Warehouse_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }

                else if (name.Equals("M_Product_Category_ID"))
                {
                    _M_Product_Category_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }

                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }

                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            log.Info("doIt - VAF_Org_ID=" + _VAF_Org_ID);
            if (_VAF_Org_ID < 0)
            {
                throw new Exception("OrgOwnership - invalid VAF_Org_ID=" + _VAF_Org_ID);
            }

            GeneralOwnership();

            if (_M_Warehouse_ID > 0)
            {
                return WarehouseOwnership();
            }

            if (_M_Product_ID > 0 || _M_Product_Category_ID > 0)
            {
                return ProductOwnership();
            }

            if (_C_BPartner_ID > 0 || _C_BP_Group_ID > 0)
            {
                return BPartnerOwnership();
            }

            return "* Not supported * **";
        }

        /// <summary>
        /// Set Warehouse Ownership
        /// </summary>
        /// <returns>""</returns>
        private String WarehouseOwnership()
        {
            log.Info("warehouseOwnership - M_Warehouse_ID=" + _M_Warehouse_ID);
            if (_VAF_Org_ID == 0)
            {
                throw new Exception("Warehouse - Org cannot be * (0)");
            }

            //	Set Warehouse
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE M_Warehouse "
                + "SET VAF_Org_ID=").Append(_VAF_Org_ID)
                .Append(" WHERE M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(" AND VAF_Client_ID=").Append(GetVAF_Client_ID())
                .Append(" AND VAF_Org_ID<>").Append(_VAF_Org_ID);
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Warehouse_ID"));

            //	Set Accounts
            sql = new StringBuilder();
            sql.Append("UPDATE M_Warehouse_Acct "
                + "SET VAF_Org_ID=").Append(_VAF_Org_ID)
                .Append(" WHERE M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(" AND VAF_Client_ID=").Append(GetVAF_Client_ID())
                .Append(" AND VAF_Org_ID<>").Append(_VAF_Org_ID);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));

            //	Set Locators
            sql = new StringBuilder();
            sql.Append("UPDATE M_Locator "
                + "SET VAF_Org_ID=").Append(_VAF_Org_ID)
                .Append(" WHERE M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(" AND VAF_Client_ID=").Append(GetVAF_Client_ID())
                .Append(" AND VAF_Org_ID<>").Append(_VAF_Org_ID);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Locator_ID"));

            //	Set Storage
            sql = new StringBuilder();
            sql.Append("UPDATE M_Storage s "
                + "SET VAF_Org_ID=").Append(_VAF_Org_ID)
                .Append(" WHERE EXISTS "
                    + "(SELECT * FROM M_Locator l WHERE l.M_Locator_ID=s.M_Locator_ID"
                    + " AND l.M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(") AND VAF_Client_ID=").Append(GetVAF_Client_ID())
                .Append(" AND VAF_Org_ID<>").Append(_VAF_Org_ID);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Storage"));
            return "";
        }

        /// <summary>
        /// Product Ownership
        /// </summary>
        /// <returns>""</returns>
        private String ProductOwnership()
        {
            log.Info("productOwnership - M_Product_Category_ID=" + _M_Product_Category_ID
                + ", M_Product_ID=" + _M_Product_ID);

            String set = " SET VAF_Org_ID=" + _VAF_Org_ID;
            if (_M_Product_Category_ID > 0)
            {
                set += " WHERE EXISTS (SELECT * FROM M_Product p"
                    + " WHERE p.M_Product_ID=x.M_Product_ID AND p.M_Product_Category_ID="
                        + _M_Product_Category_ID + ")";
            }
            else
            {
                set += " WHERE M_Product_ID=" + _M_Product_ID;
            }
            set += " AND VAF_Client_ID=" + GetVAF_Client_ID() + " AND VAF_Org_ID<>" + _VAF_Org_ID;
            log.Fine("productOwnership - " + set);

            //	Product
            String sql = "UPDATE M_Product x " + set;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Product_ID"));

            //	Acct
            sql = "UPDATE M_Product_Acct x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));

            //	BOM
            sql = "UPDATE M_Product_BOM x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Product_BOM_ID"));

            //	PO
            sql = "UPDATE M_Product_PO x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Product_PO"));

            //	Trl
            sql = "UPDATE M_Product_Trl x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "VAF_Language"));

            //Added by Pratap 30-12-15 M_Replenish,M_Substitute,C_BPartner_Product - Mantis Issue ID - 0000441
            //	M_Replenish 
            sql = "UPDATE M_Replenish x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Replenish"));

            //	M_Substitute
            sql = "UPDATE M_Substitute x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Substitute"));

            //	C_BPartner_Product
            sql = "UPDATE C_BPartner_Product x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BPartner_Product"));


            return "";
        }

        /// <summary>
        /// Business Partner Ownership
        /// </summary>
        /// <returns>""</returns>
        private String BPartnerOwnership()
        {
            log.Info("bPartnerOwnership - C_BP_Group_ID=" + _C_BP_Group_ID
                + ", C_BPartner_ID=" + _C_BPartner_ID);

            String set = " SET VAF_Org_ID=" + _VAF_Org_ID;
            if (_C_BP_Group_ID > 0)
            {
                set += " WHERE EXISTS (SELECT * FROM C_BPartner bp WHERE bp.C_BPartner_ID=x.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")";
            }
            else
            {
                set += " WHERE C_BPartner_ID=" + _C_BPartner_ID;
            }
            set += " AND VAF_Client_ID=" + GetVAF_Client_ID() + " AND VAF_Org_ID<>" + _VAF_Org_ID;
            log.Fine("bPartnerOwnership - " + set);

            //	BPartner
            String sql = "UPDATE C_BPartner x " + set;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BPartner_ID"));

            //	Acct xxx
            sql = "UPDATE C_BP_Customer_Acct x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
            sql = "UPDATE C_BP_Employee_Acct x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
            sql = "UPDATE C_BP_Vendor_Acct x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));

            //	Location
            sql = "UPDATE C_BPartner_Location x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BPartner_Location_ID"));

            //	Contcat/User
            sql = "UPDATE VAF_UserContact x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "VAF_UserContact_ID"));

            //	BankAcct
            sql = "UPDATE C_BP_BankAccount x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BP_BankAccount_ID"));

            return "";
        }

        /// <summary>
        /// Set General Ownership (i.e. Org to 0).
        /// In general for items with two parents
        /// </summary>
        private void GeneralOwnership()
        {
            String set = "SET VAF_Org_ID=0 WHERE VAF_Client_ID=" + GetVAF_Client_ID()
                + " AND VAF_Org_ID<>0";

            //	R_ContactInterest
            String sql = "UPDATE R_ContactInterest " + set;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - R_ContactInterest=" + no);
            }

            //	VAF_UserContact_Roles
            sql = "UPDATE VAF_UserContact_Roles " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - VAF_UserContact_Roles=" + no);
            }

            //	C_BPartner_Product
            sql = "UPDATE C_BPartner_Product " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - C_BPartner_Product=" + no);
            }

            //	Withholding
            sql = "UPDATE C_BP_Withholding x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - C_BP_Withholding=" + no);
            }

            //	Costing
            sql = "UPDATE M_Product_Costing " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - M_Product_Costing=" + no);
            }

            //	Replenish
            sql = "UPDATE M_Replenish " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - M_Replenish=" + no);
            }

        }
    }
}

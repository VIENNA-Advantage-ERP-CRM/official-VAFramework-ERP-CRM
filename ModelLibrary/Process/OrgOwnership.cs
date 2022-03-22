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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class OrgOwnership : ProcessEngine.SvrProcess
    {
        //Organization Parameter		
        private int _AD_Org_ID = -1;
        private int _M_Warehouse_ID = -1;
        private int _M_Product_Category_ID = -1;
        private int _M_Product_ID = -1;
        private int _C_BP_Group_ID = -1;
        private int _C_BPartner_ID = -1;
        private MSession session = null;

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
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
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
            log.Info("doIt - AD_Org_ID=" + _AD_Org_ID);
            if (_AD_Org_ID < 0)
            {
                throw new Exception("OrgOwnership - invalid AD_Org_ID=" + _AD_Org_ID);
            }

            session = MSession.Get(GetCtx());

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
            if (_AD_Org_ID == 0)
            {
                throw new Exception("Warehouse - Org cannot be * (0)");
            }

            //	Set Warehouse
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE M_Warehouse "
                + "SET AD_Org_ID=").Append(_AD_Org_ID)
                .Append(" WHERE M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(" AND AD_Client_ID=").Append(GetAD_Client_ID())
                .Append(" AND AD_Org_ID<>").Append(_AD_Org_ID);
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Warehouse_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Warehouse.Table_ID, sql.ToString(), 1);
            }

            //	VIS_0045: 11-03-22 -> Set Accounts
            if (Env.IsModuleInstalled("FRPT_"))
            {
                sql = new StringBuilder();
                sql.Append("UPDATE FRPT_Warehouse_Acct "
                    + "SET AD_Org_ID=").Append(_AD_Org_ID)
                    .Append(" WHERE M_Warehouse_ID=").Append(_M_Warehouse_ID)
                    .Append(" AND AD_Client_ID=").Append(GetAD_Client_ID())
                    .Append(" AND AD_Org_ID<>").Append(_AD_Org_ID);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
            }

            //	Set Locators
            sql = new StringBuilder();
            sql.Append("UPDATE M_Locator "
                + "SET AD_Org_ID=").Append(_AD_Org_ID)
                .Append(" WHERE M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(" AND AD_Client_ID=").Append(GetAD_Client_ID())
                .Append(" AND AD_Org_ID<>").Append(_AD_Org_ID);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Locator_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Locator.Table_ID, sql.ToString(), 1);
            }

            //	Set Storage
            sql = new StringBuilder();
            sql.Append("UPDATE M_Storage s "
                + "SET AD_Org_ID=").Append(_AD_Org_ID)
                .Append(" WHERE EXISTS "
                    + "(SELECT * FROM M_Locator l WHERE l.M_Locator_ID=s.M_Locator_ID"
                    + " AND l.M_Warehouse_ID=").Append(_M_Warehouse_ID)
                .Append(") AND AD_Client_ID=").Append(GetAD_Client_ID())
                .Append(" AND AD_Org_ID<>").Append(_AD_Org_ID);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Storage"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Storage.Table_ID, sql.ToString(), 1);
            }

            //	VIS_0045: 11-03-22 -> Set Container Storage
            if (Util.GetValueOfString(GetCtx().GetContext("#PRODUCT_CONTAINER_APPLICABLE")).Equals("Y"))
            {
                sql = new StringBuilder();
                sql.Append("UPDATE M_ContainerStorage s "
                    + "SET AD_Org_ID=").Append(_AD_Org_ID)
                    .Append(" WHERE EXISTS "
                        + "(SELECT * FROM M_Locator l WHERE l.M_Locator_ID=s.M_Locator_ID"
                        + " AND l.M_Warehouse_ID=").Append(_M_Warehouse_ID)
                    .Append(") AND AD_Client_ID=").Append(GetAD_Client_ID())
                    .Append(" AND AD_Org_ID<>").Append(_AD_Org_ID);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_ContainerStorage_ID"));
                // maintain query log
                if (no > 0)
                {
                    session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_ContainerStorage.Table_ID, sql.ToString(), 1);
                }
            }
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

            String set = " SET AD_Org_ID=" + _AD_Org_ID;
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
            set += " AND AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID<>" + _AD_Org_ID;
            log.Fine("productOwnership - " + set);

            //	Product
            String sql = "UPDATE M_Product x " + set;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Product_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Product.Table_ID, sql, 1);
            }

            //	VIS_0045: 11-03-22 -> Acct
            if (Env.IsModuleInstalled("FRPT_"))
            {
                sql = "UPDATE FRPT_Product_Acct x " + set;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
            }

            //	BOM
            sql = "UPDATE M_Product_BOM x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Product_BOM_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Product_BOM.Table_ID, sql, 1);
            }

            sql = "UPDATE M_BOM  x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_BOM_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_BOM.Table_ID, sql, 1);
            }

            sql = "UPDATE M_BOMProduct x SET AD_Org_ID = " + _AD_Org_ID +
                  " WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID<>" + _AD_Org_ID;
            if (_M_Product_Category_ID > 0)
            {
                sql += @" AND M_BOM_ID IN  (SELECT M_BOM_ID FROM M_BOM WHERE M_Product_ID IN (SELECT M_Product_ID FROM M_Product p
                         WHERE p.M_Product_Category_ID=" + _M_Product_Category_ID + "))";
            }
            else
            {
                sql += " AND M_BOM_ID IN (SELECT M_BOM_ID FROM M_BOM WHERE M_Product_ID = " + _M_Product_ID + ")";
            }
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_BOMProduct_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_BOMProduct.Table_ID, sql, 1);
            }

            //	PO
            sql = "UPDATE M_Product_PO x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Product_PO"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Product_PO.Table_ID, sql, 1);
            }

            //	Trl
            sql = "UPDATE M_Product_Trl x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "AD_Language"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Product_Trl.Table_ID, sql, 1);
            }

            //Added by Pratap 30-12-15 M_Replenish,M_Substitute,C_BPartner_Product - Mantis Issue ID - 0000441
            //	M_Replenish 
            sql = "UPDATE M_Replenish x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Replenish"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Replenish.Table_ID, sql, 1);
            }

            //	M_Substitute
            sql = "UPDATE M_Substitute x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_Substitute"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Substitute.Table_ID, sql, 1);
            }

            //	C_BPartner_Product
            sql = "UPDATE C_BPartner_Product x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BPartner_Product"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_BPartner_Product.Table_ID, sql, 1);
            }

            // VIS_0045: 11-03-22
            if (Env.IsModuleInstalled("VA076_"))
            {
                sql = "UPDATE VA076_Attributes x " + set;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "VA076_Attributes_ID"));

                sql = "UPDATE VA076_Attributes2  x " + set;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "VA076_Attributes2_ID"));
            }

            sql = "UPDATE M_RelatedProduct  x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_RelatedProduct_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_RelatedProduct.Table_ID, sql, 1);
            }

            sql = "UPDATE M_ProductLocator  x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_ProductLocator_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_ProductLocator.Table_ID, sql, 1);
            }

            sql = "UPDATE M_ProductDownload   x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_ProductDownload_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_ProductDownload.Table_ID, sql, 1);
            }

            sql = "UPDATE C_UOM_Conversion   x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_UOM_Conversion_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_UOM_Conversion.Table_ID, sql, 1);
            }

            sql = "UPDATE M_ProductAttributes   x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_ProductAttributes_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_ProductAttributes.Table_ID, sql, 1);
            }

            sql = "UPDATE M_ProductAttributeImage  x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_ProductAttributeImage_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_ProductAttributeImage.Table_ID, sql, 1);
            }

            sql = "UPDATE M_TransactionSummary  x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_TransactionSummary_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_TransactionSummary.Table_ID, sql, 1);
            }

            sql = "UPDATE M_ProductStockSummary  x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "M_ProductStockSummary_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_ProductStockSummary.Table_ID, sql, 1);
            }

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

            String set = " SET AD_Org_ID=" + _AD_Org_ID;
            if (_C_BP_Group_ID > 0)
            {
                set += " WHERE EXISTS (SELECT * FROM C_BPartner bp WHERE bp.C_BPartner_ID=x.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")";
            }
            else
            {
                set += " WHERE C_BPartner_ID=" + _C_BPartner_ID;
            }
            set += " AND AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID<>" + _AD_Org_ID;
            log.Fine("bPartnerOwnership - " + set);

            //	BPartner
            String sql = "UPDATE C_BPartner x " + set;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BPartner_ID"));

            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_BPartner.Table_ID, sql, 1);
            }

            // VIS_0045: 11-03-22 -> Acct xxx
            if (Env.IsModuleInstalled("FRPT_"))
            {
                sql = "UPDATE FRPT_BP_Customer_Acct x " + set;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
                sql = "UPDATE FRPT_BP_Employee_Acct x " + set;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
                sql = "UPDATE FRPT_BP_Vendor_Acct x " + set;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_AcctSchema_ID"));
            }

            //	Location
            sql = "UPDATE C_BPartner_Location x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BPartner_Location_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_BPartner_Location.Table_ID, sql, 1);
            }

            //	Contcat/User
            sql = "UPDATE AD_User x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "AD_User_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_AD_User.Table_ID, sql, 1);
            }

            //	BankAcct
            sql = "UPDATE C_BP_BankAccount x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "C_BP_BankAccount_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_BP_BankAccount.Table_ID, sql, 1);
            }

            // VIS_0045: 11-03-22 -> BP Access
            sql = "UPDATE AD_UserBPAccess x SET AD_Org_ID=" + _AD_Org_ID +
                   @" WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID<>" + _AD_Org_ID;
            if (_C_BP_Group_ID > 0)
            {
                sql += @" AND AD_User_ID IN (SELECT AD_User_ID FROM AD_User WHERE C_BPartner_ID IN (
                            (SELECT C_BPartner_ID FROM C_BPartner bp WHERE bp.C_BP_Group_ID=" + _C_BP_Group_ID + "))";
            }
            else
            {
                sql += " AND AD_User_ID IN (SELECT AD_User_ID FROM AD_User WHERE C_BPartner_ID=" + _C_BPartner_ID + ")";
            }
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            AddLog(0, null, new Decimal(no), Msg.Translate(GetCtx(), "AD_UserBPAccess_ID"));
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_AD_UserBPAccess.Table_ID, sql, 1);
            }

            return "";
        }

        /// <summary>
        /// Set General Ownership (i.e. Org to 0).
        /// In general for items with two parents
        /// </summary>
        private void GeneralOwnership()
        {
            String set = "SET AD_Org_ID=0 WHERE AD_Client_ID=" + GetAD_Client_ID()
                + " AND AD_Org_ID<>0";

            //	R_ContactInterest
            String sql = "UPDATE R_ContactInterest " + set;
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - R_ContactInterest=" + no);
            }
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_R_ContactInterest.Table_ID, sql.ToString(), 1);
            }

            //	AD_User_Roles
            sql = "UPDATE AD_User_Roles " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - AD_User_Roles=" + no);
            }
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_AD_User_Roles.Table_ID, sql.ToString(), 1);
            }

            //	C_BPartner_Product
            sql = "UPDATE C_BPartner_Product " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - C_BPartner_Product=" + no);
            }
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_BPartner_Product.Table_ID, sql.ToString(), 1);
            }

            //	Withholding
            sql = "UPDATE C_BP_Withholding x " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - C_BP_Withholding=" + no);
            }
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_C_BP_Withholding.Table_ID, sql.ToString(), 1);
            }

            //	Costing
            sql = "UPDATE M_Product_Costing " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - M_Product_Costing=" + no);
            }
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Product_Costing.Table_ID, sql.ToString(), 1);
            }

            //	Replenish
            sql = "UPDATE M_Replenish " + set;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("generalOwnership - M_Replenish=" + no);
            }
            // maintain query log
            if (no > 0)
            {
                session.QueryLog(GetAD_Client_ID(), GetAD_Org_ID(), X_M_Replenish.Table_ID, sql.ToString(), 1);
            }

        }
    }
}

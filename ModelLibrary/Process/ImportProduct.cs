/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportProduct
 * Purpose        : Import Products from I_Product
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class ImportProduct : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /** Effective						*/
        private DateTime? _DateValue = null;
        /** Pricelist to Update				*/
        private int _M_PriceList_Version_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("AD_Client_ID"))
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("M_PriceList_Version_ID"))
                    _M_PriceList_Version_ID = para[i].GetParameterAsInt();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            if (_DateValue == null)
                _DateValue = Utility.Util.GetValueOfDateTime(DateTime.Now);// new Timestamp (System.currentTimeMillis());
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>meesaage</returns>
        protected override String DoIt()
        {
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_Product "
                    + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Info("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IaActive, Created/Updated, 	ProductType
            sql = new StringBuilder("UPDATE I_Product "
                + "SET AD_Client_ID = COALESCE (AD_Client_ID, ").Append(_AD_Client_ID).Append("),"
                + " AD_Org_ID = COALESCE (AD_Org_ID, 0),"
                + " IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                + " ProductType = COALESCE (ProductType, 'I'),"
                + " I_ErrorMsg = NULL,"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            //	Set Optional BPartner
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET C_BPartner_ID=(SELECT C_BPartner_ID FROM C_BPartner p"
                + " WHERE i.BPartner_Value=p.Value AND i.AD_Client_ID=p.AD_Client_ID) "
                + "WHERE C_BPartner_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("BPartner=" + no);
            //
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_Product "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid BPartner,' "
                + "WHERE C_BPartner_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid BPartner=" + no);


            //	****	Find Product
            //	EAN/UPC
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET M_Product_ID=(SELECT M_Product_ID FROM M_Product p"
                + " WHERE i.UPC=p.UPC AND i.AD_Client_ID=p.AD_Client_ID) "
                + "WHERE M_Product_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Product Existing UPC=" + no);

            //	Value
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET M_Product_ID=(SELECT M_Product_ID FROM M_Product p"
                + " WHERE i.Value=p.Value AND i.AD_Client_ID=p.AD_Client_ID) "
                + "WHERE M_Product_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Product Existing Value=" + no);

            //	BP ProdNo
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET M_Product_ID=(SELECT M_Product_ID FROM M_Product_po p"
                + " WHERE i.C_BPartner_ID=p.C_BPartner_ID"
                + " AND i.VendorProductNo=p.VendorProductNo AND i.AD_Client_ID=p.AD_Client_ID) "
                + "WHERE M_Product_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Product Existing Vendor ProductNo=" + no);

            //	Set Product Category
            sql = new StringBuilder("UPDATE I_Product "
                + "SET ProductCategory_Value=(SELECT MAX(Value) FROM M_Product_Category"
                + " WHERE IsDefault='Y' AND AD_Client_ID=").Append(_AD_Client_ID).Append(") "
                + "WHERE ProductCategory_Value IS NULL AND M_Product_Category_ID IS NULL"
                + " AND M_Product_ID IS NULL"	//	set category only if product not found 
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Category Default Value=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET M_Product_Category_ID=(SELECT M_Product_Category_ID FROM M_Product_Category c"
                + " WHERE i.ProductCategory_Value=c.Value AND i.AD_Client_ID=c.AD_Client_ID) "
                + "WHERE ProductCategory_Value IS NOT NULL AND M_Product_Category_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Set Category=" + no);


            //	Copy From Product if Import does not have value
            String[] strFields = new String[] {"Value","Name","Description","DocumentNote","Help",
			"UPC","SKU","Classification","ProductType",
			"Discontinued","DiscontinuedBy","ImageURL","DescriptionURL"};
            for (int i = 0; i < strFields.Length; i++)
            {
                sql = new StringBuilder("UPDATE I_PRODUCT i "
                    + "SET ").Append(strFields[i]).Append(" = (SELECT ").Append(strFields[i]).Append(" FROM M_Product p"
                    + " WHERE i.M_Product_ID=p.M_Product_ID AND i.AD_Client_ID=p.AD_Client_ID) "
                    + "WHERE M_Product_ID IS NOT NULL"
                    + " AND ").Append(strFields[i]).Append(" IS NULL"
                    + " AND I_IsImported='N'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                if (no != 0)
                    log.Fine(strFields[i] + " - default from existing Product=" + no);
            }
            String[] numFields = new String[] {"C_UOM_ID","M_Product_Category_ID",
			"Volume","Weight","ShelfWidth","ShelfHeight","ShelfDepth","UnitsPerPallet"};
            for (int i = 0; i < numFields.Length; i++)
            {
                sql = new StringBuilder("UPDATE I_PRODUCT i "
                    + "SET ").Append(numFields[i]).Append(" = (SELECT ").Append(numFields[i]).Append(" FROM M_Product p"
                    + " WHERE i.M_Product_ID=p.M_Product_ID AND i.AD_Client_ID=p.AD_Client_ID) "
                    + "WHERE M_Product_ID IS NOT NULL"
                    + " AND (").Append(numFields[i]).Append(" IS NULL OR ").Append(numFields[i]).Append("=0)"
                    + " AND I_IsImported='N'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                if (no != 0)
                    log.Fine(numFields[i] + " default from existing Product=" + no);
            }

            //	Copy From Product_PO if Import does not have value
            String[] strFieldsPO = new String[] {"UPC",
			"PriceEffective","VendorProductNo","VendorCategory","Manufacturer",
			"Discontinued","DiscontinuedBy"};
            for (int i = 0; i < strFieldsPO.Length; i++)
            {
                sql = new StringBuilder("UPDATE I_PRODUCT i "
                    + "SET ").Append(strFieldsPO[i]).Append(" = (SELECT ").Append(strFieldsPO[i])
                    .Append(" FROM M_Product_PO p"
                    + " WHERE i.M_Product_ID=p.M_Product_ID AND i.C_BPartner_ID=p.C_BPartner_ID AND i.AD_Client_ID=p.AD_Client_ID) "
                    + "WHERE M_Product_ID IS NOT NULL AND C_BPartner_ID IS NOT NULL"
                    + " AND ").Append(strFieldsPO[i]).Append(" IS NULL"
                    + " AND I_IsImported='N'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                if (no != 0)
                    log.Fine(strFieldsPO[i] + " default from existing Product PO=" + no);
            }
            String[] numFieldsPO = new String[] {"C_UOM_ID","C_Currency_ID",
			"PriceList","PricePO","RoyaltyAmt",
			"Order_Min","Order_Pack","CostPerOrder","DeliveryTime_Promised"};
            for (int i = 0; i < numFieldsPO.Length; i++)
            {
                sql = new StringBuilder("UPDATE I_PRODUCT i "
                    + "SET ").Append(numFieldsPO[i]).Append(" = (SELECT ").Append(numFieldsPO[i])
                    .Append(" FROM M_Product_PO p"
                    + " WHERE i.M_Product_ID=p.M_Product_ID AND i.C_BPartner_ID=p.C_BPartner_ID AND i.AD_Client_ID=p.AD_Client_ID) "
                    + "WHERE M_Product_ID IS NOT NULL AND C_BPartner_ID IS NOT NULL"
                    + " AND (").Append(numFieldsPO[i]).Append(" IS NULL OR ").Append(numFieldsPO[i]).Append("=0)"
                    + " AND I_IsImported='N'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                if (no != 0)
                    log.Fine(numFieldsPO[i] + " default from existing Product PO=" + no);
            }

            //	Invalid Category
            sql = new StringBuilder("UPDATE I_Product "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ProdCategorty,' "
                + "WHERE M_Product_Category_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Category=" + no);


            //	Set UOM (System/own)
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET X12DE355 = "
                + "(SELECT MAX(X12DE355) FROM C_UOM u WHERE u.IsDefault='Y' AND u.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE X12DE355 IS NULL AND C_UOM_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set UOM Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET C_UOM_ID = (SELECT C_UOM_ID FROM C_UOM u WHERE u.X12DE355=i.X12DE355 AND u.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE C_UOM_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Set UOM=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid UOM, ' "
                + "WHERE C_UOM_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid UOM=" + no);


            //	Set Currency
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET ISO_Code=(SELECT ISO_Code FROM C_Currency c"
                + " INNER JOIN C_AcctSchema a ON (a.C_Currency_ID=c.C_Currency_ID)"
                + " INNER JOIN AD_ClientInfo ci ON (a.C_AcctSchema_ID=ci.C_AcctSchema1_ID)"
                + " WHERE ci.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE C_Currency_ID IS NULL AND ISO_Code IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Currency Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET C_Currency_ID=(SELECT C_Currency_ID FROM C_Currency c"
                + " WHERE i.ISO_Code=c.ISO_Code AND c.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE C_Currency_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("doIt- Set Currency=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Currency,' "
                + "WHERE C_Currency_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Currency=" + no);

            //	Verify ProductType
            sql = new StringBuilder("UPDATE I_Product "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ProductType,' "
                + "WHERE ProductType NOT IN ('I','S')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid ProductType=" + no);

            //	Unique UPC/Value
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Value not unique,' "
                + "WHERE I_IsImported<>'Y'"
                + " AND Value IN (SELECT Value FROM I_Product ii WHERE i.AD_Client_ID=ii.AD_Client_ID GROUP BY Value HAVING COUNT(*) > 1)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Not Unique Value=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=UPC not unique,' "
                + "WHERE I_IsImported<>'Y'"
                + " AND UPC IN (SELECT UPC FROM I_Product ii WHERE i.AD_Client_ID=ii.AD_Client_ID GROUP BY UPC HAVING COUNT(*) > 1)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Not Unique UPC=" + no);

            //	Mandatory Value
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Mandatory Value,' "
                + "WHERE Value IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Mandatory Value=" + no);

            //	Vendor Product No
            //	sql = new StringBuilder ("UPDATE I_Product i "
            //		+ "SET I_IsImported='E', I_ErrorMsg="+ts +"||'ERR=No Mandatory VendorProductNo,' "
            //		+ "WHERE I_IsImported<>'Y'"
            //		+ " AND VendorProductNo IS NULL AND (C_BPartner_ID IS NOT NULL OR BPartner_Value IS NOT NULL)").Append(clientCheck);
            //	no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
            //	log.Info(log.l3_Util, "No Mandatory VendorProductNo=" + no);
            sql = new StringBuilder("UPDATE I_Product "
                + "SET VendorProductNo=Value "
                + "WHERE C_BPartner_ID IS NOT NULL AND VendorProductNo IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("VendorProductNo Set to Value=" + no);
            //
            sql = new StringBuilder("UPDATE I_Product i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=VendorProductNo not unique,' "
                + "WHERE I_IsImported<>'Y'"
                + " AND C_BPartner_ID IS NOT NULL"
                + " AND (C_BPartner_ID, VendorProductNo) IN "
                + " (SELECT C_BPartner_ID, VendorProductNo FROM I_Product ii WHERE i.AD_Client_ID=ii.AD_Client_ID GROUP BY C_BPartner_ID, VendorProductNo HAVING COUNT(*) > 1)")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Not Unique VendorProductNo=" + no);

            //	Get Default Tax Category
            int C_TaxCategory_ID = 0;
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement
                idr = DataBase.DB.ExecuteReader("SELECT C_TaxCategory_ID FROM C_TaxCategory WHERE IsDefault='Y'" + clientCheck, null, Get_TrxName());

                if (idr.Read())
                {
                    C_TaxCategory_ID = Utility.Util.GetValueOfInt(idr[0]); //rs.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }

                throw new Exception("TaxCategory", e);
            }
            log.Fine("C_TaxCategory_ID=" + C_TaxCategory_ID);

            Commit();

            //	-------------------------------------------------------------------
            int noInsert = 0;
            int noUpdate = 0;
            int noInsertPO = 0;
            int noUpdatePO = 0;

            //	Go through Records
            log.Fine("start inserting/updating ...");
            sql = new StringBuilder("SELECT * FROM I_Product WHERE I_IsImported='N'")
                .Append(clientCheck);
            try
            {
                /*	Insert Product from Import
                PreparedStatement pstmt_insertProduct = conn.prepareStatement
                    ("INSERT INTO M_Product (M_Product_ID,"
                    + "AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,"
                    + "Value,Name,Description,DocumentNote,Help,"
                    + "UPC,SKU,C_UOM_ID,IsSummary,M_Product_Category_ID,C_TaxCategory_ID,"
                    + "ProductType,ImageURL,DescriptionURL) "
                    + "SELECT ?,"
                    + "AD_Client_ID,AD_Org_ID,'Y',SysDate,CreatedBy,SysDate,UpdatedBy,"
                    + "Value,Name,Description,DocumentNote,Help,"
                    + "UPC,SKU,C_UOM_ID,'N',M_Product_Category_ID," + C_TaxCategory_ID + ","
                    + "ProductType,ImageURL,DescriptionURL "
                    + "FROM I_Product "
                    + "WHERE I_Product_ID=?");
                */
                //	Update Product from Import
                //jz moved
                /*
                String sqlt = "UPDATE M_PRODUCT "
                    + "SET (Value,Name,Description,DocumentNote,Help,"
                    + "UPC,SKU,C_UOM_ID,M_Product_Category_ID,Classification,ProductType,"
                    + "Volume,Weight,ShelfWidth,ShelfHeight,ShelfDepth,UnitsPerPallet,"
                    + "Discontinued,DiscontinuedBy,Updated,UpdatedBy)= "
                    + "(SELECT Value,Name,Description,DocumentNote,Help,"
                    + "UPC,SKU,C_UOM_ID,M_Product_Category_ID,Classification,ProductType,"
                    + "Volume,Weight,ShelfWidth,ShelfHeight,ShelfDepth,UnitsPerPallet,"
                    + "Discontinued,DiscontinuedBy,SysDate,UpdatedBy"
                    + " FROM I_Product WHERE I_Product_ID=?) "
                    + "WHERE M_Product_ID=?";
                PreparedStatement pstmt_updateProduct = DataBase.prepareStatement
                    (sqlt, Get_TrxName());

                //	Update Product_PO from Import
                sqlt = "UPDATE M_Product_PO "
                    + "SET (IsCurrentVendor,C_UOM_ID,C_Currency_ID,UPC,"
                    + "PriceList,PricePO,RoyaltyAmt,PriceEffective,"
                    + "VendorProductNo,VendorCategory,Manufacturer,"
                    + "Discontinued,DiscontinuedBy,Order_Min,Order_Pack,"
                    + "CostPerOrder,DeliveryTime_Promised,Updated,UpdatedBy)= "
                    + "(SELECT 'Y',C_UOM_ID,C_Currency_ID,UPC,"
                    + "PriceList,PricePO,RoyaltyAmt,PriceEffective,"
                    + "VendorProductNo,VendorCategory,Manufacturer,"
                    + "Discontinued,DiscontinuedBy,Order_Min,Order_Pack,"
                    + "CostPerOrder,DeliveryTime_Promised,SysDate,UpdatedBy"
                    + " FROM I_Product"
                    + " WHERE I_Product_ID=?) "
                    + "WHERE M_Product_ID=? AND C_BPartner_ID=?";
                PreparedStatement pstmt_updateProductPO = DataBase.prepareStatement
                    (sqlt, Get_TrxName());
    */
                //	Insert Product from Import
                //PreparedStatement pstmt_insertProductPO = DataBase.prepareStatement
                String _insertProductPO = "INSERT INTO M_Product_PO (M_Product_ID,C_BPartner_ID, "
                    + "AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,"
                    + "IsCurrentVendor,C_UOM_ID,C_Currency_ID,UPC,"
                    + "PriceList,PricePO,RoyaltyAmt,PriceEffective,"
                    + "VendorProductNo,VendorCategory,Manufacturer,"
                    + "Discontinued,DiscontinuedBy,Order_Min,Order_Pack,"
                    + "CostPerOrder,DeliveryTime_Promised) "
                    + "SELECT @param1,@param2, "
                    + "AD_Client_ID,AD_Org_ID,'Y',SysDate,CreatedBy,SysDate,UpdatedBy,"
                    + "'Y',C_UOM_ID,C_Currency_ID,UPC,"
                    + "PriceList,PricePO,RoyaltyAmt,PriceEffective,"
                    + "VendorProductNo,VendorCategory,Manufacturer,"
                    + "Discontinued,DiscontinuedBy,Order_Min,Order_Pack,"
                    + "CostPerOrder,DeliveryTime_Promised "
                    + "FROM I_Product "
                    + "WHERE I_Product_ID=@param3";

                //	Set Imported = Y
                //PreparedStatement pstmt_setImported = DataBase.prepareStatement
                String _setImported = "UPDATE I_Product SET I_IsImported='Y', M_Product_ID=@param1, "
                + "Updated=SysDate, Processed='Y' WHERE I_Product_ID=@param2";

                //
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_I_Product imp = new X_I_Product(GetCtx(), idr, Get_TrxName());
                    int I_Product_ID = imp.GetI_Product_ID();
                    int M_Product_ID = imp.GetM_Product_ID();
                    int C_BPartner_ID = imp.GetC_BPartner_ID();
                    bool newProduct = M_Product_ID == 0;
                    log.Fine("I_Product_ID=" + I_Product_ID + ", M_Product_ID=" + M_Product_ID
                        + ", C_BPartner_ID=" + C_BPartner_ID);

                    //	Product
                    if (newProduct)			//	Insert new Product
                    {
                        MProduct product = new MProduct(imp);
                        product.SetC_TaxCategory_ID(C_TaxCategory_ID);
                        if (product.Save())
                        {
                            M_Product_ID = product.GetM_Product_ID();
                            log.Finer("Insert Product");
                            noInsert++;
                        }
                        else
                        {
                            StringBuilder sql0 = new StringBuilder("UPDATE I_Product i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Insert Product failed"))
                                .Append("WHERE I_Product_ID=").Append(I_Product_ID);
                            DataBase.DB.ExecuteQuery(sql0.ToString(), null, Get_TrxName());
                            continue;
                        }
                    }
                    else					//	Update Product
                    {
                        String sqlt = "UPDATE M_PRODUCT "
                            + "SET (Value,Name,Description,DocumentNote,Help,"
                            + "UPC,SKU,C_UOM_ID,M_Product_Category_ID,Classification,ProductType,"
                            + "Volume,Weight,ShelfWidth,ShelfHeight,ShelfDepth,UnitsPerPallet,"
                            + "Discontinued,DiscontinuedBy,Updated,UpdatedBy)= "
                            + "(SELECT Value,Name,Description,DocumentNote,Help,"
                            + "UPC,SKU,C_UOM_ID,M_Product_Category_ID,Classification,ProductType,"
                            + "Volume,Weight,ShelfWidth,ShelfHeight,ShelfDepth,UnitsPerPallet,"
                            + "Discontinued,DiscontinuedBy,SysDate,UpdatedBy"
                            + " FROM I_Product WHERE I_Product_ID=" + I_Product_ID + ") "
                            + "WHERE M_Product_ID=" + M_Product_ID;
                        //PreparedStatement pstmt_updateProduct = DataBase.prepareStatement
                        //(sqlt, Get_TrxName());

                        //jz pstmt_updateProduct.setInt(1, I_Product_ID);
                        //   pstmt_updateProduct.setInt(2, M_Product_ID);
                        try
                        {
                            //no = pstmt_updateProduct.ExecuteQuery();
                            no = DataBase.DB.ExecuteQuery(sqlt, null, Get_TrxName());
                            log.Finer("Update Product = " + no);
                            noUpdate++;
                        }
                        catch (Exception ex)
                        {
                            log.Warning("Update Product - " + ex.ToString());
                            StringBuilder sql0 = new StringBuilder("UPDATE I_Product i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Update Product: " + ex.ToString()))
                                .Append("WHERE I_Product_ID=").Append(I_Product_ID);
                            DataBase.DB.ExecuteQuery(sql0.ToString(), null, Get_TrxName());
                            continue;
                        }
                        //pstmt_updateProduct.close();

                    }

                    //	Do we have PO Info
                    if (C_BPartner_ID != 0)
                    {
                        no = 0;
                        //	If Product existed, Try to Update first
                        if (!newProduct)
                        {
                            String sqlt = "UPDATE M_Product_PO "
                                + "SET (IsCurrentVendor,C_UOM_ID,C_Currency_ID,UPC,"
                                + "PriceList,PricePO,RoyaltyAmt,PriceEffective,"
                                + "VendorProductNo,VendorCategory,Manufacturer,"
                                + "Discontinued,DiscontinuedBy,Order_Min,Order_Pack,"
                                + "CostPerOrder,DeliveryTime_Promised,Updated,UpdatedBy)= "
                                + "(SELECT CAST('Y' AS CHAR),C_UOM_ID,C_Currency_ID,UPC,"    //jz fix EDB unknown datatype error
                                + "PriceList,PricePO,RoyaltyAmt,PriceEffective,"
                                + "VendorProductNo,VendorCategory,Manufacturer,"
                                + "Discontinued,DiscontinuedBy,Order_Min,Order_Pack,"
                                + "CostPerOrder,DeliveryTime_Promised,SysDate,UpdatedBy"
                                + " FROM I_Product"
                                + " WHERE I_Product_ID=" + I_Product_ID + ") "
                                + "WHERE M_Product_ID=" + M_Product_ID + " AND C_BPartner_ID=" + C_BPartner_ID;
                            //PreparedStatement pstmt_updateProductPO = DataBase.prepareStatement
                            //(sqlt, Get_TrxName());
                            //jz pstmt_updateProductPO.setInt(1, I_Product_ID);
                            // pstmt_updateProductPO.setInt(2, M_Product_ID);
                            // pstmt_updateProductPO.setInt(3, C_BPartner_ID);
                            try
                            {
                                //no = pstmt_updateProductPO.ExecuteQuery();
                                no = DataBase.DB.ExecuteQuery(sqlt, null, Get_TrxName());
                                log.Finer("Update Product_PO = " + no);
                                noUpdatePO++;
                            }
                            catch (Exception ex)
                            {
                                log.Warning("Update Product_PO - " + ex.ToString());
                                noUpdate--;
                                Rollback();
                                StringBuilder sql0 = new StringBuilder("UPDATE I_Product i "
                                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Update Product_PO: " + ex.ToString()))
                                    .Append("WHERE I_Product_ID=").Append(I_Product_ID);
                                DataBase.DB.ExecuteQuery(sql0.ToString(), null, Get_TrxName());
                                continue;
                            }
                            //pstmt_updateProductPO.close();
                        }
                        if (no == 0)		//	Insert PO
                        {
                            SqlParameter[] param = new SqlParameter[3];
                            param[0] = new SqlParameter("@param1", M_Product_ID);
                            //pstmt_insertProductPO.setInt(1, M_Product_ID);
                            param[1] = new SqlParameter("@param1", C_BPartner_ID);
                            //pstmt_insertProductPO.setInt(2, C_BPartner_ID);
                            param[2] = new SqlParameter("@param1", I_Product_ID);
                            //pstmt_insertProductPO.setInt(3, I_Product_ID);

                            try
                            {
                                //no = pstmt_insertProductPO.ExecuteQuery();
                                no = DataBase.DB.ExecuteQuery(_insertProductPO, param, Get_TrxName());
                                log.Finer("Insert Product_PO = " + no);
                                noInsertPO++;
                            }
                            catch (Exception ex)
                            {
                                log.Warning("Insert Product_PO - " + ex.ToString());
                                noInsert--;			//	assume that product also did not exist
                                Rollback();
                                StringBuilder sql0 = new StringBuilder("UPDATE I_Product i "
                                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Insert Product_PO: " + ex.ToString()))
                                    .Append("WHERE I_Product_ID=").Append(I_Product_ID);
                                DataBase.DB.ExecuteQuery(sql0.ToString(), null, Get_TrxName());
                                continue;
                            }
                        }
                    }	//	C_BPartner_ID != 0

                    //	Price List
                    if (_M_PriceList_Version_ID != 0)
                    {
                        Decimal PriceList = imp.GetPriceList();
                        Decimal PriceStd = imp.GetPriceStd();
                        Decimal PriceLimit = imp.GetPriceLimit();
                        if (Env.Signum(PriceStd) != 0 && Env.Signum(PriceLimit) != 0 && Env.Signum(PriceList) != 0)
                        {
                            MProductPrice pp = MProductPrice.Get(GetCtx(),
                                _M_PriceList_Version_ID, M_Product_ID, Get_TrxName());
                            if (pp == null)
                                pp = new MProductPrice(GetCtx(),
                                    _M_PriceList_Version_ID, M_Product_ID, Get_TrxName());
                            pp.SetPrices(PriceList, PriceStd, PriceLimit);
                            pp.Save();
                        }
                    }

                    //	Update I_Product
                    //pstmt_setImported.setInt(1, M_Product_ID);
                    //pstmt_setImported.setInt(2, I_Product_ID);
                    SqlParameter[] param1 = new SqlParameter[2];
                    param1[0] = new SqlParameter("@param1", M_Product_ID);
                    param1[1] = new SqlParameter("@param2", I_Product_ID);
                    no = DataBase.DB.ExecuteQuery(_setImported, param1, Get_TrxName());
                    //
                    Commit();
                }	//	for all I_Product
                idr.Close();
                //
                //	pstmt_insertProduct.close();
                // pstmt_updateProduct.close();
                //pstmt_insertProductPO.close();
                // pstmt_updateProductPO.close();
                //pstmt_setImported.close();
                //
            }
            catch 
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_Product "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@M_Product_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdate), "@M_Product_ID@: @Updated@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertPO), "@M_Product_ID@ @Purchase@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdatePO), "@M_Product_ID@ @Purchase@: @Updated@");
            return "";
        }	//	doIt

    }
}

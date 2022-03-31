using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Classes
{
    public class QueryCollection
    {
        static dynamic queryList = new ExpandoObject();
        static IDictionary<string, object> IqueryList = (IDictionary<string, object>)queryList;
        static bool isLoad = false;

        private static void AddQueries(Ctx ctx)
        {
            queryList.VIS_1 = "SELECT dc.DocSubTypeSO FROM C_DocType dc INNER JOIN C_DocBaseType db on (dc.DocBaseType = db.DocBaseType)"
                            + " WHERE C_DocType_ID = @Param AND db.DocBaseType = 'SOO' AND dc.DocSubTypeSO IN ('WR','WI')";

            queryList.VIS_2 = "SELECT PayAmt FROM C_Payment_v WHERE C_Payment_ID=@C_Payment_ID";

            queryList.VIS_3 = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID =@param1 "
                                         + " AND M_PriceList_Version_ID =@param2 "
                                         + " AND  M_AttributeSetInstance_ID =@param3 "
                                         + "  AND C_UOM_ID=@param 4";

            queryList.VIS_4 = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                       + " AND M_PriceList_Version_ID = @param2 "
                                       + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                       + "  AND C_UOM_ID= @param3";

            queryList.VIS_5 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                        + " AND M_PriceList_Version_ID = @param2 "
                                        + " AND  M_AttributeSetInstance_ID = @param3 "
                                        + "  AND C_UOM_ID= @param4 ";

            queryList.VIS_6 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID =@param1 "
                                      + " AND M_PriceList_Version_ID =@param2 "
                                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                      + "  AND C_UOM_ID= @param3 ";

            queryList.VIS_7 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID =@param1 "
                                  + " AND M_PriceList_Version_ID = @param2 "
                                  + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                  + "  AND C_UOM_ID= @param3 ";

            queryList.VIS_8 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                        + " AND M_PriceList_Version_ID = @param2 "
                                        + " AND  M_AttributeSetInstance_ID = @param3 "
                                        + "  AND C_UOM_ID= @param4 ";

            queryList.VIS_9 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID =  @param1 "
                                      + " AND M_PriceList_Version_ID = @param2 "
                                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                      + "  AND C_UOM_ID= @param3 ";
            queryList.VIS_10 = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' "
                                      + " AND con.M_Product_ID = @param1  AND con.C_UOM_ID = @param2  AND con.C_UOM_To_ID = @param3 ";

            queryList.VIS_11 = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                  " AND con.C_UOM_ID = @param1  AND con.C_UOM_To_ID = @param2 ";

            queryList.VIS_12 = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                             + " AND M_PriceList_Version_ID = @param2 "
                                             + " AND  M_AttributeSetInstance_ID = @param3 "
                                             + "  AND C_UOM_ID= @param4 ";

            queryList.VIS_13 = "SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                           + " AND M_PriceList_Version_ID =  @param2 "
                                           + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                           + "  AND C_UOM_ID= @param3 ";

            queryList.VIS_14 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                            + " AND M_PriceList_Version_ID =  @param2 "
                                            + " AND  M_AttributeSetInstance_ID = @param3 "
                                            + "  AND C_UOM_ID= @param4 ";

            queryList.VIS_15 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                          + " AND M_PriceList_Version_ID = @param2 "
                                          + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                          + "  AND C_UOM_ID= @param3 ";
            queryList.VIS_16 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                      + " AND M_PriceList_Version_ID = @param2 "
                                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                      + "  AND C_UOM_ID= @param3 ";
            queryList.VIS_17 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                            + " AND M_PriceList_Version_ID =  @param2 "
                                            + " AND  M_AttributeSetInstance_ID = @param3 "
                                            + "  AND C_UOM_ID= @param4 ";
            queryList.VIS_18 = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                          + " AND M_PriceList_Version_ID = @param2 "
                                          + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                          + "  AND C_UOM_ID= @param3 ";
            queryList.VIS_19 = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' "
                                          + " AND con.M_Product_ID = @param1  AND con.C_UOM_ID =  @param2 AND con.C_UOM_To_ID = @param3 ";

            queryList.VIS_20 = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                      " AND con.C_UOM_ID = @param1 AND con.C_UOM_To_ID = @param2 ";

            queryList.VIS_21 = "SELECT EnforcePriceLimit FROM M_PriceList WHERE IsActive = 'Y' AND M_PriceList_ID = @param1 ";

            queryList.VIS_22 = "SELECT OverwritePriceLimit FROM AD_Role WHERE IsActive = 'Y' AND AD_Role_ID = @param1 ";

            queryList.VIS_23 = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                        + " AND M_PriceList_Version_ID = @param2 "
                                        + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                        + "  AND C_UOM_ID= @param3 ";

            queryList.VIS_24 = "SELECT PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                       + " AND M_PriceList_Version_ID = @param2 "
                                       + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID= @param3 ";

            queryList.VIS_25 = "SELECT PriceStd FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = @param1 "
                                     + " AND M_PriceList_Version_ID = @param2 "
                                     + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID= @param3 ";
            queryList.VIS_26 = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' "
                                     + " AND con.M_Product_ID = @param1 "
                           + " AND con.C_UOM_ID = @param2  AND con.C_UOM_To_ID = @param3 ";

            queryList.VIS_27 = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                      " AND con.C_UOM_ID = @param1  AND con.C_UOM_To_ID = @param2 ";

            queryList.VIS_28 = "SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive = 'Y' AND  C_BPartner_ID = @param1 "
                                     + " AND M_Product_ID = @param2 ";

            queryList.VIS_29 = "SELECT C_UOM_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = @param1 ";

            queryList.VIS_30 = "Select NoOfMonths from C_Frequency where C_Frequency_ID=@param1 ";

            queryList.VIS_31 = "select rate from c_tax WHERE c_tax_id= @param1 ";

            queryList.VIS_32 = "SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID = (SELECT C_Order_ID FROM C_Order "
                             + "WHERE DocumentNo = (SELECT DocumentNo FROM M_Requisition WHERE M_Requisition.M_Requisition_id = @Param1) "
                             + "AND AD_Client_ID = @Param2) AND M_Product_ID = @Param3";

            queryList.VIS_33 = "SELECT C_Currency_ID FROM M_PriceList where M_PriceList_ID = @Param";

            queryList.VIS_34 = "SELECT COALESCE(MAX(C_InvoiceBatchLine_ID),0) FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID = @Param";

            queryList.VIS_35 = "SELECT p.M_Product_ID, ra.Name, ra.Description, ra.Qty FROM S_ResourceAssignment ra"
                             + " INNER JOIN M_Product p ON (p.S_Resource_ID=ra.S_Resource_ID) WHERE ra.S_ResourceAssignment_ID = @Param";
            queryList.VIS_36 = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"
                             + "bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"
                             + "bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"
                             + "p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                             + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList_Version pv "
                             + "WHERE p.M_Product_ID=pp.M_Product_ID"
                             + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                             + " AND pv.M_PriceList_ID=pl.M_PriceList_ID"
                             + " AND pv.IsActive='Y' AND p.M_Product_ID=@param1"
                             + " AND pl.M_PriceList_ID=@param2 ORDER BY pv.ValidFrom DESC";

            queryList.VIS_37 = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"
                             + "bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"
                             + "bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"
                             + "p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                             + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList bpl, M_PriceList_Version pv "
                             + "WHERE p.M_Product_ID=pp.M_Product_ID"
                             + " AND pp.M_PriceList_Version_ID = pv.M_PriceList_Version_ID"
                             + " AND pv.M_PriceList_ID = bpl.M_PriceList_ID"
                             + " AND pv.IsActive = 'Y' AND bpl.M_PriceList_ID = pl.BasePriceList_ID"	//	Base
                             + " AND p.M_Product_ID = @param1 AND pl.M_PriceList_ID = @param2"	//	2
                             + " ORDER BY pv.ValidFrom DESC";

            queryList.VIS_38 = "SELECT C_Period_ID FROM C_Period WHERE C_Year_ID IN "
                             + " (SELECT C_Year_ID FROM C_Year WHERE C_Calendar_ID = "
                             + " (SELECT C_Calendar_ID FROM AD_ClientInfo WHERE AD_Client_ID=@param1))"
                             + " AND @param2 BETWEEN StartDate AND EndDate AND PeriodType='S'";

            queryList.VIS_39 = "SELECT PeriodType, StartDate, EndDate FROM C_Period WHERE C_Period_ID=@param";

            queryList.VIS_40 = "SELECT ProfitBeforeTax,C_Year_ID,C_ProfitAndLoss_ID FROM C_ProfitLoss WHERE C_ProfitLoss_ID=@Param";

            queryList.VIS_41 = "Select M_PriceList_Version_ID from M_ProductPrice where M_Product_id=@param1"
                             + " and M_PriceList_Version_ID in (select m_pricelist_version_id from m_pricelist_version"
                             + " where m_pricelist_id = @param2 and isactive='Y')";

            queryList.VIS_42 = "Select PriceStd from M_ProductPrice where M_PriceList_Version_ID=@param1 and M_Product_id=@param2";

            queryList.VIS_43 = "SELECT loc.M_Warehouse_ID FROM M_Product p INNER JOIN M_Locator loc ON p.M_Locator_ID= loc.M_Locator_ID WHERE p.M_Product_ID=@Param";

            queryList.VIS_44 = "SELECT Count(*) FROM M_Manufacturer WHERE IsActive = 'Y' AND UPC = '@Param'";

            queryList.VIS_45 = "UPDATE RC_ViewColumn SET IsGroupBy='N' WHERE RC_View_ID=@Param1 AND RC_ViewColumn_ID NOT IN(@Param2)";

            queryList.VIS_46 = "SELECT AD_Table_ID FROM AD_Table WHERE IsActive='Y' AND TableName= 'VADMS_MetaData'";

            queryList.VIS_47 = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID=@Param";

            queryList.VIS_48 = "SELECT DISTINCT M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID=@Param";

            queryList.VIS_49 = "SELECT  DiscountType FROM M_DiscountSchema WHERE M_DiscountSchema_ID=@Param1 AND IsActive='Y' AND AD_Client_ID=@Param2";

            queryList.VIS_50 = "SELECT M_Product_Category_ID, M_Product_ID, BreakValue, IsBPartnerFlatDiscount, BreakDiscount FROM M_DiscountSchemaBreak WHERE "
                             + "M_DiscountSchema_ID = @Param1 AND M_Product_ID = @Param2 AND IsActive='Y' AND AD_Client_ID= @Param3 Order BY BreakValue DESC";

            queryList.VIS_51 = "SELECT M_Product_Category_ID, M_Product_ID, BreakValue, IsBPartnerFlatDiscount, BreakDiscount FROM M_DiscountSchemaBreak WHERE "
                             + " M_DiscountSchema_ID = @Param1 AND M_Product_Category_ID = @Param2 AND IsActive='Y' AND AD_Client_ID = @Param3 Order BY BreakValue DESC";

            queryList.VIS_52 = "SELECT M_Product_Category_ID, M_Product_ID, BreakValue, IsBPartnerFlatDiscount, BreakDiscount FROM M_DiscountSchemaBreak WHERE "
                             + " M_DiscountSchema_ID = @Param1 AND M_Product_Category_ID IS NULL AND M_Product_ID IS NULL AND IsActive='Y' AND AD_Client_ID = @Param2 Order BY BreakValue DESC";

            queryList.VIS_53 = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID = @Param1 AND IsActive ='Y' AND AD_Client_ID = @Param2";

            queryList.VIS_54 = "SELECT Count(*) FROM AD_Column WHERE ColumnName = 'C_Tax_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName = 'C_TaxCategory')";

            queryList.VIS_55 = "SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID = @Param1 AND IsActive = 'Y' AND C_BPartner_Location_ID = @Param2";

            queryList.VIS_56 = "SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID = @Param1 AND IsActive = 'Y'";

            queryList.VIS_57 = "SELECT C_Tax_ID FROM VATAX_TaxCatRate WHERE C_TaxCategory_ID = @Param1 AND IsActive ='Y' and VATAX_TaxType_ID =@Param2";

            queryList.VIS_58 = "SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID=@Param";

            queryList.VIS_59 = "select FO_RES_ACCOMODATION_ID,DATE_FROM,TILL_DATE from FO_RES_ACCOMODATION where FO_OBJECT_DATA_ID=@FO_OBJ_DATA_ID";

            queryList.VIS_60 = "SELECT DAYSPAYABLE2 FROM FO_SETTINGS WHERE AD_ORG_ID=@Param";

            queryList.VIS_61 = "SELECT max(OFFERNO)FROM FO_OFFER";

            queryList.VIS_62 = "SELECT FO_PRICE_LIST_ID FROM FO_ADDRESS_PRICE WHERE FO_ADDRESS_ID=@Param";

            queryList.VIS_63 = "SELECT NoOfDays FROM C_Frequency WHERE C_Frequency_ID=@Param";

            queryList.VIS_64 = "SELECT ProfileType FROM S_Resource WHERE AD_User_ID=@Param";

            queryList.VIS_65 = "SELECT M_Product_ID, MovementQty, M_AttributeSetInstance_ID FROM M_InOutLine WHERE M_InOutLine_ID=@Param";

            queryList.VIS_66 = "SELECT SUM(ConfirmedQty + ScrappedQty) FROM M_PackageLine WHERE M_Package_ID=@Param1 AND M_InOutLine_ID=@Param2";

            queryList.VIS_67 = "SELECT EndingBalance FROM C_Cash WHERE C_CashBook_ID=@Param1 AND AD_Client_ID=@Param2 AND AD_Org_ID=@Param3 AND " +
                            "C_Cash_ID IN (SELECT Max(C_Cash_ID) FROM C_Cash WHERE C_CashBook_ID=@Param4 AND AD_Client_ID=@Param5 AND AD_Org_ID=@Param6) AND Processed='Y'";

            queryList.VIS_68 = "SELET Count(*) FROM M_Transaction WHERE M_Product_ID = @Param";

            queryList.VIS_69 = "SELET uom.FO_HOTEL_UOM_ID,s.DESCRIPTION FROM FO_HOTEL_UOM uom INNER JOIN FO_SERVICE s ON(uom.FO_HOTEL_UOM_ID = s.FO_HOTEL_UOM_ID) "
                             + "WHERE s.FO_SERVICE_ID=@FO_SERVICE_ID ";

            queryList.VIS_70 = "SELET PRICE,CHILDGROUP1,CHILDGROUP2,UOM1,UOM2 FROM FO_SERVICE_PRICE_PRICELINE WHERE CREATED=(SELECT max(CREATED) FROM FO_SERVICE_PRICE_PRICELINE "
                             + "WHERE FO_SERVICE_ID=@FO_SERVICE_ID)";

            queryList.VIS_71 = "SELECT SUM(t.CurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty FROM m_transaction t" +
                            " INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= @Param1 AND l.AD_Org_ID = @Param2" +
                            " AND t.M_Locator_ID = @Param3 AND t.M_Product_ID = @Param4 AND NVL(t.M_AttributeSetInstance_ID,0) = @Param5";


            queryList.VIS_72 = "SELECT COUNT(*) FROM AD_Table WHERE TableName='R_Request'";

            queryList.VIS_73 = "SELECT Name, PO_Name FROM AD_Element WHERE UPPER(ColumnName)=@ColumnName";
            queryList.VIS_74 = "SELECT t.Name, t.PO_Name FROM AD_Element_Trl t, AD_Element e "
                + "WHERE t.AD_Element_ID=e.AD_Element_ID AND UPPER(e.ColumnName)=@ColumnName "
                + "AND t.AD_Language=@AD_Language";

            queryList.VIS_75 = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=@tableID";

            queryList.VIS_76 = "UPDATE AD_PrintFormat SET IsDefault='N' WHERE IsDefault='Y' AND AD_Table_ID=@tableID AND AD_Tab_ID=@tabID";

            queryList.VIS_77 = "UPDATE AD_PrintFormat SET IsDefault='Y' WHERE AD_PrintFormat_ID=@printForamt";

            queryList.VIS_78 = "SELECT DISTINCT AD_Window_ID, PO_Window_ID FROM AD_Table t WHERE TableName = @targetTableName";

            queryList.VIS_79 = " SELECT p.IsSOTrx FROM @ParentTable p, @targetTableName c  WHERE @targetWhereClause AND p.@ParentTable1_ID = c.@ParentTable2_ID";


            queryList.VIS_80 = "SELECT AD_Window_ID, Name FROM AD_Window WHERE Name LIKE 'Work Center%' OR NAME LIKE 'Production Resource'";

            queryList.VIS_81 = "SELECT ISREPORT FROM AD_Process WHERE AD_Process_ID=@AD_Process_ID";


            queryList.VIS_82 = "SELECT Value, Name FROM AD_Ref_List WHERE AD_Reference_ID=@AD_Reference_ID AND IsActive='Y'";


            queryList.VIS_83 = "SELECT kc.ColumnName"
                            + " FROM AD_Ref_Table rt"
                            + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                            + "WHERE rt.AD_Reference_ID=@AD_Reference_ID";

            queryList.VIS_84 = "SELECT Columnname, tbl.TableName FROM AD_Column clm INNER JOIN AD_Table tbl ON (tbl.AD_Table_ID = clm.AD_Table_ID) WHERE AD_Column_ID = "
                      + " (SELECT Column_Key_ID FROM AD_Ref_Table WHERE AD_Reference_ID = @refid)";

            queryList.VIS_85 = "SELECT  tbl.tablename, clm.Columnname FROM ( "
                        + " SELECT kc.ColumnName, dc.ColumnName as ColName1, t.TableName,  t.AD_Table_ID "
                        + " FROM AD_Ref_Table rt"
                        + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                        + " INNER JOIN AD_Column dc ON (rt.Column_Display_ID=dc.AD_Column_ID)"
                        + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID) "
                        + "WHERE rt.AD_Reference_ID=@refid ) rr "
                    + " INNER JOIN AD_Table tbl "
                    + " ON (tbl.AD_Table_ID = rr.AD_Table_ID) "
                    + " INNER JOIN AD_Column clm "
                    + " ON (clm.AD_Table_ID      = tbl.AD_Table_ID) "
                    + " WHERE (clm.ColumnName   IN ('DocumentNo', 'Value', 'Name') "
                    + " OR clm.IsIdentifier      ='Y') "
                    + " AND clm.AD_Reference_ID IN (10,14) "
                    + " AND EXISTS "
                      + " (SELECT *  FROM AD_Column cc WHERE cc.AD_Table_ID=tbl.AD_Table_ID  AND cc.IsKey ='Y' AND cc.ColumnName = @colname)";


            queryList.VIS_86 = "SELECT t.TableName, c.ColumnName "
                + "FROM AD_Column c "
                + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID AND t.IsView='N') "
                + "WHERE (c.ColumnName IN ('DocumentNo', 'Value', 'Name') OR c.IsIdentifier='Y')"
                + " AND c.AD_Reference_ID IN (10,14)"
                + " AND EXISTS (SELECT * FROM AD_Column cc WHERE cc.AD_Table_ID=t.AD_Table_ID"
                    + " AND cc.IsKey='Y' AND cc.ColumnName=@colname)";

            queryList.VIS_87 = "SELECT kc.ColumnName, dc.ColumnName, t.TableName "
                        + "FROM AD_Ref_Table rt"
                        + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                        + " INNER JOIN AD_Column dc ON (rt.Column_Display_ID=dc.AD_Column_ID)"
                        + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID) "
                        + "WHERE rt.AD_Reference_ID=@AD_Reference_ID";
            queryList.VIS_88 = "SELECT AD_Window_ID FROM AD_Tab WHERE AD_Tab_ID =@AD_Tab_ID";

            queryList.VIS_89 = "SELECT AD_InfoWindow_ID FROM AD_InfoWindow WHERE AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName=@tableName) AND IsActive='Y'"
                    + " ORDER BY ISCUSTOMDEFAULT DESC , AD_InfoWindow_ID ASC ";


            queryList.VIS_90 = "SELECT kc.ColumnName"
                            + " FROM AD_Ref_Table rt"
                            + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                            + "WHERE rt.AD_Reference_ID=@AD_Reference_ID";
            queryList.VIS_91 = "SELECT kc.ColumnName"
                            + " FROM AD_Ref_Table rt"
                            + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                            + "WHERE rt.AD_Reference_ID=@AD_Reference_ID";

            queryList.VIS_92 = "SELECT Value FROM M_Locator WHERE IsActive='Y' and M_Locator_ID=@keyValue";

            queryList.VIS_93 = "SELECT ColumnName FROM AD_Column WHERE AD_Table_ID = 207 AND IsIdentifier  = 'Y' AND SeqNo  IS NOT NULL ORDER BY SeqNo";

            queryList.VIS_94 = "SELECT C_ValidCombination_ID, Combination, Description FROM C_ValidCombination WHERE C_ValidCombination_ID=@ID";

            queryList.VIS_95 = "SELECT Description FROM M_AttributeSetInstance WHERE M_AttributeSetInstance_ID=@M_AttributeSetInstance_ID";

            queryList.VIS_96 = "SELECT Description FROM C_GenattributeSetInstance WHERE C_GenattributeSetInstance_ID=@C_GenttributeSetInstance_ID";

            queryList.VIS_97 = "SELECT c.ColumnName, c.AD_Reference_Value_ID, c.IsParent, vr.Code FROM AD_Column c LEFT OUTER JOIN AD_Val_Rule vr ON (c.AD_Val_Rule_ID=vr.AD_Val_Rule_ID) WHERE c.AD_Column_ID=@Column_ID";

            queryList.VIS_98 = "SELECT c.ColumnName,c.IsTranslated,c.AD_Reference_ID,c.AD_Reference_Value_ID "
              + "FROM AD_Table t INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
              + "WHERE TableName=@tableName"
              + " AND c.IsIdentifier='Y' "
              + "ORDER BY c.SeqNo";

            queryList.VIS_99 = "SELECT Amount "
                    + "FROM C_DimAmt "
                    + "WHERE C_DimAmt_ID=@C_DimAmt_ID";

            queryList.VIS_100 = "SELECT AD_Process_ID,name,CLASSNAME,ENTITYTYPE FROM AD_Process WHERE value=@processName AND ISACTIVE='Y'";

            queryList.VIS_101 = "SELECT count(*) FROM AD_Table t "
        + "INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
        + "WHERE t.TableName=@TableName AND c.ColumnName='C_BPartner_ID' ";

            queryList.VIS_102 = "UPDATE AD_PrintFormat SET IsDefault='N' WHERE IsDefault='Y' AND AD_Table_ID=@Ad_Table_ID AND AD_Tab_ID=@AD_Tab_ID";

            queryList.VIS_103 = "UPDATE AD_PrintFormat SET IsDefault='Y' WHERE AD_PrintFormat_ID=@id";

            queryList.VIS_104 = "SELECT cc.ColumnName "
            + "FROM AD_Column c"
            + " INNER JOIN AD_Ref_Table r ON (c.AD_Reference_Value_ID=r.AD_Reference_ID)"
            + " INNER JOIN AD_Column cc ON (r.Column_Key_ID=cc.AD_Column_ID) "
            + "WHERE c.AD_Reference_ID IN (18,30)" 	//	Table/Search
            + " AND c.ColumnName=@colName";

            queryList.VIS_105 = "SELECT t.TableName "
            + "FROM AD_Column c"
            + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID) "
            + "WHERE c.ColumnName=@colName AND IsKey='Y'"		//	#1 Link Column
            + " AND EXISTS (SELECT * FROM AD_Column cc"
            + " WHERE cc.AD_Table_ID=t.AD_Table_ID AND cc.ColumnName=@tabKeyColumn)";	//	#2 Tab Key Column

            queryList.VIS_106 = "SELECT AD_Reference_ID FROM AD_Column WHERE ColumnName=@colName";	//	#2 Tab Key Column

            queryList.VIS_107 = "SELECT  AD_UserQuery.Code,ad_defaultuserquery.ad_user_id,ad_defaultuserquery.ad_tab_id FROM AD_UserQuery AD_UserQuery JOIN AD_DefaultUserQuery ad_defaultuserquery ON AD_UserQuery.AD_UserQuery_ID=ad_defaultuserquery.AD_UserQuery_ID WHERE AD_UserQuery.IsActive                 ='Y'" +
             "AND ad_defaultuserquery.AD_User_ID=@AD_User_ID AND AD_UserQuery.AD_Client_ID =@AD_Client_ID AND (AD_UserQuery.AD_Tab_ID =@AD_Tab_ID OR AD_UserQuery.AD_Table_ID                 = @AD_Table_ID)";

            queryList.VIS_108 = "SELECT Record_ID "
            + "FROM AD_Private_Access "
            + "WHERE AD_User_ID=@AD_User_ID AND AD_Table_ID=@AD_Table_ID AND IsActive='Y' "
            + "ORDER BY Record_ID";

            queryList.VIS_109 = "SELECT CM_Subscribe_ID, Record_ID FROM CM_Subscribe WHERE AD_User_ID=@AD_User_ID AND AD_Table_ID=@AD_Table_ID";

            queryList.VIS_110 = "SELECT vadms_windowdoclink_id,record_id FROM vadms_windowdoclink wdl JOIN vadms_document doc "
                 + " ON wdl.VADMS_Document_ID  =doc.VADMS_Document_ID  WHERE doc.vadms_docstatus!='DD' AND ad_table_id=@ad_table_id";

            queryList.VIS_111 = "SELECT CM_Chat_ID, Record_ID FROM CM_Chat WHERE AD_Table_ID=@AD_Table_ID";

            queryList.VIS_112 = "SELECT distinct att.AD_Attachment_ID, att.Record_ID FROM AD_Attachment att"
               + " INNER JOIN AD_Attachmentline al ON (al.AD_Attachment_id=att.AD_Attachment_id)"
               + " WHERE att.AD_Table_ID=@AD_Table_ID";

            queryList.VIS_113 = "SELECT AD_ExportData_ID, Record_ID FROM AD_ExportData WHERE AD_Table_ID=@AD_Table_ID";

            queryList.VIS_114 = "SELECT  CASE WHEN length(AD_Userquery.Name)>25 THEN substr(AD_Userquery.name ,0,25)||'...' ELSE AD_Userquery.Name END AS Name,AD_Userquery.Name as title, AD_UserQuery.Code, AD_UserQuery.AD_UserQuery_ID, AD_UserQuery.AD_User_ID, AD_UserQuery.AD_Tab_ID, "
+ " case  WHEN AD_UserQuery.AD_UserQuery_ID IN (Select AD_UserQuery_ID FROM AD_DefaultUserQuery WHERE AD_DefaultUserQuery.AD_Tab_ID=@AD_Tab_ID AND AD_DefaultUserQuery.AD_User_ID=@AD_User_ID  )  "
+ "then (Select AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery  WHERE AD_DefaultUserQuery.AD_Tab_ID=@AD_Tab_ID1 AND AD_DefaultUserQuery.AD_User_ID=@AD_User_ID1 )   ELSE null End as AD_DefaultUserQuery_ID"
       + " FROM AD_UserQuery AD_UserQuery WHERE AD_UserQuery.AD_Client_ID       =@AD_Client_ID AND AD_UserQuery.IsActive             ='Y' "
       + " AND AD_UserQuery.AD_Tab_ID           =@AD_Tab_ID2 AND AD_UserQuery.AD_Table_ID           =@AD_Table_ID"
       + " AND lower(AD_UserQuery.Name) like lower('%'||@queryData||'%')"
       + " ORDER BY Upper(AD_UserQuery.NAME), AD_UserQuery.AD_UserQuery_ID";


            queryList.VIS_115 = "SELECT count(*) "
            + " FROM AD_UserQuery AD_UserQuery LEFT OUTER JOIN AD_DefaultUserQuery AD_DefaultUserQuery ON AD_DefaultUserQuery.AD_UserQuery_ID=AD_UserQuery.AD_UserQuery_ID WHERE"
                               + " AD_UserQuery.AD_Client_ID=@AD_Client_ID AND AD_UserQuery.IsActive='Y'"
                               + " AND (AD_UserQuery.AD_Tab_ID=@AD_Tab_ID AND AD_UserQuery.AD_Table_ID=@AD_Table_ID)"
                               + " ORDER BY AD_UserQuery.AD_UserQuery_ID";



            queryList.VIS_116 = "SELECT  CASE WHEN length(AD_Userquery.Name)>25 THEN substr(AD_Userquery.name ,0,25)||'...' ELSE AD_Userquery.Name END AS Name,AD_Userquery.Name as title, AD_UserQuery.Code, AD_UserQuery.AD_UserQuery_ID, AD_UserQuery.AD_User_ID, AD_UserQuery.AD_Tab_ID, "
+ " case  WHEN AD_UserQuery.AD_UserQuery_ID IN (Select AD_UserQuery_ID FROM AD_DefaultUserQuery WHERE AD_DefaultUserQuery.AD_Tab_ID=@AD_Tab_ID AND AD_DefaultUserQuery.AD_User_ID=@AD_User_ID  )  "
+ "then (Select AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery  WHERE AD_DefaultUserQuery.AD_Tab_ID=@AD_Tab_ID1 AND AD_DefaultUserQuery.AD_User_ID=@AD_User_ID1 )   ELSE null End as AD_DefaultUserQuery_ID"
       + " FROM AD_UserQuery AD_UserQuery WHERE AD_UserQuery.AD_Client_ID       =@AD_Client_ID AND AD_UserQuery.IsActive             ='Y' "
       + " AND AD_UserQuery.AD_Tab_ID           =@AD_Tab_ID2 AND AD_UserQuery.AD_Table_ID           =@AD_Table_ID"
       + " ORDER BY Upper(AD_UserQuery.NAME), AD_UserQuery.AD_UserQuery_ID";


            queryList.VIS_117 = "SELECT  CASE WHEN length(AD_Userquery.Name)>25 THEN substr(AD_Userquery.name ,0,25)||'...' ELSE AD_Userquery.Name END AS Name,AD_Userquery.Name as title, AD_UserQuery.Code, AD_UserQuery.AD_UserQuery_ID, AD_UserQuery.AD_User_ID, AD_UserQuery.AD_Tab_ID, "
+ " case  WHEN AD_UserQuery.AD_UserQuery_ID IN (Select AD_UserQuery_ID FROM AD_DefaultUserQuery WHERE AD_DefaultUserQuery.AD_Tab_ID=@AD_Tab_ID AND AD_DefaultUserQuery.AD_User_ID=@AD_User_ID  )  "
+ "then (Select AD_DefaultUserQuery_ID FROM AD_DefaultUserQuery  WHERE AD_DefaultUserQuery.AD_Tab_ID=@AD_Tab_ID1 AND AD_DefaultUserQuery.AD_User_ID=@AD_User_ID1 )   ELSE null End as AD_DefaultUserQuery_ID"
       + " FROM AD_UserQuery AD_UserQuery WHERE AD_UserQuery.AD_Client_ID       =@AD_Client_ID AND AD_UserQuery.IsActive             ='Y' "
       + " AND AD_UserQuery.AD_Tab_ID           =@AD_Tab_ID2 AND AD_UserQuery.AD_Table_ID           =@AD_Table_ID"
       + " ORDER BY Upper(AD_UserQuery.NAME), AD_UserQuery.AD_UserQuery_ID";


            queryList.VIS_118 = "select ad_reportformat_id FROM AD_Process WHERE AD_Process_ID=@AD_Process_ID";

            queryList.VIS_119 = "select versionno FROM AD_ModuleINfo where Prefix='VAREPH_'";

            queryList.VIS_120 = "SELECT AD_Tree_ID FROM AD_Tree "
            + "WHERE AD_Client_ID=@AD_Client_ID AND AD_Table_ID=@AD_Table_ID AND IsActive='Y' AND IsAllNodes='Y' "
                        + "ORDER BY IsDefault DESC, AD_Tree_ID";

            queryList.VIS_121 = "SELECT AD_Tree_ID, Name FROM AD_Tree "
                    + "WHERE AD_Client_ID=@AD_Client_ID AND AD_Table_ID=@AD_Table_ID AND IsActive='Y' AND IsAllNodes='Y' "
                    + "ORDER BY IsDefault DESC, AD_Tree_ID";


            queryList.VIS_122 = "SELECT t.TableName, c.AD_Column_ID, c.ColumnName, e.Name,"	//	1..4
            + "c.IsParent, c.IsKey, c.IsIdentifier, c.IsTranslated "				//	4..8
            + "FROM AD_Table t, AD_Column c, AD_Element e "
            + "WHERE t.AD_Table_ID=@AD_Table_ID"
            + " AND t.AD_Table_ID=c.AD_Table_ID"
            + " AND (c.AD_Column_ID=@AD_ColumnSortOrder_ID OR AD_Column_ID=@AD_ColumnSortYesNo_ID"  	//	#2..3
            + " OR c.IsParent='Y' OR c.IsKey='Y' OR c.IsIdentifier='Y')"
            + " AND c.AD_Element_ID=e.AD_Element_ID";

            queryList.VIS_123 = "SELECT t.TableName, c.AD_Column_ID, c.ColumnName, et.Name,"	//	1..4
                + "c.IsParent, c.IsKey, c.IsIdentifier, c.IsTranslated "		//	4..8
                + "FROM AD_Table t, AD_Column c, AD_Element_Trl et "
                + "WHERE t.AD_Table_ID=@AD_Table_ID" //	#1
                + " AND t.AD_Table_ID=c.AD_Table_ID"
                + " AND (c.AD_Column_ID=@AD_ColumnSortOrder_ID OR AD_Column_ID=@AD_ColumnSortYesNo_ID" //	#2..3
                + "	OR c.IsParent='Y' OR c.IsKey='Y' OR c.IsIdentifier='Y')"
                + " AND c.AD_Element_ID=et.AD_Element_ID"
                + " AND et.AD_Language=@AD_Language";


            queryList.VIS_124 = "SELECT * FROM C_ValidCombination WHERE C_ValidCombination_ID=@C_ValidCombination_ID AND C_AcctSchema_ID=@C_AcctSchema_ID";

            queryList.VIS_125 = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID = @AD_Column_ID";

            queryList.VIS_126 = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=@tblID_s";

            queryList.VIS_127 = "UPDATE C_ValidCombination SET Alias=NULL WHERE C_ValidCombination_ID=@IDvalue";

            queryList.VIS_128 = "UPDATE C_ValidCombination SET Alias=@f_alies WHERE C_ValidCombination_ID=@IDvalue";

            queryList.VIS_129 = "SELECT AD_Window_ID FROM AD_Window WHERE Name='All Requests'";

            queryList.VIS_130 = "select ad_entitytype_id, entitytype, name from ad_entitytype";

            queryList.VIS_131 = "SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'";


            queryList.VIS_132 = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID =@M_Product_ID";

            queryList.VIS_133 = "SELECT "	//	Entered UOM
                //+ "l.QtyInvoiced-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.QtyInvoiced,6),"
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QUANTITY,"	//	2
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QTYENTER,"	//	2
            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"			//  3..4
            + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,"      //  5..8
            + " l.C_OrderLine_ID "                					//  9
            + " FROM C_UOM uom  INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) "
            + " INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
              + " WHERE l.C_Invoice_ID=@C_Invoice_ID GROUP BY l.QtyInvoiced,l.QtyEntered, l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                    + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID ORDER BY l.Line";

            queryList.VIS_134 = "SELECT "	//	Entered UOM
                //+ "l.QtyInvoiced-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.QtyInvoiced,6),"
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QUANTITY,"	//	2
            + "round((l.QtyInvoiced-SUM(COALESCE(mi.Qty,0))) * "					//	1               
            + "(CASE WHEN l.QtyInvoiced=0 THEN 0 ELSE l.QtyEntered/l.QtyInvoiced END ),2) as QTYENTER,"	//	2
            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"			//  3..4
            + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,"      //  5..8
            + " l.C_OrderLine_ID FROM C_UOM_Trl uom INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language=@AD_Language) INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) "
           + " LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) WHERE l.C_Invoice_ID=@C_Invoice_ID GROUP BY l.QtyInvoiced,l.QtyEntered,"
            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID  ORDER BY l.Line";


            queryList.VIS_135 = "SELECT PaymentRule FROM C_PaySelectionCheck WHERE C_PaySelection_ID = @pSelectID";

            queryList.VIS_136 = "select ad_process_id from ad_process where ad_printformat_id = (select check_printformat_id from c_bankaccountdoc where c_bankaccount_id = (select c_bankaccount_id from c_payment where c_payment_id = (select c_payment_id from c_payselectioncheck where c_payselectioncheck_id = @check_ID)) and c_bankaccountdoc.isactive = 'Y' AND rownum =1)";

            queryList.VIS_137 = "select ad_table_id from ad_table where tablename = 'C_PaySelectionCheck'";

            queryList.VIS_138 = "SELECT AD_Process_ID  FROM AD_Tab WHERE AD_Tab_ID = 330";

            queryList.VIS_139 = "select ad_table_id from ad_table where tablename = 'C_Payment'";

            queryList.VIS_140 = "SELECT M_InOut_ID FROM M_InOutLine WHERE M_InOutLine_ID=@lineID";

            queryList.VIS_141 = "SELECT M_Inventory_ID FROM M_InventoryLine WHERE M_InventoryLine_ID=@lineID";

            queryList.VIS_142 = "SELECT M_Movement_ID FROM M_MovementLine WHERE M_MovementLine_ID=@lineID";

            queryList.VIS_143 = "SELECT M_Production_ID FROM M_ProductionLine WHERE M_ProductionLine_ID=@lineID";


            queryList.VIS_144 = "SELECT Log_ID, P_ID, P_Date, P_Number, P_Msg "
                + "FROM AD_PInstance_Log "
                + "WHERE AD_PInstance_ID=@AD_PInstance_ID "
                + " ORDER BY Log_ID";

            queryList.VIS_145 = "SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA034_' AND IsActive = 'Y'";

            queryList.VIS_146 = "SELECT adt.TableName, adt.AD_Window_ID, adt.PO_Window_ID, " +
            "case when adwfa.AD_Window_ID is null then (select AD_WINDOW_ID from AD_WF_Activity where AD_WF_Process_ID = (select AD_WF_Process_ID from AD_WF_Activity where AD_WF_Activity_ID = adwfa.AD_WF_Activity_ID) and AD_WINDOW_ID is not null AND rownum = 1) else adwfa.AD_Window_ID end as ActivityWindow " +
            "FROM AD_Table adt " +
            "LEFT JOIN AD_WF_Activity adwfa on adt.AD_Table_ID = adwfa.AD_Table_ID " +
            "WHERE adt.AD_Table_ID = @AD_Table_ID and adwfa.AD_WF_Activity_ID = @AD_WF_Activity_ID";

            queryList.VIS_147 = "DELETE FROM AD_UserQueryLine WHERE AD_UserQueryLine_ID=@AD_UserQuery_ID";

            queryList.VIS_148 = "SELECT l.Value, t.Name FROM AD_Ref_List l, AD_Ref_List_Trl t "
                  + "WHERE l.AD_Ref_List_ID=t.AD_Ref_List_ID"
                  + " AND t.AD_Language='@Language_Translation@'"
                  + " AND l.AD_Reference_ID=@AD_Reference_ID AND l.IsActive='Y'";

            queryList.VIS_149 = "SELECT IsCrystalReport FROM AD_Process WHERE AD_Process_ID=@AD_Process_ID";

            queryList.VIS_150 = "select ad_table_id from ad_table where tablename = 'C_PaySelection'";


            queryList.VIS_151 = "select AD_Process_ID from AD_Process where name='VARPT_RemittancePrint'";

            queryList.VIS_152 = " SELECT AD_Process_ID from C_BankAccountDoc WHERE C_BankAccount_ID=@BankAcct_ID AND rownum=1";
        }

        public static string GetQuery(string code, Ctx ctx)
        {
            if (IqueryList.Count <= 0)
            {
                AddQueries(ctx);
                isLoad = true;
            }

            object result = ((IDictionary<string, object>)queryList)[code];
            if (result != null)
            {
                return result.ToString().Replace("@Language_Translation@", Env.GetAD_Language(ctx));
            }
            return code;
        }



    }
}
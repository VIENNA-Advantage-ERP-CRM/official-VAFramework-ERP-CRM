using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class InfoProductModel
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(InfoProductModel).FullName);

        private static InfoColumn[] s_productLayout = null;
        // Header for Price List Version 	
        private static String s_headerPriceList = "";
        // Header for Warehouse 		
        private static String s_headerWarehouse = "";
        private static int INDEX_NAME = 0;
        private static int INDEX_PATTRIBUTE = 0;
        bool isConferm = false;

        private enum Windows
        {
            PhysicalInventory = 168,
            Shipment = 169,
            InventoryMove = 170,
            MaterialReceipt = 184,
            Package = 319,
            InternalUse = 341,
        }

        StringBuilder _sqlQuery = new StringBuilder();

        public InfoColumn[] GetInfoColumns(VAdvantage.Utility.Ctx ctx)
        {
            if (s_productLayout != null)
                return s_productLayout;
            //
            s_headerPriceList = Msg.Translate(ctx, "VAM_PriceListVersion_ID");
            s_headerWarehouse = Msg.Translate(ctx, "VAM_Warehouse_ID");
            //  Euro 13
            MVAFClient client = MVAFClient.Get(ctx);
            if ("FRIE".Equals(client.GetValue()))
            {
                InfoColumn[] frieLayout =
            {
				new InfoColumn(Msg.Translate(ctx,"VAM_Product_ID"),"VAM_Product_ID", true, "p.VAM_Product_ID",DisplayType.ID).Seq(10),
				new InfoColumn(Msg.Translate(ctx, "Name"), "Name", true, "p.Name",DisplayType.String).Seq(20),
                new InfoColumn(Msg.Translate(ctx,"QtyEntered"), "QtyEntered", false, "1 as QtyEntered" , DisplayType.Quantity).Seq(30), 
				new InfoColumn(Msg.Translate(ctx, "QtyAvailable"), "QtyAvailable",true,
					"bomQtyAvailable(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyAvailable", DisplayType.Quantity).Seq(40),
				new InfoColumn(s_headerPriceList, "VAM_PriceListVersion_ID",true, "plv.Name", DisplayType.Amount).Seq(50),
                new InfoColumn(s_headerWarehouse, "VAM_Warehouse_ID",true, "w.Name", DisplayType.String).Seq(60),
				new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList",true,
					"bomPriceList(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS PriceList",  DisplayType.Amount).Seq(70),
				new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd",true,
					"bomPriceStd(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS PriceStd", DisplayType.Amount).Seq(80),
				new InfoColumn("Einzel MWSt", "",true,
					"pr.PriceStd * 1.19", DisplayType.Amount).Seq(90),
				new InfoColumn("Einzel kompl", "",true,
					"(pr.PriceStd+13) * 1.19", DisplayType.Amount).Seq(100),
				new InfoColumn("Satz kompl", "", true,
					"((pr.PriceStd+13) * 1.19) * 4", DisplayType.Amount).Seq(110),
				new InfoColumn(Msg.Translate(ctx, "QtyOnHand"), "QtyOnHand",true,
					"bomQtyOnHand(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyOnHand", DisplayType.Quantity).Seq(120),
				new InfoColumn(Msg.Translate(ctx, "QtyReserved"), "QtyReserved",true,
					"bomQtyReserved(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyReserved", DisplayType.Quantity).Seq(130),
				new InfoColumn(Msg.Translate(ctx, "QtyOrdered"), "QtyOrdered",true,
					"bomQtyOrdered(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyOrdered", DisplayType.Quantity).Seq(140),
				new InfoColumn(Msg.Translate(ctx, "Discontinued").Substring(0, 1), "Discontinued",true,
					"p.Discontinued", DisplayType.YesNo).Seq(150),
				new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin",true,
					"bomPriceStd(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID)-bomPriceLimit(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS Margin", DisplayType.Amount).Seq(160),
				new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit",true,
					"bomPriceLimit(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS PriceLimit", DisplayType.Amount).Seq(170),
				new InfoColumn(Msg.Translate(ctx, "IsInstanceAttribute"), "IsInstanceAttribute",true,
					"pa.IsInstanceAttribute", DisplayType.YesNo).Seq(180),
                new InfoColumn(Msg.Translate(ctx,"GuranteeDays"),"GuranteeDays",true, "adddays(Sysdate, p.GuaranteeDays) as GuranteeDays",DisplayType.Date).Seq(190)
                    //new InfoColumn(Msg.Translate(ctx, "Quantity"), "0 as Quantity" , typeof(Boolean)).Seq(180) 
                   
			};
                INDEX_NAME = 1;
                INDEX_PATTRIBUTE = frieLayout.Length - 1;	//	last item
                s_productLayout = frieLayout;
                return s_productLayout;
            }
            if (s_productLayout == null)
            {
                List<InfoColumn> list = new List<InfoColumn>();
                list.Add(new InfoColumn(Msg.Translate(ctx, "VAM_Product_ID"), "VAM_Product_ID", true, "p.VAM_Product_ID", DisplayType.ID).Seq(10));
                //list.Add(new InfoColumn(Msg.Translate(ctx, "SelectProduct"),"SelectProduct",true, "'N'", DisplayType.YesNo).Seq(20));
                //list.Add(new InfoColumn(Msg.Translate(ctx, "Discontinued"), "Discontinued", true, "p.Discontinued", DisplayType.YesNo).Seq(30));
                list.Add(new InfoColumn(Msg.Translate(ctx, "VAB_UOM_ID"), "VAB_UOM_ID", true, "p.VAB_UOM_ID", DisplayType.ID).Seq(20));
                list.Add(new InfoColumn(Msg.Translate(ctx, "UOM"), "UOM", true, "c.Name as UOM", DisplayType.ID).Seq(30));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Value"), "Value", true, "p.Value", DisplayType.String).Seq(40));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Name"), "Name", true, "p.Name", DisplayType.String).Seq(50));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyEntered"), "QtyEntered", false, "1 as QtyEntered", DisplayType.Quantity).Seq(60));
                list.Add(new InfoColumn(s_headerWarehouse, "Warehouse", true, "w.Name as Warehouse", DisplayType.String).Seq(80));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyAvailable"), "QtyAvailable", true,
                    "bomQtyAvailable(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyAvailable", DisplayType.Quantity).Seq(90));
                //}
                list.Add(new InfoColumn(s_headerPriceList, "PriceListVersion", true, "plv.Name as PriceListVersion", DisplayType.String).Seq(100));
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("VAPRC_", out mInfo))
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                        "bomPriceListUom(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID,pr.VAB_UOM_ID) AS PriceList", DisplayType.Amount).Seq(120));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                        "bomPriceStdUom(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID,pr.VAB_UOM_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                    }
                    else
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                            "bomPriceListAttr(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID) AS PriceList", DisplayType.Amount).Seq(120));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                            "bomPriceStdAttr(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                    }
                }
                else
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                        "bomPriceList(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS PriceList", DisplayType.Amount).Seq(120));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                        "bomPriceStd(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                }

                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyOnHand"), "QtyOnHand", true,
                    "bomQtyOnHand(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyOnHand", DisplayType.Quantity).Seq(140));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyReserved"), "QtyReserved", true,
                    "bomQtyReserved(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyReserved", DisplayType.Quantity).Seq(150));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyOrdered"), "QtyOrdered", true,
                    "bomQtyOrdered(p.VAM_Product_ID,w.VAM_Warehouse_ID,0) AS QtyOrdered", DisplayType.Quantity).Seq(160));
                //}
                if (isConferm) //IsUnconfirmed())
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "QtyUnconfirmed"), "QtyUnconfirmed", true,
                        "(SELECT SUM(c.TargetQty) FROM VAM_Inv_InOutLineConfirm c INNER JOIN VAM_Inv_InOutLine il ON (c.VAM_Inv_InOutLine_ID=il.VAM_Inv_InOutLine_ID) INNER JOIN VAM_Inv_InOut i ON (il.VAM_Inv_InOut_ID=i.VAM_Inv_InOut_ID) WHERE c.Processed='N' AND i.VAM_Warehouse_ID=w.VAM_Warehouse_ID AND il.VAM_Product_ID=p.VAM_Product_ID) AS QtyUnconfirmed",
                        DisplayType.Quantity).Seq(170));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "QtyUnconfirmedMove"), "QtyUnconfirmedMove", true,
                        "(SELECT SUM(c.TargetQty) FROM VAM_InvTrf_LineConfirm c INNER JOIN VAM_InvTrf_Line ml ON (c.VAM_InvTrf_Line_ID=ml.VAM_InvTrf_Line_ID) INNER JOIN VAM_Locator l ON (ml.VAM_LocatorTo_ID=l.VAM_Locator_ID) WHERE c.Processed='N' AND l.VAM_Warehouse_ID=w.VAM_Warehouse_ID AND ml.VAM_Product_ID=p.VAM_Product_ID) AS QtyUnconfirmedMove",
                        DisplayType.Quantity).Seq(180));
                }
                if (Env.HasModulePrefix("VAPRC_", out mInfo))
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin", true,
                            "bomPriceStdUom(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID,pr.VAB_UOM_ID)-bomPriceLimitUom(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID,pr.VAB_UOM_ID) AS Margin",
                            DisplayType.Amount).Seq(190));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit", true,
                            "bomPriceLimitUom(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID,pr.VAB_UOM_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                    }
                    else
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin", true,
                            "bomPriceStdAttr(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID)-bomPriceLimitAttr(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID) AS Margin",
                            DisplayType.Amount).Seq(190));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit", true,
                            "bomPriceLimitAttr(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID,pr.VAM_PFeature_SetInstance_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                    }
                }
                else
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin", true,
                            "bomPriceStd(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID)-bomPriceLimit(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS Margin", DisplayType.Amount).Seq(190));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit", true,
                        "bomPriceLimit(p.VAM_Product_ID, pr.VAM_PriceListVersion_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                }
                list.Add(new InfoColumn(Msg.Translate(ctx, "IsInstanceAttribute"), "IsInstanceAttribute", true, "pa.IsInstanceAttribute", DisplayType.YesNo).Seq(210));
                list.Add(new InfoColumn(Msg.Translate(ctx, "GuranteeDays"), "GuranteeDays", true, "adddays(Sysdate, p.GuaranteeDays) as GuranteeDays", DisplayType.Date).Seq(220));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Discontinued"), "Discontinued", true, "p.Discontinued", DisplayType.YesNo).Seq(230));
                list.Add(new InfoColumn(s_headerWarehouse, "VAM_Warehouse_ID", true, "w.VAM_Warehouse_ID", DisplayType.ID).Seq(240));
                list.Add(new InfoColumn(s_headerPriceList, "VAM_PriceListVersion_ID", true, "plv.VAM_PriceListVersion_ID", DisplayType.ID).Seq(250));
                s_productLayout = new InfoColumn[list.Count];
                s_productLayout = list.ToArray();
                INDEX_NAME = 3;
                INDEX_PATTRIBUTE = s_productLayout.Length - 1;	//	last item
            }
            return s_productLayout;
        }

        public InfoProductData GetData(string sql, string where, string tableName, int pageNo, bool isMobile, VAdvantage.Utility.Ctx ctx)
        {
            InfoProductData _iData = new InfoProductData();
            try
            {
                StringBuilder sbSql = new StringBuilder();
                int noofRecords = isMobile ? 50 : 100;
                where = where.Replace('●', '%');
                string sqlWhere = sql + where;
                sqlWhere = MVAFRole.GetDefault(ctx).AddAccessSQL(sqlWhere, tableName,
                                MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);
                //DataSet data = DBase.DB.ExecuteDataset(sql, null, null);
                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sqlWhere + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                int startPage = ((pageNo - 1) * pageSize) + 1;
                int endPage = pageNo * pageSize;
                string sqlPaging = @"SELECT DISTINCT p.VAM_Product_ID, NVL(pr.VAM_PFeature_SetInstance_ID,0) AS VAM_PFeature_SetInstance_ID, NVL(pr.VAB_UOM_ID,0) AS VAB_UOM_ID, 
                        NVL(pr.VAM_PriceListVersion_ID,0) AS VAM_PriceListVersion_ID, w.VAM_Warehouse_ID FROM VAM_Product p 
                        LEFT OUTER JOIN VAM_ProductPrice pr  ON (p.VAM_Product_ID=pr.VAM_Product_ID AND pr.IsActive ='Y') 
                        LEFT OUTER JOIN VAM_PFeature_Set pa ON (p.VAM_PFeature_Set_ID=pa.VAM_PFeature_Set_ID) 
                        LEFT OUTER JOIN VAB_UOM c ON (p.VAB_UOM_ID=c.VAB_UOM_ID)
                        LEFT OUTER JOIN VAM_Manufacturer mr ON (p.VAM_Product_ID = mr.VAM_Product_ID)
                        LEFT OUTER JOIN VAM_ProductFeatures patr ON (p.VAM_Product_ID = patr.VAM_Product_ID)
                        LEFT OUTER JOIN VAB_UOM_Conversion uc ON (p.VAM_Product_ID = uc.VAM_Product_ID)
                        , VAM_Warehouse w " + where + " ORDER BY p.VAM_Product_ID, VAM_PriceListVersion_ID, w.VAM_Warehouse_ID, VAM_PFeature_SetInstance_ID, VAB_UOM_ID";
                sqlPaging = MVAFRole.GetDefault(ctx).AddAccessSQL(sqlPaging, tableName, MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);
                sqlPaging = @"JOIN (SELECT row_num, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAB_UOM_ID, VAM_PriceListVersion_ID, VAM_Warehouse_ID FROM (SELECT prd.*, row_number() over (order by prd.VAM_Product_ID) AS row_num FROM 
                        (" + sqlPaging + @") prd) t WHERE row_num BETWEEN " + startPage + " AND " + endPage +
                        @") pp ON pp.VAM_Product_ID = p.VAM_Product_ID AND pp.VAM_PFeature_SetInstance_ID = NVL(pr.VAM_PFeature_SetInstance_ID,0) AND pp.VAB_UOM_ID = NVL(pr.VAB_UOM_ID,0) 
                        AND pp.VAM_Warehouse_ID = w.VAM_Warehouse_ID AND pp.VAM_PriceListVersion_ID = NVL(pr.VAM_PriceListVersion_ID,0)";
                sql += sqlPaging;       // + where;
                DataSet data = DB.ExecuteDataset(sql, null, null);
                if (data == null)
                {
                    return null;
                }

                List<DataObject> dyndata = new List<DataObject>();
                DataObject item = null;
                List<object> values = null;
                for (int i = 0; i < data.Tables[0].Columns.Count; i++)  //columns
                {
                    item = new DataObject();

                    item.ColumnName = data.Tables[0].Columns[i].ColumnName;
                    values = new List<object>();
                    for (int j = 0; j < data.Tables[0].Rows.Count; j++)  //rows
                    {
                        values.Add(data.Tables[0].Rows[j][data.Tables[0].Columns[i].ColumnName]);
                    }
                    item.Values = values;
                    item.RowCount = data.Tables[0].Rows.Count;
                    dyndata.Add(item);
                }
                _iData.data = dyndata;
                return _iData;
            }
            catch (Exception ex)
            {
                _iData.Error = ex.Message;
                return _iData;
            }
        }

        public InfoCartData GetCart(string sql, int pageNo, bool isCart, int windowID, int WarehouseID, int WarehouseToID, int LocatorID, int LocatorToID, VAdvantage.Utility.Ctx ctx)
        {
            InfoCartData _iData = new InfoCartData();
            try
            {
                if (!isCart && windowID == 170)
                {
                    StringBuilder sqlWhere = new StringBuilder("");
                    if (WarehouseID > 0)
                        sqlWhere.Append(" AND DTD001_MWarehouseSource_ID = " + WarehouseID);
                    if (WarehouseToID > 0)
                        sqlWhere.Append(" AND VAM_Warehouse_ID = " + WarehouseToID);
                    else if (LocatorToID > 0)  // JID_0985 If only from warehouse selected on move header. In scan option carts should show which have requistion no. 
                    {                          //having same source warehosue as locator of warehouse selected at move line.
                        WarehouseToID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_Warehouse_ID FROM VAM_Locator WHERE VAM_Locator_ID = " + LocatorToID));
                        if (WarehouseToID > 0)
                            sqlWhere.Append(" AND VAM_Warehouse_ID = " + WarehouseToID);
                    }
                    sql += " AND VAICNT_ReferenceNo IN (SELECT DocumentNo FROM VAM_Requisition WHERE VAF_Client_ID = " + ctx.GetVAF_Client_ID() + " AND IsActive = 'Y' AND DocStatus IN ('CO') "
                        + sqlWhere.ToString() + ")";
                }
                else if (!isCart && windowID == 168)  // JID_1030: on physical inventory system does not check that the locator is of selected warehouse on Physiacl inventory header or not.
                {
                    if (WarehouseID > 0)
                        sql += " AND VAICNT_ReferenceNo IN (SELECT Value FROM VAM_Locator WHERE IsActive = 'Y' AND VAM_Warehouse_ID = " + WarehouseID + ")";
                }

                sql += " ORDER BY VAICNT_ScanName";

                sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAICNT_InventoryCount",
                                MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);

                //DataSet data = DB.ExecuteDataset(sql, null, null);
                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sql + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                DataSet data = VIS.DBase.DB.ExecuteDatasetPaging(sql, pageNo, pageSize);
                if (data == null)
                {
                    return null;
                }

                List<InfoCart> dyndata = new List<InfoCart>();
                if (data.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < data.Tables[0].Rows.Count; i++)  //columns
                    {
                        InfoCart item = new InfoCart();
                        item.recid = i + 1;
                        item.Name = Util.GetValueOfString(data.Tables[0].Rows[i]["VAICNT_ScanName"]);
                        item.Reference = Util.GetValueOfString(data.Tables[0].Rows[i]["VAICNT_ReferenceNo"]);
                        item.TrxDate = Util.GetValueOfDateTime(data.Tables[0].Rows[i]["DateTrx"]);
                        item.count_ID = Util.GetValueOfInt(data.Tables[0].Rows[i]["VAICNT_InventoryCount_ID"]);
                        dyndata.Add(item);
                    }
                }

                _iData.data = dyndata;
                return _iData;
            }
            catch (Exception ex)
            {
                _iData.Error = ex.Message;
                return _iData;
            }
        }

        /// <summary>
        /// Function to save Products selected from either cart or scan form
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="keyColName"></param>
        /// <param name="product"></param>
        /// <param name="uoms"></param>
        /// <param name="attribute"></param>
        /// <param name="qty"></param>
        /// <param name="LocToID"></param>
        /// <param name="lineID"></param>
        /// <param name="InvCountID"></param>
        /// <param name="RefNo"></param>
        /// <param name="Locator_ID"></param>
        /// <param name="WindowID"></param>
        /// <param name="ContainerID"></param>
        /// <param name="ContainerToID"></param>
        /// <param name="ctx"></param>
        /// <returns>Status in the form of InfoSave class</returns>
        public InfoSave SetProductQty(int recordID, string keyColName, List<string> product, List<string> uoms, List<string> attribute,
            List<string> qty, int LocToID, int lineID, List<string> InvCountID, List<string> ReferenceNo,
            int Locator_ID, int WindowID, int ContainerID, int ContainerToID, VAdvantage.Utility.Ctx ctx)
        {
            #region local Variables
            InfoSave info = new InfoSave();
            string error = "";
            MVAFTableView tbl = null;
            PO po = null;
            int count = 0;

            DataSet dsProPO = null;
            DataSet dsProPrice = null;
            DataSet dsUOMConv = null;
            DataSet dsProducts = null;
            DataSet dsOrderLines = null;

            bool hasProdsPurch = false;
            bool hasProducts = false;
            bool hasOrderLines = false;
            bool hasConversions = false;

            bool fetchedProPurchasing = false;
            bool fetchedUOMConv = false;
            bool fetchedRecords = false;

            bool isSOTrx = true;
            int _Version_ID = 0;

            int VAF_Client_ID = 0;
            int VAF_Org_ID = 0;
            int VAB_BusinessPartner_ID = 0;
            int VAM_DiscountCalculation_ID = 0;
            Decimal? bpFlatDiscount = 0;
            MVABBusinessPartner bp = null;
            StringBuilder prodName = new StringBuilder("");
            List<string> errorKeys = new List<string>();
            List<string> errorProdLines = new List<string>();
            #endregion local Variables

            // Evaluate only for these 4 tables
            // get IDs like Client ID, Org ID, BP ID from parent
            if (keyColName.ToUpper().Trim() == "VAB_ORDER_ID" || keyColName.ToUpper().Trim() == "VAB_INVOICE_ID"
                || keyColName.ToUpper().Trim() == "VAM_Requisition_ID" || keyColName.ToUpper().Trim() == "VAB_PROJECT_ID")
            {
                if (keyColName.ToUpper().Trim() == "VAB_ORDER_ID")
                {
                    MVABOrder ord = new MVABOrder(ctx, recordID, null);
                    _Version_ID = GetPLVID(ord.GetVAM_PriceList_ID());
                    VAF_Client_ID = ord.GetVAF_Client_ID();
                    VAF_Org_ID = ord.GetVAF_Org_ID();
                    isSOTrx = ord.IsSOTrx();
                    VAB_BusinessPartner_ID = ord.GetVAB_BusinessPartner_ID();
                }
                else if (keyColName.ToUpper().Trim() == "VAB_INVOICE_ID")
                {
                    MInvoice inv = new MInvoice(ctx, recordID, null);
                    _Version_ID = GetPLVID(inv.GetVAM_PriceList_ID());
                    VAF_Client_ID = inv.GetVAF_Client_ID();
                    VAF_Org_ID = inv.GetVAF_Org_ID();
                    isSOTrx = inv.IsSOTrx();
                    VAB_BusinessPartner_ID = inv.GetVAB_BusinessPartner_ID();
                }
                else if (keyColName.ToUpper().Trim() == "VAM_Requisition_ID")
                {
                    MRequisition req = new MRequisition(ctx, recordID, null);
                    _Version_ID = GetPLVID(req.GetVAM_PriceList_ID());
                    VAF_Client_ID = req.GetVAF_Client_ID();
                    VAF_Org_ID = req.GetVAF_Org_ID();
                    if (req.Get_ColumnIndex("VAB_BusinessPartner_ID") > 0 && Util.GetValueOfInt(req.Get_Value("VAB_BusinessPartner_ID")) > 0)
                    {
                        VAB_BusinessPartner_ID = Util.GetValueOfInt(req.Get_Value("VAB_BusinessPartner_ID"));
                    }
                }
                else if (keyColName.ToUpper().Trim() == "VAB_PROJECT_ID")
                {
                    MProject proj = new MProject(ctx, recordID, null);
                    VAF_Client_ID = proj.GetVAF_Client_ID();
                    _Version_ID = proj.GetVAM_PriceListVersion_ID();
                    VAF_Org_ID = proj.GetVAF_Org_ID();
                    VAB_BusinessPartner_ID = proj.GetVAB_BusinessPartner_ID();
                }

                // if business partner found on parent then get Discount Schema ID and Flat Discount amount
                if (VAB_BusinessPartner_ID > 0)
                {
                    bp = new MVABBusinessPartner(ctx, VAB_BusinessPartner_ID, null);
                    VAM_DiscountCalculation_ID = bp.GetVAM_DiscountCalculation_ID();
                    bpFlatDiscount = bp.GetFlatDiscount();
                }

                if (!isSOTrx)
                {
                    if (dsProPO == null && !fetchedProPurchasing)
                    {
                        dsProPO = GetPurchaingProduct(VAF_Client_ID);
                        fetchedProPurchasing = true;
                        if (dsProPO != null && dsProPO.Tables[0].Rows.Count > 0)
                            hasProdsPurch = true;
                    }
                }
                // if UOM Pricing module is available then fetch UOM Conversions
                if (Env.IsModuleInstalled("ED011_"))
                {
                    if (!fetchedUOMConv)
                    {
                        dsUOMConv = GetUOMConversions(VAF_Client_ID);
                        fetchedUOMConv = true;
                    }
                }
            }

            // Fetch Products and Product Prices 
            if (!fetchedRecords)
            {
                if (VAF_Client_ID == 0)
                    VAF_Client_ID = ctx.GetVAF_Client_ID();
                dsProducts = GetProducts(VAF_Client_ID);
                if (dsProducts != null && dsProducts.Tables[0].Rows.Count > 0)
                    hasProducts = true;

                if (_Version_ID > 0)
                    dsProPrice = GetProductsPrice(VAF_Client_ID, _Version_ID);

                fetchedRecords = true;
            }

            #region Order Table
            if (keyColName.ToUpper().Trim() == "VAB_ORDER_ID")
            {
                tbl = new MVAFTableView(ctx, 260, null);
                MVABOrder ord = new MVABOrder(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    int _VAM_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = Util.GetValueOfInt(attribute[i]);
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("VAF_Client_ID", VAF_Client_ID);
                    po.Set_ValueNoCheck("VAF_Org_ID", VAF_Org_ID);
                    po.Set_Value("VAM_Product_ID", _VAM_Product_ID);
                    po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                    po.Set_Value("QtyOrdered", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("VAB_Order_ID", recordID);
                    if (_attribute_ID != 0)
                        po.Set_Value("VAM_PFeature_SetInstance_ID", _attribute_ID);

                    if (!ord.IsSOTrx())
                    {
                        int uomID = Util.GetValueOfInt(uoms[i]);
                        int uom = 0;

                        if (hasProdsPurch)
                        {
                            DataRow[] dr = dsProPO.Tables[0].Select(" VAM_Product_ID = " + _VAM_Product_ID + " AND VAB_BusinessPartner_ID = " + VAB_BusinessPartner_ID);
                            if (dr != null && dr.Length > 0)
                                uom = Util.GetValueOfInt(dr[0]["VAB_UOM_ID"]);
                        }

                        if (uomID != 0)
                        {
                            if (uomID != uom && uom != 0)
                                po.Set_ValueNoCheck("VAB_UOM_ID", uom);
                            else
                                po.Set_ValueNoCheck("VAB_UOM_ID", uomID);
                        }
                    }
                    else
                        po.Set_ValueNoCheck("VAB_UOM_ID", Util.GetValueOfInt(uoms[i]));

                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        SetPrices(po, GetPrices(dsProPrice, dsUOMConv, VAF_Client_ID, _VAM_Product_ID,
                            _attribute_ID, Util.GetValueOfInt(po.Get_Value("VAB_UOM_ID")), dsProducts,
                            Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), VAM_DiscountCalculation_ID, bpFlatDiscount));
                    }
                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, _VAM_Product_ID, prodName);
                    }
                }
            }
            #endregion Order

            #region Invoice
            else if (keyColName.ToUpper().Trim() == "VAB_INVOICE_ID")
            {
                tbl = new MVAFTableView(ctx, 333, null);
                MInvoice inv = new MInvoice(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    int _VAM_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = Util.GetValueOfInt(attribute[i]);
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("VAF_Client_ID", VAF_Client_ID);
                    po.Set_ValueNoCheck("VAF_Org_ID", VAF_Org_ID);
                    po.Set_Value("VAM_Product_ID", _VAM_Product_ID);
                    po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                    po.Set_Value("QtyInvoiced", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("VAB_Invoice_ID", recordID);
                    if (_attribute_ID != 0)
                        po.Set_Value("VAM_PFeature_SetInstance_ID", _attribute_ID);

                    if (!inv.IsSOTrx())
                    {
                        int uomID = Util.GetValueOfInt(uoms[i]);
                        int uom = 0;

                        if (hasProdsPurch)
                        {
                            DataRow[] dr = dsProPO.Tables[0].Select(" VAM_Product_ID = " + _VAM_Product_ID + " AND VAB_BusinessPartner_ID = " + VAB_BusinessPartner_ID);
                            if (dr != null && dr.Length > 0)
                                uom = Util.GetValueOfInt(dr[0]["VAB_UOM_ID"]);
                        }

                        if (uomID != 0)
                        {
                            if (uomID != uom && uom != 0)
                                po.Set_ValueNoCheck("VAB_UOM_ID", uom);
                            else
                                po.Set_ValueNoCheck("VAB_UOM_ID", uomID);
                        }
                    }
                    else
                        po.Set_ValueNoCheck("VAB_UOM_ID", Util.GetValueOfInt(uoms[i]));

                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        SetPrices(po, GetPrices(dsProPrice, dsUOMConv, VAF_Client_ID, _VAM_Product_ID,
                            _attribute_ID, Util.GetValueOfInt(po.Get_Value("VAB_UOM_ID")), dsProducts,
                            Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), VAM_DiscountCalculation_ID, bpFlatDiscount));
                    }
                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, _VAM_Product_ID, prodName);
                    }
                }
            }
            #endregion Invoice

            #region MR/Shipment
            else if (keyColName.ToUpper().Trim() == "VAM_Inv_InOut_ID")
            {
                tbl = new MVAFTableView(ctx, 320, null);

                int ordID = 0;

                bool saved = true;
                MInOut io = new MInOut(ctx, recordID, null);

                if (Locator_ID <= 0)
                {
                    Locator_ID = hasDefaultLocator(io.GetVAM_Warehouse_ID());
                    if (Locator_ID <= 0)
                    {
                        count = -1;
                        error = Msg.GetMsg(ctx, "DefaultLocatorNotFound");
                    }
                }

                // check applied for Locator 
                if (Locator_ID > 0)
                {
                    if (product.Count > 0)
                    {
                        string RefNo = ReferenceNo[0];
                        _sqlQuery.Clear();
                        if (RefNo != "")
                        {
                            if (WindowID == Util.GetValueOfInt(Windows.Shipment))
                                _sqlQuery.Append("SELECT VAB_Order_ID FROM VAB_Order WHERE IsActive = 'Y' AND IsSOTrx= 'Y' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                            else if (WindowID == Util.GetValueOfInt(Windows.MaterialReceipt))
                                _sqlQuery.Append("SELECT VAB_Order_ID FROM VAB_Order WHERE IsActive = 'Y' AND IsSOTrx= 'N' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                            else
                                _sqlQuery.Append("SELECT VAB_Order_ID FROM VAB_Order WHERE IsActive = 'Y' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                        }
                        else if (WindowID == Util.GetValueOfInt(Windows.Shipment))
                            _sqlQuery.Append("SELECT VAB_Order_ID FROM VAM_Inv_InOut WHERE IsActive = 'Y' AND IsSOTrx = 'Y' AND VAM_Inv_InOut_ID = " + recordID);

                        if (_sqlQuery.Length > 0)
                            ordID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

                        if (ordID > 0)
                        {
                            string selColumn = "";
                            if (Env.IsModuleInstalled("DTD001_"))
                                selColumn = " , ol.DTD001_Org_ID ";

                            _sqlQuery.Clear();
                            _sqlQuery.Append("SELECT ol.VAB_OrderLine_ID, ol.VAM_Product_ID, ol.VAM_PFeature_SetInstance_ID, ol.VAB_UOM_ID " + selColumn + " FROM VAB_OrderLine ol WHERE ol.VAB_Order_ID = " + ordID);
                            dsOrderLines = DB.ExecuteDataset(_sqlQuery.ToString());
                            if (dsOrderLines != null && dsOrderLines.Tables[0].Rows.Count > 0)
                            {
                                hasOrderLines = true;
                            }
                        }

                        if (ordID > 0)
                        {
                            io.SetVAB_Order_ID(ordID);
                            if (!io.Save())
                                saved = false;
                        }
                    }

                    if (saved)
                    {
                        for (int i = 0; i < product.Count; i++)
                        {
                            if (i > 0 && ReferenceNo[i - 1] != ReferenceNo[i])
                            {
                                hasOrderLines = true;
                                saved = true;
                                string RefNo = ReferenceNo[i];
                                _sqlQuery.Clear();
                                if (RefNo != "")
                                {
                                    if (WindowID == Util.GetValueOfInt(Windows.Shipment))
                                        _sqlQuery.Append("SELECT VAB_Order_ID FROM VAB_Order WHERE IsActive = 'Y' AND IsSOTrx= 'Y' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID()
                                            + " AND DocumentNo = '" + RefNo + "'");
                                    else if (WindowID == Util.GetValueOfInt(Windows.MaterialReceipt))
                                        _sqlQuery.Append("SELECT VAB_Order_ID FROM VAB_Order WHERE IsActive = 'Y' AND IsSOTrx= 'N' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID()
                                            + " AND DocumentNo = '" + RefNo + "'");
                                    else
                                        _sqlQuery.Append("SELECT VAB_Order_ID FROM VAB_Order WHERE IsActive = 'Y' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                                }
                                else if (WindowID == Util.GetValueOfInt(Windows.Shipment))
                                    _sqlQuery.Append("SELECT VAB_Order_ID FROM VAM_Inv_InOut WHERE IsActive = 'Y' AND IsSOTrx = 'Y' AND VAM_Inv_InOut_ID = " + recordID);

                                if (_sqlQuery.Length > 0)
                                    ordID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

                                if (ordID > 0)
                                {
                                    string selColumn = "";
                                    if (Env.IsModuleInstalled("DTD001_"))
                                        selColumn = " , ol.DTD001_Org_ID ";

                                    _sqlQuery.Clear();
                                    _sqlQuery.Append("SELECT ol.VAB_OrderLine_ID, ol.VAM_Product_ID, ol.VAM_PFeature_SetInstance_ID, ol.VAB_UOM_ID " + selColumn
                                        + " FROM VAB_OrderLine ol WHERE ol.VAB_Order_ID = " + ordID);
                                    dsOrderLines = DB.ExecuteDataset(_sqlQuery.ToString());
                                    if (dsOrderLines != null && dsOrderLines.Tables[0].Rows.Count > 0)
                                        hasOrderLines = true;
                                }

                                if (ordID > 0)
                                {
                                    io.SetVAB_Order_ID(ordID);
                                    if (!io.Save())
                                        saved = false;
                                }
                            }

                            po = tbl.GetPO(ctx, lineID, null);
                            po.Set_ValueNoCheck("VAF_Client_ID", io.GetVAF_Client_ID());
                            po.Set_ValueNoCheck("VAF_Org_ID", io.GetVAF_Org_ID());
                            po.Set_Value("VAM_Product_ID", Util.GetValueOfInt(product[i]));
                            po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                            po.Set_Value("MovementQty", Util.GetValueOfDecimal(qty[i]));
                            po.Set_ValueNoCheck("VAM_Inv_InOut_ID", recordID);
                            if (hasOrderLines)
                            {
                                DataRow[] drOL = null;
                                if (WindowID == Util.GetValueOfInt(Windows.MaterialReceipt))
                                {
                                    if (Env.IsModuleInstalled("DTD001_"))
                                        drOL = dsOrderLines.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(attribute[i])
                                            + "AND VAB_UOM_ID = " + Util.GetValueOfInt(uoms[i]) + " AND DTD001_Org_ID = " + ctx.GetVAF_Org_ID());
                                }
                                if (!(drOL != null && drOL.Length > 0))
                                {
                                    drOL = dsOrderLines.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(attribute[i])
                                        + "AND VAB_UOM_ID = " + Util.GetValueOfInt(uoms[i]));
                                    if (!(drOL != null && drOL.Length > 0))
                                        drOL = dsOrderLines.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(attribute[i])
                                        + "AND VAB_UOM_ID <> " + Util.GetValueOfInt(uoms[i]));
                                    if (!(drOL != null && drOL.Length > 0))
                                        drOL = dsOrderLines.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(attribute[i]));
                                    if (!(drOL != null && drOL.Length > 0))
                                        drOL = dsOrderLines.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]));
                                }
                                if (drOL != null && drOL.Length > 0)
                                    po.Set_ValueNoCheck("VAB_OrderLine_ID", Util.GetValueOfInt(drOL[0]["VAB_OrderLine_ID"]));
                            }
                            po.Set_Value("VAM_Locator_ID", Util.GetValueOfInt(Locator_ID));
                            if (Util.GetValueOfInt(attribute[i]) != 0)
                                po.Set_Value("VAM_PFeature_SetInstance_ID", Util.GetValueOfInt(attribute[i]));

                            if (!io.IsSOTrx())
                            {
                                if (dsProPO == null && !fetchedProPurchasing)
                                {
                                    dsProPO = GetPurchaingProduct(ctx.GetVAF_Client_ID());
                                    fetchedProPurchasing = true;
                                    if (dsProPO != null && dsProPO.Tables[0].Rows.Count > 0)
                                        hasProdsPurch = true;
                                }

                                int uomID = Util.GetValueOfInt(uoms[i]);
                                int uom = 0;
                                if (hasProdsPurch)
                                {
                                    DataRow[] dr = dsProPO.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAB_BusinessPartner_ID = " + io.GetVAB_BusinessPartner_ID());
                                    if (dr != null && dr.Length > 0)
                                        uom = Util.GetValueOfInt(dr[0]["VAB_UOM_ID"]);
                                }

                                if (uomID != 0)
                                {
                                    if (uomID != uom && uom != 0)
                                    {
                                        if (!fetchedUOMConv)
                                        {
                                            dsUOMConv = GetUOMConversions(ctx.GetVAF_Client_ID());
                                            fetchedUOMConv = true;
                                            if (dsUOMConv != null && dsUOMConv.Tables[0].Rows.Count > 0)
                                                hasConversions = true;
                                        }

                                        if (hasConversions)
                                        {
                                            Decimal? Res = 0;
                                            DataRow[] drConv = dsUOMConv.Tables[0].Select(" VAB_UOM_ID = " + uomID + " AND VAB_UOM_To_ID = " + uom + " AND VAM_Product_ID= " + Util.GetValueOfInt(product[i]));
                                            if (drConv != null && drConv.Length > 0)
                                            {
                                                Res = Util.GetValueOfDecimal(drConv[0]["MultiplyRate"]);
                                                if (Res <= 0)
                                                {
                                                    drConv = dsUOMConv.Tables[0].Select(" VAB_UOM_ID = " + uomID + " AND VAB_UOM_To_ID = " + uom);
                                                    if (drConv != null && drConv.Length > 0)
                                                        Res = Util.GetValueOfDecimal(drConv[0]["MultiplyRate"]);
                                                }
                                            }

                                            if (Res > 0)
                                                po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]) * Res);
                                        }
                                        po.Set_ValueNoCheck("VAB_UOM_ID", uom);
                                    }
                                    else
                                        po.Set_ValueNoCheck("VAB_UOM_ID", uomID);
                                }
                            }
                            else
                                po.Set_ValueNoCheck("VAB_UOM_ID", Util.GetValueOfInt(uoms[i]));

                            if (po.Get_ColumnIndex("VAM_ProductContainer_ID") > 0 && ContainerID > 0)
                                po.Set_Value("VAM_ProductContainer_ID", ContainerID);

                            if (!po.Save())
                            {
                                count++;
                                SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                            }
                        }
                    }
                }
            }
            #endregion MR/Shipment

            #region Package
            else if (keyColName.ToUpper().Trim() == "VAM_Packaging_ID")
            {
                DataSet dsMoves = null;
                bool hasMoveLines = false;

                tbl = new MVAFTableView(ctx, 663, null);

                MPackage pkg = new MPackage(ctx, recordID, null);
                string RefNo = ReferenceNo[0];
                if (Util.GetValueOfString(RefNo) != "")
                {
                    _sqlQuery.Clear();
                    _sqlQuery.Append(@"SELECT ol.VAM_InvTrf_Line_ID, ol.VAM_Product_ID, ol.MovementQty FROM VAM_InvTrf_Line ol INNER JOIN VAM_InventoryTransfer o ON ol.VAM_InventoryTransfer_ID=o.VAM_InventoryTransfer_ID
                                        WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.VAM_InvTrf_Line_ID NOT IN
                                      (SELECT NVL(VAM_InvTrf_Line_ID,0) FROM VAM_PackagingLine WHERE VAM_Packaging_ID = " + recordID + ")");

                    if (Util.GetValueOfString(RefNo) != "")
                    {
                        dsMoves = DB.ExecuteDataset(_sqlQuery.ToString());
                        if (dsMoves != null && dsMoves.Tables[0].Rows.Count > 0)
                            hasMoveLines = true;
                    }
                }

                StringBuilder sbLine = new StringBuilder("");
                StringBuilder sbWhereCond = new StringBuilder("");

                for (int i = 0; i < product.Count; i++)
                {
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("VAF_Client_ID", pkg.GetVAF_Client_ID());
                    po.Set_ValueNoCheck("VAF_Org_ID", pkg.GetVAF_Org_ID());
                    po.Set_Value("VAM_Product_ID", Util.GetValueOfInt(product[i]));
                    po.Set_Value("Qty", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("VAM_Packaging_ID", recordID);

                    if (hasMoveLines)
                    {
                        if (sbLine.Length > 0)
                        {
                            sbWhereCond.Clear();
                            sbWhereCond.Append(" AND VAM_InvTrf_Line_ID NOT IN ( " + sbLine + " ) ");
                        }
                        DataRow[] dr = dsMoves.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
                        if (dr != null && dr.Length > 0)
                        {
                            int MoveLineID = Util.GetValueOfInt(dr[0]["VAM_InvTrf_Line_ID"]);
                            if (MoveLineID > 0)
                            {
                                if (sbLine.Length > 0)
                                    sbLine.Append(", " + dr[0]["VAM_InvTrf_Line_ID"]);
                                else
                                    sbLine.Append(dr[0]["VAM_InvTrf_Line_ID"]);
                                po.Set_Value("VAM_InvTrf_Line_ID", MoveLineID);
                                po.Set_Value("DTD001_TotalQty", Util.GetValueOfInt(dr[0]["MovementQty"]));
                            }
                        }
                    }
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        po.Set_Value("VAM_PFeature_SetInstance_ID", Util.GetValueOfInt(attribute[i]));
                    }
                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                    }
                }
            }
            #endregion Package

            #region Inventory
            else if (keyColName.ToUpper().Trim() == "VAM_Inventory_ID")
            {
                tbl = new MVAFTableView(ctx, 322, null);
                MInventory inv = new MInventory(ctx, recordID, null);

                if (Locator_ID <= 0)
                {
                    Locator_ID = hasDefaultLocator(inv.GetVAM_Warehouse_ID());
                    if (Locator_ID <= 0)
                    {
                        error = Msg.GetMsg(ctx, "DefaultLocatorNotFound");
                    }
                }

                // check applied for Locator 
                if (Locator_ID > 0)
                {
                    #region Internal Use Inventory
                    DataSet dsReqs = null;
                    bool hasReqLines = false;
                    int Asset_ID = 0;
                    #endregion Internal Use Inventory

                    #region Physical Inventory
                    bool hasInvLines = false;
                    bool hasStock = false;
                    DataSet dsStockedQty = null;
                    DataSet dsInvLine = DB.ExecuteDataset("SELECT VAM_InventoryLine_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID FROM VAM_InventoryLine WHERE IsActive ='Y' AND VAF_Client_ID = " + ctx.GetVAF_Client_ID() + " AND VAM_Inventory_ID = " + recordID);
                    if (dsInvLine != null && dsInvLine.Tables[0].Rows.Count > 0)
                        hasInvLines = true;
                    #endregion Physical Inventory

                    // for Physical Inventory Window
                    int RefLocatorID = 0;
                    string RefNo = ReferenceNo[0];
                    if (WindowID == Util.GetValueOfInt(Windows.PhysicalInventory))
                    {
                        _sqlQuery.Clear();
                        if (RefNo != "")
                        {
                            _sqlQuery.Append("SELECT VAM_Locator_ID FROM VAM_Locator WHERE IsActive = 'Y' AND VAF_Client_ID = " + ctx.GetVAF_Client_ID() + " AND Value = '" + RefNo + "'");
                            RefLocatorID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));
                        }

                        if (RefLocatorID > 0)
                            Locator_ID = RefLocatorID;

                        dsStockedQty = DB.ExecuteDataset("SELECT QtyOnHand, VAM_Product_ID, VAM_PFeature_SetInstance_ID FROM VAM_Storage WHERE VAM_Locator_ID = " + Locator_ID + " AND VAF_Client_ID = " + ctx.GetVAF_Client_ID());
                        if (dsStockedQty != null && dsStockedQty.Tables[0].Rows.Count > 0)
                            hasStock = true;
                    }
                    // For Internal Use Inventory Window
                    else if (WindowID == Util.GetValueOfInt(Windows.InternalUse))
                    {
                        _sqlQuery.Clear();
                        _sqlQuery.Append("SELECT VAB_Charge_ID FROM VAB_Charge WHERE IsActive = 'Y' AND VAF_Client_ID = " + ctx.GetVAF_Client_ID() + " AND DTD001_ChargeType = 'INV'");
                        Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

                        if (Util.GetValueOfString(RefNo) != "")
                        {
                            _sqlQuery.Clear();
                            _sqlQuery.Append(@"SELECT ol.VAM_RequisitionLine_ID,  ol.VAM_Product_ID FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition o
                                    ON ol.VAM_Requisition_ID =o.VAM_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.VAM_RequisitionLine_ID NOT IN
                                      (SELECT NVL(VAM_RequisitionLine_ID,0) FROM VAM_InventoryLine WHERE VAM_Inventory_ID = " + recordID + ")");

                            if (Util.GetValueOfString(RefNo) != "")
                            {
                                dsReqs = DB.ExecuteDataset(_sqlQuery.ToString());
                                if (dsReqs != null && dsReqs.Tables[0].Rows.Count > 0)
                                    hasReqLines = true;
                            }
                        }
                    }

                    StringBuilder sbLine = new StringBuilder("");
                    StringBuilder sbWhereCond = new StringBuilder("");
                    for (int i = 0; i < product.Count; i++)
                    {
                        lineID = 0;
                        // look for Inventory Line ID only in case of Physical Inventory Window
                        if (WindowID == Util.GetValueOfInt(Windows.PhysicalInventory) && hasInvLines)
                        {
                            DataRow[] drInve = dsInvLine.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(attribute[i]));
                            if (drInve != null && drInve.Length > 0)
                                lineID = Util.GetValueOfInt(drInve[0]["VAM_InventoryLine_ID"]);
                        }

                        po = tbl.GetPO(ctx, lineID, null);
                        if (lineID > 0)
                        {
                            po.Set_Value("QtyCount", (Util.GetValueOfDecimal(po.Get_Value("QtyCount")) + Util.GetValueOfDecimal(qty[i])));
                            po.Set_Value("AsOnDateCount", po.Get_Value("QtyCount"));
                            po.Set_Value("QtyEntered", po.Get_Value("QtyCount"));
                        }
                        else
                        {
                            po.Set_ValueNoCheck("VAF_Client_ID", inv.GetVAF_Client_ID());
                            po.Set_ValueNoCheck("VAF_Org_ID", inv.GetVAF_Org_ID());
                            if (Util.GetValueOfInt(Locator_ID) > 0)
                                po.Set_Value("VAM_Locator_ID", Util.GetValueOfInt(Locator_ID));
                            else
                                po.Set_Value("VAM_Locator_ID", Locator_ID);
                            po.Set_Value("VAM_Product_ID", Util.GetValueOfInt(product[i]));
                            po.Set_ValueNoCheck("VAM_Inventory_ID", recordID);
                            if (Util.GetValueOfInt(attribute[i]) != 0)
                                po.Set_Value("VAM_PFeature_SetInstance_ID", Util.GetValueOfInt(attribute[i]));
                            else
                                po.Set_Value("VAM_PFeature_SetInstance_ID", 0);
                            // when column index found, then insert UOM on requisition line
                            if (po.Get_ColumnIndex("VAB_UOM_ID") > 0)
                                po.Set_Value("VAB_UOM_ID", Util.GetValueOfInt(uoms[i]));
                            if (po.Get_ColumnIndex("VAM_ProductContainer_ID") > 0 && ContainerID > 0)
                                po.Set_Value("VAM_ProductContainer_ID", ContainerID);
                            po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));

                            // JID_1700: when saving Product from Cart, UOM Conversion was not working 
                            po.Set_Value("AdjustmentType", MInventoryLine.ADJUSTMENTTYPE_AsOnDateCount);

                            if (WindowID == Util.GetValueOfInt(Windows.PhysicalInventory))
                            {
                                Decimal? qtyBook = 0;
                                if (hasStock)
                                {
                                    DataRow[] drStock = dsStockedQty.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(attribute[i]));
                                    if (drStock != null && drStock.Length > 0)
                                        qtyBook = Util.GetValueOfDecimal(drStock[0]["QtyOnHand"]);
                                }
                                po.Set_Value("QtyCount", Util.GetValueOfDecimal(qty[i]));
                                po.Set_ValueNoCheck("QtyBook", qtyBook);
                                po.Set_Value("AsOnDateCount", Util.GetValueOfDecimal(qty[i]));
                                po.Set_Value("OpeningStock", qtyBook);
                                if (po.Get_ColumnIndex("VAICNT_InventoryCountLine_ID") > 0 && Util.GetValueOfInt(InvCountID[i]) > 0)
                                    po.Set_Value("VAICNT_InventoryCountLine_ID", Util.GetValueOfInt(InvCountID[i]));
                            }
                            else if (WindowID == Util.GetValueOfInt(Windows.InternalUse))
                            {
                                if (i > 0 && ReferenceNo[i - 1] != ReferenceNo[i])
                                {
                                    hasReqLines = false;
                                    RefNo = ReferenceNo[i];
                                    if (Util.GetValueOfString(RefNo) != "")
                                    {
                                        _sqlQuery.Clear();
                                        _sqlQuery.Append(@"SELECT ol.VAM_RequisitionLine_ID,  ol.VAM_Product_ID FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition o
                                    ON ol.VAM_Requisition_ID =o.VAM_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.VAM_RequisitionLine_ID NOT IN
                                      (SELECT NVL(VAM_RequisitionLine_ID,0) FROM VAM_InventoryLine WHERE VAM_Inventory_ID = " + recordID + ")");

                                        if (Util.GetValueOfString(RefNo) != "")
                                        {
                                            dsReqs = DB.ExecuteDataset(_sqlQuery.ToString());
                                            if (dsReqs != null && dsReqs.Tables[0].Rows.Count > 0)
                                                hasReqLines = true;
                                        }
                                    }
                                }

                                po.Set_Value("QtyInternalUse", Util.GetValueOfDecimal(qty[i]));
                                if (Asset_ID > 0)
                                    po.Set_Value("VAB_Charge_ID", Asset_ID);
                                po.Set_Value("IsInternalUse", true);
                                if (hasReqLines)
                                {
                                    if (sbLine.Length > 0)
                                    {
                                        sbWhereCond.Clear();
                                        sbWhereCond.Append(" AND VAM_RequisitionLine_ID NOT IN ( " + sbLine + " ) ");
                                    }
                                    DataRow[] dr = dsReqs.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
                                    if (dr != null && dr.Length > 0)
                                    {
                                        int ReqLineID = Util.GetValueOfInt(dr[0]["VAM_RequisitionLine_ID"]);
                                        if (ReqLineID > 0)
                                        {
                                            if (sbLine.Length > 0)
                                                sbLine.Append(", " + dr[0]["VAM_RequisitionLine_ID"]);
                                            else
                                                sbLine.Append(dr[0]["VAM_RequisitionLine_ID"]);
                                            po.Set_Value("VAM_RequisitionLine_ID", ReqLineID);
                                        }
                                    }
                                }
                            }
                        }
                        if (!po.Save())
                        {
                            count++;
                            SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                        }
                    }
                }
            }
            #endregion Inventory

            #region Project
            else if (keyColName.ToUpper().Trim() == "VAB_PROJECT_ID")
            {
                tbl = new MVAFTableView(ctx, 434, null);
                MProject proj = new MProject(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    int _VAM_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = 0;
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("VAF_Client_ID", VAF_Client_ID);
                    po.Set_ValueNoCheck("VAF_Org_ID", proj.GetVAF_Org_ID());
                    po.Set_Value("VAM_Product_ID", _VAM_Product_ID);
                    po.Set_Value("PLANNEDQTY", Util.GetValueOfDecimal(qty[i]));
                    po.Set_Value("INVOICEDQTY", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("VAB_Project_ID", recordID);

                    Dictionary<string, Decimal?> pPrice = GetPrices(dsProPrice, dsUOMConv, VAF_Client_ID, _VAM_Product_ID,
                        _attribute_ID, Util.GetValueOfInt(po.Get_Value("VAB_UOM_ID")), dsProducts,
                        Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), VAM_DiscountCalculation_ID, bpFlatDiscount);

                    po.Set_Value("PriceList", pPrice["PriceList"]);
                    po.Set_Value("PlannedPrice", pPrice["PriceEntered"]);
                    var discount = ((pPrice["PriceList"] - pPrice["PriceEntered"]) * 100) / pPrice["PriceList"];

                    po.Set_Value("Discount", discount);
                    // oppLine.SetDiscount(Decimal.Subtract(PriceList ,PriceStd));
                    po.Set_Value("PlannedMarginAmt", (pPrice["PriceEntered"] - pPrice["PriceLimit"]));

                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                    }
                }
            }
            #endregion Project

            #region PriceList
            else if (keyColName.ToUpper().Trim() == "VAM_PriceList_ID")
            {
                for (int i = 0; i < product.Count; i++)
                {
                    int _VAM_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = Util.GetValueOfInt(attribute[i]);
                    int _uom_ID = Util.GetValueOfInt(uoms[i]);
                    MProductPrice pr = null;
                    string sql = "SELECT * FROM VAM_ProductPrice WHERE VAM_PriceListVersion_ID=" + recordID + " AND VAM_Product_ID=" + _VAM_Product_ID;
                    if (Env.IsModuleInstalled("VAPRC_"))
                    {
                        sql += " AND VAM_PFeature_SetInstance_ID = " + _attribute_ID;
                    }
                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        sql += " AND VAB_UOM_ID = " + _uom_ID;
                    }
                    DataSet ds = new DataSet();
                    try
                    {
                        ds = DB.ExecuteDataset(sql, null, null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            pr = new MProductPrice(ctx, dr, null);
                        }
                        ds = null;
                    }
                    catch (Exception e)
                    {

                    }

                    if (pr == null)
                    {
                        pr = new MProductPrice(ctx, recordID, _VAM_Product_ID, null);
                        if (Env.IsModuleInstalled("VAPRC_"))
                        {
                            pr.SetVAM_PFeature_SetInstance_ID(_attribute_ID);
                        }
                        if (Env.IsModuleInstalled("ED011_"))
                        {
                            pr.SetVAB_UOM_ID(_uom_ID);
                        }
                        pr.SetPriceLimit(0);
                        pr.SetPriceList(0);
                        pr.SetPriceStd(0);
                        if (!pr.Save())
                        {
                            count++;
                            SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                        }
                    }
                }
            }
            #endregion PriceList

            #region Requisition
            else if (keyColName.ToUpper().Trim() == "VAM_Requisition_ID")
            {
                tbl = new MVAFTableView(ctx, 703, null);
                MRequisition inv = new MRequisition(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("VAF_Client_ID", VAF_Client_ID);
                    po.Set_ValueNoCheck("VAF_Org_ID", VAF_Org_ID);
                    po.Set_Value("VAM_Product_ID", Util.GetValueOfInt(product[i]));
                    // check if new column found on Requisition Line, added in framework on 7 Dec 2018
                    if (po.Get_ColumnIndex("QtyEntered") > 0)
                    {
                        po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                        po.Set_Value("Qty", Util.GetValueOfDecimal(qty[i]));
                    }
                    else
                        po.Set_Value("Qty", Util.GetValueOfDecimal(qty[i]));

                    //if (Util.GetValueOfInt(oline_ID[i]) > 0)
                    //{
                    //    po.Set_Value("VAB_OrderLineLine_ID", Util.GetValueOfInt(oline_ID[i]));
                    //}
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        po.Set_Value("VAM_PFeature_SetInstance_ID", Util.GetValueOfInt(attribute[i]));
                    }
                    else
                    {
                        po.Set_Value("VAM_PFeature_SetInstance_ID", 0);
                    }
                    // when column index found, then insert UOM on requisition line
                    if (po.Get_ColumnIndex("VAB_UOM_ID") > 0)
                    {
                        po.Set_ValueNoCheck("VAB_UOM_ID", Util.GetValueOfInt(uoms[i]));
                    }
                    po.Set_ValueNoCheck("VAM_Requisition_ID", recordID);

                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        Dictionary<string, Decimal?> pPrice = GetPrices(dsProPrice, dsUOMConv, VAF_Client_ID, Util.GetValueOfInt(product[i]),
                            Util.GetValueOfInt(attribute[i]), Util.GetValueOfInt(po.Get_Value("VAB_UOM_ID")), dsProducts,
                            Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), VAM_DiscountCalculation_ID, bpFlatDiscount);

                        //po.Set_Value("PriceList", pPrice["PriceList"]);
                        //po.Set_Value("PriceLimit", pPrice["PriceLimit"]);
                        if (po.Get_ColumnIndex("PriceActual") > 0)
                            po.Set_ValueNoCheck("PriceActual", pPrice["PriceActual"]);
                        //po.Set_Value("PriceEntered", pPrice["PriceEntered"]);
                    }

                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                    }
                }
            }
            #endregion Requisition

            #region Inventory Move
            if (keyColName.ToUpper().Trim() == "VAM_InventoryTransfer_ID")
            {
                DataSet dsReqs = null;
                bool hasReqLines = false;

                DataSet dsAssets = null;
                bool hasAssets = false;

                tbl = new MVAFTableView(ctx, 324, null);
                MMovement mov = new MMovement(ctx, recordID, null);

                if (Locator_ID <= 0)
                    error = Msg.GetMsg(ctx, "DefaultLocatorNotFound");

                if (LocToID <= 0)
                    error = Msg.GetMsg(ctx, "LocatorToNotFound");

                string RefNo = ReferenceNo[0];

                // check applied for Locator 
                if (Locator_ID > 0 && LocToID > 0)
                {
                    if (Util.GetValueOfString(RefNo) != "")
                    {
                        _sqlQuery.Clear();
                        _sqlQuery.Append("SELECT VAA_Asset_ID, VAM_Product_ID, NVL(VAM_PFeature_SetInstance_ID,0) AS VAM_PFeature_SetInstance_ID FROM VAA_Asset WHERE IsActive = 'Y' AND VAF_Client_ID = "
                            + ctx.GetVAF_Client_ID());
                        dsAssets = DB.ExecuteDataset(_sqlQuery.ToString());
                        if (dsAssets != null && dsAssets.Tables[0].Rows.Count > 0)
                            hasAssets = true;
                    }

                    VAF_Client_ID = mov.GetVAF_Client_ID();

                    if (Util.GetValueOfString(RefNo) != "")
                    {
                        _sqlQuery.Clear();
                        _sqlQuery.Append(@"SELECT ol.VAM_RequisitionLine_ID, ol.VAM_Product_ID FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition o
                                    ON ol.VAM_Requisition_ID =o.VAM_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.VAM_RequisitionLine_ID NOT IN
                                      (SELECT NVL(VAM_RequisitionLine_ID,0) FROM VAM_InvTrf_Line WHERE VAM_InventoryTransfer_ID = " + recordID + ")");

                        if (Util.GetValueOfString(RefNo) != "")
                        {
                            dsReqs = DB.ExecuteDataset(_sqlQuery.ToString());
                            if (dsReqs != null && dsReqs.Tables[0].Rows.Count > 0)
                                hasReqLines = true;
                        }
                    }

                    StringBuilder sbLine = new StringBuilder("");
                    StringBuilder sbWhereCond = new StringBuilder("");

                    for (int i = 0; i < product.Count; i++)
                    {
                        if (i > 0 && ReferenceNo[i - 1] != ReferenceNo[i])
                        {
                            hasReqLines = false;
                            RefNo = ReferenceNo[i];
                            if (Util.GetValueOfString(RefNo) != "")
                            {
                                _sqlQuery.Clear();
                                _sqlQuery.Append(@"SELECT ol.VAM_RequisitionLine_ID, ol.VAM_Product_ID FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition o
                                    ON ol.VAM_Requisition_ID =o.VAM_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.VAM_RequisitionLine_ID NOT IN
                                      (SELECT NVL(VAM_RequisitionLine_ID,0) FROM VAM_InvTrf_Line WHERE VAM_InventoryTransfer_ID = " + recordID + ")");

                                dsReqs = DB.ExecuteDataset(_sqlQuery.ToString());
                                if (dsReqs != null && dsReqs.Tables[0].Rows.Count > 0)
                                    hasReqLines = true;
                            }
                        }


                        po = tbl.GetPO(ctx, lineID, null);

                        int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(attribute[i]);
                        int VAM_Product_ID = Util.GetValueOfInt(product[i]);

                        po.Set_ValueNoCheck("VAF_Client_ID", VAF_Client_ID);
                        po.Set_ValueNoCheck("VAF_Org_ID", mov.GetVAF_Org_ID());
                        po.Set_Value("VAM_Locator_ID", Util.GetValueOfInt(Locator_ID));
                        po.Set_Value("VAM_LocatorTo_ID", LocToID);
                        po.Set_Value("VAM_Product_ID", VAM_Product_ID);
                        po.Set_Value("MovementQty", Util.GetValueOfDecimal(qty[i]));
                        po.Set_ValueNoCheck("VAM_InventoryTransfer_ID", recordID);
                        if (po.Get_ColumnIndex("QtyEntered") > 0)
                            po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                        if (po.Get_ColumnIndex("VAB_UOM_ID") > 0)
                            po.Set_Value("VAB_UOM_ID", Util.GetValueOfInt(uoms[i]));
                        if (hasReqLines)
                        {
                            if (sbLine.Length > 0)
                            {
                                sbWhereCond.Clear();
                                sbWhereCond.Append(" AND VAM_RequisitionLine_ID NOT IN ( " + sbLine + " ) ");
                            }
                            DataRow[] dr = dsReqs.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
                            if (dr != null && dr.Length > 0)
                            {
                                int ReqLineID = Util.GetValueOfInt(dr[0]["VAM_RequisitionLine_ID"]);
                                if (ReqLineID > 0)
                                {
                                    if (sbLine.Length > 0)
                                        sbLine.Append(", " + dr[0]["VAM_RequisitionLine_ID"]);
                                    else
                                        sbLine.Append(dr[0]["VAM_RequisitionLine_ID"]);
                                    po.Set_Value("VAM_RequisitionLine_ID", ReqLineID);
                                }
                            }
                        }
                        if (VAM_PFeature_SetInstance_ID != 0)
                        {
                            po.Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
                            if (hasAssets)
                            {
                                DataRow[] drAst = dsAssets.Tables[0].Select(" VAM_Product_ID = " + VAM_Product_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID);
                                if (drAst != null && drAst.Length > 0)
                                {
                                    if (Util.GetValueOfInt(drAst[0]["VAA_Asset_ID"]) > 0)
                                        po.Set_Value("VAA_Asset_ID", Util.GetValueOfInt(drAst[0]["VAA_Asset_ID"]));
                                }
                            }
                        }

                        if (po.Get_ColumnIndex("VAM_ProductContainer_ID") > 0 && ContainerID > 0)
                            po.Set_Value("VAM_ProductContainer_ID", ContainerID);

                        if (po.Get_ColumnIndex("Ref_VAM_ProductContainerTo_ID") > 0 && ContainerToID > 0)
                            po.Set_Value("Ref_VAM_ProductContainerTo_ID", ContainerToID);

                        if (!po.Save())
                        {
                            count++;
                            SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, Util.GetValueOfInt(product[i]), prodName);
                        }
                    }
                }
            }
            #endregion Inventory Move

            #region Dispose Datasets
            if (dsProPO != null)
            {
                dsProPO.Dispose();
                dsProPO = null;
            }

            if (dsProPrice != null)
            {
                dsProPrice.Dispose();
                dsProPrice = null;
            }

            if (dsUOMConv != null)
            {
                dsUOMConv.Dispose();
                dsUOMConv = null;
            }

            if (dsProducts != null)
            {
                dsProducts.Dispose();
                dsProducts = null;
            }
            #endregion Dispose Datasets

            info.count = count;
            info.error = error;
            info.errorKeys = errorKeys;
            info.errorProds = errorProdLines;
            return info;
        }

        /// <summary>
        /// Function to set Error Messages in the list 
        /// if same key already found then only add products in the values against the keys in separte list
        /// </summary>
        /// <param name="errorKeys"></param>
        /// <param name="errorProdLines"></param>
        /// <param name="hasProducts"></param>
        /// <param name="dsProducts"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="prodName"></param>
        private void SetErrorMessages(List<string> errorKeys, List<string> errorProdLines, bool hasProducts, DataSet dsProducts, int VAM_Product_ID, StringBuilder prodName)
        {
            ValueNamePair pp = VLogger.RetrieveError();
            prodName.Clear();
            if (hasProducts)
            {
                DataRow[] drPName = dsProducts.Tables[0].Select("VAM_Product_ID = " + VAM_Product_ID);
                if (drPName != null && drPName.Length > 0)
                    prodName.Append(Util.GetValueOfString(drPName[0]["Name"]));
            }
            if (errorKeys.Contains(pp.ToString()))
            {
                int index = errorKeys.IndexOf(pp.ToString());
                errorProdLines[index] = errorProdLines[index] + ", " + prodName.ToString();
            }
            else
            {
                errorKeys.Add(pp.ToString());
                errorProdLines.Add(prodName.ToString());
            }
        }

        /// <summary>
        /// Set prices after checking the columns in the tables, where we are saving the records
        /// </summary>
        /// <param name="po"></param>
        /// <param name="pPrice"></param>
        private void SetPrices(PO po, Dictionary<string, decimal?> pPrice)
        {
            // check for the columns whether exist on table, then set values in respective columns
            if (po.Get_ColumnIndex("PriceList") > 0)
                po.Set_Value("PriceList", pPrice["PriceList"]);

            if (po.Get_ColumnIndex("PriceLimit") > 0)
                po.Set_Value("PriceLimit", pPrice["PriceLimit"]);

            if (po.Get_ColumnIndex("PriceActual") > 0)
                po.Set_ValueNoCheck("PriceActual", pPrice["PriceActual"]);

            if (po.Get_ColumnIndex("PriceEntered") > 0)
                po.Set_Value("PriceEntered", pPrice["PriceEntered"]);
        }

        //        public InfoSave SetProductQty1(int recordID, string keyColName, List<string> product, List<string> uom, List<string> attribute,
        //            List<string> qty, int LocToID, int lineID, string RefNo,
        //            int Locator_ID, int WindowID, int ContainerID, int ContainerToID, VAdvantage.Utility.Ctx ctx)
        //        {
        //            InfoSave info = new InfoSave();
        //            string error = "";
        //            MVAFTableView tbl = null;
        //            PO po = null;
        //            int count = 0;

        //            DataSet dsReqs = null;
        //            bool hasReqLines = false;

        //            #region Internal Use Inventory
        //            if (keyColName.ToUpper().Trim() == "VAM_Inventory_ID")
        //            {
        //                _sqlQuery.Clear();
        //                _sqlQuery.Append("SELECT VAB_Charge_ID FROM VAB_Charge WHERE IsActive = 'Y' AND VAF_Client_ID = " + ctx.GetVAF_Client_ID() + " AND DTD001_ChargeType = 'INV'");

        //                int Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

        //                tbl = new MVAFTableView(ctx, 322, null);
        //                MInventory inv = new MInventory(ctx, recordID, null);

        //                if (Locator_ID <= 0)
        //                {
        //                    Locator_ID = hasDefaultLocator(inv.GetVAM_Warehouse_ID());
        //                    if (Locator_ID <= 0)
        //                    {
        //                        error = Msg.GetMsg(ctx, "DefaultLocatorNotFound");
        //                    }
        //                }
        //                // check applied for Locator 
        //                if (Locator_ID > 0)
        //                {
        //                    if (Util.GetValueOfString(RefNo) != "")
        //                    {
        //                        _sqlQuery.Clear();
        //                        _sqlQuery.Append(@"SELECT ol.VAM_RequisitionLine_ID,  ol.VAM_Product_ID FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition o
        //                                    ON ol.VAM_Requisition_ID =o.VAM_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.VAM_RequisitionLine_ID NOT IN
        //                                      (SELECT NVL(VAM_RequisitionLine_ID,0) FROM VAM_InventoryLine WHERE VAM_Inventory_ID = " + recordID + ")");

        //                        if (Util.GetValueOfString(RefNo) != "")
        //                        {
        //                            dsReqs = DB.ExecuteDataset(_sqlQuery.ToString());
        //                            if (dsReqs != null && dsReqs.Tables[0].Rows.Count > 0)
        //                                hasReqLines = true;
        //                        }
        //                    }

        //                    StringBuilder sbLine = new StringBuilder("");
        //                    StringBuilder sbWhereCond = new StringBuilder("");
        //                    for (int i = 0; i < product.Count; i++)
        //                    {
        //                        po = tbl.GetPO(ctx, lineID, null);
        //                        po.Set_ValueNoCheck("VAF_Client_ID", inv.GetVAF_Client_ID());
        //                        po.Set_ValueNoCheck("VAF_Org_ID", inv.GetVAF_Org_ID());
        //                        po.Set_Value("VAM_Product_ID", Util.GetValueOfInt(product[i]));
        //                        po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
        //                        po.Set_Value("QtyInternalUse", Util.GetValueOfDecimal(qty[i]));
        //                        po.Set_Value("VAM_Locator_ID", Util.GetValueOfInt(Locator_ID));
        //                        if (Asset_ID > 0)
        //                            po.Set_Value("VAB_Charge_ID", Asset_ID);
        //                        po.Set_ValueNoCheck("VAM_Inventory_ID", recordID);
        //                        po.Set_Value("IsInternalUse", true);
        //                        po.Set_Value("VAB_UOM_ID", Util.GetValueOfInt(uom[i]));
        //                        if (hasReqLines)
        //                        {
        //                            if (sbLine.Length > 0)
        //                            {
        //                                sbWhereCond.Clear();
        //                                sbWhereCond.Append(" AND VAM_RequisitionLine_ID NOT IN ( " + sbLine + " ) ");
        //                            }
        //                            DataRow[] dr = dsReqs.Tables[0].Select(" VAM_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
        //                            if (dr != null && dr.Length > 0)
        //                            {
        //                                int ReqLineID = Util.GetValueOfInt(dr[0]["VAM_RequisitionLine_ID"]);
        //                                if (ReqLineID > 0)
        //                                {
        //                                    if (sbLine.Length > 0)
        //                                        sbLine.Append(", " + dr[0]["VAM_RequisitionLine_ID"]);
        //                                    else
        //                                        sbLine.Append(dr[0]["VAM_RequisitionLine_ID"]);
        //                                    po.Set_Value("VAM_RequisitionLine_ID", ReqLineID);
        //                                }
        //                            }
        //                        }
        //                        if (Util.GetValueOfInt(attribute[i]) != 0)
        //                            po.Set_Value("VAM_PFeature_SetInstance_ID", Util.GetValueOfInt(attribute[i]));
        //                        else
        //                            po.Set_Value("VAM_PFeature_SetInstance_ID", 0);

        //                        if (po.Get_ColumnIndex("VAM_ProductContainer_ID") > 0 && ContainerID > 0)
        //                            po.Set_Value("VAM_ProductContainer_ID", ContainerID);

        //                        if (!po.Save())
        //                        {
        //                            count++;
        //                            ValueNamePair pp = VLogger.RetrieveError();
        //                            error += pp.ToString() + "\n";
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion Internal Use Inventory

        //            info.count = count;
        //            info.error = error;
        //            return info;
        //        }

        public bool SetProductQtyStockTrasfer(int recordID, string keyColName, int VAF_TableView_ID, List<string> product, List<string> uom, List<string> attribute, List<string> qty, List<string> locID, int LocToID, int lineID, int ContainerID, VAdvantage.Utility.Ctx ctx)
        {
            MVAFTableView tbl = null;
            PO po = null;

            tbl = new MVAFTableView(ctx, VAF_TableView_ID, null);
            for (int i = 0; i < product.Count; i++)
            {
                po = tbl.GetPO(ctx, lineID, null);
                po.Set_ValueNoCheck("VAF_Client_ID", ctx.GetVAF_Client_ID());
                po.Set_ValueNoCheck("VAF_Org_ID", ctx.GetVAF_Org_ID());
                po.Set_Value("VAM_Locator_ID", Util.GetValueOfInt(locID[i]));
                po.Set_Value("VAM_LocatorTo_ID", LocToID);
                po.Set_Value("VAM_Product_ID", Util.GetValueOfInt(product[i]));
                po.Set_Value("MovementQty", Util.GetValueOfDecimal(qty[i]));
                po.Set_ValueNoCheck(keyColName, recordID);
                //if (Util.GetValueOfInt(AssetID[i]) > 0)
                //    po.Set_Value("VAA_Asset_ID", Util.GetValueOfInt(AssetID[i]));
                //if (Util.GetValueOfInt(oline_ID[i]) > 0)
                //    po.Set_Value("VAM_RequisitionLine_ID", Util.GetValueOfInt(oline_ID[i]));
                if (Util.GetValueOfInt(attribute[i]) != 0)
                    po.Set_Value("VAM_PFeature_SetInstance_ID", Util.GetValueOfInt(attribute[i]));
                string sql = "SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM SAP001_StockTransferLine WHERE SAP001_StockTransfer_ID=" + recordID;

                po.Set_Value("Line", Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));
                if (!po.Save())
                {

                }
            }

            return true;
        }

        public KeyNamePair GetAttribute(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_Product_ID = 0;
            string attributeNo = "", isLot = "", IsSerNo = "", IsGuaranteeDate = "", RefNo = "";
            DateTime? expiryDate = null;
            if (paramValue.Length > 0)
            {
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0]);
                attributeNo = Util.GetValueOfString(paramValue[1]);
                isLot = Util.GetValueOfString(paramValue[2]);
                IsSerNo = Util.GetValueOfString(paramValue[3]);
                IsGuaranteeDate = Util.GetValueOfString(paramValue[4]);
                expiryDate = Util.GetValueOfDateTime(paramValue[5]);
                RefNo = Util.GetValueOfString(paramValue[6]);
            }
            string qry = "";
            int attrID = 0;
            string name = "";
            KeyNamePair attribute = null;
            StringBuilder sql = new StringBuilder();
            MVAMPFeatureSetInstance _mast = MVAMPFeatureSetInstance.Get(Env.GetCtx(), 0, VAM_Product_ID);
            if (!string.IsNullOrEmpty(attributeNo))
            {
                qry = "SELECT VAM_PFeature_SetInstance_ID FROM VAM_ProductFeatures WHERE VAM_Product_ID = " + VAM_Product_ID + "AND UPC = '" + attributeNo + "'";
                attrID = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                if (attrID == 0)
                {
                    if (isLot == "Y")
                    {
                        _mast.SetLot(attributeNo);
                        sql.Append("UPPER(Lot) = " + attributeNo.ToUpper());
                    }
                    else if (IsSerNo == "Y")
                    {
                        _mast.SetSerNo(attributeNo);
                        sql.Append(" UPPER(SerNo) = " + attributeNo.ToUpper());
                    }
                    _mast.SetDescription(attributeNo);
                    if (IsGuaranteeDate == "Y")
                    {
                        if (sql.Length > 0)
                        {
                            sql.Append(" AND GuaranteeDate = " + GlobalVariable.TO_DATE(expiryDate, true));
                        }
                        else
                        {
                            sql.Append(" GuaranteeDate = " + GlobalVariable.TO_DATE(expiryDate, true));
                        }
                        _mast.SetGuaranteeDate(expiryDate);
                        if (!String.IsNullOrEmpty(attributeNo))
                        {
                            _mast.SetDescription(attributeNo + "_" + expiryDate);
                        }
                        else
                        {
                            _mast.SetDescription(expiryDate.ToString());
                        }
                    }
                    qry = @"SELECT VAM_PFeature_SetInstance_ID FROM VAM_PFeature_SetInstance";

                    if (sql.Length > 0)
                    {
                        sql.Insert(0, " where ");
                        qry += sql + " order by VAM_PFeature_SetInstance_id";
                    }
                    else
                    {
                        qry = "";
                    }
                    if (qry != "")
                    {
                        attrID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
                        if (attrID == 0)
                        {
                            if (_mast.Save())
                            {
                                attrID = _mast.GetVAM_PFeature_SetInstance_ID();
                                name = _mast.GetDescription();
                            }
                        }
                    }
                }
                if (attrID > 0)
                {
                    MVAMPFeatureSetInstance mas = new MVAMPFeatureSetInstance(ctx, attrID, null);
                    name = mas.GetDescription();
                }
                attribute = new KeyNamePair(attrID, name);
            }
            return attribute;
        }

        private decimal FlatDiscount(int ProductId, int ClientId, decimal amount, int DiscountSchemaId, Decimal? FlatDiscount, decimal? QtyEntered)
        {
            StringBuilder query = new StringBuilder();
            decimal amountAfterBreak = amount;
            query.Append(@"SELECT DISTINCT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive='Y' AND VAM_Product_ID = " + ProductId);
            int productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
            bool isCalulate = false;

            // Is flat Discount
            query.Clear();
            query.Append("SELECT  DiscountType  FROM VAM_DiscountCalculation WHERE "
                      + "VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId);
            string discountType = Util.GetValueOfString(DB.ExecuteScalar(query.ToString()));

            if (discountType == "F")
            {
                isCalulate = true;
                decimal discountBreakValue = (amount - ((amount * FlatDiscount.Value) / 100));
                amountAfterBreak = discountBreakValue;
                return amountAfterBreak;
            }
            else if (discountType == "B")
            {
                #region Product Based
                query.Clear();
                query.Append(@"SELECT VAM_ProductCategory_ID , VAM_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM VAM_BreakDiscount WHERE 
                                                                   VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND VAM_Product_ID = " + ProductId
                                                                           + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId + "Order BY BreakValue DESC");
                DataSet dsDiscountBreak = new DataSet();
                dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
                if (dsDiscountBreak != null)
                {
                    if (dsDiscountBreak.Tables.Count > 0)
                    {
                        if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                        {
                            int m = 0;
                            decimal discountBreakValue = 0;

                            for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                            {
                                if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                                {
                                    continue;
                                }
                                if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                    break;
                                }
                                else
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount.Value), 100));
                                    break;
                                }
                            }
                            if (isCalulate)
                            {
                                amountAfterBreak = discountBreakValue;
                                return amountAfterBreak;
                            }
                        }
                    }
                }
                #endregion

                #region Product Category Based
                query.Clear();
                query.Append(@"SELECT VAM_ProductCategory_ID , VAM_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM VAM_BreakDiscount WHERE 
                                                                   VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND VAM_ProductCategory_ID = " + productCategoryId
                                                                           + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId + "Order BY BreakValue DESC");
                dsDiscountBreak.Clear();
                dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
                if (dsDiscountBreak != null)
                {
                    if (dsDiscountBreak.Tables.Count > 0)
                    {
                        if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                        {
                            int m = 0;
                            decimal discountBreakValue = 0;

                            for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                            {
                                if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                                {
                                    continue;
                                }
                                if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                    break;
                                }
                                else
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount.Value), 100));
                                    break;
                                }
                            }
                            if (isCalulate)
                            {
                                amountAfterBreak = discountBreakValue;
                                return amountAfterBreak;
                            }
                        }
                    }
                }
                #endregion

                #region Otherwise
                query.Clear();
                query.Append(@"SELECT VAM_ProductCategory_ID , VAM_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM VAM_BreakDiscount WHERE 
                                                                   VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND VAM_ProductCategory_ID IS NULL AND VAM_Product_id IS NULL "
                                                                           + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId + "Order BY BreakValue DESC");
                dsDiscountBreak.Clear();
                dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
                if (dsDiscountBreak != null)
                {
                    if (dsDiscountBreak.Tables.Count > 0)
                    {
                        if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                        {
                            int m = 0;
                            decimal discountBreakValue = 0;

                            for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                            {
                                if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                                {
                                    continue;
                                }
                                if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                    break;
                                }
                                else
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount.Value), 100));
                                    break;
                                }
                            }
                            if (isCalulate)
                            {
                                amountAfterBreak = discountBreakValue;
                                return amountAfterBreak;
                            }
                        }
                    }
                }
                #endregion
            }

            return amountAfterBreak;
        }

        // Variant Changes Done by Mohit 08/07/2016

        public List<VariantData> GetVariants(int VAM_Product_ID, int VAM_Warehouse_ID, int ParentRec_ID, int VAM_PFeature_SetInstance_ID, string AttributeCode, Ctx ctx)
        {
            List<VariantData> _Variants = new List<VariantData>();
            StringBuilder _Str = new StringBuilder();
            string Sql = "";
            DataSet _DsVariants = null;
            _Str.Append("SELECT wh.VAM_Warehouse_ID,"
                        + " wh.Name AS Warehouse,"
                        + " asi.DESCRIPTION AS variant,"
                        + " VAM_Storage.VAM_Product_id,"
                        + " 1  AS Quantity,"
                        + " BOMQTYONHANDATTR(VAM_Storage.VAM_Product_ID,VAM_Storage.VAM_PFeature_SetInstance_ID,wh.VAM_Warehouse_ID,0) AS QtyOnHand,"
                        + " (BOMQTYONHANDATTR(VAM_Storage.VAM_Product_ID,VAM_Storage.VAM_PFeature_SetInstance_ID,wh.VAM_Warehouse_ID,0)- "
                        + " BOMQTYRESERVEDATTR(VAM_Storage.VAM_Product_ID,VAM_Storage.VAM_PFeature_SetInstance_ID,wh.VAM_Warehouse_ID,0)) AS QtyAvailable,"
                        + " BOMQTYRESERVEDATTR(VAM_Storage.VAM_Product_ID,VAM_Storage.VAM_PFeature_SetInstance_ID,wh.VAM_Warehouse_ID,0) AS QtyReserved,"
                        + " BOMQTYORDEREDATTR (VAM_Storage.VAM_Product_ID,VAM_Storage.VAM_PFeature_SetInstance_ID,wh.VAM_Warehouse_ID,0) AS QtyOrdered,"
                        + " VAM_Storage.VAM_PFeature_SetInstance_ID"
                        + " FROM VAM_Storage VAM_Storage INNER JOIN VAM_Locator loc ON (VAM_Storage.VAM_Locator_ID=loc.VAM_Locator_ID)"
                        + " INNER JOIN VAM_Warehouse wh ON (loc.VAM_Warehouse_ID=wh.VAM_Warehouse_ID) LEFT JOIN VAM_PFeature_SetInstance asi"
                        + " ON (VAM_Storage.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) WHERE VAM_Storage.IsActive='Y' AND VAM_Storage.VAM_Product_ID =" + VAM_Product_ID + "");
            if (VAM_Warehouse_ID > 0)
            {
                _Str.Append(" AND wh.VAM_Warehouse_ID=" + VAM_Warehouse_ID);
            }
            if (VAM_PFeature_SetInstance_ID > 0)
            {
                _Str.Append(" AND VAM_Storage.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID);
            }
            if (AttributeCode != "")
            {
                _Str.Append("AND VAM_Storage.VAM_PFeature_SetInstance_ID in (SELECT VAM_PFeature_SetInstance_ID from VAM_ProductFeatures WHERE VAM_Product_ID=" + VAM_Product_ID + "" +
                            " AND UPPER(UPC) like UPPER('%" + AttributeCode + "%'))");
            }
            _Str.Append(" ORDER BY Variant");

            try
            {
                Sql = MVAFRole.GetDefault(ctx).AddAccessSQL(_Str.ToString(), "VAM_Storage",
                                            MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);
                _DsVariants = DB.ExecuteDataset(Sql, null, null);
                _Str.Clear();
                Sql = "";
                if (_DsVariants != null && _DsVariants.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DsVariants.Tables[0].Rows.Count; i++)
                    {
                        VariantData VData = new VariantData();
                        VData.recid = i + 1;
                        VData.WareHouse = Util.GetValueOfString(_DsVariants.Tables[0].Rows[i]["Warehouse"]);
                        VData.Variant = Util.GetValueOfString(_DsVariants.Tables[0].Rows[i]["variant"]);
                        VData.Quantity = Util.GetValueOfDecimal(_DsVariants.Tables[0].Rows[i]["Quantity"]);
                        VData.Available = Util.GetValueOfDecimal(_DsVariants.Tables[0].Rows[i]["QtyAvailable"]);
                        VData.OnHand = Util.GetValueOfDecimal(_DsVariants.Tables[0].Rows[i]["QtyOnHand"]);
                        VData.Reserved = Util.GetValueOfDecimal(_DsVariants.Tables[0].Rows[i]["QtyReserved"]);
                        VData.Ordered = Util.GetValueOfDecimal(_DsVariants.Tables[0].Rows[i]["QtyOrdered"]);
                        VData.VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(_DsVariants.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                        VData.VAM_Product_ID = Util.GetValueOfInt(_DsVariants.Tables[0].Rows[i]["VAM_Product_id"]);
                        VData.VAM_Warehouse_ID = Util.GetValueOfInt(_DsVariants.Tables[0].Rows[i]["VAM_Warehouse_ID"]);
                        VData.ParentRec_ID = ParentRec_ID;
                        _Variants.Add(VData);
                    }
                }
            }
            catch (Exception e)
            {
                if (_DsVariants != null && _DsVariants.Tables[0].Rows.Count > 0)
                {
                    _DsVariants.Dispose();
                    _Str.Clear();
                    Sql = "";
                }
            }
            return _Variants;
        }

        // Added by Bharat on 31 May 2017
        public int GetWindowID(string fields, Ctx ctx)
        {
            int window_ID = 0;
            int tab_ID = Util.GetValueOfInt(fields);
            string sql = "SELECT VAF_Screen_ID FROM VAF_Tab WHERE VAF_Tab_ID = " + Util.GetValueOfInt(tab_ID);
            window_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return window_ID;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetWarehouse(Ctx ctx)
        {
            List<Dictionary<string, object>> retWare = null;
            string sql = MVAFRole.GetDefault(ctx).AddAccessSQL("SELECT VAM_Warehouse_ID, Value || ' - ' || Name AS ValueName FROM VAM_Warehouse WHERE IsActive='Y'",
                    "VAM_Warehouse", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO) + " ORDER BY Value";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retWare = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAM_Warehouse_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Warehouse_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ValueName"]);
                    retWare.Add(obj);
                }
            }
            return retWare;
        }

        public List<Dictionary<string, object>> GetUOM(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MVAFRole.GetDefault(ctx).AddAccessSQL("SELECT VAB_UOM_ID, Name FROM VAB_UOM WHERE IsActive = 'Y'",
                    "VAB_UOM", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAB_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_UOM_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    reDIc.Add(obj);
                }
            }
            return reDIc;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetPriceList(int PriceList, Ctx ctx)
        {
            List<Dictionary<string, object>> retPriceList = null;
            string sql = "SELECT plv.VAM_PriceListVersion_ID,"
                     + " plv.Name || ' (' || c.ISO_Code || ')' AS ValueName "
                     + "FROM VAM_PriceListVersion plv, VAM_PriceList pl, VAB_Currency c "
                     + "WHERE plv.VAM_PriceList_ID=pl.VAM_PriceList_ID" + " AND pl.VAB_Currency_ID=c.VAB_Currency_ID"
                     + " AND plv.IsActive='Y' AND pl.IsActive='Y'";

            if (PriceList != 0)
            {
                sql += " AND EXISTS (SELECT * FROM VAM_PriceList xp WHERE xp.VAM_PriceList_ID=" + PriceList + ")";
            }
            // Add Access & Order
            var qry = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAM_PriceListVersion", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO) + " ORDER BY plv.Name";
            DataSet ds = DB.ExecuteDataset(qry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retPriceList = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAM_PriceListVersion_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PriceListVersion_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ValueName"]);
                    retPriceList.Add(obj);
                }
            }
            return retPriceList;
        }

        // Added by Bharat on 31 May 2017
        public int GetDefaultPriceList(Ctx ctx)
        {
            string sql = "SELECT VAM_PriceList_ID FROM VAM_PriceList WHERE IsActive = 'Y' AND IsDefault = 'Y'";
            int PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return PriceList_ID;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetPriceListVersion(int PriceList, Ctx ctx)
        {
            List<Dictionary<string, object>> retPriceList = null;
            string sql = "SELECT plv.VAM_PriceListVersion_ID, plv.ValidFrom "
               + "FROM VAM_PriceList pl, VAM_PriceListVersion plv "
               + "WHERE pl.VAM_PriceList_ID=plv.VAM_PriceList_ID"
               + " AND plv.IsActive='Y'"
               + " AND pl.VAM_PriceList_ID=" + PriceList					//	1
               + " ORDER BY plv.ValidFrom DESC";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retPriceList = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAM_PriceListVersion_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PriceListVersion_ID"]);
                    obj["ValidFrom"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["ValidFrom"]);
                    retPriceList.Add(obj);
                }
            }
            return retPriceList;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetAttributeSet(Ctx ctx)
        {
            List<Dictionary<string, object>> retAttrSet = null;
            string sql = MVAFRole.GetDefault(ctx).AddAccessSQL("SELECT VAM_PFeature_Set_ID, Name FROM VAM_PFeature_Set WHERE IsActive='Y'",
                    "VAM_PFeature_Set", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retAttrSet = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAM_PFeature_Set_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_Set_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    retAttrSet.Add(obj);
                }
            }
            return retAttrSet;
        }

        // Added by Bharat on 31 May 2017
        public int DeleteCart(int invCount_ID, Ctx ctx)
        {
            string sql = "DELETE FROM VAICNT_InventoryCount WHERE VAICNT_InventoryCount_ID =" + invCount_ID;
            int no = DB.ExecuteQuery(sql, null, null);
            return no;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetCartData(string countID, int WindowID, Ctx ctx)
        {
            List<Dictionary<string, object>> retCart = null;
            string sql = "SELECT cl.VAM_Product_ID,prd.Name,prd.Value,cl.VAICNT_Quantity,cl.VAM_PFeature_SetInstance_ID,cl.VAB_UOM_ID,uom.Name as UOM,ic.VAICNT_ReferenceNo,cl.VAICNT_InventoryCountLine_ID,"
                        + " ats.Description FROM VAICNT_InventoryCount ic INNER JOIN VAICNT_InventoryCountLine cl ON ic.VAICNT_InventoryCount_ID = cl.VAICNT_InventoryCount_ID"
                        + " INNER JOIN VAM_Product prd ON cl.VAM_Product_ID = prd.VAM_Product_ID INNER JOIN VAB_UOM uom ON cl.VAB_UOM_ID = uom.VAB_UOM_ID LEFT JOIN VAM_PFeature_SetInstance ats"
                         + " ON cl.VAM_PFeature_SetInstance_ID = ats.VAM_PFeature_SetInstance_ID WHERE cl.IsActive = 'Y' AND ic.VAICNT_InventoryCount_ID IN (" + countID + ")";
            // JID_1700: When physical inventory showing only available stock
            if (Util.GetValueOfInt(Windows.PhysicalInventory) == WindowID || Util.GetValueOfInt(Windows.InternalUse) == WindowID)
            {
                sql += "AND prd.IsStocked='Y'";
            }
            sql += " ORDER BY ic.VAICNT_ReferenceNo, cl.Line";
            DataSet ds = DB.ExecuteDataset(sql, null, null);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                bool checkRefLine = false;
                DataSet dsLines = null;
                bool hasLines = false;
                if (Util.GetValueOfInt(Windows.MaterialReceipt) == WindowID || Util.GetValueOfInt(Windows.Shipment) == WindowID || Util.GetValueOfInt(Windows.InternalUse) == WindowID || Util.GetValueOfInt(Windows.InventoryMove) == WindowID)
                {
                    StringBuilder sqlSB = new StringBuilder("");
                    checkRefLine = true;
                    if (Util.GetValueOfInt(Windows.MaterialReceipt) == WindowID)
                    {
                        sqlSB.Append(@"SELECT ol.VAM_Product_ID, ol.VAB_UOM_ID, ol.VAM_PFeature_SetInstance_ID, o.DocumentNo, ol.VAB_OrderLine_ID AS LineID
                                    FROM VAB_Order o INNER JOIN VAB_OrderLine ol ON (ol.VAB_Order_ID = o.VAB_Order_ID) WHERE o.IsActive = 'Y' AND o.VAF_Client_ID = " + ctx.GetVAF_Client_ID()
                                    + @" AND o.IsSOTrx = 'N' AND o.DocStatus IN ('CO', 'CL') AND o.DocumentNo IN (
                                    SELECT VAICNT_ReferenceNo FROM VAICNT_InventoryCount WHERE VAICNT_InventoryCount_ID IN (" + countID + "))");
                    }
                    else if (Util.GetValueOfInt(Windows.Shipment) == WindowID)
                    {
                        sqlSB.Append(@"SELECT ol.VAM_Product_ID, ol.VAB_UOM_ID, ol.VAM_PFeature_SetInstance_ID, o.DocumentNo, ol.VAB_OrderLine_ID AS LineID
                                    FROM VAB_Order o INNER JOIN VAB_OrderLine ol ON (ol.VAB_Order_ID = o.VAB_Order_ID) WHERE o.IsActive = 'Y' AND o.VAF_Client_ID = " + ctx.GetVAF_Client_ID()
                                    + @" AND o.IsSOTrx = 'Y' AND o.DocStatus IN ('CO') AND o.DocumentNo IN (
                                    SELECT VAICNT_ReferenceNo FROM VAICNT_InventoryCount WHERE VAICNT_InventoryCount_ID IN (" + countID + "))");
                    }
                    else if (Util.GetValueOfInt(Windows.InternalUse) == WindowID || Util.GetValueOfInt(Windows.InventoryMove) == WindowID)
                    {
                        sqlSB.Append(@"SELECT ol.VAM_RequisitionLine_ID AS LineID, o.DocumentNo  ol.VAM_Product_ID, ol.VAM_PFeature_SetInstance_ID, ol.VAB_UOM_ID FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition o
                                    ON ol.VAM_Requisition_ID =o.VAM_Requisition_ID WHERE o.IsActive = 'Y' AND o.VAF_Client_ID = " + ctx.GetVAF_Client_ID()
                                    + @" AND o.DocStatus IN ('CO') AND o.DocumentNo IN (
                                    SELECT VAICNT_ReferenceNo FROM VAICNT_InventoryCount WHERE VAICNT_InventoryCount_ID IN (" + countID + "))");
                    }

                    dsLines = DB.ExecuteDataset(sqlSB.ToString());
                    if (dsLines != null && dsLines.Tables[0].Rows.Count > 0)
                        hasLines = true;
                }

                retCart = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["VAM_Product_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    obj["VAICNT_Quantity"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAICNT_Quantity"]);
                    obj["VAM_PFeature_SetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]);
                    obj["VAB_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_UOM_ID"]);
                    obj["UOM"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    obj["VAICNT_ReferenceNo"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VAICNT_ReferenceNo"]);
                    obj["VAICNT_InventoryCountLine_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAICNT_InventoryCountLine_ID"]);
                    obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    if (hasLines && checkRefLine)
                    {
                        DataRow[] dr = dsLines.Tables[0].Select(" VAM_Product_ID = " + obj["VAM_Product_ID"] + " AND VAM_PFeature_SetInstance_ID = " + obj["VAM_PFeature_SetInstance_ID"] + " AND VAB_UOM_ID = " + obj["VAB_UOM_ID"] + " AND DocumentNo = '" + obj["VAICNT_ReferenceNo"] + "'");
                        if (dr != null && dr.Length > 0)
                        {
                            obj["LineID"] = dr[0]["LineID"];
                        }
                        else
                            obj["LineID"] = 0;
                    }
                    else
                        obj["LineID"] = 0;

                    retCart.Add(obj);
                }
            }
            return retCart;
        }

        /// <summary>
        /// Returns dataset of Products from Purchasing Tab
        /// </summary>
        /// <returns>Dataset</returns>
        public DataSet GetPurchaingProduct(int VAF_Client_ID)
        {
            DataSet dsProPurch = null;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT vdr.VAB_UOM_ID, vdr.VAB_BusinessPartner_ID, p.VAM_Product_ID FROM VAM_Product p LEFT JOIN 
                            VAM_Product_PO vdr ON p.VAM_Product_ID= vdr.VAM_Product_ID WHERE p.VAF_Client_ID = " + VAF_Client_ID);
            dsProPurch = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsProPurch;
        }

        /// <summary>
        /// returns Price list version ID based on the Price List passed in the parameter
        /// </summary>
        /// <param name="VAM_PriceList_ID"></param>
        /// <returns>VAM_PriceListVersion_ID (int)</returns>
        public int GetPLVID(int VAM_PriceList_ID)
        {
            int VAM_PriceListVersion_ID = 0;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT VAM_PriceListVersion_ID FROM VAM_PriceListVersion WHERE IsActive = 'Y' 
                                AND VAM_PriceList_ID = " + VAM_PriceList_ID + @" AND ValidFrom <= SYSDATE ORDER BY ValidFrom DESC");
            VAM_PriceListVersion_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));
            return VAM_PriceListVersion_ID;
        }

        /// <summary>
        /// function to fetch Products with UOM IDs for tenant
        /// </summary>
        /// <param name="VAF_Client_ID"></param>
        /// <returns>Dataset of Products with UOM</returns>
        public DataSet GetProducts(int VAF_Client_ID)
        {
            DataSet dsProds = null;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT VAM_Product_ID, VAB_UOM_ID, Name FROM VAM_Product WHERE VAF_Client_ID = " + VAF_Client_ID);
            dsProds = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsProds;
        }

        /// <summary>
        /// function to return product prices based on Tenant and Pricelist version
        /// </summary>
        /// <param name="VAF_Client_ID"></param>
        /// <param name="VAM_PriceListVersion_ID"></param>
        /// <returns>Dataset of Product Prices</returns>
        public DataSet GetProductsPrice(int VAF_Client_ID, int VAM_PriceListVersion_ID)
        {
            DataSet dsProdPrices = null;

            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT PriceList , PriceStd , PriceLimit, VAM_Product_ID, NVL(VAM_PFeature_SetInstance_ID,0) AS VAM_PFeature_SetInstance_ID, VAB_UOM_ID
                            FROM VAM_ProductPrice WHERE Isactive='Y' AND VAF_Client_ID = " + VAF_Client_ID + " AND VAM_PriceListVersion_ID = " + VAM_PriceListVersion_ID);
            dsProdPrices = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsProdPrices;
        }

        /// <summary>
        /// function to fetch UOM Conversions for the specified Tenant
        /// </summary>
        /// <param name="VAF_Client_ID"></param>
        /// <returns>Dataset of UOM Conversions</returns>
        public DataSet GetUOMConversions(int VAF_Client_ID)
        {
            DataSet dsConvs = null;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT con.DivideRate, TRUNC(con.multiplyrate,4) AS MultiplyRate, con.VAB_UOM_ID, con.VAB_UOM_To_ID, con.VAM_Product_ID FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' AND con.VAF_Client_ID = " + VAF_Client_ID);
            dsConvs = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsConvs;
        }

        /// <summary>
        /// function to fetch Prices for the product based on the conditions 
        /// of attributes and UOM Conversions
        /// </summary>
        /// <param name="dsProdPrice"></param>
        /// <param name="dsUOMCon"></param>
        /// <param name="VAF_Client_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="M_ASI_ID"></param>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="ProdUOM"></param>
        /// <param name="QtyEntered"></param>
        /// <param name="VAM_DiscountCalculation_ID"></param>
        /// <param name="bpFlatDiscount"></param>
        /// <returns>Dictionary of Prices (PriceList, PriceLimit, PriceActual, PriceEntered)</returns>
        public Dictionary<string, Decimal?> GetPrices(DataSet dsProdPrice, DataSet dsUOMCon, int VAF_Client_ID, int VAM_Product_ID, int M_ASI_ID, int VAB_UOM_ID, DataSet dsProducts,
            Decimal? QtyEntered, int VAM_DiscountCalculation_ID, Decimal? bpFlatDiscount)
        {
            Dictionary<string, Decimal?> prodPrice = new Dictionary<string, decimal?>();

            decimal convertedprice = 0;
            Decimal? PriceList = 0, PriceLimit = 0;
            DataRow[] drPrice = null;
            bool convertPrice = false;
            Decimal? conversionRate = 0;

            // check if prices exist for Products
            if (dsProdPrice != null && dsProdPrice.Tables[0].Rows.Count > 0)
            {
                // check price against Product and Attribute set instance and UOM
                drPrice = dsProdPrice.Tables[0].Select("VAM_Product_ID = " + VAM_Product_ID + " AND VAM_PFeature_SetInstance_ID = " + M_ASI_ID + " AND VAB_UOM_ID = " + VAB_UOM_ID);
                // if price not found against above conditions then check against product and 0 Attirbute set instance and UOM
                if (!(drPrice != null && drPrice.Length > 0))
                {
                    drPrice = dsProdPrice.Tables[0].Select(" VAM_Product_ID = " + VAM_Product_ID + " AND VAM_PFeature_SetInstance_ID = 0 AND VAB_UOM_ID = " + VAB_UOM_ID);
                    // if price still not found then go for the conversions
                    if (!(drPrice != null && drPrice.Length > 0))
                    {
                        int ProdUOM = 100;
                        // Fetch Base UOM of the Product
                        if (dsProducts != null && dsProducts.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] drPUOM = dsProducts.Tables[0].Select(" VAM_Product_ID = " + VAM_Product_ID);
                            if (drPUOM != null && drPUOM.Length > 0)
                                ProdUOM = Util.GetValueOfInt(drPUOM[0]["VAB_UOM_ID"]);
                        }

                        // Fetch conversions from Base UOM to selected UOM
                        if (dsUOMCon != null && dsUOMCon.Tables[0].Rows.Count > 0)
                        {
                            // Fetched conversion rate based on Product and UOM From to UOM TO
                            DataRow[] drRate = dsUOMCon.Tables[0].Select(" VAM_Product_ID = " + VAM_Product_ID + " AND VAB_UOM_ID = " + ProdUOM + " AND VAB_UOM_To_ID = " + VAB_UOM_ID);
                            if (drRate != null && drRate.Length > 0)
                            {
                                conversionRate = Util.GetValueOfDecimal(drRate[0]["DivideRate"]);
                                convertPrice = true;
                            }
                            // if conversion rate not found, then have to check conversion only for UOM From to UOM To without Product
                            if (conversionRate == 0)
                            {
                                drRate = dsUOMCon.Tables[0].Select(" VAB_UOM_ID = " + ProdUOM + " AND VAB_UOM_To_ID = " + VAB_UOM_ID);
                                if (drRate != null && drRate.Length > 0)
                                {
                                    conversionRate = Util.GetValueOfDecimal(drRate[0]["DivideRate"]);
                                    convertPrice = true;
                                }
                            }
                        }

                        // Check for Product Price from Pricelist against Product ID , Attribute Set Instance and Base UOM of Product (this case will run only
                        // if price not found initially against the UOM passed in the parameter)
                        drPrice = dsProdPrice.Tables[0].Select("VAM_Product_ID = " + VAM_Product_ID + " AND VAM_PFeature_SetInstance_ID = " + M_ASI_ID + " AND VAB_UOM_ID = " + ProdUOM);
                        // if Price not found again
                        // then check for the price against 0 Attribute Set Instance
                        if (!(drPrice != null && drPrice.Length > 0))
                        {
                            drPrice = dsProdPrice.Tables[0].Select("VAM_Product_ID = " + VAM_Product_ID + " AND VAM_PFeature_SetInstance_ID = 0 AND VAB_UOM_ID = " + ProdUOM);
                        }
                    }
                }
            }

            // if Price found then calcluate prices with Conversion
            if (drPrice != null && drPrice.Length > 0)
            {
                convertedprice = FlatDiscount(VAM_Product_ID, VAF_Client_ID, Util.GetValueOfDecimal(drPrice[0]["PriceStd"]), VAM_DiscountCalculation_ID, bpFlatDiscount, QtyEntered);
                PriceList = Util.GetValueOfDecimal(drPrice[0]["PriceList"]);
                PriceLimit = Util.GetValueOfDecimal(drPrice[0]["PriceLimit"]);
                if (convertPrice && conversionRate > 0)
                {
                    PriceList = Decimal.Multiply(PriceList.Value, conversionRate.Value);
                    PriceLimit = Decimal.Multiply(PriceLimit.Value, conversionRate.Value);
                    convertedprice = Decimal.Multiply(convertedprice, conversionRate.Value);
                }
            }

            prodPrice["PriceList"] = PriceList;
            prodPrice["PriceLimit"] = PriceLimit;
            prodPrice["PriceActual"] = convertedprice;
            prodPrice["PriceEntered"] = convertedprice;

            return prodPrice;
        }

        /// <summary>
        /// function to fetch default Locator from the selected Warehouse
        /// </summary>
        /// <param name="VAM_Warehouse_ID"></param>
        /// <returns>Locator ID</returns>
        public int hasDefaultLocator(int VAM_Warehouse_ID)
        {
            _sqlQuery.Clear();
            _sqlQuery.Append("SELECT VAM_Locator_ID FROM VAM_Locator WHERE VAM_Warehouse_ID = " + VAM_Warehouse_ID + " AND IsDefault = 'Y' AND IsActive = 'Y'");
            return Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));
        }
        //-----------------------------------------
    }

    public class InfoProduct
    {
        public int VAF_TableView_ID
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsIdentifier
        {
            get;
            set;
        }
    }

    public class InfoProductData
    {
        public List<DataObject> data
        {
            get;
            set;
        }
        public PageSetting pSetting
        {
            get;
            set;
        }
        public string Error
        {
            get;
            set;
        }
    }

    public class InfoLines
    {
        public int _prdID { get; set; }
        public string _prdName { get; set; }
        public string _value { get; set; }
        public decimal _prodQty { get; set; }
        public int _uom { get; set; }
        public string _uomName { get; set; }
        public string _RefNo { get; set; }
        public int _Attribute { get; set; }
        public string _AttributeName { get; set; }
        public string _IsLotSerial { get; set; }
        public int _Locator_ID { get; set; }
        public int _countID { get; set; }
        public string _VAF_Session_ID { get; set; }
        public int _windowNo { get; set; }
    }

    public class InfoScanData
    {
        public List<ReferenceData> refData { get; set; }
        public int VAM_Locator_ID { get; set; }
        public int VAM_LocatorTo_ID { get; set; }
        public int ordID { get; set; }
    }

    public class ReferenceData
    {
        public int OrdLineID { get; set; }
        public int AstID { get; set; }
        public decimal qtybook { get; set; }
    }

    public class InfoSave
    {
        public int count { get; set; }
        public string error { get; set; }
        public List<string> errorKeys { get; set; }
        public List<string> errorProds { get; set; }
    }

    public class VariantData
    {
        public int recid { get; set; }
        public string WareHouse { get; set; }
        public string Variant { get; set; }
        public Decimal? Quantity { get; set; }
        public Decimal? Available { get; set; }
        public Decimal? OnHand { get; set; }
        public Decimal? Reserved { get; set; }
        public Decimal? Ordered { get; set; }
        public int VAM_Warehouse_ID { get; set; }
        public int VAM_PFeature_SetInstance_ID { get; set; }
        public int VAM_Product_ID { get; set; }
        public int ParentRec_ID { get; set; }
    }

    public class InfoCartData
    {
        public List<InfoCart> data
        {
            get;
            set;
        }
        public PageSetting pSetting
        {
            get;
            set;
        }
        public string Error
        {
            get;
            set;
        }
    }

    public class InfoCart
    {
        public int recid { get; set; }
        public int count_ID { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public DateTime? TrxDate { get; set; }
    }
}
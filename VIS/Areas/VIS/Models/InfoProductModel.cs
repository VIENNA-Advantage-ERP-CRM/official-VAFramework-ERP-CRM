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
            s_headerPriceList = Msg.Translate(ctx, "M_PriceList_Version_ID");
            s_headerWarehouse = Msg.Translate(ctx, "M_Warehouse_ID");
            //  Euro 13
            MClient client = MClient.Get(ctx);
            if ("FRIE".Equals(client.GetValue()))
            {
                InfoColumn[] frieLayout =
            {
				new InfoColumn(Msg.Translate(ctx,"M_Product_ID"),"M_Product_ID", true, "p.M_Product_ID",DisplayType.ID).Seq(10),
				new InfoColumn(Msg.Translate(ctx, "Name"), "Name", true, "p.Name",DisplayType.String).Seq(20),
                new InfoColumn(Msg.Translate(ctx,"QtyEntered"), "QtyEntered", false, "1 as QtyEntered" , DisplayType.Quantity).Seq(30), 
				new InfoColumn(Msg.Translate(ctx, "QtyAvailable"), "QtyAvailable",true,
					"bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable", DisplayType.Quantity).Seq(40),
				new InfoColumn(s_headerPriceList, "M_PriceList_Version_ID",true, "plv.Name", DisplayType.Amount).Seq(50),
                new InfoColumn(s_headerWarehouse, "M_Warehouse_ID",true, "w.Name", DisplayType.String).Seq(60),
				new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList",true,
					"bomPriceList(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceList",  DisplayType.Amount).Seq(70),
				new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd",true,
					"bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceStd", DisplayType.Amount).Seq(80),
				new InfoColumn("Einzel MWSt", "",true,
					"pr.PriceStd * 1.19", DisplayType.Amount).Seq(90),
				new InfoColumn("Einzel kompl", "",true,
					"(pr.PriceStd+13) * 1.19", DisplayType.Amount).Seq(100),
				new InfoColumn("Satz kompl", "", true,
					"((pr.PriceStd+13) * 1.19) * 4", DisplayType.Amount).Seq(110),
				new InfoColumn(Msg.Translate(ctx, "QtyOnHand"), "QtyOnHand",true,
					"bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOnHand", DisplayType.Quantity).Seq(120),
				new InfoColumn(Msg.Translate(ctx, "QtyReserved"), "QtyReserved",true,
					"bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyReserved", DisplayType.Quantity).Seq(130),
				new InfoColumn(Msg.Translate(ctx, "QtyOrdered"), "QtyOrdered",true,
					"bomQtyOrdered(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOrdered", DisplayType.Quantity).Seq(140),
				new InfoColumn(Msg.Translate(ctx, "Discontinued").Substring(0, 1), "Discontinued",true,
					"p.Discontinued", DisplayType.YesNo).Seq(150),
				new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin",true,
					"bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID)-bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS Margin", DisplayType.Amount).Seq(160),
				new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit",true,
					"bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceLimit", DisplayType.Amount).Seq(170),
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
                list.Add(new InfoColumn(Msg.Translate(ctx, "M_Product_ID"), "M_Product_ID", true, "p.M_Product_ID", DisplayType.ID).Seq(10));
                //list.Add(new InfoColumn(Msg.Translate(ctx, "SelectProduct"),"SelectProduct",true, "'N'", DisplayType.YesNo).Seq(20));
                //list.Add(new InfoColumn(Msg.Translate(ctx, "Discontinued"), "Discontinued", true, "p.Discontinued", DisplayType.YesNo).Seq(30));
                list.Add(new InfoColumn(Msg.Translate(ctx, "C_UOM_ID"), "C_UOM_ID", true, "p.C_UOM_ID", DisplayType.ID).Seq(20));
                list.Add(new InfoColumn(Msg.Translate(ctx, "UOM"), "UOM", true, "c.Name as UOM", DisplayType.ID).Seq(30));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Value"), "Value", true, "p.Value", DisplayType.String).Seq(40));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Name"), "Name", true, "p.Name", DisplayType.String).Seq(50));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyEntered"), "QtyEntered", false, "1 as QtyEntered", DisplayType.Quantity).Seq(60));
                list.Add(new InfoColumn(s_headerWarehouse, "Warehouse", true, "w.Name as Warehouse", DisplayType.String).Seq(80));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyAvailable"), "QtyAvailable", true,
                    "bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable", DisplayType.Quantity).Seq(90));
                //}
                list.Add(new InfoColumn(s_headerPriceList, "PriceListVersion", true, "plv.Name as PriceListVersion", DisplayType.String).Seq(100));
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("VAPRC_", out mInfo))
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                        "bomPriceListUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceList", DisplayType.Amount).Seq(120));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                        "bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                    }
                    else
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                            "bomPriceListAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS PriceList", DisplayType.Amount).Seq(120));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                            "bomPriceStdAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                    }
                }
                else
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                        "bomPriceList(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceList", DisplayType.Amount).Seq(120));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                        "bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                }

                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyOnHand"), "QtyOnHand", true,
                    "bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOnHand", DisplayType.Quantity).Seq(140));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyReserved"), "QtyReserved", true,
                    "bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyReserved", DisplayType.Quantity).Seq(150));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyOrdered"), "QtyOrdered", true,
                    "bomQtyOrdered(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOrdered", DisplayType.Quantity).Seq(160));
                //}
                if (isConferm) //IsUnconfirmed())
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "QtyUnconfirmed"), "QtyUnconfirmed", true,
                        "(SELECT SUM(c.TargetQty) FROM M_InOutLineConfirm c INNER JOIN M_InOutLine il ON (c.M_InOutLine_ID=il.M_InOutLine_ID) INNER JOIN M_InOut i ON (il.M_InOut_ID=i.M_InOut_ID) WHERE c.Processed='N' AND i.M_Warehouse_ID=w.M_Warehouse_ID AND il.M_Product_ID=p.M_Product_ID) AS QtyUnconfirmed",
                        DisplayType.Quantity).Seq(170));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "QtyUnconfirmedMove"), "QtyUnconfirmedMove", true,
                        "(SELECT SUM(c.TargetQty) FROM M_MovementLineConfirm c INNER JOIN M_MovementLine ml ON (c.M_MovementLine_ID=ml.M_MovementLine_ID) INNER JOIN M_Locator l ON (ml.M_LocatorTo_ID=l.M_Locator_ID) WHERE c.Processed='N' AND l.M_Warehouse_ID=w.M_Warehouse_ID AND ml.M_Product_ID=p.M_Product_ID) AS QtyUnconfirmedMove",
                        DisplayType.Quantity).Seq(180));
                }
                if (Env.HasModulePrefix("VAPRC_", out mInfo))
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin", true,
                            "bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID)-bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS Margin",
                            DisplayType.Amount).Seq(190));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit", true,
                            "bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                    }
                    else
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin", true,
                            "bomPriceStdAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID)-bomPriceLimitAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS Margin",
                            DisplayType.Amount).Seq(190));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit", true,
                            "bomPriceLimitAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                    }
                }
                else
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin", true,
                            "bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID)-bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS Margin", DisplayType.Amount).Seq(190));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit", true,
                        "bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                }
                list.Add(new InfoColumn(Msg.Translate(ctx, "IsInstanceAttribute"), "IsInstanceAttribute", true, "pa.IsInstanceAttribute", DisplayType.YesNo).Seq(210));
                list.Add(new InfoColumn(Msg.Translate(ctx, "GuranteeDays"), "GuranteeDays", true, "adddays(Sysdate, p.GuaranteeDays) as GuranteeDays", DisplayType.Date).Seq(220));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Discontinued"), "Discontinued", true, "p.Discontinued", DisplayType.YesNo).Seq(230));
                list.Add(new InfoColumn(s_headerWarehouse, "M_Warehouse_ID", true, "w.M_Warehouse_ID", DisplayType.ID).Seq(240));
                list.Add(new InfoColumn(s_headerPriceList, "M_PriceList_Version_ID", true, "plv.M_PriceList_Version_ID", DisplayType.ID).Seq(250));
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
                sqlWhere = MRole.GetDefault(ctx).AddAccessSQL(sqlWhere, tableName,
                                MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                //DataSet data = DBase.DB.ExecuteDataset(sql, null, null);
                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM ( " + sqlWhere + " ) t", null, null));
                int pageSize = 50;
                PageSetting pSetting = new PageSetting();
                pSetting.CurrentPage = pageNo;
                pSetting.TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                _iData.pSetting = pSetting;

                int startPage = ((pageNo - 1) * pageSize) + 1;
                int endPage = pageNo * pageSize;
                string sqlPaging = @"SELECT p.M_Product_ID, NVL(pr.M_AttriButeSetInstance_ID,0) AS M_AttriButeSetInstance_ID, NVL(pr.C_UOM_ID,0) AS C_UOM_ID, 
                        NVL(pr.M_PriceList_Version_ID,0) AS M_PriceList_Version_ID, w.M_Warehouse_ID FROM M_Product p 
                        LEFT OUTER JOIN M_ProductPrice pr  ON (p.M_Product_ID=pr.M_Product_ID AND pr.IsActive ='Y') 
                        LEFT OUTER JOIN M_AttributeSet pa ON (p.M_AttributeSet_ID=pa.M_AttributeSet_ID) 
                        LEFT OUTER JOIN C_UOM c ON (p.C_UOM_ID=c.C_UOM_ID)
                        , M_Warehouse w " + where + " ORDER BY p.M_Product_ID, M_PriceList_Version_ID, w.M_Warehouse_ID, M_AttriButeSetInstance_ID, C_UOM_ID";
                sqlPaging = MRole.GetDefault(ctx).AddAccessSQL(sqlPaging, tableName, MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                sqlPaging = @"JOIN (SELECT row_num, M_Product_ID, M_AttriButeSetInstance_ID, C_UOM_ID, M_PriceList_Version_ID, M_Warehouse_ID FROM (SELECT prd.*, row_number() over (order by prd.M_Product_ID) AS row_num FROM 
                        (" + sqlPaging + @") prd) t WHERE row_num BETWEEN " + startPage + " AND " + endPage +
                        @") pp ON pp.M_Product_ID = p.M_Product_ID AND pp.M_AttriButeSetInstance_ID = NVL(pr.M_AttriButeSetInstance_ID,0) AND pp.C_UOM_ID = NVL(pr.C_UOM_ID,0) 
                        AND pp.M_Warehouse_ID = w.M_Warehouse_ID AND pp.M_PriceList_Version_ID = NVL(pr.M_PriceList_Version_ID,0)";
                sql += sqlPaging + where;
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
                        sqlWhere.Append(" AND M_Warehouse_ID = " + WarehouseToID);
                    else if (LocatorToID > 0)  // JID_0985 If only from warehouse selected on move header. In scan option carts should show which have requistion no. 
                    {                          //having same source warehosue as locator of warehouse selected at move line.
                        WarehouseToID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Warehouse_ID FROM M_Locator WHERE M_Locator_ID = " + LocatorToID));
                        if (WarehouseToID > 0)
                            sqlWhere.Append(" AND M_Warehouse_ID = " + WarehouseToID);
                    }
                    sql += " AND VAICNT_ReferenceNo IN (SELECT DocumentNo FROM M_Requisition WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND IsActive = 'Y' AND DocStatus IN ('CO') "
                        + sqlWhere.ToString() + ")";
                }
                else if (!isCart && windowID == 168)  // JID_1030: on physical inventory system does not check that the locator is of selected warehouse on Physiacl inventory header or not.
                {
                    if (WarehouseID > 0)
                        sql += " AND VAICNT_ReferenceNo IN (SELECT Value FROM M_Locator WHERE IsActive = 'Y' AND M_Warehouse_ID = " + WarehouseID + ")";
                }

                sql += " ORDER BY VAICNT_ScanName";

                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "VAICNT_InventoryCount",
                                MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

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
            MTable tbl = null;
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

            int AD_Client_ID = 0;
            int AD_Org_ID = 0;
            int C_BPartner_ID = 0;
            int M_DiscountSchema_ID = 0;
            Decimal? bpFlatDiscount = 0;
            MBPartner bp = null;
            StringBuilder prodName = new StringBuilder("");
            List<string> errorKeys = new List<string>();
            List<string> errorProdLines = new List<string>();
            #endregion local Variables

            // Evaluate only for these 4 tables
            // get IDs like Client ID, Org ID, BP ID from parent
            if (keyColName.ToUpper().Trim() == "C_ORDER_ID" || keyColName.ToUpper().Trim() == "C_INVOICE_ID"
                || keyColName.ToUpper().Trim() == "M_REQUISITION_ID" || keyColName.ToUpper().Trim() == "C_PROJECT_ID")
            {
                if (keyColName.ToUpper().Trim() == "C_ORDER_ID")
                {
                    MOrder ord = new MOrder(ctx, recordID, null);
                    _Version_ID = GetPLVID(ord.GetM_PriceList_ID());
                    AD_Client_ID = ord.GetAD_Client_ID();
                    AD_Org_ID = ord.GetAD_Org_ID();
                    isSOTrx = ord.IsSOTrx();
                    C_BPartner_ID = ord.GetC_BPartner_ID();
                }
                else if (keyColName.ToUpper().Trim() == "C_INVOICE_ID")
                {
                    MInvoice inv = new MInvoice(ctx, recordID, null);
                    _Version_ID = GetPLVID(inv.GetM_PriceList_ID());
                    AD_Client_ID = inv.GetAD_Client_ID();
                    AD_Org_ID = inv.GetAD_Org_ID();
                    isSOTrx = inv.IsSOTrx();
                    C_BPartner_ID = inv.GetC_BPartner_ID();
                }
                else if (keyColName.ToUpper().Trim() == "M_REQUISITION_ID")
                {
                    MRequisition req = new MRequisition(ctx, recordID, null);
                    _Version_ID = GetPLVID(req.GetM_PriceList_ID());
                    AD_Client_ID = req.GetAD_Client_ID();
                    AD_Org_ID = req.GetAD_Org_ID();
                    if (req.Get_ColumnIndex("C_BPartner_ID") > 0 && Util.GetValueOfInt(req.Get_Value("C_BPartner_ID")) > 0)
                    {
                        C_BPartner_ID = Util.GetValueOfInt(req.Get_Value("C_BPartner_ID"));
                    }
                }
                else if (keyColName.ToUpper().Trim() == "C_PROJECT_ID")
                {
                    MProject proj = new MProject(ctx, recordID, null);
                    AD_Client_ID = proj.GetAD_Client_ID();
                    _Version_ID = proj.GetM_PriceList_Version_ID();
                    AD_Org_ID = proj.GetAD_Org_ID();
                    C_BPartner_ID = proj.GetC_BPartner_ID();
                }

                // if business partner found on parent then get Discount Schema ID and Flat Discount amount
                if (C_BPartner_ID > 0)
                {
                    bp = new MBPartner(ctx, C_BPartner_ID, null);
                    M_DiscountSchema_ID = bp.GetM_DiscountSchema_ID();
                    bpFlatDiscount = bp.GetFlatDiscount();
                }

                if (!isSOTrx)
                {
                    if (dsProPO == null && !fetchedProPurchasing)
                    {
                        dsProPO = GetPurchaingProduct(AD_Client_ID);
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
                        dsUOMConv = GetUOMConversions(AD_Client_ID);
                        fetchedUOMConv = true;
                    }
                }
            }

            // Fetch Products and Product Prices 
            if (!fetchedRecords)
            {
                if (AD_Client_ID == 0)
                    AD_Client_ID = ctx.GetAD_Client_ID();
                dsProducts = GetProducts(AD_Client_ID);
                if (dsProducts != null && dsProducts.Tables[0].Rows.Count > 0)
                    hasProducts = true;

                if (_Version_ID > 0)
                    dsProPrice = GetProductsPrice(AD_Client_ID, _Version_ID);

                fetchedRecords = true;
            }

            #region Order Table
            if (keyColName.ToUpper().Trim() == "C_ORDER_ID")
            {
                tbl = new MTable(ctx, 260, null);
                MOrder ord = new MOrder(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    int _m_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = Util.GetValueOfInt(attribute[i]);
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("AD_Client_ID", AD_Client_ID);
                    po.Set_ValueNoCheck("AD_Org_ID", AD_Org_ID);
                    po.Set_Value("M_Product_ID", _m_Product_ID);
                    po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                    po.Set_Value("QtyOrdered", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("C_Order_ID", recordID);
                    if (_attribute_ID != 0)
                        po.Set_Value("M_AttributeSetInstance_ID", _attribute_ID);

                    if (!ord.IsSOTrx())
                    {
                        int uomID = Util.GetValueOfInt(uoms[i]);
                        int uom = 0;

                        if (hasProdsPurch)
                        {
                            DataRow[] dr = dsProPO.Tables[0].Select(" M_Product_ID = " + _m_Product_ID + " AND C_BPartner_ID = " + C_BPartner_ID);
                            if (dr != null && dr.Length > 0)
                                uom = Util.GetValueOfInt(dr[0]["C_UOM_ID"]);
                        }

                        if (uomID != 0)
                        {
                            if (uomID != uom && uom != 0)
                                po.Set_ValueNoCheck("C_UOM_ID", uom);
                            else
                                po.Set_ValueNoCheck("C_UOM_ID", uomID);
                        }
                    }
                    else
                        po.Set_ValueNoCheck("C_UOM_ID", Util.GetValueOfInt(uoms[i]));

                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        SetPrices(po, GetPrices(dsProPrice, dsUOMConv, AD_Client_ID, _m_Product_ID,
                            _attribute_ID, Util.GetValueOfInt(po.Get_Value("C_UOM_ID")), dsProducts,
                            Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), M_DiscountSchema_ID, bpFlatDiscount));
                    }
                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, _m_Product_ID, prodName);
                    }
                }
            }
            #endregion Order

            #region Invoice
            else if (keyColName.ToUpper().Trim() == "C_INVOICE_ID")
            {
                tbl = new MTable(ctx, 333, null);
                MInvoice inv = new MInvoice(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    int _m_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = Util.GetValueOfInt(attribute[i]);
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("AD_Client_ID", AD_Client_ID);
                    po.Set_ValueNoCheck("AD_Org_ID", AD_Org_ID);
                    po.Set_Value("M_Product_ID", _m_Product_ID);
                    po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                    po.Set_Value("QtyInvoiced", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("C_Invoice_ID", recordID);
                    if (_attribute_ID != 0)
                        po.Set_Value("M_AttributeSetInstance_ID", _attribute_ID);

                    if (!inv.IsSOTrx())
                    {
                        int uomID = Util.GetValueOfInt(uoms[i]);
                        int uom = 0;

                        if (hasProdsPurch)
                        {
                            DataRow[] dr = dsProPO.Tables[0].Select(" M_Product_ID = " + _m_Product_ID + " AND C_BPartner_ID = " + C_BPartner_ID);
                            if (dr != null && dr.Length > 0)
                                uom = Util.GetValueOfInt(dr[0]["C_UOM_ID"]);
                        }

                        if (uomID != 0)
                        {
                            if (uomID != uom && uom != 0)
                                po.Set_ValueNoCheck("C_UOM_ID", uom);
                            else
                                po.Set_ValueNoCheck("C_UOM_ID", uomID);
                        }
                    }
                    else
                        po.Set_ValueNoCheck("C_UOM_ID", Util.GetValueOfInt(uoms[i]));

                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        SetPrices(po, GetPrices(dsProPrice, dsUOMConv, AD_Client_ID, _m_Product_ID,
                            _attribute_ID, Util.GetValueOfInt(po.Get_Value("C_UOM_ID")), dsProducts,
                            Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), M_DiscountSchema_ID, bpFlatDiscount));
                    }
                    if (!po.Save())
                    {
                        count++;
                        SetErrorMessages(errorKeys, errorProdLines, hasProducts, dsProducts, _m_Product_ID, prodName);
                    }
                }
            }
            #endregion Invoice

            #region MR/Shipment
            else if (keyColName.ToUpper().Trim() == "M_INOUT_ID")
            {
                tbl = new MTable(ctx, 320, null);

                int ordID = 0;

                bool saved = true;
                MInOut io = new MInOut(ctx, recordID, null);

                if (Locator_ID <= 0)
                {
                    Locator_ID = hasDefaultLocator(io.GetM_Warehouse_ID());
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
                                _sqlQuery.Append("SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND IsSOTrx= 'Y' AND AD_Client_ID =" + ctx.GetAD_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                            else if (WindowID == Util.GetValueOfInt(Windows.MaterialReceipt))
                                _sqlQuery.Append("SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND IsSOTrx= 'N' AND AD_Client_ID =" + ctx.GetAD_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                            else
                                _sqlQuery.Append("SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND AD_Client_ID =" + ctx.GetAD_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                        }
                        else if (WindowID == Util.GetValueOfInt(Windows.Shipment))
                            _sqlQuery.Append("SELECT C_Order_ID FROM M_InOut WHERE IsActive = 'Y' AND IsSOTrx = 'Y' AND M_InOut_ID = " + recordID);

                        if (_sqlQuery.Length > 0)
                            ordID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

                        if (ordID > 0)
                        {
                            string selColumn = "";
                            if (Env.IsModuleInstalled("DTD001_"))
                                selColumn = " , ol.DTD001_Org_ID ";

                            _sqlQuery.Clear();
                            _sqlQuery.Append("SELECT ol.C_OrderLine_ID, ol.M_Product_ID, ol.M_AttributeSetInstance_ID, ol.C_UOM_ID " + selColumn + " FROM C_OrderLine ol WHERE ol.C_Order_ID = " + ordID);
                            dsOrderLines = DB.ExecuteDataset(_sqlQuery.ToString());
                            if (dsOrderLines != null && dsOrderLines.Tables[0].Rows.Count > 0)
                            {
                                hasOrderLines = true;
                            }
                        }

                        if (ordID > 0)
                        {
                            io.SetC_Order_ID(ordID);
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
                                        _sqlQuery.Append("SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND IsSOTrx= 'Y' AND AD_Client_ID =" + ctx.GetAD_Client_ID()
                                            + " AND DocumentNo = '" + RefNo + "'");
                                    else if (WindowID == Util.GetValueOfInt(Windows.MaterialReceipt))
                                        _sqlQuery.Append("SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND IsSOTrx= 'N' AND AD_Client_ID =" + ctx.GetAD_Client_ID()
                                            + " AND DocumentNo = '" + RefNo + "'");
                                    else
                                        _sqlQuery.Append("SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND AD_Client_ID =" + ctx.GetAD_Client_ID() + " AND DocumentNo = '" + RefNo + "'");
                                }
                                else if (WindowID == Util.GetValueOfInt(Windows.Shipment))
                                    _sqlQuery.Append("SELECT C_Order_ID FROM M_InOut WHERE IsActive = 'Y' AND IsSOTrx = 'Y' AND M_InOut_ID = " + recordID);

                                if (_sqlQuery.Length > 0)
                                    ordID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

                                if (ordID > 0)
                                {
                                    string selColumn = "";
                                    if (Env.IsModuleInstalled("DTD001_"))
                                        selColumn = " , ol.DTD001_Org_ID ";

                                    _sqlQuery.Clear();
                                    _sqlQuery.Append("SELECT ol.C_OrderLine_ID, ol.M_Product_ID, ol.M_AttributeSetInstance_ID, ol.C_UOM_ID " + selColumn
                                        + " FROM C_OrderLine ol WHERE ol.C_Order_ID = " + ordID);
                                    dsOrderLines = DB.ExecuteDataset(_sqlQuery.ToString());
                                    if (dsOrderLines != null && dsOrderLines.Tables[0].Rows.Count > 0)
                                        hasOrderLines = true;
                                }

                                if (ordID > 0)
                                {
                                    io.SetC_Order_ID(ordID);
                                    if (!io.Save())
                                        saved = false;
                                }
                            }

                            po = tbl.GetPO(ctx, lineID, null);
                            po.Set_ValueNoCheck("AD_Client_ID", io.GetAD_Client_ID());
                            po.Set_ValueNoCheck("AD_Org_ID", io.GetAD_Org_ID());
                            po.Set_Value("M_Product_ID", Util.GetValueOfInt(product[i]));
                            po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                            po.Set_Value("MovementQty", Util.GetValueOfDecimal(qty[i]));
                            po.Set_ValueNoCheck("M_InOut_ID", recordID);
                            if (hasOrderLines)
                            {
                                DataRow[] drOL = null;
                                if (WindowID == Util.GetValueOfInt(Windows.MaterialReceipt))
                                {
                                    if (Env.IsModuleInstalled("DTD001_"))
                                        drOL = dsOrderLines.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(attribute[i])
                                            + "AND C_UOM_ID = " + Util.GetValueOfInt(uoms[i]) + " AND DTD001_Org_ID = " + ctx.GetAD_Org_ID());
                                }
                                if (!(drOL != null && drOL.Length > 0))
                                {
                                    drOL = dsOrderLines.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(attribute[i])
                                        + "AND C_UOM_ID = " + Util.GetValueOfInt(uoms[i]));
                                    if (!(drOL != null && drOL.Length > 0))
                                        drOL = dsOrderLines.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(attribute[i])
                                        + "AND C_UOM_ID <> " + Util.GetValueOfInt(uoms[i]));
                                    if (!(drOL != null && drOL.Length > 0))
                                        drOL = dsOrderLines.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(attribute[i]));
                                    if (!(drOL != null && drOL.Length > 0))
                                        drOL = dsOrderLines.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]));
                                }
                                if (drOL != null && drOL.Length > 0)
                                    po.Set_ValueNoCheck("C_OrderLine_ID", Util.GetValueOfInt(drOL[0]["C_OrderLine_ID"]));
                            }
                            po.Set_Value("M_Locator_ID", Util.GetValueOfInt(Locator_ID));
                            if (Util.GetValueOfInt(attribute[i]) != 0)
                                po.Set_Value("M_AttributeSetInstance_ID", Util.GetValueOfInt(attribute[i]));

                            if (!io.IsSOTrx())
                            {
                                if (dsProPO == null && !fetchedProPurchasing)
                                {
                                    dsProPO = GetPurchaingProduct(ctx.GetAD_Client_ID());
                                    fetchedProPurchasing = true;
                                    if (dsProPO != null && dsProPO.Tables[0].Rows.Count > 0)
                                        hasProdsPurch = true;
                                }

                                int uomID = Util.GetValueOfInt(uoms[i]);
                                int uom = 0;
                                if (hasProdsPurch)
                                {
                                    DataRow[] dr = dsProPO.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND C_BPartner_ID = " + io.GetC_BPartner_ID());
                                    if (dr != null && dr.Length > 0)
                                        uom = Util.GetValueOfInt(dr[0]["C_UOM_ID"]);
                                }

                                if (uomID != 0)
                                {
                                    if (uomID != uom && uom != 0)
                                    {
                                        if (!fetchedUOMConv)
                                        {
                                            dsUOMConv = GetUOMConversions(ctx.GetAD_Client_ID());
                                            fetchedUOMConv = true;
                                            if (dsUOMConv != null && dsUOMConv.Tables[0].Rows.Count > 0)
                                                hasConversions = true;
                                        }

                                        if (hasConversions)
                                        {
                                            Decimal? Res = 0;
                                            DataRow[] drConv = dsUOMConv.Tables[0].Select(" C_UOM_ID = " + uomID + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + Util.GetValueOfInt(product[i]));
                                            if (drConv != null && drConv.Length > 0)
                                            {
                                                Res = Util.GetValueOfDecimal(drConv[0]["MultiplyRate"]);
                                                if (Res <= 0)
                                                {
                                                    drConv = dsUOMConv.Tables[0].Select(" C_UOM_ID = " + uomID + " AND C_UOM_To_ID = " + uom);
                                                    if (drConv != null && drConv.Length > 0)
                                                        Res = Util.GetValueOfDecimal(drConv[0]["MultiplyRate"]);
                                                }
                                            }

                                            if (Res > 0)
                                                po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]) * Res);
                                        }
                                        po.Set_ValueNoCheck("C_UOM_ID", uom);
                                    }
                                    else
                                        po.Set_ValueNoCheck("C_UOM_ID", uomID);
                                }
                            }
                            else
                                po.Set_ValueNoCheck("C_UOM_ID", Util.GetValueOfInt(uoms[i]));

                            if (po.Get_ColumnIndex("M_ProductContainer_ID") > 0 && ContainerID > 0)
                                po.Set_Value("M_ProductContainer_ID", ContainerID);

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
            else if (keyColName.ToUpper().Trim() == "M_PACKAGE_ID")
            {
                DataSet dsMoves = null;
                bool hasMoveLines = false;

                tbl = new MTable(ctx, 663, null);

                MPackage pkg = new MPackage(ctx, recordID, null);
                string RefNo = ReferenceNo[0];
                if (Util.GetValueOfString(RefNo) != "")
                {
                    _sqlQuery.Clear();
                    _sqlQuery.Append(@"SELECT ol.M_MovementLine_ID, ol.M_Product_ID, ol.MovementQty FROM M_MovementLine ol INNER JOIN M_Movement o ON ol.M_Movement_ID=o.M_Movement_ID
                                        WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.M_MovementLine_ID NOT IN
                                      (SELECT NVL(M_MovementLine_ID,0) FROM M_PackageLine WHERE M_Package_ID = " + recordID + ")");

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
                    po.Set_ValueNoCheck("AD_Client_ID", pkg.GetAD_Client_ID());
                    po.Set_ValueNoCheck("AD_Org_ID", pkg.GetAD_Org_ID());
                    po.Set_Value("M_Product_ID", Util.GetValueOfInt(product[i]));
                    po.Set_Value("Qty", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("M_Package_ID", recordID);

                    if (hasMoveLines)
                    {
                        if (sbLine.Length > 0)
                        {
                            sbWhereCond.Clear();
                            sbWhereCond.Append(" AND M_MovementLine_ID NOT IN ( " + sbLine + " ) ");
                        }
                        DataRow[] dr = dsMoves.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
                        if (dr != null && dr.Length > 0)
                        {
                            int MoveLineID = Util.GetValueOfInt(dr[0]["M_MovementLine_ID"]);
                            if (MoveLineID > 0)
                            {
                                if (sbLine.Length > 0)
                                    sbLine.Append(", " + dr[0]["M_MovementLine_ID"]);
                                else
                                    sbLine.Append(dr[0]["M_MovementLine_ID"]);
                                po.Set_Value("M_MovementLine_ID", MoveLineID);
                                po.Set_Value("DTD001_TotalQty", Util.GetValueOfInt(dr[0]["MovementQty"]));
                            }
                        }
                    }
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        po.Set_Value("M_AttributeSetInstance_ID", Util.GetValueOfInt(attribute[i]));
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
            else if (keyColName.ToUpper().Trim() == "M_INVENTORY_ID")
            {
                tbl = new MTable(ctx, 322, null);
                MInventory inv = new MInventory(ctx, recordID, null);

                if (Locator_ID <= 0)
                {
                    Locator_ID = hasDefaultLocator(inv.GetM_Warehouse_ID());
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
                    DataSet dsInvLine = DB.ExecuteDataset("SELECT M_InventoryLine_ID, M_Product_ID, M_AttributeSetInstance_ID FROM M_InventoryLine WHERE IsActive ='Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND M_Inventory_ID = " + recordID);
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
                            _sqlQuery.Append("SELECT M_Locator_ID FROM M_Locator WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND Value = '" + RefNo + "'");
                            RefLocatorID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));
                        }

                        if (RefLocatorID > 0)
                            Locator_ID = RefLocatorID;

                        dsStockedQty = DB.ExecuteDataset("SELECT QtyOnHand, M_Product_ID, M_AttributeSetInstance_ID FROM M_Storage WHERE M_Locator_ID = " + Locator_ID + " AND AD_Client_ID = " + ctx.GetAD_Client_ID());
                        if (dsStockedQty != null && dsStockedQty.Tables[0].Rows.Count > 0)
                            hasStock = true;
                    }
                    // For Internal Use Inventory Window
                    else if (WindowID == Util.GetValueOfInt(Windows.InternalUse))
                    {
                        _sqlQuery.Clear();
                        _sqlQuery.Append("SELECT C_Charge_ID FROM C_Charge WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND DTD001_ChargeType = 'INV'");
                        Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

                        if (Util.GetValueOfString(RefNo) != "")
                        {
                            _sqlQuery.Clear();
                            _sqlQuery.Append(@"SELECT ol.M_RequisitionLine_ID,  ol.M_Product_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o
                                    ON ol.M_Requisition_ID =o.M_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.M_RequisitionLine_ID NOT IN
                                      (SELECT NVL(M_Requisitionline_ID,0) FROM M_InventoryLine WHERE M_Inventory_ID = " + recordID + ")");

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
                            DataRow[] drInve = dsInvLine.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(attribute[i]));
                            if (drInve != null && drInve.Length > 0)
                                lineID = Util.GetValueOfInt(drInve[0]["M_InventoryLine_ID"]);
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
                            po.Set_ValueNoCheck("AD_Client_ID", inv.GetAD_Client_ID());
                            po.Set_ValueNoCheck("AD_Org_ID", inv.GetAD_Org_ID());
                            if (Util.GetValueOfInt(Locator_ID) > 0)
                                po.Set_Value("M_Locator_ID", Util.GetValueOfInt(Locator_ID));
                            else
                                po.Set_Value("M_Locator_ID", Locator_ID);
                            po.Set_Value("M_Product_ID", Util.GetValueOfInt(product[i]));
                            po.Set_ValueNoCheck("M_Inventory_ID", recordID);
                            if (Util.GetValueOfInt(attribute[i]) != 0)
                                po.Set_Value("M_AttributeSetInstance_ID", Util.GetValueOfInt(attribute[i]));
                            else
                                po.Set_Value("M_AttributeSetInstance_ID", 0);
                            // when column index found, then insert UOM on requisition line
                            if (po.Get_ColumnIndex("C_UOM_ID") > 0)
                                po.Set_Value("C_UOM_ID", Util.GetValueOfInt(uoms[i]));
                            if (po.Get_ColumnIndex("M_ProductContainer_ID") > 0 && ContainerID > 0)
                                po.Set_Value("M_ProductContainer_ID", ContainerID);
                            po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));

                            // JID_1700: when saving Product from Cart, UOM Conversion was not working 
                            po.Set_Value("AdjustmentType", MInventoryLine.ADJUSTMENTTYPE_AsOnDateCount);

                            if (WindowID == Util.GetValueOfInt(Windows.PhysicalInventory))
                            {
                                Decimal? qtyBook = 0;
                                if (hasStock)
                                {
                                    DataRow[] drStock = dsStockedQty.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(attribute[i]));
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
                                        _sqlQuery.Append(@"SELECT ol.M_RequisitionLine_ID,  ol.M_Product_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o
                                    ON ol.M_Requisition_ID =o.M_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.M_RequisitionLine_ID NOT IN
                                      (SELECT NVL(M_Requisitionline_ID,0) FROM M_InventoryLine WHERE M_Inventory_ID = " + recordID + ")");

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
                                    po.Set_Value("C_Charge_ID", Asset_ID);
                                po.Set_Value("IsInternalUse", true);
                                if (hasReqLines)
                                {
                                    if (sbLine.Length > 0)
                                    {
                                        sbWhereCond.Clear();
                                        sbWhereCond.Append(" AND M_RequisitionLine_ID NOT IN ( " + sbLine + " ) ");
                                    }
                                    DataRow[] dr = dsReqs.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
                                    if (dr != null && dr.Length > 0)
                                    {
                                        int ReqLineID = Util.GetValueOfInt(dr[0]["M_RequisitionLine_ID"]);
                                        if (ReqLineID > 0)
                                        {
                                            if (sbLine.Length > 0)
                                                sbLine.Append(", " + dr[0]["M_RequisitionLine_ID"]);
                                            else
                                                sbLine.Append(dr[0]["M_RequisitionLine_ID"]);
                                            po.Set_Value("M_RequisitionLine_ID", ReqLineID);
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
            else if (keyColName.ToUpper().Trim() == "C_PROJECT_ID")
            {
                tbl = new MTable(ctx, 434, null);
                MProject proj = new MProject(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    int _m_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = 0;
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("AD_Client_ID", AD_Client_ID);
                    po.Set_ValueNoCheck("AD_Org_ID", proj.GetAD_Org_ID());
                    po.Set_Value("M_Product_ID", _m_Product_ID);
                    po.Set_Value("PLANNEDQTY", Util.GetValueOfDecimal(qty[i]));
                    po.Set_Value("INVOICEDQTY", Util.GetValueOfDecimal(qty[i]));
                    po.Set_ValueNoCheck("C_Project_ID", recordID);

                    Dictionary<string, Decimal?> pPrice = GetPrices(dsProPrice, dsUOMConv, AD_Client_ID, _m_Product_ID,
                        _attribute_ID, Util.GetValueOfInt(po.Get_Value("C_UOM_ID")), dsProducts,
                        Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), M_DiscountSchema_ID, bpFlatDiscount);

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
            else if (keyColName.ToUpper().Trim() == "M_PRICELIST_ID")
            {
                for (int i = 0; i < product.Count; i++)
                {
                    int _m_Product_ID = Util.GetValueOfInt(product[i]);
                    int _attribute_ID = Util.GetValueOfInt(attribute[i]);
                    int _uom_ID = Util.GetValueOfInt(uoms[i]);
                    MProductPrice pr = null;
                    string sql = "SELECT * FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + recordID + " AND M_Product_ID=" + _m_Product_ID;
                    if (Env.IsModuleInstalled("VAPRC_"))
                    {
                        sql += " AND M_AttributeSetInstance_ID = " + _attribute_ID;
                    }
                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        sql += " AND C_UOM_ID = " + _uom_ID;
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
                        pr = new MProductPrice(ctx, recordID, _m_Product_ID, null);
                        if (Env.IsModuleInstalled("VAPRC_"))
                        {
                            pr.SetM_AttributeSetInstance_ID(_attribute_ID);
                        }
                        if (Env.IsModuleInstalled("ED011_"))
                        {
                            pr.SetC_UOM_ID(_uom_ID);
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
            else if (keyColName.ToUpper().Trim() == "M_REQUISITION_ID")
            {
                tbl = new MTable(ctx, 703, null);
                MRequisition inv = new MRequisition(ctx, recordID, null);

                for (int i = 0; i < product.Count; i++)
                {
                    po = tbl.GetPO(ctx, lineID, null);
                    po.Set_ValueNoCheck("AD_Client_ID", AD_Client_ID);
                    po.Set_ValueNoCheck("AD_Org_ID", AD_Org_ID);
                    po.Set_Value("M_Product_ID", Util.GetValueOfInt(product[i]));
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
                    //    po.Set_Value("C_OrderLineLine_ID", Util.GetValueOfInt(oline_ID[i]));
                    //}
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        po.Set_Value("M_AttributeSetInstance_ID", Util.GetValueOfInt(attribute[i]));
                    }
                    else
                    {
                        po.Set_Value("M_AttributeSetInstance_ID", 0);
                    }
                    // when column index found, then insert UOM on requisition line
                    if (po.Get_ColumnIndex("C_UOM_ID") > 0)
                    {
                        po.Set_ValueNoCheck("C_UOM_ID", Util.GetValueOfInt(uoms[i]));
                    }
                    po.Set_ValueNoCheck("M_Requisition_ID", recordID);

                    if (Env.IsModuleInstalled("ED011_"))
                    {
                        Dictionary<string, Decimal?> pPrice = GetPrices(dsProPrice, dsUOMConv, AD_Client_ID, Util.GetValueOfInt(product[i]),
                            Util.GetValueOfInt(attribute[i]), Util.GetValueOfInt(po.Get_Value("C_UOM_ID")), dsProducts,
                            Util.GetValueOfDecimal(po.Get_Value("QtyEntered")), M_DiscountSchema_ID, bpFlatDiscount);

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
            if (keyColName.ToUpper().Trim() == "M_MOVEMENT_ID")
            {
                DataSet dsReqs = null;
                bool hasReqLines = false;

                DataSet dsAssets = null;
                bool hasAssets = false;

                tbl = new MTable(ctx, 324, null);
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
                        _sqlQuery.Append("SELECT A_Asset_ID, M_Product_ID, NVL(M_AttributeSetInstance_ID,0) AS M_AttributeSetInstance_ID FROM A_Asset WHERE IsActive = 'Y' AND AD_Client_ID = "
                            + ctx.GetAD_Client_ID());
                        dsAssets = DB.ExecuteDataset(_sqlQuery.ToString());
                        if (dsAssets != null && dsAssets.Tables[0].Rows.Count > 0)
                            hasAssets = true;
                    }

                    AD_Client_ID = mov.GetAD_Client_ID();

                    if (Util.GetValueOfString(RefNo) != "")
                    {
                        _sqlQuery.Clear();
                        _sqlQuery.Append(@"SELECT ol.M_RequisitionLine_ID, ol.M_Product_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o
                                    ON ol.M_Requisition_ID =o.M_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.M_RequisitionLine_ID NOT IN
                                      (SELECT NVL(M_Requisitionline_ID,0) FROM M_MovementLine WHERE M_Movement_ID = " + recordID + ")");

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
                                _sqlQuery.Append(@"SELECT ol.M_RequisitionLine_ID, ol.M_Product_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o
                                    ON ol.M_Requisition_ID =o.M_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.M_RequisitionLine_ID NOT IN
                                      (SELECT NVL(M_Requisitionline_ID,0) FROM M_MovementLine WHERE M_Movement_ID = " + recordID + ")");

                                dsReqs = DB.ExecuteDataset(_sqlQuery.ToString());
                                if (dsReqs != null && dsReqs.Tables[0].Rows.Count > 0)
                                    hasReqLines = true;
                            }
                        }


                        po = tbl.GetPO(ctx, lineID, null);

                        int M_AttributeSetInstance_ID = Util.GetValueOfInt(attribute[i]);
                        int M_Product_ID = Util.GetValueOfInt(product[i]);

                        po.Set_ValueNoCheck("AD_Client_ID", AD_Client_ID);
                        po.Set_ValueNoCheck("AD_Org_ID", mov.GetAD_Org_ID());
                        po.Set_Value("M_Locator_ID", Util.GetValueOfInt(Locator_ID));
                        po.Set_Value("M_LocatorTo_ID", LocToID);
                        po.Set_Value("M_Product_ID", M_Product_ID);
                        po.Set_Value("MovementQty", Util.GetValueOfDecimal(qty[i]));
                        po.Set_ValueNoCheck("M_Movement_ID", recordID);
                        if (po.Get_ColumnIndex("QtyEntered") > 0)
                            po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
                        if (po.Get_ColumnIndex("C_UOM_ID") > 0)
                            po.Set_Value("C_UOM_ID", Util.GetValueOfInt(uoms[i]));
                        if (hasReqLines)
                        {
                            if (sbLine.Length > 0)
                            {
                                sbWhereCond.Clear();
                                sbWhereCond.Append(" AND M_RequisitionLine_ID NOT IN ( " + sbLine + " ) ");
                            }
                            DataRow[] dr = dsReqs.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
                            if (dr != null && dr.Length > 0)
                            {
                                int ReqLineID = Util.GetValueOfInt(dr[0]["M_RequisitionLine_ID"]);
                                if (ReqLineID > 0)
                                {
                                    if (sbLine.Length > 0)
                                        sbLine.Append(", " + dr[0]["M_RequisitionLine_ID"]);
                                    else
                                        sbLine.Append(dr[0]["M_RequisitionLine_ID"]);
                                    po.Set_Value("M_RequisitionLine_ID", ReqLineID);
                                }
                            }
                        }
                        if (M_AttributeSetInstance_ID != 0)
                        {
                            po.Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
                            if (hasAssets)
                            {
                                DataRow[] drAst = dsAssets.Tables[0].Select(" M_Product_ID = " + M_Product_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID);
                                if (drAst != null && drAst.Length > 0)
                                {
                                    if (Util.GetValueOfInt(drAst[0]["A_Asset_ID"]) > 0)
                                        po.Set_Value("A_Asset_ID", Util.GetValueOfInt(drAst[0]["A_Asset_ID"]));
                                }
                            }
                        }

                        if (po.Get_ColumnIndex("M_ProductContainer_ID") > 0 && ContainerID > 0)
                            po.Set_Value("M_ProductContainer_ID", ContainerID);

                        if (po.Get_ColumnIndex("Ref_M_ProductContainerTo_ID") > 0 && ContainerToID > 0)
                            po.Set_Value("Ref_M_ProductContainerTo_ID", ContainerToID);

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
        /// <param name="M_Product_ID"></param>
        /// <param name="prodName"></param>
        private void SetErrorMessages(List<string> errorKeys, List<string> errorProdLines, bool hasProducts, DataSet dsProducts, int M_Product_ID, StringBuilder prodName)
        {
            ValueNamePair pp = VLogger.RetrieveError();
            prodName.Clear();
            if (hasProducts)
            {
                DataRow[] drPName = dsProducts.Tables[0].Select("M_Product_ID = " + M_Product_ID);
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
        //            MTable tbl = null;
        //            PO po = null;
        //            int count = 0;

        //            DataSet dsReqs = null;
        //            bool hasReqLines = false;

        //            #region Internal Use Inventory
        //            if (keyColName.ToUpper().Trim() == "M_INVENTORY_ID")
        //            {
        //                _sqlQuery.Clear();
        //                _sqlQuery.Append("SELECT C_Charge_ID FROM C_Charge WHERE IsActive = 'Y' AND AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND DTD001_ChargeType = 'INV'");

        //                int Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));

        //                tbl = new MTable(ctx, 322, null);
        //                MInventory inv = new MInventory(ctx, recordID, null);

        //                if (Locator_ID <= 0)
        //                {
        //                    Locator_ID = hasDefaultLocator(inv.GetM_Warehouse_ID());
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
        //                        _sqlQuery.Append(@"SELECT ol.M_RequisitionLine_ID,  ol.M_Product_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o
        //                                    ON ol.M_Requisition_ID =o.M_Requisition_ID WHERE o.Documentno = '" + Util.GetValueOfString(RefNo) + @"' AND ol.M_RequisitionLine_ID NOT IN
        //                                      (SELECT NVL(M_Requisitionline_ID,0) FROM M_InventoryLine WHERE M_Inventory_ID = " + recordID + ")");

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
        //                        po.Set_ValueNoCheck("AD_Client_ID", inv.GetAD_Client_ID());
        //                        po.Set_ValueNoCheck("AD_Org_ID", inv.GetAD_Org_ID());
        //                        po.Set_Value("M_Product_ID", Util.GetValueOfInt(product[i]));
        //                        po.Set_Value("QtyEntered", Util.GetValueOfDecimal(qty[i]));
        //                        po.Set_Value("QtyInternalUse", Util.GetValueOfDecimal(qty[i]));
        //                        po.Set_Value("M_Locator_ID", Util.GetValueOfInt(Locator_ID));
        //                        if (Asset_ID > 0)
        //                            po.Set_Value("C_Charge_ID", Asset_ID);
        //                        po.Set_ValueNoCheck("M_Inventory_ID", recordID);
        //                        po.Set_Value("IsInternalUse", true);
        //                        po.Set_Value("C_UOM_ID", Util.GetValueOfInt(uom[i]));
        //                        if (hasReqLines)
        //                        {
        //                            if (sbLine.Length > 0)
        //                            {
        //                                sbWhereCond.Clear();
        //                                sbWhereCond.Append(" AND M_RequisitionLine_ID NOT IN ( " + sbLine + " ) ");
        //                            }
        //                            DataRow[] dr = dsReqs.Tables[0].Select(" M_Product_ID = " + Util.GetValueOfInt(product[i]) + sbWhereCond);
        //                            if (dr != null && dr.Length > 0)
        //                            {
        //                                int ReqLineID = Util.GetValueOfInt(dr[0]["M_RequisitionLine_ID"]);
        //                                if (ReqLineID > 0)
        //                                {
        //                                    if (sbLine.Length > 0)
        //                                        sbLine.Append(", " + dr[0]["M_RequisitionLine_ID"]);
        //                                    else
        //                                        sbLine.Append(dr[0]["M_RequisitionLine_ID"]);
        //                                    po.Set_Value("M_RequisitionLine_ID", ReqLineID);
        //                                }
        //                            }
        //                        }
        //                        if (Util.GetValueOfInt(attribute[i]) != 0)
        //                            po.Set_Value("M_AttributeSetInstance_ID", Util.GetValueOfInt(attribute[i]));
        //                        else
        //                            po.Set_Value("M_AttributeSetInstance_ID", 0);

        //                        if (po.Get_ColumnIndex("M_ProductContainer_ID") > 0 && ContainerID > 0)
        //                            po.Set_Value("M_ProductContainer_ID", ContainerID);

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

        public bool SetProductQtyStockTrasfer(int recordID, string keyColName, int AD_Table_ID, List<string> product, List<string> uom, List<string> attribute, List<string> qty, List<string> locID, int LocToID, int lineID, int ContainerID, VAdvantage.Utility.Ctx ctx)
        {
            MTable tbl = null;
            PO po = null;

            tbl = new MTable(ctx, AD_Table_ID, null);
            for (int i = 0; i < product.Count; i++)
            {
                po = tbl.GetPO(ctx, lineID, null);
                po.Set_ValueNoCheck("AD_Client_ID", ctx.GetAD_Client_ID());
                po.Set_ValueNoCheck("AD_Org_ID", ctx.GetAD_Org_ID());
                po.Set_Value("M_Locator_ID", Util.GetValueOfInt(locID[i]));
                po.Set_Value("M_LocatorTo_ID", LocToID);
                po.Set_Value("M_Product_ID", Util.GetValueOfInt(product[i]));
                po.Set_Value("MovementQty", Util.GetValueOfDecimal(qty[i]));
                po.Set_ValueNoCheck(keyColName, recordID);
                //if (Util.GetValueOfInt(AssetID[i]) > 0)
                //    po.Set_Value("A_Asset_ID", Util.GetValueOfInt(AssetID[i]));
                //if (Util.GetValueOfInt(oline_ID[i]) > 0)
                //    po.Set_Value("M_RequisitionLine_ID", Util.GetValueOfInt(oline_ID[i]));
                if (Util.GetValueOfInt(attribute[i]) != 0)
                    po.Set_Value("M_AttributeSetInstance_ID", Util.GetValueOfInt(attribute[i]));
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
            int M_Product_ID = 0;
            string attributeNo = "", isLot = "", IsSerNo = "", IsGuaranteeDate = "", RefNo = "";
            DateTime? expiryDate = null;
            if (paramValue.Length > 0)
            {
                M_Product_ID = Util.GetValueOfInt(paramValue[0]);
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
            MAttributeSetInstance _mast = MAttributeSetInstance.Get(Env.GetCtx(), 0, M_Product_ID);
            if (!string.IsNullOrEmpty(attributeNo))
            {
                qry = "SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE M_Product_ID = " + M_Product_ID + "AND UPC = '" + attributeNo + "'";
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
                    qry = @"SELECT M_AttributeSetInstance_ID FROM M_AttributeSetINstance";

                    if (sql.Length > 0)
                    {
                        sql.Insert(0, " where ");
                        qry += sql + " order by m_attributesetinstance_id";
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
                                attrID = _mast.GetM_AttributeSetInstance_ID();
                                name = _mast.GetDescription();
                            }
                        }
                    }
                }
                if (attrID > 0)
                {
                    MAttributeSetInstance mas = new MAttributeSetInstance(ctx, attrID, null);
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
            query.Append(@"SELECT DISTINCT M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID = " + ProductId);
            int productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
            bool isCalulate = false;

            // Is flat Discount
            query.Clear();
            query.Append("SELECT  DiscountType  FROM M_DiscountSchema WHERE "
                      + "M_DiscountSchema_ID = " + DiscountSchemaId + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId);
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
                query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_ID = " + ProductId
                                                                           + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
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
                query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID = " + productCategoryId
                                                                           + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
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
                query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID IS NULL AND m_product_id IS NULL "
                                                                           + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
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

        public List<VariantData> GetVariants(int M_Product_ID, int M_warehouse_ID, int ParentRec_ID, int M_AttributeSetInstance_ID, string AttributeCode, Ctx ctx)
        {
            List<VariantData> _Variants = new List<VariantData>();
            StringBuilder _Str = new StringBuilder();
            string Sql = "";
            DataSet _DsVariants = null;
            _Str.Append("SELECT wh.M_warehouse_ID,"
                        + " wh.Name AS Warehouse,"
                        + " asi.DESCRIPTION AS variant,"
                        + " M_Storage.M_Product_id,"
                        + " 1  AS Quantity,"
                        + " BOMQTYONHANDATTR(M_Storage.M_Product_ID,M_Storage.M_AttributeSetInstance_ID,wh.M_warehouse_ID,0) AS QtyOnHand,"
                        + " (BOMQTYONHANDATTR(M_Storage.M_Product_ID,M_Storage.M_AttributeSetInstance_ID,wh.M_warehouse_ID,0)- "
                        + " BOMQTYRESERVEDATTR(M_Storage.M_Product_ID,M_Storage.M_AttributeSetInstance_ID,wh.M_warehouse_ID,0)) AS QtyAvailable,"
                        + " BOMQTYRESERVEDATTR(M_Storage.M_Product_ID,M_Storage.M_AttributeSetInstance_ID,wh.M_warehouse_ID,0) AS QtyReserved,"
                        + " BOMQTYORDEREDATTR (M_Storage.M_Product_ID,M_Storage.M_AttributeSetInstance_ID,wh.M_warehouse_ID,0) AS QtyOrdered,"
                        + " M_Storage.M_ATTRIBUTESETINSTANCE_ID"
                        + " FROM m_storage M_Storage INNER JOIN m_locator loc ON (M_Storage.M_LOCATOR_ID=loc.M_Locator_ID)"
                        + " INNER JOIN M_warehouse wh ON (loc.M_WAREHOUSE_ID=wh.M_warehouse_ID) LEFT JOIN M_ATTRIBUTESETINSTANCE asi"
                        + " ON (M_Storage.M_ATTRIBUTESETINSTANCE_ID=asi.M_ATTRIBUTESETINSTANCE_ID) WHERE M_Storage.IsActive='Y' AND M_Storage.M_Product_ID =" + M_Product_ID + "");
            if (M_warehouse_ID > 0)
            {
                _Str.Append(" AND wh.M_WAREHOUSE_ID=" + M_warehouse_ID);
            }
            if (M_AttributeSetInstance_ID > 0)
            {
                _Str.Append(" AND M_Storage.M_ATTRIBUTESETINSTANCE_ID=" + M_AttributeSetInstance_ID);
            }
            if (AttributeCode != "")
            {
                _Str.Append("AND M_Storage.M_AttributeSetInstance_ID in (SELECT M_AttributeSetInstance_ID from M_ProductAttributes WHERE M_Product_ID=" + M_Product_ID + "" +
                            " AND UPPER(UPC) like UPPER('%" + AttributeCode + "%'))");
            }
            _Str.Append(" ORDER BY Variant");

            try
            {
                Sql = MRole.GetDefault(ctx).AddAccessSQL(_Str.ToString(), "M_Storage",
                                            MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
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
                        VData.M_AttributeSetInstance_ID = Util.GetValueOfInt(_DsVariants.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]);
                        VData.M_Product_ID = Util.GetValueOfInt(_DsVariants.Tables[0].Rows[i]["M_Product_id"]);
                        VData.M_Warehouse_ID = Util.GetValueOfInt(_DsVariants.Tables[0].Rows[i]["M_warehouse_ID"]);
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
            string sql = "SELECT AD_Window_ID FROM AD_Tab WHERE AD_Tab_ID = " + Util.GetValueOfInt(tab_ID);
            window_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return window_ID;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetWarehouse(Ctx ctx)
        {
            List<Dictionary<string, object>> retWare = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT M_Warehouse_ID, Value || ' - ' || Name AS ValueName FROM M_Warehouse WHERE IsActive='Y'",
                    "M_Warehouse", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO) + " ORDER BY Value";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retWare = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_Warehouse_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Warehouse_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ValueName"]);
                    retWare.Add(obj);
                }
            }
            return retWare;
        }

        public List<Dictionary<string, object>> GetUOM(Ctx ctx)
        {
            List<Dictionary<string, object>> reDIc = null;
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT C_UOM_ID, Name FROM C_UOM WHERE IsActive = 'Y'",
                    "C_UOM", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                reDIc = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
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
            string sql = "SELECT plv.M_PriceList_Version_ID,"
                     + " plv.Name || ' (' || c.ISO_Code || ')' AS ValueName "
                     + "FROM M_PriceList_Version plv, M_PriceList pl, C_Currency c "
                     + "WHERE plv.M_PriceList_ID=pl.M_PriceList_ID" + " AND pl.C_Currency_ID=c.C_Currency_ID"
                     + " AND plv.IsActive='Y' AND pl.IsActive='Y'";

            if (PriceList != 0)
            {
                sql += " AND EXISTS (SELECT * FROM M_PriceList xp WHERE xp.M_PriceList_ID=" + PriceList + ")";
            }
            // Add Access & Order
            var qry = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_PriceList_Version", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO) + " ORDER BY plv.Name";
            DataSet ds = DB.ExecuteDataset(qry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retPriceList = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_PriceList_Version_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_PriceList_Version_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["ValueName"]);
                    retPriceList.Add(obj);
                }
            }
            return retPriceList;
        }

        // Added by Bharat on 31 May 2017
        public int GetDefaultPriceList(Ctx ctx)
        {
            string sql = "SELECT M_PriceList_ID FROM M_PriceList WHERE IsActive = 'Y' AND IsDefault = 'Y'";
            int PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return PriceList_ID;
        }

        // Added by Bharat on 31 May 2017
        public List<Dictionary<string, object>> GetPriceListVersion(int PriceList, Ctx ctx)
        {
            List<Dictionary<string, object>> retPriceList = null;
            string sql = "SELECT plv.M_PriceList_Version_ID, plv.ValidFrom "
               + "FROM M_PriceList pl, M_PriceList_Version plv "
               + "WHERE pl.M_PriceList_ID=plv.M_PriceList_ID"
               + " AND plv.IsActive='Y'"
               + " AND pl.M_PriceList_ID=" + PriceList					//	1
               + " ORDER BY plv.ValidFrom DESC";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retPriceList = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_PriceList_Version_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_PriceList_Version_ID"]);
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
            string sql = MRole.GetDefault(ctx).AddAccessSQL("SELECT M_AttributeSet_ID, Name FROM M_AttributeSet WHERE IsActive='Y'",
                    "M_AttributeSet", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO) + " ORDER BY Name";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retAttrSet = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_AttributeSet_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
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
            string sql = "SELECT cl.M_Product_ID,prd.Name,prd.Value,cl.VAICNT_Quantity,cl.M_AttributeSetInstance_ID,cl.C_UOM_ID,uom.Name as UOM,ic.VAICNT_ReferenceNo,cl.VAICNT_InventoryCountLine_ID,"
                        + " ats.Description FROM VAICNT_InventoryCount ic INNER JOIN VAICNT_InventoryCountLine cl ON ic.VAICNT_InventoryCount_ID = cl.VAICNT_InventoryCount_ID"
                        + " INNER JOIN M_Product prd ON cl.M_Product_ID = prd.M_Product_ID INNER JOIN C_UOM uom ON cl.C_UOM_ID = uom.C_UOM_ID LEFT JOIN M_AttributeSetInstance ats"
                         + " ON cl.M_AttributeSetInstance_ID = ats.M_AttributeSetInstance_ID WHERE cl.IsActive = 'Y' AND ic.VAICNT_InventoryCount_ID IN (" + countID + ")";
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
                        sqlSB.Append(@"SELECT ol.M_Product_ID, ol.C_UOM_ID, ol.M_AttributeSetInstance_ID, o.DocumentNo, ol.c_OrderLine_ID AS LineID
                                    FROM C_Order o INNER JOIN C_OrderLine ol ON (ol.C_Order_ID = o.C_Order_ID) WHERE o.IsActive = 'Y' AND o.AD_Client_ID = " + ctx.GetAD_Client_ID()
                                    + @" AND o.IsSOTrx = 'N' AND o.DocStatus IN ('CO', 'CL') AND o.DocumentNo IN (
                                    SELECT VAICNT_ReferenceNo FROM VAICNT_InventoryCount WHERE VAICNT_InventoryCount_ID IN (" + countID + "))");
                    }
                    else if (Util.GetValueOfInt(Windows.Shipment) == WindowID)
                    {
                        sqlSB.Append(@"SELECT ol.M_Product_ID, ol.C_UOM_ID, ol.M_AttributeSetInstance_ID, o.DocumentNo, ol.c_OrderLine_ID AS LineID
                                    FROM C_Order o INNER JOIN C_OrderLine ol ON (ol.C_Order_ID = o.C_Order_ID) WHERE o.IsActive = 'Y' AND o.AD_Client_ID = " + ctx.GetAD_Client_ID()
                                    + @" AND o.IsSOTrx = 'Y' AND o.DocStatus IN ('CO') AND o.DocumentNo IN (
                                    SELECT VAICNT_ReferenceNo FROM VAICNT_InventoryCount WHERE VAICNT_InventoryCount_ID IN (" + countID + "))");
                    }
                    else if (Util.GetValueOfInt(Windows.InternalUse) == WindowID || Util.GetValueOfInt(Windows.InventoryMove) == WindowID)
                    {
                        sqlSB.Append(@"SELECT ol.M_RequisitionLine_ID AS LineID, o.DocumentNo  ol.M_Product_ID, ol.M_AttributeSetInstance_ID, ol.C_UOM_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o
                                    ON ol.M_Requisition_ID =o.M_Requisition_ID WHERE o.IsActive = 'Y' AND o.AD_Client_ID = " + ctx.GetAD_Client_ID()
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
                    obj["M_Product_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]);
                    obj["Name"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    obj["VAICNT_Quantity"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAICNT_Quantity"]);
                    obj["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                    obj["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    obj["UOM"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["UOM"]);
                    obj["VAICNT_ReferenceNo"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["VAICNT_ReferenceNo"]);
                    obj["VAICNT_InventoryCountLine_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAICNT_InventoryCountLine_ID"]);
                    obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    if (hasLines && checkRefLine)
                    {
                        DataRow[] dr = dsLines.Tables[0].Select(" M_Product_ID = " + obj["M_Product_ID"] + " AND M_AttributeSetInstance_ID = " + obj["M_AttributeSetInstance_ID"] + " AND C_UOM_ID = " + obj["C_UOM_ID"] + " AND DocumentNo = '" + obj["VAICNT_ReferenceNo"] + "'");
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
        public DataSet GetPurchaingProduct(int AD_Client_ID)
        {
            DataSet dsProPurch = null;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT vdr.C_UOM_ID, vdr.C_BPartner_ID, p.M_Product_ID FROM M_Product p LEFT JOIN 
                            M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.AD_Client_ID = " + AD_Client_ID);
            dsProPurch = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsProPurch;
        }

        /// <summary>
        /// returns Price list version ID based on the Price List passed in the parameter
        /// </summary>
        /// <param name="M_PriceList_ID"></param>
        /// <returns>M_PriceList_Version_ID (int)</returns>
        public int GetPLVID(int M_PriceList_ID)
        {
            int M_PriceList_Version_ID = 0;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' 
                                AND M_PriceList_ID = " + M_PriceList_ID + @" AND ValidFrom <= SYSDATE ORDER BY ValidFrom DESC");
            M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));
            return M_PriceList_Version_ID;
        }

        /// <summary>
        /// function to fetch Products with UOM IDs for tenant
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <returns>Dataset of Products with UOM</returns>
        public DataSet GetProducts(int AD_Client_ID)
        {
            DataSet dsProds = null;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT M_Product_ID, C_UOM_ID, Name FROM M_Product WHERE AD_Client_ID = " + AD_Client_ID);
            dsProds = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsProds;
        }

        /// <summary>
        /// function to return product prices based on Tenant and Pricelist version
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <param name="M_PriceList_Version_ID"></param>
        /// <returns>Dataset of Product Prices</returns>
        public DataSet GetProductsPrice(int AD_Client_ID, int M_PriceList_Version_ID)
        {
            DataSet dsProdPrices = null;

            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT PriceList , PriceStd , PriceLimit, M_Product_ID, NVL(M_AttributeSetInstance_ID,0) AS M_AttributeSetInstance_ID, C_UOM_ID
                            FROM M_ProductPrice WHERE Isactive='Y' AND AD_Client_ID = " + AD_Client_ID + " AND M_PriceList_Version_ID = " + M_PriceList_Version_ID);
            dsProdPrices = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsProdPrices;
        }

        /// <summary>
        /// function to fetch UOM Conversions for the specified Tenant
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <returns>Dataset of UOM Conversions</returns>
        public DataSet GetUOMConversions(int AD_Client_ID)
        {
            DataSet dsConvs = null;
            _sqlQuery.Clear();
            _sqlQuery.Append(@"SELECT con.DivideRate, TRUNC(con.multiplyrate,4) AS MultiplyRate, con.C_UOM_ID, con.C_UOM_To_ID, con.M_Product_ID FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' AND con.AD_Client_ID = " + AD_Client_ID);
            dsConvs = DB.ExecuteDataset(_sqlQuery.ToString());
            return dsConvs;
        }

        /// <summary>
        /// function to fetch Prices for the product based on the conditions 
        /// of attributes and UOM Conversions
        /// </summary>
        /// <param name="dsProdPrice"></param>
        /// <param name="dsUOMCon"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_ASI_ID"></param>
        /// <param name="C_UOM_ID"></param>
        /// <param name="ProdUOM"></param>
        /// <param name="QtyEntered"></param>
        /// <param name="M_DiscountSchema_ID"></param>
        /// <param name="bpFlatDiscount"></param>
        /// <returns>Dictionary of Prices (PriceList, PriceLimit, PriceActual, PriceEntered)</returns>
        public Dictionary<string, Decimal?> GetPrices(DataSet dsProdPrice, DataSet dsUOMCon, int AD_Client_ID, int M_Product_ID, int M_ASI_ID, int C_UOM_ID, DataSet dsProducts,
            Decimal? QtyEntered, int M_DiscountSchema_ID, Decimal? bpFlatDiscount)
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
                drPrice = dsProdPrice.Tables[0].Select("M_Product_ID = " + M_Product_ID + " AND M_AttributeSetInstance_ID = " + M_ASI_ID + " AND C_UOM_ID = " + C_UOM_ID);
                // if price not found against above conditions then check against product and 0 Attirbute set instance and UOM
                if (!(drPrice != null && drPrice.Length > 0))
                {
                    drPrice = dsProdPrice.Tables[0].Select(" M_Product_ID = " + M_Product_ID + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID = " + C_UOM_ID);
                    // if price still not found then go for the conversions
                    if (!(drPrice != null && drPrice.Length > 0))
                    {
                        int ProdUOM = 100;
                        // Fetch Base UOM of the Product
                        if (dsProducts != null && dsProducts.Tables[0].Rows.Count > 0)
                        {
                            DataRow[] drPUOM = dsProducts.Tables[0].Select(" M_Product_ID = " + M_Product_ID);
                            if (drPUOM != null && drPUOM.Length > 0)
                                ProdUOM = Util.GetValueOfInt(drPUOM[0]["C_UOM_ID"]);
                        }

                        // Fetch conversions from Base UOM to selected UOM
                        if (dsUOMCon != null && dsUOMCon.Tables[0].Rows.Count > 0)
                        {
                            // Fetched conversion rate based on Product and UOM From to UOM TO
                            DataRow[] drRate = dsUOMCon.Tables[0].Select(" M_Product_ID = " + M_Product_ID + " AND C_UOM_ID = " + ProdUOM + " AND C_UOM_To_ID = " + C_UOM_ID);
                            if (drRate != null && drRate.Length > 0)
                            {
                                conversionRate = Util.GetValueOfDecimal(drRate[0]["DivideRate"]);
                                convertPrice = true;
                            }
                            // if conversion rate not found, then have to check conversion only for UOM From to UOM To without Product
                            if (conversionRate == 0)
                            {
                                drRate = dsUOMCon.Tables[0].Select(" C_UOM_ID = " + ProdUOM + " AND C_UOM_To_ID = " + C_UOM_ID);
                                if (drRate != null && drRate.Length > 0)
                                {
                                    conversionRate = Util.GetValueOfDecimal(drRate[0]["DivideRate"]);
                                    convertPrice = true;
                                }
                            }
                        }

                        // Check for Product Price from Pricelist against Product ID , Attribute Set Instance and Base UOM of Product (this case will run only
                        // if price not found initially against the UOM passed in the parameter)
                        drPrice = dsProdPrice.Tables[0].Select("M_Product_ID = " + M_Product_ID + " AND M_AttributeSetInstance_ID = " + M_ASI_ID + " AND C_UOM_ID = " + ProdUOM);
                        // if Price not found again
                        // then check for the price against 0 Attribute Set Instance
                        if (!(drPrice != null && drPrice.Length > 0))
                        {
                            drPrice = dsProdPrice.Tables[0].Select("M_Product_ID = " + M_Product_ID + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID = " + ProdUOM);
                        }
                    }
                }
            }

            // if Price found then calcluate prices with Conversion
            if (drPrice != null && drPrice.Length > 0)
            {
                convertedprice = FlatDiscount(M_Product_ID, AD_Client_ID, Util.GetValueOfDecimal(drPrice[0]["PriceStd"]), M_DiscountSchema_ID, bpFlatDiscount, QtyEntered);
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
        /// <param name="M_Warehouse_ID"></param>
        /// <returns>Locator ID</returns>
        public int hasDefaultLocator(int M_Warehouse_ID)
        {
            _sqlQuery.Clear();
            _sqlQuery.Append("SELECT M_Locator_ID FROM M_Locator WHERE M_Warehouse_ID = " + M_Warehouse_ID + " AND IsDefault = 'Y' AND IsActive = 'Y'");
            return Util.GetValueOfInt(DB.ExecuteScalar(_sqlQuery.ToString()));
        }
        //-----------------------------------------
    }

    public class InfoProduct
    {
        public int AD_Table_ID
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
        public string _AD_Session_ID { get; set; }
        public int _windowNo { get; set; }
    }

    public class InfoScanData
    {
        public List<ReferenceData> refData { get; set; }
        public int M_Locator_ID { get; set; }
        public int M_LocatorTo_ID { get; set; }
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
        public int M_Warehouse_ID { get; set; }
        public int M_AttributeSetInstance_ID { get; set; }
        public int M_Product_ID { get; set; }
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
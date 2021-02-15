using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Utility;
using VAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using System.Text;

namespace VIS.Models
{
    public class RelatedProductModel
    {
        /// <summary>
        /// Getting Product Data
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute</param>
        /// <param name="M_PriceList_ID">Price List</param>
        /// <param name="Table_ID">Table ID</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>Product Details</returns>
        public RelatedProductData GetProductData(Ctx ctx, int M_Product_ID, int M_AttributeSetInstance_ID, int C_UOM_ID, int M_PriceList_ID, int Table_ID, int Record_ID)
        {
            RelatedProductData res = null;
            DateTime? validFrom = null;

            if (MOrder.Table_ID.Equals(Table_ID))
            {

                validFrom = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT DateOrdered FROM C_Order WHERE C_Order_ID = " + Record_ID));
            }

            if (validFrom == null)
            {
                validFrom = DateTime.Now.Date;
            }

            int M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' 
                                AND M_PriceList_ID = " + M_PriceList_ID + @" AND ValidFrom <= " + DB.TO_DATE(validFrom, true) + " ORDER BY ValidFrom DESC"));

            string sql = "SELECT P.M_Product_ID, " + (C_UOM_ID > 0 ? C_UOM_ID.ToString() : "P.C_UOM_ID") + @" AS C_UOM_ID, P.Value, P.Name, 1 AS QtyEntered,
                    BOMPRICELISTUOM(P.M_PRODUCT_ID, PR.M_PRICELIST_VERSION_ID, PR.M_ATTRIBUTESETINSTANCE_ID, PR.C_UOM_ID) AS PriceList,
                    BOMPRICESTDUOM(P.M_PRODUCT_ID, PR.M_PRICELIST_VERSION_ID, PR.M_ATTRIBUTESETINSTANCE_ID, PR.C_UOM_ID) AS UnitPrice,
                    BOMPRICELIMITUOM(P.M_PRODUCT_ID, PR.M_PRICELIST_VERSION_ID, PR.M_ATTRIBUTESETINSTANCE_ID, PR.C_UOM_ID) AS LimitPrice
                    FROM M_PRODUCT P LEFT JOIN M_PRODUCTPRICE PR ON ( P.M_PRODUCT_ID = PR.M_PRODUCT_ID AND PR.ISACTIVE = 'Y' 
                    AND NVL(PR.M_AttributeSetInstance_ID, 0) = " + M_AttributeSetInstance_ID + " AND PR.M_PRICELIST_VERSION_ID = " + M_PriceList_Version_ID
                    + " AND PR.C_UOM_ID = " + (C_UOM_ID > 0 ? C_UOM_ID.ToString() : "P.C_UOM_ID") + @") WHERE P.M_Product_ID = " + M_Product_ID;
            DataSet _ds = DB.ExecuteDataset(sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                res = new RelatedProductData();
                res.Product_ID = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["M_Product_ID"]);
                res.Product = Util.GetValueOfString(_ds.Tables[0].Rows[0]["Name"]);
                res.AttributeSetInstance_ID = M_AttributeSetInstance_ID;
                res.Value = Util.GetValueOfString(_ds.Tables[0].Rows[0]["Value"]);
                //res.UOM = Util.GetValueOfString(_ds.Tables[0].Rows[0]["UName"]);
                res.UOM_ID = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_UOM_ID"]);
                res.QtyEntered = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["QtyEntered"]);
                res.PriceStd = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["UnitPrice"]);
                res.PriceList = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["PriceList"]);
            }
            return res;
        }

        /// <summary>
        /// Getting Related Product Data
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute</param>
        /// <param name="M_PriceList_ID">Price List</param>
        /// <param name="Table_ID">Table ID</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns></returns>
        public List<RelatedProductData> GetRelatedProduct(Ctx ctx, int M_Product_ID, int M_AttributeSetInstance_ID, int C_UOM_ID, int M_PriceList_ID, string Relatedtype, int Table_ID, int Record_ID)
        {
            List<RelatedProductData> retData = null;
            RelatedProductData res;
            DateTime? validFrom = null;

            if (MOrder.Table_ID.Equals(Table_ID))
            {

                validFrom = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT DateOrdered FROM C_Order WHERE C_Order_ID = " + Record_ID));
            }

            if (validFrom == null)
            {
                validFrom = DateTime.Now.Date;
            }

            int M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' 
                                AND M_PriceList_ID = " + M_PriceList_ID + @" AND ValidFrom <= " + DB.TO_DATE(validFrom, true) + " ORDER BY ValidFrom DESC"));

            string sql = @"SELECT P.M_Product_ID, " + (C_UOM_ID > 0 ? C_UOM_ID.ToString() : "P.C_UOM_ID") + @" AS C_UOM_ID, P.Value, R.Name, 1 AS QtyEntered,
                    R.RelatedProductType, (SELECT Name FROM AD_Ref_List WHERE AD_Reference_ID=(SELECT AD_Reference_ID FROM AD_Reference WHERE Name='M_RelatedProduct Type')
                    AND Value = R.RelatedProductType) AS RelatedType,
                    BOMPRICELISTUOM(P.M_PRODUCT_ID, PR.M_PRICELIST_VERSION_ID, PR.M_ATTRIBUTESETINSTANCE_ID, PR.C_UOM_ID) AS PriceList,
                    BOMPRICESTDUOM(P.M_PRODUCT_ID, PR.M_PRICELIST_VERSION_ID, PR.M_ATTRIBUTESETINSTANCE_ID, PR.C_UOM_ID) AS UnitPrice,
                    BOMPRICELIMITUOM(P.M_PRODUCT_ID, PR.M_PRICELIST_VERSION_ID, PR.M_ATTRIBUTESETINSTANCE_ID, PR.C_UOM_ID) AS LimitPrice
                    FROM M_RelatedProduct R INNER JOIN M_PRODUCT P ON (p.M_Product_ID = R.RelatedProduct_ID) LEFT JOIN M_PRODUCTPRICE PR ON ( P.M_PRODUCT_ID = PR.M_PRODUCT_ID AND PR.ISACTIVE = 'Y' 
                    AND NVL(PR.M_AttributeSetInstance_ID, 0) = " + M_AttributeSetInstance_ID + " AND PR.M_PRICELIST_VERSION_ID = " + M_PriceList_Version_ID
                    + "AND PR.C_UOM_ID = " + (C_UOM_ID > 0 ? C_UOM_ID.ToString() : "P.C_UOM_ID") + @") WHERE R.M_Product_ID = " + M_Product_ID;
            if (!String.IsNullOrEmpty(Relatedtype))
            {
                sql += " AND R.RelatedProductType = " + DB.TO_STRING(Relatedtype);
            }
            DataSet _ds = DB.ExecuteDataset(sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                retData = new List<RelatedProductData>();
                for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                {
                    res = new RelatedProductData();
                    res.Product_ID = Util.GetValueOfInt(_ds.Tables[0].Rows[i]["M_Product_ID"]);
                    res.Product = Util.GetValueOfString(_ds.Tables[0].Rows[i]["Name"]);
                    res.RelatedProductType = Util.GetValueOfString(_ds.Tables[0].Rows[i]["RelatedProductType"]);
                    res.RelatedType = Util.GetValueOfString(_ds.Tables[0].Rows[i]["RelatedType"]);
                    res.AttributeSetInstance_ID = M_AttributeSetInstance_ID;
                    res.Value = Util.GetValueOfString(_ds.Tables[0].Rows[i]["Value"]);
                    //res.UOM = Util.GetValueOfString(_ds.Tables[0].Rows[i]["UName"]);
                    res.UOM_ID = Util.GetValueOfInt(_ds.Tables[0].Rows[i]["C_UOM_ID"]);
                    res.QtyEntered = Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["QtyEntered"]);
                    res.PriceStd = Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["UnitPrice"]);
                    res.PriceList = Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["PriceList"]);
                    retData.Add(res);
                }
            }
            return retData;
        }

        /// <summary>
        /// Creatd Related Product Lines
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="ReqLines">Related Product Data</param>
        /// <param name="Table_ID">Table ID</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>Message as string</returns>
        public string CreateRelatedLines(Ctx ctx, List<RelatedProductData> ReqLines, int Table_ID, int Record_ID)
        {
            string _msg = "";

            if (MOrder.Table_ID.Equals(Table_ID))
            {
                _msg = CreateSQLines(ctx, ReqLines, Record_ID);
            }

            else if (MProject.Table_ID.Equals(Table_ID))
            {
                _msg = CreateOppLines(ctx, ReqLines, Record_ID);
            }

            return _msg;
        }

        /// <summary>
        /// Create Sales Quotation Lines
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ReqLines">Related Product data</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>Message as string</returns>
        public string CreateSQLines(Ctx ctx, List<RelatedProductData> ReqLines, int Record_ID)
        {
            int TotalCount, SavedCount = 0;
            int LineNo;
            string msg;
            MOrderLine POLine;

            LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT MAX(Line) FROM C_OrderLine WHERE C_Order_ID=" + Record_ID));
            MOrder PO = new MOrder(ctx, Record_ID, null);
            TotalCount = ReqLines.Count;
            for (int i = 0; i < ReqLines.Count; i++)
            {
                LineNo += 10;
                POLine = new MOrderLine(PO);
                POLine.SetM_Product_ID(ReqLines[i].Product_ID);
                POLine.SetM_AttributeSetInstance_ID(ReqLines[i].AttributeSetInstance_ID);
                POLine.SetC_UOM_ID(ReqLines[i].UOM_ID);
                POLine.SetQtyEntered(ReqLines[i].QtyEntered);
                POLine.SetQtyOrdered(ReqLines[i].QtyEntered);
                POLine.SetDatePromised(PO.GetDatePromised());
                POLine.SetLine(LineNo);
                POLine.SetDateOrdered(DateTime.Now);
                POLine.SetPriceEntered(ReqLines[i].PriceStd);
                POLine.SetPriceActual(ReqLines[i].PriceStd);
                POLine.SetPriceList(ReqLines[i].PriceList);
                if (POLine.Save())
                {
                    SavedCount++;
                }
            }
            if (TotalCount == SavedCount)
            {
                msg = Msg.GetMsg(ctx, "VIS_SuccessFullyInserted");
            }
            else
            {
                msg = SavedCount + Msg.GetMsg(ctx, "VIS_NoOfLineSaved") + TotalCount + ".";
            }
            return msg;
        }

        /// <summary>
        /// Create Opportunity Lines
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ReqLines">Related Product data</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>Message as string</returns>
        public string CreateOppLines(Ctx ctx, List<RelatedProductData> ReqLines, int Record_ID)
        {
            int TotalCount, SavedCount = 0;
            int LineNo;
            Decimal Discount;
            string msg;
            MProjectLine POLine;

            LineNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT MAX(Line) FROM C_ProjectLine WHERE C_Project_ID=" + Record_ID));
            MProject PO = new MProject(ctx, Record_ID, null);
            TotalCount = ReqLines.Count;
            for (int i = 0; i < ReqLines.Count; i++)
            {
                LineNo += 10;
                POLine = new MProjectLine(PO);
                POLine.SetM_Product_ID(ReqLines[i].Product_ID);
                if (POLine.Get_ColumnIndex("M_AttributeSetInstance_ID") >= 0)
                {
                    POLine.SetM_AttributeSetInstance_ID(ReqLines[i].AttributeSetInstance_ID);
                }
                if (POLine.Get_ColumnIndex("C_UOM_ID") >= 0)
                {
                    POLine.Set_Value("C_UOM_ID", ReqLines[i].UOM_ID);
                }
                POLine.SetPlannedQty(ReqLines[i].QtyEntered);
                POLine.SetLine(LineNo);

                if (Math.Sign(ReqLines[i].PriceList) == 0)
                    Discount = Env.ZERO;
                else
                {
                    Decimal multiplier = Decimal.Round(Decimal.Divide(Decimal.Multiply(ReqLines[i].PriceStd, Env.ONEHUNDRED)
                        , ReqLines[i].PriceList), 2, MidpointRounding.AwayFromZero);
                    Discount = Decimal.Subtract(Env.ONEHUNDRED, multiplier);
                }
                POLine.SetDiscount(Discount);
                POLine.SetPlannedPrice(ReqLines[i].PriceStd);
                POLine.SetPriceList(ReqLines[i].PriceList);
                if (POLine.Save())
                {
                    SavedCount++;
                }
            }
            if (TotalCount == SavedCount)
            {
                msg = Msg.GetMsg(ctx, "VIS_SuccessFullyInserted");
            }
            else
            {
                msg = SavedCount + Msg.GetMsg(ctx, "VIS_NoOfLineSaved") + TotalCount + ".";
            }
            return msg;
        }
    }

    public class RelatedType
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }

    public class RelatedProductData
    {
        public int Product_ID { get; set; }
        public int AttributeSetInstance_ID { get; set; }
        public string Value { get; set; }
        public string Product { get; set; }
        public string RelatedType { get; set; }
        public string RelatedProductType { get; set; }
        // public string UOM { get; set; }
        public int UOM_ID { get; set; }
        public decimal QtyEntered { get; set; }
        public decimal PriceStd { get; set; }
        public decimal PriceList { get; set; }
    }
}
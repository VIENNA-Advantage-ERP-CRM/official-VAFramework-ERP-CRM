using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public enum TransactionType
    { 
        Order = 1,
        Invoice = 2,
        Requisition = 3,
        TimeExpense = 4,
        Contract = 5
    }

    public class MPriceListVersionModel
    {
        private string sql = "";
        private string _tableName = string.Empty;
        private string _keyColumnName = string.Empty;
        private string _columnName = string.Empty;
        public int GetM_PriceList_Version_ID(Ctx ctx, string fields)
        {
            int M_PriceList_ID=0, transactionId = 0, productId=0, uomId=0, attrSetInstId=0, tranType=0;

            if (!string.IsNullOrEmpty(fields))
            {
                string[] paramValue = fields.Split(',');
                M_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
                if(paramValue.Length >= 2)
                    transactionId = Util.GetValueOfInt(paramValue[1].ToString());
                if (paramValue.Length >= 3)
                    productId = Util.GetValueOfInt(paramValue[2].ToString());
                if (paramValue.Length >= 4)
                    uomId = Util.GetValueOfInt(paramValue[3].ToString());
                if (paramValue.Length >= 5)
                    attrSetInstId = Util.GetValueOfInt(paramValue[4].ToString());
                if (paramValue.Length >= 6)
                {
                    tranType = Util.GetValueOfInt(paramValue[5].ToString());
                    SetTableAndColumnName((TransactionType)tranType);
                }
            }

            return GetM_PriceList_Version_ID(ctx, M_PriceList_ID.ToString(), transactionId.ToString(), productId.ToString(), uomId.ToString(), attrSetInstId.ToString());

            //sql = "SELECT COUNT(*) FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID = " + M_PriceList_ID;
            //if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 1)
            //{
            //    sql = "SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID = " + M_PriceList_ID;
            //}
            //else
            //{
            //    sql = "SELECT (M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive='Y' AND M_PriceList_ID = " + M_PriceList_ID;
            //}
            //sql = "SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID = " + M_PriceList_ID + @" AND VALIDFROM <= SYSDATE ORDER BY VALIDFROM DESC";
            //return Util.GetValueOfInt(DB.ExecuteScalar(sql));
        }

        public int GetM_PriceList_Version_ID(Ctx ctx, string fields, TransactionType transactionType)
        {
            SetTableAndColumnName(transactionType);

            return GetM_PriceList_Version_ID(ctx, fields);
        }

        private void SetTableAndColumnName(TransactionType transactionType)
        {
            if (transactionType == TransactionType.Invoice)
            {
                _tableName = "C_Invoice";
                _keyColumnName = "C_Invoice_ID";
                _columnName = "DateInvoiced";
            }
            else if (transactionType == TransactionType.Order)
            {
                _tableName = "C_Order";
                _keyColumnName = "C_Order_ID";
                _columnName = "DateOrdered";
            }
            else if (transactionType == TransactionType.Requisition)
            {
                _tableName = "M_Requisition";
                _keyColumnName = "M_Requisition_ID";
                _columnName = "DateDoc";
            }
            else if (transactionType == TransactionType.TimeExpense)
            {
                _tableName = "S_TimeExpense";
                _keyColumnName = "S_TimeExpense_ID";
                _columnName = "DateReport";
            }
            else if (transactionType == TransactionType.Contract)
            {
                _tableName = "C_Contract";
                _keyColumnName = "C_Contract_ID";
                _columnName = "StartDate";
            }
            else
            {
                _tableName = "C_Order";
                _keyColumnName = "C_Order_ID";
                _columnName = "DateOrdered";
            }
        }
        
        public int GetM_PriceList_Version_ID(Ctx ctx, string priceListId, string transactionId, string productId, string uomId, string attrSetInstId)
        {
            if (Util.GetValueOfInt(transactionId) > 0)
            {
                sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv WHERE plv.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                    + @" AND plv.VALIDFROM <= (SELECT " + _columnName + " FROM " + _tableName + " WHERE " + _keyColumnName + "=" + Util.GetValueOfInt(transactionId)
                    + ") ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";

                if (Util.GetValueOfInt(productId) > 0)
                {
                    sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                        + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                        + " WHERE plv.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                        + @" AND plv.VALIDFROM <= (SELECT " + _columnName + " FROM " + _tableName + " WHERE " + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                        + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                        + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";

                    if (Util.GetValueOfInt(uomId) > 0)
                    {                      
                        sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                + " WHERE plv.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                + @" AND plv.VALIDFROM <= (SELECT " + _columnName + " FROM " + _tableName + " WHERE " + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                                + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)                                
                                + " AND NVL(pp.C_UOM_ID, 0) = " + Util.GetValueOfInt(uomId)
                                + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                    }

                    if (Util.GetValueOfInt(attrSetInstId) > 0)
                    {
                        sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                            + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                            + " WHERE plv.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                            + @" AND plv.VALIDFROM <= (SELECT " + _columnName + " FROM " + _tableName + " WHERE " + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                            + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                            + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = " + Util.GetValueOfInt(attrSetInstId)
                            + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                    }

                    if (Util.GetValueOfInt(uomId) > 0 && Util.GetValueOfInt(attrSetInstId) > 0)
                    {
                        sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                            + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                            + " WHERE plv.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                            + @" AND plv.VALIDFROM <= (SELECT " + _columnName + " FROM " + _tableName + " WHERE " + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                            + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                            + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = " + Util.GetValueOfInt(attrSetInstId)
                            + " AND NVL(pp.C_UOM_ID, 0) = " + Util.GetValueOfInt(uomId)
                            + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                    }
                }
                                
                return Util.GetValueOfInt(DB.ExecuteScalar(sql));
            }
            else
            {
                return 0;
            }
        }

        public int GetM_PriceList_Version_ID_ForOrder(Ctx ctx, string fields, string OrderId)
        {
            string[] paramValue = fields.Split(',');
            int M_PriceList_ID;
            M_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());

            if (Util.GetValueOfInt(OrderId) > 0)
            {
                sql = "SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' AND M_PriceList_ID = " + M_PriceList_ID + @" AND VALIDFROM <= (SELECT DATEORDERED FROM C_ORDER WHERE C_ORDER_ID=" + Util.GetValueOfInt(OrderId) + ") ORDER BY VALIDFROM DESC, M_PriceList_Version_ID DESC";


                return Util.GetValueOfInt(DB.ExecuteScalar(sql));
            }
            else
            {
                return 0;
            }
        }

        // Added by Bharat on 12/May/2017
        public Dictionary<string, int> GetPriceList(Ctx ctx, string fields)
        {
            int M_PriceListVersion_ID = Util.GetValueOfInt(fields);
            Dictionary<string, int> retDic = new Dictionary<string, int>();
            MPriceListVersion ver = new MPriceListVersion(ctx, M_PriceListVersion_ID, null);
            retDic["M_PriceList_ID"] = ver.GetM_PriceList_ID();
            MPriceList list = new MPriceList(ctx, ver.GetM_PriceList_ID(), null);
            retDic["C_Currency_ID"] = list.GetC_Currency_ID();
            return retDic;
        }
    }
}
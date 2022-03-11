using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    /// <summary>
    /// Screen Type enum
    /// </summary>
    public enum ScreenType
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
        private DateTime? _transactionDate = null;
        /**	Logger			*/
        protected VLogger log = null;

        public MPriceListVersionModel()
        {
            if (log == null)
            {
                log = VLogger.GetVLogger(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Get Price List Version ID
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameters as comma seperated string</param>
        /// <returns>Price List Version ID</returns>
        public int GetM_PriceList_Version_ID(Ctx ctx, string fields)
        {
            /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
            int M_PriceList_ID = 0, transactionId = 0, productId = 0, uomId = 0, attrSetInstId = 0, tranType = 0;

            if (!string.IsNullOrEmpty(fields))
            {
                string[] paramValue = fields.Split(',');
                M_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
                if (paramValue.Length >= 2)
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
                    SetScreenTableAndColumnNames((ScreenType)tranType);
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

        /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
        /// <summary>
        /// Get Price List Version on ValidFrom Date
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameters as comma seperated string</param>
        /// <param name="screenType">Screen type</param>
        /// <returns>Price List Version ID</returns>
        public int GetM_PriceList_Version_ID(Ctx ctx, string fields, ScreenType screenType)
        {
            SetScreenTableAndColumnNames(screenType);

            return GetM_PriceList_Version_ID(ctx, fields);
        }

        /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
        /// <summary>
        /// Set Screen Table And ColumnName
        /// </summary>
        /// <param name="screenType">Screen Type</param>
        private void SetScreenTableAndColumnNames(ScreenType screenType)
        {
            if (screenType == ScreenType.Invoice)
            {
                _tableName = "C_Invoice";
                _keyColumnName = "C_Invoice_ID";
                _columnName = "DateInvoiced";
            }
            else if (screenType == ScreenType.Order)
            {
                _tableName = "C_Order";
                _keyColumnName = "C_Order_ID";
                _columnName = "DateOrdered";
            }
            else if (screenType == ScreenType.Requisition)
            {
                _tableName = "M_Requisition";
                _keyColumnName = "M_Requisition_ID";
                _columnName = "DateDoc";
            }
            else if (screenType == ScreenType.TimeExpense)
            {
                _tableName = "S_TimeExpense";
                _keyColumnName = "S_TimeExpense_ID";
                _columnName = "DateReport";
            }
            else if (screenType == ScreenType.Contract)
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

        /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
        /// <summary>
        /// Get Price List Version on ValidFrom Date
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="priceListId">Price List Id</param>
        /// <param name="transactionId">Transaction Id of screen</param>
        /// <param name="productId">Product Id</param>
        /// <param name="uomId">UOM Id</param>
        /// <param name="attrSetInstId">Attribute Set Instance ID</param>
        /// <returns>Price List Version Id</returns>
        public int GetM_PriceList_Version_ID(Ctx ctx, string priceListId, string transactionId, string productId, string uomId, string attrSetInstId)
        {
            int priceListVersionId = 0;
            int UOM_EACH = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID FROM M_Product WHERE M_Product_ID = " + productId));

            try
            {
                if (Util.GetValueOfInt(transactionId) > 0)
                {
                    if (Util.GetValueOfInt(productId) > 0)
                    {
                        if (Util.GetValueOfInt(uomId) > 0 && Util.GetValueOfInt(attrSetInstId) > 0)
                        {
                            sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                    + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                    + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                    + @" AND plv.VALIDFROM <= (SELECT t." + _columnName + " FROM " + _tableName + " t WHERE t.IsActive = 'Y' AND t." + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                                    + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                                    + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = " + Util.GetValueOfInt(attrSetInstId)
                                    + " AND NVL(pp.C_UOM_ID, 0) = " + Util.GetValueOfInt(uomId)
                                    + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                        }
                        else if (Util.GetValueOfInt(uomId) > 0 && Util.GetValueOfInt(attrSetInstId) <= 0)
                        {
                            sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                    + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                    + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                    + @" AND plv.VALIDFROM <= (SELECT t." + _columnName + " FROM " + _tableName + " t WHERE t.IsActive = 'Y' AND t." + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                                    + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                                    + " AND NVL(pp.C_UOM_ID, 0) = " + Util.GetValueOfInt(uomId)
                                    + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = 0"
                                    + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                        }
                        else if (Util.GetValueOfInt(attrSetInstId) > 0 && Util.GetValueOfInt(uomId) <= 0)
                        {
                            sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                    + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                    + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                    + @" AND plv.VALIDFROM <= (SELECT t." + _columnName + " FROM " + _tableName + " t WHERE t.IsActive = 'Y' AND t." + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                                    + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                                    + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = " + Util.GetValueOfInt(attrSetInstId)
                                    //+ " AND NVL(pp.C_UOM_ID, 0) = 0"
                                    + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                        }
                        else
                        {
                            sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                    + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                    + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                    + @" AND plv.VALIDFROM <= (SELECT t." + _columnName + " FROM " + _tableName + " t WHERE t.IsActive = 'Y' AND t." + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                                    + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                                    + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = 0"
                                    //+ " AND NVL(pp.C_UOM_ID, 0) = 0"
                                    + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                        }
                    }
                    else
                    {
                        sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv WHERE plv.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                + @" AND plv.VALIDFROM <= (SELECT t." + _columnName + " FROM " + _tableName + " t WHERE t.IsActive = 'Y' AND t." + _keyColumnName + "=" + Util.GetValueOfInt(transactionId)
                                + ") ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";
                    }

                    priceListVersionId = Util.GetValueOfInt(DB.ExecuteScalar(sql));

                    if (Util.GetValueOfInt(uomId) > 0 && Util.GetValueOfInt(uomId) != Util.GetValueOfInt(UOM_EACH) && priceListVersionId == 0)
                    {
                        sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + priceListId
                                + @" AND plv.VALIDFROM <= (SELECT t." + _columnName + " FROM " + _tableName + " t WHERE t.IsActive = 'Y' AND t." + _keyColumnName + "=" + Util.GetValueOfInt(transactionId) + ") "
                                + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId)
                                + " AND NVL(pp.M_AttributeSetInstance_ID, 0)=" + attrSetInstId
                                + " AND NVL(pp.C_UOM_ID, 0) = " + Util.GetValueOfInt(UOM_EACH)
                                + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";

                        priceListVersionId = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                    }
                }
            }
            catch (Exception ex)
            {
                log.SaveError("Error in GetM_PriceList_Version_ID()", ex);
                return Util.GetValueOfInt(priceListVersionId);
            }

            return Util.GetValueOfInt(priceListVersionId);
        }

        /// <summary>
        /// Get M_PriceList_Version_ID for Contract
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="fields">parameters</param>
        /// <returns></returns>
        public int GetM_PriceList_Version_ID_On_Transaction_Date(Ctx ctx, string fields)
        {
            /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
            int M_PriceList_ID = 0, productId = 0, priceListVersionId = 0, uomId = 0, attrSetInstId = 0;

            if (!string.IsNullOrEmpty(fields))
            {
                string[] paramValue = fields.Split(',');
                M_PriceList_ID = Util.GetValueOfInt(paramValue[0].ToString());
                if (paramValue.Length > 1)
                {
                    if (paramValue[1] != null && paramValue[1] != "")
                    {
                        _transactionDate = Convert.ToDateTime(paramValue[1]);
                    }
                }
                if (paramValue.Length > 2)
                    productId = Util.GetValueOfInt(paramValue[2].ToString());
                if (paramValue.Length > 3)
                    uomId = Util.GetValueOfInt(paramValue[3].ToString());
                if (paramValue.Length > 4)
                    attrSetInstId = Util.GetValueOfInt(paramValue[4].ToString());
            }

            if (M_PriceList_ID > 0 && productId > 0 && !string.IsNullOrEmpty(Convert.ToString(_transactionDate)))
            {
                sql = "SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                            + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                            + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + M_PriceList_ID
                            + @" AND plv.VALIDFROM <= " + GlobalVariable.TO_DATE(_transactionDate, true)
                            + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(productId);

                if (uomId > 0)
                    sql = sql + " AND NVL(pp.C_UOM_ID, 0) = " + uomId;

                if (attrSetInstId > 0)
                    sql = sql + " AND NVL(pp.M_AttributeSetInstance_ID, 0) = " + attrSetInstId;

                sql = sql + " ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC";

                priceListVersionId = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            }

            return priceListVersionId;
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
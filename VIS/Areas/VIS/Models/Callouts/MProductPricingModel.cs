using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;
using VIS.Controllers;
using VAdvantage.DataBase;
using System.Data;

//using ViennaAdvantageWeb.Areas.VIS.DataContracts;

namespace VIS.Models
{
    public class MProductPricingModel
    {
        Dictionary<string, object> retDic = null;

        /// <summary>
        /// GetProductPricing
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public ProductDataOut GetProductPricing(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_Product_ID, C_BPartner_ID, M_PriceList_ID, M_PriceList_Version_ID, M_AttributeSetInstance_ID = 0, countED011 = 0, C_UOM_ID = 0;
            decimal Qty;
            bool isSOTrx;

            //Assign parameter value
            M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            C_BPartner_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Qty = Util.GetValueOfDecimal(paramValue[2].ToString());
            isSOTrx = Convert.ToBoolean(paramValue[3]);
            M_PriceList_ID = Util.GetValueOfInt(paramValue[4].ToString());
            M_PriceList_Version_ID = Util.GetValueOfInt(paramValue[5].ToString());
            DateTime? orderDate = Util.GetValueOfDateTime(paramValue[6]);
            DateTime? orderDate1 = Util.GetValueOfDateTime(paramValue[7]);

            //if (paramValue.Length > 8)    
            if (paramValue.Length == 9 || paramValue.Length == 11)
            {
                M_AttributeSetInstance_ID = Util.GetValueOfInt(paramValue[8].ToString());
            }

            if (paramValue.Length > 9)
            {
                if (paramValue.Length == 11)
                {
                    C_UOM_ID = Util.GetValueOfInt(paramValue[9].ToString());
                    countED011 = Util.GetValueOfInt(paramValue[10].ToString());
                }
                else if (paramValue.Length == 10)
                {
                    C_UOM_ID = Util.GetValueOfInt(paramValue[8].ToString());
                    countED011 = Util.GetValueOfInt(paramValue[9].ToString());
                }
            }

            /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
            if (!string.IsNullOrEmpty(Util.GetValueOfString(orderDate)))
            {
                StringBuilder sbparams = new StringBuilder();
                sbparams.Append(Util.GetValueOfInt(M_PriceList_ID));
                sbparams.Append(",").Append(Convert.ToDateTime(orderDate).ToString("MM-dd-yyyy"));
                sbparams.Append(",").Append(Util.GetValueOfInt(M_Product_ID));
                sbparams.Append(",").Append(Util.GetValueOfInt(C_UOM_ID));
                sbparams.Append(",").Append(Util.GetValueOfInt(M_AttributeSetInstance_ID));
                MPriceListVersionModel objPriceList = new MPriceListVersionModel();
                M_PriceList_Version_ID = objPriceList.GetM_PriceList_Version_ID_On_Transaction_Date(ctx, sbparams.ToString());
            }

            //End Assign parameter value

            MProductPricing pp = new MProductPricing(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(),
                        M_Product_ID, C_BPartner_ID, Qty, isSOTrx);

            //var M_PriceList_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_ID");
            pp.SetM_PriceList_ID(M_PriceList_ID);
            /** PLV is only accurate if PL selected in header */
            //var M_PriceList_Version_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);

            //if (paramValue.Length > 8)
            if (paramValue.Length == 9 || paramValue.Length == 11)
            {
                pp.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            }
            //var orderDate = System.Convert.ToDateTime(mTab.getValue("DateOrdered"));
            pp.SetPriceDate(orderDate);
            pp.SetPriceDate1(orderDate1);

            if (countED011 > 0)
            {
                pp.SetC_UOM_ID(C_UOM_ID);
            }

            //Get product stock
            MProduct product = MProduct.Get(ctx, M_Product_ID);


            ProductDataOut objInfo = new ProductDataOut
            {

                //** Price List - ValidFrom date : return 0 when plv is not available ** Dt:03/26/2021 ** Modified By: Kumar **//

                PriceList = (M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceList()),
                PriceLimit = (M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceLimit()),
                PriceActual = (M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceStd()),
                PriceEntered = (M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceStd()),
                PriceStd = (M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceStd()),
                LineAmt = pp.GetLineAmt(MCurrency.Get(ctx, pp.GetC_Currency_ID()).GetStdPrecision()),
                C_Currency_ID = System.Convert.ToInt32(pp.GetC_Currency_ID()),
                Discount = pp.GetDiscount(),
                C_UOM_ID = System.Convert.ToInt32(pp.GetC_UOM_ID()),
                //QtyOrdered= mTab.GetValue("QtyEntered"));
                EnforcePriceLimit = pp.IsEnforcePriceLimit(),
                DiscountSchema = pp.IsDiscountSchema(),
                IsStocked = product.IsStocked(),
                DocumentNote = product.GetDocumentNote()
            };
            product = null;
            pp = null;
            return objInfo;
        }


        /// <summary>
        /// Get StdPrice from Product Price
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Fields</param>
        /// <returns>StdPrice</returns>
        public Dictionary<String, Object> GetProductdata(Ctx ctx, string fields)
        {
            int UOM = 0, M_Product_ID = 0, Attribute = 0, M_PriceList_ID = 0;
            string[] paramValue = fields.Split(',');
            M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            Attribute = Util.GetValueOfInt(paramValue[1].ToString());
            M_PriceList_ID = Util.GetValueOfInt(paramValue[2].ToString());
            UOM = Util.GetValueOfInt(paramValue[3].ToString());
            retDic = new Dictionary<string, object>();

            string sql = @"SELECT M_ProductPrice.PriceStd ,M_Product.C_UOM_ID, M_Product.IsBOM, M_Product.IsVerified FROM M_ProductPrice
                            INNER JOIN M_Product ON M_Product.M_Product_ID = M_ProductPrice.M_Product_ID 
                            WHERE M_ProductPrice.M_PriceList_Version_ID = (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version
                            WHERE IsActive = 'Y'" +
                            " AND M_PriceList_ID =" + M_PriceList_ID +
                            ") AND M_ProductPrice.M_Product_ID=" + M_Product_ID +
                            " AND NVL(M_ProductPrice.M_AttributeSetInstance_ID,0)=" + Attribute;

            if (UOM > 0)
            {
                sql += " AND M_ProductPrice.C_UOM_ID=" + UOM;
            }
            else
            {
                sql += " AND M_ProductPrice.C_UOM_ID = M_Product.C_UOM_ID";
            }

            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic["PriceStd"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);
                retDic["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_ID"]);
                if (Util.GetValueOfString(ds.Tables[0].Rows[0]["IsVerified"]).Equals("Y"))
                {
                    retDic["IsBOM"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsBOM"]);
                }
            }
            else
            {
                ds = DB.ExecuteDataset(@"SELECT C_UOM_ID, IsBOM, IsVerified FROM M_Product WHERE M_Product_ID = " + M_Product_ID, null, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retDic["C_UOM_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_UOM_ID"]);
                    if (Util.GetValueOfString(ds.Tables[0].Rows[0]["IsVerified"]).Equals("Y"))
                    {
                        retDic["IsBOM"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsBOM"]);
                    }
                }
            }
            return retDic;
        }

        /// <summary>
        /// when we change QtyEntered, UOM or Attribute
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of Parameters</param>
        /// <returns>Price Data</returns>
        public Dictionary<String, Object> GetPricesOnChange(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');
            Dictionary<String, Object> retDic = new Dictionary<string, Object>();

            //Assign parameter value
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _m_requisitionID = Util.GetValueOfInt(paramValue[1].ToString());
            int _m_AttributeSetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _c_Uom_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _ad_Client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _c_BPartner_Id = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[6].ToString());
            int _m_PriceList_ID = Util.GetValueOfInt(paramValue[7].ToString());
            string _transactionDate = string.Empty;

            //End Assign parameter value
            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            int _priceListVersion_Id = 0;
            int uomPrecision = 2;

            StringBuilder sbparams = new StringBuilder();
            sbparams.Append(Util.GetValueOfInt(_m_PriceList_ID));
            if (Util.GetValueOfDateTime(_transactionDate) != null)
                sbparams.Append(",").Append(Util.GetValueOfString(_transactionDate));
            else
                sbparams.Append(",").Append(Util.GetValueOfInt(_m_requisitionID));
            sbparams.Append(",").Append(Util.GetValueOfInt(_m_Product_Id));
            sbparams.Append(",").Append(Util.GetValueOfInt(_c_Uom_Id));
            sbparams.Append(",").Append(Util.GetValueOfInt(_m_AttributeSetInstance_Id));

            MPriceListVersionModel objPLV = new MPriceListVersionModel();

            if (Util.GetValueOfDateTime(_transactionDate) != null)
                _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID_On_Transaction_Date(ctx, sbparams.ToString());
            else
                _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, sbparams.ToString(), ScreenType.Requisition);
            uomPrecision = MUOM.GetPrecision(ctx, _c_Uom_Id);
            sql.Clear();
            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE IsActive='Y' AND M_Product_ID = " + _m_Product_Id
                            + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                            + " AND NVL(M_AttributeSetInstance_ID, 0)=" + _m_AttributeSetInstance_Id
                            + "  AND C_UOM_ID=" + _c_Uom_Id);

            DataSet ds1 = DB.ExecuteDataset(sql.ToString());
            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                PriceEntered = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]);
                PriceList = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceList"]);
                PriceLimit = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceLimit"]);
            }
            else
            {
                // get uom from product
                var paramStr = _m_Product_Id.ToString();
                MProductModel objProduct = new MProductModel();
                var prodC_UOM_ID = objProduct.GetC_UOM_ID(ctx, paramStr);
                sql.Clear();
                sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                            + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                            + " AND NVL(M_AttributeSetInstance_ID, 0)=" + _m_AttributeSetInstance_Id
                            + " AND C_UOM_ID=" + prodC_UOM_ID);

                DataSet ds = DB.ExecuteDataset(sql.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    PriceEntered = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]);
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                    PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);


                    sql.Clear();
                    sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                                               " AND con.M_Product_ID = " + _m_Product_Id +
                                               " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + _c_Uom_Id);
                    var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (rate == 0)
                    {
                        sql.Clear();
                        sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                              " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + _c_Uom_Id);
                        rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    }

                    if (rate == 0)
                    {
                        rate = 1;
                    }

                    PriceEntered = PriceEntered * rate;
                    PriceList = PriceList * rate;
                    PriceLimit = PriceLimit * rate;
                }
            }

            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceList"] = PriceList;
            retDic["PriceLimit"] = PriceLimit;
            retDic["UOMPrecision"] = uomPrecision;
            retDic["QtyOrdered"] = MUOMConversion.ConvertProductFrom(ctx, _m_Product_Id, _c_Uom_Id,
                decimal.Round(_qtyEntered, uomPrecision, MidpointRounding.AwayFromZero));

            return retDic;
        }
    }
}
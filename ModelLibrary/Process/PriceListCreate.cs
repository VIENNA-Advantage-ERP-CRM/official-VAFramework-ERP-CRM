
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using System.Data;
//using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.WF;

using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class PriceListCreate : ProcessEngine.SvrProcess
    {
        //Delete Old Prices			
        private bool _deleteOld = false;
        // Price List Version			
        private int _VAM_PriceListVersion_ID = 0;
        // Price List Version			
        private MPriceListVersion _plv = null;

        /// <summary>
        /// Prepare - e.g., get Parameters
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
                else if (name.Equals("DeleteOld"))
                {
                    _deleteOld = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAM_PriceListVersion_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("VAM_PriceListVersion_ID=" + _VAM_PriceListVersion_ID
                    + ", DeleteOld=" + _deleteOld);
            _plv = new MPriceListVersion(GetCtx(), _VAM_PriceListVersion_ID, Get_TrxName());
            if (_plv.Get_ID() == 0 || _plv.Get_ID() != _VAM_PriceListVersion_ID)
            {
                throw new Exception("@NotFound@  @VAM_PriceListVersion_ID@=" + _VAM_PriceListVersion_ID);
            }
            //	
            String error = CheckPrerequisites();
            if (error != null && error.Length > 0)
            {
                throw new Exception(error);
            }
            return Create();
        }

        /// <summary>
        /// 	Prepare Calculations
        /// </summary>
        /// <returns>error message</returns>
        private String CheckPrerequisites()
        {
            String clientWhere = " AND VAF_Client_ID=" + _plv.GetVAF_Client_ID();

            //	PO Prices must exists
            int no = DataBase.DB.ExecuteQuery("UPDATE VAM_Product_PO SET PriceList = 0 WHERE PriceList IS NULL" + clientWhere, null, Get_TrxName());
            no = DataBase.DB.ExecuteQuery("UPDATE VAM_Product_PO SET PriceLastPO = 0 WHERE PriceLastPO IS NULL" + clientWhere, null, Get_TrxName());
            no = DataBase.DB.ExecuteQuery("UPDATE VAM_Product_PO SET PricePO = PriceLastPO "
                + "WHERE (PricePO IS NULL OR PricePO = 0) AND PriceLastPO <> 0" + clientWhere, null,
                Get_TrxName());
            no = DataBase.DB.ExecuteQuery(
                "UPDATE	VAM_Product_PO SET PricePO = 0 WHERE PricePO IS NULL" + clientWhere, null,
                Get_TrxName());
            //	Set default current vendor
            no = DataBase.DB.ExecuteQuery(
                "UPDATE VAM_Product_PO p SET IsCurrentVendor = 'Y' "
                + "WHERE IsCurrentVendor = 'N'"
                + " AND NOT EXISTS "
                    + "(SELECT pp.VAM_Product_ID FROM VAM_Product_PO pp "
                    + "WHERE pp.VAM_Product_ID=p.VAM_Product_ID "
                    + "GROUP BY pp.VAM_Product_ID HAVING COUNT(*) > 1)" + clientWhere, null,
                Get_TrxName());

            /**
             *	Make sure that we have only one active product vendor
             */
            String sql = "SELECT * FROM VAM_Product_PO po "
                + "WHERE IsCurrentVendor='Y' AND IsActive='Y'"
                + clientWhere
                + " AND EXISTS (SELECT VAM_Product_ID FROM VAM_Product_PO x "
                    + "WHERE x.VAM_Product_ID=po.VAM_Product_ID"
                    + " AND IsCurrentVendor='Y' AND IsActive='Y' "
                    + "GROUP BY VAM_Product_ID HAVING COUNT(*) > 1) "
                + "ORDER BY VAM_Product_ID, Created";

            int success = 0;
            int errors = 0;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                int VAM_Product_ID = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    MProductPO po = new MProductPO(GetCtx(), dr, Get_TrxName());
                    if (VAM_Product_ID != po.GetVAM_Product_ID())
                    {
                        VAM_Product_ID = po.GetVAM_Product_ID();
                        continue;
                    }
                    po.SetIsCurrentVendor(false);
                    if (po.Save())
                    {
                        success++;
                    }
                    else
                    {
                        errors++;
                        log.Warning("Not updated " + po);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            log.Info("Current Vendor - Changes=" + success + ", Errors=" + errors);
            return null;
        }

        /// <summary>
        /// 	Create Price List
        /// </summary>
        /// <returns>info message</returns>
        private String Create()
        {
            StringBuilder info = new StringBuilder();
            //	Delete Old Data	
            if (_deleteOld)
            {
                int no = DataBase.DB.ExecuteQuery(
                    "DELETE VAM_ProductPrice "
                    + "WHERE VAM_PriceListVersion_ID=" + _VAM_PriceListVersion_ID, null,
                    Get_TrxName());
                log.Info("Deleted=" + no);
                info.Append("@Deleted@=").Append(no).Append(" - ");
            }

            int VAM_PriceListVersion_Base_ID = _plv.GetVAM_PriceListVersion_Base_ID();
            MPriceList pl = _plv.GetPriceList();
            int curPrecision = pl.GetStandardPrecision();

            /**
             *	For All Discount Lines in Sequence
             */
            MVAMDiscountCalculation ds = new MVAMDiscountCalculation(GetCtx(), _plv.GetVAM_DiscountCalculation_ID(), Get_TrxName());
            MVAMPriceDiscount[] dsl = ds.GetLines(false);
            for (int i = 0; i < dsl.Length; i++)
            {
                MVAMPriceDiscount dsLine = dsl[i];
                String message = "#" + dsLine.GetSeqNo();
                String dd = dsLine.GetDescription();
                if (dd != null && dd.Length > 0)
                {
                    message += " " + dd;
                }
                //	Clear Temporary Table
                int noDeleted = DataBase.DB.ExecuteQuery("DELETE FROM VAT_Selection", null, Get_TrxName());
                //	Create Selection in Temporary Table
                String sql = null;
                int VAM_PriceDiscount_ID = dsLine.GetVAM_PriceDiscount_ID();
                int p2 = VAM_PriceListVersion_Base_ID;
                if (p2 == 0)	//	Create from PO
                {
                    p2 = dsLine.GetVAF_Client_ID();
                    sql = "INSERT INTO VAT_Selection (VAT_Selection_ID) "
                        + "SELECT DISTINCT po.VAM_Product_ID "
                        + "FROM VAM_Product_PO po "
                        + " INNER JOIN VAM_Product p ON (p.VAM_Product_ID=po.VAM_Product_ID)"
                        + " INNER JOIN VAM_PriceDiscount dl ON (dl.VAM_PriceDiscount_ID=" + VAM_PriceDiscount_ID + ") "	//	#1
                        + "WHERE p.VAF_Client_ID IN (" + p2 + ", 0)"		//	#2
                        + " AND p.IsActive='Y' AND po.IsActive='Y' AND po.IsCurrentVendor='Y'"
                        //	Optional Restrictions
                        + " AND (dl.VAM_ProductCategory_ID IS NULL OR p.VAM_ProductCategory_ID=dl.VAM_ProductCategory_ID)"
                        + " AND (dl.VAB_BusinessPartner_ID IS NULL OR po.VAB_BusinessPartner_ID=dl.VAB_BusinessPartner_ID)"
                        + " AND (dl.VAM_Product_ID IS NULL OR p.VAM_Product_ID=dl.VAM_Product_ID)";
                }
                else			//	Create from Price List **
                {
                    sql = "INSERT INTO VAT_Selection (VAT_Selection_ID) "
                        + "SELECT DISTINCT p.VAM_Product_ID "
                        + "FROM VAM_ProductPrice pp"
                        + " INNER JOIN VAM_Product p ON (p.VAM_Product_ID=pp.VAM_Product_ID)"
                        + " INNER JOIN VAM_PriceDiscount dl ON (dl.VAM_PriceDiscount_ID=" + VAM_PriceDiscount_ID + ") "	//	#1
                        + "WHERE pp.VAM_PriceListVersion_ID=" + p2	//#2 PriceList_Version_Base_ID
                        + " AND p.IsActive='Y' AND pp.IsActive='Y'"
                        //	Optional Restrictions
                        + " AND (dl.VAM_ProductCategory_ID IS NULL OR p.VAM_ProductCategory_ID=dl.VAM_ProductCategory_ID)"
                        + " AND (dl.VAB_BusinessPartner_ID IS NULL OR EXISTS "
                            + "(SELECT * FROM VAM_Product_PO po "
                            + "WHERE po.VAM_Product_ID=p.VAM_Product_ID AND po.VAB_BusinessPartner_ID=dl.VAB_BusinessPartner_ID))"
                        + " AND (dl.VAM_Product_ID IS NULL OR p.VAM_Product_ID=dl.VAM_Product_ID)";
                }
                //idr = DataBase.prepareStatement(sql, get_TrxName());
                int noSelected = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                message += ": @Selected@=" + noSelected;
                //	Delete Prices in Selection, so that we can insert
                if (VAM_PriceListVersion_Base_ID == 0
                    || VAM_PriceListVersion_Base_ID != _VAM_PriceListVersion_ID)
                {
                    sql = "DELETE FROM VAM_ProductPrice pp "
                        + "WHERE pp.VAM_PriceListVersion_ID=" + _VAM_PriceListVersion_ID
                        + " AND EXISTS (SELECT * FROM VAT_Selection s WHERE pp.VAM_Product_ID=s.VAT_Selection_ID)";
                    noDeleted = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                    message += ", @Deleted@=" + noDeleted;
                }
                //	Copy (Insert) Prices
                int noInserted = 0;
                sql = "INSERT INTO VAM_ProductPrice "
                    + "(VAM_PriceListVersion_ID, VAM_Product_ID,"
                    + " VAF_Client_ID, VAF_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " PriceList, PriceStd, PriceLimit) ";
                //
                if (VAM_PriceListVersion_Base_ID == _VAM_PriceListVersion_ID)
                {
                    sql = null;	//	We have Prices already
                }
                else if (VAM_PriceListVersion_Base_ID == 0)
                {
                    /**	Copy and Convert from Product_PO	*/
                    sql += "SELECT plv.VAM_PriceListVersion_ID, po.VAM_Product_ID,"
                        + " plv.VAF_Client_ID, plv.VAF_Org_ID, 'Y', SysDate, plv.UpdatedBy, SysDate, plv.UpdatedBy,"
                        //	Price List
                        + " COALESCE(currencyConvert(po.PriceList,"
                        + " po.VAB_Currency_ID, pl.VAB_Currency_ID, dl.ConversionDate, dl.VAB_CurrencyType_ID, plv.VAF_Client_ID, plv.VAF_Org_ID), -po.PriceList),"
                        //	Price Std
                        + " COALESCE(currencyConvert(po.PriceList,"
                        + "	po.VAB_Currency_ID, pl.VAB_Currency_ID, dl.ConversionDate, dl.VAB_CurrencyType_ID, plv.VAF_Client_ID, plv.VAF_Org_ID), -po.PriceList),"
                        //	Price Limit
                        + " COALESCE(currencyConvert(po.PricePO,"
                        + " po.VAB_Currency_ID, pl.VAB_Currency_ID, dl.ConversionDate, dl.VAB_CurrencyType_ID, plv.VAF_Client_ID, plv.VAF_Org_ID), -po.PricePO) "
                        //
                        + "FROM VAM_Product_PO po"
                        + " INNER JOIN VAM_PriceListVersion plv ON (plv.VAM_PriceListVersion_ID=" + _VAM_PriceListVersion_ID + ")"	//	#1
                        + " INNER JOIN VAM_PriceList pl ON (pl.VAM_PriceList_ID=plv.VAM_PriceList_ID)"
                        + " INNER JOIN VAM_PriceDiscount dl ON (dl.VAM_PriceDiscount_ID=" + VAM_PriceDiscount_ID + ") "	//	#2
                        //
                        + "WHERE EXISTS (SELECT * FROM VAT_Selection s WHERE po.VAM_Product_ID=s.VAT_Selection_ID)"
                        + " AND po.IsCurrentVendor='Y' AND po.IsActive='Y'";
                }
                else
                {
                    /**	Copy and Convert from other PriceList_Version	*/
                    sql += "SELECT plv.VAM_PriceListVersion_ID, pp.VAM_Product_ID,"
                        + " plv.VAF_Client_ID, plv.VAF_Org_ID, 'Y', SysDate, plv.UpdatedBy, SysDate, plv.UpdatedBy,"
                        //	Price List
                        + " COALESCE(currencyConvert(pp.PriceList,"
                        + " bpl.VAB_Currency_ID, pl.VAB_Currency_ID, dl.ConversionDate, dl.VAB_CurrencyType_ID, plv.VAF_Client_ID, plv.VAF_Org_ID), -pp.PriceList),"
                        //	Price Std
                        + " COALESCE(currencyConvert(pp.PriceStd,"
                        + " bpl.VAB_Currency_ID, pl.VAB_Currency_ID, dl.ConversionDate, dl.VAB_CurrencyType_ID, plv.VAF_Client_ID, plv.VAF_Org_ID), -pp.PriceStd),"
                        //	Price Limit
                        + " COALESCE(currencyConvert(pp.PriceLimit,"
                        + " bpl.VAB_Currency_ID, pl.VAB_Currency_ID, dl.ConversionDate, dl.VAB_CurrencyType_ID, plv.VAF_Client_ID, plv.VAF_Org_ID), -pp.PriceLimit) "
                        //
                        + "FROM VAM_ProductPrice pp"
                        + " INNER JOIN VAM_PriceListVersion plv ON (plv.VAM_PriceListVersion_ID=" + _VAM_PriceListVersion_ID + ")"	//	#1
                        + " INNER JOIN VAM_PriceList pl ON (pl.VAM_PriceList_ID=plv.VAM_PriceList_ID)"
                        + " INNER JOIN VAM_PriceListVersion bplv ON (pp.VAM_PriceListVersion_ID=bplv.VAM_PriceListVersion_ID)"
                        + " INNER JOIN VAM_PriceList bpl ON (bplv.VAM_PriceList_ID=bpl.VAM_PriceList_ID)"
                        + " INNER JOIN VAM_PriceDiscount dl ON (dl.VAM_PriceDiscount_ID=" + VAM_PriceDiscount_ID + ") "	//	#2
                        //
                        + "WHERE ";
                    if (VAM_PriceListVersion_Base_ID != 0)
                    {
                        sql += "pp.VAM_PriceListVersion_ID=" + VAM_PriceListVersion_Base_ID + " AND";	//	#3 VAM_PriceListVersion_Base_ID
                    }
                    sql += "  EXISTS (SELECT * FROM VAT_Selection s WHERE pp.VAM_Product_ID=s.VAT_Selection_ID)"
                        + " AND pp.IsActive='Y'";
                }
                if (sql != null)
                {
                    //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                    //pstmt.setInt(1, _VAM_PriceListVersion_ID);
                    // pstmt.setInt(2, VAM_PriceDiscount_ID);
                    //if (VAM_PriceListVersion_Base_ID != 0)
                    //{
                    //    pstmt.setInt(3, VAM_PriceListVersion_Base_ID);
                    // }
                    noInserted = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                    message += " @Inserted@=" + noInserted;
                }

                /** Calculations	**/
                MProductPrice[] pp = _plv.GetProductPrice(
                    "AND EXISTS (SELECT * FROM VAT_Selection s "
                    + "WHERE s.VAT_Selection_ID=VAM_ProductPrice.VAM_Product_ID)");
                for (int j = 0; j < pp.Length; j++)
                {
                    MProductPrice price = pp[j];
                    Decimal priceList = price.GetPriceList();
                    Decimal priceStd = price.GetPriceStd();
                    Decimal priceLimit = price.GetPriceLimit();
                    //
                    price.SetPriceList(Calculate(dsLine.GetList_Base(),
                        priceList, priceStd, priceLimit, dsLine.GetList_Fixed(),
                        dsLine.GetList_AddAmt(), dsLine.GetList_Discount(),
                        dsLine.GetList_Rounding(), curPrecision));

                    price.SetPriceStd(Calculate(dsLine.GetStd_Base(),
                        priceList, priceStd, priceLimit, dsLine.GetStd_Fixed(),
                        dsLine.GetStd_AddAmt(), dsLine.GetStd_Discount(),
                        dsLine.GetStd_Rounding(), curPrecision));

                    price.SetPriceLimit(Calculate(dsLine.GetLimit_Base(),
                        priceList, priceStd, priceLimit, dsLine.GetLimit_Fixed(),
                        dsLine.GetLimit_AddAmt(), dsLine.GetLimit_Discount(),
                        dsLine.GetLimit_Rounding(), curPrecision));
                    price.Save();
                }	//	for all products

                //	Clear Temporary Table
                noDeleted = DataBase.DB.ExecuteQuery("DELETE FROM VAT_Selection", null, Get_TrxName());
                //
                AddLog(message);
            }	//	for all lines

            MProductPrice[] ppl = _plv.GetProductPrice(true);
            info.Append(" - @Records@=").Append(ppl.Length);
            return info.ToString();
        }

        /// <summary>
        /// Calculate Price
        /// </summary>
        /// <param name="base1">rule</param>
        /// <param name="list">price</param>
        /// <param name="std">price</param>
        /// <param name="limit">price</param>
        /// <param name="fix">amount</param>
        /// <param name="add">amount</param>
        /// <param name="discount">percent</param>
        /// <param name="round">rule</param>
        /// <param name="curPrecision"></param>
        /// <returns>calculated price</returns>
        private Decimal Calculate(String base1,
            Decimal list, Decimal std, Decimal limit, Decimal fix,
            Decimal add, Decimal discount, String round, int curPrecision)
        {
            Decimal? calc = null;
            double dd = 0.0;
            if (MVAMPriceDiscount.LIST_BASE_ListPrice.Equals(base1))
            {
                dd = Convert.ToDouble(list);//.doubleValue();
            }
            else if (MVAMPriceDiscount.LIST_BASE_StandardPrice.Equals(base1))
            {
                dd = Convert.ToDouble(std);//.doubleValue();
            }
            else if (MVAMPriceDiscount.LIST_BASE_LimitPOPrice.Equals(base1))
            {
                dd = Convert.ToDouble(limit);//.doubleValue();
            }
            else if (MVAMPriceDiscount.LIST_BASE_FixedPrice.Equals(base1))
            {
                calc = fix;
            }
            else
            {
                throw new Exception("Unknown Base=" + base1);
            }
            if (calc == null)
            {
                if (Env.Signum(add) != 0)
                {
                    dd += Convert.ToDouble(add);//.doubleValue();
                }
                if (Env.Signum(discount) != 0)
                {
                    dd *= 1 - (Convert.ToDouble(discount) / 100.0);
                }
                calc = new Decimal(dd);
            }
            //	Rounding
            if (MVAMPriceDiscount.LIST_ROUNDING_CurrencyPrecision.Equals(round))
            {
                calc = Decimal.Round(calc.Value, curPrecision, MidpointRounding.AwayFromZero);//calc.setScale(curPrecision, Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_Dime102030.Equals(round))
            {
                calc = Decimal.Round(calc.Value, 1, MidpointRounding.AwayFromZero);//calc.setScale(1, Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_Hundred.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -2, MidpointRounding.AwayFromZero);//calc.setScale(-2, Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_Nickel051015.Equals(round))
            {
                Decimal mm = new Decimal(20);
                calc = Decimal.Multiply(calc.Value, mm);
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
                calc = Decimal.Round(Decimal.Divide(calc.Value, mm), 2, MidpointRounding.AwayFromZero);// Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_NoRounding.Equals(round))
            {
                ;
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_Quarter255075.Equals(round))
            {
                Decimal mm = new Decimal(4);
                calc = Decimal.Multiply(calc.Value, mm);
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);// calc.setScale(0, Decimal.ROUND_HALF_UP);
                calc = Decimal.Round(Decimal.Divide(calc.Value, mm), 2, MidpointRounding.AwayFromZero);// calc.divide(mm, 2, Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_Ten10002000.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -1, MidpointRounding.AwayFromZero);//calc.setScale(-1, Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_Thousand.Equals(round))
            {
                calc = Decimal.Round(calc.Value, -3, MidpointRounding.AwayFromZero);//calc.setScale(-3, Decimal.ROUND_HALF_UP);
            }
            else if (MVAMPriceDiscount.LIST_ROUNDING_WholeNumber00.Equals(round))
            {
                calc = Decimal.Round(calc.Value, 0, MidpointRounding.AwayFromZero);//calc.setScale(0, Decimal.ROUND_HALF_UP);
            }

            return calc.Value;
        }
    }
}

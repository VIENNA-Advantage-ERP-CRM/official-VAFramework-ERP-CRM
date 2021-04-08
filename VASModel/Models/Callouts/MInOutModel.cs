using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;


namespace VIS.Models
{
    public class MVAMInvInOutModel
    {
        /// <summary>
        /// GetInOut
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetInOut(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');
            int Orig_InOut_ID;

            //Assign parameter value
            Orig_InOut_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MVAMInvInOut io = new MVAMInvInOut(ctx, Orig_InOut_ID, null);
            //End Assign parameter

            Dictionary<String, String> retDic = new Dictionary<string, string>();
            //retDic["MovementDate"] = io.GetMovementDate().ToString();
            retDic["VAB_Project_ID"] = io.GetVAB_Project_ID().ToString();
            retDic["VAB_Promotion_ID"] = io.GetVAB_Promotion_ID().ToString();
            retDic["VAB_BillingCode_ID"] = io.GetVAB_BillingCode_ID().ToString();
            retDic["VAF_OrgTrx_ID"] = io.GetVAF_OrgTrx_ID().ToString();
            retDic["User1_ID"] = io.GetUser1_ID().ToString();
            retDic["User2_ID"] = io.GetUser2_ID().ToString();
            retDic["IsDropShip"] = io.IsDropShip() ? "Y" : "N";
            retDic["VAM_Warehouse_ID"] = io.GetVAM_Warehouse_ID().ToString();
            return retDic;
        }

        // Added by Bharat on 19 May 2017
        public Dictionary<String, Object> GetWarehouse(Ctx ctx, string param)
        {
            int VAM_Warehouse_ID = Util.GetValueOfInt(param);
            Dictionary<string, object> retDic = null;
            string sql = "SELECT w.VAF_Org_ID, l.VAM_Locator_ID"
            + " FROM VAM_Warehouse w"
            + " LEFT OUTER JOIN VAM_Locator l ON (l.VAM_Warehouse_ID=w.VAM_Warehouse_ID AND l.IsDefault='Y') "
            + "WHERE w.VAM_Warehouse_ID=" + VAM_Warehouse_ID;		//	1
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["VAF_Org_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_Org_ID"]);
                retDic["VAM_Locator_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAM_Locator_ID"]);
            }
            return retDic;
        }

        // Change by mohit to remove client side queries- 19 May 2017
        public Dictionary<string, object> GetDocumentTypeData(string fields)
        {
            Dictionary<string, object> retValue = null;
            DataSet _ds = null;
            string sql = "SELECT d.docBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
           + " FROM VAB_DocTypes d, VAF_Record_Seq s "
           + " WHERE VAB_DocTypes_ID=" + Util.GetValueOfInt(fields)
           + " AND d.DocNoSequence_ID=s.VAF_Record_Seq_ID(+)";
            try
            {
                _ds = DB.ExecuteDataset(sql, null, null);
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = new Dictionary<string, object>();
                    retValue["docBaseType"] = _ds.Tables[0].Rows[0]["docBaseType"].ToString();
                    retValue["IsDocNoControlled"] = _ds.Tables[0].Rows[0]["IsDocNoControlled"].ToString();
                    retValue["CurrentNext"] = _ds.Tables[0].Rows[0]["CurrentNext"].ToString();
                    retValue["IsReturnTrx"] = _ds.Tables[0].Rows[0]["IsReturnTrx"].ToString();
                }
            }
            catch (Exception e)
            {
                if (_ds != null)
                {
                    _ds.Dispose();
                    _ds = null;
                }
            }
            return retValue;
        }

        // Get Locator from warehouse

        public int GetWarehouseLocator(string fields)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT MIN(VAM_Locator_ID) FROM VAM_Locator WHERE IsActive = 'Y' AND VAM_Warehouse_ID = " + Util.GetValueOfInt(fields), null, null));
        }
        //Get UOM Conversion
        public Dictionary<string, object> GetUOMConversion(Ctx ctx, string fields)
        {
            Dictionary<string, object> retValue = null;
            string[] paramString = fields.Split(',');
            MVAMInvInOut inout = new MVAMInvInOut(ctx, Util.GetValueOfInt(paramString[0]), null);
            int VAM_Product_ID = Util.GetValueOfInt(paramString[1]);
            int VAB_UOM_ID = Util.GetValueOfInt(paramString[2]);
            try
            {
                int uom = Util.GetValueOfInt(DB.ExecuteScalar("SELECT vdr.VAB_UOM_ID FROM VAM_Product p LEFT JOIN VAM_Product_PO vdr ON p.VAM_Product_ID= vdr.VAM_Product_ID WHERE p.VAM_Product_ID=" + VAM_Product_ID + " AND vdr.VAB_BusinessPartner_ID = " + inout.GetVAB_BusinessPartner_ID(), null, null));

                if (VAB_UOM_ID != 0)
                {

                    if (VAB_UOM_ID != uom && uom != 0)
                    {
                        retValue = new Dictionary<string, object>();
                        retValue["multiplyrate"] = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM VAB_UOM_Conversion WHERE VAB_UOM_ID = " + VAB_UOM_ID + " AND VAB_UOM_To_ID = " + uom + " AND VAM_Product_ID= " + VAM_Product_ID + " AND IsActive='Y'"));
                        if (Util.GetValueOfDecimal(retValue["multiplyrate"]) <= 0)
                        {
                            retValue["multiplyrate"] = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM VAB_UOM_Conversion WHERE VAB_UOM_ID = " + VAB_UOM_ID + " AND VAB_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                        }
                        retValue["uom"] = uom;
                    }

                }
            }
            catch (Exception e)
            {

            }
            return retValue;

        }
    }
}
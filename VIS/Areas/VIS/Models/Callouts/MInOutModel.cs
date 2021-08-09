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
    public class MInOutModel
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
            MInOut io = new MInOut(ctx, Orig_InOut_ID, null);
            //End Assign parameter

            Dictionary<String, String> retDic = new Dictionary<string, string>();
            //retDic["MovementDate"] = io.GetMovementDate().ToString();
            retDic["C_Project_ID"] = io.GetC_Project_ID().ToString();
            retDic["C_Campaign_ID"] = io.GetC_Campaign_ID().ToString();
            retDic["C_Activity_ID"] = io.GetC_Activity_ID().ToString();
            retDic["AD_OrgTrx_ID"] = io.GetAD_OrgTrx_ID().ToString();
            retDic["User1_ID"] = io.GetUser1_ID().ToString();
            retDic["User2_ID"] = io.GetUser2_ID().ToString();
            retDic["IsDropShip"] = io.IsDropShip() ? "Y" : "N";
            retDic["M_Warehouse_ID"] = io.GetM_Warehouse_ID().ToString();
            return retDic;
        }

        // Added by Bharat on 19 May 2017
        public Dictionary<String, Object> GetWarehouse(Ctx ctx, string param)
        {
            int M_Warehouse_ID = Util.GetValueOfInt(param);
            Dictionary<string, object> retDic = null;
            string sql = "SELECT w.AD_Org_ID, l.M_Locator_ID"
            + " FROM M_Warehouse w"
            + " LEFT OUTER JOIN M_Locator l ON (l.M_Warehouse_ID=w.M_Warehouse_ID AND l.IsDefault='Y') "
            + "WHERE w.M_Warehouse_ID=" + M_Warehouse_ID;		//	1
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["AD_Org_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Org_ID"]);
                retDic["M_Locator_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Locator_ID"]);
            }
            return retDic;
        }

        // Change by mohit to remove client side queries- 19 May 2017
        public Dictionary<string, object> GetDocumentTypeData(string fields)
        {
            Dictionary<string, object> retValue = null;
            DataSet _ds = null;
            string sql = "SELECT d.docBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
           + " FROM C_DocType d, AD_Sequence s "
           + " WHERE C_DocType_ID=" + Util.GetValueOfInt(fields)
           + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
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
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT MIN(M_Locator_ID) FROM M_Locator WHERE IsActive = 'Y' AND M_Warehouse_ID = " + Util.GetValueOfInt(fields), null, null));
        }
        //Get UOM Conversion
        public Dictionary<string, object> GetUOMConversion(Ctx ctx, string fields)
        {
            Dictionary<string, object> retValue = null;
            string[] paramString = fields.Split(',');
            MInOut inout = new MInOut(ctx, Util.GetValueOfInt(paramString[0]), null);
            int M_Product_ID = Util.GetValueOfInt(paramString[1]);
            int C_UOM_ID = Util.GetValueOfInt(paramString[2]);
            try
            {
                int uom = Util.GetValueOfInt(DB.ExecuteScalar("SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + M_Product_ID + " AND vdr.C_BPartner_ID = " + inout.GetC_BPartner_ID(), null, null));

                if (C_UOM_ID != 0)
                {

                    if (C_UOM_ID != uom && uom != 0)
                    {
                        retValue = new Dictionary<string, object>();
                        retValue["multiplyrate"] = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + C_UOM_ID + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + M_Product_ID + " AND IsActive='Y'"));
                        if (Util.GetValueOfDecimal(retValue["multiplyrate"]) <= 0)
                        {
                            retValue["multiplyrate"] = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + C_UOM_ID + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
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
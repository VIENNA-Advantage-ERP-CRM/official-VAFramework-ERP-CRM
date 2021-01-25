using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MInvoiceBatchLineModel
    {
        /// <summary>
        /// GetInvoiceBatchLine
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetInvoiceBatchLine(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_InvoiceBatchLine_ID;
            //decimal Qty;
            //bool isSOTrx;
            Dictionary<String, String> retDic = new Dictionary<string, string>();
            //Assign parameter value
            C_InvoiceBatchLine_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value

            MInvoiceBatchLine last = new MInvoiceBatchLine(ctx, C_InvoiceBatchLine_ID, null);
            //	Need to Increase when different DocType or BP
            retDic["VAB_DocTypes_ID"] = last.GetVAB_DocTypes_ID().ToString();
            retDic["VAB_BusinessPartner_ID"] = last.GetVAB_BusinessPartner_ID().ToString();
            retDic["DocumentNo"] = last.GetDocumentNo();
            //	New Number            
            return retDic;

        }
    }
}
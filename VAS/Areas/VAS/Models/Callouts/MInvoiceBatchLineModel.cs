using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MVABInvoiceBatchLineModel
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
            int VAB_BatchInvoiceLine_ID;
            //decimal Qty;
            //bool isSOTrx;
            Dictionary<String, String> retDic = new Dictionary<string, string>();
            //Assign parameter value
            VAB_BatchInvoiceLine_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value

            MVABInvoiceBatchLine last = new MVABInvoiceBatchLine(ctx, VAB_BatchInvoiceLine_ID, null);
            //	Need to Increase when different DocType or BP
            retDic["VAB_DocTypes_ID"] = last.GetVAB_DocTypes_ID().ToString();
            retDic["VAB_BusinessPartner_ID"] = last.GetVAB_BusinessPartner_ID().ToString();
            retDic["DocumentNo"] = last.GetDocumentNo();
            //	New Number            
            return retDic;

        }
    }
}
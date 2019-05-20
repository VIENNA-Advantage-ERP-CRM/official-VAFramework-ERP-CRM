/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCurrency
 * Purpose        : Currency setting
 * Class Used     : X_C_Currency
 * Chronological    Development
 * Raghunandan      28-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MCurrency : X_C_Currency
    {
        //Store System Currencies			
        private static CCache<int, MCurrency> currencies = new CCache<int, MCurrency>("C_Currency", 50);

        /// <summary>
        /// Currency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Currency_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCurrency(Ctx ctx, int C_Currency_ID, Trx trxName)
            : base(ctx, C_Currency_ID, trxName)
        {
            if (C_Currency_ID == 0)
            {
                SetIsEMUMember(false);
                SetIsEuro(false);
                SetStdPrecision(2);
                SetCostingPrecision(4);
            }
        }

        /// <summary>
        /// Currency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ISO_Code">ISO</param>
        /// <param name="Description">Name</param>
        /// <param name="CurSymbol">symbol</param>
        /// <param name="StdPrecision">prec</param>
        /// <param name="CostingPrecision">prec</param>
        /// <param name="trxName">transaction</param>
        public MCurrency(Ctx ctx, string ISO_Code,
            String description, String curSymbol, int stdPrecision, int costingPrecision, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetISO_Code(ISO_Code);
            SetDescription(description);
            SetCurSymbol(curSymbol);
            SetStdPrecision(stdPrecision);
            SetCostingPrecision(costingPrecision);
            SetIsEMUMember(false);
            SetIsEuro(false);
        }

        /// <summary>
        /// Currency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">ResultSet</param>
        /// <param name="trxName">transaction</param>
        public MCurrency(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }


        /// <summary>
        /// Get Currency
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <returns>ISO Code</returns>
        public static MCurrency Get(Ctx ctx, int C_Currency_ID)
        {
            //	Try Cache
            int key = C_Currency_ID;
            MCurrency retValue = currencies[key];
            if (retValue != null)
                return retValue;
            //	Create it
            retValue = new MCurrency(ctx, C_Currency_ID, null);
            //	Save in System
            if (retValue.GetAD_Client_ID() == 0)
                currencies.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Currency Iso Code.
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <returns>ISO Code</returns>
        public static string GetISO_Code(Ctx ctx, int C_Currency_ID)
        {
            String contextKey = "C_Currency_" + C_Currency_ID;
            String retValue = ctx.GetContext(contextKey);
            if (retValue != null && retValue.Length > 0)
                return retValue;

            //	Create it
            MCurrency c = Get(ctx, C_Currency_ID);
            retValue = c.GetISO_Code();
            ctx.SetContext(contextKey, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Standard Precision.
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <returns>Standard Precision</returns>
        public static int GetStdPrecision(Ctx ctx, int C_Currency_ID)
        {
            MCurrency c = Get(ctx, C_Currency_ID);
            return c.GetStdPrecision();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return "MCurrency[" + GetC_Currency_ID()
                + "-" + GetISO_Code() + "-" + GetCurSymbol()
                + "," + GetDescription()
                + ",Precision=" + GetStdPrecision() + "/" + GetCostingPrecision() + "]";
        }

    }
}

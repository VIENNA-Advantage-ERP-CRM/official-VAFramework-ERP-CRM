/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_Calender
 * Chronological Development
 * Veena Pandey     07-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABCalendar : X_VAB_Calender
    {
        //	Cache
        private static CCache<int, MVABCalendar> cache = new CCache<int, MVABCalendar>("VAB_Calender", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Calender_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABCalendar (Ctx ctx, int VAB_Calender_ID, Trx trxName)
            : base(ctx, VAB_Calender_ID, trxName)
	    {
	    }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABCalendar(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">parent</param>
        public MVABCalendar(MVAFClient client)
            : base(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetName(client.GetName() + " " + Utility.Msg.Translate(client.GetCtx(), "VAB_Calender_ID"));
        }

        /// <summary>
        /// Get MCalendar from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Calender_ID">id</param>
        /// <returns>MCalendar</returns>
        public static MVABCalendar Get(Context ctx, int VAB_Calender_ID)
        {
            int key = VAB_Calender_ID;
            MVABCalendar retValue = (MVABCalendar)cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVABCalendar(ctx, VAB_Calender_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }
        /// <summary>
        /// Get MCalendar from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Calender_ID">id</param>
        /// <returns>MCalendar</returns>
        public static MVABCalendar Get(Ctx ctx, int VAB_Calender_ID)
        {
            int key = VAB_Calender_ID;
            MVABCalendar retValue = (MVABCalendar)cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVABCalendar(ctx, VAB_Calender_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Default Calendar for Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>MCalendar</returns>
        public static MVABCalendar GetDefault(Context ctx)
        {
            return GetDefault(ctx, ctx.GetVAF_Client_ID());
        }

        /// <summary>
        /// Get Default Calendar for Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">id</param>
        /// <returns>MCalendar</returns>
        public static MVABCalendar GetDefault(Context ctx, int VAF_Client_ID)
        {
            MVAFClientDetail info = MVAFClientDetail.Get(ctx, VAF_Client_ID);
            return Get(ctx, info.GetVAB_Calender_ID());
        }

        /// <summary>
        /// Create (current) Calendar Year
        /// </summary>
        /// <param name="locale">locale</param>
        /// <returns>The Year</returns>
        //public MYear CreateYear(Locale locale)
        public MVABYear CreateYear(CultureInfo locale)
        {
            if (Get_ID() == 0)
                return null;
            MVABYear year = new MVABYear(this);
            if (year.Save())
                year.CreateStdPeriods(locale);
            //
            return year;
        }

    }
}

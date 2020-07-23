/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Calendar
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
    public class MCalendar : X_C_Calendar
    {
        //	Cache
        private static CCache<int, MCalendar> cache = new CCache<int, MCalendar>("C_Calendar", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Calendar_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCalendar (Ctx ctx, int C_Calendar_ID, Trx trxName)
            : base(ctx, C_Calendar_ID, trxName)
	    {
	    }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MCalendar(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="client">parent</param>
        public MCalendar(MClient client)
            : base(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetName(client.GetName() + " " + Utility.Msg.Translate(client.GetCtx(), "C_Calendar_ID"));
        }

        /// <summary>
        /// Get MCalendar from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Calendar_ID">id</param>
        /// <returns>MCalendar</returns>
        public static MCalendar Get(Context ctx, int C_Calendar_ID)
        {
            int key = C_Calendar_ID;
            MCalendar retValue = (MCalendar)cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCalendar(ctx, C_Calendar_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }
        /// <summary>
        /// Get MCalendar from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Calendar_ID">id</param>
        /// <returns>MCalendar</returns>
        public static MCalendar Get(Ctx ctx, int C_Calendar_ID)
        {
            int key = C_Calendar_ID;
            MCalendar retValue = (MCalendar)cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCalendar(ctx, C_Calendar_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Default Calendar for Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>MCalendar</returns>
        public static MCalendar GetDefault(Context ctx)
        {
            return GetDefault(ctx, ctx.GetAD_Client_ID());
        }

        /// <summary>
        /// Get Default Calendar for Client
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">id</param>
        /// <returns>MCalendar</returns>
        public static MCalendar GetDefault(Context ctx, int AD_Client_ID)
        {
            MClientInfo info = MClientInfo.Get(ctx, AD_Client_ID);
            return Get(ctx, info.GetC_Calendar_ID());
        }

        /// <summary>
        /// Create (current) Calendar Year
        /// </summary>
        /// <param name="locale">locale</param>
        /// <returns>The Year</returns>
        //public MYear CreateYear(Locale locale)
        public MYear CreateYear(CultureInfo locale)
        {
            if (Get_ID() == 0)
                return null;
            MYear year = new MYear(this);
            if (year.Save())
                year.CreateStdPeriods(locale);
            //
            return year;
        }

    }
}

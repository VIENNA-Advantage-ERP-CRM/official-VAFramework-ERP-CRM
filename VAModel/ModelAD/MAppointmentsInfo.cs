using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MAppointmentsInfo : X_AppointmentsInfo
    {

        public MAppointmentsInfo(Context ctx, int AppointmentsInfo_ID, Trx trxName)
            : base(ctx, AppointmentsInfo_ID, trxName)
        {
            /** if (AppointmentsInfo_ID == 0)
            {
            SetAppointmentsInfo_ID (0);
            }
             */
        }
        public MAppointmentsInfo(Ctx ctx, int AppointmentsInfo_ID, Trx trxName)
            : base(ctx, AppointmentsInfo_ID, trxName)
        {
            /** if (AppointmentsInfo_ID == 0)
            {
            SetAppointmentsInfo_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public MAppointmentsInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public MAppointmentsInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public MAppointmentsInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}

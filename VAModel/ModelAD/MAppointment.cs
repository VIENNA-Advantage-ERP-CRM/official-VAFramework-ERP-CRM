using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Utility;
namespace VAdvantage.Model
{

    class MAppointment : X_Appointment
    {
        /**
	  * Load Constructor
	  * @param ctx context
	  * @param rs result set
	  * @param trxName transaction
	  */
        public MAppointment(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

            // TODO Auto-generated constructor stub
        }
        /**
        * 	Standard Constructor
        *	@param ctx context
        *	@param FO_ARRANGEMENT_ID id
        *	@param trxName transaction
        */
        public MAppointment(Context ctx, int MAPPOPINTMENT,
                Trx trxName)
            : base(ctx, MAPPOPINTMENT, trxName)
        {

            // TODO Auto-generated constructor stub
        }

        /**
       * 	Standard Constructor
       *	@param ctx context
       *	@param FO_ARRANGEMENT_ID id
       *	@param trxName transaction
       */
        public MAppointment(Ctx ctx, int MAPPOPINTMENT,
                Trx trxName)
            : base(ctx, MAPPOPINTMENT, trxName)
        {

            // TODO Auto-generated constructor stub
        }

    }
}
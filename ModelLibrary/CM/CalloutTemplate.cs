using System;
using System.Collections.Generic;
//using System.Linq;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.CM;
using VAdvantage.Logging;
using System.Text;
using VAdvantage.Controller;

namespace VAdvantage.CM
{
    public class CalloutTemplate : CalloutEngine
    {
        /**
         *	Invoice Line - Charge.
         * 		- updates PriceActual from Charge
         * 		- sets PriceLimit, PriceList to zero
         * 	Calles tax
         *	@param ctx context
         *	@param WindowNo window no
         *	@param mTab tab
         *	@param mField field
         *	@param value value
         *	@return null or error message
         */
        public String Invalidate(Ctx ctx, int WindowNo, GridTab mTab, GridField mField, Object value)
        {
            //	Summary ?
            if (mTab.GetValue("IsSummary") != null)
            {
                mTab.SetValue("IsValid", false);
                return "";
            }
            return "";
        }	//	charge

    }
}

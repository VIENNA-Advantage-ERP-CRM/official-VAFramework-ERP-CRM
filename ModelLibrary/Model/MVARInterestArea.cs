/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVARInterestArea
 * Purpose        : Interest Area.
 *                  manually
 * Class Used     : VAR_InterestArea
 * Chronological    Development
 * Raghunandan      24-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVARInterestArea : X_VAR_InterestArea
    {
        private int _VAF_UserContact_ID = -1;
        private MVARInterestedUser _ci = null;
        /**	Cache						*/
        private static CCache<int, MVARInterestArea> _cache =
            new CCache<int, MVARInterestArea>("X_VAR_InterestArea", 5);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVARInterestArea).FullName);


        /**
        * 	Get all active interest areas
        *	@param ctx context
        *	@return interest areas
        */
        public static MVARInterestArea[] GetAll(Ctx ctx)
        {
            List<MVARInterestArea> list = new List<MVARInterestArea>();
            String sql = "SELECT * FROM VAR_InterestArea WHERE IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MVARInterestArea ia = new MVARInterestArea(ctx, dr, null);
                    list.Add(ia);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
               _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            
            MVARInterestArea[] retValue = new MVARInterestArea[list.Count];
            retValue=list.ToArray();
            return retValue;
        }


        /**
         * 	Get MVARInterestArea from Cache
         *	@param ctx context
         *	@param VAR_InterestArea_ID id
         *	@return MVARInterestArea
         */
        public static MVARInterestArea Get(Ctx ctx, int VAR_InterestArea_ID)
        {
            int key = VAR_InterestArea_ID;
            MVARInterestArea retValue = (MVARInterestArea)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVARInterestArea(ctx, VAR_InterestArea_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } 

        /**
         * 	Constructor
         *	@param ctx context
         *	@param VAR_InterestArea_ID interest area
         *	@param trxName transaction
         */
        public MVARInterestArea(Ctx ctx, int VAR_InterestArea_ID, Trx trxName)
            : base(ctx, VAR_InterestArea_ID, trxName)
        {
            
            if (VAR_InterestArea_ID == 0)
            {
                //	setName (null);
                //	setR_InterestArea_ID (0);
                SetIsSelfService(false);
            }
        }	

        /**
         * 	Loader Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVARInterestArea(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            
        }


        /**
         * 	Get Value
         *	@return value
         */
        public new String GetValue()
        {
            String s = base.GetValue();
            if (s != null && s.Length > 0)
                return s;
            return base.GetName();
        }	

        /**
         * 	String representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVARInterestArea[")
                .Append(Get_ID()).Append(" - ").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        /**	Set Subscription info "constructor".
         * 	Create inactive Subscription
         *	@param VAF_UserContact_ID contact
         */
        public void SetSubscriptionInfo(int VAF_UserContact_ID)
        {
            _VAF_UserContact_ID = VAF_UserContact_ID;
            _ci = MVARInterestedUser.Get(GetCtx(), GetR_InterestArea_ID(), VAF_UserContact_ID,
                false, Get_TrxName());
        }

        /**
         * 	Set VAF_UserContact_ID
         *	@param VAF_UserContact_ID user
         */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            _VAF_UserContact_ID = VAF_UserContact_ID;
        }

        /**
         * 	Get VAF_UserContact_ID
         *	@return user
         */
        public int GetVAF_UserContact_ID()
        {
            return _VAF_UserContact_ID;
        }

        /**
         * 	Get Subscribe Date
         *	@return subscribe date
         */
        public DateTime? GetSubscribeDate()
        {
            if (_ci != null)
                return _ci.GetSubscribeDate();
            return null;
        }

        /**
         * 	Get Opt Out Date
         *	@return opt-out date
         */
        public DateTime? GetOptOutDate()
        {
            if (_ci != null)
                return _ci.GetOptOutDate();
            return null;
        }

        /**
         * 	Is Subscribed
         *	@return true if subscribed
         */
        public bool IsSubscribed()
        {
            if (_VAF_UserContact_ID <= 0 || _ci == null)
                return false;
            //	We have a BPartner Contact
            return _ci.IsSubscribed();
        }
    }
}

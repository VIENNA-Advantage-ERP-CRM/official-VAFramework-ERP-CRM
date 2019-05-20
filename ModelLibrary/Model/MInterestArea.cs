/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInterestArea
 * Purpose        : Interest Area.
 *                  manually
 * Class Used     : X_R_InterestArea
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
using System.Windows.Forms;
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
    public class MInterestArea : X_R_InterestArea
    {
        private int _AD_User_ID = -1;
        private MContactInterest _ci = null;
        /**	Cache						*/
        private static CCache<int, MInterestArea> _cache =
            new CCache<int, MInterestArea>("R_InterestArea", 5);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MInterestArea).FullName);


        /**
        * 	Get all active interest areas
        *	@param ctx context
        *	@return interest areas
        */
        public static MInterestArea[] GetAll(Ctx ctx)
        {
            List<MInterestArea> list = new List<MInterestArea>();
            String sql = "SELECT * FROM R_InterestArea WHERE IsActive='Y'";
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
                    MInterestArea ia = new MInterestArea(ctx, dr, null);
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
            
            MInterestArea[] retValue = new MInterestArea[list.Count];
            retValue=list.ToArray();
            return retValue;
        }


        /**
         * 	Get MInterestArea from Cache
         *	@param ctx context
         *	@param R_InterestArea_ID id
         *	@return MInterestArea
         */
        public static MInterestArea Get(Ctx ctx, int R_InterestArea_ID)
        {
            int key = R_InterestArea_ID;
            MInterestArea retValue = (MInterestArea)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MInterestArea(ctx, R_InterestArea_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } 

        /**
         * 	Constructor
         *	@param ctx context
         *	@param R_InterestArea_ID interest area
         *	@param trxName transaction
         */
        public MInterestArea(Ctx ctx, int R_InterestArea_ID, Trx trxName)
            : base(ctx, R_InterestArea_ID, trxName)
        {
            
            if (R_InterestArea_ID == 0)
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
        public MInterestArea(Ctx ctx, DataRow dr, Trx trxName)
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
            StringBuilder sb = new StringBuilder("MInterestArea[")
                .Append(Get_ID()).Append(" - ").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        /**	Set Subscription info "constructor".
         * 	Create inactive Subscription
         *	@param AD_User_ID contact
         */
        public void SetSubscriptionInfo(int AD_User_ID)
        {
            _AD_User_ID = AD_User_ID;
            _ci = MContactInterest.Get(GetCtx(), GetR_InterestArea_ID(), AD_User_ID,
                false, Get_TrxName());
        }

        /**
         * 	Set AD_User_ID
         *	@param AD_User_ID user
         */
        public void SetAD_User_ID(int AD_User_ID)
        {
            _AD_User_ID = AD_User_ID;
        }

        /**
         * 	Get AD_User_ID
         *	@return user
         */
        public int GetAD_User_ID()
        {
            return _AD_User_ID;
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
            if (_AD_User_ID <= 0 || _ci == null)
                return false;
            //	We have a BPartner Contact
            return _ci.IsSubscribed();
        }
    }
}

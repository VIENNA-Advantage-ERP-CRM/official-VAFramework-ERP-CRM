/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Business Partner Contact Interest.
 *                  Vienna complies with spam laws.
 *                  If the opt out date is set (by the user), 
 *                  you should not subscribe the user again.
 *                  Internally, the isActive flag is used.
 * Purpose        : Charge Modle
 * Class Used     : X_R_ContactInterest
 * Chronological    Development
 * Raghunandan      23-Jun-2009
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
using System.Data.SqlClient;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MContactInterest : X_R_ContactInterest
    {
        /**
	 * 	Get Contact Interest
	 *	@param ctx context
	 *	@param R_InterestArea_ID interest ares
	 *	@param AD_User_ID user
	 * 	@param isActive create as active
	 *	@param trxName transaction
	 *	@return Contact Interest 
	 */
        public static MContactInterest Get(Ctx ctx,
            int R_InterestArea_ID, int AD_User_ID, Boolean isActive, Trx trxName)
        {
            MContactInterest retValue = null;
            String sql = "SELECT * FROM R_ContactInterest "
                + "WHERE R_InterestArea_ID=@R_InterestArea_ID AND AD_User_ID=@AD_User_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@R_InterestArea_ID", R_InterestArea_ID);
                param[1] = new SqlParameter("@AD_User_ID", AD_User_ID);

                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MContactInterest(ctx, dr, trxName);
                    break;
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

            //	New
            if (retValue == null)
            {
                retValue = new MContactInterest(ctx, R_InterestArea_ID, AD_User_ID,isActive, trxName);
                	_log.Fine("NOT found - " + retValue);
            }
            else
            {
                _log.Fine("Found - " + retValue);
            }
            return retValue;
        }	


        /**************************************************************************
         * 	Persistency Constructor
         *	@param ctx context
         *	@param ignored ignored
         *	@param trxName transaction
         */
        public MContactInterest(Ctx ctx, int ignored, Trx trxName) :
            base(ctx, 0, trxName)
        {

            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
        }	//	MContactInterest

        /**
         * 	Constructor
         * 	@param ctx context
         * 	@param R_InterestArea_ID interest area
         * 	@param AD_User_ID partner contact
         * 	@param isActive create as active
         *	@param trxName transaction
         */
        public MContactInterest(Ctx ctx, int R_InterestArea_ID, int AD_User_ID,
            Boolean isActive, Trx trxName) :
            base(ctx, 0, trxName)
        {
            //super(ctx, 0, trxName);
            SetR_InterestArea_ID(R_InterestArea_ID);
            SetAD_User_ID(AD_User_ID);
            SetIsActive(isActive);
        }	//	MContactInterest

        /**
         *  Create & Load existing Persistent Object.
         *  @param ctx context
         *  @param dr load from current result Set position (no navigation, not closed)
         *	@param trxName transaction
         */
        public MContactInterest(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //	super();
        }	//	MContactInterest

        //	Static Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MContactInterest).FullName);

        /**
         * 	Set OptOut Date
         * 	User action only.
         *	@param OptOutDate date
         */
        public new void SetOptOutDate(DateTime? OptOutDate)
        {
            if (OptOutDate == null)
                OptOutDate = DateTime.Now;
            log.Fine("" + OptOutDate);
            base.SetOptOutDate(OptOutDate);
            SetIsActive(false);
        }

        /**
         * 	Unsubscribe.
         * 	User action only.
         */
        public void Unsubscribe()
        {
            SetOptOutDate(null);
        }	

        /**
         * 	Is Opted Out
         *	@return true if opted out
         */
        public Boolean IsOptOut()
        {
            return GetOptOutDate() != null;
        }	//	isOptOut

        /**
         * 	Set Subscribe Date
         * 	User action only.
         *	@param SubscribeDate date
         */
        public new void SetSubscribeDate(DateTime? SubscribeDate)
        {
            if (SubscribeDate == null)
                SubscribeDate = DateTime.Now;// new Timestamp(System.currentTimeMillis());
            log.Fine("" + SubscribeDate);
            base.SetSubscribeDate(SubscribeDate);
            base.SetOptOutDate(null);
            SetIsActive(true);
        }	

        /**
         * 	Subscribe
         * 	User action only.
         */
        public void Subscribe()
        {
            SetSubscribeDate(null);
            if (!IsActive())
                SetIsActive(true);
        }	

        /**
         * 	Subscribe.
         * 	User action only.
         * 	@param subscribe subscribe
         */
        public void Subscribe(Boolean subscribe)
        {
            if (subscribe)
                SetSubscribeDate(null);
            else
                SetOptOutDate(null);
        }	//	subscribe


        /**
         * 	Is Subscribed.
         * 	Active is Set internally, 
         * 	the opt out date is Set by the user via the web UI.
         *	@return true if subscribed
         */
        public Boolean IsSubscribed()
        {
            return IsActive() && GetOptOutDate() == null;
        }	//	isSubscribed

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (success && newRecord && IsSubscribed())
            {
                MInterestArea ia = MInterestArea.Get(GetCtx(), GetR_InterestArea_ID());
                if (ia.GetR_Source_ID() != 0)
                {
                    String summary = "Subscribe: " + ia.GetName();
                    //
                    MSource source = MSource.Get(GetCtx(), ia.GetR_Source_ID());
                    MUser user = null;
                    if (Get_TrxName() == null)
                        user = MUser.Get(GetCtx(), GetAD_User_ID());
                    else
                        user = new MUser(GetCtx(), GetAD_User_ID(), Get_TrxName());
                    //	Create Request
                    if (MSource.SOURCECREATETYPE_Both.Equals(source.GetSourceCreateType())
                        || MSource.SOURCECREATETYPE_Request.Equals(source.GetSourceCreateType()))
                    {
                        MRequest request = new MRequest(GetCtx(), 0, Get_TrxName());
                        request.SetClientOrg(this);
                        request.SetSummary(summary);
                        request.SetAD_User_ID(GetAD_User_ID());
                        request.SetC_BPartner_ID(user.GetC_BPartner_ID());
                        request.SetR_Source_ID(source.GetR_Source_ID());
                        request.Save();
                    }
                    //	Create Lead
                    if (MSource.SOURCECREATETYPE_Both.Equals(source.GetSourceCreateType())
                        || MSource.SOURCECREATETYPE_Lead.Equals(source.GetSourceCreateType()))
                    {
                        MLead lead = new MLead(GetCtx(), 0, Get_TrxName());
                        lead.SetClientOrg(this);
                        lead.SetDescription(summary);
                        lead.SetAD_User_ID(GetAD_User_ID());
                        lead.SetR_InterestArea_ID(GetR_InterestArea_ID());
                        lead.SetC_BPartner_ID(user.GetC_BPartner_ID());
                        lead.SetR_Source_ID(source.GetR_Source_ID());
                        lead.Save();
                    }
                }
            }
            return success;
        }	//	afterSave

        /**
         * 	String representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MContactInterest[")
                .Append("R_InterestArea_ID=").Append(GetR_InterestArea_ID())
                .Append(",AD_User_ID=").Append(GetAD_User_ID())
                .Append(",Subscribed=").Append(IsSubscribed())
                .Append("]");
            return sb.ToString();
        }	//	toString

       

    }
}

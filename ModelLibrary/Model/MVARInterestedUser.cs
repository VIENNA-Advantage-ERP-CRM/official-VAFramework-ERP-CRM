/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Business Partner Contact Interest.
 *                  Vienna complies with spam laws.
 *                  If the opt out date is set (by the user), 
 *                  you should not subscribe the user again.
 *                  Internally, the isActive flag is used.
 * Purpose        : Charge Modle
 * Class Used     : X_VAR_InterestedUser
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVARInterestedUser : X_VAR_InterestedUser
    {
        /**
	 * 	Get Contact Interest
	 *	@param ctx context
	 *	@param VAR_InterestArea_ID interest ares
	 *	@param VAF_UserContact_ID user
	 * 	@param isActive create as active
	 *	@param trxName transaction
	 *	@return Contact Interest 
	 */
        public static MVARInterestedUser Get(Ctx ctx,
            int VAR_InterestArea_ID, int VAF_UserContact_ID, Boolean isActive, Trx trxName)
        {
            MVARInterestedUser retValue = null;
            String sql = "SELECT * FROM VAR_InterestedUser "
                + "WHERE VAR_InterestArea_ID=@VAR_InterestArea_ID AND VAF_UserContact_ID=@VAF_UserContact_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@VAR_InterestArea_ID", VAR_InterestArea_ID);
                param[1] = new SqlParameter("@VAF_UserContact_ID", VAF_UserContact_ID);

                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVARInterestedUser(ctx, dr, trxName);
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
                retValue = new MVARInterestedUser(ctx, VAR_InterestArea_ID, VAF_UserContact_ID,isActive, trxName);
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
        public MVARInterestedUser(Ctx ctx, int ignored, Trx trxName) :
            base(ctx, 0, trxName)
        {

            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
        }	//	MVARInterestedUser

        /**
         * 	Constructor
         * 	@param ctx context
         * 	@param VAR_InterestArea_ID interest area
         * 	@param VAF_UserContact_ID partner contact
         * 	@param isActive create as active
         *	@param trxName transaction
         */
        public MVARInterestedUser(Ctx ctx, int VAR_InterestArea_ID, int VAF_UserContact_ID,
            Boolean isActive, Trx trxName) :
            base(ctx, 0, trxName)
        {
            //super(ctx, 0, trxName);
            SetR_InterestArea_ID(VAR_InterestArea_ID);
            SetVAF_UserContact_ID(VAF_UserContact_ID);
            SetIsActive(isActive);
        }	//	MVARInterestedUser

        /**
         *  Create & Load existing Persistent Object.
         *  @param ctx context
         *  @param dr load from current result Set position (no navigation, not closed)
         *	@param trxName transaction
         */
        public MVARInterestedUser(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //	super();
        }	//	MVARInterestedUser

        //	Static Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MVARInterestedUser).FullName);

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
                MVARInterestArea ia = MVARInterestArea.Get(GetCtx(), GetR_InterestArea_ID());
                if (ia.GetVAR_Source_ID() != 0)
                {
                    String summary = "Subscribe: " + ia.GetName();
                    //
                    MSource source = MSource.Get(GetCtx(), ia.GetVAR_Source_ID());
                    MVAFUserContact user = null;
                    if (Get_TrxName() == null)
                        user = MVAFUserContact.Get(GetCtx(), GetVAF_UserContact_ID());
                    else
                        user = new MVAFUserContact(GetCtx(), GetVAF_UserContact_ID(), Get_TrxName());
                    //	Create Request
                    if (MSource.SOURCECREATETYPE_Both.Equals(source.GetSourceCreateType())
                        || MSource.SOURCECREATETYPE_Request.Equals(source.GetSourceCreateType()))
                    {
                        MRequest request = new MRequest(GetCtx(), 0, Get_TrxName());
                        request.SetClientOrg(this);
                        request.SetSummary(summary);
                        request.SetVAF_UserContact_ID(GetVAF_UserContact_ID());
                        request.SetVAB_BusinessPartner_ID(user.GetVAB_BusinessPartner_ID());
                        request.SetVAR_Source_ID(source.GetVAR_Source_ID());
                        request.Save();
                    }
                    //	Create Lead
                    if (MSource.SOURCECREATETYPE_Both.Equals(source.GetSourceCreateType())
                        || MSource.SOURCECREATETYPE_Lead.Equals(source.GetSourceCreateType()))
                    {
                        MVABLead lead = new MVABLead(GetCtx(), 0, Get_TrxName());
                        lead.SetClientOrg(this);
                        lead.SetDescription(summary);
                        lead.SetVAF_UserContact_ID(GetVAF_UserContact_ID());
                        lead.SetR_InterestArea_ID(GetR_InterestArea_ID());
                        lead.SetVAB_BusinessPartner_ID(user.GetVAB_BusinessPartner_ID());
                        lead.SetVAR_Source_ID(source.GetVAR_Source_ID());
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
            StringBuilder sb = new StringBuilder("MVARInterestedUser[")
                .Append("VAR_InterestArea_ID=").Append(GetR_InterestArea_ID())
                .Append(",VAF_UserContact_ID=").Append(GetVAF_UserContact_ID())
                .Append(",Subscribed=").Append(IsSubscribed())
                .Append("]");
            return sb.ToString();
        }	//	toString

       

    }
}

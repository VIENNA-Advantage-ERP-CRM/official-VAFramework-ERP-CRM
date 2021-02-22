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
    public class MVARReqStatus : X_VAR_Req_Status
    {
        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MVARReqStatus).FullName);
        //private static CLogger s_log = CLogger.GetCLogger(MVARReqStatus.class);
        /**	Cache							*/
        static private CCache<int, MVARReqStatus> _cache = new CCache<int, MVARReqStatus>("VAR_Req_Status", 10);
        /**	Default Cache (Key=Client)		*/
        static private CCache<int, MVARReqStatus> _cacheDefault = new CCache<int, MVARReqStatus>("VAR_Req_Status", 10);


        /**
	 * 	Get Request Status (cached)
	 *	@param ctx context
	 *	@param VAR_Req_Status_ID id
	 *	@return Request Status or null
	 */
        public static MVARReqStatus Get(Ctx ctx, int VAR_Req_Status_ID)
        {
            if (VAR_Req_Status_ID == 0)
                return null;
            int key = VAR_Req_Status_ID;
            MVARReqStatus retValue = (MVARReqStatus)_cache[key];
            if (retValue == null)
            {
                retValue = new MVARReqStatus(ctx, VAR_Req_Status_ID, null);
                _cache.Add(key, retValue);
            }
            return retValue;
        }	//	Get

        /**
         * 	Get Default Request Status
         *	@param ctx context
         *	@param VAR_Req_Type_ID request type
         *	@return Request Type
         */
        public static MVARReqStatus GetDefault(Ctx ctx, int VAR_Req_Type_ID)
        {
            int key = VAR_Req_Type_ID;
            MVARReqStatus retValue = (MVARReqStatus)_cacheDefault[key];
            if (retValue != null)
                return retValue;
            //	Get New
            String sql = "SELECT * FROM VAR_Req_Status s "
                + "WHERE EXISTS (SELECT * FROM VAR_Req_Type rt "
                    + "WHERE rt.VAR_Req_StatusCategory_ID=s.VAR_Req_StatusCategory_ID"
                    + " AND rt.VAR_Req_Type_ID=@VAR_Req_Type_ID)"
                + " AND IsDefault='Y' "
                + "ORDER BY SeqNo";
            //PreparedStatement pstmt = null;
            DataTable dt=null;
            IDataReader idr=null;
            try
            {
                //	pstmt = DataBase.prepareStatement (sql, null);
                //	pstmt.SetInt(1, VAR_Req_Type_ID);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAR_Req_Type_ID", VAR_Req_Type_ID);
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                //ResultSet dr = pstmt.executeQuery ();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVARReqStatus(ctx, dr, null);
                }
                
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null; 
            }
            if (retValue != null)
                _cacheDefault.Add(key, retValue);
            return retValue;
        }	//	GetDefault

        /**
         * 	Get Closed Status
         *	@param ctx context
         *	@return Request Type
         */
        public static MVARReqStatus[] GetClosed(Ctx ctx)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            String sql = "SELECT * FROM VAR_Req_Status "
                + "WHERE VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y' AND IsClosed='Y' "
                + "ORDER BY Value";
            List<MVARReqStatus> list = new List<MVARReqStatus>();
            IDataReader idr = null;
            DataTable dt = null ;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVARReqStatus(ctx, dr, null));
                }

            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Severe(ex.ToString());
            }
            finally {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MVARReqStatus[] retValue = new MVARReqStatus[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /***
         * 	Default Constructor
         *	@param ctx context
         *	@param VAR_Req_Status_ID is
         *	@param trxName trx
         */
        public MVARReqStatus(Ctx ctx, int VAR_Req_Status_ID, Trx trxName) :
            base(ctx, VAR_Req_Status_ID, trxName)
        {
            //super (ctx, VAR_Req_Status_ID, trxName);
            if (VAR_Req_Status_ID == 0)
            {
                //	SetValue (null);
                //	SetName (null);
                SetIsClosed(false);	// N
                SetIsDefault(false);
                SetIsFinalClose(false);	// N
                SetIsOpen(false);
                SetIsWebCanUpdate(true);
            }
        }	//	MVARReqStatus

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MVARReqStatus(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, dr, trxName);
        }	//	MVARReqStatus


        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (IsOpen() && IsClosed())
                SetIsClosed(false);
            if (IsFinalClose() && !IsClosed())
                SetIsFinalClose(false);
            //
            if (!IsWebCanUpdate() && GetUpdate_Status_ID() != 0)
                SetUpdate_Status_ID(0);
            if (GetTimeoutDays() == 0 && GetNext_Status_ID() != 0)
                SetNext_Status_ID(0);
            //
            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVARReqStatus[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }
}

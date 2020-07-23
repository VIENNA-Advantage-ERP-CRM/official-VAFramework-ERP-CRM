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
    public class MStatus : X_R_Status
    {
        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MStatus).FullName);
        //private static CLogger s_log = CLogger.GetCLogger(MStatus.class);
        /**	Cache							*/
        static private CCache<int, MStatus> _cache = new CCache<int, MStatus>("R_Status", 10);
        /**	Default Cache (Key=Client)		*/
        static private CCache<int, MStatus> _cacheDefault = new CCache<int, MStatus>("R_Status", 10);


        /**
	 * 	Get Request Status (cached)
	 *	@param ctx context
	 *	@param R_Status_ID id
	 *	@return Request Status or null
	 */
        public static MStatus Get(Ctx ctx, int R_Status_ID)
        {
            if (R_Status_ID == 0)
                return null;
            int key = R_Status_ID;
            MStatus retValue = (MStatus)_cache[key];
            if (retValue == null)
            {
                retValue = new MStatus(ctx, R_Status_ID, null);
                _cache.Add(key, retValue);
            }
            return retValue;
        }	//	Get

        /**
         * 	Get Default Request Status
         *	@param ctx context
         *	@param R_RequestType_ID request type
         *	@return Request Type
         */
        public static MStatus GetDefault(Ctx ctx, int R_RequestType_ID)
        {
            int key = R_RequestType_ID;
            MStatus retValue = (MStatus)_cacheDefault[key];
            if (retValue != null)
                return retValue;
            //	Get New
            String sql = "SELECT * FROM R_Status s "
                + "WHERE EXISTS (SELECT * FROM R_RequestType rt "
                    + "WHERE rt.R_StatusCategory_ID=s.R_StatusCategory_ID"
                    + " AND rt.R_RequestType_ID=@R_RequestType_ID)"
                + " AND IsDefault='Y' "
                + "ORDER BY SeqNo";
            //PreparedStatement pstmt = null;
            DataTable dt=null;
            IDataReader idr=null;
            try
            {
                //	pstmt = DataBase.prepareStatement (sql, null);
                //	pstmt.SetInt(1, R_RequestType_ID);
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@R_RequestType_ID", R_RequestType_ID);
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                //ResultSet dr = pstmt.executeQuery ();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MStatus(ctx, dr, null);
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
        public static MStatus[] GetClosed(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM R_Status "
                + "WHERE AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y' AND IsClosed='Y' "
                + "ORDER BY Value";
            List<MStatus> list = new List<MStatus>();
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
                    list.Add(new MStatus(ctx, dr, null));
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

            MStatus[] retValue = new MStatus[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /***
         * 	Default Constructor
         *	@param ctx context
         *	@param R_Status_ID is
         *	@param trxName trx
         */
        public MStatus(Ctx ctx, int R_Status_ID, Trx trxName) :
            base(ctx, R_Status_ID, trxName)
        {
            //super (ctx, R_Status_ID, trxName);
            if (R_Status_ID == 0)
            {
                //	SetValue (null);
                //	SetName (null);
                SetIsClosed(false);	// N
                SetIsDefault(false);
                SetIsFinalClose(false);	// N
                SetIsOpen(false);
                SetIsWebCanUpdate(true);
            }
        }	//	MStatus

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MStatus(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, dr, trxName);
        }	//	MStatus


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
            StringBuilder sb = new StringBuilder("MStatus[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }
}

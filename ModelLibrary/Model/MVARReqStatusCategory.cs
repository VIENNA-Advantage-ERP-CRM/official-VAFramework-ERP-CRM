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
    public class MVARReqStatusCategory : X_VAR_Req_StatusCategory
    {
        /**
         * 	Get Default Status Categpru for Client
         *	@param ctx context
         *	@return status category or null
         */
        public static MVARReqStatusCategory GetDefault(Ctx ctx)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            String sql = "SELECT * FROM VAR_Req_StatusCategory "
                + "WHERE VAF_Client_ID in (0,@VAF_Client_ID) AND IsDefault='Y' "
                + "ORDER BY VAF_Client_ID DESC";
            MVARReqStatusCategory retValue = null;
            //PreparedStatement pstmt = null;
            DataTable dt = null;
            IDataReader idr=null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAF_Client_ID", VAF_Client_ID);

                //pstmt = DataBase.prepareStatement (sql, null);
                //pstmt.SetInt (1, VAF_Client_ID);
                 idr = DataBase.DB.ExecuteReader(sql, param);
                 dt = new DataTable();
                 dt.Load(idr);
                 idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVARReqStatusCategory(ctx, dr, null);
                }

                //pstmt.close ();
                //pstmt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            //try
            //{
            //    if (pstmt != null)
            //        pstmt.close ();
            //    pstmt = null;
            //}
            //catch (Exception e)
            //{
            //    pstmt = null;
            //}
            return retValue;
        }	//	GetDefault

        /**
         * 	Get Default Status Categpru for Client
         *	@param ctx context
         *	@return status category or null
         */
        public static MVARReqStatusCategory CreateDefault(Ctx ctx)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            MVARReqStatusCategory retValue = new MVARReqStatusCategory(ctx, 0, null);
            retValue.SetClientOrg(VAF_Client_ID, 0);
            retValue.SetName(Msg.GetMsg(ctx, "Standard", true));
            retValue.SetIsDefault(true);
            if (!retValue.Save())
                return null;
            String sql = "UPDATE VAR_Req_Status SET VAR_Req_StatusCategory_ID=" + retValue.GetVAR_Req_StatusCategory_ID()
                + " WHERE VAR_Req_StatusCategory_ID IS NULL AND VAF_Client_ID=" + VAF_Client_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, null);
            _log.Info("Default for VAF_Client_ID=" + VAF_Client_ID + " - Status #" + no);
            return retValue;
        }	

        /**
         * 	Get Request Status Category from Cache
         *	@param ctx context
         *	@param VAR_Req_StatusCategory_ID id
         *	@return RStatusCategory
         */
        public static MVARReqStatusCategory Get(Ctx ctx, int VAR_Req_StatusCategory_ID)
        {
            int key = VAR_Req_StatusCategory_ID;
            MVARReqStatusCategory retValue = (MVARReqStatusCategory)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVARReqStatusCategory(ctx, VAR_Req_StatusCategory_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }	//	Get

        /**	Cache						*/
        private static CCache<int, MVARReqStatusCategory> _cache
            = new CCache<int, MVARReqStatusCategory>("VAR_Req_StatusCategory", 20);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVARReqStatusCategory).FullName);


        /**************************************************************************
         * 	Default Constructor
         *	@param ctx context
         *	@param VAR_Req_StatusCategory_ID id
         *	@param trxName trx
         */
        public MVARReqStatusCategory(Ctx ctx, int VAR_Req_StatusCategory_ID, Trx trxName) :
            base(ctx, VAR_Req_StatusCategory_ID, trxName)
        {
            //super (ctx, VAR_Req_StatusCategory_ID, trxName);
            if (VAR_Req_StatusCategory_ID == 0)
            {
                //	SetName (null);
                SetIsDefault(false);
            }
        }	//	RStatusCategory

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MVARReqStatusCategory(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, dr, trxName);
        }	//	RStatusCategory

        /**	The Status						*/
        private MVARReqStatus[] _status = null;

        /**
         * 	Get all Status
         *	@param reload reload
         *	@return Status array 
         */
        public MVARReqStatus[] GetStatus(Boolean reload)
        {
            if (_status != null && !reload)
                return _status;
            String sql = "SELECT * FROM VAR_Req_Status "
                + "WHERE VAR_Req_StatusCategory_ID= " + GetVAR_Req_StatusCategory_ID()
                + " ORDER BY SeqNo";
            List<MVARReqStatus> list = new List<MVARReqStatus>();
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
                    list.Add(new MVARReqStatus(GetCtx(), dr, null));
                }


            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log (Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            //
            _status = new MVARReqStatus[list.Count];
            _status = list.ToArray();
            return _status;
        }	//	GetStatus

        /**
         * 	Get Default VAR_Req_Status_ID
         *	@return id or 0
         */
        public int GetDefaultVAR_Req_Status_ID()
        {
            if (_status == null)
                GetStatus(false);
            for (int i = 0; i < _status.Length; i++)
            {
                if (_status[i].IsDefault() && _status[i].IsActive())
                    return _status[i].GetVAR_Req_Status_ID();
            }
            if (_status.Length > 0
                && _status[0].IsActive())
                return _status[0].GetVAR_Req_Status_ID();
            return 0;
        }	//	GetDefaultVAR_Req_Status_ID

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("RStatusCategory[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }	//	toString

    }
}

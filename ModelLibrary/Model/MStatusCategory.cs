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
    public class MStatusCategory : X_R_StatusCategory
    {
        /**
         * 	Get Default Status Categpru for Client
         *	@param ctx context
         *	@return status category or null
         */
        public static MStatusCategory GetDefault(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM R_StatusCategory "
                + "WHERE AD_Client_ID in (0,@AD_Client_ID) AND IsDefault='Y' "
                + "ORDER BY AD_Client_ID DESC";
            MStatusCategory retValue = null;
            //PreparedStatement pstmt = null;
            DataTable dt = null;
            IDataReader idr=null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_Client_ID", AD_Client_ID);

                //pstmt = DataBase.prepareStatement (sql, null);
                //pstmt.SetInt (1, AD_Client_ID);
                 idr = DataBase.DB.ExecuteReader(sql, param);
                 dt = new DataTable();
                 dt.Load(idr);
                 idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MStatusCategory(ctx, dr, null);
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
        public static MStatusCategory CreateDefault(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            MStatusCategory retValue = new MStatusCategory(ctx, 0, null);
            retValue.SetClientOrg(AD_Client_ID, 0);
            retValue.SetName(Msg.GetMsg(ctx, "Standard", true));
            retValue.SetIsDefault(true);
            if (!retValue.Save())
                return null;
            String sql = "UPDATE R_Status SET R_StatusCategory_ID=" + retValue.GetR_StatusCategory_ID()
                + " WHERE R_StatusCategory_ID IS NULL AND AD_Client_ID=" + AD_Client_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, null);
            _log.Info("Default for AD_Client_ID=" + AD_Client_ID + " - Status #" + no);
            return retValue;
        }	

        /**
         * 	Get Request Status Category from Cache
         *	@param ctx context
         *	@param R_StatusCategory_ID id
         *	@return RStatusCategory
         */
        public static MStatusCategory Get(Ctx ctx, int R_StatusCategory_ID)
        {
            int key = R_StatusCategory_ID;
            MStatusCategory retValue = (MStatusCategory)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MStatusCategory(ctx, R_StatusCategory_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }	//	Get

        /**	Cache						*/
        private static CCache<int, MStatusCategory> _cache
            = new CCache<int, MStatusCategory>("R_StatusCategory", 20);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MStatusCategory).FullName);


        /**************************************************************************
         * 	Default Constructor
         *	@param ctx context
         *	@param R_StatusCategory_ID id
         *	@param trxName trx
         */
        public MStatusCategory(Ctx ctx, int R_StatusCategory_ID, Trx trxName) :
            base(ctx, R_StatusCategory_ID, trxName)
        {
            //super (ctx, R_StatusCategory_ID, trxName);
            if (R_StatusCategory_ID == 0)
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
        public MStatusCategory(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, dr, trxName);
        }	//	RStatusCategory

        /**	The Status						*/
        private MStatus[] _status = null;

        /**
         * 	Get all Status
         *	@param reload reload
         *	@return Status array 
         */
        public MStatus[] GetStatus(Boolean reload)
        {
            if (_status != null && !reload)
                return _status;
            String sql = "SELECT * FROM R_Status "
                + "WHERE R_StatusCategory_ID= " + GetR_StatusCategory_ID()
                + " ORDER BY SeqNo";
            List<MStatus> list = new List<MStatus>();
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
                    list.Add(new MStatus(GetCtx(), dr, null));
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
            _status = new MStatus[list.Count];
            _status = list.ToArray();
            return _status;
        }	//	GetStatus

        /**
         * 	Get Default R_Status_ID
         *	@return id or 0
         */
        public int GetDefaultR_Status_ID()
        {
            if (_status == null)
                GetStatus(false);
            for (int i = 0; i < _status.Length; i++)
            {
                if (_status[i].IsDefault() && _status[i].IsActive())
                    return _status[i].GetR_Status_ID();
            }
            if (_status.Length > 0
                && _status[0].IsActive())
                return _status[0].GetR_Status_ID();
            return 0;
        }	//	GetDefaultR_Status_ID

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

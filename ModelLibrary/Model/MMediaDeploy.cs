/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMediaDeploy
 * Purpose        : Media Deployment Model
 * Class Used     : X_CM_MediaDeploy
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMediaDeploy : X_CM_MediaDeploy
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_MediaDeploy_ID">id</param>
        /// <param name="trxName">trx</param>
        public MMediaDeploy(Ctx ctx, int CM_MediaDeploy_ID, Trx trxName)
            : base(ctx, CM_MediaDeploy_ID, trxName)
        {

        }	//	MMediaDeploy

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MMediaDeploy(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MMediaDeploy
        public MMediaDeploy(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }

        /// <summary>
        /// Deployment Parent Constructor
        /// </summary>
        /// <param name="server">server</param>
        /// <param name="media">media</param>
        public MMediaDeploy(MMediaServer server, MMedia media)
            : this(server.GetCtx(), 0, server.Get_TrxName())
        {

            SetCM_Media_Server_ID(server.GetCM_Media_Server_ID());
            SetCM_Media_ID(media.GetCM_Media_ID());
            SetClientOrg(server);
            //
            SetIsDeployed(true);
            SetLastSynchronized(DateTime.Now);// new Timestamp(System.currentTimeMillis()));
        }	//	MMediaDeploy


        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MMedia).FullName);//.class);

        /// <summary>
        /// getByMediaAndProject Get All deployers by Media ID and WebProject
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="CM_Media_ID">id</param>
        /// <param name="thisProject">web project</param>
        /// <param name="createIfMissing">createIfMissing Whether we create or not</param>
        /// <param name="trxName">trx</param>
        /// <returns> Array of MediaDeploy</returns>
        public static MMediaDeploy[] GetByMediaAndProject(Ctx ctx, int CM_Media_ID, MWebProject thisProject, bool createIfMissing, Trx trxName)
        {
            List<MMediaDeploy> list = new List<MMediaDeploy>();
            MMediaServer[] theseServers = MMediaServer.GetMediaServer(thisProject);
            if (theseServers != null && theseServers.Length > 0)
                for (int i = 0; i < theseServers.Length; i++)
                {
                    list.Add(GetByMedia(ctx, CM_Media_ID, theseServers[i].Get_ID(), createIfMissing, trxName));
                }
            MMediaDeploy[] retValue = new MMediaDeploy[list.Count];// .size ()];
            retValue = list.ToArray();
            return retValue;
        }

        /** _mserver 	contains current MMediaServer */
        private MMediaServer _mserver = null;

        /// <summary>
        /// getServer getCurrentMediaServer
        /// </summary>
        /// <returns>return media server</returns>
        public MMediaServer GetServer()
        {
            if (_mserver == null)
            {
                _mserver = new MMediaServer(GetCtx(), GetCM_Media_Server_ID(), Get_TrxName());
            }
            return _mserver;
        }

        /// <summary>
        /// getByMedia returns MMediaDeploy Object corresponding to an MMedia Item
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_Media_ID">id of media item</param>
        /// <param name="CM_Media_Server_ID">id</param>
        /// <param name="createIfMissing"><add missing entery/param>
        /// <param name="trxName">trx</param>
        /// <returns> Object or NULL if not existant</returns>
        public static MMediaDeploy GetByMedia(Ctx ctx, int CM_Media_ID, int CM_Media_Server_ID, bool createIfMissing, Trx trxName)
        {
            MMediaDeploy thisMMediaDeploy = null;
            String sql = "SELECT * FROM CM_MediaDeploy WHERE CM_Media_ID=@param1 AND CM_Media_Server_ID=@param2";
            SqlParameter[] param = new SqlParameter[2];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, trxName);
                //pstmt.setInt (1, CM_Media_ID);
                param[0] = new SqlParameter("@param1", CM_Media_ID);
                //pstmt.setInt (2, CM_Media_Server_ID);
                param[1] = new SqlParameter("@param2", CM_Media_Server_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (idr.Read())
                {
                    thisMMediaDeploy = (new MMediaDeploy(ctx, idr, trxName));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            if (thisMMediaDeploy == null && createIfMissing)
            {
                thisMMediaDeploy = new MMediaDeploy(ctx, 0, trxName);
                thisMMediaDeploy.SetCM_Media_Server_ID(CM_Media_Server_ID);
                thisMMediaDeploy.SetCM_Media_ID(CM_Media_ID);
                thisMMediaDeploy.SetIsDeployed(false);
                thisMMediaDeploy.SetLastSynchronized(null);
                thisMMediaDeploy.Save();
            }
            return thisMMediaDeploy;
        }

    }	
}

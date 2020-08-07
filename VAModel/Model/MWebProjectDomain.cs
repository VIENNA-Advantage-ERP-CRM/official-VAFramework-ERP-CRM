/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWebProjectDomain
 * Purpose        : Web Project Domain
 * Class Used     : X_CM_WebProject_Domain
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
    public class MWebProjectDomain : X_CM_WebProject_Domain
    {
        /**	serialVersionUID	*/
       // private static long serialVersionUID = 5134789895039452551L;

        /** Logger */
        private static VLogger _log = VLogger.GetVLogger(typeof(MContainer).FullName);//.class);

        /// <summary>
        /// Web Project Domain Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_WebProject_Domain_ID">id</param>
        /// <param name="trxName">trx</param>
        public MWebProjectDomain(Ctx ctx, int CM_WebProject_Domain_ID, Trx trxName)
            : base(ctx, CM_WebProject_Domain_ID, trxName)
        {

        }	//	MWebProjectDomain

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MWebProjectDomain(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MWebProjectDomain(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }

        /// <summary>
        /// Get WebProjectDomain by Name
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ServerName">server</param>
        /// <param name="trxName">trx</param>
        /// <returns>container element</returns>
        public static MWebProjectDomain Get(Ctx ctx, String ServerName, Trx trxName)
        {
            MWebProjectDomain thisWebProjectDomain = null;
            String sql = "SELECT * FROM CM_WebProject_Domain WHERE lower(FQDN) LIKE @param ORDER by CM_WebProject_Domain_ID DESC";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, trxName);
                //pstmt.setString(1, ServerName);
                param[0] = new SqlParameter("@param", ServerName);
                idr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (idr.Read())
                {
                    thisWebProjectDomain = (new MWebProjectDomain(ctx, idr, trxName));
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
            return thisWebProjectDomain;
        }


    }

}

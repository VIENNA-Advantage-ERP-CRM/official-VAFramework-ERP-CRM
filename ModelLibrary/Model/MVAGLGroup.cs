﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Data;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    /// <summary>
    /// GL Category
    /// </summary>
    public class MVAGLGroup : X_VAGL_Group
    {
        public static MVAGLGroup Get(Ctx ctx, int VAGL_Group_ID)
        {
            int key = VAGL_Group_ID;
            MVAGLGroup retValue = (MVAGLGroup)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAGLGroup(ctx, VAGL_Group_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache[key] = retValue;
            return retValue;
        }	//	get


        /// <summary>
        /// Get Default Category
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CategoryType">optional CategoryType (ignored, if not exists)</param>
        /// <returns>GL Category or null</returns>
        public static MVAGLGroup GetDefault(Ctx ctx, String CategoryType)
        {
            MVAGLGroup retValue = null;
            String sql = "SELECT * FROM VAGL_Group "
                + "WHERE VAF_Client_ID=@clientid AND IsDefault='Y'";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@clientid", ctx.GetVAF_Client_ID());
                DataSet rs = DataBase.DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in rs.Tables[0].Rows)
                {
                    MVAGLGroup temp = new MVAGLGroup(ctx, dr, null);
                    if (CategoryType != null && CategoryType.Equals(temp.GetCategoryType()))
                    {
                        retValue = temp;
                        break;
                    }
                    if (retValue == null)
                        retValue = temp;
                }
                rs.Dispose();
            }
            catch (Exception e)
            {
                
                s_log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }	//	getDefault

        /// <summary>
        /// Get Default System Category
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>categoty</returns>
        public static MVAGLGroup GetDefaultSystem(Ctx ctx)
        {
            MVAGLGroup retValue = GetDefault(ctx, CATEGORYTYPE_SystemGenerated);
            if (retValue == null
                || !retValue.GetCategoryType().Equals(CATEGORYTYPE_SystemGenerated))
            {
                retValue = new MVAGLGroup(ctx, 0, null);
                retValue.SetName("Default System");
                retValue.SetCategoryType(CATEGORYTYPE_SystemGenerated);
                retValue.SetIsDefault(true);
                if (!retValue.Save())
                    throw new Exception("Could not save default system GL Category");
            }
            return retValue;
        }	//	getDefaultSystem

        /**	Logger						*/
            private static VLogger s_log = VLogger.GetVLogger(typeof(MVAGLGroup).FullName);
        /**	Cache						*/
        private static CCache<int, MVAGLGroup> s_cache = new CCache<int, MVAGLGroup>("VAGL_Group", 5);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAGL_Group_ID"></param>
        /// <param name="trxName"></param>
        public MVAGLGroup(Ctx ctx, int VAGL_Group_ID, Trx trxName)
            : base(ctx, VAGL_Group_ID, trxName)
        {
            
            if (VAGL_Group_ID == 0)
            {
                //	setName (null);
                SetCategoryType(CATEGORYTYPE_Manual);
                SetIsDefault(false);
            }
        }	//	MVAGLGroup

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MVAGLGroup(Ctx ctx, DataRow  rs, Trx trxName) : base(ctx, rs, trxName)
        {
            
        }	//	MVAGLGroup
    }
}
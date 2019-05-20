using System;
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
    public class MGLCategory : X_GL_Category
    {
        public static MGLCategory Get(Ctx ctx, int GL_Category_ID)
        {
            int key = GL_Category_ID;
            MGLCategory retValue = (MGLCategory)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MGLCategory(ctx, GL_Category_ID, null);
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
        public static MGLCategory GetDefault(Ctx ctx, String CategoryType)
        {
            MGLCategory retValue = null;
            String sql = "SELECT * FROM GL_Category "
                + "WHERE AD_Client_ID=@clientid AND IsDefault='Y'";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@clientid", ctx.GetAD_Client_ID());
                DataSet rs = DataBase.DB.ExecuteDataset(sql, param);
                foreach(DataRow dr in rs.Tables[0].Rows)
                {
                    MGLCategory temp = new MGLCategory(ctx, dr, null);
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
        public static MGLCategory GetDefaultSystem(Ctx ctx)
        {
            MGLCategory retValue = GetDefault(ctx, CATEGORYTYPE_SystemGenerated);
            if (retValue == null
                || !retValue.GetCategoryType().Equals(CATEGORYTYPE_SystemGenerated))
            {
                retValue = new MGLCategory(ctx, 0, null);
                retValue.SetName("Default System");
                retValue.SetCategoryType(CATEGORYTYPE_SystemGenerated);
                retValue.SetIsDefault(true);
                if (!retValue.Save())
                    throw new Exception("Could not save default system GL Category");
            }
            return retValue;
        }	//	getDefaultSystem

        /**	Logger						*/
            private static VLogger s_log = VLogger.GetVLogger(typeof(MGLCategory).FullName);
        /**	Cache						*/
        private static CCache<int, MGLCategory> s_cache = new CCache<int, MGLCategory>("GL_Category", 5);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="GL_Category_ID"></param>
        /// <param name="trxName"></param>
        public MGLCategory(Ctx ctx, int GL_Category_ID, Trx trxName)
            : base(ctx, GL_Category_ID, trxName)
        {
            
            if (GL_Category_ID == 0)
            {
                //	setName (null);
                SetCategoryType(CATEGORYTYPE_Manual);
                SetIsDefault(false);
            }
        }	//	MGLCategory

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MGLCategory(Ctx ctx, DataRow  rs, Trx trxName) : base(ctx, rs, trxName)
        {
            
        }	//	MGLCategory
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Controller
{
    public class GridTabPanelVO
    {

        /** Context - replicated    */
        private Ctx ctx;
        /** Window No - replicated  */
        public int windowNo;
        /**	Tab	ID			*/
        public int AD_Tab_ID;
        /**          */
        public string Name;
        /**       */
        public int SeqNo;
        /**       */
        public string IconPath;
        /***      **/
        public bool IsDefault;
        /**         */
        public string Classname;
        /**       ***/
        public int AD_TabPanel_ID;

        /**       ***/
        public string ExtraInfo;




        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="newCtx">context</param>
        /// <param name="windowNo">window number</param>
        private GridTabPanelVO(Ctx newCtx, int windowNm)
        {
            ctx = newCtx;
            windowNo = windowNm;
        }   //  MTabVO

        public GridTabPanelVO(Ctx newCtx, int windowNm, int AD_Tab_ID)
        {
            ctx = newCtx;
            windowNo = windowNm;
            this.AD_Tab_ID = AD_Tab_ID;
        }


        /// <summary>
        ///Return the SQL statement used for the MTabVO.create
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        public static String GetSQL(Ctx ctx)
        {
            //  View only returns IsActive='Y'
            String sql = "SELECT panel.classname, panel.Name, panel.iconpath, panel.isdefault, panel.seqno, panel.ad_tabpanel_id, panel.ad_tab_id, panel.ExtraInfo FROM AD_TabPanel panel WHERE panel.AD_Tab_ID =@tabID AND panel.IsActive='Y'";
            if (!Env.IsBaseLanguage(ctx, "AD_Window"))
            {
                sql = "SELECT panel.classname, trl.Name, panel.iconpath, panel.isdefault, panel.seqno, panel.ad_tabpanel_id, panel.ad_tab_id, panel.ExtraInfo FROM AD_TabPanel panel JOIN AD_TabPanel_trl  trl ON panel.ad_tabpanel_id=trl.ad_tabpanel_id "
                    + " WHERE panel.AD_Tab_ID =@tabID AND trl.AD_Language='" + Env.GetAD_Language(ctx) + "'";
            }
            sql += " ORDER BY panel.SeqNo, panel.ad_tabpanel_id asc";
            return sql;
        }


        /// <summary>
        ///  Create Field Value Object
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Tab_ID">Tab Id</param>
        /// <param name="AD_TabPanel_ID">Tab Panel ID</param>
        /// <param name="dr">datarow</param>
        /// <returns>object of this Class</returns>
        public static GridTabPanelVO Create(Ctx ctx, int windowNo, int AD_Tab_ID, IDataReader dr)
        {
            GridTabPanelVO vo = new GridTabPanelVO(ctx, windowNo, AD_Tab_ID);
            try
            {
                vo.AD_Tab_ID = Convert.ToInt32(dr["AD_Tab_ID"]);
                vo.AD_TabPanel_ID = Convert.ToInt32(dr["AD_TabPanel_ID"]);
                vo.Classname = dr["ClassName"].ToString();
                vo.IconPath = dr["IconPath"].ToString();
                vo.IsDefault = dr["IsDefault"].Equals("Y") ? true : false;
                vo.Name = dr["Name"].ToString();
                vo.SeqNo = Convert.ToInt32(dr["Seqno"]);
                vo.ExtraInfo = dr["ExtraInfo"].ToString();
            }
            catch (Exception ex)
            {
                VLogger.Get().Log(Level.SEVERE, "Exception while getting Tab Pabel info for Tab =" + AD_Tab_ID, ex);
                return null;
            }
            return vo;
        }


        public GridTabPanelVO Clone(Ctx newCtx, int windowNo)
        {
            GridTabPanelVO clone = new GridTabPanelVO(newCtx, windowNo, AD_Tab_ID);
            clone.AD_TabPanel_ID = AD_TabPanel_ID;
            clone.Classname = Classname;
            clone.IconPath = IconPath;
            clone.IsDefault = IsDefault;
            clone.Name = Name;
            clone.SeqNo = SeqNo;
            clone.ExtraInfo = ExtraInfo;
            return clone;
        }
    }
}

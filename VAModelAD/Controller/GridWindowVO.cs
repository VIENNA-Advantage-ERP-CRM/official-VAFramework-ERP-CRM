/*
 *  Model Window Value Object
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.Utility;

namespace VAdvantage.Controller
{
    public class GridWindowVO
    {
        /// <summary>
        ///Create Window Value Object
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        /// <param name="AD_Window_ID"></param>
        /// <returns></returns>
        public static GridWindowVO Create(Context ctx, int windowNo, int AD_Window_ID)
        {
            return Create(ctx, windowNo, AD_Window_ID, 0);
        }   //  create

        /**************************************************************************
         *  Private Constructor
         *  @param Context context
         *  @param windowNo window no
         */
        private GridWindowVO(Ctx newCtx, int windowNm)
        {
            ctx = newCtx;
            windowNo = windowNm;
        }   //  MWindowVO

        //static long serialVersionUID = 3802628212531678981L;

        /** Properties      */
        private Ctx ctx;
        /** Window Number	*/
        public int windowNo;

        /** Window				*/
        public int AD_Window_ID = 0;
        /** Name				*/
        public String Name = "";
        /** Display Name  */
        public String DisplayName = "";

        /** Description			*/
        public String Description = "";
        /** Help				*/
        public String Help = "";
        /** Window Type			*/
        public String WindowType = "";
        /** Image				*/
        public int VAF_Image_ID = 0;
        /** Color				*/
        public int VAF_Colour_ID = 0;
        /** Read Write			*/
        public String IsReadWrite = null;
        /** Window Width		*/
        public int WinWidth = 0;
        /** Window Height		*/
        public int WinHeight = 0;
        /** Sales Order Trx		*/
        public bool IsSOTrx = false;

        /** Tabs contains MTabVO elements   */
        private List<GridTabVO> Tabs = null;
        /** Base Table		*/
        public int VAF_TableView_ID = 0;

        public bool hasPanel = false;

        /** Qyery				*/
        public const String WINDOWTYPE_QUERY = "Q";
        /** Transaction			*/
        public const String WINDOWTYPE_TRX = "T";
        /** Maintenance			*/
        public const String WINDOWTYPE_MMAINTAIN = "M";

        public bool IsAppointment = false;
        public bool IsTask = false;
        public bool IsEmail = false;
        public bool IsLetter = false;
        public bool IsSms = false;
        public bool IsFaxEmail = false;
        public bool IsChat = true;
        public bool IsAttachment = true;
        public bool IsHistory = true;
        public bool IsCheckRequest = true;
        public bool IsWorkFlow = true;
        public bool IsCopyReocrd = true;
        public bool IsSubscribedRecord = true;
        public bool IsZoomAcross = true;
        public bool IsCreatedDocument = true;
        public bool IsUploadedDocument = true;
        public bool IsViewDocument = true;
        public bool IsAttachDocumentFrom = true;
        public bool IsPrivateRecordLock = true;
        public bool IsImportMap = true;
        public bool IsMarkToExport = true;
        public bool IsArchive = true;
        public bool IsAttachmail = true;
        public bool IsRoleCenterView = true;
        public string FontName = "";
        public string ImageUrl = "";
        public bool IsCompositeView = true;


        public List<GridTabVO> GetTabs()
        {
            return Tabs;
        }

        public Ctx GetCtx()
        {
            return ctx;
        }

        /// <summary>
        ///Create Window Value Object
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="VAF_MenuConfig_ID"></param>
        /// <returns></returns>
        public static GridWindowVO Create(Ctx ctx, int windowNo, int AD_Window_ID, int VAF_MenuConfig_ID)
        {
            VLogger.Get().Config("#" + windowNo
                + " - AD_Window_ID=" + AD_Window_ID + "; VAF_MenuConfig_ID=" + VAF_MenuConfig_ID);
            GridWindowVO vo = new GridWindowVO(ctx, windowNo);
            vo.AD_Window_ID = AD_Window_ID;
            IDataReader dr = null;
            //  Get Window_ID if required	- (used by HTML UI)
            if (vo.AD_Window_ID == 0 && VAF_MenuConfig_ID != 0)
            {
                String sql0 = "SELECT AD_Window_ID, IsSOTrx, IsReadOnly FROM VAF_MenuConfig "
                    + "WHERE VAF_MenuConfig_ID=" + VAF_MenuConfig_ID.ToString() + " AND Action='W'";
                try
                {
                    dr = DataBase.DB.ExecuteReader(sql0, null);
                    if (dr.Read())
                    {
                        vo.AD_Window_ID = Utility.Util.GetValueOfInt(dr[0]);
                        String IsSOTrx = dr[1].ToString();
                        ctx.SetContext(windowNo, "IsSOTrx", (IsSOTrx != "" && IsSOTrx.Equals("Y")));
                        //
                        String IsReadOnly = dr[2].ToString();
                        if (IsReadOnly != "" && IsReadOnly.Equals("Y"))
                            vo.IsReadWrite = "Y";
                        else
                            vo.IsReadWrite = "N";
                    }
                    dr.Close();
                    dr = null;

                }
                catch (System.Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    VLogger.Get().Log(Level.SEVERE, "Menu", e);
                    return null;
                }
                VLogger.Get().Config("AD_Window_ID=" + vo.AD_Window_ID);
            }

            //  --  Get Window

            int VAF_Role_ID = vo.ctx.GetVAF_Role_ID();

            StringBuilder sql01 = new StringBuilder("SELECT Name,Description,Help,WindowType, "
             + "VAF_Colour_ID,VAF_Image_ID, IsReadWrite, WinHeight,WinWidth, "
             + "IsSOTrx, AD_UserDef_Win_ID,IsAppointment,IsTask,IsEmail,IsLetter,IsSms,IsFaxEmail,Name2, "
             + "ISCHAT, ISATTACHMENT,ISHISTORY,ISCHECKREQUEST,ISCOPYRECORD,ISSUBSCRIBERECORD,ISZOOMACROSS,ISCREATEDOCUMENT,ISUPLOADDOCUMENT,ISVIEWDOCUMENT,IsAttachDocumentFrom, "
             + " ISIMPORTMAP,ISMARKTOEXPORT,ISARCHIVE,ISATTACHEMAIL,ISROLECENTERVIEW , FontName, ImageUrl, IsCompositeView ");

            if (Utility.Env.IsBaseLanguage(vo.ctx, "AD_Window"))
            {
                sql01.Append("FROM AD_Window_v WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
                sql01.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            }


            else
            {
                sql01.Append("FROM AD_Window_vt w WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
                sql01.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
                sql01.Append(" AND VAF_Language='")
                .Append(Utility.Env.GetVAF_Language(vo.ctx)).Append("'");
            }



            // Commented BY Karan 21 Dec 2018.. To imrove performance
            //StringBuilder sql = new StringBuilder("SELECT Name,Description,Help,WindowType, "
            //    + "VAF_Colour_ID,VAF_Image_ID, IsReadWrite, WinHeight,WinWidth, "
            //    + "IsSOTrx, AD_UserDef_Win_ID,IsAppointment,IsTask,IsEmail,IsLetter,IsSms,IsFaxEmail,Name2 ");

            //if (Utility.Env.IsBaseLanguage(vo.ctx, "AD_Window"))
            //{
            //    sql.Append("FROM AD_Window_v WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
            //    sql.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            //}


            //else
            //{
            //    sql.Append("FROM AD_Window_vt w WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
            //    sql.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            //    sql.Append(" AND VAF_Language='")
            //    .Append(Utility.Env.GetVAF_Language(vo.ctx)).Append("'");
            //}

            ////Without Name2 Field
            //StringBuilder sql2 = new StringBuilder("SELECT Name,Description,Help,WindowType, "
            //    + "VAF_Colour_ID,VAF_Image_ID, IsReadWrite, WinHeight,WinWidth, "
            //    + "IsSOTrx, AD_UserDef_Win_ID, IsAppointment,IsTask,IsEmail,IsLetter,IsSms ");

            //if (Utility.Env.IsBaseLanguage(vo.ctx, "AD_Window"))
            //{
            //    sql2.Append("FROM AD_Window_v WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
            //    sql2.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            //}

            //else
            //{
            //    sql2.Append("FROM AD_Window_vt w WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
            //    sql2.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            //    sql2.Append(" AND VAF_Language='")
            //    .Append(Utility.Env.GetVAF_Language(vo.ctx)).Append("'");
            //}


            ////Default Query [ Window + Web]
            //StringBuilder sql3 = new StringBuilder("SELECT Name,Description,Help,WindowType, "
            //   + "VAF_Colour_ID,VAF_Image_ID, IsReadWrite, WinHeight,WinWidth, "
            //   + "IsSOTrx, AD_UserDef_Win_ID ");

            //if (Utility.Env.IsBaseLanguage(vo.ctx, "AD_Window"))
            //{
            //    sql3.Append("FROM AD_Window_v WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
            //    sql3.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            //}

            //else
            //{
            //    sql3.Append("FROM AD_Window_vt w WHERE AD_Window_ID=" + vo.AD_Window_ID.ToString());
            //    sql3.Append(" AND VAF_Role_ID=" + VAF_Role_ID);
            //    sql3.Append(" AND VAF_Language='")
            //    .Append(Utility.Env.GetVAF_Language(vo.ctx)).Append("'");
            //}

            //int VAF_Client_ID = vo.ctx.getVAF_Client_ID();

            int AD_UserDef_Win_ID = 0;
            //IDataReader dr = null;
            try
            {
                //	create statement

                //try
                //{
                dr = DataBase.DB.ExecuteReader(sql01.ToString(), null);
                //}
                //catch
                //{
                //    try
                //    {
                //        dr = DataBase.DB.ExecuteReader(sql.ToString(), null);
                //    }
                //    catch
                //    {
                //        try
                //        {
                //            dr = DataBase.DB.ExecuteReader(sql2.ToString(), null);
                //        }
                //        catch
                //        {
                //            dr = DataBase.DB.ExecuteReader(sql3.ToString(), null);
                //        }
                //    }
                //}
                // 	get data

                if (dr.Read())
                {
                    vo.Name = dr[0].ToString();
                    vo.Description = dr[1].ToString();
                    if (vo.Description == null)
                        vo.Description = "";
                    vo.Help = dr[2].ToString();
                    if (vo.Help == null)
                        vo.Help = "";
                    vo.WindowType = dr[3].ToString();
                    //
                    vo.VAF_Colour_ID = Utility.Util.GetValueOfInt(dr[4]);
                    vo.VAF_Image_ID = Utility.Util.GetValueOfInt(dr[5]);
                    vo.IsReadWrite = dr[6].ToString();
                    //
                    vo.WinHeight = Utility.Util.GetValueOfInt(dr[7]);
                    vo.WinWidth = Utility.Util.GetValueOfInt(dr[8]);
                    //
                    vo.IsSOTrx = "Y".Equals(dr[9].ToString());
                    AD_UserDef_Win_ID = Utility.Util.GetValueOfInt(dr[10]);

                    if (dr.FieldCount > 11)
                    {
                        vo.IsAppointment = "Y".Equals(dr[11].ToString());
                        vo.IsTask = "Y".Equals(dr[12].ToString());
                        vo.IsEmail = "Y".Equals(dr[13].ToString());
                        vo.IsLetter = "Y".Equals(dr[14].ToString());
                        vo.IsSms = "Y".Equals(dr[15].ToString());
                        if (dr.FieldCount > 16)
                        {
                            vo.IsFaxEmail = "Y".Equals(dr[16].ToString());
                            vo.DisplayName = dr[17].ToString();
                        }
                        if (dr.FieldCount > 18)
                        {
                            vo.IsChat = !("N".Equals(dr[18].ToString()));
                            vo.IsAttachment = !("N".Equals(dr[19].ToString()));
                            vo.IsHistory = !("N".Equals(dr[20].ToString()));
                            vo.IsCheckRequest = !("N".Equals(dr[21].ToString()));

                            vo.IsCopyReocrd = !("N".Equals(dr[22].ToString()));
                            vo.IsSubscribedRecord = !("N".Equals(dr[23].ToString()));
                            vo.IsZoomAcross = !("N".Equals(dr[24].ToString()));
                            vo.IsCreatedDocument = !("N".Equals(dr[25].ToString()));
                            vo.IsUploadedDocument = !("N".Equals(dr[26].ToString()));
                            vo.IsViewDocument = !("N".Equals(dr[27].ToString()));
                            vo.IsAttachDocumentFrom = !("N".Equals(dr[28].ToString()));

                            vo.IsImportMap = !("N".Equals(dr[29].ToString()));
                            vo.IsMarkToExport = !("N".Equals(dr[30].ToString()));
                            vo.IsArchive = !("N".Equals(dr[31].ToString()));
                            vo.IsAttachmail = !("N".Equals(dr[32].ToString()));
                            vo.IsRoleCenterView = !("N".Equals(dr[33].ToString()));
                            vo.FontName= dr[34].ToString();
                            vo.ImageUrl = dr[35].ToString();
                            if (vo.ImageUrl != "" && vo.ImageUrl.Contains("/"))
                            {
                                vo.ImageUrl = vo.ImageUrl.Substring(vo.ImageUrl.LastIndexOf("/") + 1);
                            }
                            vo.IsCompositeView = dr[36].ToString() == "Y";
                        }
                    }
                }
                else
                    vo = null;
                dr.Close();
                dr = null;
            }
            catch (System.Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, sql01.ToString(), ex);
                return null;
            }
            if (dr != null)
            {
                dr.Close();
                dr = null;
            }
            //	Not found
            if (vo == null)
            {
                VLogger.Get().Log(Level.SEVERE, "No Window - AD_Window_ID=" + AD_Window_ID
                    + ", VAF_Role_ID=" + VAF_Role_ID + " - " + sql01);
                VLogger.Get().SaveError("AccessTableNoView", "(Not found)");
                return null;
            }
            //	Read Write
            if (vo.IsReadWrite == null)
            {
                VLogger.Get().SaveError("AccessTableNoView", "(found)");
                return null;
            }

            //  Create Tabs
            CreateTabs(vo, AD_UserDef_Win_ID);
            if (vo.Tabs == null || vo.Tabs.Count == 0)
                return null;

            return vo;
        }   //  create


        /// <summary>
        ///Create Window Value Object
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="VAF_MenuConfig_ID"></param>
        /// <returns></returns>
        public static GridWindowVO Create(Ctx ctx, int windowNo, int AD_Window_ID)
        {
            VLogger.Get().Config("#" + windowNo
                + " - AD_Window_ID=" + AD_Window_ID);
            GridWindowVO vo = new GridWindowVO(ctx, windowNo);
            vo.AD_Window_ID = AD_Window_ID;
            IDataReader dr = null;
            //  --  Get Window

            //int VAF_Role_ID = vo.ctx.GetVAF_Role_ID();
            bool isBase = false;

            if (Utility.Env.IsBaseLanguage(vo.ctx, "AD_Window"))
            {
                isBase = true;
            }



            StringBuilder sql = new StringBuilder("SELECT ");


            //w.ad_window_id, u.vaf_client_id, u.VAF_Role_id AS userdef_role_id, u.ad_user_id,u.ad_userdef_win_id, u.customizationname, COALESCE(u.name, w.name) AS name,")
            // .Append("COALESCE(u.description, w.description) AS description, COALESCE(u.help, w.help) AS help, w.windowtype, w.ad_color_id, w.VAF_Image_id, COALESCE(u.winheight, w.winheight) AS winheight,")
            // .Append("COALESCE(u.winwidth, w.winwidth) AS winwidth, w.VAF_ContextScope_id,'Y' AS IsSOTrx,'Y' AS IsReadWrite, w.isdefault ");


            sql.Append(" w.AD_Window_ID                          AS AD_Window_ID      , ")
               .Append(" u.VAF_Client_ID                            AS VAF_Client_ID      , ")
               .Append(" u.VAF_Role_ID                            AS userdef_role_id   , ")
               .Append(" u.AD_user_ID                            AS AD_User_ID        , ")
               .Append(" u.AD_UserDef_Win_ID                     AS AD_UserDef_Win_ID , ")
               .Append(" u.CustomizationName                     AS CustomizationName , ");
            if (isBase)
            {

                sql.Append(" COALESCE(u.name, w.name)                AS Name              , ")
                  .Append(" COALESCE(u.description, w.description)  AS Description       , ")
                  .Append(" COALESCE(u.help, w.help)                AS Help              , ");
            }
            else
            {
                sql.Append(" COALESCE(u.Name,trl.Name)               AS Name             , ")
                    .Append(" COALESCE(u.Description,trl.Description) AS Description      , ")
                    .Append(" COALESCE(u.Help,trl.Help)               AS Help             , ");
            }

            sql.Append(" w.WindowType                            AS WindowType        , ")
            .Append(" w.VAF_Colour_ID                           AS VAF_Colour_ID       , ")
            .Append(" w.VAF_Image_ID                           AS VAF_Image_ID       , ")
            .Append(" COALESCE(u.WinHeight,w.WinHeight)       AS WinHeight         , ")
            .Append(" COALESCE(u.WinWidth,w.WinWidth)         AS WinWidth          , ")
            .Append(" w.VAF_ContextScope_ID                         AS VAF_ContextScope_ID     , ")
            .Append(" COALESCE((SELECT IsSOTrx FROM VAF_ContextScope ctx WHERE w.VAF_ContextScope_ID=ctx.VAF_ContextScope_ID ),'Y')        AS IsSOTrx    , ")
            .Append(" a.VAF_Role_ID                            AS VAF_Role_ID        , ")
            .Append(" a.IsReadWrite                           AS IsReadWrite       , ")
            .Append(" w.IsDefault                             AS IsDefault           ");


            if (isBase)
            {
                sql.Append(" FROM AD_Window w ");
            }
            else
            {
                sql.Append(" FROM AD_Window w ")
               .Append("  INNER JOIN AD_Window_Trl trl ON w.AD_Window_id = trl.AD_Window_ID ");
            }

            sql.Append(" INNER JOIN AD_Window_Access a ON w.AD_Window_ID = a.AD_Window_ID ")
            .Append(" LEFT OUTER JOIN AD_userdef_win u ON w.AD_Window_ID = u.AD_Window_ID ")
             .Append(" WHERE w.AD_Window_ID=" + vo.AD_Window_ID);

            if (!isBase)
            {
                sql.Append(" AND trl.VAF_Language='")
             .Append(Utility.Env.GetVAF_Language(vo.ctx)).Append("'");
            }


            //int VAF_Client_ID = vo.ctx.getVAF_Client_ID();

            int AD_UserDef_Win_ID = 0;
            //IDataReader dr = null;
            try
            {
                //	create statement
                dr = DataBase.DB.ExecuteReader(sql.ToString(), null);
                // 	get data

                if (dr.Read())
                {
                    vo.Name = dr["Name"].ToString();
                    vo.Description = dr["Description"].ToString();
                    if (vo.Description == null)
                        vo.Description = "";
                    vo.Help = dr["Help"].ToString();
                    if (vo.Help == null)
                        vo.Help = "";
                    vo.WindowType = dr["WindowType"].ToString();
                    //
                    vo.VAF_Colour_ID = Utility.Util.GetValueOfInt(dr["VAF_Colour_ID"]);
                    vo.VAF_Image_ID = Utility.Util.GetValueOfInt(dr["VAF_Image_ID"]);
                    vo.IsReadWrite = dr["IsReadWrite"].ToString();
                    //
                    vo.WinHeight = Utility.Util.GetValueOfInt(dr["WinHeight"]);
                    vo.WinWidth = Utility.Util.GetValueOfInt(dr["WinWidth"]);
                    //
                    vo.IsSOTrx = "Y".Equals(dr["IsSOTrx"].ToString());
                    AD_UserDef_Win_ID = Utility.Util.GetValueOfInt(dr["AD_UserDef_Win_ID"]);
                }
                else
                    vo = null;
                dr.Close();
                dr = null;
            }
            catch (System.Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, sql.ToString(), ex);
                return null;
            }
            if (dr != null)
            {
                dr.Close();
                dr = null;
            }
            //	Not found
            if (vo == null)
            {
                VLogger.Get().Log(Level.SEVERE, "No Window - AD_Window_ID=" + AD_Window_ID + " - " + sql);
                VLogger.Get().SaveError("AccessTableNoView", "(Not found)");
                return null;
            }
            //	Read Write
            if (vo.IsReadWrite == null)
            {
                VLogger.Get().SaveError("AccessTableNoView", "(found)");
                return null;
            }

            //  Create Tabs
            CreateEditorTabs(vo, AD_UserDef_Win_ID);
            if (vo.Tabs == null || vo.Tabs.Count == 0)
                return null;

            return vo;
        }


        /// <summary>
        /// Create Window Tabs
        /// </summary>
        /// <param name="mWindowVO"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        private static bool CreateEditorTabs(GridWindowVO mWindowVO, int AD_UserDef_Win_ID)
        {
            mWindowVO.Tabs = new List<GridTabVO>();

            bool isBase = false;

            if (Utility.Env.IsBaseLanguage(mWindowVO.ctx, "VAF_Tab"))
            {
                isBase = true;
            }

            StringBuilder sql = new StringBuilder(" SELECT ")
            .Append(" t.VAF_Tab_ID                                 AS VAF_Tab_ID            , ")
            .Append(" t.AD_Window_ID                              AS AD_Window_ID         , ")
            .Append(" t.VAF_TableView_ID                               AS VAF_TableView_ID          , ")
            .Append(" t.VAF_ContextScope_ID                             AS VAF_ContextScope_ID        , ")
            .Append(" uw.VAF_Role_ID                               AS UserDef_Role_ID      , ")
            .Append(" uw.AD_User_ID                               AS AD_User_ID           , ")
            .Append(" uw.AD_UserDef_Win_ID                        AS AD_UserDef_Win_ID    , ")
            .Append(" uw.CustomizationName                        AS CustomizationName    , ")
            .Append(" u.AD_UserDef_Tab_ID                         AS AD_UserDef_Tab_ID    , ");
            if (isBase)
            {

                sql.Append(" COALESCE(u.Name,t.Name)                     AS Name                 , ")
                .Append(" COALESCE(u.Description,t.Description)       AS Description          , ")
                .Append(" COALESCE(u.Help,t.Help)                     AS Help                 , ");
            }
            else
            {
                sql.Append(" COALESCE(u.Name,trl.Name)                   AS Name                 , ")
                    .Append(" COALESCE(u.Description,trl.Description)     AS Description          , ")
                    .Append(" COALESCE(u.Help,trl.Help)                   AS Help                 , ");
            }

            sql.Append(" COALESCE(u.SeqNo,t.SeqNo)                   AS SeqNo                , ")
            .Append(" COALESCE(u.IsSingleRow,t.IsSingleRow)       AS IsSingleRow          , ")
            .Append(" t.HasTree                                   AS HasTree              , ")
            .Append(" t.IsInfoTab                                 AS IsInfoTab            , ")
            .Append(" t.Referenced_Tab_ID                         AS Referenced_Tab_ID    , ")
            .Append(" tbl.ReplicationType                         AS ReplicationType      , ")
            .Append(" tbl.TableName                               AS TableName            , ")
            .Append(" tbl.AccessLevel                             AS AccessLevel          , ")
            .Append(" tbl.IsSecurityEnabled                       AS IsSecurityEnabled    , ")
            .Append(" tbl.IsDeleteable                            AS IsDeleteable         , ")
            .Append(" tbl.IsHighVolume                            AS IsHighVolume         , ")
            .Append(" tbl.IsView                                  AS IsView               , ")
            .Append(" t.IsTranslationTab                          AS IsTranslationTab     , ")
            .Append(" COALESCE(u.IsReadOnly,t.IsReadOnly)         AS IsReadOnly           , ")
            .Append(" t.VAF_Image_ID                               AS VAF_Image_ID          , ")
            .Append(" t.TabLevel                                  AS TabLevel             , ")
            .Append(" t.WhereClause                               AS WhereClause          , ")
            .Append(" t.OrderByClause                             AS OrderByClause        , ")
            .Append(" t.CommitWarning                             AS CommitWarning        , ")
            .Append(" COALESCE(u.ReadOnlyLogic,t.ReadOnlyLogic)   AS ReadOnlyLogic        , ")
            .Append(" COALESCE(u.IsDisplayed,t.IsDisplayed)       AS IsDisplayed          , ")
            .Append(" COALESCE(u.DisplayLogic,t.DisplayLogic)     AS DisplayLogic         , ")
            .Append(" t.VAF_Column_ID                              AS VAF_Column_ID         , ")
            .Append(" c.ColumnName                                AS LinkColumnName       , ")
            .Append(" t.VAF_Job_ID                             AS VAF_Job_ID        , ")
            .Append(" t.IsSortTab                                 AS IsSortTab            , ")
            .Append(" t.IsAdvancedTab                             AS IsAdvancedTab        , ")
            .Append(" COALESCE(u.IsInsertRecord,t.IsInsertRecord) AS IsInsertRecord       , ")
            .Append(" t.VAF_ColumnSortOrder_ID                     AS VAF_ColumnSortOrder_ID, ")
            .Append(" t.VAF_ColumnSortYesNo_ID                     AS VAF_ColumnSortYesNo_ID, ")
            .Append(" t.Included_Tab_ID                           AS Included_Tab_ID, ")
            .Append(" t.ShowSummaryLevelNodes As ShowSummaryLevelNodes ");
            if (isBase)
            {
                sql.Append(" FROM VAF_Tab t ");
            }
            else
            {
                sql.Append(" FROM VAF_Tab t ")
               .Append("  INNER JOIN VAF_Tab_TL trl ON t.VAF_Tab_ID = trl.VAF_Tab_ID ");
                isBase = true;
            }

            sql.Append(" INNER JOIN VAF_TableView tbl ON (t.VAF_TableView_ID = tbl.VAF_TableView_ID) ")
            .Append(" LEFT OUTER JOIN VAF_Column c ON (t.VAF_Column_ID=c.VAF_Column_ID) ")
            .Append(" LEFT OUTER JOIN AD_UserDef_Tab u ON (u.VAF_Tab_ID=t.VAF_Tab_ID) ")
            .Append(" LEFT OUTER JOIN AD_UserDef_Win uw  ON (uw.AD_UserDef_Win_ID=u.AD_UserDef_Win_ID) ")

            .Append(" WHERE t.AD_Window_ID = @windowID ");

            if (!isBase)
            {

                sql.Append(" AND  trl.VAF_Language='" + Env.GetVAF_Language(mWindowVO.ctx) + "'");
            }

            if (AD_UserDef_Win_ID != 0)
                sql.Append(" AND u.AD_UserDef_Win_ID=" + AD_UserDef_Win_ID);

            sql.Append(" ORDER BY SeqNo");



            int TabNo = 0;
            IDataReader dr = null;
            try
            {
                //	create statement
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@windowID", mWindowVO.AD_Window_ID);
                dr = DataBase.DB.ExecuteReader(sql.ToString(), param);
                bool firstTab = true;
                while (dr.Read())
                {
                    if (mWindowVO.VAF_TableView_ID == 0)
                        mWindowVO.VAF_TableView_ID = Utility.Util.GetValueOfInt(dr["VAF_TableView_ID"]);
                    //  Create TabVO
                    int onlyCurrentDays = 0;
                    if (mWindowVO.WindowType.Equals(WINDOWTYPE_TRX))
                        onlyCurrentDays = 1;
                    GridTabVO mTabVO = GridTabVO.CreateEditorTabs(mWindowVO, TabNo, dr,
                        mWindowVO.WindowType.Equals(WINDOWTYPE_QUERY),  //  isRO
                        onlyCurrentDays, AD_UserDef_Win_ID);
                    if (mTabVO == null && firstTab)
                        break;		//	don't continue if first tab is null
                    if (mTabVO != null)
                    {
                        if (!mTabVO.IsReadOnly && "N".Equals(mWindowVO.IsReadWrite))
                            mTabVO.IsReadOnly = true;
                        mWindowVO.Tabs.Add(mTabVO);
                        TabNo++;        //  must be same as mWindow.getTab(x)
                        firstTab = false;
                    }
                }
                dr.Close();
                dr = null;
                param = null;
            }
            catch (System.Data.Common.DbException e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, "createTabs", e);
                return false;
            }

            //  No Tabs
            if (TabNo == 0 || mWindowVO.Tabs.Count == 0)
            {
                VLogger.Get().Log(Level.SEVERE, "No Tabs - AD_Window_ID="
                    + mWindowVO.AD_Window_ID + " - " + sql);
                return false;
            }

            //	Put base table of window in ctx (for VDocAction)
            mWindowVO.ctx.SetContext(mWindowVO.windowNo, "BaseTable_ID", mWindowVO.VAF_TableView_ID);
            return true;
        }






        /// <summary>
        /// Create Window Tabs
        /// </summary>
        /// <param name="mWindowVO"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        private static bool CreateTabs(GridWindowVO mWindowVO, int AD_UserDef_Win_ID)
        {
            mWindowVO.Tabs = new List<GridTabVO>();

            String sql = GridTabVO.GetSQL(mWindowVO.ctx, AD_UserDef_Win_ID);
            int TabNo = 0;
            IDataReader dr = null;
            try
            {
                //	create statement
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@windowID", mWindowVO.AD_Window_ID);
                dr = DataBase.DB.ExecuteReader(sql, param);
                bool firstTab = true;
                while (dr.Read())
                {
                    if (mWindowVO.VAF_TableView_ID == 0)
                        mWindowVO.VAF_TableView_ID = Utility.Util.GetValueOfInt(dr["VAF_TableView_ID"]);
                    //  Create TabVO
                    int onlyCurrentDays = 0;
                    if (mWindowVO.WindowType.Equals(WINDOWTYPE_TRX))
                        onlyCurrentDays = 1;
                    GridTabVO mTabVO = GridTabVO.Create(mWindowVO, TabNo, dr,
                        mWindowVO.WindowType.Equals(WINDOWTYPE_QUERY),  //  isRO
                        onlyCurrentDays, AD_UserDef_Win_ID);
                    if (mTabVO == null && firstTab)
                        break;		//	don't continue if first tab is null
                    if (mTabVO != null)
                    {
                        if (!mTabVO.IsReadOnly && "N".Equals(mWindowVO.IsReadWrite))
                            mTabVO.IsReadOnly = true;
                        mWindowVO.Tabs.Add(mTabVO);
                        TabNo++;        //  must be same as mWindow.getTab(x)
                        firstTab = false;
                    }
                }
                dr.Close();
                dr = null;
                param = null;
            }
            catch (System.Data.Common.DbException e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, "createTabs", e);
                return false;
            }

            //  No Tabs
            if (TabNo == 0 || mWindowVO.Tabs.Count == 0)
            {
                VLogger.Get().Log(Level.SEVERE, "No Tabs - AD_Window_ID="
                    + mWindowVO.AD_Window_ID + " - " + sql);
                return false;
            }

            //	Put base table of window in ctx (for VDocAction)
            mWindowVO.ctx.SetContext(mWindowVO.windowNo, "BaseTable_ID", mWindowVO.VAF_TableView_ID);
            return true;
        }

        /**
         *  Set Context including contained elements
         *  @param newCtx context
         */
        public void SetCtx(Ctx newCtx)
        {
            ctx = newCtx;
            for (int i = 0; i < Tabs.Count; i++)
            {
                GridTabVO tab = Tabs[i];
                tab.SetCtx(newCtx);
            }
        }   //  setCtx

        /**
         * 	Clone
         * 	@param windowNo no
         *	@return WindowVO
         */
        public GridWindowVO Clone(Ctx ctx, int windowNo)
        {
            GridWindowVO clone = null;
            try
            {
                clone = new GridWindowVO(ctx, windowNo);
                clone.AD_Window_ID = AD_Window_ID;
                clone.Name = Name;
                clone.Description = Description;
                clone.Help = Help;
                clone.WindowType = WindowType;
                clone.VAF_Image_ID = VAF_Image_ID;
                clone.VAF_Colour_ID = VAF_Colour_ID;
                clone.IsReadWrite = IsReadWrite;
                clone.WinWidth = WinWidth;
                clone.WinHeight = WinHeight;
                clone.IsSOTrx = IsSOTrx;
                ctx.SetContext(windowNo, "IsSOTrx", clone.IsSOTrx);
                clone.VAF_TableView_ID = VAF_TableView_ID;
                ctx.SetContext(windowNo, "BaseTable_ID", clone.VAF_TableView_ID);

                clone.IsAppointment = IsAppointment;
                clone.IsTask = IsTask;
                clone.IsEmail = IsEmail;
                clone.IsLetter = IsLetter;
                clone.IsSms = IsSms;
                clone.IsFaxEmail = IsFaxEmail;
                clone.DisplayName = DisplayName;
                clone.IsChat = IsChat;
                clone.IsAttachment = IsAttachment;
                clone.IsHistory = IsHistory;
                clone.IsCheckRequest = IsCheckRequest;

                clone.IsCopyReocrd = IsCopyReocrd;
                clone.IsSubscribedRecord = IsSubscribedRecord;
                clone.IsZoomAcross = IsZoomAcross;
                clone.IsCreatedDocument = IsCreatedDocument;
                clone.IsUploadedDocument = IsUploadedDocument;
                clone.IsViewDocument = IsViewDocument;
                clone.IsAttachDocumentFrom = IsAttachDocumentFrom;

                clone.IsImportMap = IsImportMap;
                clone.IsMarkToExport = IsMarkToExport;
                clone.IsArchive = IsArchive;
                clone.IsAttachmail = IsAttachmail;
                clone.IsRoleCenterView = IsRoleCenterView;
                clone.FontName = FontName;
                clone.ImageUrl = ImageUrl;
                clone.IsCompositeView = IsCompositeView;

                //
                clone.hasPanel = hasPanel;



                clone.Tabs = new List<GridTabVO>();
                for (int i = 0; i < Tabs.Count; i++)
                {
                    GridTabVO tab = Tabs[i];
                    GridTabVO cloneTab = tab.Clone(clone.ctx, windowNo);
                    if (cloneTab == null)
                        return null;
                    clone.Tabs.Add(cloneTab);
                }
            }
            catch
            {
                clone = null;
            }
            return clone;
        }	//	clone
    }
}   //  MWindowVO


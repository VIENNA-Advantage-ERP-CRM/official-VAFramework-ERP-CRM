/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Model Tab Value Object(get and set TAb attributes) (table)
// Class Used     : GlobalVariable.cs, CommonFunctions.cs
// Created By     : Harwinder 
// Date           : 13 jan 2009
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Common;

namespace VAdvantage.Controller
{
    public class GridTabVO
    {
        /** Context - replicated    */
        private Ctx ctx;
        /** Window No - replicated  */
        public int windowNo;
        /** AD Window - replicated  */
        public int AD_Window_ID;

        /** Tab No (not AD_Tab_ID) 0.. */
        public int tabNo;

        /**	Tab	ID			*/
        public int AD_Tab_ID;
        /**	Tab	ID			*/
        public int Referenced_Tab_ID;
        /** Name			*/
        public String Name = "";
        /** Description		*/
        public String Description = "";
        /** Help			*/
        public String Help = "";
        /** Single Row		*/
        public bool IsSingleRow = false;
        /** Read Only		*/
        public bool IsReadOnly = false;
        /** Insert Record	*/
        public bool IsInsertRecord = true;
        /** Tree			*/
        public bool HasTree = false;
        /** Table			*/
        public int AD_Table_ID;
        /** Primary Parent Column   */
        public int AD_Column_ID = 0;
        /** Table Name		*/
        public String TableName;
        /** Table is View	*/
        public bool IsView = false;
        /** Table Access Level	*/
        public String AccessLevel;
        /** Security		*/
        public bool IsSecurityEnabled = false;
        /** Table Deleteable	*/
        public bool IsDeleteable = false;
        /** Table High Volume	*/
        public bool IsHighVolume = false;
        /** Process			*/
        public int AD_Process_ID = 0;
        /** Commit Warning	*/
        public String CommitWarning;
        /** Where			*/
        public String WhereClause;
        /** Order by		*/
        public String OrderByClause;
        /** Tab Read Only	*/
        public String ReadOnlyLogic;
        /** Tab Display		*/
        public String DisplayLogic;
        /** Level			*/
        public int TabLevel = 0;
        /** Image			*/
        public int AD_Image_ID = 0;
        /** Included Tab	*/
        public int Included_Tab_ID = 0;
        /** Replication Type	*/
        public String ReplicationType = "L";

        /** Sort Tab			*/
        public bool IsSortTab = false;
        /** Column Sort			*/
        public int AD_ColumnSortOrder_ID = 0;
        /** Column Displayed	*/
        public int AD_ColumnSortYesNo_ID = 0;

        /**	Only Current Days - derived	*/
        public int onlyCurrentDays = 0;

        public List<string> locationCols = new List<string>();

        /** Fields contain MFieldVO entities    */
        private List<GridFieldVO> fields = null;

        /** Fields contain MFieldVO entities    */
        private List<GridTabPanelVO> panels = null;

        /** Show Summary Level			*/
        public bool ShowSummaryLevel = false;

        /****   Has tab Panels   ***/
        public bool HasPanels = false;

        /****   Is Header Panel   ***/
        public bool IsHeaderPanel = false;

        /****   Is Header Panel not show in multi row  ***/
        public bool HPanelNotShowInMultiRow = false;

        /****   Grid Layout   ***/
        public int AD_HeaderLayout_ID = 0;

        /****   Header Height   ***/
        public decimal HeaderHeight = 0;

        ///****   Header Back Color   ***/
        public string HeaderBackColor = "";

        /****   Header Alignment   ***/
        public string HeaderAlignment = "";

        ///****   Header Name   ***/
        //public string HeaderName = "";

        ///****   Header Total Column   ***/
        //public int HeaderTotalColumn = 0;

        ///****   Header Total Row   ***/
        //public int HeaderTotalRow = 0;


        /****   Header Total Row   ***/
        public Decimal HeaderWidth = 0;

        /*** Header Padding  *******/
        public string HeaderPadding = "";

        /****   Header Items   ***/
        public List<HeaderPanelGrid> HeaderItems = null;

        public string TabPanelAlignment = "V";

        // Maintain versions on approval // for Master data Versioning
        public bool MaintainVerOnApproval = false;

        // Maintain versions on table level // for Master data Versioning
        public bool IsMaintainVersions = false;

        /** Tab Layout		*/
        public string TabLayout = "N";

        /** New Record View
         *S---> Single View
         *G---> Grid View
         *else--> Current View
         */
        public string NewRecordView = "";

        public int DefaultCardID = 0;

        public List<GridFieldVO> GetFields()
        {
            return fields;
        }


        public List<GridTabPanelVO> GetPanels()
        {
            return panels;
        }

        public Ctx GetCtx()
        {
            return ctx;
        }

        /// <summary>
        /// *	Create MTab VO
        /// </summary>
        /// <param name="wVO">Window VO Object</param>
        /// <param name="TabNo">Tab Number</param>
        /// <param name="dr">Datarow</param>
        /// <param name="isRO">Is Read Only</param>
        /// <param name="onlyCurrentDays">only current day</param>
        /// <param name="AD_UserDef_Win_ID">window id</param>
        /// <returns>this object</returns>
        public static GridTabVO Create(GridWindowVO wVO, int TabNo, IDataReader dr,
            bool isRO, int onlyCurrentDays, int AD_UserDef_Win_ID)
        {
            VLogger.Get().Config("#" + TabNo);

            GridTabVO vo = new GridTabVO(wVO.GetCtx(), wVO.windowNo);
            vo.AD_Window_ID = wVO.AD_Window_ID;
            vo.tabNo = TabNo;
            //
            if (!LoadTabDetails(vo, dr))
                return null;

            if (isRO)
            {
                VLogger.Get().Fine("Tab is ReadOnly");
                vo.IsReadOnly = true;
            }
            vo.onlyCurrentDays = onlyCurrentDays;

            //  Create Fields
            if (vo.IsSortTab)
            {
                vo.fields = new List<GridFieldVO>();	//	dummy
            }
            else
            {
                CreateFields(vo, AD_UserDef_Win_ID);
                if (vo.fields == null || vo.fields.Count == 0)
                {
                    VLogger.Get().Log(Level.SEVERE, vo.Name + ": No Fields");
                    return null;
                }

                if (vo.IsHeaderPanel)
                {
                    CreateHeaderPanel(vo);
                }

                CreateTabPanels(vo);

                CreateCardPanels(vo, wVO.GetCtx());

                if (vo.panels != null && vo.panels.Count > 0)
                {
                    vo.HasPanels = true;
                    wVO.hasPanel = true;
                }
            }
            return vo;
        }	//	create


        /// <summary>
        /// *	Create MTab VO
        /// </summary>
        /// <param name="wVO">Window VO Object</param>
        /// <param name="TabNo">Tab Number</param>
        /// <param name="dr">Datarow</param>
        /// <param name="isRO">Is Read Only</param>
        /// <param name="onlyCurrentDays">only current day</param>
        /// <param name="AD_UserDef_Win_ID">window id</param>
        /// <returns>this object</returns>
        public static GridTabVO CreateEditorTabs(GridWindowVO wVO, int TabNo, IDataReader dr,
            bool isRO, int onlyCurrentDays, int AD_UserDef_Win_ID)
        {
            VLogger.Get().Config("#" + TabNo);

            GridTabVO vo = new GridTabVO(wVO.GetCtx(), wVO.windowNo);
            vo.AD_Window_ID = wVO.AD_Window_ID;
            vo.tabNo = TabNo;
            //
            if (!LoadTabDetails(vo, dr))
                return null;

            if (isRO)
            {
                VLogger.Get().Fine("Tab is ReadOnly");
                vo.IsReadOnly = true;
            }
            vo.onlyCurrentDays = onlyCurrentDays;

            //  Create Fields
            if (vo.IsSortTab)
            {
                vo.fields = new List<GridFieldVO>();	//	dummy
            }
            else
            {
                CreateEditorFields(vo, AD_UserDef_Win_ID);
                if (vo.fields == null || vo.fields.Count == 0)
                {
                    VLogger.Get().Log(Level.SEVERE, vo.Name + ": No Fields");
                    return null;
                }
            }
            return vo;
        }	//	create




        /// <summary>
        /// *	Create MTab VO
        /// </summary>
        /// <param name="wVO">Window VO Object</param>
        /// <param name="TabNo">Tab Number</param>
        /// <param name="dr">Datarow</param>
        /// <param name="isRO">Is Read Only</param>
        /// <param name="onlyCurrentDays">only current day</param>
        /// <param name="AD_UserDef_Win_ID">window id</param>
        /// <returns>this object</returns>
        public static GridTabVO Create(Ctx ctx, int windowNo, int AD_Window_Id, int TabNo, IDataReader dr,
            bool isRO, int onlyCurrentDays, int AD_UserDef_Win_ID)
        {
            VLogger.Get().Config("#" + TabNo);

            GridTabVO vo = new GridTabVO(ctx, windowNo);
            vo.AD_Window_ID = AD_Window_Id;
            vo.tabNo = TabNo;
            //
            if (!LoadTabDetails(vo, dr))
                return null;

            if (isRO)
            {
                VLogger.Get().Fine("Tab is ReadOnly");
                vo.IsReadOnly = true;
            }
            vo.onlyCurrentDays = onlyCurrentDays;

            //  Create Fields
            if (vo.IsSortTab)
            {
                vo.fields = new List<GridFieldVO>();	//	dummy
            }
            else
            {
                CreateFields(vo, AD_UserDef_Win_ID);
                if (vo.fields == null || vo.fields.Count == 0)
                {
                    VLogger.Get().Log(Level.SEVERE, vo.Name + ": No Fields");
                    return null;
                }
            }
            return vo;
        }	//	create

        /// <summary>
        /// Load Tab Details from dr into vo
        /// </summary>
        /// <param name="vo">GridTabVO object</param>
        /// <param name="dr">Datarow</param>
        /// <returns>true if successful</returns>
        private static bool LoadTabDetails(GridTabVO vo, IDataReader dr)
        {
            MRole role = MRole.GetDefault(vo.ctx, false);
            bool showTrl = "Y".Equals(vo.ctx.GetContext("#ShowTrl")) || DataBase.GlobalVariable.IsVisualEditor;
            bool showAcct = "Y".Equals(vo.ctx.GetContext("#ShowAcct")) || DataBase.GlobalVariable.IsVisualEditor;
            bool showAdvanced = "Y".Equals(vo.ctx.GetContext("#ShowAdvanced")) || DataBase.GlobalVariable.IsVisualEditor;
            //	VLogger.get().warning("ShowTrl=" + showTrl + ", showAcct=" + showAcct);
            try
            {
                vo.AD_Tab_ID = Utility.Util.GetValueOfInt(dr["AD_Tab_ID"]);
                vo.Referenced_Tab_ID = Utility.Util.GetValueOfInt(dr["Referenced_Tab_ID"]);
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "AD_Tab_ID", vo.AD_Tab_ID.ToString());
                vo.Name = Utility.Util.GetValueOfString(dr["Name"]);
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "Name", vo.Name);

                //	Translation Tab	**
                if (Utility.Util.GetValueOfString(dr["IsTranslationTab"]).Equals("Y"))
                {
                    //	Document Translation
                    vo.TableName = Utility.Util.GetValueOfString(dr["TableName"]);
                    if (!Env.IsBaseTranslation(vo.TableName)	//	C_UOM, ...
                        && !Common.Common.IsMultiLingualDocument(vo.ctx))
                        showTrl = false;
                    if (!showTrl)
                    {
                        VLogger.Get().Config("TrlTab Not displayed - AD_Tab_ID="
                            + vo.AD_Tab_ID + "=" + vo.Name + ", Table=" + vo.TableName
                            + ", BaseTrl=" + Env.IsBaseTranslation(vo.TableName)
                            + ", MultiLingual=" + Common.Common.IsMultiLingualDocument(vo.ctx));
                        return false;
                    }
                }
                //	Advanced Tab	**
                if (!showAdvanced && Utility.Util.GetValueOfString(dr["IsAdvancedTab"]).Equals("Y"))
                {
                    VLogger.Get().Config("AdvancedTab Not displayed - AD_Tab_ID="
                        + vo.AD_Tab_ID + " " + vo.Name);
                    return false;
                }
                //	Accounting Info Tab	**
                if (!showAcct && Utility.Util.GetValueOfString(dr["IsInfoTab"]).Equals("Y"))
                {
                    VLogger.Get().Fine("AcctTab Not displayed - AD_Tab_ID="
                        + vo.AD_Tab_ID + " " + vo.Name);
                    return false;
                }

                //	DisplayLogic
                vo.DisplayLogic = Utility.Util.GetValueOfString(dr["DisplayLogic"]);

                //	Access Level
                vo.AccessLevel = Utility.Util.GetValueOfString(dr["AccessLevel"]);
                if (!role.CanView(vo.ctx, vo.AccessLevel) && !DataBase.GlobalVariable.IsVisualEditor)	//	No Access
                {
                    VLogger.Get().Fine("No Role Access - AD_Tab_ID=" + vo.AD_Tab_ID + " " + vo.Name);
                    return false;
                }	//	Used by MField.getDefault
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "AccessLevel", vo.AccessLevel);

                //	Table Access
                vo.AD_Table_ID = Utility.Util.GetValueOfInt(dr["AD_Table_ID"]);
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "AD_Table_ID", vo.AD_Table_ID.ToString());
                if (!role.IsTableAccess(vo.AD_Table_ID, true) && !DataBase.GlobalVariable.IsVisualEditor)
                {
                    VLogger.Get().Config("No Table Access - AD_Tab_ID="
                        + vo.AD_Tab_ID + " " + vo.Name);
                    return false;
                }

                if (role.IsTableReadOnly(vo.AD_Table_ID) && !DataBase.GlobalVariable.IsVisualEditor)
                {
                    vo.IsReadOnly = true;
                }

                if (Utility.Util.GetValueOfString(dr["IsReadOnly"]).Equals("Y"))
                    vo.IsReadOnly = true;
                vo.ReadOnlyLogic = Utility.Util.GetValueOfString(dr["ReadOnlyLogic"]);
                if (Utility.Util.GetValueOfString(dr["IsInsertRecord"]).Equals("N"))
                    vo.IsInsertRecord = false;

                //
                vo.Description = Utility.Util.GetValueOfString(dr["Description"]);
                if (vo.Description == null)
                    vo.Description = "";
                vo.Help = Utility.Util.GetValueOfString(dr["Help"]);
                if (vo.Help == null)
                    vo.Help = "";

                if (Utility.Util.GetValueOfString(dr["IsSingleRow"]).Equals("Y"))
                    vo.IsSingleRow = true;
                if (Utility.Util.GetValueOfString(dr["HasTree"]).Equals("Y"))
                    vo.HasTree = true;

                if (Utility.Util.GetValueOfString(dr["ShowSummaryLevelNodes"]).Equals("Y"))
                    vo.ShowSummaryLevel = true;


                vo.AD_Table_ID = Utility.Util.GetValueOfInt(dr["AD_Table_ID"]);
                vo.TableName = Utility.Util.GetValueOfString(dr["TableName"]);
                if (Utility.Util.GetValueOfString(dr["IsView"]).Equals("Y"))
                    vo.IsView = true;
                vo.AD_Column_ID = Utility.Util.GetValueOfInt(dr["AD_Column_ID"]);   //  Primary Parent Column

                if (Utility.Util.GetValueOfString(dr["IsSecurityEnabled"]).Equals("Y"))
                    vo.IsSecurityEnabled = true;
                if (Utility.Util.GetValueOfString(dr["IsDeleteable"]).Equals("Y"))
                    vo.IsDeleteable = true;
                if (Utility.Util.GetValueOfString(dr["IsHighVolume"]).Equals("Y"))
                    vo.IsHighVolume = true;

                vo.CommitWarning = Utility.Util.GetValueOfString(dr["CommitWarning"]);
                if (vo.CommitWarning == null)
                    vo.CommitWarning = "";
                vo.WhereClause = Utility.Util.GetValueOfString(dr["WhereClause"]);
                if (vo.WhereClause == null)
                    vo.WhereClause = "";

                vo.OrderByClause = Utility.Util.GetValueOfString(dr["OrderByClause"]);
                if (vo.OrderByClause == null)
                    vo.OrderByClause = "";

                vo.AD_Process_ID = Utility.Util.GetValueOfInt(dr["AD_Process_ID"]);
                //if (dr.wasNull())
                //    vo.AD_Process_ID = 0;
                vo.AD_Image_ID = Utility.Util.GetValueOfInt(dr["AD_Image_ID"]);
                //if (dr.wasNull())
                //    vo.AD_Image_ID = 0;
                vo.Included_Tab_ID = Utility.Util.GetValueOfInt(dr["Included_Tab_ID"]);
                //if (dr.wasNull())
                //    vo.Included_Tab_ID = 0;
                //
                vo.TabLevel = Utility.Util.GetValueOfInt(dr["TabLevel"]);
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "TabLevel", vo.TabLevel.ToString());
                //if (dr.wasNull())
                //    vo.TabLevel = 0;
                //
                vo.IsSortTab = Utility.Util.GetValueOfString(dr["IsSortTab"]).Equals("Y");
                if (vo.IsSortTab)
                {
                    vo.AD_ColumnSortOrder_ID = Utility.Util.GetValueOfInt(dr["AD_ColumnSortOrder_ID"]);
                    vo.AD_ColumnSortYesNo_ID = Utility.Util.GetValueOfInt(dr["AD_ColumnSortYesNo_ID"]);
                }
                //
                //	Replication Type - set R/O if Reference
                try
                {
                    //int index = dr.fin .findColumn("ReplicationType");
                    vo.ReplicationType = Utility.Util.GetValueOfString(dr["ReplicationType"]);
                    if ("R".Equals(vo.ReplicationType))
                        vo.IsReadOnly = true;
                }
                catch
                {
                }

                /***************Worked For Header Panel By Karan*****************/
                vo.IsHeaderPanel = dr["Isheaderpanel"].Equals("Y");

                /***************Worked For Header Panel By Mandeep * ****************/
                vo.HPanelNotShowInMultiRow = dr["HPanelNotShowInMultiRow"].Equals("Y");

                vo.AD_HeaderLayout_ID = Util.GetValueOfInt(dr["AD_HeaderLayout_ID"]);

                vo.HeaderAlignment = Utility.Util.GetValueOfString(dr["HeaderAlignment"]);

                vo.TabPanelAlignment = Utility.Util.GetValueOfString(dr["TabPanelAlignment"]);

                vo.HeaderHeight = Utility.Util.GetValueOfDecimal(dr["Height"]);

                vo.HeaderWidth = Utility.Util.GetValueOfDecimal(dr["Width"]);

                vo.HeaderPadding = Utility.Util.GetValueOfString(dr["Padding"]);

                vo.HeaderBackColor = Utility.Util.GetValueOfString(dr["HeaderBackgroundColor"]);

                /***************** End Header panel work ***************/

                // set property for Maintain version on approval
                vo.MaintainVerOnApproval = Utility.Util.GetValueOfString(dr["MaintainVerOnApproval"]).Equals("Y");

                vo.IsMaintainVersions = Utility.Util.GetValueOfString(dr["IsMaintainVersions"]).Equals("Y");

                vo.TabLayout = Utility.Util.GetValueOfString(dr["TabLayout"]);

                vo.NewRecordView = Util.GetValueOfString(dr["NewRecordView"]);
            }
            catch (System.Exception ex)
            {
                VLogger.Get().Log(Level.SEVERE, "", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Create Tab Fields
        /// </summary>
        /// <param name="mTabVO"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        private static bool CreateFields(GridTabVO mTabVO, int AD_UserDef_Win_ID)
        {
            mTabVO.fields = new List<GridFieldVO>();

            String sql = GridFieldVO.GetSQL(mTabVO.ctx, AD_UserDef_Win_ID);
            int AD_Tab_ID = mTabVO.Referenced_Tab_ID;
            if (AD_Tab_ID == 0)
                AD_Tab_ID = mTabVO.AD_Tab_ID;
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", AD_Tab_ID);
                //pstmt.setInt(1, AD_Tab_ID);
                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    GridFieldVO voF = GridFieldVO.Create(mTabVO.ctx,
                        mTabVO.windowNo, mTabVO.tabNo,
                        mTabVO.AD_Window_ID, AD_Tab_ID,
                        mTabVO.IsReadOnly, dr);
                    if (voF != null)
                    {
                        mTabVO.fields.Add(voF);
                        if (voF.displayType == VAdvantage.Classes.DisplayType.Location && voF.IsDisplayedf)
                        {
                            mTabVO.locationCols.Add(voF.ColumnName);
                        }
                    }
                }

                dr.Close();
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                VLogger.Get().Log(Level.SEVERE, sql, e);
                return false;
            }
            return mTabVO.fields.Count != 0;
        }

        private static bool CreateTabPanels(GridTabVO mTabVO)
        {
            mTabVO.panels = new List<GridTabPanelVO>();
            String sql = GridTabPanelVO.GetSQL(mTabVO.ctx);
            int AD_Tab_ID = mTabVO.Referenced_Tab_ID;
            if (AD_Tab_ID == 0)
                AD_Tab_ID = mTabVO.AD_Tab_ID;
            IDataReader dr = null;
            try
            {
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", AD_Tab_ID);
                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    GridTabPanelVO voF = GridTabPanelVO.Create(mTabVO.ctx,
                        mTabVO.windowNo, AD_Tab_ID, dr);
                    if (voF != null)
                    {
                        mTabVO.panels.Add(voF);
                    }
                }

                dr.Close();
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                VLogger.Get().Log(Level.SEVERE, sql, e);
                return false;
            }
            return mTabVO.panels.Count != 0;
        }
        /// <summary>
        /// Create list of header panel items.
        /// </summary>
        /// <param name="mTabVO"></param>
        private static void CreateHeaderPanel(GridTabVO mTabVO)
        {
            if (mTabVO.AD_HeaderLayout_ID > 0)
            {
                mTabVO.HeaderItems = GetHeaderPanelItems(mTabVO.AD_HeaderLayout_ID);
            }
        }

        /// <summary>
        /// Fetch Header Panel (card Template) info
        /// </summary>
        /// <param name="headerLayoutID"></param>
        /// <returns></returns>
        public static List<HeaderPanelGrid> GetHeaderPanelItems(int headerLayoutID)
        {
            VAdvantage.Common.Common fun = new Common.Common();
            List<HeaderPanelGrid> hitems = fun.GetCardTemplateItems(headerLayoutID);
            return hitems;

        }

        /// <summary>
        /// Create card panels for current tab
        /// </summary>
        /// <param name="mTabVO"></param>
        /// <param name="ctx"></param>
        private static void CreateCardPanels(GridTabVO mTabVO, Ctx ctx)
        {
            VAdvantage.Common.Common cFun = new VAdvantage.Common.Common();
            CardViewData card = cFun.GetCardViewDetails(ctx.GetAD_User_ID(), mTabVO.AD_Tab_ID, 0, ctx );
            if (card != null)
            {
                //mTabVO.Cards.Add(card);
                mTabVO.DefaultCardID = card.AD_CardView_ID;
            }
        }


        /// <summary>
        ///  Create Tab Fields
        /// </summary>
        /// <param name="mTabVO"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        private static bool CreateEditorFields(GridTabVO mTabVO, int AD_UserDef_Win_ID)
        {
            mTabVO.fields = new List<GridFieldVO>();

            bool isBase = true;
            if (!Env.IsBaseLanguage(mTabVO.ctx, "AD_Tab"))
            {
                isBase = false;
            }

            StringBuilder sql = new StringBuilder(" SELECT ")
           .Append(" t.AD_Window_ID                                                      AS AD_Window_ID         , ")
           .Append(" f.AD_Tab_ID                                                         AS AD_Tab_ID            , ")
           .Append(" f.AD_Field_ID                                                       AS AD_Field_ID          , ")
           .Append(" tbl.AD_Table_ID                                                     AS AD_Table_ID          , ")
           .Append(" f.AD_Column_ID                                                      AS AD_Column_ID         , ")
           .Append(" uw.AD_Role_ID                                                       AS UserDef_Role_ID      , ")
           .Append(" uw.AD_User_ID                                                       AS AD_User_ID           , ")
           .Append(" uw.AD_UserDef_Win_ID                                                AS AD_UserDef_Win_ID    , ")
           .Append(" uw.CustomizationName                                                AS CustomizationName    , ")
           .Append(" u.AD_UserDef_Tab_ID                                                 AS AD_UserDef_Tab_ID    , ")
           .Append(" u.AD_UserDef_Field_ID                                               AS AD_UserDef_Field_ID  , ");
            if (!isBase)
            {
                sql.Append(" COALESCE(u.Name,trl.Name)                                           AS Name                 , ")
                   .Append(" COALESCE(u.Description,trl.Description)                             AS Description          , ")
                   .Append(" COALESCE(u.Help,trl.Help)                                           AS Help                 , ");
            }
            else
            {
                sql.Append(" COALESCE(u.Name,f.Name)                                             AS Name                 , ")
                   .Append(" COALESCE(u.Description,f.Description)                               AS Description          , ")
                   .Append(" COALESCE(u.Help,f.Help)                                             AS Help                 , ");
            }

            sql.Append(" COALESCE(u.IsDisplayed,f.IsDisplayed)                               AS IsDisplayed          , ")
               .Append(" COALESCE(u.DisplayLogic,f.DisplayLogic)                             AS DisplayLogic         , ")
               .Append(" COALESCE(u.DisplayLength,f.DisplayLength)                           AS DisplayLength        , ")
               .Append(" COALESCE(u.SeqNo,f.SeqNo)                                           AS SeqNo                , ")
               .Append(" COALESCE(u.SortNo,f.SortNo)                                         AS SortNo               , ")
               .Append(" COALESCE(u.IsSameLine,f.IsSameLine)                                 AS IsSameLine           , ")
               .Append(" f.IsHeading                                                         AS IsHeading            , ")
               .Append(" f.IsFieldOnly                                                       AS IsFieldOnly          , ")
               .Append(" f.IsReadOnly                                                        AS IsReadOnly           , ")
               .Append(" f.IsEncrypted                                                       AS IsEncryptedField     , ")
               .Append(" f.ObscureType                                                       AS ObscureType          , ")
               .Append(" f.IsDefaultFocus                                                    AS IsDefaultFocus       , ")
               .Append(" COALESCE(u.MRSeqNo,f.MRSeqNo)                                       AS MRSeqNo              , ")
               .Append(" c.ColumnName                                                        AS ColumnName           , ")
               .Append(" c.ColumnSQL                                                         AS ColumnSQL            , ")
               .Append(" c.FieldLength                                                       AS FieldLength          , ")
               .Append(" c.VFormat                                                           AS VFormat              , ")
               .Append(" COALESCE(u.DefaultValue,COALESCE(f.DefaultValue,c.DefaultValue))    AS DefaultValue         , ")
               .Append(" c.IsKey                                                             AS IsKey                , ")
               .Append(" c.IsParent                                                          AS IsParent             , ")
               .Append(" c.IsMandatory                                                       AS IsMandatory          , ")
               .Append(" c.MandatoryLogic                                                    AS MandatoryLogic       , ")
               .Append(" COALESCE(COALESCE(u.IsMandatoryUI,f.IsMandatoryUI),c.IsMandatoryUI) AS IsMandatoryUI        , ")
               .Append(" c.IsIdentifier                                                      AS IsIdentifier         , ")
               .Append(" c.IsTranslated                                                      AS IsTranslated         , ")
               .Append(" c.AD_Reference_Value_ID                                             AS AD_Reference_Value_ID, ")
               .Append(" c.Callout                                                           AS Callout              , ")
               .Append(" c.IsCallout                                                         AS IsCallout            , ")
               .Append(" COALESCE(f.AD_Reference_ID,c.AD_Reference_ID)                       AS AD_Reference_ID      , ")
               .Append(" c.AD_Val_Rule_ID                                                    AS AD_Val_Rule_ID       , ")
               .Append(" c.AD_Process_ID                                                     AS AD_Process_ID        , ")
               .Append(" c.IsAlwaysUpdateable                                                AS IsAlwaysUpdateable   , ")
               .Append(" c.ReadOnlyLogic                                                     AS ReadOnlyLogic        , ")
               .Append(" c.IsUpdateable                                                      AS IsUpdateable         , ")
               .Append(" c.IsEncrypted                                                       AS IsEncryptedColumn    , ")
               .Append(" COALESCE(u.IsSelectionColumn,c.IsSelectionColumn)                   AS IsSelectionColumn    , ")
               .Append(" COALESCE(u.SelectionSeqNo,c.SelectionSeqNo)                         AS SelectionSeqNo       , ")
               .Append(" COALESCE(u.IsSummaryColumn,c.IsSummaryColumn)                       AS IsSummaryColumn      , ")
               .Append(" COALESCE(u.SummarySeqNo,c.SummarySeqNo)                             AS SummarySeqNo         , ")
               .Append(" tbl.TableName                                                       AS TableName            , ")
               .Append(" COALESCE(u.ValueMin,c.ValueMin)                                     AS ValueMin             , ")
               .Append(" COALESCE(u.ValueMax,c.ValueMax)                                     AS ValueMax             , ")
               .Append(" fgt.Name                                                            AS FieldGroup           , ")
               .Append(" vr.Code                                                             AS ValidationCode  ");

            if (isBase)
            {
                sql.Append(" FROM AD_Field f ");
            }
            else
            {
                sql.Append(" FROM AD_Field f ")
               .Append("  INNER JOIN AD_Field_Trl trl ON f.AD_Field_ID = trl.AD_Field_ID ");
            }
            sql.Append(" INNER JOIN AD_Tab t ON (f.AD_Tab_ID = t.AD_Tab_ID) LEFT OUTER JOIN ");

            if (isBase)
            {
                sql.Append(" AD_FieldGroup fgt ON (f.AD_FieldGroup_ID = fgt.AD_FieldGroup_ID) ");
            }
            else
            {
                sql.Append(" AD_FieldGroup_Trl fgt ON (f.AD_FieldGroup_ID = fgt.AD_FieldGroup_ID  AND trl.AD_Language  =fgt.AD_Language) ");
            }
            sql.Append(" LEFT OUTER JOIN AD_Column c  ON (f.AD_Column_ID = c.AD_Column_ID) INNER JOIN AD_Table tbl ON (c.AD_Table_ID = tbl.AD_Table_ID) ")
               .Append(" INNER JOIN AD_Reference r ON (c.AD_Reference_ID = r.AD_Reference_ID) LEFT OUTER JOIN AD_Val_Rule vr ON (c.AD_Val_Rule_ID=vr.AD_Val_Rule_ID) ")
               .Append(" LEFT OUTER JOIN AD_UserDef_Field u ON (f.AD_Field_ID=u.AD_Field_ID) LEFT OUTER JOIN AD_UserDef_Tab ut ON (ut.AD_UserDef_Tab_ID=u.AD_UserDef_Tab_ID) ")
               .Append(" LEFT OUTER JOIN AD_UserDef_Win uw ON (uw.AD_UserDef_Win_ID=ut.AD_UserDef_Win_ID) ")

               .Append(" WHERE f.AD_Tab_ID=@tabID");

            if (!isBase)
            {
                sql.Append(" AND trl.AD_Language='" + Env.GetAD_Language(mTabVO.ctx) + "'");
            }
            if (AD_UserDef_Win_ID != 0)
                sql.Append(" AND u.AD_UserDef_Win_ID=" + AD_UserDef_Win_ID);

            sql.Append(" ORDER BY IsDisplayed DESC, SeqNo");


            int AD_Tab_ID = mTabVO.Referenced_Tab_ID;
            if (AD_Tab_ID == 0)
                AD_Tab_ID = mTabVO.AD_Tab_ID;
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", AD_Tab_ID);
                //pstmt.setInt(1, AD_Tab_ID);
                dr = DataBase.DB.ExecuteReader(sql.ToString(), param, null);
                while (dr.Read())
                {
                    GridFieldVO voF = GridFieldVO.Create(mTabVO.ctx,
                        mTabVO.windowNo, mTabVO.tabNo,
                        mTabVO.AD_Window_ID, AD_Tab_ID,
                        mTabVO.IsReadOnly, dr);
                    if (voF != null)
                        mTabVO.fields.Add(voF);
                }
                dr.Close();
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                VLogger.Get().Log(Level.SEVERE, sql.ToString(), e);
                return false;
            }
            return mTabVO.fields.Count != 0;
        }

        /// <summary>
        ///Return the SQL statement used for the MTabVO.create
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_UserDef_Win_ID"></param>
        /// <returns></returns>
        public static String GetSQL(Ctx ctx, int AD_UserDef_Win_ID)
        {
            //  View only returns IsActive='Y'
            String sql = "SELECT * FROM AD_Tab_v WHERE AD_Window_ID=@windowID";
            if (!Env.IsBaseLanguage(ctx, "AD_Window"))
                sql = "SELECT * FROM AD_Tab_vt WHERE AD_Window_ID=@windowID"
                    + " AND AD_Language='" + Env.GetAD_Language(ctx) + "'";
            if (AD_UserDef_Win_ID != 0)
                sql += " AND AD_UserDef_Win_ID=" + AD_UserDef_Win_ID;
            sql += " ORDER BY SeqNo";
            return sql;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="newCtx">context</param>
        /// <param name="windowNo">window number</param>
        private GridTabVO(Ctx newCtx, int windowNm)
        {
            ctx = newCtx;
            windowNo = windowNm;
        }   //  MTabVO

        //static long serialVersionUID = 9160212869277319305L;

        /// <summary>
        /// Set Context including contained elements
        /// </summary>
        /// <param name="newCtx">context</param>
        public void SetCtx(Ctx newCtx)
        {
            ctx = newCtx;
            for (int i = 0; i < fields.Count; i++)
            {
                GridFieldVO field = fields[i];
                field.SetCtx(newCtx);
            }
        }

        /// <summary>
        ///	Get Variable Value (Evaluatee)
        /// </summary>
        /// <param name="variableName">key value</param>
        /// <returns></returns>
        public String Get_ValueAsString(String variableName)
        {
            return ctx.GetContext(windowNo, variableName, false);   // not just window
        }

        /// <summary>
        ///	Clone object
        /// </summary>
        /// <param name="myCtx">Context</param>
        /// <param name="windowNo">window number</param>
        /// <returns></returns>
        public GridTabVO Clone(Ctx myCtx, int windowNo)
        {
            GridTabVO clone = new GridTabVO(myCtx, windowNo);
            clone.AD_Window_ID = AD_Window_ID;
            clone.tabNo = tabNo;

            //
            clone.AD_Tab_ID = AD_Tab_ID;
            myCtx.SetContext(windowNo, clone.tabNo, "AD_Tab_ID", clone.AD_Tab_ID.ToString());
            clone.Referenced_Tab_ID = Referenced_Tab_ID;
            clone.Name = Name;
            myCtx.SetContext(windowNo, clone.tabNo, "Name", clone.Name);
            clone.Description = Description;
            clone.Help = Help;
            clone.IsSingleRow = IsSingleRow;
            clone.IsReadOnly = IsReadOnly;
            clone.IsInsertRecord = IsInsertRecord;
            clone.HasTree = HasTree;
            clone.ShowSummaryLevel = ShowSummaryLevel;
            clone.AD_Table_ID = AD_Table_ID;
            clone.AD_Column_ID = AD_Column_ID;
            clone.TableName = TableName;
            clone.IsView = IsView;
            clone.AccessLevel = AccessLevel;
            clone.IsSecurityEnabled = IsSecurityEnabled;
            clone.IsDeleteable = IsDeleteable;
            clone.IsHighVolume = IsHighVolume;
            clone.AD_Process_ID = AD_Process_ID;
            clone.CommitWarning = CommitWarning;
            clone.WhereClause = WhereClause;
            clone.OrderByClause = OrderByClause;
            clone.ReadOnlyLogic = ReadOnlyLogic;
            clone.DisplayLogic = DisplayLogic;
            clone.TabLevel = TabLevel;
            clone.AD_Image_ID = AD_Image_ID;
            clone.Included_Tab_ID = Included_Tab_ID;
            clone.ReplicationType = ReplicationType;
            myCtx.SetContext(windowNo, clone.tabNo, "AccessLevel", clone.AccessLevel);
            myCtx.SetContext(windowNo, clone.tabNo, "AD_Table_ID", clone.AD_Table_ID.ToString());
            myCtx.SetContext(windowNo, clone.tabNo, "TabLevel", clone.TabLevel.ToString());

            //
            clone.IsSortTab = IsSortTab;
            clone.AD_ColumnSortOrder_ID = AD_ColumnSortOrder_ID;
            clone.AD_ColumnSortYesNo_ID = AD_ColumnSortYesNo_ID;
            //  Derived
            clone.onlyCurrentDays = 0;


            //Tab Panles
            clone.panels = new List<GridTabPanelVO>();
            for (int i = 0; i < panels.Count; i++)
            {
                GridTabPanelVO tpo = panels[i];
                GridTabPanelVO clonetp = tpo.Clone(myCtx, windowNo);
                if (clonetp == null)
                    return null;
                clone.panels.Add(clonetp);
            }
            if (clone.panels != null && clone.panels.Count > 0)
            {
                clone.HasPanels = true;
            }

            clone.locationCols = locationCols;

            clone.IsHeaderPanel = IsHeaderPanel;
            clone.AD_HeaderLayout_ID = AD_HeaderLayout_ID;
            clone.HPanelNotShowInMultiRow = HPanelNotShowInMultiRow;
            clone.HeaderAlignment = HeaderAlignment;
            clone.TabPanelAlignment = TabPanelAlignment;
            clone.HeaderItems = HeaderItems;
            clone.HeaderHeight = HeaderHeight;
            clone.HeaderWidth = HeaderWidth;
            clone.HeaderPadding = HeaderPadding;
            clone.HeaderBackColor = HeaderBackColor;

            // set Maintain Version on Approval from Tab
            clone.MaintainVerOnApproval = MaintainVerOnApproval;

            clone.IsMaintainVersions = IsMaintainVersions;
            clone.TabLayout = TabLayout;
           // clone.DefaultCardID = DefaultCardID;
            clone.NewRecordView = NewRecordView;

            clone.fields = new List<GridFieldVO>();
            for (int i = 0; i < fields.Count; i++)
            {
                GridFieldVO field = fields[i];
                GridFieldVO cloneField = field.Clone(myCtx, windowNo, tabNo,
                    AD_Window_ID, AD_Tab_ID, IsReadOnly);
                if (cloneField == null)
                    return null;
                clone.fields.Add(cloneField);
            }

            return clone;
        }
    }
}


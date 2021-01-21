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

        /** Tab No (not VAF_Tab_ID) 0.. */
        public int tabNo;

        /**	Tab	ID			*/
        public int VAF_Tab_ID;
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
        public int VAF_TableView_ID;
        /** Primary Parent Column   */
        public int VAF_Column_ID = 0;
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
        public int VAF_Job_ID = 0;
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
        public int VAF_Image_ID = 0;
        /** Included Tab	*/
        public int Included_Tab_ID = 0;
        /** Replication Type	*/
        public String ReplicationType = "L";

        /** Sort Tab			*/
        public bool IsSortTab = false;
        /** Column Sort			*/
        public int VAF_ColumnSortOrder_ID = 0;
        /** Column Displayed	*/
        public int VAF_ColumnSortYesNo_ID = 0;

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

        /****   Grid Layout   ***/
        public int VAF_HeaderLayout_ID = 0;

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
                vo.VAF_Tab_ID = Utility.Util.GetValueOfInt(dr["VAF_Tab_ID"]);
                vo.Referenced_Tab_ID = Utility.Util.GetValueOfInt(dr["Referenced_Tab_ID"]);
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "VAF_Tab_ID", vo.VAF_Tab_ID.ToString());
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
                        VLogger.Get().Config("TrlTab Not displayed - VAF_Tab_ID="
                            + vo.VAF_Tab_ID + "=" + vo.Name + ", Table=" + vo.TableName
                            + ", BaseTrl=" + Env.IsBaseTranslation(vo.TableName)
                            + ", MultiLingual=" + Common.Common.IsMultiLingualDocument(vo.ctx));
                        return false;
                    }
                }
                //	Advanced Tab	**
                if (!showAdvanced && Utility.Util.GetValueOfString(dr["IsAdvancedTab"]).Equals("Y"))
                {
                    VLogger.Get().Config("AdvancedTab Not displayed - VAF_Tab_ID="
                        + vo.VAF_Tab_ID + " " + vo.Name);
                    return false;
                }
                //	Accounting Info Tab	**
                if (!showAcct && Utility.Util.GetValueOfString(dr["IsInfoTab"]).Equals("Y"))
                {
                    VLogger.Get().Fine("AcctTab Not displayed - VAF_Tab_ID="
                        + vo.VAF_Tab_ID + " " + vo.Name);
                    return false;
                }

                //	DisplayLogic
                vo.DisplayLogic = Utility.Util.GetValueOfString(dr["DisplayLogic"]);

                //	Access Level
                vo.AccessLevel = Utility.Util.GetValueOfString(dr["AccessLevel"]);
                if (!role.CanView(vo.ctx, vo.AccessLevel) && !DataBase.GlobalVariable.IsVisualEditor)	//	No Access
                {
                    VLogger.Get().Fine("No Role Access - VAF_Tab_ID=" + vo.VAF_Tab_ID + " " + vo.Name);
                    return false;
                }	//	Used by MField.getDefault
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "AccessLevel", vo.AccessLevel);

                //	Table Access
                vo.VAF_TableView_ID = Utility.Util.GetValueOfInt(dr["VAF_TableView_ID"]);
                vo.ctx.SetContext(vo.windowNo, vo.tabNo, "VAF_TableView_ID", vo.VAF_TableView_ID.ToString());
                if (!role.IsTableAccess(vo.VAF_TableView_ID, true) && !DataBase.GlobalVariable.IsVisualEditor)
                {
                    VLogger.Get().Config("No Table Access - VAF_Tab_ID="
                        + vo.VAF_Tab_ID + " " + vo.Name);
                    return false;
                }

                if (role.IsTableReadOnly(vo.VAF_TableView_ID) && !DataBase.GlobalVariable.IsVisualEditor)
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


                vo.VAF_TableView_ID = Utility.Util.GetValueOfInt(dr["VAF_TableView_ID"]);
                vo.TableName = Utility.Util.GetValueOfString(dr["TableName"]);
                if (Utility.Util.GetValueOfString(dr["IsView"]).Equals("Y"))
                    vo.IsView = true;
                vo.VAF_Column_ID = Utility.Util.GetValueOfInt(dr["VAF_Column_ID"]);   //  Primary Parent Column

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

                vo.VAF_Job_ID = Utility.Util.GetValueOfInt(dr["VAF_Job_ID"]);
                //if (dr.wasNull())
                //    vo.VAF_Job_ID = 0;
                vo.VAF_Image_ID = Utility.Util.GetValueOfInt(dr["VAF_Image_ID"]);
                //if (dr.wasNull())
                //    vo.VAF_Image_ID = 0;
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
                    vo.VAF_ColumnSortOrder_ID = Utility.Util.GetValueOfInt(dr["VAF_ColumnSortOrder_ID"]);
                    vo.VAF_ColumnSortYesNo_ID = Utility.Util.GetValueOfInt(dr["VAF_ColumnSortYesNo_ID"]);
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

                vo.VAF_HeaderLayout_ID = Util.GetValueOfInt(dr["VAF_HeaderLayout_ID"]);

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
            int VAF_Tab_ID = mTabVO.Referenced_Tab_ID;
            if (VAF_Tab_ID == 0)
                VAF_Tab_ID = mTabVO.VAF_Tab_ID;
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", VAF_Tab_ID);
                //pstmt.setInt(1, VAF_Tab_ID);
                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    GridFieldVO voF = GridFieldVO.Create(mTabVO.ctx,
                        mTabVO.windowNo, mTabVO.tabNo,
                        mTabVO.AD_Window_ID, VAF_Tab_ID,
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
            int VAF_Tab_ID = mTabVO.Referenced_Tab_ID;
            if (VAF_Tab_ID == 0)
                VAF_Tab_ID = mTabVO.VAF_Tab_ID;
            IDataReader dr = null;
            try
            {
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", VAF_Tab_ID);
                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    GridTabPanelVO voF = GridTabPanelVO.Create(mTabVO.ctx,
                        mTabVO.windowNo, VAF_Tab_ID, dr);
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
            if (mTabVO.VAF_HeaderLayout_ID > 0)
            {
                DataSet dsGridLayout = DataBase.DB.ExecuteDataset("SELECT * FROM VAF_GridLayout  WHERE IsActive='Y' AND VAF_HeaderLayout_ID=" + mTabVO.VAF_HeaderLayout_ID +" ORDER BY SeqNo Asc");
                if (dsGridLayout != null && dsGridLayout.Tables[0].Rows.Count > 0)
                {
                    mTabVO.HeaderItems = new List<HeaderPanelGrid>();
                    
                    foreach (DataRow dr in dsGridLayout.Tables[0].Rows)
                    {
                        HeaderPanelGrid hGrid = new HeaderPanelGrid
                        {
                          
                            HeaderBackColor = Utility.Util.GetValueOfString(dr["BackgroundColor"]),

                            HeaderName = Utility.Util.GetValueOfString(dr["Name"]),

                            HeaderTotalColumn = Utility.Util.GetValueOfInt(dr["TotalColumns"]),

                            HeaderTotalRow = Utility.Util.GetValueOfInt(dr["TotalRows"]),

                            HeaderPadding = Utility.Util.GetValueOfString(dr["Padding"]),

                            VAF_GridLayout_ID = Utility.Util.GetValueOfInt(dr["VAF_GridLayout_ID"]),
                        };

                        DataSet ds = DataBase.DB.ExecuteDataset("SELECT AlignItems,   ColumnSpan,   Justifyitems,   Rowspan,   Seqno,   Startcolumn,   Startrow," +
                            " VAF_GridLayoutItems_ID,BackgroundColor, FontColor, FontSize,padding FROM VAF_GridLayoutitems WHERE IsActive      ='Y' AND VAF_GridLayout_ID=" + hGrid.VAF_GridLayout_ID);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            hGrid.HeaderItems = new Dictionary<int, object>();
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                hGrid.HeaderItems[Convert.ToInt32(row["SeqNo"])] = new HeaderPanelItemsVO
                                {
                                    VAF_GridLayoutItems_ID = Convert.ToInt32(row["VAF_GridLayoutItems_ID"]),
                                    ColumnSpan = Convert.ToInt32(row["ColumnSpan"]),
                                    AlignItems = Convert.ToString(row["AlignItems"]),
                                    JustifyItems = Convert.ToString(row["JustifyItems"]),
                                    RowSpan = Convert.ToInt32(row["RowSpan"]),
                                    SeqNo = Convert.ToInt32(row["SeqNo"]),
                                    StartColumn = Convert.ToInt32(row["StartColumn"]),
                                    StartRow = Convert.ToInt32(row["StartRow"]),
                                    BackgroundColor= Convert.ToString(row["BackgroundColor"]),
                                    FontColor = Convert.ToString(row["FontColor"]),
                                    FontSize = Convert.ToString(row["FontSize"]),
                                    Padding = Convert.ToString(row["Padding"]),
                                };
                            }
                        }
                        mTabVO.HeaderItems.Add(hGrid);
                    }
                }
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
            if (!Env.IsBaseLanguage(mTabVO.ctx, "VAF_Tab"))
            {
                isBase = false;
            }

            StringBuilder sql = new StringBuilder(" SELECT ")
           .Append(" t.AD_Window_ID                                                      AS AD_Window_ID         , ")
           .Append(" f.VAF_Tab_ID                                                         AS VAF_Tab_ID            , ")
           .Append(" f.VAF_Field_ID                                                       AS VAF_Field_ID          , ")
           .Append(" tbl.VAF_TableView_ID                                                     AS VAF_TableView_ID          , ")
           .Append(" f.VAF_Column_ID                                                      AS VAF_Column_ID         , ")
           .Append(" uw.VAF_Role_ID                                                       AS UserDef_Role_ID      , ")
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
               .Append(" c.VAF_Control_Ref_Value_ID                                             AS VAF_Control_Ref_Value_ID, ")
               .Append(" c.Callout                                                           AS Callout              , ")
               .Append(" c.IsCallout                                                         AS IsCallout            , ")
               .Append(" COALESCE(f.VAF_Control_Ref_ID,c.VAF_Control_Ref_ID)                       AS VAF_Control_Ref_ID      , ")
               .Append(" c.VAF_DataVal_Rule_ID                                                    AS VAF_DataVal_Rule_ID       , ")
               .Append(" c.VAF_Job_ID                                                     AS VAF_Job_ID        , ")
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
                sql.Append(" FROM VAF_Field f ");
            }
            else
            {
                sql.Append(" FROM VAF_Field f ")
               .Append("  INNER JOIN VAF_Field_TL trl ON f.VAF_Field_ID = trl.VAF_Field_ID ");
            }
            sql.Append(" INNER JOIN VAF_Tab t ON (f.VAF_Tab_ID = t.VAF_Tab_ID) LEFT OUTER JOIN ");

            if (isBase)
            {
                sql.Append(" VAF_FieldSection fgt ON (f.VAF_FieldSection_ID = fgt.VAF_FieldSection_ID) ");
            }
            else
            {
                sql.Append(" VAF_FieldSection_Tl fgt ON (f.VAF_FieldSection_ID = fgt.VAF_FieldSection_ID  AND trl.VAF_Language  =fgt.VAF_Language) ");
            }
            sql.Append(" LEFT OUTER JOIN VAF_Column c  ON (f.VAF_Column_ID = c.VAF_Column_ID) INNER JOIN VAF_TableView tbl ON (c.VAF_TableView_ID = tbl.VAF_TableView_ID) ")
               .Append(" INNER JOIN VAF_Control_Ref r ON (c.VAF_Control_Ref_ID = r.VAF_Control_Ref_ID) LEFT OUTER JOIN VAF_DataVal_Rule vr ON (c.VAF_DataVal_Rule_ID=vr.VAF_DataVal_Rule_ID) ")
               .Append(" LEFT OUTER JOIN AD_UserDef_Field u ON (f.VAF_Field_ID=u.VAF_Field_ID) LEFT OUTER JOIN AD_UserDef_Tab ut ON (ut.AD_UserDef_Tab_ID=u.AD_UserDef_Tab_ID) ")
               .Append(" LEFT OUTER JOIN AD_UserDef_Win uw ON (uw.AD_UserDef_Win_ID=ut.AD_UserDef_Win_ID) ")

               .Append(" WHERE f.VAF_Tab_ID=@tabID");

            if (!isBase)
            {
                sql.Append(" AND trl.VAF_Language='" + Env.GetVAF_Language(mTabVO.ctx) + "'");
            }
            if (AD_UserDef_Win_ID != 0)
                sql.Append(" AND u.AD_UserDef_Win_ID=" + AD_UserDef_Win_ID);

            sql.Append(" ORDER BY IsDisplayed DESC, SeqNo");


            int VAF_Tab_ID = mTabVO.Referenced_Tab_ID;
            if (VAF_Tab_ID == 0)
                VAF_Tab_ID = mTabVO.VAF_Tab_ID;
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                param[0] = new System.Data.SqlClient.SqlParameter("@tabID", VAF_Tab_ID);
                //pstmt.setInt(1, VAF_Tab_ID);
                dr = DataBase.DB.ExecuteReader(sql.ToString(), param, null);
                while (dr.Read())
                {
                    GridFieldVO voF = GridFieldVO.Create(mTabVO.ctx,
                        mTabVO.windowNo, mTabVO.tabNo,
                        mTabVO.AD_Window_ID, VAF_Tab_ID,
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
            String sql = "SELECT * FROM VAF_Tab_v WHERE AD_Window_ID=@windowID";
            if (!Env.IsBaseLanguage(ctx, "AD_Window"))
                sql = "SELECT * FROM VAF_Tab_vtl WHERE AD_Window_ID=@windowID"
                    + " AND VAF_Language='" + Env.GetVAF_Language(ctx) + "'";
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
            clone.VAF_Tab_ID = VAF_Tab_ID;
            myCtx.SetContext(windowNo, clone.tabNo, "VAF_Tab_ID", clone.VAF_Tab_ID.ToString());
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
            clone.VAF_TableView_ID = VAF_TableView_ID;
            clone.VAF_Column_ID = VAF_Column_ID;
            clone.TableName = TableName;
            clone.IsView = IsView;
            clone.AccessLevel = AccessLevel;
            clone.IsSecurityEnabled = IsSecurityEnabled;
            clone.IsDeleteable = IsDeleteable;
            clone.IsHighVolume = IsHighVolume;
            clone.VAF_Job_ID = VAF_Job_ID;
            clone.CommitWarning = CommitWarning;
            clone.WhereClause = WhereClause;
            clone.OrderByClause = OrderByClause;
            clone.ReadOnlyLogic = ReadOnlyLogic;
            clone.DisplayLogic = DisplayLogic;
            clone.TabLevel = TabLevel;
            clone.VAF_Image_ID = VAF_Image_ID;
            clone.Included_Tab_ID = Included_Tab_ID;
            clone.ReplicationType = ReplicationType;
            myCtx.SetContext(windowNo, clone.tabNo, "AccessLevel", clone.AccessLevel);
            myCtx.SetContext(windowNo, clone.tabNo, "VAF_TableView_ID", clone.VAF_TableView_ID.ToString());
            myCtx.SetContext(windowNo, clone.tabNo, "TabLevel", clone.TabLevel.ToString());

            //
            clone.IsSortTab = IsSortTab;
            clone.VAF_ColumnSortOrder_ID = VAF_ColumnSortOrder_ID;
            clone.VAF_ColumnSortYesNo_ID = VAF_ColumnSortYesNo_ID;
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
            clone.VAF_HeaderLayout_ID = VAF_HeaderLayout_ID;
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

            clone.fields = new List<GridFieldVO>();
            for (int i = 0; i < fields.Count; i++)
            {
                GridFieldVO field = fields[i];
                GridFieldVO cloneField = field.Clone(myCtx, windowNo, tabNo,
                    AD_Window_ID, VAF_Tab_ID, IsReadOnly);
                if (cloneField == null)
                    return null;
                clone.fields.Add(cloneField);
            }

            return clone;
        }
    }
}


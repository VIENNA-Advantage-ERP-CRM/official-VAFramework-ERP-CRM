/********************************************************
// Module Name    : Run Time Show Window
// Purpose        : Model Field Value Object(get and set field attributes) (columns of table)
// Class Used     : GlobalVariable.cs, CommonFunctions.cs
// Created By     : Harwinder 
// Date           : 13 jan 2009
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.Classes;

namespace VAdvantage.Controller
{
    //public interface Evaluatee
    //{
    //    /// <summary>
    //    /// Get Variable Value
    //    /// </summary>
    //    /// <param name="variableName"></param>
    //    /// <returns></returns>
    //    string GetValueAsString(string variableName);
    //}
    public class GridFieldVO : FieldVObj, Evaluatee
    {
        /*callout text*/
        public String Callout = "";
        /** Ctx                     */
        private Ctx ctx = null;
        /** Lookup Value Object     */
        public Classes.VLookUpInfo lookupInfo = null;

        /// <summary>
        /// Return the SQL statement used for the MFieldVO.create
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_UserDef_Win_ID">window id</param>
        /// <returns>Sql string</returns>
        public static String GetSQL(Ctx ctx, int AD_UserDef_Win_ID)
        {
            //	IsActive is part of View
            String sql = "SELECT * FROM AD_Field_v WHERE AD_Tab_ID=@tabID";
            if (!Env.IsBaseLanguage(ctx, "AD_Tab"))
                sql = "SELECT * FROM AD_Field_vt WHERE AD_Tab_ID=@tabID"
                    + " AND AD_Language='" + Env.GetAD_Language(ctx) + "'";
            if (AD_UserDef_Win_ID != 0)
                sql += " AND AD_UserDef_Win_ID=" + AD_UserDef_Win_ID;
            sql += " ORDER BY IsDisplayed DESC, SeqNo";
            return sql;
        }


        public Ctx GetCtx()
        {
            return ctx;
        }

        /// <summary>
        ///  Create Field Value Object
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="WindowNo">window number</param>
        /// <param name="TabNo">tab number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <param name="AD_Tab_ID">Tab Id</param>
        /// <param name="readOnly">is readonly</param>
        /// <param name="dr">datarow</param>
        /// <returns>object of this Class</returns>
        public static GridFieldVO Create(Ctx ctx, int windowNo, int tabNo,
            int AD_Window_ID, int AD_Tab_ID, bool readOnly, IDataReader dr)
        {
            GridFieldVO vo = new GridFieldVO(ctx, windowNo, tabNo,
                AD_Window_ID, AD_Tab_ID, readOnly);
            String columnName = "ColumnName";
            try
            {
                vo.ColumnName = dr["ColumnName"].ToString();
                if (vo.ColumnName == null || columnName.Trim().Length == 0)
                    return null;

                // VLogger.Get().Fine(vo.ColumnName);

                //ResultSetMetaData rsmd = dr.getMetaData();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    columnName = dr.GetName(i).ToUpper();// rsmd.getColumnName(i);
                    if (columnName.Equals("NAME"))
                        vo.Header = dr[i].ToString();
                    else if (columnName.Equals("AD_REFERENCE_ID"))
                        vo.displayType = Utility.Util.GetValueOfInt(dr[i]);//  Utility.Util.GetValueOfInt(dr[i])
                    else if (columnName.Equals("AD_COLUMN_ID"))
                        vo.AD_Column_ID = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("AD_INFOWINDOW_ID"))
                        vo.AD_InfoWindow_ID = Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("AD_TABLE_ID"))
                        vo.AD_Table_ID = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("DISPLAYLENGTH"))
                        vo.DisplayLength = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("ISSAMELINE"))
                        vo.IsSameLine = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISDISPLAYED"))
                        vo.IsDisplayedf = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("MRISDISPLAYED"))
                        vo.IsDisplayedMR = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("DISPLAYLOGIC"))
                        vo.DisplayLogic = dr[i].ToString();
                    else if (columnName.Equals("STYLELOGIC"))
                        vo.StyleLogic = dr[i].ToString();
                    else if (columnName.Equals("DEFAULTVALUE"))
                        vo.DefaultValue = dr[i].ToString();
                    else if (columnName.Equals("ISMANDATORYUI"))
                        vo.IsMandatoryUI = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISREADONLY"))
                        vo.IsReadOnly = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISUPDATEABLE"))
                        vo.IsUpdateable = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISALWAYSUPDATEABLE"))
                        vo.IsAlwaysUpdateable = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISHEADING"))
                        vo.IsHeading = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISFIELDONLY"))
                        vo.IsFieldOnly = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISENCRYPTEDFIELD"))
                        vo.IsEncryptedField = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISENCRYPTEDCOLUMN"))
                        vo.IsEncryptedColumn = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISSELECTIONCOLUMN"))
                        vo.IsSelectionColumn = "Y".Equals(dr[i].ToString());
                    //else if (columnName.Equals("ISINCLUDEDCOLUMN"))
                    //    vo.IsIncludedColumn = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("SELECTIONSEQNO"))
                        vo.SelectionSeqNo = Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("SORTNO"))
                        vo.SortNo = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("FIELDLENGTH"))
                    {
                        vo.FieldLength = Utility.Util.GetValueOfInt(dr[i]);
                       var overWriteLength= Utility.Util.GetValueOfInt(dr["OVERWRITEFIELDLENGTH"]);
                        if (overWriteLength > 0)
                            vo.FieldLength = overWriteLength;
                    }
                    //else if (columnName.Equals(""))
                    //{
                    //    int length = Utility.Util.GetValueOfInt(dr[i]);
                    //    if (length > 0)
                    //        vo.FieldLength = length;
                    //}
                    else if (columnName.Equals("VFORMAT"))
                        vo.VFormat = dr[i].ToString();
                    else if (columnName.Equals("VFORMATERROR"))
                        vo.VFormatError = dr[i].ToString();
                    else if (columnName.Equals("VALUEMIN"))
                        vo.ValueMin = dr[i].ToString();
                    else if (columnName.Equals("VALUEMAX"))
                        vo.ValueMax = dr[i].ToString();
                    else if (columnName.Equals("FIELDGROUP"))
                        vo.FieldGroup = Env.TrimModulePrefix(dr[i].ToString());
                    else if (columnName.Equals("ISKEY"))
                        vo.IsKey = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("ISPARENT"))
                        vo.IsParent = "Y".Equals(dr[i].ToString());
                    else if (columnName.Equals("DESCRIPTION"))
                        vo.Description = dr[i].ToString();
                    else if (columnName.Equals("HELP"))
                        vo.Help = dr[i].ToString();
                    else if (columnName.Equals("CALLOUT"))
                    {

                        vo.Callout = dr[i].ToString();

                        if (!string.IsNullOrEmpty(vo.Callout))
                        {

                            Tuple<string, string, string> tpl = null;

                            StringTokenizer st = new StringTokenizer(vo.Callout, ";,", false);
                            StringBuilder callouts = new StringBuilder();

                            bool hasModulePrefix = Env.HasModulePrefix(vo.ColumnName, out tpl);


                            while (st.HasMoreTokens())      //  for each callout
                            {
                                string prefix = "";

                                String cmd = st.NextToken().Trim();
                                if (hasModulePrefix)
                                {
                                    prefix = vo.ColumnName.Substring(0, vo.ColumnName.IndexOf('_'));
                                }
                                else
                                {
                                    String className = cmd.Substring(0, cmd.LastIndexOf("."));
                                    className = className.Remove(0, className.LastIndexOf(".") + 1);

                                    if (Env.HasModulePrefix(className, out tpl))
                                    {
                                        prefix = className.Substring(0, className.IndexOf('_'));
                                    }
                                }

                                if (callouts.Length > 0)
                                {
                                    if (prefix.Length > 0)
                                        callouts.Append(";").Append(cmd.Replace("ViennaAdvantage", prefix));
                                    else
                                    {
                                        callouts.Append(";").Append(cmd);
                                    }
                                }
                                else
                                {
                                    if (prefix.Length > 0)
                                        callouts.Append(cmd.Replace("ViennaAdvantage", prefix));
                                    else
                                    {
                                        callouts.Append(";").Append(cmd);
                                    }
                                }
                            }
                            vo.Callout = callouts.ToString();
                        }
                    }
                    else if (columnName.Equals("AD_PROCESS_ID"))
                        vo.AD_Process_ID = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("AD_FORM_ID"))
                        vo.AD_Form_ID = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("READONLYLOGIC"))
                        vo.ReadOnlyLogic = dr[i].ToString();
                    else if (columnName.Equals("MANDATORYLOGIC"))
                        vo.mandatoryLogic = dr[i].ToString();
                    else if (columnName.Equals("OBSCURETYPE"))
                        vo.ObscureType = dr[i].ToString();
                    else if (columnName.Equals("ISDEFAULTFOCUS"))
                        vo.IsDefaultFocus = "Y".Equals(dr[i].ToString());
                    //
                    else if (columnName.Equals("AD_REFERENCE_VALUE_ID"))
                        vo.AD_Reference_Value_ID = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("VALIDATIONCODE"))
                        vo.ValidationCode = dr[i].ToString();
                    else if (columnName.Equals("COLUMNSQL"))
                        vo.ColumnSQL = dr[i].ToString();
                    else if (columnName.Equals("AD_FIELD_ID"))
                        vo.AD_Field_ID = Utility.Util.GetValueOfInt(dr[i]);
                    else if (columnName.Equals("MOBILELISTINGFORMAT"))
                        vo.MobileListingFormat = Utility.Util.GetValueOfString(dr[i]);
                    else if (columnName.Equals("MRSEQNO"))
                    {
                        if (dr[i] != null && dr[i] != DBNull.Value)
                        {
                            int mrseq = Util.GetValueOfInt(dr[i]);
                            if (mrseq >= 0)
                            {
                                vo.mrSeqNo = mrseq;
                            }
                        }
                    }

                    else if (columnName.Equals("ZOOMWINDOW_ID"))
                    {
                        vo.ZoomWindow_ID = Util.GetValueOfInt(dr[i]);
                    }

                    else if (columnName.Equals("ISLINK"))
                    {
                        vo.isLink = "Y".Equals(Util.GetValueOfString(dr[i]));
                    }

                    else if (columnName.Equals("ISRIGHTPANELINK"))
                    {
                        vo.isRightPaneLink = "Y".Equals(Util.GetValueOfString(dr[i]));
                    }

                    else if (columnName.Equals("ISCOPY"))
                    {
                        vo.IsCopy = "Y".Equals(Util.GetValueOfString(dr[i]));
                    }
                    else if (columnName.Equals("COLUMNWIDTH"))
                    {
                        vo.ColumnWidth = Util.GetValueOfInt(dr[i]);
                    }
                    else if (columnName.Equals("ISBACKGROUNDPROCESS"))
                    {
                        vo.IsBackgroundProcess = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("ASKUSERBGPROCESS"))
                    {
                        vo.AskUserBGProcess = "Y".Equals(dr[i].ToString());
                    }

                    else if (columnName.Equals("ISIDENTIFIER"))
                    {
                        vo.IsIdentifier = "Y".Equals(dr[i].ToString());
                    }
                    /******************************/
                    else if (columnName.Equals("Isheaderpanelitem", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.IsHeaderPanelitem = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("Headeroverridereference", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.HeaderOverrideReference = Utility.Util.GetValueOfInt(dr[i]);
                    }
                    else if (columnName.Equals("HeaderStyle", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.HeaderStyle = dr[i].ToString();
                    }
                    else if (columnName.Equals("HeaderHeadingOnly", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.HeaderHeadingOnly = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("HeaderSeqno", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.HeaderSeqno = Utility.Util.GetValueOfDecimal(dr[i]);
                    }
                    else if (columnName.Equals("HeaderIconOnly", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.HeaderIconOnly = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("HtmlStyle", StringComparison.OrdinalIgnoreCase))
                    {
                        string htmlStyle = dr[i].ToString();
                        if (htmlStyle != null && htmlStyle.Length > 0 && htmlStyle.IndexOf("@") > -1)
                        {
                            string[] stylearr = htmlStyle.Split('|');

                            if (stylearr != null && stylearr.Length > 0)
                            {
                                for (int m = 0; m < stylearr.Length; m++)
                                {
                                    if (stylearr[m].IndexOf("@img::") > -1)
                                    {
                                        vo.GridImageStyle = stylearr[m].Replace("@img::", "");
                                    }
                                    else if (stylearr[m].IndexOf("@value::") > -1)
                                    {
                                        vo.HtmlStyle = stylearr[m].Replace("@value::", "");
                                    }
                                }
                            }

                        }
                        else
                        {
                            vo.HtmlStyle = htmlStyle;
                        }
                    }
                    else if (columnName.Equals("ShowIcon", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.ShowIcon = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("AD_Image_ID", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.AD_Image_ID = Utility.Util.GetValueOfInt(dr[i]);
                    }
                    else if (columnName.Equals("PlaceHolder", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.PlaceHolder = Utility.Util.GetValueOfString(dr[i]);
                    }
                    else if (columnName.Equals("FontName", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.FontClass = Utility.Util.GetValueOfString(dr[i]);
                    }
                    else if (columnName.Equals("ImageUrl", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.ImageName = Utility.Util.GetValueOfString(dr[i]);
                        if (vo.ImageName != "" && vo.ImageName.Contains("/"))
                        {
                            vo.ImageName = vo.ImageName.Substring(vo.ImageName.LastIndexOf("/") + 1);
                        }
                    }
                    // new column added for maintain versions
                    else if (columnName.Equals("ISMAINTAINVERSIONS", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.IsMaintainVersions = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("CellSpace", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.CellSpace = Utility.Util.GetValueOfInt(dr[i]);
                    }
                    else if (columnName.Equals("FieldBreadth", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.FieldBreadth = Utility.Util.GetValueOfInt(dr[i]);
                    }
                    else if (columnName.Equals("IsLineBreak", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.LineBreak = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("FieldGroupDefault", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.FieldGroupDefault = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("ShowFilterOption", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.ShowFilterOption = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("IsUnique", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.IsUnique = "Y".Equals(dr[i].ToString());
                    }
                    else if (columnName.Equals("isSwitch", StringComparison.OrdinalIgnoreCase))
                    {
                        vo.IsSwitch = "Y".Equals(dr[i].ToString());
                    }

                }
                if (vo.Header == null)
                    vo.Header = vo.ColumnName;
            }
            catch (Exception e)
            {
                VLogger.Get().Log(Level.SEVERE, "ColumnName=" + columnName, e);
                return null;
            }
            vo.InitFinish();
            return vo;
        }

        /// <summary>
        /// Init Field for Process Parameter
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="WindowNo">window number</param>
        /// <param name="dr">dataRow</param>
        /// <returns>this object</returns>
        public static GridFieldVO CreateParameter(Ctx ctx, int windowNo, IDataReader dr)
        {
            GridFieldVO vo = new GridFieldVO(ctx, windowNo, 0, 0, 0, false);
            vo.isProcess = true;
            vo.IsDisplayedf = true;
            vo.IsDisplayedMR = true;
            vo.IsReadOnly = false;
            vo.IsUpdateable = true;

            try
            {
                vo.AD_Table_ID = 0;
                vo.AD_Column_ID = Utility.Util.GetValueOfInt(dr["AD_Process_Para_ID"]);	//	**
                vo.ColumnName = dr["ColumnName"].ToString();
                vo.Header = dr["Name"].ToString();
                vo.Description = Utility.Util.GetValueOfString(dr["Description"]);
                vo.Help = Utility.Util.GetValueOfString(dr["Help"]);
                vo.displayType = Utility.Util.GetValueOfInt(dr["AD_Reference_ID"]);
                vo.AD_Reference_ID = Utility.Util.GetValueOfInt(dr["AD_Reference_ID"]);
                vo.IsMandatoryUI = Utility.Util.GetValueOfString(dr["IsMandatoryUI"]).Equals("Y");
                vo.FieldLength = Utility.Util.GetValueOfInt(dr["FieldLength"]);
                vo.DisplayLength = vo.FieldLength;
                vo.DefaultValue = Utility.Util.GetValueOfString(dr["DefaultValue"]);
                vo.DefaultValue2 = Utility.Util.GetValueOfString(dr["DefaultValue2"]);
                vo.VFormat = Utility.Util.GetValueOfString(dr["VFormat"]);
                vo.ValueMin = Utility.Util.GetValueOfString(dr["ValueMin"]);
                vo.ValueMax = Utility.Util.GetValueOfString(dr["ValueMax"]);
                vo.isRange = Utility.Util.GetValueOfString(dr["IsRange"]).Equals("Y");
                //
                vo.AD_Reference_Value_ID = Utility.Util.GetValueOfInt(dr["AD_Reference_Value_ID"]);
                vo.ValidationCode = Utility.Util.GetValueOfString(dr["ValidationCode"]);
                vo.AD_InfoWindow_ID = Util.GetValueOfInt(dr["AD_INFOWINDOW_ID"]);
            }
            catch (System.Exception e)
            {
                VLogger.Get().Log(Level.SEVERE, "createParameter", e);
            }
            //
            vo.InitFinish();
            if (vo.DefaultValue2 == null)
                vo.DefaultValue2 = "";
            return vo;
        }

        /// <summary>
        /// Create range "to" Parameter Field from "from" Parameter Field
        /// </summary>
        /// <param name="voF">object</param>
        /// <returns>this object</returns>
        public static GridFieldVO CreateCrystalParameter(GridFieldVO voF)
        {
            GridFieldVO voT = new GridFieldVO(voF.ctx, voF.windowNo, voF.tabNo,
                voF.AD_Window_ID, voF.AD_Tab_ID, voF.tabReadOnly);
            voT.isProcess = true;
            voT.IsDisplayedf = true;
            voT.IsReadOnly = false;
            voT.IsUpdateable = true;
            //
            voT.AD_Table_ID = voF.AD_Table_ID;
            voT.AD_Column_ID = voF.AD_Column_ID;    //  AD_Process_Para_ID
            voT.ColumnName = voF.ColumnName;
            voT.Header = voF.Header;
            voT.Description = voF.Description;
            //voT.Help = voF.Help;
            voT.displayType = voF.displayType;
            voT.IsMandatoryUI = voF.IsMandatoryUI;
            voT.FieldLength = voF.FieldLength;
            //voT.DisplayLength = voF.FieldLength;
            //voT.DefaultValue = voF.DefaultValue2;
            //voT.VFormat = voF.VFormat;
            //voT.ValueMin = voF.ValueMin;
            //voT.ValueMax = voF.ValueMax;
            voT.isRange = voF.isRange;
            //
            return voT;
        }   //  createParameter

        /// <summary>
        /// Create range "to" Parameter Field from "from" Parameter Field
        /// </summary>
        /// <param name="voF">object</param>
        /// <returns>this object</returns>
        public static GridFieldVO CreateParameter(GridFieldVO voF)
        {
            GridFieldVO voT = new GridFieldVO(voF.ctx, voF.windowNo, voF.tabNo,
                voF.AD_Window_ID, voF.AD_Tab_ID, voF.tabReadOnly);
            voT.isProcess = true;
            voT.IsDisplayedf = true;
            voT.IsReadOnly = false;
            voT.IsUpdateable = true;
            //
            voT.AD_Table_ID = voF.AD_Table_ID;
            voT.AD_Column_ID = voF.AD_Column_ID;    //  AD_Process_Para_ID
            voT.ColumnName = voF.ColumnName;
            voT.Header = voF.Header;
            voT.Description = voF.Description;
            voT.Help = voF.Help;
            voT.displayType = voF.displayType;
            voT.IsMandatoryUI = voF.IsMandatoryUI;
            voT.FieldLength = voF.FieldLength;
            voT.DisplayLength = voF.FieldLength;
            voT.DefaultValue = voF.DefaultValue2;
            voT.VFormat = voF.VFormat;
            voT.ValueMin = voF.ValueMin;
            voT.ValueMax = voF.ValueMax;
            voT.isRange = voF.isRange;
            //
            return voT;
        }   //  createParameter


        /// <summary>
        /// Make a standard field (Created/Updated/By)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="windowNo">window number</param>
        /// <param name="tabNo">tab number</param>
        /// <param name="AD_Window_ID">Window_ID</param>
        /// <param name="AD_Tab_ID">Tab Id</param>
        /// <param name="tabReadOnly">tab readonly</param>
        /// <param name="isCreated">is prefic created column</param>
        /// <param name="isTimestamp">is prefix "update" column</param>
        /// <returns></returns>
        public static GridFieldVO CreateStdField(Ctx ctx, int windowNo, int tabNo,
            int AD_Window_ID, int AD_Tab_ID, bool tabReadOnly,
            bool isCreated, bool isTimestamp)
        {
            GridFieldVO vo = new GridFieldVO(ctx, windowNo, tabNo,
                AD_Window_ID, AD_Tab_ID, tabReadOnly);
            vo.ColumnName = isCreated ? "Created" : "Updated";
            if (!isTimestamp)
                vo.ColumnName += "By";
            vo.displayType = isTimestamp ? DisplayType.DateTime : DisplayType.Table;
            if (!isTimestamp)
                vo.AD_Reference_Value_ID = 110;		//	AD_User Table Reference
            vo.IsDisplayedf = false;
            vo.IsMandatoryUI = false;
            vo.IsReadOnly = false;
            vo.IsUpdateable = true;
            vo.InitFinish();
            return vo;
        }   //  initStdField


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx">Current Ctx of an application</param>
        /// <param name="iWindowId">window number</param>
        /// <param name="dr">Datarow contating resultset</param>
        /// <returns></returns>
        public static GridFieldVO CreateParameter(Ctx t_ctx, int iWindowId, System.Data.DataRow dr)
        {
            GridFieldVO vo = new GridFieldVO(t_ctx, iWindowId, 0, 0, 0, false);
            vo.isProcess = true;
            vo.IsDisplayedf = true;
            vo.IsReadOnly = false;
            vo.IsUpdateable = true;
            try
            {
                vo.AD_Table_ID = 0;
                vo.AD_Column_ID = int.Parse(dr["AD_Process_Para_ID"].ToString());
                vo.ColumnName = dr["COLUMNNAME"].ToString();
                vo.Header = dr["Name"].ToString();
                vo.name = dr["Name"].ToString();
                vo.Description = dr["Description"].ToString();
                vo.Help = dr["Help"].ToString();
                vo.displayType = int.Parse(dr["AD_Reference_ID"].ToString());
                vo.AD_Reference_ID = int.Parse(dr["AD_Reference_ID"].ToString());
                vo.IsMandatoryUI = dr["IsMandatoryUI"].ToString() == "Y" ? true : false;
                vo.FieldLength = int.Parse(dr["FIELDLENGTH"].ToString());
                vo.DisplayLength = vo.FieldLength;
                vo.DefaultValue = dr["DefaultValue"].ToString();
                vo.DefaultValue2 = dr["DEFAULTVALUE2"].ToString();
                vo.ValueMin = dr["VALUEMIN"].ToString();
                vo.ValueMax = dr["VALUEMAX"].ToString();
                vo.isRange = dr["ISRANGE"].ToString() == "Y";
                vo.AD_Reference_Value_ID = int.Parse((dr["AD_REFERENCE_VALUE_ID"].ToString() == "") ? "0" : dr["AD_REFERENCE_VALUE_ID"].ToString());
                vo.ValidationCode = dr["VALIDATIONCODE"].ToString();
                vo.AD_InfoWindow_ID = Util.GetValueOfInt(dr["AD_INFOWINDOW_ID"]);
                vo.LoadRecursiveData = dr["LoadRecursiveData"].ToString() == "Y" ? true : false;
                vo.ShowChildOfSelected = dr["ShowChildOfSelected"].ToString() == "Y" ? true : false;

                //Check If parameter is marked encrypted or not.
                vo.IsEncryptedField = dr["IsEncrypted"].ToString() == "Y" ? true : false;
                //Set Zoom Window
                vo.ZoomWindow_ID = Util.GetValueOfInt(dr["ZoomWindow_ID"]);
                dr.Delete();

            }
            catch
            {
            }

            vo.InitFinish();
            return vo;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx">Current Ctx of an application</param>
        /// <param name="iWindowId">window number</param>
        /// <param name="dr">Datarow contating resultset</param>
        /// <returns></returns>
        public static GridFieldVO CreateCrystalParameter(Ctx t_ctx, int iWindowId, System.Data.DataRow dr)
        {
            GridFieldVO vo = new GridFieldVO(t_ctx, iWindowId, 0, 0, 0, false);
            vo.isProcess = true;
            vo.IsDisplayedf = true;
            vo.IsReadOnly = false;
            vo.IsUpdateable = true;
            try
            {
                vo.AD_Table_ID = 0;
                vo.AD_Column_ID = int.Parse(dr["AD_CrystalReport_Para_ID"].ToString());
                vo.ColumnName = dr["COLUMNNAME"].ToString();
                vo.Header = dr["Name"].ToString();
                vo.name = dr["Name"].ToString();
                vo.Description = dr["Description"].ToString();
                vo.displayType = int.Parse(dr["AD_Reference_ID"].ToString());
                vo.AD_Reference_ID = int.Parse(dr["AD_Reference_ID"].ToString());
                vo.IsMandatoryUI = dr["IsMandatoryUI"].ToString() == "Y" ? true : false;
                vo.FieldLength = int.Parse(dr["FIELDLENGTH"].ToString());
                vo.DisplayLength = vo.FieldLength;
                vo.DefaultValue = dr["DefaultValue"].ToString();
                //vo.DefaultValue2 = dr["DEFAULTVALUE2"].ToString();
                //vo.ValueMin = dr["VALUEMIN"].ToString();
                //vo.ValueMax = dr["VALUEMAX"].ToString();
                vo.isRange = dr["ISRANGE"].ToString() == "Y";
                vo.AD_Reference_Value_ID = int.Parse((dr["AD_REFERENCE_VALUE_ID"].ToString() == "") ? "0" : dr["AD_REFERENCE_VALUE_ID"].ToString());
                vo.ValidationCode = dr["VALIDATIONCODE"].ToString();
                vo.AD_InfoWindow_ID = Util.GetValueOfInt(dr["AD_INFOWINDOW_ID"]);
                dr.Delete();

            }
            catch
            {
            }

            vo.InitFinish();
            return vo;

        }



        /// <summary>
        ///  *  Private constructor.
        /// </summary>
        /// <param name="newCtx">context</param>
        /// <param name="windowNo">windoe number</param>
        /// <param name="tabNo">tab number</param>
        /// <param name="ad_Window_ID">windoe _id</param>
        /// <param name="ad_Tab_ID">tab id</param>
        /// <param name="TabReadOnly">ia tab readonly</param>
        public GridFieldVO(Ctx newCtx, int windowNm, int tabNm,
            int ad_Window_ID, int ad_Tab_ID, bool TabReadOnly)
        {
            ctx = newCtx;
            windowNo = windowNm;
            tabNo = tabNm;
            AD_Window_ID = ad_Window_ID;
            AD_Tab_ID = ad_Tab_ID;
            tabReadOnly = TabReadOnly;
        }   //  MFieldVO

        /// <summary>
        /// copy const
        /// </summary>
        /// <param name="newCtx">context</param>
        /// <param name="f">object</param>
        public GridFieldVO(Ctx newCtx, FieldVObj f)
        {
            GridFieldVO vo = this;
            this.ctx = newCtx;
            vo.isProcess = true;
            vo.IsDisplayedf = true;
            vo.IsReadOnly = false;
            vo.IsUpdateable = true;

            vo.AD_Table_ID = 0;
            vo.AD_Column_ID = 0;
            vo.ColumnName = f.ColumnName;
            vo.Header = "";
            vo.Description = f.Description;
            vo.Help = f.Help;
            vo.displayType = f.displayType;
            vo.IsMandatoryUI = f.IsMandatoryUI;
            vo.FieldLength = f.FieldLength;
            vo.DisplayLength = f.DisplayLength;
            vo.DefaultValue = f.DefaultValue;
            vo.DefaultValue2 = f.DefaultValue2;
            vo.VFormat = f.VFormat;
            vo.VFormatError = f.VFormatError;
            vo.ValueMin = f.ValueMin;
            vo.ValueMax = f.ValueMax;
            vo.isRange = f.isRange;
            //
            vo.AD_Reference_Value_ID = f.AD_Reference_Value_ID;
            vo.ValidationCode = f.ValidationCode;
            //
            vo.InitFinish();
            if (vo.DefaultValue2 == null)
                vo.DefaultValue2 = "";
        }

        //static long serialVersionUID = 4385061125114436797L;



        /// <summary>
        /// Set Ctx including contained elements
        /// </summary>
        /// <param name="newCtx"></param>
        public void SetCtx(Ctx newCtx)
        {
            ctx = newCtx;
        }

        /// <summary>
        ///Validate Fields and create LookupInfo if required
        /// </summary>
        public void InitFinish()
        {
            //  Not null fields
            if (DisplayLogic == null)
                DisplayLogic = "";
            if (DefaultValue == null)
                DefaultValue = "";
            if (FieldGroup == null)
                FieldGroup = "";
            if (Description == null)
                Description = "";
            if (Help == null)
                Help = "";
            if (Callout == null)
                Callout = "";
            if (ReadOnlyLogic == null)
                ReadOnlyLogic = "";
            if (StyleLogic == null)
                StyleLogic = "";


            //  Create Lookup, if not ID

            if (ctx.SkipLookup) //No need if call from Visual editor
            {
                return;
            }
            if (DisplayType.IsLookup(displayType))
            {
                if (IsDisplayedf || IsDisplayedMR || ColumnName.ToLower().Equals("createdby") || ColumnName.ToLower().Equals("updatedby")
                    || IsHeaderPanelitem)
                {
                    try
                    {
                        lookupInfo = VLookUpFactory.GetLookUpInfo(ctx, windowNo, displayType,
                            AD_Column_ID, Env.GetLanguage(ctx), ColumnName, AD_Reference_Value_ID,
                            IsParent, ValidationCode);
                    }
                    catch (Exception e)     //  Cannot create Lookup
                    {
                        VLogger.Get().Log(Level.SEVERE, "No LookupInfo for " + ColumnName, e);
                        displayType = DisplayType.ID;
                    }
                }
                else
                {
                    displayType = DisplayType.ID;
                }
            }

        }

        /// <summary>
        ///Clone Field object
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="windowNo">window number</param>
        /// <param name="tabNo">tab number</param>
        /// <param name="ad_Window_ID">window_id</param>
        /// <param name="ad_Tab_ID">tab_ id</param>
        /// <param name="TabReadOnly">is tabreadonly</param>
        /// <returns>this object</returns>
        public GridFieldVO Clone(Ctx ctx, int windowNo, int tabNo,
            int ad_Window_ID, int ad_Tab_ID,
            bool TabReadOnly)
        {
            GridFieldVO clone = new GridFieldVO(ctx, windowNo, tabNo,
            ad_Window_ID, ad_Tab_ID, TabReadOnly);

            clone.isProcess = false;
            //  Database Fields
            clone.ColumnName = ColumnName;
            clone.ColumnSQL = ColumnSQL;
            clone.Header = Header;
            clone.displayType = displayType;
            clone.AD_Table_ID = AD_Table_ID;
            clone.AD_Column_ID = AD_Column_ID;
            clone.DisplayLength = DisplayLength;
            clone.IsSameLine = IsSameLine;
            clone.IsDisplayedf = IsDisplayedf;
            clone.IsDisplayedMR = IsDisplayedMR;
            clone.DisplayLogic = DisplayLogic;
            clone.StyleLogic = StyleLogic;
            clone.DefaultValue = DefaultValue;
            clone.IsMandatoryUI = IsMandatoryUI;
            clone.IsReadOnly = IsReadOnly;
            clone.IsUpdateable = IsUpdateable;
            clone.IsAlwaysUpdateable = IsAlwaysUpdateable;
            clone.IsHeading = IsHeading;
            clone.IsFieldOnly = IsFieldOnly;
            clone.IsEncryptedField = IsEncryptedField;
            clone.IsEncryptedColumn = IsEncryptedColumn;
            clone.IsSelectionColumn = IsSelectionColumn;
            //clone.IsIncludedColumn = IsIncludedColumn;
            clone.SelectionSeqNo = SelectionSeqNo;
            clone.SortNo = SortNo;
            clone.FieldLength = FieldLength;
            clone.VFormat = VFormat;
            clone.VFormatError = VFormatError;
            clone.ValueMin = ValueMin;
            clone.ValueMax = ValueMax;
            clone.FieldGroup = FieldGroup;
            clone.IsKey = IsKey;
            clone.IsParent = IsParent;
            clone.Callout = Callout;
            clone.AD_Process_ID = AD_Process_ID;
            clone.Description = Description;
            clone.Help = Help;
            clone.ReadOnlyLogic = ReadOnlyLogic;
            clone.ObscureType = ObscureType;
            clone.IsDefaultFocus = IsDefaultFocus;
            //	Lookup
            clone.ValidationCode = ValidationCode;
            clone.AD_Reference_Value_ID = AD_Reference_Value_ID;
            clone.lookupInfo = lookupInfo;

            //  Process Parameter
            clone.isRange = isRange;
            clone.DefaultValue2 = DefaultValue2;


            //Lakhwinder
            clone.AD_Field_ID = AD_Field_ID;
            clone.AD_InfoWindow_ID = AD_InfoWindow_ID;

            //Harwinder
            clone.MobileListingFormat = MobileListingFormat;
            clone.mrSeqNo = mrSeqNo;
            clone.ZoomWindow_ID = ZoomWindow_ID;
            clone.isLink = isLink;
            clone.isRightPaneLink = isRightPaneLink;
            clone.IsCopy = IsCopy;
            clone.ColumnWidth = ColumnWidth;

            clone.AD_Form_ID = AD_Form_ID;
            clone.IsBackgroundProcess = IsBackgroundProcess;
            clone.AskUserBGProcess = AskUserBGProcess;
            clone.IsHeaderPanelitem = IsHeaderPanelitem;
            clone.HeaderOverrideReference = HeaderOverrideReference;
            clone.HeaderStyle = HeaderStyle;
            clone.HeaderHeadingOnly = HeaderHeadingOnly;
            clone.HeaderSeqno = HeaderSeqno;
            clone.HeaderIconOnly = HeaderIconOnly;
            clone.HtmlStyle = HtmlStyle;
            clone.ShowIcon = ShowIcon;
            clone.AD_Image_ID = AD_Image_ID;
            clone.FontClass = FontClass;
            clone.ImageName = ImageName;
            clone.IsMaintainVersions = IsMaintainVersions;
            clone.CellSpace = CellSpace;
            clone.FieldBreadth = FieldBreadth;
            clone.LineBreak = LineBreak;
            clone.FieldGroupDefault = FieldGroupDefault;
            clone.ShowFilterOption = ShowFilterOption;
            clone.IsUnique = IsUnique;
            clone.IsSwitch = IsSwitch;
            clone.IsIdentifier = IsIdentifier;
            clone.GridImageStyle = GridImageStyle;
            return clone;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MFieldVO[");
            sb.Append(AD_Column_ID).Append("-").Append(ColumnName)
                .Append("]");
            return sb.ToString();
        }


        public GridFieldVO(String columnName, String name, int displayType)
        {
            this.ColumnName = columnName;
            this.name = name.Replace("&", "");
            this.displayType = displayType;
            this.IsDisplayedf = true;
        }

        /// <summary>
        /// Evalutee inteface
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public String GetValueAsString(String variableName)
        {
            return ctx.GetContext(windowNo, variableName, true);
        }



    }
}

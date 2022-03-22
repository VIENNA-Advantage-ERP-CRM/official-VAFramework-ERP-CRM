/********************************************************
 * Module Name    : Report
 * Purpose        : Launch Report
 * Author         : Jagmohan Bhatt
 * Date           : 02-June-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Login;
using VAdvantage.Logging;

namespace VAdvantage.Print
{
    public class MPrintFormatItem : X_AD_PrintFormatItem
    {
        private static VLogger s_log = VLogger.GetVLogger(typeof(MPrintFormatItem).FullName);
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintFormatItem_ID">AD_PrintFormatItem_ID</param>
        /// <param name="trxName">transaction</param>
        public MPrintFormatItem(Ctx ctx, int AD_PrintFormatItem_ID, Trx trxName)
            : base(ctx, AD_PrintFormatItem_ID, trxName)
        {

            //	Default Setting
            if (AD_PrintFormatItem_ID == 0)
            {
                SetFieldAlignmentType(FIELDALIGNMENTTYPE_Default);
                SetLineAlignmentType(LINEALIGNMENTTYPE_None);
                SetPrintFormatType(PRINTFORMATTYPE_Text);
                SetPrintAreaType(PRINTAREATYPE_Content);
                SetShapeType(SHAPETYPE_NormalRectangle);
                //
                SetIsCentrallyMaintained(true);
                SetIsRelativePosition(true);
                SetIsNextLine(false);
                SetIsNextPage(false);
                SetIsSetNLPosition(false);
                SetIsFilledRectangle(false);
                SetIsImageField(false);
                SetXSpace(0);
                SetYSpace(0);
                SetXPosition(0);
                SetYPosition(0);
                SetMaxWidth(0);
                SetIsFixedWidth(false);
                SetIsHeightOneLine(false);
                SetMaxHeight(0);
                SetLineWidth(1);
                SetArcDiameter(0);
                //
                SetIsOrderBy(false);
                SetSortNo(0);
                SetIsGroupBy(false);
                SetIsPageBreak(false);
                SetIsSummarized(false);
                SetIsAveraged(false);
                SetIsCounted(false);
                SetIsMinCalc(false);
                SetIsMaxCalc(false);
                SetIsVarianceCalc(false);
                SetIsDeviationCalc(false);
                SetIsRunningTotal(false);
                SetImageIsAttached(false);
                SetIsSuppressNull(false);
            }
        }	//	MPrintFormatItem

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">Datarow</param>
        /// <param name="trxName">transaction</param>
        public MPrintFormatItem(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MPrintFormatItem

        /**	Locally cached column name			*/
        private String _columnName = null;
        /** Locally cached label translations			*/
        private Dictionary<String, String> _translationLabel;
        /** Locally cached suffix translations			*/
        private Dictionary<String, String> _translationSuffix;

        /// <summary>
        /// Get print name with language
        /// </summary>
        /// <param name="language">language - ignored if IsMultiLingualDocument not 'Y'</param>
        /// <returns>print name</returns>
        public String GetPrintName(Language language)
        {
            if (language == null || Env.IsBaseLanguage(language, "AD_PrintFormatItem"))
                return GetPrintName();
            LoadTranslations();

            String retValue = "";
            if (_translationLabel.ContainsKey(language.GetAD_Language()))
                retValue = (String)_translationLabel[language.GetAD_Language()];
            if (string.IsNullOrEmpty(retValue))
                return GetPrintName();
            return retValue;
        }	//	getPrintName

        /// <summary>
        /// Get print name suffix with language
        /// </summary>
        /// <param name="language">language - ignored if IsMultiLingualDocument not 'Y'</param>
        /// <returns>print name suffix</returns>
        public String GetPrintNameSuffix(Language language)
        {
            if (language == null || Env.IsBaseLanguage(language, "AD_PrintFormatItem"))// GlobalVariable.IsBaseLanguage())
                return GetPrintNameSuffix();
            LoadTranslations();
            // String retValue = (String)_translationSuffix[language.GetAD_Language()];
            String retValue = Utility.Util.GetValueOfString(_translationSuffix[language.GetAD_Language()]);
            if (retValue == null || retValue.Length == 0)
                return GetPrintNameSuffix();
            return retValue;
        }	//	getPrintNameSuffix


        /// <summary>
        /// Load Translations
        /// </summary>
        private void LoadTranslations()
        {
            if (_translationLabel == null)
            {
                _translationLabel = new Dictionary<String, String>();
                _translationSuffix = new Dictionary<String, String>();
                String sql = "SELECT AD_Language, PrintName, PrintNameSuffix FROM AD_PrintFormatItem_Trl WHERE AD_PrintFormatItem_ID=@formatitem";
                IDataReader dr = null;
                try
                {
                    SqlParameter[] param = new SqlParameter[1];
                    param[0] = new SqlParameter("@formatitem", Get_ID());
                    dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                    while (dr.Read())
                    {
                        _translationLabel.Add(dr[0].ToString(), dr[1].ToString());
                        _translationSuffix.Add(dr[0].ToString(), dr[2].ToString());
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    log.Log(Level.SEVERE, "loadTrl", e);
                }
            }
        }	//	loadTranslations



        /// <summary>
        /// Type field
        /// </summary>
        /// <returns>true, if type field</returns>
        public Boolean IsTypeField()
        {
            return GetPrintFormatType().Equals(PRINTFORMATTYPE_Field);
        }

        /// <summary>
        /// Type Text
        /// </summary>
        /// <returns>true, if type text</returns>
        public Boolean IsTypeText()
        {
            return GetPrintFormatType().Equals(PRINTFORMATTYPE_Text);
        }

        /// <summary>
        /// Type PrintFormat
        /// </summary>
        /// <returns>true if printformat</returns>
        public Boolean IsTypePrintFormat()
        {
            return GetPrintFormatType().Equals(PRINTFORMATTYPE_PrintFormat);
        }

        /// <summary>
        /// Type Image
        /// </summary>
        /// <returns>true, if image</returns>
        public Boolean IsTypeImage()
        {
            return GetPrintFormatType().Equals(PRINTFORMATTYPE_Image);
        }

        /// <summary>
        /// Type Box
        /// </summary>
        /// <returns>true if box</returns>
        public Boolean IsTypeBox()
        {
            return GetPrintFormatType().Equals(PRINTFORMATTYPE_Line)
                || GetPrintFormatType().Equals(PRINTFORMATTYPE_Rectangle);
        }


        /// <summary>
        /// Field center
        /// </summary>
        /// <returns>true, if center</returns>
        public Boolean IsFieldCenter()
        {
            return GetFieldAlignmentType().Equals(FIELDALIGNMENTTYPE_Center);
        }

        /// <summary>
        /// Field Align Leading
        /// </summary>
        /// <returns>true if leading</returns>
        public Boolean IsFieldAlignLeading()
        {
            return GetFieldAlignmentType().Equals(FIELDALIGNMENTTYPE_LeadingLeft);
        }

        /// <summary>
        /// Field Align Trailing
        /// </summary>
        /// <returns>true if trailing</returns>
        public Boolean IsFieldAlignTrailing()
        {
            return GetFieldAlignmentType().Equals(FIELDALIGNMENTTYPE_TrailingRight);
        }

        /// <summary>
        /// Field Align Block
        /// </summary>
        /// <returns>true if Block</returns>
        public Boolean IsFieldAlignBlock()
        {
            return GetFieldAlignmentType().Equals(FIELDALIGNMENTTYPE_Block);
        }

        /// <summary>
        /// Field Align Block
        /// </summary>
        /// <returns>true if block</returns>
        public Boolean IsFieldAlignDefault()
        {
            return GetFieldAlignmentType().Equals(FIELDALIGNMENTTYPE_Default);
        }


        /// <summary>
        /// Line Align Center
        /// </summary>
        /// <returns>true if center</returns>
        public Boolean IsLineAlignCenter()
        {
            return GetLineAlignmentType().Equals(LINEALIGNMENTTYPE_Center);
        }

        /// <summary>
        /// Line Align Leading
        /// </summary>
        /// <returns>true if leading</returns>
        public Boolean IsLineAlignLeading()
        {
            return GetLineAlignmentType().Equals(LINEALIGNMENTTYPE_LeadingLeft);
        }

        /// <summary>
        /// Line Align Trailing
        /// </summary>
        /// <returns>true if trailing</returns>
        public Boolean IsLineAlignTrailing()
        {
            return GetLineAlignmentType().Equals(LINEALIGNMENTTYPE_TrailingRight);
        }

        /// <summary>
        /// Header
        /// </summary>
        /// <returns>true if area is header</returns>
        public Boolean IsHeader()
        {
            return GetPrintAreaType().Equals(PRINTAREATYPE_Header);
        }

        /// <summary>
        /// Content
        /// </summary>
        /// <returns>true if area is centent</returns>
        public Boolean IsContent()
        {
            return GetPrintAreaType().Equals(PRINTAREATYPE_Content);
        }

        /// <summary>
        /// Footer
        /// </summary>
        /// <returns>true if area is footer</returns>
        public Boolean IsFooter()
        {
            return GetPrintAreaType().Equals(PRINTAREATYPE_Footer);
        }

        /// <summary>
        /// Barcode
        /// </summary>
        /// <returns>true if barcode selected</returns>
        public Boolean IsBarcode()
        {
            String s = GetBarcodeType();
            return s != null && s.Length > 0;
        }

        /**	Lookup Map of AD_Column_ID for ColumnName	*/
        private static CCache<int, String> _columns = new CCache<int, String>("AD_PrintFormatItem", 200);

        /// <summary>
        /// Get ColumnName from AD_Column_ID
        /// </summary>
        /// <returns>ColumnName</returns>
        public String GetColumnName()
        {
            if (_columnName == null)	//	Get Column Name from AD_Column not index
                _columnName = GetColumnName(GetAD_Column_ID());
            return _columnName;
        }	//	getColumnName

        /// <summary>
        /// Get Column Name from AD_Column_ID.
        /// Be careful not to confuse it with PO method getAD_Column_ID (index)
        /// </summary>
        /// <param name="AD_Column_ID">AD_Column_ID column</param>
        /// <returns>Column Name</returns>
        private static String GetColumnName(int AD_Column_ID)
        {
            if (AD_Column_ID == 0)
                return null;
            //
            String retValue = (String)_columns[AD_Column_ID];
            if (retValue == null)
            {
                String sql = "SELECT ColumnName FROM AD_Column WHERE AD_Column_ID=@param1";
                IDataReader dr = null;
                try
                {
                    SqlParameter[] param = new SqlParameter[1];
                    param[0] = new SqlParameter("@param1", AD_Column_ID);
                    dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                    while (dr.Read())
                    {
                        retValue = dr[0].ToString();
                        _columns.Add(AD_Column_ID, retValue);
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }

                    s_log.Log(Level.SEVERE, "AD_Column_ID=" + AD_Column_ID, e);
                }
            }
            return retValue;
        }	//	getColumnName


        /// <summary>
        /// Create Print Format Item from Column
        /// </summary>
        /// <param name="format">parent</param>
        /// <param name="AD_Column_ID">column</param>
        /// <param name="seqNo">sequence of display if 0 it is not printed</param>
        /// <returns>Format Item</returns>
        public static MPrintFormatItem CreateFromColumn(MPrintFormat format, int AD_Column_ID, int seqNo)
        {
            MPrintFormatItem pfi = new MPrintFormatItem(format.GetCtx(), 0, null);
            pfi.SetAD_PrintFormat_ID(format.GetAD_PrintFormat_ID());
            pfi.SetClientOrg(format);
            pfi.SetAD_Column_ID(AD_Column_ID);
            pfi.SetPrintFormatType(PRINTFORMATTYPE_Field);
            //SqlParameter[] param = null;

            //	translation is dome by trigger
            String sql = "SELECT c.ColumnName,e.Name,e.PrintName, "		//	1..3
                + "c.AD_Reference_ID,c.IsKey,c.SeqNo "					//	4..6
                + "FROM AD_Column c, AD_Element e "
                + "WHERE c.AD_Column_ID='" + AD_Column_ID + "'"
                + " AND c.AD_Element_ID=e.AD_Element_ID";
            //	translate base entry if single language - trigger copies to trl tables

            //Boolean trl = !Env.IsMultiLingualDocument(format.GetCtx()) && !GlobalVariable.IsBaseLanguage();
            Boolean trl = !Common.Common.IsMultiLingualDocument(format.GetCtx()) && !Language.IsBaseLanguage(Login.Language.GetBaseAD_Language());
            if (trl)
                sql = "SELECT c.ColumnName,e.Name,e.PrintName, "		//	1..3
                    + "c.AD_Reference_ID,c.IsKey,c.SeqNo "				//	4..6
                    + "FROM AD_Column c, AD_Element_Trl e "
                    + "WHERE c.AD_Column_ID='" + AD_Column_ID + "'"
                    + " AND c.AD_Element_ID=e.AD_Element_ID"
                    + " AND e.AD_Language='" + GlobalVariable.GetLanguageCode() + "'";
            IDataReader dr = null;
            try
            {
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                if (dr.Read())
                {
                    String ColumnName = dr[0].ToString();
                    pfi.SetName(dr[1].ToString());
                    pfi.SetPrintName(dr[2].ToString());
                    int displayType = dr[3].ToString() != "" ? Utility.Util.GetValueOfInt(dr[3].ToString()) : 0;
                    if (DisplayType.IsNumeric(displayType))
                        pfi.SetFieldAlignmentType(FIELDALIGNMENTTYPE_TrailingRight);
                    else if (displayType == DisplayType.Text || displayType == DisplayType.Memo)
                        pfi.SetFieldAlignmentType(FIELDALIGNMENTTYPE_Block);
                    else
                        pfi.SetFieldAlignmentType(FIELDALIGNMENTTYPE_LeadingLeft);
                    Boolean isKey = "Y".Equals(dr[4].ToString());
                    //
                    if (isKey
                        || ColumnName.StartsWith("Created") || ColumnName.StartsWith("Updated")
                        || ColumnName.Equals("AD_Client_ID") || ColumnName.Equals("AD_Org_ID")
                        || ColumnName.Equals("IsActive") || ColumnName.Equals("Export_ID")
                        || displayType == DisplayType.Button || displayType == DisplayType.Binary
                        || displayType == DisplayType.ID || displayType == DisplayType.Image
                        || displayType == DisplayType.RowID
                        || seqNo == 0)
                    {
                        pfi.SetIsPrinted(false);
                        pfi.SetSeqNo(0);
                    }
                    else
                    {
                        pfi.SetIsPrinted(true);
                        pfi.SetSeqNo(seqNo);
                    }
                    int idSeqNo = dr[5].ToString() != "" ? Utility.Util.GetValueOfInt(dr[5].ToString()) : 0;	//	IsIdentifier SortNo
                    if (idSeqNo > 0)
                    {
                        pfi.SetIsOrderBy(true);
                        pfi.SetSortNo(idSeqNo);
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                s_log.Severe(e.ToString());
            }
            if (!pfi.Save())
                return null;
            //	pfi.dump();
            return pfi;
        }	//	createFromColumn


        /// <summary>
        /// Create Print Format Item from Column
        /// </summary>
        /// <param name="format">parent</param>
        /// <param name="AD_Column_ID">column</param>
        /// <param name="seqNo">sequence of display if 0 it is not printed</param>
        /// <returns>Format Item</returns>
        public static MPrintFormatItem CreateFromColumn(MPrintFormat format, int AD_Column_ID, int AD_Field_ID, int seqNo, bool isMESeqDefined)
        {
            MPrintFormatItem pfi = new MPrintFormatItem(format.GetCtx(), 0, null);
            pfi.SetAD_PrintFormat_ID(format.GetAD_PrintFormat_ID());
            pfi.SetClientOrg(format);
            pfi.SetAD_Column_ID(AD_Column_ID);
            pfi.SetPrintFormatType(PRINTFORMATTYPE_Field);
            //SqlParameter[] param = null;

            //	translation is dome by trigger
            String sql = "SELECT c.ColumnName,e.Name,e.PrintName, "		//	1..3
                + "c.AD_Reference_ID,c.IsKey,c.SeqNo,  f.MRSeqNo,  f.MRIsDisplayed,f.IsDisplayed "					//	4..6
                + "FROM AD_Column c, AD_Element e ,AD_Field f "
                + "WHERE c.AD_Column_ID=" + AD_Column_ID + " AND f.AD_Field_ID="+AD_Field_ID+""
                + " AND c.AD_Element_ID=e.AD_Element_ID";
            //	translate base entry if single language - trigger copies to trl tables

            //Boolean trl = !Env.IsMultiLingualDocument(format.GetCtx()) && !GlobalVariable.IsBaseLanguage();
            Boolean trl = !Common.Common.IsMultiLingualDocument(format.GetCtx()) && !Language.IsBaseLanguage(Login.Language.GetBaseAD_Language());
            if (trl)
                sql = "SELECT c.ColumnName,e.Name,e.PrintName, "		//	1..3
                    + "c.AD_Reference_ID,c.IsKey,c.SeqNo , f.MRSeqNo,  f.MRIsDisplayed,f.IsDisplayed "				//	4..6
                    + "FROM AD_Column c, AD_Element_Trl e ,AD_Field f"
                    + "WHERE c.AD_Column_ID=" + AD_Column_ID + "  AND f.AD_Field_ID=" + AD_Field_ID + ""
                    + " AND c.AD_Element_ID=e.AD_Element_ID"
                    + " AND e.AD_Language='" + GlobalVariable.GetLanguageCode() + "'";
            IDataReader dr = null;
            try
            {
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                if (dr.Read())
                {
                    String ColumnName = dr[0].ToString();
                    pfi.SetName(dr[1].ToString());
                    pfi.SetPrintName(dr[2].ToString());
                    int displayType = dr[3].ToString() != "" ? Utility.Util.GetValueOfInt(dr[3].ToString()) : 0;
                    if (DisplayType.IsNumeric(displayType))
                        pfi.SetFieldAlignmentType(FIELDALIGNMENTTYPE_TrailingRight);
                    else if (displayType == DisplayType.Text || displayType == DisplayType.Memo)
                        pfi.SetFieldAlignmentType(FIELDALIGNMENTTYPE_Block);
                    else
                        pfi.SetFieldAlignmentType(FIELDALIGNMENTTYPE_LeadingLeft);
                    Boolean isKey = "Y".Equals(dr[4].ToString());
                    //
                    if (isKey
                        || ColumnName.StartsWith("Created") || ColumnName.StartsWith("Updated")
                        || ColumnName.Equals("AD_Client_ID") || ColumnName.Equals("AD_Org_ID")
                        || ColumnName.Equals("IsActive") || ColumnName.Equals("Export_ID")
                        || displayType == DisplayType.Button || displayType == DisplayType.Binary
                        || displayType == DisplayType.ID || displayType == DisplayType.Image
                        || displayType == DisplayType.RowID
                        || seqNo == 0)
                    {
                        pfi.SetIsPrinted(false);
                        pfi.SetSeqNo(0);
                    }
                    else if (isMESeqDefined)
                    {
                        if (dr[7].ToString() == "Y")
                        {
                            pfi.SetIsPrinted(true);
                        }
                        else
                        {
                            pfi.SetIsPrinted(false);
                        }
                        pfi.SetSeqNo(seqNo);
                    }
                    else
                    {
                        pfi.SetIsPrinted(true);
                        pfi.SetSeqNo(seqNo);
                    }


                    int idSeqNo = dr[5].ToString() != "" ? Utility.Util.GetValueOfInt(dr[5].ToString()) : 0;	//	IsIdentifier SortNo
                    if (idSeqNo > 0)
                    {
                        pfi.SetIsOrderBy(true);
                        pfi.SetSortNo(idSeqNo);
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                s_log.Severe(e.ToString());
            }
            if (!pfi.Save())
                return null;
            //	pfi.dump();
            return pfi;
        }	//	createFromColumn

        /// <summary>
        /// Copy existing Definition To Client
        /// </summary>
        /// <param name="To_Client_ID">to client</param>
        /// <param name="AD_PrintFormat_ID">parent print format</param>
        /// <returns>format item</returns>
        public MPrintFormatItem CopyToClient(int To_Client_ID, int AD_PrintFormat_ID)
        {
            MPrintFormatItem to = new MPrintFormatItem(Env.GetContext(), 0, null);
            MPrintFormatItem.CopyValues(this, to);
            to.SetClientOrg(To_Client_ID, 0);
            to.SetAD_PrintFormat_ID(AD_PrintFormat_ID);
            to.Save();
            return to;
        }	//	copyToClient

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>success</returns>
        protected override Boolean BeforeSave(bool newRecord)
        {
            //	Order
            if (!IsOrderBy())
            {
                SetSortNo(0);
                SetIsGroupBy(false);
                SetIsPageBreak(false);
            }
            //	Rel Position
            if (IsRelativePosition())
            {
                SetXPosition(0);
                SetYPosition(0);
            }
            else
            {
                SetXSpace(0);
                SetYSpace(0);
            }
            //	Image
            if (IsImageField())
            {
                SetImageIsAttached(false);
                SetImageURL(null);
            }
            return true;
        }	//	beforeSave


        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Set Translation from Element
            if (newRecord
                //	&& MClient.get(getCtx()).isMultiLingualDocument()
                && GetPrintName() != null && GetPrintName().Length > 0)
            {
                String sql = "UPDATE AD_PrintFormatItem_Trl "
                    + "SET PrintName = (SELECT e.PrintName "
                        + "FROM AD_Element_Trl e, AD_Column c "
                        + "WHERE e.AD_Language=AD_PrintFormatItem_Trl.AD_Language"
                        + " AND e.AD_Element_ID=c.AD_Element_ID"
                        + " AND c.AD_Column_ID=" + GetAD_Column_ID() + ") "
                    + "WHERE AD_PrintFormatItem_ID = " + Get_ID()
                    + " AND EXISTS (SELECT * "
                        + "FROM AD_Element_Trl e, AD_Column c "
                        + "WHERE e.AD_Language=AD_PrintFormatItem_Trl.AD_Language"
                        + " AND e.AD_Element_ID=c.AD_Element_ID"
                        + " AND c.AD_Column_ID=" + GetAD_Column_ID()
                        + " AND AD_PrintFormatItem_Trl.AD_PrintFormatItem_ID = " + Get_ID() + ")"
                    + " AND EXISTS (SELECT * FROM AD_Client "
                        + "WHERE AD_Client_ID=AD_PrintFormatItem_Trl.AD_Client_ID AND IsMultiLingualDocument='Y')";
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            }

            return success;
        }	//	afterSave
    }
}

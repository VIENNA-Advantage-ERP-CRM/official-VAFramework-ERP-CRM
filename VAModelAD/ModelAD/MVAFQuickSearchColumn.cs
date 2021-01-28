/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_QuickSearchColumn
 * Chronological Development
 * Veena Pandey     31-Aug-09
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAFQuickSearchColumn : X_VAF_QuickSearchColumn
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_QuickSearchColumn_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFQuickSearchColumn(Ctx ctx, int VAF_QuickSearchColumn_ID, Trx trxName)
            : base(ctx, VAF_QuickSearchColumn_ID, trxName)
        {
            if (VAF_QuickSearchColumn_ID == 0)
            {
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsDisplayed(true);	// Y
                SetIsQueryCriteria(false);
                SetSeqNo(0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_QuickSearchColumn WHERE VAF_QuickSearchWindow_ID=@VAF_QuickSearchWindow_ID@
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAFQuickSearchColumn(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Sync Terminology
            if ((newRecord || Is_ValueChanged("VAF_ColumnDic_ID"))
                && GetVAF_ColumnDic_ID() != 0
                && IsCentrallyMaintained())
            {
                M_VAFColumnDic element = new M_VAFColumnDic(GetCtx(), GetVAF_ColumnDic_ID(), null);
                SetName(element.GetName());
                SetDescription(element.GetDescription());
                SetHelp(element.GetHelp());
            }
            //	Auto Numbering
            if (newRecord && GetSeqNo() == 0)
            {
                String sql = "SELECT COALESCE(MAX(SeqNo),0)+10 FROM VAF_QuickSearchColumn WHERE VAF_QuickSearchWindow_ID=" + GetVAF_QuickSearchWindow_ID();
                int no = CoreLibrary.DataBase.DB.GetSQLValue(null, sql);
                SetSeqNo(no);
            }

            return true;
        }
    }
}

/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_InfoColumn
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
    public class MInfoColumn : X_AD_InfoColumn
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_InfoColumn_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MInfoColumn(Ctx ctx, int AD_InfoColumn_ID, Trx trxName)
            : base(ctx, AD_InfoColumn_ID, trxName)
        {
            if (AD_InfoColumn_ID == 0)
            {
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsDisplayed(true);	// Y
                SetIsQueryCriteria(false);
                SetSeqNo(0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_InfoColumn WHERE AD_InfoWindow_ID=@AD_InfoWindow_ID@
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MInfoColumn(Ctx ctx, DataRow dr, Trx trxName)
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
            if ((newRecord || Is_ValueChanged("AD_Element_ID"))
                && GetAD_Element_ID() != 0
                && IsCentrallyMaintained())
            {
                M_Element element = new M_Element(GetCtx(), GetAD_Element_ID(), null);
                SetName(element.GetName());
                SetDescription(element.GetDescription());
                SetHelp(element.GetHelp());
            }
            //	Auto Numbering
            if (newRecord && GetSeqNo() == 0)
            {
                String sql = "SELECT COALESCE(MAX(SeqNo),0)+10 FROM AD_InfoColumn WHERE AD_InfoWindow_ID=" + GetAD_InfoWindow_ID();
                int no = DataBase.DB.GetSQLValue(null, sql);
                SetSeqNo(no);
            }

            return true;
        }
    }
}

/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_CtrlRef_Table
 * Chronological Development
 * Veena Pandey     12-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
    /// <summary>
    /// Table Referenece Model
    /// </summary>
  public  class MVAFCtrlRefTable : X_VAF_CtrlRef_Table
    {
        /**	Cache						*/
        private static CCache<int, MVAFCtrlRefTable> cache = new CCache<int, MVAFCtrlRefTable>("VAF_CtrlRef_Table", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Control_Ref_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFCtrlRefTable(Ctx ctx, int VAF_Control_Ref_ID, Trx trxName)
            : base(ctx, VAF_Control_Ref_ID, trxName)
        {
            if (VAF_Control_Ref_ID == 0)
            {
                //	setVAF_TableView_ID (0);
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsValueDisplayed(false);
            }
        }

        /// <summary>
        /// Overload Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFCtrlRefTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get MRefTable from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Control_Ref_ID">id</param>
        /// <returns>MRefTable</returns>
        public static MVAFCtrlRefTable Get(Ctx ctx, int VAF_Control_Ref_ID)
        {
            int key = VAF_Control_Ref_ID;
            MVAFCtrlRefTable retValue = (MVAFCtrlRefTable)cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAFCtrlRefTable(ctx, VAF_Control_Ref_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <returns>table name</returns>
        public String GetTableName()
        {
            int VAF_TableView_ID = GetVAF_TableView_ID();
            return MVAFTableView.GetTableName(GetCtx(), VAF_TableView_ID);
        }

        /// <summary>
        /// Get Key ColumnName
        /// </summary>
        /// <returns>Key Column Name</returns>
        public String GetKeyColumnName()
        {
            int VAF_Column_ID = GetColumn_Key_ID();
            return MVAFColumn.GetColumnName(GetCtx(), VAF_Column_ID);
        }

        /// <summary>
        /// Get Display ColumnName
        /// </summary>
        /// <returns>Display Column Name</returns>
        public String GetDisplayColumnName()
        {
            int VAF_Column_ID = GetColumn_Display_ID();
            return MVAFColumn.GetColumnName(GetCtx(), VAF_Column_ID);
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRefTable[");
            sb.Append(GetVAF_Control_Ref_ID()).Append("-")
                .Append(GetVAF_TableView_ID()).Append("]");
            return sb.ToString();
        }
    }
}

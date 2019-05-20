/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_Ref_Table
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
  public  class MRefTable : X_AD_Ref_Table
    {
        /**	Cache						*/
        private static CCache<int, MRefTable> cache = new CCache<int, MRefTable>("AD_Ref_Table", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MRefTable(Ctx ctx, int AD_Reference_ID, Trx trxName)
            : base(ctx, AD_Reference_ID, trxName)
        {
            if (AD_Reference_ID == 0)
            {
                //	setAD_Table_ID (0);
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
        public MRefTable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get MRefTable from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">id</param>
        /// <returns>MRefTable</returns>
        public static MRefTable Get(Ctx ctx, int AD_Reference_ID)
        {
            int key = AD_Reference_ID;
            MRefTable retValue = (MRefTable)cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MRefTable(ctx, AD_Reference_ID, null);
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
            int AD_Table_ID = GetAD_Table_ID();
            return MTable.GetTableName(GetCtx(), AD_Table_ID);
        }

        /// <summary>
        /// Get Key ColumnName
        /// </summary>
        /// <returns>Key Column Name</returns>
        public String GetKeyColumnName()
        {
            int AD_Column_ID = GetColumn_Key_ID();
            return MColumn.GetColumnName(GetCtx(), AD_Column_ID);
        }

        /// <summary>
        /// Get Display ColumnName
        /// </summary>
        /// <returns>Display Column Name</returns>
        public String GetDisplayColumnName()
        {
            int AD_Column_ID = GetColumn_Display_ID();
            return MColumn.GetColumnName(GetCtx(), AD_Column_ID);
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRefTable[");
            sb.Append(GetAD_Reference_ID()).Append("-")
                .Append(GetAD_Table_ID()).Append("]");
            return sb.ToString();
        }
    }
}

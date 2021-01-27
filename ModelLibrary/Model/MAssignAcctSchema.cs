/********************************************************
 * Class Name     : MAssignAcctSchema
 * Purpose        : Assigned Accounting Schema Model
 * Class Used     : X_GL_AssignedAcctSchema
 * Chronological    Development
 * Amit           21-Oct-2019
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MAssignAcctSchema : X_VAGL_AssignAcctSchema
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAGL_AssignAcctSchema_ID">id</param>
        /// <param name="trxName"> transaction</param>
        public MAssignAcctSchema(Ctx ctx, int VAGL_AssignAcctSchema_ID, Trx trxName)
            : base(ctx, VAGL_AssignAcctSchema_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MAssignAcctSchema(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// This function is used to get the object of Assigned Accouting schema 
        /// </summary>
        /// <param name="ctx">currenct context</param>
        /// <param name="VAGL_JRNL_ID">journal ID</param>
        /// <param name="VAB_AccountBook_ID">Accounting Schema ID</param>
        /// <param name="trxName">Trx</param>
        /// <returns>object of AssignAcctSchema Class</returns>
        public static MAssignAcctSchema GetOrCreate(Ctx ctx, int VAGL_JRNL_ID, int VAB_AccountBook_ID, Trx trxName)
        {
            MAssignAcctSchema retValue = null;
            String sql = "SELECT VAGL_AssignAcctSchema_ID FROM VAGL_AssignAcctSchema WHERE IsActive = 'Y' AND VAGL_JRNL_ID = " + VAGL_JRNL_ID +
                         @" AND VAB_AccountBook_ID = " + VAB_AccountBook_ID;
            int recordId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));
            retValue = new MAssignAcctSchema(ctx, recordId, trxName);
            return retValue;
        }

    }
}

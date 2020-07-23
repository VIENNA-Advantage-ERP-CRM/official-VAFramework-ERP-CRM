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
    public class MAssignAcctSchema : X_GL_AssignAcctSchema
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="GL_AssignAcctSchema_ID">id</param>
        /// <param name="trxName"> transaction</param>
        public MAssignAcctSchema(Ctx ctx, int GL_AssignAcctSchema_ID, Trx trxName)
            : base(ctx, GL_AssignAcctSchema_ID, trxName)
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
        /// <param name="GL_Journal_ID">journal ID</param>
        /// <param name="C_AcctSchema_ID">Accounting Schema ID</param>
        /// <param name="trxName">Trx</param>
        /// <returns>object of AssignAcctSchema Class</returns>
        public static MAssignAcctSchema GetOrCreate(Ctx ctx, int GL_Journal_ID, int C_AcctSchema_ID, Trx trxName)
        {
            MAssignAcctSchema retValue = null;
            String sql = "SELECT GL_AssignAcctSchema_ID FROM GL_AssignAcctSchema WHERE IsActive = 'Y' AND GL_Journal_ID = " + GL_Journal_ID +
                         @" AND C_AcctSchema_ID = " + C_AcctSchema_ID;
            int recordId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));
            retValue = new MAssignAcctSchema(ctx, recordId, trxName);
            return retValue;
        }

    }
}

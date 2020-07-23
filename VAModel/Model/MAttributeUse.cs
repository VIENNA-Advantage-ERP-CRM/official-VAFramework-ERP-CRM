/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttributeUse
 * Purpose        : Attribute Use Model 
 * Class Used     : X_M_AttributeUse
 * Chronological    Development
 * Raghunandan     22-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAttributeUse : X_M_AttributeUse
    {
        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MAttributeUse(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {
            if (ignored != 0)
            {
                throw new Exception("Multi-Key");
            }
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr"></param>
        /// <param name="trxName">transaction</param>
        public MAttributeUse(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }


        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	also used for afterDelete
            String sql = "UPDATE M_AttributeSet mas"
                + " SET IsInstanceAttribute='Y' "
                + "WHERE M_AttributeSet_ID=" + GetM_AttributeSet_ID()
                + " AND IsInstanceAttribute='N'"
                + " AND (IsSerNo='Y' OR IsLot='Y' OR IsGuaranteeDate='Y'"
                    + " OR EXISTS (SELECT * FROM M_AttributeUse mau"
                        + " INNER JOIN M_Attribute ma ON (mau.M_Attribute_ID=ma.M_Attribute_ID) "
                        + "WHERE mau.M_AttributeSet_ID=mas.M_AttributeSet_ID"
                        + " AND mau.IsActive='Y' AND ma.IsActive='Y'"
                        + " AND ma.IsInstanceAttribute='Y')"
                        + ")";
            int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            if (no != 0)
            {
                log.Fine("afterSave - Set Instance Attribute");
            }
            //
            sql = "UPDATE M_AttributeSet mas"
                + " SET IsInstanceAttribute='N' "
                + "WHERE M_AttributeSet_ID=" + GetM_AttributeSet_ID()
                + " AND IsInstanceAttribute='Y'"
                + "	AND IsSerNo='N' AND IsLot='N' AND IsGuaranteeDate='N'"
                + " AND NOT EXISTS (SELECT * FROM M_AttributeUse mau"
                    + " INNER JOIN M_Attribute ma ON (mau.M_Attribute_ID=ma.M_Attribute_ID) "
                    + "WHERE mau.M_AttributeSet_ID=mas.M_AttributeSet_ID"
                    + " AND mau.IsActive='Y' AND ma.IsActive='Y'"
                    + " AND ma.IsInstanceAttribute='Y')";
            no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            if (no != 0)
            {
                log.Fine("afterSave - Reset Instance Attribute");
            }
            return success;
        }


        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            AfterSave(false, success);
            return success;
        }
    }
}

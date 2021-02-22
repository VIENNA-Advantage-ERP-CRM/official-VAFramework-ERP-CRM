/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVASResAssignment
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     10-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MVASResAssignment : X_VAS_Res_Assignment
    {
        /// <summary>
        /// Stnadard Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="VAS_Res_Assignment_ID">id</param>
        /// <param name="trxName">trx</param>
        public MVASResAssignment(Ctx ctx, int VAS_Res_Assignment_ID, Trx trxName)
            : base(ctx, VAS_Res_Assignment_ID, trxName)
        {
            p_info.SetUpdateable(true);		//	default table is not updateable
            //	Default values
            if (VAS_Res_Assignment_ID == 0)
            {
                SetAssignDateFrom(DateTime.Now);
                SetQty((Decimal)(1.0));
                SetName(".");
                SetIsConfirmed(false);
            }
        }

        /// <summary>
        /// Load Contsructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datrow</param>
        /// <param name="trxName"></param>
        public MVASResAssignment(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>true</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            /*
            v_Description := :new.Name;
        IF (:new.Description IS NOT NULL AND LENGTH(:new.Description) > 0) THEN
            v_Description := v_Description || ' (' || :new.Description || ')';			
        END IF;
	
        -- Update Expense Line
        UPDATE VAS_ExpenseReportLine
          SET  Description = v_Description,
            Qty = :new.Qty
        WHERE VAS_Res_Assignment_ID = :new.VAS_Res_Assignment_ID
          AND (Description <> v_Description OR Qty <> :new.Qty);
	  
        -- Update Order Line
        UPDATE VAB_OrderLine
          SET  Description = v_Description,
            QtyOrdered = :new.Qty
        WHERE VAS_Res_Assignment_ID = :new.VAS_Res_Assignment_ID
          AND (Description <> v_Description OR QtyOrdered <> :new.Qty);

        -- Update Invoice Line
        UPDATE VAB_InvoiceLine
          SET  Description = v_Description,
            QtyInvoiced = :new.Qty
        WHERE VAS_Res_Assignment_ID = :new.VAS_Res_Assignment_ID
          AND (Description <> v_Description OR QtyInvoiced <> :new.Qty);
          */
            return success;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>string</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVASResAssignment[ID=");
            sb.Append(Get_ID())
                .Append(",VAS_Resource_ID=").Append(GetS_Resource_ID())
                .Append(",From=").Append(GetAssignDateFrom())
                .Append(",To=").Append(GetAssignDateTo())
                .Append(",Qty=").Append(GetQty())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if not confirmed</returns>
        protected override bool BeforeDelete()
        {
            //	 allow to delete, when not confirmed
            if (IsConfirmed())
                return false;

            return true;
        }

    }
}

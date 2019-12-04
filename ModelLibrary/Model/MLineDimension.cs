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
    public class MLineDimension : X_GL_LineDimension
    {
        public MLineDimension(Ctx ctx, int GL_LineDimension_ID, Trx trxName)
             : base(ctx, GL_LineDimension_ID, trxName)
        {
        }

        public MLineDimension(Ctx ctx, DataRow dr, Trx trxName)
             : base(ctx, dr, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            MJournalLine obj = new MJournalLine(GetCtx(), GetGL_JournalLine_ID(), Get_Trx());
            // In Case of reversal, bypass this condition
            if (!(obj.Get_ColumnIndex("ReversalDoc_ID") > 0 && obj.GetReversalDoc_ID() > 0))
            {
                string val = "";
                if (obj.GetAmtSourceDr() > 0)
                {
                    val = " AmtSourceDr ";
                }
                else
                {
                    val = " AmtSourceCr ";
                }

                string sql = "SELECT SUM(amount) FROM Gl_Linedimension WHERE GL_JournalLine_ID=" + Get_Value("GL_JournalLine_ID") + " AND Gl_Linedimension_ID NOT IN( " + GetGL_LineDimension_ID() + ")";
                Decimal count = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
                count += GetAmount();

                sql = "SELECT " + val + " FROM GL_JournalLine WHERE GL_JournalLine_ID=" + Get_Value("GL_JournalLine_ID");
                Decimal amtcount = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));

                if (count > amtcount)
                {
                    log.SaveWarning("AmoutCheck", "");
                    return false;
                }
            }

            return true;
        }




    }
}

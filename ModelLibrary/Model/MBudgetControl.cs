using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MBudgetControl : X_GL_BudgetControl
    {
        public MBudgetControl(Ctx ctx, int GL_BudgetControl_ID, Trx trxName)
           : base(ctx, GL_BudgetControl_ID, trxName)
        {

        }

        public MBudgetControl(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

      


    }
}

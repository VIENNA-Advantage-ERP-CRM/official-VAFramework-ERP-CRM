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
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    class MVABIncomeTaxLines:X_VAB_IncomeTaxLines
    {
        public MVABIncomeTaxLines(Ctx ctx, int VAB_IncomeTaxLines_ID, Trx trxName)
            : base(ctx, VAB_IncomeTaxLines_ID, trxName)
        {

        }

        public MVABIncomeTaxLines(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }        
    }
}

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
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    class MIncomeTaxLines:X_C_IncomeTaxLines
    {
        public MIncomeTaxLines(Ctx ctx, int C_IncomeTaxLines_ID, Trx trxName)
            : base(ctx, C_IncomeTaxLines_ID, trxName)
        {

        }

        public MIncomeTaxLines(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }        
    }
}

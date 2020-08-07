/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDiscountSchemaLine
 * Purpose        : Discount Schema Line (Price List) Model
 * Class Used     : X_M_DiscountSchemaLine
 * Chronological    Development
 * Raghunandan     11-Jun-2009
  ******************************************************/
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
namespace VAdvantage.Model
{
    public class MDiscountSchemaLine : X_M_DiscountSchemaLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_DiscountSchemaLine_ID"></param>
        /// <param name="trxName"></param>
        public MDiscountSchemaLine(Ctx ctx, int M_DiscountSchemaLine_ID, Trx trxName)
            : base(ctx, M_DiscountSchemaLine_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MDiscountSchemaLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

    }
}

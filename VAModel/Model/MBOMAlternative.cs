using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using VAdvantage.SqlExec;
using System.Reflection;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    /// <summary>
    /// BOM Alternative Model
    /// </summary>
    public class MBOMAlternative : X_M_BOMAlternative
    {
        #region Private Variables
        //Logger for class MBOMAlternative 
        private static VLogger s_log = VLogger.GetVLogger(typeof(MBOMAlternative).FullName);
       // private static long SERIALVERSIONUID = 1L;
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_BOMAlternative_ID"></param>
        /// <param name="trx"></param>
        public MBOMAlternative(Ctx ctx, int M_BOMAlternative_ID, Trx trx)
            : base(ctx, M_BOMAlternative_ID, trx)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trx"></param>
        public MBOMAlternative(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Model
{
   public class MTabPanel:X_AD_TabPanel
    {

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">Data row</param>
        /// <param name="trxName">transaction</param>
       public MTabPanel(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super(ctx, rs, trxName);
        }

       /// <summary>
       ///	Load Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="rs">Data row</param>
       /// <param name="trxName">transaction</param>
       public MTabPanel(Ctx ctx, int AD_TabPanel_ID, Trx trxName)
           : base(ctx, AD_TabPanel_ID, trxName)
       {
           //super(ctx, rs, trxName);
       }


           /// <summary>
        ///	Parent Constructor
        /// </summary>
        /// <param name="parent">parent parent</param>
       public MTabPanel(MTab parent)
            : base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //this(parent.GetCtx(), 0, parent.Get_TrxName());
            SetClientOrg(parent);
            SetAD_Tab_ID(parent.GetAD_Tab_ID());
        }

    }
} 

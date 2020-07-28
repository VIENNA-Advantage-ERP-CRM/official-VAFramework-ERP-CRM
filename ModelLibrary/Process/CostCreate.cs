/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CostCreate
 * Purpose        : Create/Update Costing for Product 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     26-Oct-2009
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class CostCreate : ProcessEngine.SvrProcess
    {
        //Product				
        private int _M_Product_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                //	log.fine("prepare - " + para[i]);
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (text with variables)</returns>
        protected override String DoIt()
        {
            log.Info("M_Product_ID=" + _M_Product_ID);
            if (_M_Product_ID == 0)
            {
                throw new Exception("@NotFound@: @M_Product_ID@ = " + _M_Product_ID);
            }
            MProduct product = MProduct.Get(GetCtx(), _M_Product_ID);
            if (product.Get_ID() != _M_Product_ID)
            {
                throw new Exception("@NotFound@: @M_Product_ID@ = " + _M_Product_ID);
            }
            //
            if (MCostDetail.ProcessProduct(product, Get_Trx()))
            {
                return "@OK@";
            }
            return "@Error@";
        }
    }
}

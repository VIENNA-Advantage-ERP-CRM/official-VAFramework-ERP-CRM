/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PackageCreate
 * Purpose        : Create Package from Shipment for Shipper
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     21-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class PackageCreate : ProcessEngine.SvrProcess
    {
        //	Shipper				
        private int _VAM_ShippingMethod_ID = 0;
        // Parent				
        private int _VAM_Inv_InOut_ID = 0;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAM_ShippingMethod_ID"))
                {
                    _VAM_ShippingMethod_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_Invoice_ID"))
                {
                    _VAM_Inv_InOut_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            if (_VAM_Inv_InOut_ID == 0)
            {
                _VAM_Inv_InOut_ID = GetRecord_ID();
            }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("doIt - VAM_Inv_InOut_ID=" + _VAM_Inv_InOut_ID + ", VAM_ShippingMethod_ID=" + _VAM_ShippingMethod_ID);
            if (_VAM_Inv_InOut_ID == 0)
            {
                throw new Exception("No Shipment");
            }
            if (_VAM_ShippingMethod_ID == 0)
            {
                throw new Exception("No Shipper");
            }

            string sql = "select VAM_Inv_InOut_ID from VAM_Inv_InOutConfirm where VAM_Inv_InOutConfirm_ID = " + _VAM_Inv_InOut_ID;
            int _M_Shipment_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

            MInOut shipment = new MInOut(GetCtx(), _M_Shipment_ID, null);
            if (shipment.Get_ID() != _M_Shipment_ID)
            {
                throw new Exception("Cannot find Shipment ID=" + _VAM_Inv_InOut_ID);
            }
            MShipper shipper = new MShipper(GetCtx(), _VAM_ShippingMethod_ID, Get_TrxName());
            if (shipper.Get_ID() != _VAM_ShippingMethod_ID)
            {
                throw new Exception("Cannot find Shipper ID=" + _VAM_Inv_InOut_ID);
            }
           
            MPackage pack = MPackage.Create(shipment, shipper, null, Get_TrxName());

            return pack.GetDocumentNo();
        }
    }
}

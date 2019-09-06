/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InOutCreateConfirm
 * Purpose        : class used ti confirm the document for shipment etc.
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     20-July-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;



using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class InOutCreateConfirm : ProcessEngine.SvrProcess
    {
        #region Variables
        //	Process Message 			
        private String _processMsg = null;
        //	Shipment				
        private int _M_InOut_ID = 0;
        //	Confirmation Type		
        private String _ConfirmType = null;
        #endregion

        /// <summary>
        ///  Prepare - e.g., get Parameters.
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
                else if (name.Equals("ConfirmType"))
                {
                    _ConfirmType = (String)para[i].GetParameter();
                }
                else
                {
                    //log.log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            _M_InOut_ID = GetRecord_ID();
        }

        /// <summary>
        /// Create Confirmation
        /// </summary>
        /// <returns>document no</returns>
        protected override String DoIt()
        {
            //log.info("M_InOut_ID=" + _M_InOut_ID + ", Type=" + _ConfirmType);
            MInOut shipment = new MInOut(GetCtx(), _M_InOut_ID, null);
            if (shipment.Get_ID() == 0)
            {
                throw new ArgumentException("Not found M_InOut_ID=" + _M_InOut_ID);
            }

            MInOutLine[] lines = shipment.GetLines();
            if (lines == null || lines.Length == 0)
            {
                _processMsg = Msg.GetMsg(GetCtx(), "NoLines"); //Return Msg in Process Message
                throw new ArgumentException(_processMsg);
            }
            //
            MInOutConfirm[] confirmations = shipment.GetConfirmations(false);
            MInOutConfirm confirm = null;
            int count = 0;
            if (confirmations != null && confirmations.Length > 0)
            {
                for (Int32 i = 0; i < confirmations.Length; i++)
                {
                    confirm = null;
                    count += 1;
                    confirm = confirmations[i];
                    if (confirm.GetDocStatus() == "VO")
                    {
                        if (count == confirmations.Length)
                        {
                            confirm = MInOutConfirm.Create(shipment, _ConfirmType, false);
                            break;
                        }
                    }
                    if (confirm.GetDocStatus() == "DR")
                    {
                        confirm = MInOutConfirm.Create(shipment, _ConfirmType, true);
                        break;
                    }
                }
            }
            else
                confirm = MInOutConfirm.Create(shipment, _ConfirmType, true);
            //throw new Exception("Cannot create Confirmation for " + shipment.GetDocumentNo());

            if (confirm == null)
            {
                throw new Exception("Cannot create Confirmation for " + shipment.GetDocumentNo());
            }
            if (shipment.GetDocStatus() == "DR") //To set docstatus for MR in InProgress state when shipment/MR is in Drafted state
            {
                shipment.SetDocStatus("IP");
                shipment.FreezeDoc();
            }
            //
            return "Open Shipment Confirmation Number generated: " + confirm.GetDocumentNo();
        }
    }
}

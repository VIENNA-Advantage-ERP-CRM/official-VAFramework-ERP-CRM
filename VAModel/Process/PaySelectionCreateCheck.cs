/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PaySelectionCreateCheck
 * Purpose        : Create Checks from Payment Selection Line
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     13-Nov-2009
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
    public class PaySelectionCreateCheck : ProcessEngine.SvrProcess
    {
        //	Target Payment Rule			
        private String _PaymentRule = null;
        //	Payment Selection			
        private int _C_PaySelection_ID = 0;
        // The checks					
        private List<MPaySelectionCheck> _list = new List<MPaySelectionCheck>();

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
                else if (name.Equals("PaymentRule"))
                {
                    _PaymentRule = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_PaySelection_ID = GetRecord_ID();
            if (_PaymentRule != null && _PaymentRule.Equals(X_C_Order.PAYMENTRULE_DirectDebit))
            {
                _PaymentRule = null;
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            log.Info("C_PaySelection_ID=" + _C_PaySelection_ID
                + ", PaymentRule=" + _PaymentRule);

            MPaySelection psel = new MPaySelection(GetCtx(), _C_PaySelection_ID, Get_TrxName());
            if (psel.Get_ID() == 0)
            {
                throw new ArgumentException("Not found C_PaySelection_ID=" + _C_PaySelection_ID);
            }
            if (psel.IsProcessed())
            {
                throw new ArgumentException("@Processed@");
            }
            //
            MPaySelectionLine[] lines = psel.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MPaySelectionLine line = lines[i];
                if (!line.IsActive() || line.IsProcessed())
                {
                    continue;
                }
                CreateCheck(line);
            }
            //
            psel.SetProcessed(true);
            psel.Save();

            return "@C_PaySelectionCheck_ID@ - #" + _list.Count;
        }

        /// <summary>
        /// Create Check from line
        /// </summary>
        /// <param name="line">line</param>
        private void CreateCheck(MPaySelectionLine line)
        {
            //	Try to find one
            for (int i = 0; i < _list.Count; i++)
            {
                MPaySelectionCheck check = (MPaySelectionCheck)_list[i];
                //	Add to existing
                if (check.GetC_BPartner_ID() == line.GetInvoice().GetC_BPartner_ID())
                {
                    check.AddLine(line);
                    if (!check.Save())
                    {
                        throw new Exception("Cannot save MPaySelectionCheck");
                    }
                    line.SetC_PaySelectionCheck_ID(check.GetC_PaySelectionCheck_ID());
                    line.SetProcessed(true);
                    if (!line.Save())
                    {
                        throw new Exception("Cannot save MPaySelectionLine");
                    }
                    return;
                }
            }
            //	Create new
            String PaymentRule = line.GetPaymentRule();
            if (_PaymentRule != null && _PaymentRule != " ")
            {
                if (!X_C_Order.PAYMENTRULE_DirectDebit.Equals(PaymentRule))
                {
                    PaymentRule = _PaymentRule;
                }
            }
            MPaySelectionCheck check1 = new MPaySelectionCheck(line, PaymentRule);
            if (!check1.IsValid())
            {
                int C_BPartner_ID = check1.GetC_BPartner_ID();
                MBPartner bp = MBPartner.Get(GetCtx(), C_BPartner_ID);
                String msg = "@NotFound@ @C_BP_BankAccount@: " + bp.GetName();
                throw new Exception(msg);
            }
            if (!check1.Save())
            {
                throw new Exception("Cannot save MPaySelectionCheck");
            }
            line.SetC_PaySelectionCheck_ID(check1.GetC_PaySelectionCheck_ID());
            line.SetProcessed(true);
            if (!line.Save())
            {
                throw new Exception("Cannot save MPaySelectionLine");
            }
            _list.Add(check1);
        }
    }
}

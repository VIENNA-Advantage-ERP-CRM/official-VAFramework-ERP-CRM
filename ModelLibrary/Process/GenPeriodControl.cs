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
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;

namespace VAdvantage.Process
{
    public class GenPeriodControl : SvrProcess
    {
        //Period					
        private int _C_Period_ID = 0;
        //Organization
        private string orgs = null;
        private string[] _AD_Org_ID;


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

                }
                else if (name.Equals("AD_Org_ID"))
                {
                    orgs = Util.GetValueOfString(para[i].GetParameter());
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_Period_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MPeriod period = new MPeriod(GetCtx(), _C_Period_ID, Get_Trx());

            // Get all Document type related to Tenant
            MDocType[] types = MDocType.GetOfClient(GetCtx());
            int count = 0;
            List<String> baseTypes = new List<String>();
            ValueNamePair vp = null;

            // Split multiselected organizations to get array
            if (!String.IsNullOrEmpty(orgs))
            {
                _AD_Org_ID = orgs.Split(',');
            }


            for (int i = 0; i < types.Length; i++)
            {
                MDocType type = types[i];
                String docBaseType = type.GetDocBaseType();
                if (baseTypes.Contains(docBaseType))
                    continue;

                // loop on multiple selected organizations
                for (int j = 0; j < _AD_Org_ID.Length; j++)
                {
                    MPeriodControl pc = new MPeriodControl(period, docBaseType);
                    pc.SetAD_Org_ID(Util.GetValueOfInt(_AD_Org_ID[j]));
                    if (pc.Save())
                    {
                        count++;
                    }
                    else
                    {
                        vp = VLogger.RetrieveError();
                        if (vp != null)
                        {
                            log.Severe(Msg.GetMsg(GetCtx(), "PeriodCtlNotSaved") + ", " + vp.GetName());
                        }
                        else
                        {
                            log.Severe(Msg.GetMsg(GetCtx(), "PeriodCtlNotSaved"));
                        }
                    }
                }
                baseTypes.Add(docBaseType);
            }

            log.Fine("PeriodControl #" + count);

            return Msg.GetMsg(GetCtx(), "PeriodControlGenerated");
        }

    }
}

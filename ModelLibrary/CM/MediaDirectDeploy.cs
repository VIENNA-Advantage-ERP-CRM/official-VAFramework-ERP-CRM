using System;
using System.Collections.Generic;
//using System.Linq;
using VAdvantage.CM;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.ProcessEngine;

namespace VAdvantage.CM
{
    public class MediaDirectDeploy : ProcessEngine.SvrProcess
    {

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
                //else if (name.equals("CM_WebProject_ID"))
                //p_CM_WebProject_ID = ((BigDecimal)para[i].getParameter()).intValue();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        } // prepare


        protected override String DoIt()
        {
            MMedia thisMedia = new MMedia(GetCtx(), GetRecord_ID(), Get_TrxName());
            MMediaServer[] theseServers = MMediaServer.GetMediaServer(thisMedia.GetWebProject());
            if (theseServers != null)
            {
                for (int i = 0; i < theseServers.Length; i++)
                { 
                    MMediaDeploy thisDeploy = MMediaDeploy.GetByMedia(GetCtx(), GetRecord_ID(), theseServers[i].Get_ID(), true, Get_TrxName());
                    thisDeploy.SetIsDeployed(false);
                    thisDeploy.Save(Get_TrxName());
                    thisDeploy.Load(Get_TrxName());
                    Get_Trx().Commit();
                    theseServers[i].Deploy();
                }
            }
            return null;
        }

    }

}

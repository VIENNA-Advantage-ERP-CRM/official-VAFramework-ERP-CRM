using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MTeamForcastModel
    {
        /// <summary>
        /// Get SuperVisor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int GetSupervisor(Ctx ctx, int C_Team_ID)
        {
           string sql="SELECT SUpervisor_ID FROM C_Team WHERE C_Team_ID="+ C_Team_ID;
            return Util.GetValueOfInt(DB.ExecuteScalar(sql));       
        }
    }
}
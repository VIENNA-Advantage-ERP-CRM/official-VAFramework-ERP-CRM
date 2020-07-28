/********************************************************
 * Module Name    : Process
 * Purpose        : Delere Notes (Notice)
 * Author         : Jagmohan Bhatt
 * Date           : 10-Nov-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.DataBase;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class NoteDelete : ProcessEngine.SvrProcess
    {
        private int p_AD_User_ID = -1;

        //Prepare
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if ((para[i].GetParameter() == null) || (para[i].GetParameter().ToString() == ""))
                { }
                else if (name.Equals("AD_User_ID"))
                {
                    p_AD_User_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
            }
        }



        /// <summary>
        /// Do IT
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            log.Info("doIt - AD_User_ID=" + p_AD_User_ID);

            String sql = "DELETE FROM AD_Note WHERE AD_Client_ID=" + GetAD_Client_ID();
            if (p_AD_User_ID > 0)
            {
                sql += " AND AD_User_ID=" + p_AD_User_ID;
            }
            //
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            return "@Deleted@ = " + no;
        }
    }
}

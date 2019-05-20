using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;


/* Process: Transfer OwnerShip
 * Writer : Arpit Singh
 * Date   : 4/2/12 
 */

namespace VAdvantage.Process
{
    class TransferOwnerShip : SvrProcess
    {
        int Record_ID, FromSalesRep_ID, ToSalesRep_ID;
        //String msg;
        protected override void Prepare()
        {
            Record_ID = GetRecord_ID();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("RefSalesRep_ID"))
                {
                    FromSalesRep_ID = Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
                }

                else if (name.Equals("SalesRep_ID"))
                {
                    ToSalesRep_ID = Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else
                {

                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }



        }


        protected override String DoIt()
        {
            string Sql = "Select C_Lead_ID From C_Lead where SalesRep_ID=" + FromSalesRep_ID+" and isactive='Y' and Ad_Org_id="+GetCtx().GetAD_Org_ID();
            IDataReader dr =DB.ExecuteReader(Sql);
            while (dr.Read())
            {
                VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), Util.GetValueOfInt(dr[0]), null);
                lead.SetSalesRep_ID(ToSalesRep_ID);
                lead.Save();
                {
                    
                }

            }
            
            dr.Close();

            Sql = "Select C_Project_ID From C_Project where SalesRep_ID=" + FromSalesRep_ID + " and isactive='Y' and Ad_Org_id=" + GetCtx().GetAD_Org_ID();
            dr = DB.ExecuteReader(Sql);
            while (dr.Read())
            {
                VAdvantage.Model.X_C_Project Project = new VAdvantage.Model.X_C_Project(GetCtx(), Util.GetValueOfInt(dr[0]), null);
                Project.SetSalesRep_ID(ToSalesRep_ID);
                Project.Save();
                {

                }

            }
            dr.Close();

            Sql = "Select C_BPartner_ID From C_BPartner where SalesRep_ID=" + FromSalesRep_ID + " and isactive='Y' and Ad_Org_id=" + GetCtx().GetAD_Org_ID();
            dr = DB.ExecuteReader(Sql);
            while (dr.Read())
            {
                VAdvantage.Model.X_C_BPartner BP = new VAdvantage.Model.X_C_BPartner(GetCtx(), Util.GetValueOfInt(dr[0]), null);
                BP.SetSalesRep_ID(ToSalesRep_ID);
                BP.Save();
                {

                }

            }
            dr.Close();





            return Msg.GetMsg(GetCtx(), "RecordsTransferedSuccessfully"); ;
        }
      





    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;

/* Process: Generate Target List 
 * Writer :Arpit Singh
 * Date   : 25/1/12 
 */

namespace ViennaAdvantageServer.Process
{
    class CreateTargetList : SvrProcess
    {
        int Record_ID, Campaign_id, Table_id;

        protected override void Prepare()
        {
            Record_ID = GetRecord_ID();
            Table_id = GetTable_ID();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_MasterTargetList_ID"))
                {
                    Campaign_id = Util.GetValueOfInt(Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else
                {

                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }



        }


        protected override String DoIt()
        {
            string sql = "Select vaf_tableview_id from vaf_tableview where tablename='VAB_Lead'";
            int leadTable_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            sql = "Select vaf_tableview_id from vaf_tableview where tablename='VAB_BusinessPartner'";
            int BPartnerTable_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            VAdvantage.Model.X_VAB_TargetList TList = new VAdvantage.Model.X_VAB_TargetList(GetCtx(), 0, null);

            if (Table_id == leadTable_ID)// VAB_Lead
            {
                VAdvantage.Model.X_VAB_Lead lead = new VAdvantage.Model.X_VAB_Lead(GetCtx(), Record_ID, null);

                TList.Set_ValueNoCheck("VAB_MasterTargetList_ID", Campaign_id);
                // TList.SetVAB_MasterTargetList_ID(Campaign_id);
                TList.SetVAB_Lead_ID(Record_ID);
                TList.SetAddress1(lead.GetAddress1());
                TList.SetAddress2(lead.GetAddress2());
                TList.SetVAB_City_ID(lead.GetVAB_City_ID());
                TList.SetCity(lead.GetCity());
                TList.SetVAB_RegionState_ID(lead.GetVAB_RegionState_ID());
                TList.SetRegionName(lead.GetRegionName());
                TList.SetVAB_Country_ID(lead.GetVAB_Country_ID());
                TList.SetPostal(lead.GetPostal());


                if (TList.Save())
                {
                    return Msg.GetMsg(GetCtx(), "TargetListCreate");
                }
                else
                {
                    return GetRetrievedError(TList, "TargetListNotCreate");
                    //return Msg.GetMsg(GetCtx(), "TargetListNotCreate");
                }
            }

            if (Table_id == BPartnerTable_ID) // VAB_BusinessPartner 
            {
                string Query = "select isprospect from VAB_BusinessPartner where VAB_BusinessPartner_id=" + Record_ID;
                string P = Util.GetValueOfString(DB.ExecuteScalar(Query));
                if (P == "Y")
                {
                    // TList.SetVAB_MasterTargetList_ID(Campaign_id);
                    TList.Set_ValueNoCheck("VAB_MasterTargetList_ID", Campaign_id);
                    TList.SetRef_BPartner_ID(Record_ID);
                    sql = "Select VAB_Address_id from VAB_BPart_Location where VAB_BusinessPartner_id=" + Record_ID;
                    object locID = DB.ExecuteScalar(sql);
                    TList.SetVAB_Address_ID(Util.GetValueOfInt(locID));
                }
                else
                {
                    Query = "select iscustomer from VAB_BusinessPartner where VAB_BusinessPartner_id=" + Record_ID;
                    P = Util.GetValueOfString(DB.ExecuteScalar(Query));
                    if (P == "Y")
                    {
                        //TList.SetVAB_MasterTargetList_ID(Campaign_id);
                        TList.Set_ValueNoCheck("VAB_MasterTargetList_ID", Campaign_id);
                        TList.SetVAB_BusinessPartner_ID(Record_ID);
                        sql = "Select VAB_Address_id from VAB_BPart_Location where VAB_BusinessPartner_id=" + Record_ID;
                        object locID = DB.ExecuteScalar(sql);
                        TList.SetVAB_Address_ID(Util.GetValueOfInt(locID));
                    }
                    else
                        return Msg.GetMsg(GetCtx(), "TargetListNotCreate");

                }
                if (TList.Save())
                {
                    return Msg.GetMsg(GetCtx(), "TargetListCreate");
                }
                else
                {
                    return GetRetrievedError(TList, "TargetListNotCreate");
                    //return Msg.GetMsg(GetCtx(), "TargetListNotCreate");
                }
            }


            return Msg.GetMsg(GetCtx(), "TargetListCreate");
        }




    }
}

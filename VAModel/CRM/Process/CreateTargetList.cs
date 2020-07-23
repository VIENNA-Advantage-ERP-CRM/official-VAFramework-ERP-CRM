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

namespace VAdvantage.Process
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
                else if (name.Equals("C_MasterTargetList_ID"))
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
            string sql = "Select ad_table_id from ad_table where tablename='C_Lead'";
            int leadTable_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            sql = "Select ad_table_id from ad_table where tablename='C_BPartner'";
            int BPartnerTable_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            VAdvantage.Model.X_C_TargetList TList = new VAdvantage.Model.X_C_TargetList(GetCtx(), 0, null);

            if (Table_id == leadTable_ID)// C_Lead
            {
                VAdvantage.Model.X_C_Lead lead = new VAdvantage.Model.X_C_Lead(GetCtx(), Record_ID, null);

                TList.Set_ValueNoCheck("C_MasterTargetList_ID", Campaign_id);
                // TList.SetC_MasterTargetList_ID(Campaign_id);
                TList.SetC_Lead_ID(Record_ID);
                TList.SetAddress1(lead.GetAddress1());
                TList.SetAddress2(lead.GetAddress2());
                TList.SetC_City_ID(lead.GetC_City_ID());
                TList.SetCity(lead.GetCity());
                TList.SetC_Region_ID(lead.GetC_Region_ID());
                TList.SetRegionName(lead.GetRegionName());
                TList.SetC_Country_ID(lead.GetC_Country_ID());
                TList.SetPostal(lead.GetPostal());


                if (TList.Save())
                {
                    return Msg.GetMsg(GetCtx(), "TargetListCreate");
                }
                return Msg.GetMsg(GetCtx(), "TargetListNotCreate");
            }

            if (Table_id == BPartnerTable_ID) // C_BPartner 
            {
                string Query = "select isprospect from c_bpartner where c_bpartner_id=" + Record_ID;
                string P = Util.GetValueOfString(DB.ExecuteScalar(Query));
                if (P == "Y")
                {
                    // TList.SetC_MasterTargetList_ID(Campaign_id);
                    TList.Set_ValueNoCheck("C_MasterTargetList_ID", Campaign_id);
                    TList.SetRef_BPartner_ID(Record_ID);
                    sql = "Select C_Location_id from c_bpartner_Location where c_bpartner_id=" + Record_ID;
                    object locID = DB.ExecuteScalar(sql);
                    TList.SetC_Location_ID(Util.GetValueOfInt(locID));
                }
                else
                {
                    Query = "select iscustomer from c_bpartner where c_bpartner_id=" + Record_ID;
                    P = Util.GetValueOfString(DB.ExecuteScalar(Query));
                    if (P == "Y")
                    {
                        //TList.SetC_MasterTargetList_ID(Campaign_id);
                        TList.Set_ValueNoCheck("C_MasterTargetList_ID", Campaign_id);
                        TList.SetC_BPartner_ID(Record_ID);
                        sql = "Select C_Location_id from c_bpartner_Location where c_bpartner_id=" + Record_ID;
                        object locID = DB.ExecuteScalar(sql);
                        TList.SetC_Location_ID(Util.GetValueOfInt(locID));
                    }
                    else
                        return Msg.GetMsg(GetCtx(), "TargetListNotCreate");

                }
                if (TList.Save())
                {
                    return Msg.GetMsg(GetCtx(), "TargetListCreate");
                }

                return Msg.GetMsg(GetCtx(), "TargetListNotCreate");
            }


            return Msg.GetMsg(GetCtx(), "TargetListCreate");
        }




    }
}

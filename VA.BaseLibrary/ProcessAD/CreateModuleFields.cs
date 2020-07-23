using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
   public class CreateModuleFields:SvrProcess
    {
        protected override void Prepare()
        {

        }

        protected override string DoIt()
        {

            if (GetRecord_ID() == 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_TabNotFound");
            }

            MModuleTab mTab = new MModuleTab(GetCtx(), GetRecord_ID(), null);

            InsertORUpdateFields(mTab.GetAD_Tab_ID(), mTab);

            return "";
        }


        private string InsertORUpdateFields(int AD_Tab_ID, MModuleTab mTab)
        {
            MTab tab = new MTab(GetCtx(), AD_Tab_ID, null);
            MField[] fields = tab.GetFields(true, null);
            if (fields == null || fields.Length == 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_FieldsNotFound" + " " + tab.GetName());
            }
            string sql = "select AD_Field_ID, AD_MOduleField_ID FROM AD_MOduleField where IsActive='Y'  AND ad_moduletab_id=" + mTab.GetAD_ModuleTab_ID();
            IDataReader idr = DB.ExecuteReader(sql);
            DataTable dt = new DataTable();
            dt.Load(idr);
            idr.Close();

            Dictionary<int, int> existingFields = new Dictionary<int, int>();

            foreach (DataRow dr in dt.Rows)
            {
                existingFields[Convert.ToInt32(dr["AD_Field_ID"])] = Convert.ToInt32(dr["AD_MOduleField_ID"]);
            }

            for (int i = 0; i < fields.Length; i++)
            {
                if (!fields[i].IsDisplayed())
                {
                    continue;
                }
                MModuleField mField = null;
                if (existingFields.ContainsKey(fields[i].GetAD_Field_ID()))
                {
                    mField = new MModuleField(GetCtx(), existingFields[fields[i].GetAD_Field_ID()], null);
                }
                else
                {
                    mField = new MModuleField(GetCtx(), 0, null);
                    mField.SetAD_Field_ID(fields[i].GetAD_Field_ID());
                    mField.SetAD_ModuleTab_ID(mTab.GetAD_ModuleTab_ID());
                }

                mField.SetName(fields[i].GetName());
                mField.SetDescription(fields[i].GetDescription());

                if (mField.Save())
                {

                }
            }

            return "";

        }


    }
}

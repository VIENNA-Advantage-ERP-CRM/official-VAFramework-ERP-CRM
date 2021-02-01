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

            MVAFModuleTab mTab = new MVAFModuleTab(GetCtx(), GetRecord_ID(), null);

            InsertORUpdateFields(mTab.GetVAF_Tab_ID(), mTab);

            return "";
        }


        private string InsertORUpdateFields(int VAF_Tab_ID, MVAFModuleTab mTab)
        {
            MVAFTab tab = new MVAFTab(GetCtx(), VAF_Tab_ID, null);
            MVAFField[] fields = tab.GetFields(true, null);
            if (fields == null || fields.Length == 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_FieldsNotFound" + " " + tab.GetName());
            }
            string sql = "select VAF_Field_ID, VAF_ModuleField_ID FROM VAF_ModuleField where IsActive='Y'  AND VAF_ModuleTab_id=" + mTab.GetVAF_ModuleTab_ID();
            IDataReader idr = DB.ExecuteReader(sql);
            DataTable dt = new DataTable();
            dt.Load(idr);
            idr.Close();

            Dictionary<int, int> existingFields = new Dictionary<int, int>();

            foreach (DataRow dr in dt.Rows)
            {
                existingFields[Convert.ToInt32(dr["VAF_Field_ID"])] = Convert.ToInt32(dr["VAF_ModuleField_ID"]);
            }

            for (int i = 0; i < fields.Length; i++)
            {
                if (!fields[i].IsDisplayed())
                {
                    continue;
                }
                MVAFModuleField mField = null;
                if (existingFields.ContainsKey(fields[i].GetVAF_Field_ID()))
                {
                    mField = new MVAFModuleField(GetCtx(), existingFields[fields[i].GetVAF_Field_ID()], null);
                }
                else
                {
                    mField = new MVAFModuleField(GetCtx(), 0, null);
                    mField.SetVAF_Field_ID(fields[i].GetVAF_Field_ID());
                    mField.SetVAF_ModuleTab_ID(mTab.GetVAF_ModuleTab_ID());
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

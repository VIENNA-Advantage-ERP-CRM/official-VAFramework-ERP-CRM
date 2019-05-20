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
    public class CreateModuleTabAndFields : SvrProcess
    {
        int _windowNo = 0;
        protected override void Prepare()
        {

        }

        protected override string DoIt()
        {
            MModuleWindow mWindow = new MModuleWindow(GetCtx(), GetRecord_ID(), null);
            _windowNo = mWindow.GetAD_Window_ID();

            if (_windowNo == 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_WindowNotFound");
            }

            MWindow window = new MWindow(GetCtx(), _windowNo, null);
            MTab[] tabs = window.GetTabs(false, null);
            if (tabs == null || tabs.Length == 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_TabNotFound");
            }

            string sql = "select AD_Tab_ID,AD_ModuleTab_ID FROM AD_ModuleTab WHERE isActive='Y' AND ad_modulewindow_id=" + mWindow.GetAD_ModuleWindow_ID();
            IDataReader idr = DB.ExecuteReader(sql);
            DataTable dt = new DataTable();
            dt.Load(idr);
            idr.Close();

            Dictionary<int, int> existingTabs = new Dictionary<int, int>();

            foreach (DataRow dr in dt.Rows)
            {
                existingTabs[Convert.ToInt32(dr["AD_Tab_ID"])] = Convert.ToInt32(dr["AD_ModuleTab_ID"]);
            }

            for (int i = 0; i < tabs.Length; i++)
            {
                MModuleTab mTab = null;
                if (existingTabs.ContainsKey(tabs[i].GetAD_Tab_ID()))
                {
                    mTab = new MModuleTab(GetCtx(), existingTabs[tabs[i].GetAD_Tab_ID()], null);
                    InsertORUpdateFields(tabs[i].GetAD_Tab_ID(), mTab);
                }
                else
                {
                    mTab = new MModuleTab(GetCtx(), 0, null);

                    mTab.SetAD_Tab_ID(tabs[i].GetAD_Tab_ID());
                    mTab.SetAD_ModuleWindow_ID(GetRecord_ID());
                    if (mTab.Save())
                    {
                        InsertORUpdateFields(tabs[i].GetAD_Tab_ID(), mTab);
                    }
                }
            }

            return "Done";
        }

        private string InsertORUpdateFields(int AD_Tab_ID, MModuleTab mTab)
        {
            MTab tab = new MTab(GetCtx(), AD_Tab_ID, null);
            MField[] fields = tab.GetFields(true, null);
            if (fields == null || fields.Length == 0)
            {
                return Msg.GetMsg(GetCtx(), "VIS_FieldsNotFound" + " " + tab.GetName());
            }
            string sql = "select AD_Field_ID, AD_MOduleField_ID FROM AD_MOduleField where IsActive='Y' AND ad_moduletab_id=" + mTab.GetAD_ModuleTab_ID();
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

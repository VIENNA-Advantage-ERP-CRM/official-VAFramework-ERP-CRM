using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Controller;

namespace VAdvantage.Model
{
    public class GridTabPanel
    {
        /** Value Object                */
        public GridTabPanelVO _vo;

        public GridTabPanel(GridTabPanelVO vo)
        {
            _vo = vo;
        }

        public int GetVAF_Tab_ID()
        {
            return _vo.VAF_Tab_ID;
        }

        public int GetVAF_TabPanel_ID()
        {
            return _vo.VAF_TabPanel_ID;
        }

        public int GetSeqNo()
        {
            return _vo.SeqNo;
        }

        public string GetName()
        {
            return _vo.Name;
        }

        public string GetClassName()
        {
            return _vo.Classname;
        }

        public string GetIconPath()
        {
            return _vo.IconPath;
        }

        public bool IsDefault()
        {
            return _vo.IsDefault;
        }

        public String GetExtraInfo()
        {
            return _vo.ExtraInfo;
        }



    }
}

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

        public int GetAD_Tab_ID()
        {
            return _vo.AD_Tab_ID;
        }

        public int GetAD_TabPanel_ID()
        {
            return _vo.AD_TabPanel_ID;
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

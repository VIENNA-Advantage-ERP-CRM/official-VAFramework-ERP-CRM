using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Controller;

namespace VAdvantage.Classes
{
    public class CardViewData
    {
        public int FieldGroupID { get; set; }
        public List<CardViewCol> IncludedCols { get; set; }
        public List<CardViewCondition> Conditions { get; set; }
        public int AD_CardView_ID { get; set; }

        public bool IsDefault { get; set; }

        public string Name { get; set; }
        public int AD_HeaderLayout_ID { get; set; }
        public string Style { get; set; }
        public string Padding { get; set; }
        public string groupSequence { get; set; }

        public List<HeaderPanelGrid> HeaderItems { get; set; }
    }

    public class CardViewCondition
    {
        public string Color { get; set; }
        public string ConditionValue
        {
            get;
            set;
        }
        public string FColor { get; set; }
    }

    public class CardViewCol
    {
        public int AD_Field_ID { get; set; }
        public int SeqNo { get; set; }
        public string HTMLStyle { get; set; }
        public bool HideIcon { get; set; }
        public bool HideText { get; set; }
    }

    public class CardsInfo
    {
        public string Name { get; set; }
        public int AD_CardView_ID { get; set; }

        public bool IsDefault { get; set; }
        public int created { get; set; }
    }
}

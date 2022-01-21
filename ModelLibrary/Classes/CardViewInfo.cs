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
        public string FieldGroupName { get; set; }
        public List<CardViewCol> IncludedCols { get; set; }
        public List<CardViewCondition> Conditions { get; set; }
        public int AD_CardView_ID { get; set; }

        public bool IsDefault { get; set; }

        public string Name { get; set; }
        public int AD_HeaderLayout_ID { get; set; }
        public string Style { get; set; }
        public string Padding { get; set; }
        public string GroupSequence { get; set; }
        public string ExcludedGroup { get; set; }
        public string OrderByClause { get; set; }
        public bool DisableWindowPageSize { get; set; }

        public List<HeaderPanelGrid> HeaderItems { get; set; }
        public List<CardGroupCount> GroupCount { get; set; }
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
        public int SortNo { get; set; }
        public string HTMLStyle { get; set; }
        public bool HideIcon { get; set; }
        public bool HideText { get; set; }
    }

    public class CardsInfo
    {
        public string Name { get; set; }
        public int AD_CardView_ID { get; set; }

        public bool IsDefault { get; set; }
        public int Created { get; set; }
    }

    public class CardGroupCount {
        public string Group { get; set; }
        public int Count { get; set; }
    }

}

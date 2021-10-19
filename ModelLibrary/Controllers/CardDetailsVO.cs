using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.Controller;

namespace VAdvantage.Model
{
    public class CardDetailsVO
    {

    }

    public class Cards
    {
        public int AD_CardView_ID { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public int AD_HeaderLayout_ID { get; set; }
        public int AD_Field_ID { get; set; }
        
        public CardViewData CardInfo { get; set; }

    }
}

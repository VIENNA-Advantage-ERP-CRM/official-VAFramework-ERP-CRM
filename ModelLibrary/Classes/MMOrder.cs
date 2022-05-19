using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Model;

namespace ModelLibrary.Classes
{
    public class MMOrder : ModelAction
    {
        private MOrder morder;

        public MMOrder(PO order)
        {
            this.morder = (MOrder)order;
        }


        public override bool OnBeforeSave(bool newRecord, out bool skipBase)
        {
            skipBase = true;
           
            //code
            return false;
        }
    }
}

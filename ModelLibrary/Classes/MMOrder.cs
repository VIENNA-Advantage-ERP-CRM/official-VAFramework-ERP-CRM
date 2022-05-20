using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Model;

namespace ModelLibrary.Classes
{
    /// <summary>
    /// MMOrder extendable class 
    /// action 
    /// </summary>
    public class MMOrder : ModelAction
    {
        private MOrder morder;

        public MMOrder(PO order)
        {
            this.morder = (MOrder)order;
        }

        /// <summary>
        /// On before save function
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="skipBase"></param>
        /// <returns></returns>
        public override bool BeforeSave(bool newRecord, out bool skipBase)
        {
            skipBase = true;
           
            //code
            return false;
        }
    }
}

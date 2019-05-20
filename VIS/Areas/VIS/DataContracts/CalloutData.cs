using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.DataContracts
{
    public class ProductDataOut
    {
        public decimal PriceList { get; set; }
        public decimal PriceLimit { get; set; }
        public decimal PriceStd { get; set; }
        public decimal PriceActual { get; set; }
        public decimal PriceEntered { get; set; }
        public int C_Currency_ID { get; set; }
        public decimal Discount { get; set; }
        public decimal LineAmt { get; set; }
        public int C_UOM_ID { get; set; }
        public int QtyOrdered { get; set; }
        public bool EnforcePriceLimit { get; set; }
        public bool DiscountSchema { get; set; }
        public bool IsStocked { get; set; }
    }
}
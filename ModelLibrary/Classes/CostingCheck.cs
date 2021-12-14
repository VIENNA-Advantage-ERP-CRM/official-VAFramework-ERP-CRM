﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace ModelLibrary.Classes
{
    public class CostingCheck
    {
        protected internal VLogger log = null;
        private Ctx _ctx = null;
        private int AD_Client_ID = 0, AD_Org_ID = 0, M_ASI_ID = 0;
        public MProduct product = null;
        public MInventory inventory = null;
        public MInventoryLine inventoryLine = null;
        public MInOut inout = null;
        public MInOutLine inoutline = null;
        public MMovement movement = null;
        public MMovementLine movementline = null;
        public MInvoice invoice = null;
        public MInvoiceLine invoiceline = null;
        public MOrder order = null;
        public MOrderLine orderline = null;
        public PO po = null;
        private Decimal Price = 0, Qty = 0;
        public String costingMethod = String.Empty;
        public int costingElement = 0;
        public String costinglevel = String.Empty;
        public bool isReversal = false;
        public DataSet dsAccountingSchema = null;
        public CostingCheck(Ctx ctx)
        {
            _ctx = ctx;
            log = VLogger.GetVLogger(this.GetType().FullName);
        }

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            return;
        }

        public void SetProduct(MProduct _product)
        {
            product = _product;
            return;
        }

        public void SetInventory(MInventory _inventory)
        {
            inventory = _inventory;
            return;
        }



    }
}

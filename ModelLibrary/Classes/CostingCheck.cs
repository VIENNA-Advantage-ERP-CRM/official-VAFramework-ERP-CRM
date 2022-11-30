/********************************************************
 * Project Name   : VAdvantage
 * Module Name    : ModelLibrary
 * Class Name     : CostingCheck
 * Purpose        : costing checks
 * Class Used     : none
 * Chronological  : Development
 * Amit           : 12-Jan-2022
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace ModelLibrary.Classes
{
    public class CostingCheck
    {
        protected internal VLogger log = null;
        private Ctx _ctx = null;
        public int AD_Client_ID = 0, AD_Org_ID = 0, AD_OrgTo_ID = 0, M_ASI_ID = 0, M_Warehouse_ID = 0, M_WarehouseTo_ID = 0;
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
        public int costingElement = 0, M_CostType_ID = 0;
        public int definedCostingElement = 0; /* Costing Element ID against selected Costing Method on Product Category or Accounting Schema*/
        public int Lifo_ID = 0, Fifo_ID = 0;
        public String costinglevel = String.Empty;
        public String MMPolicy = String.Empty;
        public bool? isReversal;
        public DataSet dsAccountingSchema = null;
        public String isMatchFromForm = "N";
        public DateTime? movementDate = null;
        private StringBuilder query = new StringBuilder();
        public int M_Transaction_ID = 0, M_TransactionTo_ID = 0;
        public string errorMessage = String.Empty;
        public decimal? onHandQty = null;
        public bool IsCostCalculationfromProcess = false;
        public decimal? currentQtyonQueue = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        public CostingCheck(Ctx ctx)
        {
            _ctx = ctx;
            log = VLogger.GetVLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Setter Property of Ctx
        /// </summary>
        /// <param name="ctx">ctx</param>
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            return;
        }

        /// <summary>
        /// Setter Property of Product
        /// </summary>
        /// <param name="_product">Product</param>
        public void SetProduct(MProduct _product)
        {
            product = _product;
            return;
        }

        /// <summary>
        /// Setter Property of Inventory
        /// </summary>
        /// <param name="_inventory">inventory</param>
        public void SetInventory(MInventory _inventory)
        {
            inventory = _inventory;
            return;
        }

        public void GetLifoAndFIFoID(int AD_Client_ID)
        {
            query.Clear();
            query.Append(@"SELECT M_CostElement_ID, CostingMethod FROM M_CostElement WHERE IsActive = 'Y' AND 
                            CostingMethod IN ('" + MCostElement.COSTINGMETHOD_Fifo + "', '" + MCostElement.COSTINGMETHOD_Lifo + @"')
                            AND AD_Client_ID = " + AD_Client_ID);
            DataSet dsCostElement = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsCostElement != null && dsCostElement.Tables.Count > 0 && dsCostElement.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsCostElement.Tables[0].Rows.Count; i++)
                {
                    if (dsCostElement.Tables[0].Rows[i]["CostingMethod"].ToString().Equals(MCostElement.COSTINGMETHOD_Fifo))
                    {
                        Fifo_ID = Convert.ToInt32(dsCostElement.Tables[0].Rows[i]["M_CostElement_ID"]);
                    }
                    else
                    {
                        Lifo_ID = Convert.ToInt32(dsCostElement.Tables[0].Rows[i]["M_CostElement_ID"]);
                    }
                }
            }
        }

        public DataSet GetAccountingSchema(int AD_Client_ID)
        {
            query.Clear();
            query.Append(@"Select C_Acctschema_Id From C_Acctschema
                                WHERE Isactive = 'Y' AND C_Acctschema_Id = (SELECT C_Acctschema1_Id FROM Ad_Clientinfo 
                                WHERE Ad_Client_Id = " + AD_Client_ID + @" )
                                Union
                                Select C_Acctschema_Id From C_Acctschema Where Isactive = 'Y' And Ad_Client_Id = " + AD_Client_ID + @"
                                AND C_Acctschema_Id != (SELECT C_Acctschema1_Id FROM Ad_Clientinfo WHERE Ad_Client_Id = " + AD_Client_ID + " )");
            return DB.ExecuteDataset(query.ToString(), null, null);
        }

        public void ResetProperty()
        {
            AD_Client_ID = 0; 
            AD_Org_ID = 0; 
            AD_OrgTo_ID = 0; 
            M_ASI_ID = 0; 
            M_Warehouse_ID = 0; 
            M_WarehouseTo_ID = 0;
            product = null;
            inventoryLine = null;
            inoutline = null;
            movementline = null;
            invoiceline = null;
            order = null;
            orderline = null;
            po = null;
            costingMethod = String.Empty;
            costingElement = 0; 
            M_CostType_ID = 0;
            definedCostingElement = 0;
            Lifo_ID = 0; 
            Fifo_ID = 0;
            costinglevel = String.Empty;
            MMPolicy = String.Empty;
            isMatchFromForm = "N";
            movementDate = null;
            query.Clear();
            M_Transaction_ID = 0; M_TransactionTo_ID = 0;
            errorMessage = String.Empty;
        }

    }
}

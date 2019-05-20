/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_ProjectIssue
 * Chronological Development
 * Veena Pandey     17-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProjectIssue : X_C_ProjectIssue
    {
        /**	Parent				*/
        private MProject _parent = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ProjectIssue_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectIssue(Ctx ctx, int C_ProjectIssue_ID, Trx trxName)
            : base(ctx, C_ProjectIssue_ID, trxName)
        {
            if (C_ProjectIssue_ID == 0)
            {
                //	setC_Project_ID (0);
                //	setLine (0);
                //	setM_Locator_ID (0);
                //	setM_Product_ID (0);
                //	setMovementDate (new Timestamp(System.currentTimeMillis()));
                SetMovementQty(Env.ZERO);
                SetPosted(false);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MProjectIssue(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Parent Constructor
        /// </summary>
        /// <param name="project">parent</param>
        public MProjectIssue(MProject project)
            : this(project.GetCtx(), 0, project.Get_TrxName())
        {
            SetClientOrg(project.GetAD_Client_ID(), project.GetAD_Org_ID());
            SetC_Project_ID(project.GetC_Project_ID());	//	Parent
            SetLine(GetNextLine());
            _parent = project;
            //
            //	setM_Locator_ID (0);
            //	setM_Product_ID (0);
            //
            SetMovementDate(DateTime.Now);
            SetMovementQty(Env.ZERO);
            SetPosted(false);
            SetProcessed(false);
        }

        /// <summary>
        /// Get the next Line No
        /// </summary>
        /// <returns>next line no</returns>
        private int GetNextLine()
        {
            return DataBase.DB.GetSQLValue(Get_TrxName(),
                "SELECT COALESCE(MAX(Line),0)+10 FROM C_ProjectIssue WHERE C_Project_ID=@param1", GetC_Project_ID());
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>project</returns>
        public MProject GetParent()
        {
            if (_parent == null && GetC_Project_ID() != 0)
                _parent = new MProject(GetCtx(), GetC_Project_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Process Issue
        /// </summary>
        /// <returns>true if processed</returns>
        public bool Process()
        {
            if (!Save())
                return false;
            if (GetM_Product_ID() == 0)
            {
                log.Log(Level.SEVERE, "No Product");
                return false;
            }

            MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());

            //	If not a stocked Item nothing to do
            if (!product.IsStocked())
            {
                SetProcessed(true);
                return Save();
            }

            /** @todo Transaction */

            //	**	Create Material Transactions **
            MTransaction mTrx = new MTransaction(GetCtx(), GetAD_Org_ID(),
                MTransaction.MOVEMENTTYPE_WorkOrderPlus,
                GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(),
                Decimal.Negate(GetMovementQty()), GetMovementDate(), Get_TrxName());
            mTrx.SetC_ProjectIssue_ID(GetC_ProjectIssue_ID());
            //
            MLocator loc = MLocator.Get(GetCtx(), GetM_Locator_ID());
            if (MStorage.Add(GetCtx(), loc.GetM_Warehouse_ID(), GetM_Locator_ID(),
                    GetM_Product_ID(), GetM_AttributeSetInstance_ID(), GetM_AttributeSetInstance_ID(),
                    Decimal.Negate(GetMovementQty()), null, null, Get_TrxName()))
            {
                if (mTrx.Save(Get_TrxName()))
                {
                    SetProcessed(true);
                    if (Save())
                    {
                        return true;
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Issue not saved");		//	requires trx !!
                    }
                }
                else
                {
                    log.Log(Level.SEVERE, "Transaction not saved");	//	requires trx !!
                }
            }
            else
            {
                log.Log(Level.SEVERE, "Storage not updated");			//	OK
            }
            //
            return false;
        }

        /// <summary>
        /// Set Mandatory Values
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="movementQty">qty</param>
        public void SetMandatory(int M_Locator_ID, int M_Product_ID, Decimal MovementQty)
        {
            SetM_Locator_ID(M_Locator_ID);
            SetM_Product_ID(M_Product_ID);
            SetMovementQty(MovementQty);
        }
    }
}

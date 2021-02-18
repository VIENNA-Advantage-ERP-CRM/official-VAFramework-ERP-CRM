/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_ProjectSupply
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
    public class MVABProjectSupply : X_VAB_ProjectSupply
    {
        /**	Parent				*/
        private MVABProject _parent = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_ProjectSupply_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABProjectSupply(Ctx ctx, int VAB_ProjectSupply_ID, Trx trxName)
            : base(ctx, VAB_ProjectSupply_ID, trxName)
        {
            if (VAB_ProjectSupply_ID == 0)
            {
                //	setVAB_Project_ID (0);
                //	setLine (0);
                //	setVAM_Locator_ID (0);
                //	setVAM_Product_ID (0);
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
        public MVABProjectSupply(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Parent Constructor
        /// </summary>
        /// <param name="project">parent</param>
        public MVABProjectSupply(MVABProject project)
            : this(project.GetCtx(), 0, project.Get_TrxName())
        {
            SetClientOrg(project.GetVAF_Client_ID(), project.GetVAF_Org_ID());
            SetVAB_Project_ID(project.GetVAB_Project_ID());	//	Parent
            SetLine(GetNextLine());
            _parent = project;
            //
            //	setVAM_Locator_ID (0);
            //	setVAM_Product_ID (0);
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
                "SELECT COALESCE(MAX(Line),0)+10 FROM VAB_ProjectSupply WHERE VAB_Project_ID=@param1", GetVAB_Project_ID());
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>project</returns>
        public MVABProject GetParent()
        {
            if (_parent == null && GetVAB_Project_ID() != 0)
                _parent = new MVABProject(GetCtx(), GetVAB_Project_ID(), Get_TrxName());
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
            if (GetVAM_Product_ID() == 0)
            {
                log.Log(Level.SEVERE, "No Product");
                return false;
            }

            MProduct product = MProduct.Get(GetCtx(), GetVAM_Product_ID());

            //	If not a stocked Item nothing to do
            if (!product.IsStocked())
            {
                SetProcessed(true);
                return Save();
            }

            /** @todo Transaction */

            //	**	Create Material Transactions **
            MTransaction mTrx = new MTransaction(GetCtx(), GetVAF_Org_ID(),
                MTransaction.MOVEMENTTYPE_WorkOrderPlus,
                GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(),
                Decimal.Negate(GetMovementQty()), GetMovementDate(), Get_TrxName());
            mTrx.SetVAB_ProjectSupply_ID(GetVAB_ProjectSupply_ID());
            //
            MVAMLocator loc = MVAMLocator.Get(GetCtx(), GetVAM_Locator_ID());
            if (MStorage.Add(GetCtx(), loc.GetVAM_Warehouse_ID(), GetVAM_Locator_ID(),
                    GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), GetVAM_PFeature_SetInstance_ID(),
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
        /// <param name="VAM_Locator_ID">locator</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="movementQty">qty</param>
        public void SetMandatory(int VAM_Locator_ID, int VAM_Product_ID, Decimal MovementQty)
        {
            SetVAM_Locator_ID(VAM_Locator_ID);
            SetVAM_Product_ID(VAM_Product_ID);
            SetMovementQty(MovementQty);
        }
    }
}

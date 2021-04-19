/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAGLDistributionLine
 * Purpose        : GL Distribution Line Model
 * Class Used     : X_VAGL_DistributionLine class
 * Chronological    Development
 * Deepak           19-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAGLDistributionLine : X_VAGL_DistributionLine
    {
       /// <summary>
       /// Standard Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="VAGL_DistributionLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAGLDistributionLine(Ctx ctx, int VAGL_DistributionLine_ID, Trx trxName):base(ctx, VAGL_DistributionLine_ID, trxName)
        {
            //super(ctx, VAGL_DistributionLine_ID, trxName);
            if (VAGL_DistributionLine_ID == 0)
            {
                //	setVAGL_Distribution_ID (0);		//	Parent
                //	setLine (0);
                //
                SetOverwriteAcct(false);
                SetOverwriteActivity(false);
                SetOverwriteBPartner(false);
                SetOverwriteCampaign(false);
                SetOverwriteLocFrom(false);
                SetOverwriteLocTo(false);
                SetOverwriteOrg(false);
                SetOverwriteOrgTrx(false);
                SetOverwriteProduct(false);
                SetOverwriteProject(false);
                SetOverwriteSalesRegion(false);
                SetOverwriteUser1(false);
                SetOverwriteUser2(false);
                //
                SetPercentDistribution(Env.ZERO);
            }
        }	//	MVAGLDistributionLine

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAGLDistributionLine(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MVAGLDistributionLine

        /**	The Parent						*/
        private MVAGLDistribution _parent = null;
        /** The Amount						*/
        private Decimal? _amt = null;
        /** The Base Account				*/
        private MVABAccount _account = null;

        /// <summary>
        ///	Get Parent
        /// </summary>
        /// <returns>Returns the parent.</returns>
        public MVAGLDistribution GetParent()
        {
            if (_parent == null)
            {
                _parent = new MVAGLDistribution(GetCtx(), GetVAGL_Distribution_ID(), Get_TrxName());
            }
            return _parent;
        }	//	getParent

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">The parent to set.</param>
        public void SetParent(MVAGLDistribution parent)
        {
            _parent = parent;
        }	//	setParent

        /// <summary>
        /// Set Account
        /// </summary>
        /// <param name="acct">account</param>
        public void SetAccount(MVABAccount acct)
        {
            _account = acct;
        }	//	setAccount

        /// <summary>
        ///	Get Account Combination based on Account and Overwrite
        /// </summary>
        /// <returns>account</returns>
        public MVABAccount GetAccount()
        {
            MVABAccount acct = MVABAccount.Get(GetCtx(),
                _account.GetVAF_Client_ID(),
                IsOverwriteOrg() && GetOrg_ID() != 0 ? GetOrg_ID() : _account.GetVAF_Org_ID(),
                _account.GetVAB_AccountBook_ID(),
                IsOverwriteAcct() && GetAccount_ID() != 0 ? GetAccount_ID() : _account.GetAccount_ID(),
                    _account.GetVAB_SubAcct_ID(),
                //	
                IsOverwriteProduct() ? GetVAM_Product_ID() : _account.GetVAM_Product_ID(),
                IsOverwriteBPartner() ? GetVAB_BusinessPartner_ID() : _account.GetVAB_BusinessPartner_ID(),
                IsOverwriteOrgTrx() ? GetVAF_OrgTrx_ID() : _account.GetVAF_OrgTrx_ID(),
                IsOverwriteLocFrom() ? GetVAB_LocFrom_ID() : _account.GetVAB_LocFrom_ID(),
                IsOverwriteLocTo() ? GetVAB_LocTo_ID() : _account.GetVAB_LocTo_ID(),
                IsOverwriteSalesRegion() ? GetVAB_SalesRegionState_ID() : _account.GetVAB_SalesRegionState_ID(),
                IsOverwriteProject() ? GetVAB_Project_ID() : _account.GetVAB_Project_ID(),
                IsOverwriteCampaign() ? GetVAB_Promotion_ID() : _account.GetVAB_Promotion_ID(),
                IsOverwriteActivity() ? GetVAB_BillingCode_ID() : _account.GetVAB_BillingCode_ID(),
                IsOverwriteUser1() ? GetUser1_ID() : _account.GetUser1_ID(),
                IsOverwriteUser2() ? GetUser2_ID() : _account.GetUser2_ID(),
                    _account.GetUserElement1_ID(),
                    _account.GetUserElement2_ID());
            return acct;
        }	//	setAccount


        /// <summary>
        /// Get Distribution Amount
        /// </summary>
        /// <returns>Returns the amt.</returns>
        public Decimal GetAmt()
        {
            return Utility.Util.GetValueOfDecimal( _amt);
        }	//	getAmt

        /// <summary>
        /// Set Distribution Amount
        /// </summary>
        /// <param name="amt">The amt to set.</param>
        public void SetAmt(Decimal amt)
        {
            _amt = amt;
        }	//	setAmt

        /// <summary>
        /// Set Distribution Amount
        /// </summary>
        /// <param name="amt">The amt to set to be multiplied by percent.</param>
        /// <param name="precision">precision</param>
        public void CalculateAmt(Decimal amt, int precision)
        {
            //_amt = amt.multiply(getPercentDistribution());
            _amt =Decimal.Multiply( amt,GetPercentDistribution());
            //_amt = _amt.divide(Env.ONEHUNDRED, precision, BigDecimal.ROUND_HALF_UP);
            _amt = Decimal.Round(Decimal.Divide(Utility.Util.GetValueOfDecimal(_amt), Env.ONEHUNDRED), precision, MidpointRounding.AwayFromZero);
        }	//	setAmt
       


        /// <summary>
        /// 	Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected Boolean beforeSave(Boolean newRecord)
        {
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM VAGL_DistributionLine WHERE VAGL_Distribution_ID=@Param1";
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetVAGL_Distribution_ID());
                SetLine(ii);
            }
            //	Reset not selected Overwrite
            if (!IsOverwriteAcct() && GetAccount_ID() != 0)
            {
                SetAccount_ID(0);
            }
            if (!IsOverwriteActivity() && GetVAB_BillingCode_ID() != 0)
            {
                SetVAB_BillingCode_ID(0);
            }
            if (!IsOverwriteBPartner() && GetVAB_BusinessPartner_ID() != 0)
            {
                SetVAB_BusinessPartner_ID(0);
            }
            if (!IsOverwriteCampaign() && GetVAB_Promotion_ID() != 0)
            {
                SetVAB_Promotion_ID(0);
            }
            if (!IsOverwriteLocFrom() && GetVAB_LocFrom_ID() != 0)
            {
                SetVAB_LocFrom_ID(0);
            }
            if (!IsOverwriteLocTo() && GetVAB_LocTo_ID() != 0)
            {
                SetVAB_LocTo_ID(0);
            }
            if (!IsOverwriteOrg() && GetOrg_ID() != 0)
            {
                SetOrg_ID(0);
            }
            if (!IsOverwriteOrgTrx() && GetVAF_OrgTrx_ID() != 0)
            {
                SetVAF_OrgTrx_ID(0);
            }
            if (!IsOverwriteProduct() && GetVAM_Product_ID() != 0)
            {
                SetVAM_Product_ID(0);
            }
            if (!IsOverwriteProject() && GetVAB_Project_ID() != 0)
            {
                SetVAB_Project_ID(0);
            }
            if (!IsOverwriteSalesRegion() && GetVAB_SalesRegionState_ID() != 0)
            {
                SetVAB_SalesRegionState_ID(0);
            }
            if (!IsOverwriteUser1() && GetUser1_ID() != 0)
            {
                SetUser1_ID(0);
            }
            if (!IsOverwriteUser2() && GetUser2_ID() != 0)
            {
                SetUser2_ID(0);
            }

            //	Account Overwrite cannot be 0
            if (IsOverwriteAcct() && GetAccount_ID() == 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@Account_ID@ = 0"));
                return false;
            }
            //	Org Overwrite cannot be 0
            if (IsOverwriteOrg() && GetOrg_ID() == 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@Org_ID@ = 0"));
                return false;
            } 
            return true;
        }	//	beforeSave

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected Boolean afterSave(Boolean newRecord, Boolean success)
        {
            GetParent();
            _parent.Validate();
            _parent.Save();
            return success;
        }	//	afterSave

    }	//	MVAGLDistributionLine

}

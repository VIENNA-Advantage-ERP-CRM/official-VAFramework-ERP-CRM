/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistributionLine
 * Purpose        : GL Distribution Line Model
 * Class Used     : X_GL_DistributionLine class
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
    public class MDistributionLine : X_GL_DistributionLine
    {
       /// <summary>
       /// Standard Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="GL_DistributionLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDistributionLine(Ctx ctx, int GL_DistributionLine_ID, Trx trxName):base(ctx, GL_DistributionLine_ID, trxName)
        {
            //super(ctx, GL_DistributionLine_ID, trxName);
            if (GL_DistributionLine_ID == 0)
            {
                //	setGL_Distribution_ID (0);		//	Parent
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
        }	//	MDistributionLine

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MDistributionLine(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MDistributionLine

        /**	The Parent						*/
        private MDistribution _parent = null;
        /** The Amount						*/
        private Decimal? _amt = null;
        /** The Base Account				*/
        private MAccount _account = null;

        /// <summary>
        ///	Get Parent
        /// </summary>
        /// <returns>Returns the parent.</returns>
        public MDistribution GetParent()
        {
            if (_parent == null)
            {
                _parent = new MDistribution(GetCtx(), GetGL_Distribution_ID(), Get_TrxName());
            }
            return _parent;
        }	//	getParent

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">The parent to set.</param>
        public void SetParent(MDistribution parent)
        {
            _parent = parent;
        }	//	setParent

        /// <summary>
        /// Set Account
        /// </summary>
        /// <param name="acct">account</param>
        public void SetAccount(MAccount acct)
        {
            _account = acct;
        }	//	setAccount

        /// <summary>
        ///	Get Account Combination based on Account and Overwrite
        /// </summary>
        /// <returns>account</returns>
        public MAccount GetAccount()
        {
            MAccount acct = MAccount.Get(GetCtx(),
                _account.GetAD_Client_ID(),
                IsOverwriteOrg() && GetOrg_ID() != 0 ? GetOrg_ID() : _account.GetAD_Org_ID(),
                _account.GetC_AcctSchema_ID(),
                IsOverwriteAcct() && GetAccount_ID() != 0 ? GetAccount_ID() : _account.GetAccount_ID(),
                    _account.GetC_SubAcct_ID(),
                //	
                IsOverwriteProduct() ? GetM_Product_ID() : _account.GetM_Product_ID(),
                IsOverwriteBPartner() ? GetC_BPartner_ID() : _account.GetC_BPartner_ID(),
                IsOverwriteOrgTrx() ? GetAD_OrgTrx_ID() : _account.GetAD_OrgTrx_ID(),
                IsOverwriteLocFrom() ? GetC_LocFrom_ID() : _account.GetC_LocFrom_ID(),
                IsOverwriteLocTo() ? GetC_LocTo_ID() : _account.GetC_LocTo_ID(),
                IsOverwriteSalesRegion() ? GetC_SalesRegion_ID() : _account.GetC_SalesRegion_ID(),
                IsOverwriteProject() ? GetC_Project_ID() : _account.GetC_Project_ID(),
                IsOverwriteCampaign() ? GetC_Campaign_ID() : _account.GetC_Campaign_ID(),
                IsOverwriteActivity() ? GetC_Activity_ID() : _account.GetC_Activity_ID(),
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
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM GL_DistributionLine WHERE GL_Distribution_ID=@Param1";
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetGL_Distribution_ID());
                SetLine(ii);
            }
            //	Reset not selected Overwrite
            if (!IsOverwriteAcct() && GetAccount_ID() != 0)
            {
                SetAccount_ID(0);
            }
            if (!IsOverwriteActivity() && GetC_Activity_ID() != 0)
            {
                SetC_Activity_ID(0);
            }
            if (!IsOverwriteBPartner() && GetC_BPartner_ID() != 0)
            {
                SetC_BPartner_ID(0);
            }
            if (!IsOverwriteCampaign() && GetC_Campaign_ID() != 0)
            {
                SetC_Campaign_ID(0);
            }
            if (!IsOverwriteLocFrom() && GetC_LocFrom_ID() != 0)
            {
                SetC_LocFrom_ID(0);
            }
            if (!IsOverwriteLocTo() && GetC_LocTo_ID() != 0)
            {
                SetC_LocTo_ID(0);
            }
            if (!IsOverwriteOrg() && GetOrg_ID() != 0)
            {
                SetOrg_ID(0);
            }
            if (!IsOverwriteOrgTrx() && GetAD_OrgTrx_ID() != 0)
            {
                SetAD_OrgTrx_ID(0);
            }
            if (!IsOverwriteProduct() && GetM_Product_ID() != 0)
            {
                SetM_Product_ID(0);
            }
            if (!IsOverwriteProject() && GetC_Project_ID() != 0)
            {
                SetC_Project_ID(0);
            }
            if (!IsOverwriteSalesRegion() && GetC_SalesRegion_ID() != 0)
            {
                SetC_SalesRegion_ID(0);
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

    }	//	MDistributionLine

}

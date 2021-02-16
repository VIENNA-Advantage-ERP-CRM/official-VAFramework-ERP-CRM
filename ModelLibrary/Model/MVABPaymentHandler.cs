/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_PaymentHandler
 * Chronological Development
 * Veena Pandey     23-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABPaymentHandler : X_VAB_PaymentHandler
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABPaymentHandler).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_PaymentHandler_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABPaymentHandler(Ctx ctx, int VAB_PaymentHandler_ID, Trx trxName)
            : base(ctx, VAB_PaymentHandler_ID, trxName)
        {
            if (VAB_PaymentHandler_ID == 0)
            {
                //	setVAB_Bank_Acct_ID (0);		//	Parent
                //	setUserID (null);
                //	setPassword (null);
                //	setHostAddress (null);
                //	setHostPort (0);
                SetCommission(Env.ZERO);
                SetAcceptVisa(false);
                SetAcceptMC(false);
                SetAcceptAMEX(false);
                SetAcceptDiners(false);
                SetCostPerTrx(Env.ZERO);
                SetAcceptCheck(false);
                SetRequireVV(false);
                SetAcceptCorporate(false);
                SetAcceptDiscover(false);
                SetAcceptATM(false);
                SetAcceptDirectDeposit(false);
                SetAcceptDirectDebit(false);
                //	setName (null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABPaymentHandler(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Does Payment Processor accepts tender / CC
        /// </summary>
        /// <param name="tenderType">tender type</param>
        /// <param name="creditCardType">credit card type</param>
        /// <returns>true if acceptes</returns>
        public bool Accepts(String tenderType, String creditCardType)
        {
            if ((MVABPayment.TENDERTYPE_DirectDeposit.Equals(tenderType) && IsAcceptDirectDeposit())
                || (MVABPayment.TENDERTYPE_DirectDebit.Equals(tenderType) && IsAcceptDirectDebit())
                || (MVABPayment.TENDERTYPE_Check.Equals(tenderType) && IsAcceptCheck())
                //
                || (MVABPayment.CREDITCARDTYPE_ATM.Equals(creditCardType) && IsAcceptATM())
                || (MVABPayment.CREDITCARDTYPE_Amex.Equals(creditCardType) && IsAcceptAMEX())
                || (MVABPayment.CREDITCARDTYPE_PurchaseCard.Equals(creditCardType) && IsAcceptCorporate())
                || (MVABPayment.CREDITCARDTYPE_Diners.Equals(creditCardType) && IsAcceptDiners())
                || (MVABPayment.CREDITCARDTYPE_Discover.Equals(creditCardType) && IsAcceptDiscover())
                || (MVABPayment.CREDITCARDTYPE_MasterCard.Equals(creditCardType) && IsAcceptMC())
                || (MVABPayment.CREDITCARDTYPE_Visa.Equals(creditCardType) && IsAcceptVisa()))
                return true;
            return false;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaymentProcessor[")
                .Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get BankAccount & PaymentProcessor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="tender">optional Tender see TENDER_</param>
        /// <param name="CCType">optional CC Type see CC_</param>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAB_Currency_ID">Currency (ignored)</param>
        /// <param name="amt">Amount (ignored)</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Array of BankAccount[0] & PaymentProcessor[1] or null</returns>
        public static MVABPaymentHandler[] Find(Ctx ctx, String tender, String CCType,
            int VAF_Client_ID, int VAB_Currency_ID, Decimal amt, Trx trxName)
        {
            List<MVABPaymentHandler> list = new List<MVABPaymentHandler>();
            StringBuilder sql = new StringBuilder("SELECT * "
                + "FROM VAB_PaymentHandler "
                + "WHERE VAF_Client_ID=@clid AND IsActive='Y'"				//	#1
                + " AND (VAB_Currency_ID IS NULL OR VAB_Currency_ID=@curid)"		//	#2
                + " AND (MinimumAmt IS NULL OR MinimumAmt = 0 OR MinimumAmt <= @amot)");	//	#3
            if (MVABPayment.TENDERTYPE_DirectDeposit.Equals(tender))
                sql.Append(" AND AcceptDirectDeposit='Y'");
            else if (MVABPayment.TENDERTYPE_DirectDebit.Equals(tender))
                sql.Append(" AND AcceptDirectDebit='Y'");
            else if (MVABPayment.TENDERTYPE_Check.Equals(tender))
                sql.Append(" AND AcceptCheck='Y'");
            //  CreditCards
            else if (MVABPayment.CREDITCARDTYPE_ATM.Equals(CCType))
                sql.Append(" AND AcceptATM='Y'");
            else if (MVABPayment.CREDITCARDTYPE_Amex.Equals(CCType))
                sql.Append(" AND AcceptAMEX='Y'");
            else if (MVABPayment.CREDITCARDTYPE_Visa.Equals(CCType))
                sql.Append(" AND AcceptVISA='Y'");
            else if (MVABPayment.CREDITCARDTYPE_MasterCard.Equals(CCType))
                sql.Append(" AND AcceptMC='Y'");
            else if (MVABPayment.CREDITCARDTYPE_Diners.Equals(CCType))
                sql.Append(" AND AcceptDiners='Y'");
            else if (MVABPayment.CREDITCARDTYPE_Discover.Equals(CCType))
                sql.Append(" AND AcceptDiscover='Y'");
            else if (MVABPayment.CREDITCARDTYPE_PurchaseCard.Equals(CCType))
                sql.Append(" AND AcceptCORPORATE='Y'");
            //
            try
            {
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@clid", VAF_Client_ID);
                param[1] = new SqlParameter("@curid", VAB_Currency_ID);
                param[2] = new SqlParameter("@amot", amt);
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVABPaymentHandler(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "find - " + sql, e);
                return null;
            }
            //
            if (list.Count == 0)
            {
                _log.Warning("find - not found - VAF_Client_ID=" + VAF_Client_ID
                    + ", VAB_Currency_ID=" + VAB_Currency_ID + ", Amt=" + amt);
            }
            else
            {
                _log.Fine("find - #" + list.Count + " - VAF_Client_ID=" + VAF_Client_ID
                    + ", VAB_Currency_ID=" + VAB_Currency_ID + ", Amt=" + amt);
            }
            MVABPaymentHandler[] retValue = new MVABPaymentHandler[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_PaymentProcessor
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
    public class MPaymentProcessor : X_C_PaymentProcessor
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MPaymentProcessor).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_PaymentProcessor_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPaymentProcessor(Ctx ctx, int C_PaymentProcessor_ID, Trx trxName)
            : base(ctx, C_PaymentProcessor_ID, trxName)
        {
            if (C_PaymentProcessor_ID == 0)
            {
                //	setC_BankAccount_ID (0);		//	Parent
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
        public MPaymentProcessor(Ctx ctx, DataRow dr, Trx trxName)
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
            if ((MPayment.TENDERTYPE_DirectDeposit.Equals(tenderType) && IsAcceptDirectDeposit())
                || (MPayment.TENDERTYPE_DirectDebit.Equals(tenderType) && IsAcceptDirectDebit())
                || (MPayment.TENDERTYPE_Check.Equals(tenderType) && IsAcceptCheck())
                //
                || (MPayment.CREDITCARDTYPE_ATM.Equals(creditCardType) && IsAcceptATM())
                || (MPayment.CREDITCARDTYPE_Amex.Equals(creditCardType) && IsAcceptAMEX())
                || (MPayment.CREDITCARDTYPE_PurchaseCard.Equals(creditCardType) && IsAcceptCorporate())
                || (MPayment.CREDITCARDTYPE_Diners.Equals(creditCardType) && IsAcceptDiners())
                || (MPayment.CREDITCARDTYPE_Discover.Equals(creditCardType) && IsAcceptDiscover())
                || (MPayment.CREDITCARDTYPE_MasterCard.Equals(creditCardType) && IsAcceptMC())
                || (MPayment.CREDITCARDTYPE_Visa.Equals(creditCardType) && IsAcceptVisa()))
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
        /// <param name="AD_Client_ID">client</param>
        /// <param name="C_Currency_ID">Currency (ignored)</param>
        /// <param name="amt">Amount (ignored)</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Array of BankAccount[0] & PaymentProcessor[1] or null</returns>
        public static MPaymentProcessor[] Find(Ctx ctx, String tender, String CCType,
            int AD_Client_ID, int C_Currency_ID, Decimal amt, Trx trxName)
        {
            List<MPaymentProcessor> list = new List<MPaymentProcessor>();
            StringBuilder sql = new StringBuilder("SELECT * "
                + "FROM C_PaymentProcessor "
                + "WHERE AD_Client_ID=@clid AND IsActive='Y'"				//	#1
                + " AND (C_Currency_ID IS NULL OR C_Currency_ID=@curid)"		//	#2
                + " AND (MinimumAmt IS NULL OR MinimumAmt = 0 OR MinimumAmt <= @amot)");	//	#3
            if (MPayment.TENDERTYPE_DirectDeposit.Equals(tender))
                sql.Append(" AND AcceptDirectDeposit='Y'");
            else if (MPayment.TENDERTYPE_DirectDebit.Equals(tender))
                sql.Append(" AND AcceptDirectDebit='Y'");
            else if (MPayment.TENDERTYPE_Check.Equals(tender))
                sql.Append(" AND AcceptCheck='Y'");
            //  CreditCards
            else if (MPayment.CREDITCARDTYPE_ATM.Equals(CCType))
                sql.Append(" AND AcceptATM='Y'");
            else if (MPayment.CREDITCARDTYPE_Amex.Equals(CCType))
                sql.Append(" AND AcceptAMEX='Y'");
            else if (MPayment.CREDITCARDTYPE_Visa.Equals(CCType))
                sql.Append(" AND AcceptVISA='Y'");
            else if (MPayment.CREDITCARDTYPE_MasterCard.Equals(CCType))
                sql.Append(" AND AcceptMC='Y'");
            else if (MPayment.CREDITCARDTYPE_Diners.Equals(CCType))
                sql.Append(" AND AcceptDiners='Y'");
            else if (MPayment.CREDITCARDTYPE_Discover.Equals(CCType))
                sql.Append(" AND AcceptDiscover='Y'");
            else if (MPayment.CREDITCARDTYPE_PurchaseCard.Equals(CCType))
                sql.Append(" AND AcceptCORPORATE='Y'");
            //
            try
            {
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@clid", AD_Client_ID);
                param[1] = new SqlParameter("@curid", C_Currency_ID);
                param[2] = new SqlParameter("@amot", amt);
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MPaymentProcessor(ctx, dr, trxName));
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
                _log.Warning("find - not found - AD_Client_ID=" + AD_Client_ID
                    + ", C_Currency_ID=" + C_Currency_ID + ", Amt=" + amt);
            }
            else
            {
                _log.Fine("find - #" + list.Count + " - AD_Client_ID=" + AD_Client_ID
                    + ", C_Currency_ID=" + C_Currency_ID + ", Amt=" + amt);
            }
            MPaymentProcessor[] retValue = new MPaymentProcessor[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
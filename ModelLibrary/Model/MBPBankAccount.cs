/********************************************************
 * Module Name    : Vframwork
 * Purpose        : BP Bank Account Model
 * Class Used     : X_C_BP_BankAccount
 * Chronological Development
 * Raghunandan    24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using System.Data;
using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MBPBankAccount : X_C_BP_BankAccount
    {
        /** Bank Link			*/
        private MBank _bank = null;
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MBPBankAccount).FullName);
        /**
         * 	Get Accounst Of BPartner
         *	@param ctx context
         *	@param C_BPartner_ID bpartner
         *	@return
         */
        public static MBPBankAccount[] GetOfBPartner(Ctx ctx, int C_BPartner_ID)
        {
            String sql = "SELECT * FROM C_BP_BankAccount WHERE C_BPartner_ID=" + C_BPartner_ID
                + " AND IsActive='Y'";
            List<MBPBankAccount> list = new List<MBPBankAccount>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                 idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MBPBankAccount(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            MBPBankAccount[] retValue = new MBPBankAccount[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Constructor
         *	@param ctx context
         *	@param C_BP_BankAccount_ID BP bank account
         *	@param trxName transaction
         */
        public MBPBankAccount(Ctx ctx, int C_BP_BankAccount_ID, Trx trxName)
            : base(ctx, C_BP_BankAccount_ID, trxName)
        {

            if (C_BP_BankAccount_ID == 0)
            {
                //	setC_BPartner_ID (0);
                SetIsACH(false);
                SetBPBankAcctUse(BPBANKACCTUSE_Both);
            }
        }

        /**
         * 	Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MBPBankAccount(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Constructor
         *	@param ctx context
         * 	@param bp BP
         *	@param bpc BP Contact
         * 	@param location Location
         */
        public MBPBankAccount(Ctx ctx, MBPartner bp, MUser bpc, MLocation location)
            : this(ctx, 0, bp.Get_TrxName())
        {

            SetIsACH(false);
            //
            SetC_BPartner_ID(bp.GetC_BPartner_ID());
            //
            SetA_Name(bpc.GetName());
            SetA_EMail(bpc.GetEMail());
            //
            SetA_Street(location.GetAddress1());
            SetA_City(location.GetCity());
            SetA_Zip(location.GetPostal());
            SetA_State(location.GetRegionName(true));
            SetA_Country(location.GetCountryName());
        }

        /**
         * 	Is Direct Deposit
         *	@return true if dd
         */
        public bool IsDirectDeposit()
        {
            if (!IsACH())
                return false;
            String s = GetBPBankAcctUse();
            if (s == null)
                return true;
            return (s.Equals(BPBANKACCTUSE_Both) || s.Equals(BPBANKACCTUSE_DirectDeposit));
        }

        /**
         * 	Is Direct Debit
         *	@return true if dd
         */
        public bool IsDirectDebit()
        {
            if (!IsACH())
                return false;
            String s = GetBPBankAcctUse();
            if (s == null)
                return true;
            return (s.Equals(BPBANKACCTUSE_Both) || s.Equals(BPBANKACCTUSE_DirectDebit));
        }


        /**
         * 	Get Bank
         *	@return bank
         */
        public MBank GetBank()
        {
            int C_Bank_ID = GetC_Bank_ID();
            if (C_Bank_ID == 0)
                return null;
            if (_bank == null)
                _bank = new MBank(GetCtx(), C_Bank_ID, Get_TrxName());
            return _bank;
        }

        /**
         * 	Get Routing No
         *	@return routing No
         */
        public new String GetRoutingNo()
        {
            MBank bank = GetBank();
            String rt = base.GetRoutingNo();
            if (bank != null)
                rt = bank.GetRoutingNo();
            return rt;
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	maintain routing on bank level
            if (IsACH() && GetBank() != null)
                SetRoutingNo(null);
            //	Verify Bank
            MBank bank = GetBank();
            if (bank != null)
            {
                BankVerificationInterface verify = bank.GetVerificationClass();
                if (verify != null)
                {
                    String errorMsg = verify.VerifyRoutingNo(bank.GetC_Country_ID(), GetRoutingNo());
                    if (errorMsg != null)
                    {
                        log.SaveError("Error", "@Invalid@ @RoutingNo@ " + errorMsg);
                        return false;
                    }
                    //
                    errorMsg = verify.VerifyAccountNo(bank, GetAccountNo());
                    if (errorMsg != null)
                    {
                        log.SaveError("Error", "@Invalid@ @AccountNo@ " + errorMsg);
                        return false;
                    }
                    errorMsg = verify.VerifyBBAN(bank, GetBBAN());
                    if (errorMsg != null)
                    {
                        log.SaveError("Error", "@Invalid@ @BBAN@ " + errorMsg);
                        return false;
                    }
                    errorMsg = verify.VerifyIBAN(bank, GetIBAN());
                    if (errorMsg != null)
                    {
                        log.SaveError("Error", "@Invalid@ @IBAN@ " + errorMsg);
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         *	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBP_BankAccount[")
                .Append(Get_ID())
                .Append(", Name=").Append(GetA_Name())
                .Append("]");
            return sb.ToString();
        }
    }
}

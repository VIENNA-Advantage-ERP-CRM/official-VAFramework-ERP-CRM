/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MFactAcct
 * Purpose        : Accounting Fact Model
 * Class Used     : X_Fact_Acct
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MFactAcct : X_Fact_Acct
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MFactAcct).FullName);
        /*	Delete Accounting
        *	@param VAF_TableView_ID table
        *	@param Record_ID record
        *	@param trxName transaction
        *	@return number of rows or -1 for error
        */
        public static int Delete(int VAF_TableView_ID, int Record_ID, Trx trxName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM Fact_Acct WHERE VAF_TableView_ID=").Append(VAF_TableView_ID)
                .Append(" AND Record_ID=").Append(Record_ID);
            int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sb.ToString(), null, trxName));
            if (no == -1)
            {
               _log.Log(Level.SEVERE, "failed: VAF_TableView_ID=" + VAF_TableView_ID + ", Record_ID" + Record_ID);
            }
            else
            {
                _log.Fine("delete - VAF_TableView_ID=" + VAF_TableView_ID + ", Record_ID=" + Record_ID + " - #" + no);
            }
            return no;
        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param Fact_Acct_ID id
         *	@param trxName transaction
         */
        public MFactAcct(Ctx ctx, int Fact_Acct_ID, Trx trxName)
            : base(ctx, Fact_Acct_ID, trxName)
        {

        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MFactAcct(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        public MFactAcct(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MFactAcct[");
            sb.Append(Get_ID()).Append("-Acct=").Append(GetAccount_ID())
                .Append(",Dr=").Append(GetAmtSourceDr()).Append("|").Append(GetAmtAcctDr())
                .Append(",Cr=").Append(GetAmtSourceCr()).Append("|").Append(GetAmtAcctCr())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Derive MAccount from record
         *	@return Valid Account Combination
         */
        public MAccount GetMAccount()
        {
            MAccount acct = MAccount.Get(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(),
                GetVAB_AccountBook_ID(), GetAccount_ID(), GetC_SubAcct_ID(),
                GetM_Product_ID(), GetVAB_BusinessPartner_ID(), GetVAF_OrgTrx_ID(),
                GetC_LocFrom_ID(), GetC_LocTo_ID(), GetC_SalesRegion_ID(),
                GetC_Project_ID(), GetVAB_Promotion_ID(), GetVAB_BillingCode_ID(),
                GetUser1_ID(), GetUser2_ID(), GetUserElement1_ID(), GetUserElement2_ID());
            if (acct != null && acct.Get_ID() == 0)
                acct.Save();
            return acct;
        }
    }
}

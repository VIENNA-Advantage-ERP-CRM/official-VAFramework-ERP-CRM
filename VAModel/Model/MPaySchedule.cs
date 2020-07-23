/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_PaySchedule
 * Chronological Development
 * Veena Pandey     22-June-2009
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
    public class MPaySchedule : X_C_PaySchedule
    {
        /**	Parent					*/
        public MPaymentTerm _parent = null;

        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param C_PaySchedule_ID id
	 *	@param trxName transaction
	 */
        public MPaySchedule(Ctx ctx, int C_PaySchedule_ID, Trx trxName)
            : base(ctx, C_PaySchedule_ID, trxName)
        {
            if (C_PaySchedule_ID == 0)
            {
                //	setC_PaymentTerm_ID (0);	//	Parent
                SetPercentage(Env.ZERO);
                SetDiscount(Env.ZERO);
                SetDiscountDays(0);
                SetGraceDays(0);
                SetNetDays(0);
                SetIsValid(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MPaySchedule(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * @return Returns the parent.
	     */
        public MPaymentTerm GetParent()
        {
            if (_parent == null)
                _parent = new MPaymentTerm(GetCtx(), GetC_PaymentTerm_ID(), Get_TrxName());
            return _parent;
        }

        /**
         * @param parent The parent to set.
         */
        public void SetParent(MPaymentTerm parent)
        {
            _parent = parent;
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (Is_ValueChanged("Percentage") || Is_ValueChanged("IsActive"))
            {
                log.Fine("beforeSave");
                SetIsValid(false);
            }

            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
            {
                return success;
            }

            // set zero in net days at header if schedules created.
            string qry = "UPDATE C_PaymentTerm SET NetDays = 0 WHERE C_PaymentTerm_ID = " + GetC_PaymentTerm_ID();
            int nos = DB.ExecuteQuery(qry, null, Get_Trx());
            if (nos <= 0)
            {
                log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "PayTermNotUpdated"));
            }

            if (newRecord || Is_ValueChanged("Percentage") || Is_ValueChanged("IsActive"))
            {
                log.Fine("afterSave");
                GetParent();
                _parent.Validate();
                _parent.Save();
            }

            //SI_0646_4 : System should give the warning message on any change on payment term,if  payment term is used in transaction.
            if (Is_Changed() && !newRecord)
            {
                string sql = @" SELECT SUM(COUNT) FROM (
                              SELECT COUNT(*) AS COUNT FROM C_Order  WHERE IsActive = 'Y' AND DocStatus NOT IN ('RE' , 'VO') AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                              @" UNION ALL 
                              SELECT COUNT(*) AS COUNT FROM C_Invoice  WHERE IsActive = 'Y' AND DocStatus NOT IN ('RE' , 'VO') AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                              @" UNION ALL 
                              SELECT COUNT(*) AS COUNT FROM C_Project  WHERE IsActive = 'Y' AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                              @" UNION ALL 
                              SELECT COUNT(*) AS COUNT FROM C_Contract  WHERE IsActive = 'Y' AND DocStatus NOT IN ('RE' , 'VO') AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                               " ) t";
                int no = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (no > 0)
                {
                    log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VIS_ConflictChanges"));
                }
            }

            return success;
        }

        protected override bool BeforeDelete()
        {
            return true;
        }

        protected override bool AfterDelete(bool success)
        {
            // Check Record is Valid or Not (SI_0343)
            GetParent();
            _parent.Validate();
            _parent.Save();
            return true;
        }

    }
}

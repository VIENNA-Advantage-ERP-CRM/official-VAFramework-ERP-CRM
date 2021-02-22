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

namespace VAdvantage.Model
{
    public class MVARRequestUpdate : X_VAR_Req_Update
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param VAR_Req_Update_ID id
	 *	@param trxName trx
	 */
        public MVARRequestUpdate(Ctx ctx, int VAR_Req_Update_ID,
            Trx trxName) :
            base(ctx, VAR_Req_Update_ID, trxName)
        {
            //super (ctx, VAR_Req_Update_ID, trxName);
        }	//	MVARRequestUpdate

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MVARRequestUpdate(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, dr, trxName);
        }	//	MVARRequestUpdate

        /**
         * 	Parent Constructor
         *	@param parent request
         */
        public MVARRequestUpdate(MVARRequest parent) :
            base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //super (parent.GetContext(), 0, parent.Get_TrxName());
            SetClientOrg(parent);
            SetVAR_Request_ID(parent.GetVAR_Request_ID());
            //
            SetStartTime(parent.GetStartTime());
            SetEndTime(parent.GetEndTime());
            SetResult(parent.GetResult());
            SetQtySpent(parent.GetQtySpent());
            SetQtyInvoiced(parent.GetQtyInvoiced());
            SetVAM_ProductSpent_ID(parent.GetVAM_ProductSpent_ID());
            SetConfidentialTypeEntry(parent.GetConfidentialTypeEntry());
        }	//	MVARRequestUpdate

        /**
         * 	Do we have new info
         *	@return true if new info
         */
        public Boolean IsNewInfo()
        {
            return GetResult() != null;
        }	//	isNewInfo

        /**
         * 	Get Name of creator
         *	@return name
         */
        public String GetCreatedByName()
        {
            MVAFUserContact user = MVAFUserContact.Get(GetCtx(), GetCreatedBy());
            return user.GetName();
        }	//	GetCreatedByName

        /**
         * 	Get Confidential Entry Text (for jsp)
         *	@return text
         */
        public String GetConfidentialEntryText()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), CONFIDENTIALTYPEENTRY_VAF_Control_Ref_ID, GetConfidentialTypeEntry());
        }	//	GetConfidentialTextEntry

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetConfidentialTypeEntry() == null)
                SetConfidentialTypeEntry(CONFIDENTIALTYPEENTRY_PublicInformation);
            return true;
        }	//	beforeSave
    }
}

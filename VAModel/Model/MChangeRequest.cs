/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MChangeRequest
 * Purpose        : To handle change request
 * Class Used     : X_M_ChangeRequest
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MChangeRequest : X_M_ChangeRequest
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param M_ChangeRequest_ID ix
	 *	@param trxName trx
	 */
        public MChangeRequest(Ctx ctx, int M_ChangeRequest_ID, Trx trxName) :
            base(ctx, M_ChangeRequest_ID, trxName)
        {

            if (M_ChangeRequest_ID == 0)
            {
                //	SetName (null);
                SetIsApproved(false);
                SetProcessed(false);
            }
        }	//	MChangeRequest

        /**
         * 	CRM Request Constructor
         *	@param request request
         *	@param group request group
         */
        public MChangeRequest(MRequest request, MGroup group)
            : this(request.GetCtx(), 0, request.Get_Trx())
        {

            SetClientOrg(request);
            SetName(Msg.GetElement(GetCtx(), "R_Request_ID") + ": " + request.GetDocumentNo());
            SetHelp(request.GetSummary());
            //
            SetM_BOM_ID(group.GetM_BOM_ID());
            SetM_ChangeNotice_ID(group.GetM_ChangeNotice_ID());
        }	//	MChangeRequest

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MChangeRequest(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }	//	MChangeRequest

        /**
         * 	Get CRM Requests of Change Requests
         *	@return requests
         */
        public MRequest[] GetRequests()
        {
           // String sql = "SELECT * FROM R_Request WHERE M_ChangeRequest_ID=?";//Code not found by raghu 3-march-2011
            return null;
        }	//	GetRequests


        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true/false
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Have at least one
            if (GetM_BOM_ID() == 0 && GetM_ChangeNotice_ID() == 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@: @M_BOM_ID@ / @M_ChangeNotice_ID@"));
                return false;
            }

            //	Derive ChangeNotice from BOM if defined
            if (newRecord && GetM_BOM_ID() != 0 && GetM_ChangeNotice_ID() == 0)
            {
                MBOM bom = new MBOM(GetCtx(), GetM_BOM_ID(), Get_Trx());
                if (bom.GetM_ChangeNotice_ID() != 0)
                    SetM_BOM_ID(bom.GetM_ChangeNotice_ID());
            }

            return true;
        }	//	beforeSave
    }
}

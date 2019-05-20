using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Process;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class DocumentEngine : DocActionVariables, DocAction
    {
        /// <summary>
        /// Doc Engine (Drafted)
        /// </summary>
        /// <param name="po">document</param>
        public DocumentEngine(DocAction po)
            : this(po, STATUS_DRAFTED)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="po"></param>
        /// <param name="docStatus"></param>
        public DocumentEngine(DocAction po, String docStatus)
        {
            _document = po;
            if (docStatus != null)
                _status = docStatus;
        }


        /** Persistent Document 	*/
        private DocAction _document;
        /** Document Status			*/
        private String _status = STATUS_DRAFTED;
        /**	Process Message 		*/
        private String _message = null;
        /** Actual Doc Action		*/
        private String _action = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String GetDocStatus()
        {
            return _status;
        }

        /// <summary>
        /// Set Doc Status - Ignored
        /// </summary>
        /// <param name="ignored">Status is not set directly</param>
        public void SetDocStatus(String ignored)
        {
        }

        /// <summary>
        /// Document is Drafted
        /// </summary>
        /// <returns>true if drafted</returns>
        public bool IsDrafted()
        {
            return STATUS_DRAFTED.Equals(_status);
        }

        /// <summary>
        /// Document is Invalid
        /// </summary>
        /// <returns>true if Invalid</returns>
        public bool IsInvalid()
        {
            return STATUS_INVALID.Equals(_status);
        }

        /// <summary>
        /// Document is In Progress
        /// </summary>
        /// <returns>true if In Progress</returns>
        public bool IsInProgress()
        {
            return STATUS_INPROGRESS.Equals(_status);
        }


        /// <summary>
        /// Document is Approved
        /// </summary>
        /// <returns>return true if Approved</returns>
        public bool IsApproved()
        {
            return STATUS_APPROVED.Equals(_status);
        }

        /// <summary>
        /// Document is not Approved
        /// </summary>
        /// <returns>return true if not approved</returns>
        public bool IsNotApproved()
        {
            return STATUS_NOTAPPROVED.Equals(_status);
        }

        /// <summary>
        /// Document is Waiting Payment or Confirmation
        /// </summary>
        /// <returns>return true if waiting payment</returns>
        public bool IsWaiting()
        {
            return STATUS_WAITINGPAYMENT.Equals(_status)
                || STATUS_WAITINGCONFIRMATION.Equals(_status);
        }

        /// <summary>
        /// Document is completed
        /// </summary>
        /// <returns>return true if completed</returns>
        public bool IsCompleted()
        {
            return STATUS_COMPLETED.Equals(_status);
        }

        /// <summary>
        /// Document is Reversed
        /// </summary>
        /// <returns>return ture if reversed</returns>
        public bool IsReversed()
        {
            return STATUS_REVERSED.Equals(_status);
        }

        /// <summary>
        /// Document is closed
        /// </summary>
        /// <returns>return true if closed</returns>
        public bool IsClosed()
        {
            return STATUS_CLOSED.Equals(_status);
        }

        /// <summary>
        /// Document is voided
        /// </summary>
        /// <returns>return true if voided</returns>
        public bool IsVoided()
        {
            return STATUS_VOIDED.Equals(_status);
        }

        /// <summary>
        /// Document is Unknown
        /// </summary>
        /// <returns>return true if unknown</returns>
        public bool IsUnknown()
        {
            return STATUS_UNKNOWN.Equals(_status) ||
                !(IsDrafted() || IsInvalid() || IsInProgress() || IsNotApproved()
                    || IsApproved() || IsWaiting() || IsCompleted()
                    || IsReversed() || IsClosed() || IsVoided());
        }

        /// <summary>
        /// Process actual document.
        /// Checks if user (document) action is valid and then process action
        /// Calls the individual actions which call the document action
        /// </summary>
        /// <param name="processAction">document action based on workflow</param>
        /// <param name="docAction">document action based on document</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String processAction, String docAction)
        {
            _message = null;
            _action = null;
            //	Std User Workflows - see MWFNodeNext.isValidFor

            if (IsValidAction(processAction))	//	WF Selection first
                _action = processAction;
            //
            else if (IsValidAction(docAction))	//	User Selection second
                _action = docAction;
            //	Nothing to do
            else if (processAction.Equals(ACTION_NONE)
                || docAction.Equals(ACTION_NONE))
            {
                //if (_document != null)
                //    _document.get_Logger().info("**** No Action (Prc=" + processAction + "/Doc=" + docAction + ") " + _document);
                return true;
            }
            else
            {
                throw new Exception("Status=" + GetDocStatus()
                    + " - Invalid Actions: Process=" + processAction + ", Doc=" + docAction);
            }
            //if (_document != null)
            //    _document.get_Logger().info("**** Action=" + _action + " (Prc=" + processAction + "/Doc=" + docAction + ") " + _document);
            bool success = ProcessIt(_action);
            //if (_document != null)
            //    _document.get_Logger().fine("**** Action=" + _action + " - Success=" + success);
            return success;

            
        }

        ///Manfacturing
        /// <summary>
        /// Process document.  This replaces DocAction.processIt().
        /// </summary>
        /// <param name="doc">Doc Action</param>
        /// <param name="processAction">processAction</param>
        /// <returns>true if performed</returns>
        /// <date>07-march-2011</date>
        /// <writer>raghu</writer>
        public static Boolean ProcessIt(DocAction doc, String processAction)
        {
            DateTime time = DateTime.Now.Date;

            Boolean success = false;
            VAdvantage.Utility.Ctx ctx = doc.GetCtx();
            Boolean oldIsBatchMode = ctx.IsBatchMode();
            ctx.SetBatchMode(true);
            DocumentEngine engine = new DocumentEngine(doc, doc.GetDocStatus());
            success = engine.ProcessIt(processAction, doc.GetDocAction());
            ctx.SetBatchMode(oldIsBatchMode);

            return success;
        }

        /// <summary>
        /// Process actual document - do not call directly.
        /// Calls the individual actions which call the document action
        /// </summary>
        /// <param name="action">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String action)
        {
            _message = null;
            _action = action;
            //
            if (ACTION_UNLOCK.Equals(_action))
                return UnlockIt();
            if (ACTION_INVALIDATE.Equals(_action))
                return InvalidateIt();
            if (ACTION_PREPARE.Equals(_action))
                return STATUS_INPROGRESS.Equals(PrepareIt());
            if (ACTION_APPROVE.Equals(_action))
                return ApproveIt();
            if (ACTION_REJECT.Equals(_action))
                return RejectIt();
            if (ACTION_COMPLETE.Equals(_action) || ACTION_WAITCOMPLETE.Equals(_action))
            {
                String status = null;
                if (IsDrafted() || IsInvalid())		//	prepare if not prepared yet
                {
                    status = PrepareIt();
                    if (!STATUS_INPROGRESS.Equals(status))
                        return false;
                }
                status = CompleteIt();
                if (_document != null
                    && !Ini.IsClient())		//	Post Immediate if on Server
                {

                    MClient client = MClient.Get(_document.GetCtx(), _document.GetAD_Client_ID());
                    if (STATUS_COMPLETED.Equals(status) && client.IsPostImmediate())
                    {
                        _document.Save();
                        PostIt();
                    }
                }
                return STATUS_COMPLETED.Equals(status)
                    || STATUS_INPROGRESS.Equals(status)
                    || STATUS_WAITINGPAYMENT.Equals(status)
                    || STATUS_WAITINGCONFIRMATION.Equals(status);
            }
            if (ACTION_REACTIVATE.Equals(_action))
                return ReActivateIt();
            if (ACTION_REVERSE_ACCRUAL.Equals(_action))
                return ReverseAccrualIt();
            if (ACTION_REVERSE_CORRECT.Equals(_action))
                return ReverseCorrectIt();
            if (ACTION_CLOSE.Equals(_action))
                return CloseIt();
            if (ACTION_VOID.Equals(_action))
                return VoidIt();
            if (ACTION_POST.Equals(_action))
                return PostIt();
            //
            return false;
        }


        /// <summary>
        /// Unlock Document.
        /// Status: Drafted
        /// </summary>
        /// <returns>true if success </returns>
        public bool UnlockIt()
        {
            if (!IsValidAction(ACTION_UNLOCK))
                return false;
            if (_document != null)
            {
                if (_document.UnlockIt())
                {
                    _status = STATUS_DRAFTED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_DRAFTED;
            return true;
        }


        /// <summary>
        /// Invalidate Document.
        /// Status: Invalid
        /// </summary>
        /// <returns>true if success </returns>
        public bool InvalidateIt()
        {
            if (!IsValidAction(ACTION_INVALIDATE))
                return false;
            if (_document != null)
            {
                if (_document.InvalidateIt())
                {
                    _status = STATUS_INVALID;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_INVALID;
            return true;
        }


        /// <summary>
        /// Process Document.
        /// Status is set by process
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            if (!IsValidAction(ACTION_PREPARE))
                return _status;
            if (_document != null)
            {
                _status = _document.PrepareIt();
                _document.SetDocStatus(_status);
            }
            return _status;
        }


        /// <summary>
        /// Approve Document.
        /// Status: Approved
        /// </summary>
        /// <returns>true if success </returns>
        public bool ApproveIt()
        {
            if (!IsValidAction(ACTION_APPROVE))
                return false;
            if (_document != null)
            {
                if (_document.ApproveIt())
                {
                    _status = STATUS_APPROVED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_APPROVED;
            return true;
        }


        /// <summary>
        /// Reject Approval.
        /// Status: Not Approved
        /// </summary>
        /// <returns>true if success </returns>
        public bool RejectIt()
        {
            if (!IsValidAction(ACTION_REJECT))
                return false;
            if (_document != null)
            {
                if (_document.RejectIt())
                {
                    _status = STATUS_NOTAPPROVED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_NOTAPPROVED;
            return true;
        }

        /// <summary>
        /// Complete Document.
        /// Status is set by process
        /// </summary>
        /// <returns>new document status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            if (!IsValidAction(ACTION_COMPLETE))
                return _status;
            if (_document != null)
            {
                _status = _document.CompleteIt();
                _document.SetDocStatus(_status);
            }
            return _status;
        }


        /// <summary>
        /// Post Document
        /// Does not change status
        /// </summary>
        /// <returns>true if success </returns>
        public bool PostIt()
        {
            if (!IsValidAction(ACTION_POST)
                || _document == null)
                return false;
            try
            {
                //	Should work on Client and Server
                //InitialContext ctx = CConnection.get().getInitialContext(true);
                //ServerHome serverHome = (ServerHome)ctx.lookup(ServerHome.JNDI_NAME);
                //if (serverHome != null)
                //{
                //    Server server = serverHome.create();
                //    if (server != null)
                //    {
                //        String error = server.postImmediate(GlobalVariable.GetContext(),
                //            _document.GetAD_Client_ID(),
                //            _document.Get_Table_ID(), _document.Get_ID(),
                //            true, _document.Get_TrxName());
                //        //_document.get_Logger().config("Server: " + error == null ? "OK" : error);
                //        return error == null;
                //    }
                //}
                //else
                //    _document.get_Logger().config("NoServerHome");
            }
            catch (Exception e)
            {
                VAdvantage.Logging.VLogger.Get().Config("(ex) " + e.Message);
            }
            return false;
        }

        /**
         * 	Void Document.
         * 	Status: Voided
         * 	@return true if success 
         * 	
         */
        public bool VoidIt()
        {
            if (!IsValidAction(ACTION_VOID))
                return false;
            if (_document != null)
            {
                if (_document.VoidIt())
                {
                    _status = STATUS_VOIDED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_VOIDED;
            return true;
        }

        /**
         * 	Close Document.
         * 	Status: Closed
         * 	@return true if success 
        
         */
        public bool CloseIt()
        {
            if (_document != null 	//	orders can be closed any time
                && _document.Get_Table_ID() == X_C_Order.Table_ID)
            {
            }
            else if (!IsValidAction(ACTION_CLOSE))
                return false;
            if (_document != null)
            {
                if (_document.CloseIt())
                {
                    _status = STATUS_CLOSED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_CLOSED;
            return true;
        }

        /**
         * 	Reverse Correct Document.
         * 	Status: Reversed
         * 	@return true if success 
     
         */
        public bool ReverseCorrectIt()
        {
            if (!IsValidAction(ACTION_REVERSE_CORRECT))
                return false;
            if (_document != null)
            {
                if (_document.ReverseCorrectIt())
                {
                    _status = STATUS_REVERSED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_REVERSED;
            return true;
        }

        /**
         * 	Reverse Accrual Document.
         * 	Status: Reversed
         * 	@return true if success 

         */
        public bool ReverseAccrualIt()
        {
            if (!IsValidAction(ACTION_REVERSE_ACCRUAL))
                return false;
            if (_document != null)
            {
                if (_document.ReverseAccrualIt())
                {
                    _status = STATUS_REVERSED;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_REVERSED;
            return true;
        }

        /** 
         * 	Re-activate Document.
         * 	Status: In Progress
         * 	@return true if success 
  
         */
        public bool ReActivateIt()
        {
            if (!IsValidAction(ACTION_REACTIVATE))
                return false;
            if (_document != null)
            {
                if (_document.ReActivateIt())
                {
                    _status = STATUS_INPROGRESS;
                    _document.SetDocStatus(_status);
                    return true;
                }
                return false;
            }
            _status = STATUS_INPROGRESS;
            return true;
        }

        /**
         * 	Set Document Status to new Status
         *	@param newStatus new status
         */
        void SetStatus(String newStatus)
        {
            _status = newStatus;
        }

        /// <summary>
        /// Is The Action Valid based on current state
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>true if valid</returns>
        public bool IsValidAction(String action)
        {
            String[] options = GetActionOptions();
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Equals(action))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get Action Options based on current Status
        /// </summary>
        /// <returns>array of actions</returns>
        public String[] GetActionOptions()
        {
            if (IsInvalid())
                return new String[] {ACTION_PREPARE, ACTION_INVALIDATE, 
				ACTION_UNLOCK, ACTION_VOID};

            if (IsDrafted())
                return new String[] {ACTION_PREPARE, ACTION_INVALIDATE, ACTION_COMPLETE, 
				ACTION_UNLOCK, ACTION_VOID};

            if (IsInProgress() || IsApproved())
                return new String[] {ACTION_COMPLETE, ACTION_WAITCOMPLETE, 
				ACTION_APPROVE, ACTION_REJECT, 
				ACTION_UNLOCK, ACTION_VOID, ACTION_PREPARE};

            if (IsNotApproved())
                return new String[] {ACTION_REJECT, ACTION_PREPARE, 
				ACTION_UNLOCK, ACTION_VOID};

            if (IsWaiting())
                return new String[] {ACTION_COMPLETE, ACTION_WAITCOMPLETE,
				ACTION_REACTIVATE, ACTION_VOID, ACTION_CLOSE};

            if (IsCompleted())
                return new String[] {ACTION_CLOSE, ACTION_REACTIVATE, 
				ACTION_REVERSE_ACCRUAL, ACTION_REVERSE_CORRECT, 
				ACTION_POST, ACTION_VOID};

            if (IsClosed())
                return new String[] { ACTION_POST, ACTION_REOPEN };

            if (IsReversed() || IsVoided())
                return new String[] { ACTION_POST };

            return new String[] { };
        }

        /**
         * 	Get Process Message
         *	@return clear text error message
         */
        public String GetProcessMsg()
        {
            return _message;
        }

        /**
         * 	Get Process Message
         *	@param msg clear text error message
         */
        public void SetProcessMsg(String msg)
        {
            _message = msg;
        }


        /**	Document Exception Message		*/
        private static String EXCEPTION_MSG = "Document Engine is no Document";

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>throws exception</returns>
        public String GetSummary()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document No
        /// </summary>
        /// <returns>throws exception</returns>
        public String GetDocumentNo()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>throws excepton</returns>
        public String GetDocumentInfo()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document Owner
        /// </summary>
        /// <returns>throws exception</returns>
        public int GetDoc_User_ID()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>throws exception</returns>
        public int GetC_Currency_ID()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>throws Exception</returns>
        public Decimal GetApprovalAmt()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document Client
        /// </summary>
        /// <returns>thrwos exception</returns>
        public int GetAD_Client_ID()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Document Organization
        /// </summary>
        /// <returns>thrwo Exception</returns>
        public int GetAD_Org_ID()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get DocAction
        /// </summary>
        /// <returns>return DocAction</returns>
        public String GetDocAction()
        {
            return _action;
        }

        /// <summary>
        /// Save Document
        /// </summary>
        /// <returns>throw exception</returns>
        public bool Save()
        {
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Context
        /// </summary>
        /// <returns>context</returns>
        public Utility.Ctx GetCtx()
        {
            if (_document != null)
                return _document.GetCtx();
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get ID of record
        /// </summary>
        /// <returns>ID</returns>
        public int Get_ID()
        {
            if (_document != null)
                return _document.Get_ID();
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get AD Table
        /// </summary>
        /// <returns>AD_Table_ID</returns>
        public int Get_Table_ID()
        {
            if (_document != null)
                return _document.Get_Table_ID();
            throw new Exception(EXCEPTION_MSG);
        }

        /// <summary>
        /// Get Transaction
        /// </summary>
        /// <returns>trx name</returns>
        public Trx Get_Trx()
        {
            return null;
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>null</returns>
        public System.IO.FileInfo CreatePDF()
        {
            return null;
        }

        /// <summary>
        ///  Check to see if the appropriate periods are open for this document
        /// </summary>
        /// <param name="doc">Doc</param>
        /// <returns>null if all periods open; otherwise the error message</returns>
        public static String IsPeriodOpen(DocAction doc)
        {
            List<int> docOrgs = new List<int>();
            String errorMsg = null;
            if (errorMsg == null)
            {
                // check if lines exist
                // get all the orgs stamped on the document lines
                VAdvantage.Utility.Env.QueryParams qParams = doc.GetLineOrgsQueryInfo();
                if (qParams != null)
                {
                    //Object[][] result = QueryUtil.ExecuteQuery(doc.Get_Trx(), qParams.sql,
                    //                        qParams.parameters.ToList());
                    Object[][] result = VAdvantage.Utility.QueryUtil.ExecuteQuery(doc.Get_Trx(), qParams.sql,
                                            qParams.parameters.ToList());

                    foreach (Object[] row in result)
                    {
                        docOrgs.Add(Utility.Util.GetValueOfInt(Utility.Util.GetValueOfDecimal(row[0])));
                    }
                    // check if lines are missing
                    if (result.Length == 0)
                    {
                        errorMsg = "@NoLines@";
                    }
                }
            }

            if (errorMsg == null)
            {
                DateTime? docDate = doc.GetDocumentDate();
                String docBaseType = doc.GetDocBaseType();

                if (docDate != null && docBaseType != null)
                {
                    // check if period is open

                    // add doc header org to the list of orgs
                    if (!docOrgs.Contains(doc.GetAD_Org_ID()))
                    {
                        docOrgs.Add(doc.GetAD_Org_ID());
                    }
                    // Std Period open?
                    errorMsg = MPeriod.IsOpen(doc.GetCtx(), doc.GetAD_Client_ID(), docOrgs,
                        docDate, docBaseType);
                }
            }
            return errorMsg;
        }


        public Utility.Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }


        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }
    }
}

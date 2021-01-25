/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDocTypeCounter
 * Purpose        : Counter Document Type Model
 * Class Used     : X_VAB_InterCompanyDoc
 * Chronological    Development
 * Raghunandan     08-Jun-2009
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
    public class MDocTypeCounter : X_VAB_InterCompanyDoc
    {
        #region Varables
        //	Object Cache				
        private static CCache<int, MDocTypeCounter> s_cache = new CCache<int, MDocTypeCounter>("VAB_InterCompanyDoc", 20);
        //	Counter Relationship Cache	
        private static CCache<int, MDocTypeCounter> s_counter = new CCache<int, MDocTypeCounter>("VAB_InterCompanyDoc", 20);
        //	Static Logger	
        //private static CLogger	s_log	= CLogger.getCLogger (MDocTypeCounter.class);
        private static VLogger _log = VLogger.GetVLogger(typeof(MDocTypeCounter).FullName);
        #endregion

        /**
        * 	Get Counter document for document type
        *	@param ctx context
        *	@param VAB_DocTypes_ID base document
        *	@return counter document VAB_DocTypes_ID or 0 or -1 if no counter doc
        */
        public static int GetCounterDocType_ID(Ctx ctx, int VAB_DocTypes_ID)
        {
            //	Direct Relationship
            MDocTypeCounter dtCounter = GetCounterDocType(ctx, VAB_DocTypes_ID);
            if (dtCounter != null)
            {
                if (!dtCounter.IsCreateCounter() || !dtCounter.IsValid())
                    return -1;
                return dtCounter.GetCounter_VAB_DocTypes_ID();
            }
            return 0;
            //	Indirect Relationship
            //int Counter_VAB_DocTypes_ID = 0;
            //MDocType dt = MDocType.Get(ctx, VAB_DocTypes_ID);
            //if (!dt.IsCreateCounter())
            //    return -1;
            //String cDocBaseType = "";// = getCounterDocBaseType(dt.getDocBaseType());
            //if (cDocBaseType == null)
            //    return 0;
            //MDocType[] counters = MDocType.GetOfDocBaseType(ctx, cDocBaseType);
            //for (int i = 0; i < counters.Length; i++)
            //{
            //    MDocType counter = counters[i];
            //    if (counter.IsDefaultCounterDoc())
            //    {
            //        Counter_VAB_DocTypes_ID = counter.GetVAB_DocTypes_ID();
            //        break;
            //    }
            //    if (counter.IsDefault())
            //        Counter_VAB_DocTypes_ID = counter.GetVAB_DocTypes_ID();
            //    else if (i == 0)
            //        Counter_VAB_DocTypes_ID = counter.GetVAB_DocTypes_ID();
            //}
            //return Counter_VAB_DocTypes_ID;
        }

        /**
        * 	Get (first) valid Counter document for document type
        *	@param ctx context
        *	@param VAB_DocTypes_ID base document
        *	@return counter document (may be invalid) or null
        */
        public static MDocTypeCounter GetCounterDocType(Ctx ctx, int VAB_DocTypes_ID)
        {
            int key = (int)VAB_DocTypes_ID;
            MDocTypeCounter retValue = (MDocTypeCounter)s_counter[key];
            if (retValue != null)
                return retValue;

            //	Direct Relationship
            MDocTypeCounter temp = null;
            String sql = "SELECT * FROM VAB_InterCompanyDoc WHERE IsActive = 'Y' AND VAB_DocTypes_ID=" + VAB_DocTypes_ID;
            //  DataSet pstmt = new DataSet();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                //while (idr.Read() && retValue == null)
                {
                    if (retValue == null)
                    {
                        retValue = new MDocTypeCounter(ctx, dr, null);
                        if (!retValue.IsCreateCounter() || !retValue.IsValid())
                        {
                            temp = retValue;
                            retValue = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, "getCounterDocType", e);
            }
            finally
            {
                dt = null;
            }

            if (retValue != null)	//	valid
                return retValue;
            if (temp != null) 		//	invalid
                return temp;
            return null;			//	nothing found
        }

        /*	Get MDocTypeCounter from Cache
        *	@param ctx context
        *	@param VAB_InterCompanyDoc_ID id
        *	@return MDocTypeCounter
        *	@param trxName transaction
        */
        public static MDocTypeCounter Get(Ctx ctx, int VAB_InterCompanyDoc_ID, Trx trxName)
        {
            int key = VAB_InterCompanyDoc_ID;
            MDocTypeCounter retValue = (MDocTypeCounter)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MDocTypeCounter(ctx, VAB_InterCompanyDoc_ID, trxName);
            if (retValue.Get_ID() != 0)
            {
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /**
         * 	Get Counter Document BaseType
         *	@param DocBaseType Document Base Type (e.g. SOO)
         *	@return Counter Document BaseType (e.g. POO) or null if there is none
         */
        public static String GetCounterDocBaseType(String DocBaseType)
        {
            if (DocBaseType == null)
            {
                return null;
            }
            String retValue = null;
            //	SO/PO
            if (MDocBaseType.DOCBASETYPE_SALESORDER.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_PURCHASEORDER;
            }
            else if (MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_SALESORDER;
            }
            //	AP/AR Invoice
            else if (MDocBaseType.DOCBASETYPE_APINVOICE.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_ARINVOICE;
            }
            else if (MDocBaseType.DOCBASETYPE_ARINVOICE.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_APINVOICE;
            }
            //	Shipment
            else if (MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_MATERIALRECEIPT;
            }
            else if (MDocBaseType.DOCBASETYPE_MATERIALRECEIPT.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_MATERIALDELIVERY;
            }
            //	AP/AR CreditMemo
            else if (MDocBaseType.DOCBASETYPE_APCREDITMEMO.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_ARCREDITMEMO;
            }
            else if (MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_APCREDITMEMO;
            }
            //	Receipt / Payment
            else if (MDocBaseType.DOCBASETYPE_ARRECEIPT.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_APPAYMENT;
            }
            else if (MDocBaseType.DOCBASETYPE_APPAYMENT.Equals(DocBaseType))
            {
                retValue = MDocBaseType.DOCBASETYPE_ARRECEIPT;
            }
            //
            else
            {
                _log.Log(Level.SEVERE, "getCounterDocBaseType for " + DocBaseType + ": None found");
            }
            return retValue;
        }

        /* Standard Constructor
        *	@param ctx context
        *	@param VAB_InterCompanyDoc_ID id
        *	@param trxName transaction
        */
        public MDocTypeCounter(Ctx ctx, int VAB_InterCompanyDoc_ID, Trx trxName)
            : base(ctx, VAB_InterCompanyDoc_ID, trxName)
        {
            if (VAB_InterCompanyDoc_ID == 0)
            {
                SetIsCreateCounter(true);	// Y
                SetIsValid(false);
            }
        }


        /*	Load Constructor
        *	@param ctx context
        *	@param dr result set
        *	@param trxName transaction
        */
        public MDocTypeCounter(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Set VAB_DocTypes_ID
         *	@param VAB_DocTypes_ID id
         */
        public new void SetVAB_DocTypes_ID(int VAB_DocTypes_ID)
        {
            base.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
            if (IsValid())
            {
                SetIsValid(false);
            }
        }

        /**
         * 	Set Counter VAB_DocTypes_ID
         *	@param Counter_VAB_DocTypes_ID id
         */
        public new void SetCounter_VAB_DocTypes_ID(int Counter_VAB_DocTypes_ID)
        {
            base.SetCounter_VAB_DocTypes_ID(Counter_VAB_DocTypes_ID);
            if (IsValid())
            {
                SetIsValid(false);
            }
        }

        /*	Get Doc Type
         *	@return doc type or null if not existing
         */
        public MDocType GetDocType()
        {
            MDocType dt = null;
            if (GetVAB_DocTypes_ID() > 0)
            {
                dt = MDocType.Get(GetCtx(), GetVAB_DocTypes_ID());
                if (dt.Get_ID() == 0)
                {
                    dt = null;
                }
            }
            return dt;
        }

        /**
        * 	Get Counter Doc Type
        *	@return counter doc type or null if not existing
        */
        public MDocType GetCounterDocType()
        {
            MDocType dt = null;
            if (GetCounter_VAB_DocTypes_ID() > 0)
            {
                dt = MDocType.Get(GetCtx(), GetCounter_VAB_DocTypes_ID());
                if (dt.Get_ID() == 0)
                {
                    dt = null;
                }
            }
            return dt;
        }

        /*Validate Document Type compatability
        *	@return Error message or null if valid
        */
        public String Validate()
        {
            MDocType dt = GetDocType();
            if (dt == null)
            {
                log.Log(Level.SEVERE, "No DocType=" + GetVAB_DocTypes_ID());
                SetIsValid(false);
                return "No Document Type";
            }
            MDocType c_dt = GetCounterDocType();
            if (c_dt == null)
            {
                log.Log(Level.SEVERE, "No Counter DocType=" + GetCounter_VAB_DocTypes_ID());
                SetIsValid(false);
                return "No Counter Document Type";
            }
            //
            String dtBT = dt.GetDocBaseType();
            String c_dtBT = c_dt.GetDocBaseType();
            log.Fine(dtBT + " -> " + c_dtBT);

            //	SO / PO
            if ((MDocBaseType.DOCBASETYPE_SALESORDER.Equals(dtBT) && MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(c_dtBT))
                || (MDocBaseType.DOCBASETYPE_SALESORDER.Equals(c_dtBT) && MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(dtBT)))
            {
                SetIsValid(true);
            }
            //	AP/AR Invoice
            else if ((MDocBaseType.DOCBASETYPE_APINVOICE.Equals(dtBT) && MDocBaseType.DOCBASETYPE_ARINVOICE.Equals(c_dtBT))
                || (MDocBaseType.DOCBASETYPE_APINVOICE.Equals(c_dtBT) && MDocBaseType.DOCBASETYPE_ARINVOICE.Equals(dtBT)))
            {
                SetIsValid(true);
            }
            //	Shipment
            else if ((MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(dtBT) && MDocBaseType.DOCBASETYPE_MATERIALRECEIPT.Equals(c_dtBT))
                || (MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(c_dtBT) && MDocBaseType.DOCBASETYPE_MATERIALRECEIPT.Equals(dtBT)))
            {
                SetIsValid(true);
            }
            //	AP/AR CreditMemo
            else if ((MDocBaseType.DOCBASETYPE_APCREDITMEMO.Equals(dtBT) && MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(c_dtBT))
                || (MDocBaseType.DOCBASETYPE_APCREDITMEMO.Equals(c_dtBT) && MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(dtBT)))
            {
                SetIsValid(true);
            }
            //	Receipt / Payment
            else if ((MDocBaseType.DOCBASETYPE_ARRECEIPT.Equals(dtBT) && MDocBaseType.DOCBASETYPE_APPAYMENT.Equals(c_dtBT))
                || (MDocBaseType.DOCBASETYPE_ARRECEIPT.Equals(c_dtBT) && MDocBaseType.DOCBASETYPE_APPAYMENT.Equals(dtBT)))
            {
                SetIsValid(true);
            }
            else
            {
                log.Warning("NOT - " + dtBT + " -> " + c_dtBT);
                SetIsValid(false);
                return "Not valid";
            }
            //	Counter should have document numbering 
            if (!c_dt.IsDocNoControlled())
            {
                return "Counter Document Type should be automatically Document Number controlled";
            }
            return null;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDocTypeCounter[");
            sb.Append(Get_ID()).Append(",").Append(GetName())
                .Append(",VAB_DocTypes_ID=").Append(GetVAB_DocTypes_ID())
                .Append(",Counter=").Append(GetCounter_VAB_DocTypes_ID())
                .Append(",DocAction=").Append(GetDocAction())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);

            if (!newRecord
                && (Is_ValueChanged("VAB_DocTypes_ID") || Is_ValueChanged("Counter_VAB_DocTypes_ID")))
                SetIsValid(false);

            //	try to validate
            if (!IsValid())
                Validate();
            return true;
        }
    }
}

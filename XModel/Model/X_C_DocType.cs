namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for VAB_DocTypes
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_DocTypes : PO
    {
        public X_VAB_DocTypes(Context ctx, int VAB_DocTypes_ID, Trx trxName)
            : base(ctx, VAB_DocTypes_ID, trxName)
        {
            /** if (VAB_DocTypes_ID == 0)
            {
            SetVAB_DocTypes_ID (0);
            SetDocBaseType (null);
            SetDocumentCopies (0);	// 1
            SetVAGL_Group_ID (0);
            SetHasCharges (false);
            SetIsCreateCounter (true);	// Y
            SetIsDefault (false);
            SetIsDefaultCounterDoc (false);
            SetIsDocNoControlled (true);	// Y
            SetIsInTransit (false);
            SetIsIndexed (false);
            SetIsPickQAConfirm (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);
            SetIsShipConfirm (false);
            SetIsSplitWhenDifference (false);	// N
            SetName (null);
            SetPrintName (null);
            }
             */
        }
        public X_VAB_DocTypes(Ctx ctx, int VAB_DocTypes_ID, Trx trxName)
            : base(ctx, VAB_DocTypes_ID, trxName)
        {
            /** if (VAB_DocTypes_ID == 0)
            {
            SetVAB_DocTypes_ID (0);
            SetDocBaseType (null);
            SetDocumentCopies (0);	// 1
            SetVAGL_Group_ID (0);
            SetHasCharges (false);
            SetIsCreateCounter (true);	// Y
            SetIsDefault (false);
            SetIsDefaultCounterDoc (false);
            SetIsDocNoControlled (true);	// Y
            SetIsInTransit (false);
            SetIsIndexed (false);
            SetIsPickQAConfirm (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);
            SetIsShipConfirm (false);
            SetIsSplitWhenDifference (false);	// N
            SetName (null);
            SetPrintName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_DocTypes(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_DocTypes(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_DocTypes(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_DocTypes()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514371858L;
        /** Last Updated Timestamp 7/29/2010 1:07:35 PM */
        public static long updatedMS = 1280389055069L;
        /** VAF_TableView_ID=217 */
        public static int Table_ID;
        // =217;

        /** TableName=VAB_DocTypes */
        public static String Table_Name = "VAB_DocTypes";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(6);
        /** AccessLevel
        @return 6 - System - Client 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAB_DocTypes[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Print Format.
        @param VAF_Print_Rpt_Layout_ID Data Print Format */
        public void SetVAF_Print_Rpt_Layout_ID(int VAF_Print_Rpt_Layout_ID)
        {
            if (VAF_Print_Rpt_Layout_ID <= 0) Set_Value("VAF_Print_Rpt_Layout_ID", null);
            else
                Set_Value("VAF_Print_Rpt_Layout_ID", VAF_Print_Rpt_Layout_ID);
        }
        /** Get Print Format.
        @return Data Print Format */
        public int GetVAF_Print_Rpt_Layout_ID()
        {
            Object ii = Get_Value("VAF_Print_Rpt_Layout_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAB_DocTypesDifference_ID VAF_Control_Ref_ID=170 */
        public static int VAB_DocTypesDIFFERENCE_ID_VAF_Control_Ref_ID = 170;
        /** Set Difference Document.
        @param VAB_DocTypesDifference_ID Document type for generating in dispute Shipments */
        public void SetVAB_DocTypesDifference_ID(int VAB_DocTypesDifference_ID)
        {
            if (VAB_DocTypesDifference_ID <= 0) Set_Value("VAB_DocTypesDifference_ID", null);
            else
                Set_Value("VAB_DocTypesDifference_ID", VAB_DocTypesDifference_ID);
        }
        /** Get Difference Document.
        @return Document type for generating in dispute Shipments */
        public int GetVAB_DocTypesDifference_ID()
        {
            Object ii = Get_Value("VAB_DocTypesDifference_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAB_DocTypesInvoice_ID VAF_Control_Ref_ID=170 */
        public static int VAB_DocTypesINVOICE_ID_VAF_Control_Ref_ID = 170;
        /** Set Document Type for Invoice.
        @param VAB_DocTypesInvoice_ID Document type used for invoices generated from this sales document */
        public void SetVAB_DocTypesInvoice_ID(int VAB_DocTypesInvoice_ID)
        {
            if (VAB_DocTypesInvoice_ID <= 0) Set_Value("VAB_DocTypesInvoice_ID", null);
            else
                Set_Value("VAB_DocTypesInvoice_ID", VAB_DocTypesInvoice_ID);
        }
        /** Get Document Type for Invoice.
        @return Document type used for invoices generated from this sales document */
        public int GetVAB_DocTypesInvoice_ID()
        {
            Object ii = Get_Value("VAB_DocTypesInvoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAB_DocTypesProforma_ID VAF_Control_Ref_ID=170 */
        public static int VAB_DocTypesPROFORMA_ID_VAF_Control_Ref_ID = 170;
        /** Set Document Type for ProForma.
        @param VAB_DocTypesProforma_ID Document type used for pro forma invoices generated from this sales document */
        public void SetVAB_DocTypesProforma_ID(int VAB_DocTypesProforma_ID)
        {
            if (VAB_DocTypesProforma_ID <= 0) Set_Value("VAB_DocTypesProforma_ID", null);
            else
                Set_Value("VAB_DocTypesProforma_ID", VAB_DocTypesProforma_ID);
        }
        /** Get Document Type for ProForma.
        @return Document type used for pro forma invoices generated from this sales document */
        public int GetVAB_DocTypesProforma_ID()
        {
            Object ii = Get_Value("VAB_DocTypesProforma_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAB_DocTypesShipment_ID VAF_Control_Ref_ID=170 */
        public static int VAB_DocTypesSHIPMENT_ID_VAF_Control_Ref_ID = 170;
        /** Set Document Type for Shipment.
        @param VAB_DocTypesShipment_ID Document type used for shipments generated from this sales document */
        public void SetVAB_DocTypesShipment_ID(int VAB_DocTypesShipment_ID)
        {
            if (VAB_DocTypesShipment_ID <= 0) Set_Value("VAB_DocTypesShipment_ID", null);
            else
                Set_Value("VAB_DocTypesShipment_ID", VAB_DocTypesShipment_ID);
        }
        /** Get Document Type for Shipment.
        @return Document type used for shipments generated from this sales document */
        public int GetVAB_DocTypesShipment_ID()
        {
            Object ii = Get_Value("VAB_DocTypesShipment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Document Type.
        @param VAB_DocTypes_ID Document type or rules */
        public void SetVAB_DocTypes_ID(int VAB_DocTypes_ID)
        {
            if (VAB_DocTypes_ID < 0) throw new ArgumentException("VAB_DocTypes_ID is mandatory.");
            Set_ValueNoCheck("VAB_DocTypes_ID", VAB_DocTypes_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetVAB_DocTypes_ID()
        {
            Object ii = Get_Value("VAB_DocTypes_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }

        /** DocBaseType VAF_Control_Ref_ID=432 */
        public static int DOCBASETYPE_VAF_Control_Ref_ID = 432;
        /** Set Document BaseType.
        @param DocBaseType Logical type of document */
        public void SetDocBaseType(String DocBaseType)
        {
            if (DocBaseType.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                DocBaseType = DocBaseType.Substring(0, 3);
            }
            Set_Value("DocBaseType", DocBaseType);
        }
        /** Get Document BaseType.
        @return Logical type of document */
        public String GetDocBaseType()
        {
            return (String)Get_Value("DocBaseType");
        }

        /** DocNoSequence_ID VAF_Control_Ref_ID=128 */
        public static int DOCNOSEQUENCE_ID_VAF_Control_Ref_ID = 128;
        /** Set Document Sequence.
        @param DocNoSequence_ID Document sequence determines the numbering of documents */
        public void SetDocNoSequence_ID(int DocNoSequence_ID)
        {
            if (DocNoSequence_ID <= 0) Set_Value("DocNoSequence_ID", null);
            else
                Set_Value("DocNoSequence_ID", DocNoSequence_ID);
        }
        /** Get Document Sequence.
        @return Document sequence determines the numbering of documents */
        public int GetDocNoSequence_ID()
        {
            Object ii = Get_Value("DocNoSequence_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** DocSubTypeSO VAF_Control_Ref_ID=148 */
        public static int DOCSUBTYPESO_VAF_Control_Ref_ID = 148;
        /** Quotation = OB */
        public static String DOCSUBTYPESO_Quotation = "OB";
        /** Proposal = ON */
        public static String DOCSUBTYPESO_Proposal = "ON";
        /** Prepay Order = PR */
        public static String DOCSUBTYPESO_PrepayOrder = "PR";
        /** Standard Order = SO */
        public static String DOCSUBTYPESO_StandardOrder = "SO";
        /** On Credit Order = WI */
        public static String DOCSUBTYPESO_OnCreditOrder = "WI";
        /** Warehouse Order = WP */
        public static String DOCSUBTYPESO_WarehouseOrder = "WP";
        /** POS Order = WR */
        public static String DOCSUBTYPESO_POSOrder = "WR";

        /** Blanket Order = BO */
        public static String DOCSUBTYPESO_BlanketOrder = "BO";


        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocSubTypeSOValid(String test)
        {
            return test == null || test.Equals("OB") || test.Equals("ON") || test.Equals("PR") || test.Equals("SO") || test.Equals("WI") || test.Equals("WP") || test.Equals("WR") || test.Equals("BO");
        }
        /** Set SO Sub Type.
        @param DocSubTypeSO Sales Order Sub Type */
        public void SetDocSubTypeSO(String DocSubTypeSO)
        {
            if (!IsDocSubTypeSOValid(DocSubTypeSO))
                throw new ArgumentException("DocSubTypeSO Invalid value - " + DocSubTypeSO + " - Reference_ID=148 - OB - ON - PR - SO - WI - WP - WR - BO");
            if (DocSubTypeSO != null && DocSubTypeSO.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocSubTypeSO = DocSubTypeSO.Substring(0, 2);
            }
            Set_Value("DocSubTypeSO", DocSubTypeSO);
        }
        /** Get SO Sub Type.
        @return Sales Order Sub Type */
        public String GetDocSubTypeSO()
        {
            return (String)Get_Value("DocSubTypeSO");
        }
        /** Set Document Copies.
        @param DocumentCopies Number of copies to be printed */
        public void SetDocumentCopies(int DocumentCopies)
        {
            Set_Value("DocumentCopies", DocumentCopies);
        }
        /** Get Document Copies.
        @return Number of copies to be printed */
        public int GetDocumentCopies()
        {
            Object ii = Get_Value("DocumentCopies");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Document Note.
        @param DocumentNote Additional information for a Document */
        public void SetDocumentNote(String DocumentNote)
        {
            if (DocumentNote != null && DocumentNote.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                DocumentNote = DocumentNote.Substring(0, 2000);
            }
            Set_Value("DocumentNote", DocumentNote);
        }
        /** Get Document Note.
        @return Additional information for a Document */
        public String GetDocumentNote()
        {
            return (String)Get_Value("DocumentNote");
        }
        /** Set GL Category.
        @param VAGL_Group_ID General Ledger Category */
        public void SetVAGL_Group_ID(int VAGL_Group_ID)
        {
            if (VAGL_Group_ID < 1) throw new ArgumentException("VAGL_Group_ID is mandatory.");
            Set_Value("VAGL_Group_ID", VAGL_Group_ID);
        }
        /** Get GL Category.
        @return General Ledger Category */
        public int GetVAGL_Group_ID()
        {
            Object ii = Get_Value("VAGL_Group_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charges.
        @param HasCharges Charges can be added to the document */
        public void SetHasCharges(Boolean HasCharges)
        {
            Set_Value("HasCharges", HasCharges);
        }
        /** Get Charges.
        @return Charges can be added to the document */
        public Boolean IsHasCharges()
        {
            Object oo = Get_Value("HasCharges");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Pro forma Invoice.
        @param HasProforma Indicates if Pro Forma Invoices can be generated from this document */
        public void SetHasProforma(Boolean HasProforma)
        {
            Set_Value("HasProforma", HasProforma);
        }
        /** Get Pro forma Invoice.
        @return Indicates if Pro Forma Invoices can be generated from this document */
        public Boolean IsHasProforma()
        {
            Object oo = Get_Value("HasProforma");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Create Counter Document.
        @param IsCreateCounter Create Counter Document */
        public void SetIsCreateCounter(Boolean IsCreateCounter)
        {
            Set_Value("IsCreateCounter", IsCreateCounter);
        }
        /** Get Create Counter Document.
        @return Create Counter Document */
        public Boolean IsCreateCounter()
        {
            Object oo = Get_Value("IsCreateCounter");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Default.
        @param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault)
        {
            Set_Value("IsDefault", IsDefault);
        }
        /** Get Default.
        @return Default value */
        public Boolean IsDefault()
        {
            Object oo = Get_Value("IsDefault");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Default Counter Document.
        @param IsDefaultCounterDoc The document type is the default counter document type */
        public void SetIsDefaultCounterDoc(Boolean IsDefaultCounterDoc)
        {
            Set_Value("IsDefaultCounterDoc", IsDefaultCounterDoc);
        }
        /** Get Default Counter Document.
        @return The document type is the default counter document type */
        public Boolean IsDefaultCounterDoc()
        {
            Object oo = Get_Value("IsDefaultCounterDoc");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Document is Number Controlled.
        @param IsDocNoControlled The document has a document sequence */
        public void SetIsDocNoControlled(Boolean IsDocNoControlled)
        {
            Set_Value("IsDocNoControlled", IsDocNoControlled);
        }
        /** Get Document is Number Controlled.
        @return The document has a document sequence */
        public Boolean IsDocNoControlled()
        {
            Object oo = Get_Value("IsDocNoControlled");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set In Transit.
        @param IsInTransit Movement is in transit */
        public void SetIsInTransit(Boolean IsInTransit)
        {
            Set_Value("IsInTransit", IsInTransit);
        }
        /** Get In Transit.
        @return Movement is in transit */
        public Boolean IsInTransit()
        {
            Object oo = Get_Value("IsInTransit");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Indexed.
        @param IsIndexed Index the document for the internal search engine */
        public void SetIsIndexed(Boolean IsIndexed)
        {
            Set_Value("IsIndexed", IsIndexed);
        }
        /** Get Indexed.
        @return Index the document for the internal search engine */
        public Boolean IsIndexed()
        {
            Object oo = Get_Value("IsIndexed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Pick/QA Confirmation.
        @param IsPickQAConfirm Require Pick or QA Confirmation before processing */
        public void SetIsPickQAConfirm(Boolean IsPickQAConfirm)
        {
            Set_Value("IsPickQAConfirm", IsPickQAConfirm);
        }
        /** Get Pick/QA Confirmation.
        @return Require Pick or QA Confirmation before processing */
        public Boolean IsPickQAConfirm()
        {
            Object oo = Get_Value("IsPickQAConfirm");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Return Transaction.
        @param IsReturnTrx This is a return transaction */
        public void SetIsReturnTrx(Boolean IsReturnTrx)
        {
            Set_Value("IsReturnTrx", IsReturnTrx);
        }
        /** Get Return Transaction.
        @return This is a return transaction */
        public Boolean IsReturnTrx()
        {
            Object oo = Get_Value("IsReturnTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sales Transaction.
        @param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx)
        {
            Set_Value("IsSOTrx", IsSOTrx);
        }
        /** Get Sales Transaction.
        @return This is a Sales Transaction */
        public Boolean IsSOTrx()
        {
            Object oo = Get_Value("IsSOTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Ship/Receipt Confirmation.
        @param IsShipConfirm Require Ship or Receipt Confirmation before processing */
        public void SetIsShipConfirm(Boolean IsShipConfirm)
        {
            Set_Value("IsShipConfirm", IsShipConfirm);
        }
        /** Get Ship/Receipt Confirmation.
        @return Require Ship or Receipt Confirmation before processing */
        public Boolean IsShipConfirm()
        {
            Object oo = Get_Value("IsShipConfirm");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Split when Difference.
        @param IsSplitWhenDifference Split document when there is a difference */
        public void SetIsSplitWhenDifference(Boolean IsSplitWhenDifference)
        {
            Set_Value("IsSplitWhenDifference", IsSplitWhenDifference);
        }
        /** Get Split when Difference.
        @return Split document when there is a difference */
        public Boolean IsSplitWhenDifference()
        {
            Object oo = Get_Value("IsSplitWhenDifference");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }
        /** Set Print Text.
        @param PrintName The label text to be printed on a document or correspondence. */
        public void SetPrintName(String PrintName)
        {
            if (PrintName == null) throw new ArgumentException("PrintName is mandatory.");
            if (PrintName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                PrintName = PrintName.Substring(0, 60);
            }
            Set_Value("PrintName", PrintName);
        }
        /** Get Print Text.
        @return The label text to be printed on a document or correspondence. */
        public String GetPrintName()
        {
            return (String)Get_Value("PrintName");
        }

        /** R_MailText_ID VAF_Control_Ref_ID=274 */
        public static int R_MAILTEXT_ID_VAF_Control_Ref_ID = 274;
        /** Set Mail Template.
        @param R_MailText_ID Text templates for mailings */
        public void SetR_MailText_ID(int R_MailText_ID)
        {
            if (R_MailText_ID <= 0) Set_Value("R_MailText_ID", null);
            else
                Set_Value("R_MailText_ID", R_MailText_ID);
        }
        /** Get Mail Template.
        @return Text templates for mailings */
        public int GetR_MailText_ID()
        {
            Object ii = Get_Value("R_MailText_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        //@param VA019_IsPackaging Packaging */
        public void SetVA019_IsPackaging(Boolean VA019_IsPackaging) { Set_Value("VA019_IsPackaging", VA019_IsPackaging); }/** Get Packaging.
        @return Packaging */
        public Boolean IsVA019_IsPackaging() { Object oo = Get_Value("VA019_IsPackaging"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Wastage.
        @param VA019_IsWastage Wastage */
        public void SetVA019_IsWastage(Boolean VA019_IsWastage) { Set_Value("VA019_IsWastage", VA019_IsWastage); }/** Get Wastage.
        @return Wastage */
        public Boolean IsVA019_IsWastage() { Object oo = Get_Value("VA019_IsWastage"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** VAPOS_OrderType VAF_Control_Ref_ID=1000308 */
        public static int VAPOS_ORDERTYPE_VAF_Control_Ref_ID = 1000308;/** Dine In = E */
        public static String VAPOS_ORDERTYPE_DineIn = "E";/** Home Delivery = H */
        public static String VAPOS_ORDERTYPE_HomeDelivery = "H";/** Pick Order = P */
        public static String VAPOS_ORDERTYPE_PickOrder = "P";/** QSR = Q */
        public static String VAPOS_ORDERTYPE_QSR = "Q";/** Return Order = R */
        public static String VAPOS_ORDERTYPE_ReturnOrder = "R";/** Warehouse Order = W */
        public static String VAPOS_ORDERTYPE_WarehouseOrder = "W";/** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVAPOS_OrderTypeValid(String test) { return test == null || test.Equals("E") || test.Equals("H") || test.Equals("P") || test.Equals("Q") || test.Equals("R") || test.Equals("W"); }/** Set POS Order Type.
        @param VAPOS_OrderType POS Order Type */
        public void SetVAPOS_OrderType(String VAPOS_OrderType)
        {
            if (!IsVAPOS_OrderTypeValid(VAPOS_OrderType))
                throw new ArgumentException("VAPOS_OrderType Invalid value - " + VAPOS_OrderType + " - Reference_ID=1000308 - E - H - P - Q - R - W"); if (VAPOS_OrderType != null && VAPOS_OrderType.Length > 1) { log.Warning("Length > 1 - truncated"); VAPOS_OrderType = VAPOS_OrderType.Substring(0, 1); } Set_Value("VAPOS_OrderType", VAPOS_OrderType);
        }/** Get POS Order Type.
        @return POS Order Type */
        public String GetVAPOS_OrderType() { return (String)Get_Value("VAPOS_OrderType"); }
        /** VAPOS_POSMode VAF_Control_Ref_ID=1000561 */
        public static int VAPOS_POSMODE_VAF_Control_Ref_ID = 1000561;/** Resturant = RS */
        public static String VAPOS_POSMODE_Resturant = "RS";/** Retail = RT */
        public static String VAPOS_POSMODE_Retail = "RT";/** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVAPOS_POSModeValid(String test) { return test == null || test.Equals("RS") || test.Equals("RT"); }/** Set POS Mode.
        @param VAPOS_POSMode POS Mode */
        public void SetVAPOS_POSMode(String VAPOS_POSMode)
        {
            if (!IsVAPOS_POSModeValid(VAPOS_POSMode))
                throw new ArgumentException("VAPOS_POSMode Invalid value - " + VAPOS_POSMode + " - Reference_ID=1000561 - RS - RT"); if (VAPOS_POSMode != null && VAPOS_POSMode.Length > 2) { log.Warning("Length > 2 - truncated"); VAPOS_POSMode = VAPOS_POSMode.Substring(0, 2); } Set_Value("VAPOS_POSMode", VAPOS_POSMode);
        }/** Get POS Mode.
        @return POS Mode */
        public String GetVAPOS_POSMode() { return (String)Get_Value("VAPOS_POSMode"); }

        /** Set Posting code.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value != null && Value.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                Value = Value.Substring(0, 30);
            }
            Set_Value("Value", Value);
        }

        /** Get Posting code.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }

        /** Set Release Document.
        @param IsReleaseDocument Release Document */
        public void SetIsReleaseDocument(Boolean IsReleaseDocument)
        {
            Set_Value("IsReleaseDocument", IsReleaseDocument);
        }
        /** Get Release Document.
        @return Release Document */
        public Boolean IsReleaseDocument()
        {
            Object oo = Get_Value("IsReleaseDocument");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool))
                    return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Document Type for Releases.
        @param DocumentTypeforReleases Document Type for Releases */
        public void SetDocumentTypeforReleases(int DocumentTypeforReleases)
        {
            Set_Value("DocumentTypeforReleases", DocumentTypeforReleases);
        }
        /** Get Document Type for Releases.
        @return Document Type for Releases */
        public int GetDocumentTypeforReleases()
        {
            Object ii = Get_Value("DocumentTypeforReleases");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Overwrite Date on Complete.
        @param IsOverwriteDateOnComplete Overwrite Date on Complete */
        public void SetIsOverwriteDateOnComplete(Boolean IsOverwriteDateOnComplete)
        {
            Set_Value("IsOverwriteDateOnComplete",
                IsOverwriteDateOnComplete);
        }

        /** Get Overwrite Date on Complete.
        @return Overwrite Date on Complete */
        public Boolean IsOverwriteDateOnComplete()
        {
            Object oo = Get_Value("IsOverwriteDateOnComplete");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Overwrite Sequence on Complete.
        @param IsOverwriteSeqOnComplete Overwrite Sequence on Complete */
        public void SetIsOverwriteSeqOnComplete(Boolean IsOverwriteSeqOnComplete)
        {
            Set_Value("IsOverwriteSeqOnComplete", IsOverwriteSeqOnComplete);
        }

        /** Get Overwrite Sequence on Complete.
        @return Overwrite Sequence on Complete */
        public Boolean IsOverwriteSeqOnComplete()
        {
            Object oo = Get_Value("IsOverwriteSeqOnComplete");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool))
                    return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** CompletedSequence_ID VAF_Control_Ref_ID=128 */
        public static int COMPLETEDSEQUENCE_ID_VAF_Control_Ref_ID = 128;
        /** Set Document Sequence On Complete.
        @param CompletedSequence_ID Document Sequence On Complete */
        public void SetCompletedSequence_ID(int CompletedSequence_ID)
        {
            if (CompletedSequence_ID <= 0) 
                Set_Value("CompletedSequence_ID", null);
            else
                Set_Value("CompletedSequence_ID", CompletedSequence_ID);
        }
        /** Get Document Sequence On Complete.
        @return Document Sequence On Complete */
        public int GetCompletedSequence_ID() 
        {
            Object ii = Get_Value("CompletedSequence_ID"); 
            if (ii == null) 
                return 0; 
            return Convert.ToInt32(ii); 
        }

        /// <summary>
        /// Set Treat As Discount.
        /// </summary>
        /// <param name="TreatAsDiscount">This checkbox indicates if an invoice is considered as discount invoice while calculating product costing. Also the system will not allow to create return to vendor invoice.</param>
        public void SetTreatAsDiscount(Boolean TreatAsDiscount) { Set_Value("TreatAsDiscount", TreatAsDiscount); }
        /// <summary>
        ///  Get Treat As Discount.
        /// </summary>
        /// <returns>This checkbox indicates if an invoice is considered as discount invoice while calculating product costing. Also the system will not allow to create return to vendor invoice.</returns>
        public Boolean IsTreatAsDiscount() { Object oo = Get_Value("TreatAsDiscount"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

    }
}

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
    /** Generated Model for M_TaskList
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_TaskList : PO
    {
        public X_M_TaskList(Context ctx, int M_TaskList_ID, Trx trxName)
            : base(ctx, M_TaskList_ID, trxName)
        {
            /** if (M_TaskList_ID == 0)
            {
            SetC_DocType_ID (0);
            SetM_TaskList_ID (0);
            SetM_Warehouse_ID (0);
            }
             */
        }
        public X_M_TaskList(Ctx ctx, int M_TaskList_ID, Trx trxName)
            : base(ctx, M_TaskList_ID, trxName)
        {
            /** if (M_TaskList_ID == 0)
            {
            SetC_DocType_ID (0);
            SetM_TaskList_ID (0);
            SetM_Warehouse_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_TaskList(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_TaskList(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_TaskList(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_TaskList()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27581088221967L;
        /** Last Updated Timestamp 3/1/2011 12:31:45 PM */
        public static long updatedMS = 1298962905178L;
        /** AD_Table_ID=1023 */
        public static int Table_ID;
        // =1023;

        /** TableName=M_TaskList */
        public static String Table_Name = "M_TaskList";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_M_TaskList[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_User_ID AD_Reference_ID=286 */
        public static int AD_USER_ID_AD_Reference_ID = 286;
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Business Partner Contact */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_DocType_ID AD_Reference_ID=170 */
        public static int C_DOCTYPE_ID_AD_Reference_ID = 170;
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
            Set_Value("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Date Completed.
        @param DateCompleted Date Completed */
        public void SetDateCompleted(DateTime? DateCompleted)
        {
            Set_Value("DateCompleted", (DateTime?)DateCompleted);
        }
        /** Get Date Completed.
        @return Date Completed */
        public DateTime? GetDateCompleted()
        {
            return (DateTime?)Get_Value("DateCompleted");
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

        /** DocBaseType AD_Reference_ID=432 */
        public static int DOCBASETYPE_AD_Reference_ID = 432;
        /** Set Document BaseType.
        @param DocBaseType Logical type of document */
        public void SetDocBaseType(String DocBaseType)
        {
            throw new ArgumentException("DocBaseType Is virtual column");
        }
        /** Get Document BaseType.
        @return Logical type of document */
        public String GetDocBaseType()
        {
            return (String)Get_Value("DocBaseType");
        }

        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;
        /** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocStatusValid(String test)
        {
            return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus != null && DocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocStatus = DocStatus.Substring(0, 2);
            }
            Set_Value("DocStatus", DocStatus);
        }
        /** Get Document Status.
        @return The current status of the document */
        public String GetDocStatus()
        {
            return (String)Get_Value("DocStatus");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo != null && DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_Value("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Set Task List.
        @param M_TaskList_ID List of Warehouse Tasks */
        public void SetM_TaskList_ID(int M_TaskList_ID)
        {
            if (M_TaskList_ID < 1) throw new ArgumentException("M_TaskList_ID is mandatory.");
            Set_ValueNoCheck("M_TaskList_ID", M_TaskList_ID);
        }
        /** Get Task List.
        @return List of Warehouse Tasks */
        public int GetM_TaskList_ID()
        {
            Object ii = Get_Value("M_TaskList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PickMethod AD_Reference_ID=474 */
        public static int PICKMETHOD_AD_Reference_ID = 474;
        /** Cluster Picking = C */
        public static String PICKMETHOD_ClusterPicking = "C";
        /** Order Picking = O */
        public static String PICKMETHOD_OrderPicking = "O";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPickMethodValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("O");
        }
        /** Set Pick Method.
        @param PickMethod Pick method to be used when generating pick lists */
        public void SetPickMethod(String PickMethod)
        {
            if (!IsPickMethodValid(PickMethod))
                throw new ArgumentException("PickMethod Invalid value - " + PickMethod + " - Reference_ID=474 - C - O");
            if (PickMethod != null && PickMethod.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PickMethod = PickMethod.Substring(0, 1);
            }
            Set_Value("PickMethod", PickMethod);
        }
        /** Get Pick Method.
        @return Pick method to be used when generating pick lists */
        public String GetPickMethod()
        {
            return (String)Get_Value("PickMethod");
        }
    }

}

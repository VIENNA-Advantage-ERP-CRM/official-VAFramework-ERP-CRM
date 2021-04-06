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
    /** Generated Model for VAF_WFlow_Incharge
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_WFlow_Incharge : PO
    {
        public X_VAF_WFlow_Incharge(Context ctx, int VAF_WFlow_Incharge_ID, Trx trxName) : base(ctx, VAF_WFlow_Incharge_ID, trxName)
        {
            /** if (VAF_WFlow_Incharge_ID == 0)
{
SetVAF_WFlow_Incharge_ID (0);
SetRecordType (null);	// U
SetName (null);
SetResponsibleType (null);
}
             */
        }
        public X_VAF_WFlow_Incharge(Ctx ctx, int VAF_WFlow_Incharge_ID, Trx trxName) : base(ctx, VAF_WFlow_Incharge_ID, trxName)
        {
            /** if (VAF_WFlow_Incharge_ID == 0)
{
SetVAF_WFlow_Incharge_ID (0);
SetRecordType (null);	// U
SetName (null);
SetResponsibleType (null);
}
             */
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_WFlow_Incharge(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_WFlow_Incharge(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_WFlow_Incharge(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_WFlow_Incharge()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514366357L;
        /** Last Updated Timestamp 7/29/2010 1:07:29 PM */
        public static long updatedMS = 1280389049568L;
        /** VAF_TableView_ID=646 */
        public static int Table_ID;
        // =646;

        /** TableName=VAF_WFlow_Incharge */
        public static String Table_Name = "VAF_WFlow_Incharge";

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
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
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
        /** Info
@return info
*/
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAF_WFlow_Incharge[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Role.
@param VAF_Role_ID Responsibility Role */
        public void SetVAF_Role_ID(int VAF_Role_ID)
        {
            if (VAF_Role_ID <= 0) Set_Value("VAF_Role_ID", null);
            else
                Set_Value("VAF_Role_ID", VAF_Role_ID);
        }
        /** Get Role.
@return Responsibility Role */
        public int GetVAF_Role_ID()
        {
            Object ii = Get_Value("VAF_Role_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAF_UserContact_ID VAF_Control_Ref_ID=286 */
        public static int VAF_USERCONTACT_ID_VAF_Control_Ref_ID = 286;
        /** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }
        /** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
        public int GetVAF_UserContact_ID()
        {
            Object ii = Get_Value("VAF_UserContact_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Workflow Responsible.
@param VAF_WFlow_Incharge_ID Responsible for Workflow Execution */
        public void SetVAF_WFlow_Incharge_ID(int VAF_WFlow_Incharge_ID)
        {
            if (VAF_WFlow_Incharge_ID < 1) throw new ArgumentException("VAF_WFlow_Incharge_ID is mandatory.");
            Set_ValueNoCheck("VAF_WFlow_Incharge_ID", VAF_WFlow_Incharge_ID);
        }
        /** Get Workflow Responsible.
@return Responsible for Workflow Execution */
        public int GetVAF_WFlow_Incharge_ID()
        {
            Object ii = Get_Value("VAF_WFlow_Incharge_ID");
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

        /** RecordType VAF_Control_Ref_ID=389 */
        public static int RecordType_VAF_Control_Ref_ID = 389;
        /** Set Entity Type.
@param RecordType Dictionary Entity Type;
         Determines ownership and synchronization */
        public void SetRecordType(String RecordType)
        {
            if (RecordType.Length > 4)
            {
                log.Warning("Length > 4 - truncated");
                RecordType = RecordType.Substring(0, 4);
            }
            Set_Value("RecordType", RecordType);
        }
        /** Get Entity Type.
@return Dictionary Entity Type;
         Determines ownership and synchronization */
        public String GetRecordType()
        {
            return (String)Get_Value("RecordType");
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

        /** ResponsibleType VAF_Control_Ref_ID=304 */
        public static int RESPONSIBLETYPE_VAF_Control_Ref_ID = 304;
        /** SQL = C */
        public static String RESPONSIBLETYPE_SQL = "C";
        /** Human = H */
        public static String RESPONSIBLETYPE_Human = "H";
        /** Organization = O */
        public static String RESPONSIBLETYPE_Organization = "O";
        /** Role = R */
        public static String RESPONSIBLETYPE_Role = "R";
        /** System Resource = S */
        public static String RESPONSIBLETYPE_SysteMVASResource = "S";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsResponsibleTypeValid(String test)
        {
            return test.Equals("H") || test.Equals("O") || test.Equals("R") || test.Equals("S");
        }
        /** Set Responsible Type.
@param ResponsibleType Type of the Responsibility for a workflow */
        public void SetResponsibleType(String ResponsibleType)
        {
            if (ResponsibleType == null) throw new ArgumentException("ResponsibleType is mandatory");
            if (!IsResponsibleTypeValid(ResponsibleType))
                throw new ArgumentException("ResponsibleType Invalid value - " + ResponsibleType + " - Reference_ID=304 - H - O - R - S");
            if (ResponsibleType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ResponsibleType = ResponsibleType.Substring(0, 1);
            }
            Set_Value("ResponsibleType", ResponsibleType);
        }
        /** Get Responsible Type.
@return Type of the Responsibility for a workflow */
        public String GetResponsibleType()
        {
            return (String)Get_Value("ResponsibleType");
        }

        /** Set Custom SQL.
        @param CustomSQL Custom SQL */
        public void SetCustomSQL(String CustomSQL) 
        { 
            if (CustomSQL != null && CustomSQL.Length > 2000)
            { 
                log.Warning("Length > 2000 - truncated"); 
                CustomSQL = CustomSQL.Substring(0, 2000);
            } 
            Set_Value("CustomSQL", CustomSQL); 
        }
        /** Get Custom SQL.
        @return Custom SQL */
        public String GetCustomSQL() 
        { 
            return (String)Get_Value("CustomSQL"); 
        }
    }

}

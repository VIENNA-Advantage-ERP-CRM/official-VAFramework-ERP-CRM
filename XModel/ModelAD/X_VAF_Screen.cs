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
    /** Generated Model for VAF_Screen
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_Screen : PO
    {
        public X_VAF_Screen(Context ctx, int VAF_Screen_ID, Trx trxName) : base(ctx, VAF_Screen_ID, trxName)
        {
            /** if (VAF_Screen_ID == 0)
{
SetVAF_Screen_ID (0);
SetRecordType (null);	// U
SetIsBetaFunctionality (false);
SetIsDefault (false);
SetName (null);
SetWindowType (null);	// M
}
             */
        }
        public X_VAF_Screen(Ctx ctx, int VAF_Screen_ID, Trx trxName) : base(ctx, VAF_Screen_ID, trxName)
        {
            /** if (VAF_Screen_ID == 0)
{
SetVAF_Screen_ID (0);
SetRecordType (null);	// U
SetIsBetaFunctionality (false);
SetIsDefault (false);
SetName (null);
SetWindowType (null);	// M
}
             */
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_Screen(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_Screen(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_Screen(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAF_Screen()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514366388L;
        /** Last Updated Timestamp 7/29/2010 1:07:29 PM */
        public static long updatedMS = 1280389049599L;
        /** VAF_TableView_ID=105 */
        public static int Table_ID;
        // =105;

        /** TableName=VAF_Screen */
        public static String Table_Name = "VAF_Screen";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(4);
        /** AccessLevel
@return 4 - System 
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
            StringBuilder sb = new StringBuilder("X_VAF_Screen[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set System Color.
@param VAF_Colour_ID Color for backgrounds or indicators */
        public void SetVAF_Colour_ID(int VAF_Colour_ID)
        {
            if (VAF_Colour_ID <= 0) Set_Value("VAF_Colour_ID", null);
            else
                Set_Value("VAF_Colour_ID", VAF_Colour_ID);
        }
        /** Get System Color.
@return Color for backgrounds or indicators */
        public int GetVAF_Colour_ID()
        {
            Object ii = Get_Value("VAF_Colour_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Context Area.
@param VAF_ContextScope_ID Business Domain Area Terminology */
        public void SetVAF_ContextScope_ID(int VAF_ContextScope_ID)
        {
            if (VAF_ContextScope_ID <= 0) Set_Value("VAF_ContextScope_ID", null);
            else
                Set_Value("VAF_ContextScope_ID", VAF_ContextScope_ID);
        }
        /** Get Context Area.
@return Business Domain Area Terminology */
        public int GetVAF_ContextScope_ID()
        {
            Object ii = Get_Value("VAF_ContextScope_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Image.
@param VAF_Image_ID Image or Icon */
        public void SetVAF_Image_ID(int VAF_Image_ID)
        {
            if (VAF_Image_ID <= 0) Set_Value("VAF_Image_ID", null);
            else
                Set_Value("VAF_Image_ID", VAF_Image_ID);
        }
        /** Get Image.
@return Image or Icon */
        public int GetVAF_Image_ID()
        {
            Object ii = Get_Value("VAF_Image_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Window.
@param VAF_Screen_ID Data entry or display window */
        public void SetVAF_Screen_ID(int VAF_Screen_ID)
        {
            if (VAF_Screen_ID < 1) throw new ArgumentException("VAF_Screen_ID is mandatory.");
            Set_ValueNoCheck("VAF_Screen_ID", VAF_Screen_ID);
        }
        /** Get Window.
@return Data entry or display window */
        public int GetVAF_Screen_ID()
        {
            Object ii = Get_Value("VAF_Screen_ID");
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
        /** Set Comment.
@param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Help = Help.Substring(0, 2000);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
@return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }
        /** Set Beta Functionality.
@param IsBetaFunctionality This functionality is considered Beta */
        public void SetIsBetaFunctionality(Boolean IsBetaFunctionality)
        {
            Set_Value("IsBetaFunctionality", IsBetaFunctionality);
        }
        /** Get Beta Functionality.
@return This functionality is considered Beta */
        public Boolean IsBetaFunctionality()
        {
            Object oo = Get_Value("IsBetaFunctionality");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Customization Default.
@param IsCustomDefault Default Customization */
        public void SetIsCustomDefault(Boolean IsCustomDefault)
        {
            Set_Value("IsCustomDefault", IsCustomDefault);
        }
        /** Get Customization Default.
@return Default Customization */
        public Boolean IsCustomDefault()
        {
            Object oo = Get_Value("IsCustomDefault");
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
        /** Set Process Now.
@param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
@return Process Now */
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Window Height.
@param WinHeight Window Height */
        public void SetWinHeight(int WinHeight)
        {
            Set_Value("WinHeight", WinHeight);
        }
        /** Get Window Height.
@return Window Height */
        public int GetWinHeight()
        {
            Object ii = Get_Value("WinHeight");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Window Width.
@param WinWidth Window Width */
        public void SetWinWidth(int WinWidth)
        {
            Set_Value("WinWidth", WinWidth);
        }
        /** Get Window Width.
@return Window Width */
        public int GetWinWidth()
        {
            Object ii = Get_Value("WinWidth");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** WindowType VAF_Control_Ref_ID=108 */
        public static int WINDOWTYPE_VAF_Control_Ref_ID = 108;
        /** Maintain = M */
        public static String WINDOWTYPE_Maintain = "M";
        /** Query Only = Q */
        public static String WINDOWTYPE_QueryOnly = "Q";
        /** Single Record = S */
        public static String WINDOWTYPE_SingleRecord = "S";
        /** Transaction = T */
        public static String WINDOWTYPE_Transaction = "T";
        /** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsWindowTypeValid(String test)
        {
            return test.Equals("M") || test.Equals("Q") || test.Equals("S") || test.Equals("T");
        }
        /** Set WindowType.
@param WindowType Type or classification of a Window */
        public void SetWindowType(String WindowType)
        {
            if (WindowType == null) throw new ArgumentException("WindowType is mandatory");
            if (!IsWindowTypeValid(WindowType))
                throw new ArgumentException("WindowType Invalid value - " + WindowType + " - Reference_ID=108 - M - Q - S - T");
            if (WindowType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                WindowType = WindowType.Substring(0, 1);
            }
            Set_Value("WindowType", WindowType);
        }
        /** Get WindowType.
@return Type or classification of a Window */
        public String GetWindowType()
        {
            return (String)Get_Value("WindowType");
        }

        /** Set Display Name.
@param DisplayName Window Name */
        public void SetDisplayName(String DisplayName) { if (DisplayName == null) throw new ArgumentException("DisplayName is mandatory."); if (DisplayName.Length > 50) { log.Warning("Length > 50 - truncated"); DisplayName = DisplayName.Substring(0, 50); } Set_Value("DisplayName", DisplayName); }/** Get Display Name.
@return Window Name */
        public String GetDisplayName() { return (String)Get_Value("DisplayName"); }


    }

}

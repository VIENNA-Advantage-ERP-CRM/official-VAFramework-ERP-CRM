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
    /** Generated Model for RC_ViewPane
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_RC_ViewPane : PO
    {
        public X_RC_ViewPane(Context ctx, int RC_ViewPane_ID, Trx trxName)
            : base(ctx, RC_ViewPane_ID, trxName)
        {
            /** if (RC_ViewPane_ID == 0)
            {
            SetRC_ViewPane_ID (0);
            SetRC_View_ID (0);
            }
             */
        }
        public X_RC_ViewPane(Ctx ctx, int RC_ViewPane_ID, Trx trxName)
            : base(ctx, RC_ViewPane_ID, trxName)
        {
            /** if (RC_ViewPane_ID == 0)
            {
            SetRC_ViewPane_ID (0);
            SetRC_View_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ViewPane(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ViewPane(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RC_ViewPane(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_RC_ViewPane()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27706188504343L;
        /** Last Updated Timestamp 2/16/2015 10:36:29 AM */
        public static long updatedMS = 1424063187554L;
        /** AD_Table_ID=1000238 */
        public static int Table_ID;
        // =1000238;

        /** TableName=RC_ViewPane */
        public static String Table_Name = "RC_ViewPane";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_RC_ViewPane[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** BG_Color_ID AD_Reference_ID=266 */
        public static int BG_COLOR_ID_AD_Reference_ID = 266;
        /** Set BG Color.
        @param BG_Color_ID BG Color */
        public void SetBG_Color_ID(int BG_Color_ID)
        {
            if (BG_Color_ID <= 0) Set_Value("BG_Color_ID", null);
            else
                Set_Value("BG_Color_ID", BG_Color_ID);
        }
        /** Get BG Color.
        @return BG Color */
        public int GetBG_Color_ID()
        {
            Object ii = Get_Value("BG_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Colspan.
        @param Colspan Colspan */
        public void SetColspan(int Colspan)
        {
            Set_Value("Colspan", Colspan);
        }
        /** Get Colspan.
        @return Colspan */
        public int GetColspan()
        {
            Object ii = Get_Value("Colspan");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }

        /** Font_Color_ID AD_Reference_ID=266 */
        public static int FONT_COLOR_ID_AD_Reference_ID = 266;
        /** Set Font Color.
        @param Font_Color_ID Font Color */
        public void SetFont_Color_ID(int Font_Color_ID)
        {
            if (Font_Color_ID <= 0) Set_Value("Font_Color_ID", null);
            else
                Set_Value("Font_Color_ID", Font_Color_ID);
        }
        /** Get Font Color.
        @return Font Color */
        public int GetFont_Color_ID()
        {
            Object ii = Get_Value("Font_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** HeaderBG_Color_ID AD_Reference_ID=266 */
        public static int HEADERBG_COLOR_ID_AD_Reference_ID = 266;
        /** Set Header BG Color.
        @param HeaderBG_Color_ID Header BG Color */
        public void SetHeaderBG_Color_ID(int HeaderBG_Color_ID)
        {
            if (HeaderBG_Color_ID <= 0) Set_Value("HeaderBG_Color_ID", null);
            else
                Set_Value("HeaderBG_Color_ID", HeaderBG_Color_ID);
        }
        /** Get Header BG Color.
        @return Header BG Color */
        public int GetHeaderBG_Color_ID()
        {
            Object ii = Get_Value("HeaderBG_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** HeaderFont_Color_ID AD_Reference_ID=266 */
        public static int HEADERFONT_COLOR_ID_AD_Reference_ID = 266;
        /** Set Header Font Color.
        @param HeaderFont_Color_ID Header Font Color */
        public void SetHeaderFont_Color_ID(int HeaderFont_Color_ID)
        {
            if (HeaderFont_Color_ID <= 0) Set_Value("HeaderFont_Color_ID", null);
            else
                Set_Value("HeaderFont_Color_ID", HeaderFont_Color_ID);
        }
        /** Get Header Font Color.
        @return Header Font Color */
        public int GetHeaderFont_Color_ID()
        {
            Object ii = Get_Value("HeaderFont_Color_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Height AD_Reference_ID=1000157 */
        public static int HEIGHT_AD_Reference_ID = 1000157;
        /** 100% = 100 */
        public static String HEIGHT_100 = "100";
        /** 50% = 50 */
        public static String HEIGHT_50 = "50";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsHeightValid(String test)
        {
            return test == null || test.Equals("100") || test.Equals("50");
        }
        /** Set Height.
        @param Height Height */
        public void SetHeight(String Height)
        {
            if (!IsHeightValid(Height))
                throw new ArgumentException("Height Invalid value - " + Height + " - Reference_ID=1000157 - 100 - 50");
            if (Height != null && Height.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                Height = Height.Substring(0, 3);
            }
            Set_Value("Height", Height);
        }
        /** Get Height.
        @return Height */
        public String GetHeight()
        {
            return (String)Get_Value("Height");
        }
        /** Set Max Value.
        @param MaxValue Max Value */
        public void SetMaxValue(int MaxValue)
        {
            Set_Value("MaxValue", MaxValue);
        }
        /** Get Max Value.
        @return Max Value */
        public int GetMaxValue()
        {
            Object ii = Get_Value("MaxValue");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Min Value.
        @param MinValue Min Value */
        public void SetMinValue(int MinValue)
        {
            Set_Value("MinValue", MinValue);
        }
        /** Get Min Value.
        @return Min Value */
        public int GetMinValue()
        {
            Object ii = Get_Value("MinValue");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Name = Name.Substring(0, 50);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Set RC_ViewPane_ID.
        @param RC_ViewPane_ID RC_ViewPane_ID */
        public void SetRC_ViewPane_ID(int RC_ViewPane_ID)
        {
            if (RC_ViewPane_ID < 1) throw new ArgumentException("RC_ViewPane_ID is mandatory.");
            Set_ValueNoCheck("RC_ViewPane_ID", RC_ViewPane_ID);
        }
        /** Get RC_ViewPane_ID.
        @return RC_ViewPane_ID */
        public int GetRC_ViewPane_ID()
        {
            Object ii = Get_Value("RC_ViewPane_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Role Center View.
        @param RC_View_ID Role Center View */
        public void SetRC_View_ID(int RC_View_ID)
        {
            if (RC_View_ID < 1) throw new ArgumentException("RC_View_ID is mandatory.");
            Set_ValueNoCheck("RC_View_ID", RC_View_ID);
        }
        /** Get Role Center View.
        @return Role Center View */
        public int GetRC_View_ID()
        {
            Object ii = Get_Value("RC_View_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Rowspan.
        @param Rowspan Rowspan */
        public void SetRowspan(int Rowspan)
        {
            Set_Value("Rowspan", Rowspan);
        }
        /** Get Rowspan.
        @return Rowspan */
        public int GetRowspan()
        {
            Object ii = Get_Value("Rowspan");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sequence.
        @param SeqNo Method of ordering elements;
         lowest number comes first */
        public void SetSeqNo(int SeqNo)
        {
            Set_Value("SeqNo", SeqNo);
        }
        /** Get Sequence.
        @return Method of ordering elements;
         lowest number comes first */
        public int GetSeqNo()
        {
            Object ii = Get_Value("SeqNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Width AD_Reference_ID=1000156 */
        public static int WIDTH_AD_Reference_ID = 1000156;
        /** 100% = 100 */
        public static String WIDTH_100 = "100";
        /** 25% = 25 */
        public static String WIDTH_25 = "25";
        /** 50% = 50 */
        public static String WIDTH_50 = "50";
        /** 75% = 75 */
        public static String WIDTH_75 = "75";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWidthValid(String test)
        {
            return test == null || test.Equals("100") || test.Equals("25") || test.Equals("50") || test.Equals("75");
        }
        /** Set Width.
        @param Width Width */
        public void SetWidth(String Width)
        {
            if (!IsWidthValid(Width))
                throw new ArgumentException("Width Invalid value - " + Width + " - Reference_ID=1000156 - 100 - 25 - 50 - 75");
            if (Width != null && Width.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                Width = Width.Substring(0, 3);
            }
            Set_Value("Width", Width);
        }
        /** Get Width.
        @return Width */
        public String GetWidth()
        {
            return (String)Get_Value("Width");
        }
    }

}

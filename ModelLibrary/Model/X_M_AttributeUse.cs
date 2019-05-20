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
    /** Generated Model for M_AttributeUse
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_AttributeUse : PO
    {
        public X_M_AttributeUse(Context ctx, int M_AttributeUse_ID, Trx trxName)
            : base(ctx, M_AttributeUse_ID, trxName)
        {
            /** if (M_AttributeUse_ID == 0)
            {
            SetM_AttributeSet_ID (0);
            SetM_Attribute_ID (0);
            SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_AttributeUse WHERE M_AttributeSet_ID=@M_AttributeSet_ID@
            }
             */
        }
        public X_M_AttributeUse(Ctx ctx, int M_AttributeUse_ID, Trx trxName)
            : base(ctx, M_AttributeUse_ID, trxName)
        {
            /** if (M_AttributeUse_ID == 0)
            {
            SetM_AttributeSet_ID (0);
            SetM_Attribute_ID (0);
            SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_AttributeUse WHERE M_AttributeSet_ID=@M_AttributeSet_ID@
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_AttributeUse(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_AttributeUse(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_AttributeUse(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_AttributeUse()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514378409L;
        /** Last Updated Timestamp 7/29/2010 1:07:41 PM */
        public static long updatedMS = 1280389061620L;
        /** AD_Table_ID=563 */
        public static int Table_ID;
        // =563;

        /** TableName=M_AttributeUse */
        public static String Table_Name = "M_AttributeUse";

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
            StringBuilder sb = new StringBuilder("X_M_AttributeUse[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Attribute Set.
        @param M_AttributeSet_ID Product Attribute Set */
        public void SetM_AttributeSet_ID(int M_AttributeSet_ID)
        {
            if (M_AttributeSet_ID < 0) throw new ArgumentException("M_AttributeSet_ID is mandatory.");
            Set_ValueNoCheck("M_AttributeSet_ID", M_AttributeSet_ID);
        }
        /** Get Attribute Set.
        @return Product Attribute Set */
        public int GetM_AttributeSet_ID()
        {
            Object ii = Get_Value("M_AttributeSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute.
        @param M_Attribute_ID Product Attribute */
        public void SetM_Attribute_ID(int M_Attribute_ID)
        {
            if (M_Attribute_ID < 1) throw new ArgumentException("M_Attribute_ID is mandatory.");
            Set_ValueNoCheck("M_Attribute_ID", M_Attribute_ID);
        }
        /** Get Attribute.
        @return Product Attribute */
        public int GetM_Attribute_ID()
        {
            Object ii = Get_Value("M_Attribute_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetM_Attribute_ID().ToString());
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
    }

}

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
    /** Generated Model for VAM_PFeature_Use
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_PFeature_Use : PO
    {
        public X_VAM_PFeature_Use(Context ctx, int VAM_PFeature_Use_ID, Trx trxName)
            : base(ctx, VAM_PFeature_Use_ID, trxName)
        {
            /** if (VAM_PFeature_Use_ID == 0)
            {
            SetVAM_PFeature_Set_ID (0);
            SetVAM_ProductFeature_ID (0);
            SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAM_PFeature_Use WHERE VAM_PFeature_Set_ID=@VAM_PFeature_Set_ID@
            }
             */
        }
        public X_VAM_PFeature_Use(Ctx ctx, int VAM_PFeature_Use_ID, Trx trxName)
            : base(ctx, VAM_PFeature_Use_ID, trxName)
        {
            /** if (VAM_PFeature_Use_ID == 0)
            {
            SetVAM_PFeature_Set_ID (0);
            SetVAM_ProductFeature_ID (0);
            SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAM_PFeature_Use WHERE VAM_PFeature_Set_ID=@VAM_PFeature_Set_ID@
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PFeature_Use(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PFeature_Use(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PFeature_Use(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_PFeature_Use()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514378409L;
        /** Last Updated Timestamp 7/29/2010 1:07:41 PM */
        public static long updatedMS = 1280389061620L;
        /** VAF_TableView_ID=563 */
        public static int Table_ID;
        // =563;

        /** TableName=VAM_PFeature_Use */
        public static String Table_Name = "VAM_PFeature_Use";

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
            StringBuilder sb = new StringBuilder("X_VAM_PFeature_Use[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Attribute Set.
        @param VAM_PFeature_Set_ID Product Attribute Set */
        public void SetVAM_PFeature_Set_ID(int VAM_PFeature_Set_ID)
        {
            if (VAM_PFeature_Set_ID < 0) throw new ArgumentException("VAM_PFeature_Set_ID is mandatory.");
            Set_ValueNoCheck("VAM_PFeature_Set_ID", VAM_PFeature_Set_ID);
        }
        /** Get Attribute Set.
        @return Product Attribute Set */
        public int GetVAM_PFeature_Set_ID()
        {
            Object ii = Get_Value("VAM_PFeature_Set_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute.
        @param VAM_ProductFeature_ID Product Attribute */
        public void SetVAM_ProductFeature_ID(int VAM_ProductFeature_ID)
        {
            if (VAM_ProductFeature_ID < 1) throw new ArgumentException("VAM_ProductFeature_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductFeature_ID", VAM_ProductFeature_ID);
        }
        /** Get Attribute.
        @return Product Attribute */
        public int GetVAM_ProductFeature_ID()
        {
            Object ii = Get_Value("VAM_ProductFeature_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetVAM_ProductFeature_ID().ToString());
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

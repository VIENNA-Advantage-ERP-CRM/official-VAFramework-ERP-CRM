using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Generic PO implementation, this can be use together
    /// with ModelValidator as alternative to the classic 
    /// generated model class and extend ( X_ & M_ ) approach.
    /// </summary>
    public class GenericPO : PO
    {

        #region Private Variables

       // private static long serialVersionUID = -6558017105997010172L;
        private int tableID = 0;
        private String tableName = null;
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;

        #endregion

        /**
	 * @param tableName
	 * @param ctx
	 * @param ID
	 */
        public GenericPO(String tableName, Ctx ctx, int ID)
            : base(new PropertiesWrapper(ctx, tableName), ID, null)
        {

        }

        /**
         * @param tableName
         * @param ctx
         * @param dr
         */
        public GenericPO(String tableName, Ctx ctx, DataRow dr)
            : base(new PropertiesWrapper(ctx, tableName), dr, null)
        {

        }

        /**
         * @param tableName
         * @param ctx
         * @param ID
         * @param trxName
         */
        public GenericPO(String tableName, Ctx ctx, int ID, Trx trxName)
            : base(new PropertiesWrapper(ctx, tableName), ID, trxName)
        {

        }

        /**
         * @param tableName
         * @param ctx
         * @param dr
         * @param trxName
         */
        public GenericPO(String tableName, Ctx ctx, DataRow dr, Trx trxName)
            : base(new PropertiesWrapper(ctx, tableName), dr, trxName)
        {

        }

        /// <summary>
        /// Load Meta Data
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected override POInfo InitPO(Ctx ctx)
        {
            PropertiesWrapper wrapper = (PropertiesWrapper)ctx;
            p_ctx = wrapper.source;
            tableName = wrapper.tableName;
            tableID = MTable.Get_Table_ID(tableName);
            // log.info("Table_ID: "+Table_ID);
            POInfo poi = POInfo.GetPOInfo(ctx, tableID);
            return poi;
        }

        protected override POInfo InitPO(Context ctx)
        {
            //PropertiesWrapper wrapper = (PropertiesWrapper)ctx.GetCtx();
            //p_ctx = wrapper.source;
            //tableName = wrapper.tableName;
            //tableID = MTable.Get_Table_ID(tableName);
            //// log.info("Table_ID: "+Table_ID);
            //POInfo poi = POInfo.GetPOInfo(ctx, tableID);
            //return poi;
            return null;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("GenericPO[Table=").Append(
                    "" + tableID + ",ID=").Append(Get_ID()).Append("]");
            return sb.ToString();
        }



        /// <summary>
        /// Set Trx Organization. Performing or initiating organization
        /// </summary>
        /// <param name="AD_OrgTrx_ID"></param>
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID == 0)
            {
                Set_Value("AD_OrgTrx_ID", null);
            }
            else
            {
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
            }
        }

        /// <summary>
        /// Get Trx Organization. Performing or initiating organization
        /// </summary>
        /// <returns></returns>
        public int GetAD_OrgTrx_ID()
        {
            int ii = Util.GetValueOfInt(Get_Value("AD_OrgTrx_ID"));
            //if (ii == null)
            //{
            //    return 0;
            //}
            return ii;
        }


        protected override int Get_AccessLevel()
        {
            return Util.GetValueOfInt(p_info.GetAccessLevel());
        }



        /// <summary>
        /// Wrapper class to workaround the limit of PO constructor that doesn't take a tableName or
        /// tableID parameter. Note that in the generated class scenario ( X_ ), tableName and tableId
        /// is generated as a static field.
        /// </summary>
        public class PropertiesWrapper : Ctx
        {
           // private static long serialVersionUID = 8887531951501323594L;
            public Ctx source;
            public String tableName;

            public PropertiesWrapper(Ctx source, String tableName)
            {
                this.source = source;
                this.tableName = tableName;
            }
        }


    }
}

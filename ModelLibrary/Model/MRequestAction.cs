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

namespace VAdvantage.Model
{
    public class MRequestAction : X_VAR_Req_History
    {
        /**
	     * 	Persistency Constructor
	     *	@param ctx context
	     *	@param VAR_Req_History_ID id
	     */
        public MRequestAction(Ctx ctx, int VAR_Req_History_ID, Trx trxName) :
            base(ctx, VAR_Req_History_ID, trxName)
        {
        }

        /**
         * 	Load Construtor
         *	@param ctx context
         *	@param rs result set
         */
        public MRequestAction(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        /**
         * 	Parent Action Constructor
         *	@param request parent
         *	@param newRecord new (copy all)
         */
        public MRequestAction(MRequest request, bool newRecord)
            : this(request.GetCtx(), 0, request.Get_TrxName())
        {
            SetClientOrg(request);
            SetVAR_Request_ID(request.GetVAR_Request_ID());
        }

        /**
         * 	Add Null Column
         *	@param columnName
         */
        public void AddNullColumn(String columnName)
        {
            String nc = GetNullColumns();
            if (nc == null)
                SetNullColumns(columnName);
            else
                SetNullColumns(nc + ";" + columnName);
        }

        /**
         * 	Get Name of creator
         *	@return name
         */
        public String GetCreatedByName()
        {
            MUser user = MUser.Get(GetCtx(), GetCreatedBy());
            return user.GetName();
        }

        /**
         * 	Get Changes as HTML string
         *	@return changes
         */
        public String GetChangesHTML()
        {
            StringBuilder sb = new StringBuilder();
            GetChangeHTML(sb, "Priority");
            GetChangeHTML(sb, "PriorityUser");
            GetChangeHTML(sb, "VAR_Category_ID");
            GetChangeHTML(sb, "VAR_Group_ID");
            GetChangeHTML(sb, "VAR_Req_Type_ID");
            GetChangeHTML(sb, "VAR_Resolution_ID");
            GetChangeHTML(sb, "VAR_Req_Status_ID");
            GetChangeHTML(sb, "SalesRep_ID");
            GetChangeHTML(sb, "Summary");
            //
            //	GetChangeHTML(sb, "VAF_Org_ID");		//	always stored
            GetChangeHTML(sb, "VAF_Role_ID");
            GetChangeHTML(sb, "VAF_UserContact_ID");
            GetChangeHTML(sb, "VAB_BillingCode_ID");
            GetChangeHTML(sb, "VAB_BusinessPartner_ID");
            GetChangeHTML(sb, "VAB_Invoice_ID");
            GetChangeHTML(sb, "VAB_Order_ID");
            GetChangeHTML(sb, "VAB_Payment_ID");
            GetChangeHTML(sb, "VAB_Project_ID");
            GetChangeHTML(sb, "DateNextAction");

            GetChangeHTML(sb, "IsEscalated");
            GetChangeHTML(sb, "IsInvoiced");
            GetChangeHTML(sb, "IsSelfService");
            GetChangeHTML(sb, "M_InOut_ID");
            GetChangeHTML(sb, "M_Product_ID");
            GetChangeHTML(sb, "M_RMA_ID");
            GetChangeHTML(sb, "VAA_Asset_ID");

            if (sb.Length == 0)
                sb.Append("./.");
            //	Unicode check
            char[] chars = sb.ToString().ToCharArray();
            sb = new StringBuilder(chars.Length);
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                int ii = (int)c;
                if (ii > 255)
                    sb.Append("&#").Append(ii).Append(";");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /**
         * 	Get individual Change HTML
         *	@param sb string to append to
         *	@param columnName column name
         */
        private void GetChangeHTML(StringBuilder sb, String columnName)
        {
            if (Get_Value(columnName) != null)
            {
                if (sb.Length > 0)
                    sb.Append("<br>");
                sb.Append(Msg.GetElement(GetCtx(), columnName))
                    .Append(": ").Append(Get_DisplayValue(columnName, true));
            }
            else
            {
                String nc = GetNullColumns();
                if (nc != null && nc.IndexOf(columnName) != -1)
                {
                    if (sb.Length > 0) 
                        sb.Append("<br>");
                    sb.Append("(")
                        .Append(Msg.GetElement(GetCtx(), columnName))
                        .Append(")");
                }
            }
        }

        /// <summary>
        /// Get Column Value
        /// </summary>
        /// <param name="columnName"> Column name</param>
        /// <returns>Returns value of the column.</returns>
        public string getColumnValue(string columnName)
        {
            return Get_DisplayValue(columnName, true);
        }
        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            return true;
        }
    }
}

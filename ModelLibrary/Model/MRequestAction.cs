using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MRequestAction : X_R_RequestAction
    {
        /**
	     * 	Persistency Constructor
	     *	@param ctx context
	     *	@param R_RequestAction_ID id
	     */
        public MRequestAction(Ctx ctx, int R_RequestAction_ID, Trx trxName) :
            base(ctx, R_RequestAction_ID, trxName)
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
            SetR_Request_ID(request.GetR_Request_ID());
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
            GetChangeHTML(sb, "R_Category_ID");
            GetChangeHTML(sb, "R_Group_ID");
            GetChangeHTML(sb, "R_RequestType_ID");
            GetChangeHTML(sb, "R_Resolution_ID");
            GetChangeHTML(sb, "R_Status_ID");
            GetChangeHTML(sb, "SalesRep_ID");
            GetChangeHTML(sb, "Summary");
            //
            //	GetChangeHTML(sb, "AD_Org_ID");		//	always stored
            GetChangeHTML(sb, "AD_Role_ID");
            GetChangeHTML(sb, "AD_User_ID");
            GetChangeHTML(sb, "C_Activity_ID");
            GetChangeHTML(sb, "C_BPartner_ID");
            GetChangeHTML(sb, "C_Invoice_ID");
            GetChangeHTML(sb, "C_Order_ID");
            GetChangeHTML(sb, "C_Payment_ID");
            GetChangeHTML(sb, "C_Project_ID");
            GetChangeHTML(sb, "DateNextAction");

            GetChangeHTML(sb, "IsEscalated");
            GetChangeHTML(sb, "IsInvoiced");
            GetChangeHTML(sb, "IsSelfService");
            GetChangeHTML(sb, "M_InOut_ID");
            GetChangeHTML(sb, "M_Product_ID");
            GetChangeHTML(sb, "M_RMA_ID");
            GetChangeHTML(sb, "A_Asset_ID");

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAModelAD.Reflection
{
    internal class GetTypes
    {
        /* MReportTree */
        private static MethodInfo _fnGetWhereClause = null;

        internal static string GetTreeWhereClause(Ctx _ctx, string columnName, int _PA_Hierarchy_ID, int value)
        {
            if (_fnGetWhereClause == null)
            {
                _fnGetWhereClause = Assembly.Load("ModelLibrary").GetType("VAdvantage.Report.MReportTree")
                    .GetMethod("GetWhereClause");
            }
            string result1 = "";
            if (_fnGetWhereClause != null)
            {
                string ele = "";
                if (columnName.Equals("AD_Org_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_Organization;
                }
                else if (columnName.Equals("C_BPartner_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_BPartner;
                }
                else if (columnName.Equals("M_Product_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_Product;
                }
                else if (columnName.Equals("C_Project_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_Project;
                }
                else if (columnName.Equals("AD_OrgTrx_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx;
                }
                else if (columnName.Equals("Account_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_Account;
                }
                else if (columnName.Equals("C_Campaign_ID", StringComparison.OrdinalIgnoreCase))
                {
                    ele = X_C_AcctSchema_Element.ELEMENTTYPE_Campaign;
                }
                if (ele != "")
                    result1 = (string)_fnGetWhereClause.Invoke(null, new object[] { _ctx, _PA_Hierarchy_ID, ele, Convert.ToInt32(value) });
                else
                    result1 = null;
            }
            return result1;
        }
    }
}
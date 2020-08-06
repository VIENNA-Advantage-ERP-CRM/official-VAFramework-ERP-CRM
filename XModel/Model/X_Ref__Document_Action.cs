using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace VAdvantage.Model
{
    public class X_Ref__Document_Action
    {
        /** <None> = -- */
        public static string NONE = "--";
        /** Approve = AP */
        public static string APPROVE = "AP";
        /** Close = CL */
        public static string CLOSE = "CL";
        /** Complete = CO */
        public static string COMPLETE = "CO";
        /** Invalidate = IN */
        public static string INVALIDATE = "IN";
        /** Post = PO */
        public static string POST = "PO";
        /** Prepare = PR */
        public static string PREPARE = "PR";
        /** Reverse - Accrual = RA */
        public static string REVERSE__ACCRUAL = "RA";
        /** Reverse - Correct = RC */
        public static string REVERSE__CORRECT = "RC";
        /** Re-activate = RE */
        public static string RE__ACTIVATE = "RE";
        /** Reject = RJ */
        public static string REJECT = "RJ";
        /** Void = VO */
        public static string VOID = "VO";
        /** Wait Complete = WC */
        public static string WAIT_COMPLETE = "WC";
        /** Unlock = XL */
        public static string UNLOCK = "XL";

        public static List<String> RefDocumentActionList()
        {
            List<String> list = new List<string>();
            list.Add(NONE);
            list.Add(APPROVE);
            list.Add(CLOSE);
            list.Add(COMPLETE);
            list.Add(INVALIDATE);
            list.Add(POST);
            list.Add(PREPARE);
            list.Add(REVERSE__ACCRUAL);
            list.Add(REVERSE__CORRECT);
            list.Add(RE__ACTIVATE);
            list.Add(REJECT);
            list.Add(VOID);
            list.Add(WAIT_COMPLETE);
            list.Add(UNLOCK);
            return list;
        }
    }

    public class X_RefDocument_Action
    {
        public static int AD_Reference_ID = 135;
        private String value;

        private X_RefDocument_Action(String value)
        {
            this.value = value;

        }
        public String GetValue()
        {
            return this.value;

        }
        public static bool IsValid(String test)
        {
            foreach (var v in X_Ref__Document_Action.RefDocumentActionList())
            {
                if (v.ToString().Equals(test))
                    return true;
            }
            return false;
        }
    }
}


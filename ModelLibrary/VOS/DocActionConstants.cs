using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.VOS
{

    public class DocActionConstants
    {
        /** Complete = CO */
        public static String ACTION_Complete = "CO";
        /** Wait Complete = WC */
        public static String ACTION_WaitComplete = "WC";
        /** Approve = AP */
        public static String ACTION_Approve = "AP";
        /** Reject = RJ */
        public static String ACTION_Reject = "RJ";
        /** Post = PO */
        public static String ACTION_Post = "PO";
        /** Void = VO */
        public static String ACTION_Void = "VO";
        /** Close = CL */
        public static String ACTION_Close = "CL";
        /** Reverse - Correct = RC */
        public static String ACTION_Reverse_Correct = "RC";
        /** Reverse - Accrual = RA */
        public static String ACTION_Reverse_Accrual = "RA";
        /** ReActivate = RE */
        public static String ACTION_ReActivate = "RE";
        /** <None> = -- */
        public static String ACTION_None = "--";
        /** Prepare = PR */
        public static String ACTION_Prepare = "PR";
        /** Unlock = XL */
        public static String ACTION_Unlock = "XL";
        /** Invalidate = IN */
        public static String ACTION_Invalidate = "IN";
        /** ReOpen = OP */
        public static String ACTION_ReOpen = "OP";

        /** Drafted = DR */
        public static String STATUS_Drafted = "DR";
        /** Completed = CO */
        public static String STATUS_Completed = "CO";
        /** Approved = AP */
        public static String STATUS_Approved = "AP";
        /** Invalid = IN */
        public static String STATUS_Invalid = "IN";
        /** Not Approved = NA */
        public static String STATUS_NotApproved = "NA";
        /** Voided = VO */
        public static String STATUS_Voided = "VO";
        /** Reversed = RE */
        public static String STATUS_Reversed = "RE";
        /** Closed = CL */
        public static String STATUS_Closed = "CL";
        /** Unknown = ?? */
        public static String STATUS_Unknown = "??";
        /** In Progress = IP */
        public static String STATUS_InProgress = "IP";
        /** Waiting Payment = WP */
        public static String STATUS_WaitingPayment = "WP";
        /** Waiting Confirmation = WC */
        public static String STATUS_WaitingConfirmation = "WC";
    }
}

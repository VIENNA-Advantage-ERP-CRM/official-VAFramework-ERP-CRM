/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABInvoiceSchedule  
 * Purpose        : Invoice Schedule Model
 * Class Used     : X_VAB_sched_Invoice
 * Chronological    Development
 * Raghunandan     07-Sep-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABInvoiceSchedule : X_VAB_sched_Invoice
    {
        /**
	 * 	Get MVABInvoiceSchedule from Cache
	 *	@param ctx context
	 *	@param VAB_sched_Invoice_ID id
	 *	@param trxName transaction
	 *	@return MVABInvoiceSchedule
	 */
        public static MVABInvoiceSchedule Get(Ctx ctx, int VAB_sched_Invoice_ID, Trx trxName)
        {
            int key = Convert.ToInt32((VAB_sched_Invoice_ID));
            MVABInvoiceSchedule retValue = (MVABInvoiceSchedule)s_cache[key];
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MVABInvoiceSchedule(ctx, VAB_sched_Invoice_ID, trxName);
            if (retValue.Get_ID() != 0)
            {
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /**	Cache						*/
        private static CCache<int, MVABInvoiceSchedule> s_cache = new CCache<int, MVABInvoiceSchedule>("VAB_sched_Invoice", 5);


        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAB_sched_Invoice_ID id
         *	@param trxName transaction
         */
        public MVABInvoiceSchedule(Ctx ctx, int VAB_sched_Invoice_ID, Trx trxName)
            : base(ctx, VAB_sched_Invoice_ID, trxName)
        {

        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVABInvoiceSchedule(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Can I send Invoice
         * 	@param xDate date
         * 	@param orderAmt order amount
         *	@return true if I can send Invoice
         */
        public bool CanInvoice(DateTime? xDate, Decimal orderAmt)
        {
            //	Amount
            if (IsAmount() && GetAmt() != null && orderAmt != null
                && orderAmt.CompareTo(GetAmt()) >= 0)
            {
                return true;
            }

            //	Daily
            if (INVOICEFREQUENCY_Daily.Equals(GetInvoiceFrequency()))
            {
                return true;
            }

            //	Remove time
            xDate = TimeUtil.GetDay(xDate);
            //Calendar today = TimeUtil.getToday();
           // MessageBox.Show("Check Today");
            DateTime? today = DateTime.Now.Date;

            //	Weekly
            if (INVOICEFREQUENCY_Weekly.Equals(GetInvoiceFrequency()))
            {
               // MessageBox.Show("Check this function");
                ////Calendar cutoff = TimeUtil.getToday();
                //DateTime cutoff = DateTime.Now.Date;
                //cutoff.set(Calendar.DAY_OF_WEEK, getCalendarDay(getInvoiceWeekDayCutoff()));

                //if (cutoff.after(today))
                //{
                //    cutoff.add(Calendar.DAY_OF_YEAR, -7);
                //}
                //DateTime cutoffDate = new DateTime(cutoff.getTimeInMillis());
                //log.fine("canInvoice - Date=" + xDate + " > Cutoff=" + cutoffDate
                //    + " - " + xDate.after(cutoffDate));
                //if (xDate.after(cutoffDate))
                //{
                //    return false;
                //}
                ////
                //Calendar invoice = TimeUtil.getToday();
                //invoice.set(Calendar.DAY_OF_WEEK, getCalendarDay(getInvoiceWeekDay()));
                //if (invoice.after(today))
                //{
                //    invoice.add(Calendar.DAY_OF_YEAR, -7);
                //}
                //DateTime invoiceDate = new DateTime(invoice.getTimeInMillis());
                //log.fine("canInvoice - Date=" + xDate + " > Invoice=" + invoiceDate
                //    + " - " + xDate.after(invoiceDate));
                //if (xDate.after(invoiceDate))
                //{
                //    return false;
                //}
                return true;
            }

            //	Monthly
            if (INVOICEFREQUENCY_Monthly.Equals(GetInvoiceFrequency())
                || INVOICEFREQUENCY_TwiceMonthly.Equals(GetInvoiceFrequency()))
            {
                if (GetInvoiceDayCutoff() > 0)
                {
                    //Calendar cutoff = TimeUtil.getToday();
                    //cutoff.set(Calendar.DAY_OF_MONTH, getInvoiceDayCutoff());
                    //if (cutoff.after(today))
                    //    cutoff.add(Calendar.MONTH, -1);
                    //DateTime cutoffDate = new DateTime(cutoff.getTimeInMillis());
                    //log.fine("canInvoice - Date=" + xDate + " > Cutoff=" + cutoffDate
                    //    + " - " + xDate.after(cutoffDate));
                    //if (xDate.after(cutoffDate))
                    //    return false;
                }
                //Calendar invoice = TimeUtil.getToday();
                //invoice.set(Calendar.DAY_OF_MONTH, getInvoiceDay());
                //if (invoice.after(today))
                //    invoice.add(Calendar.MONTH, -1);
                //DateTime invoiceDate = new DateTime(invoice.getTimeInMillis());
                //log.fine("canInvoice - Date=" + xDate + " > Invoice=" + invoiceDate
                //    + " - " + xDate.after(invoiceDate));
                //if (xDate.after(invoiceDate))
                //{
                //    return false;
                //}
                return true;
            }

            //	Bi-Monthly (+15)
            if (INVOICEFREQUENCY_TwiceMonthly.Equals(GetInvoiceFrequency()))
            {
                if (GetInvoiceDayCutoff() > 0)
                {
                    //Calendar cutoff = TimeUtil.getToday();
                    //cutoff.set(Calendar.DAY_OF_MONTH, getInvoiceDayCutoff() + 15);
                    //if (cutoff.after(today))
                    //    cutoff.add(Calendar.MONTH, -1);
                    //DateTime cutoffDate = new DateTime(cutoff.getTimeInMillis());
                    //if (xDate.after(cutoffDate))
                    //    return false;
                }
                //Calendar invoice = TimeUtil.getToday();
                //invoice.set(Calendar.DAY_OF_MONTH, getInvoiceDay() + 15);
                //if (invoice.after(today))
                //    invoice.add(Calendar.MONTH, -1);
                //DateTime invoiceDate = new DateTime(invoice.getTimeInMillis());
                //if (xDate.after(invoiceDate))
                //{
                //    return false;
                //}
                //return true;
            }
            return false;
        }

        /**
         * 	Convert to Calendar day
         *	@param day Invoice Week Day
         *	@return day
         */
        private int GetCalendarDay(String day)
        {
            //if (INVOICEWEEKDAY_Friday.Equals(day))
            //{
            //    return Calendar.FRIDAY;
            //}
            //if (INVOICEWEEKDAY_Saturday.equals(day))
            //{
            //    return Calendar.SATURDAY;
            //}
            //if (INVOICEWEEKDAY_Sunday.equals(day))
            //{
            //    return Calendar.SUNDAY;
            //}
            //if (INVOICEWEEKDAY_Monday.equals(day))
            //{
            //    return Calendar.MONDAY;
            //}
            //if (INVOICEWEEKDAY_Tuesday.equals(day))
            //{
            //    return Calendar.TUESDAY;
            //}
            //if (INVOICEWEEKDAY_Wednesday.equals(day))
            //{
            //    return Calendar.WEDNESDAY;
            //}
            ////	if (INVOICEWEEKDAY_Thursday.equals(day))
            //return Calendar.THURSDAY;
            return 0;
        }
    }
}

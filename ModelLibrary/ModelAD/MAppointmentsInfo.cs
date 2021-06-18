using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.PushNotif;

namespace VAdvantage.Model
{
    public class MAppointmentsInfo : X_AppointmentsInfo
    {

        public MAppointmentsInfo(Context ctx, int AppointmentsInfo_ID, Trx trxName)
            : base(ctx, AppointmentsInfo_ID, trxName)
        {
            /** if (AppointmentsInfo_ID == 0)
            {
            SetAppointmentsInfo_ID (0);
            }
             */
        }
        public MAppointmentsInfo(Ctx ctx, int AppointmentsInfo_ID, Trx trxName)
            : base(ctx, AppointmentsInfo_ID, trxName)
        {
            /** if (AppointmentsInfo_ID == 0)
            {
            SetAppointmentsInfo_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public MAppointmentsInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public MAppointmentsInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public MAppointmentsInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /* 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return true if can be saved
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            #region Push Notification

            string type, title, body;

            if (IsTask())
            {
                string priorityValue = $@"SELECT Name FROM AD_Ref_List WHERE AD_Reference_ID = 
                                            (SELECT AD_Reference_ID FROM AD_Reference WHERE Name = '_PriorityRule')
                                            AND Value = {GetPriorityKey()} AND IsActive = 'Y'";
                string priority = Util.GetValueOfString(DB.ExecuteScalar(priorityValue));
                type = "T";
                title = Msg.GetMsg(GetCtx(), "Task");
                body = GetSubject() + " (" + priority + ")";
            }
            else
            {
                type = "A";
                title = Msg.GetMsg(GetCtx(), "Appointment");
                body = GetSubject() + " (" + GetStartDate().Value.ToLocalTime() + ")";
            }

            if (newRecord)
            {
                PushNotification.SendNotificationToUser(GetAD_User_ID(), GetAD_Window_ID(), GetRecord_ID(), title, body, type);
            }
            // Checking if notification is read by user 
            else if (Is_ValueChanged("IsRead") && IsRead())
            {
                int creatorUserId = GetCreatedBy();
                PushNotification.SendNotificationToUser(creatorUserId, GetAD_Window_ID(), GetRecord_ID(), title, body, type);
            }

            #endregion

            return true;
        }
    }
}

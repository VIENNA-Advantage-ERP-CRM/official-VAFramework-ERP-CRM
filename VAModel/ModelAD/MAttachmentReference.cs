using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAttachmentReference: X_AD_AttachmentReference
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MAttachmentReference).FullName);

        public MAttachmentReference(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MAttachmentReference(Ctx ctx, int AD_AttachmentReference_ID, Trx trx)
            : base(ctx, AD_AttachmentReference_ID, trx)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            return true;
        }

        public int GetURIByAttachmentLineID(int AD_AttachmentLine_ID)
        {
            string attURISql = @"
SELECT 
AD_AttachmentReference_ID, AD_AttachmentLine_ID, AD_AttachmentRef 
FROM 
AD_AttachmentReference 
WHERE IsActive = 'Y' AND AD_AttachmentLine_ID = " + AD_AttachmentLine_ID + " AND AD_Client_ID = " + GetAD_Client_ID();

            return Util.GetValueOfInt(DB.ExecuteScalar(attURISql));
        }

        public int GetAttachmentLineIDByURI(string attchmentURI)
        {
            string attLineSql = @"
SELECT 
AD_AttachmentReference_ID, AD_AttachmentLine_ID, AD_AttachmentRef 
FROM 
AD_AttachmentReference 
WHERE IsActive = 'Y' AND AD_AttachmentRef = '" + attchmentURI + "' AND AD_Client_ID = " + GetAD_Client_ID();

            return Util.GetValueOfInt(DB.ExecuteScalar(attLineSql));
        }
    }
}

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
    public class MVAFAttachmentReference: X_VAF_AttachmentReference
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFAttachmentReference).FullName);

        public MVAFAttachmentReference(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MVAFAttachmentReference(Ctx ctx, int VAF_AttachmentReference_ID, Trx trx)
            : base(ctx, VAF_AttachmentReference_ID, trx)
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

        public int GetURIByAttachmentLineID(int VAF_AttachmentLine_ID)
        {
            string attURISql = @"
SELECT 
VAF_AttachmentReference_ID, VAF_AttachmentLine_ID, VAF_AttachmentRef 
FROM 
VAF_AttachmentReference 
WHERE IsActive = 'Y' AND VAF_AttachmentLine_ID = " + VAF_AttachmentLine_ID + " AND VAF_Client_ID = " + GetVAF_Client_ID();

            return Util.GetValueOfInt(DB.ExecuteScalar(attURISql));
        }

        public int GetAttachmentLineIDByURI(string attchmentURI)
        {
            string attLineSql = @"
SELECT 
VAF_AttachmentReference_ID, VAF_AttachmentLine_ID, VAF_AttachmentRef 
FROM 
VAF_AttachmentReference 
WHERE IsActive = 'Y' AND VAF_AttachmentRef = '" + attchmentURI + "' AND VAF_Client_ID = " + GetVAF_Client_ID();

            return Util.GetValueOfInt(DB.ExecuteScalar(attLineSql));
        }
    }
}

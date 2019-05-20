using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MGmailContactSettings : X_GmailContactSettings
    {
         

        public MGmailContactSettings(Ctx ctx, int GmailContactsSettings_ID, Trx trxName)
            : base(ctx, GmailContactsSettings_ID, trxName)
        {

        }

        public MGmailContactSettings(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super(ctx, rs, trxName);
        }

    }
}

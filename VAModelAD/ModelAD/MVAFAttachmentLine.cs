using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViennaAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFAttachmentLine: X_VAF_AttachmentLine
    {
        public MVAFAttachmentLine(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MVAFAttachmentLine(Ctx ctx, int VAF_AttachmentLine_ID, Trx trx)
            : base(ctx, VAF_AttachmentLine_ID, trx)
        {

        }
    }
}

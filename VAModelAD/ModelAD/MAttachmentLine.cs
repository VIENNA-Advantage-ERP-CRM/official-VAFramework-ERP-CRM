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
    public class MAttachmentLine: X_AD_AttachmentLine
    {
        public MAttachmentLine(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MAttachmentLine(Ctx ctx, int AD_AttachmentLine_ID, Trx trx)
            : base(ctx, AD_AttachmentLine_ID, trx)
        {

        }
    }
}

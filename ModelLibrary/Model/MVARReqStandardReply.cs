using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
    public class MVARReqStandardReply : X_VAR_Req_StandardReply
    {

        /**
         * 	Default Constructor
         *	@param ctx context
         *	@param VAR_Req_StandardReply_ID id
         * 	@param trxName trx
         */
        public MVARReqStandardReply(Ctx ctx, int VAR_Req_StandardReply_ID, Trx trxName):
            base(ctx, VAR_Req_StandardReply_ID, trxName)
        {
            
        }	//	MVARReqStandardReply

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MVARReqStandardReply(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MVARReqStandardReply

    
    }
}

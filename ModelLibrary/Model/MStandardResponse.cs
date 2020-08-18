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
    public class MStandardResponse : X_R_StandardResponse
    {

        /**
         * 	Default Constructor
         *	@param ctx context
         *	@param R_StandardResponse_ID id
         * 	@param trxName trx
         */
        public MStandardResponse(Ctx ctx, int R_StandardResponse_ID, Trx trxName):
            base(ctx, R_StandardResponse_ID, trxName)
        {
            
        }	//	MStandardResponse

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MStandardResponse(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MStandardResponse

    
    }
}

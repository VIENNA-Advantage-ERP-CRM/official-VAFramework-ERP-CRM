/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAssetDelivery
 * Purpose        : Transaction perpose
 * Class Used     : X_A_Asset_Delivery
 * Chronological    Development
 * Raghunandan     11-Jun-2009
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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;


namespace VAdvantage.Model
{
    public class MAssetDelivery : X_A_Asset_Delivery
    {
        /* 	Constructor
        * 	@param ctx context
        * 	@param A_Asset_Delivery_ID id or 0
        * 	@param trxName trx
        */
        public MAssetDelivery(Ctx ctx, int A_Asset_Delivery_ID, Trx trxName)
            : base(ctx, A_Asset_Delivery_ID, trxName)
        {
            if (A_Asset_Delivery_ID == 0)
            {
                SetMovementDate(new DateTime(CommonFunctions.CurrentTimeMillis()));
            }
        }

        /**
         *  Load Constructor
         *  @param ctx context
         *  @param rs result set record
         *	@param trxName transaction
         */
        public MAssetDelivery(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Create Asset Delivery for HTTP Request
         * 	@param asset asset
         * 	@param request request
         * 	@param AD_User_ID BP Contact
         */
        //public MAssetDelivery(MAsset asset,
        //    HttpServletRequest request, int AD_User_ID)
        //{
        //    base(asset.getCtx(), 0, asset.get_TrxName());
        //    setClientOrg(asset);
        //    //	Asset Info
        //    setA_Asset_ID(asset.getA_Asset_ID());
        //    setLot(asset.getLot());
        //    setSerNo(asset.getSerNo());
        //    setVersionNo(asset.getVersionNo());
        //    //
        //    SetMovementDate(new DateTime(System.currentTimeMillis()));
        //    //	Request
        //    setURL(request.getRequestURL().toString());
        //    setReferrer(request.getHeader("Referer"));
        //    setRemote_Addr(request.getRemoteAddr());
        //    setRemote_Host(request.getRemoteHost());
        //    //	Who
        //    setAD_User_ID(AD_User_ID);
        //    //
        //    save();
        //}

        /**
         * 	Create Asset Delivery for EMail
         * 	@param asset asset
         * 	@param email optional email
         * 	@param messageID access ID
         * 	@param AD_User_ID BP Contact
         */
        public MAssetDelivery(MAsset asset, EMail email, String messageID, int AD_User_ID)
            : base(asset.GetCtx(), 0, asset.Get_TrxName())
        {
            SetClientOrg(asset);
            //	Asset Info
            SetA_Asset_ID(asset.GetA_Asset_ID());
            SetLot(asset.GetLot());
            SetSerNo(asset.GetSerNo());
            SetVersionNo(asset.GetVersionNo());
            //
            SetMovementDate(new DateTime(CommonFunctions.CurrentTimeMillis()));
            //	EMail
            if (email != null)
            {
                SetEMail(email.GetTo().ToString());
                SetMessageID(email.GetMessageID());
            }
            else
                SetMessageID(messageID);
            //	Who
            SetAD_User_ID(AD_User_ID);
            //
            Save();
        }

        /**
         * 	String representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAssetDelivery[")
                .Append(Get_ID())
                .Append(",A_Asset_ID=").Append(GetA_Asset_ID())
                .Append(",MovementDate=").Append(GetMovementDate())
                .Append("]");
            return sb.ToString();
        }

    }
}

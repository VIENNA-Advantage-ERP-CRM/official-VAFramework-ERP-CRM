/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAssetDelivery
 * Purpose        : Transaction perpose
 * Class Used     : X_VAA_AssetDelivery
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
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
//using System.Web.UI;


namespace VAdvantage.Model
{
    public class MVAAAssetDelivery : X_VAA_AssetDelivery
    {
        /* 	Constructor
        * 	@param ctx context
        * 	@param VAA_AssetDelivery_ID id or 0
        * 	@param trxName trx
        */
        public MVAAAssetDelivery(Ctx ctx, int VAA_AssetDelivery_ID, Trx trxName)
            : base(ctx, VAA_AssetDelivery_ID, trxName)
        {
            if (VAA_AssetDelivery_ID == 0)
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
        public MVAAAssetDelivery(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Create Asset Delivery for HTTP Request
         * 	@param asset asset
         * 	@param request request
         * 	@param VAF_UserContact_ID BP Contact
         */
        //public MAssetDelivery(MAsset asset,
        //    HttpServletRequest request, int VAF_UserContact_ID)
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
        //    setVAF_UserContact_ID(VAF_UserContact_ID);
        //    //
        //    save();
        //}

        /**
         * 	Create Asset Delivery for EMail
         * 	@param asset asset
         * 	@param email optional email
         * 	@param messageID access ID
         * 	@param VAF_UserContact_ID BP Contact
         */
        public MVAAAssetDelivery(MVAAsset asset, EMail email, String messageID, int VAF_UserContact_ID)
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
            SetVAF_UserContact_ID(VAF_UserContact_ID);
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
                .Append(",VAA_Asset_ID=").Append(GetA_Asset_ID())
                .Append(",MovementDate=").Append(GetMovementDate())
                .Append("]");
            return sb.ToString();
        }

    }
}

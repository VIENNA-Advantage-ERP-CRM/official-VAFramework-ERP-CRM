/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMPackaging
 * Purpose        : Package Model
 * Class Used     : X_VAM_Packaging
 * Chronological    Development
 * Raghunandan     21-Oct-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMPackaging : X_VAM_Packaging
    {
        /// <summary>
        /// Create one Package for Shipment 
        /// </summary>
        /// <param name="shipment">shipment</param>
        /// <param name="shipper">shipper</param>
        /// <param name="shipDate">null for today</param>
        /// <returns>package</returns>
        /// 
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMPackaging).FullName); //Arpit

        public static MVAMPackaging Create(MVAMInvInOut shipment, MVAMShippingMethod shipper, DateTime? shipDate, Trx trxName)
        {
            MVAMPackaging retValue = new MVAMPackaging(shipment, shipper);
            if (shipDate != null)
            {
                retValue.SetShipDate(shipDate);
            }
            //Edited : Arpit Rai, 13 Sept,2017
            DateTime? moveDate = Convert.ToDateTime(shipment.GetMovementDate());
            String documentNo = shipment.GetDocumentNo();
            retValue.SetDateAcct(DateTime.Now.Date);
            retValue.SetVAF_Client_ID(shipment.GetVAF_Client_ID());
            retValue.SetVAF_Org_ID(shipment.GetVAF_Org_ID());
        
            if (!retValue.Save(trxName))
            {
                trxName.Rollback();
                _log.Log(Level.SEVERE, "Error While Generating Package");
            }
            //Arpit
            //	Lines
            MVAMInvInOutLine[] lines = shipment.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvInOutLine sLine = lines[i];
                MVAMPackagingLine pLine = new MVAMPackagingLine(retValue);
                //Arpit
                //pLine.SetInOutLine(sLine);
                //Changes in Below Method to create Lines and their values from Shipment/MR Lines to Package Lines
                pLine.SetInOutLine(sLine, moveDate, documentNo,
                    retValue.GetVAF_Client_ID(), retValue.GetVAF_Org_ID());

                if (!pLine.Save(trxName))
                {
                    trxName.Rollback();
                    _log.Log(Level.SEVERE, "Error While Generating Package Lines");
                }
                //Arpit
            }	//	lines
            return retValue;
        }

        /// <summary>
        /// MVAMPackaging
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Packaging_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMPackaging(Ctx ctx, int VAM_Packaging_ID, Trx trxName)
            : base(ctx, VAM_Packaging_ID, trxName)
        {
            if (VAM_Packaging_ID == 0)
            {
                //SetShipDate(new DateTime(System.currentTimeMillis()));
                SetShipDate(new DateTime(CommonFunctions.CurrentTimeMillis()));
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAMPackaging(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Shipment Constructor
        /// </summary>
        /// <param name="shipment">shipment</param>
        /// <param name="shipper">shipper</param>
        public MVAMPackaging(MVAMInvInOut shipment, MVAMShippingMethod shipper)
            : this(shipment.GetCtx(), 0, shipment.Get_TrxName())
        {
            SetClientOrg(shipment);
            SetVAM_Inv_InOut_ID(shipment.GetVAM_Inv_InOut_ID());
            SetVAM_ShippingMethod_ID(shipper.GetVAM_ShippingMethod_ID());
        }
    }
}

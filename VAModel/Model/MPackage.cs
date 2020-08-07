/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPackage
 * Purpose        : Package Model
 * Class Used     : X_M_Package
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPackage : X_M_Package
    {
        /// <summary>
        /// Create one Package for Shipment 
        /// </summary>
        /// <param name="shipment">shipment</param>
        /// <param name="shipper">shipper</param>
        /// <param name="shipDate">null for today</param>
        /// <returns>package</returns>
        /// 
        private static VLogger _log = VLogger.GetVLogger(typeof(MPackage).FullName); //Arpit

        public static MPackage Create(MInOut shipment, MShipper shipper, DateTime? shipDate, Trx trxName)
        {
            MPackage retValue = new MPackage(shipment, shipper);
            if (shipDate != null)
            {
                retValue.SetShipDate(shipDate);
            }
            //Edited : Arpit Rai, 13 Sept,2017
            DateTime? moveDate = Convert.ToDateTime(shipment.GetMovementDate());
            String documentNo = shipment.GetDocumentNo();
            retValue.SetDateAcct(DateTime.Now.Date);
            retValue.SetAD_Client_ID(shipment.GetAD_Client_ID());
            retValue.SetAD_Org_ID(shipment.GetAD_Org_ID());
        
            if (!retValue.Save(trxName))
            {
                trxName.Rollback();
                _log.Log(Level.SEVERE, "Error While Generating Package");
            }
            //Arpit
            //	Lines
            MInOutLine[] lines = shipment.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInOutLine sLine = lines[i];
                MPackageLine pLine = new MPackageLine(retValue);
                //Arpit
                //pLine.SetInOutLine(sLine);
                //Changes in Below Method to create Lines and their values from Shipment/MR Lines to Package Lines
                pLine.SetInOutLine(sLine, moveDate, documentNo,
                    retValue.GetAD_Client_ID(), retValue.GetAD_Org_ID());

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
        /// MPackage
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Package_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPackage(Ctx ctx, int M_Package_ID, Trx trxName)
            : base(ctx, M_Package_ID, trxName)
        {
            if (M_Package_ID == 0)
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
        public MPackage(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Shipment Constructor
        /// </summary>
        /// <param name="shipment">shipment</param>
        /// <param name="shipper">shipper</param>
        public MPackage(MInOut shipment, MShipper shipper)
            : this(shipment.GetCtx(), 0, shipment.Get_TrxName())
        {
            SetClientOrg(shipment);
            SetM_InOut_ID(shipment.GetM_InOut_ID());
            SetM_Shipper_ID(shipper.GetM_Shipper_ID());
        }
    }
}

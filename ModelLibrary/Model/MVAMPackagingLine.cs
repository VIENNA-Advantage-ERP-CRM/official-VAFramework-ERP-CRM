/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMPackagingLine
 * Purpose        : Package Line Model
 * Class Used     : X_VAM_PackagingLine
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
    public class MVAMPackagingLine : X_VAM_PackagingLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PackagingLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMPackagingLine(Ctx ctx, int VAM_PackagingLine_ID, Trx trxName)
            : base(ctx, VAM_PackagingLine_ID, trxName)
        {
            if (VAM_PackagingLine_ID == 0)
            {
                //	setVAM_Packaging_ID (0);
                //	setVAM_Inv_InOutLine_ID (0);
                SetQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAMPackagingLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">header</param>
        public MVAMPackagingLine(MVAMPackaging parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_Packaging_ID(parent.GetVAM_Packaging_ID());
        }

        /// <summary>
        /// Set Shipment Line
        /// </summary>
        /// <param name="line">line</param>
        public void SetInOutLine(MVAMInvInOutLine line, DateTime? moveDate, String DocumentNo, Int32 Client_ID, Int32 Org_ID)
        {
            SetVAM_Inv_InOutLine_ID(line.GetVAM_Inv_InOutLine_ID());
            SetQty(line.GetMovementQty());
            //Edited :Arpit Rai ,13 Sept,2017 
            //to Set Client,Org,Confirm Date,Confirm Date, Scrapped Qty, Difference Qty & Reference No
            SetVAF_Client_ID(Client_ID);
            SetVAF_Org_ID(Org_ID);
            SetVAM_Product_ID(line.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(line.GetVAM_PFeature_SetInstance_ID());
            if (Util.GetValueOfInt(DB.ExecuteQuery("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'")) > 0
               )
            {
                if (moveDate != null)
                    SetDTD001_ConfirmDate(moveDate);
                SetDTD001_ReferenceNo(DocumentNo); //Set Reference No from MR/Shipment to package Lines
            }
            SetConfirmedQty(line.GetConfirmedQty());
            SetScrappedQty(line.GetScrappedQty());
            SetDifferenceQty(Decimal.Subtract(line.GetMovementQty(), line.GetConfirmedQty() + line.GetScrappedQty()));
            //Arpit
        }
        /// <summary>
        /// Edited :  Abhishek , 17/10/2014
        Decimal _count = 0;
        public Decimal SetMovementLine(VAdvantage.Model.MVAMInvTrfLine line)
        {

            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'"));
            if (_CountDTD001 > 0)
            {
                SetVAM_InvTrf_Line_ID(line.GetVAM_InvTrf_Line_ID());
                SetDTD001_TotalQty(line.GetMovementQty());
                SetVAM_Product_ID(line.GetVAM_Product_ID());
                SetVAM_PFeature_SetInstance_ID(line.GetVAM_PFeature_SetInstance_ID());
                decimal totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM VAM_PackagingLine WHERE VAM_InvTrf_Line_ID=" + line.GetVAM_InvTrf_Line_ID()));
                SetQty(line.GetMovementQty() - totalPackQty);
                _count = Util.GetValueOfDecimal(line.GetMovementQty()) - totalPackQty;
                SetDTD001_AlreadyPackQty(totalPackQty);
                SetConfirmedQty(line.GetMovementQty() - totalPackQty);
                SetDTD001_ConfirmDate(System.DateTime.Now);

            }
            return _count;
        }
        public Decimal SetInoutLine(VAdvantage.Model.MVAMInvInOutLine line)
        {
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='DTD001_'"));
            if (_CountDTD001 > 0)
            {
                SetVAM_Inv_InOutLine_ID(line.GetVAM_Inv_InOutLine_ID());
                SetDTD001_TotalQty(line.GetMovementQty());
                SetVAM_Product_ID(line.GetVAM_Product_ID());
                SetVAM_PFeature_SetInstance_ID(line.GetVAM_PFeature_SetInstance_ID());
                decimal totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM VAM_PackagingLine WHERE VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID()));
                SetQty(line.GetMovementQty() - totalPackQty);
                _count = Util.GetValueOfDecimal(line.GetMovementQty()) - totalPackQty;
                SetDTD001_AlreadyPackQty(totalPackQty);
                SetConfirmedQty(line.GetMovementQty() - totalPackQty);
                SetDTD001_ConfirmDate(System.DateTime.Now);
            }
            return _count;
        }
        protected override bool BeforeSave(bool newRecord)
        {
            #region IsConfirm
            GetVAM_Packaging_ID();
            GetVAM_InvTrf_Line_ID();
            GetVAM_Inv_InOutLine_ID();
            string sql = null;
            string DocS = null;

            if (Util.GetValueOfInt(GetVAM_InvTrf_Line_ID()) != 0)
            {
                sql = "Select mv.docstatus FROM VAM_InvTrf_Line ml   INNER JOIN VAM_InventoryTransfer mv   ON (ml.VAM_InventoryTransfer_id=mv.VAM_InventoryTransfer_id) where ml.VAM_InvTrf_Line_id=" + GetVAM_InvTrf_Line_ID();
                DocS = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (DocS == "CO")
                {
                }
                else if (DocS == "IP")
                {
                    SetDTD001_IsConfirm(true);
                }
            }
            if (Util.GetValueOfInt(GetVAM_Inv_InOutLine_ID()) != 0)
            {
                sql = "SELECT o.docstatus    FROM  VAM_Inv_InOutLine ol    INNER JOIN VAM_Inv_InOut o     ON (ol.VAM_Inv_InOut_ID=o.VAM_Inv_InOut_ID) where ol.VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID();
                DocS = Util.GetValueOfString(DB.ExecuteScalar(sql));
                if (DocS == "CO")
                {
                }
                else if (DocS == "IP")
                {
                    SetDTD001_IsConfirm(true);
                }
            }

            #endregion
            Decimal difference = 0;
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'"));
            if (_CountDTD001 > 0)
            {
                decimal totalPackQty = 0;
                if (GetConfirmedQty() > GetQty())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ConfirmQtyNotGreater"));
                    return false;
                }
                if (GetVAM_Inv_InOutLine_ID() > 0 && newRecord)
                {
                    //totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM VAM_PackagingLine WHERE VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID()));
                    //totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM VAM_PackagingLine WHERE VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM VAM_PackagingLine WHERE  VAM_Packaging_ID=" + GetVAM_Packaging_ID() + " AND VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID()));
                }
                else if (GetVAM_InvTrf_Line_ID() > 0 && newRecord)
                {
                    // totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM VAM_PackagingLine WHERE VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM VAM_PackagingLine WHERE VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID()));
                }
                else if (GetVAM_Inv_InOutLine_ID() > 0 && !(newRecord))
                {
                    // totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM VAM_PackagingLine WHERE VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID() + " AND VAM_PackagingLine_ID<>" + GetVAM_PackagingLine_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM VAM_PackagingLine WHERE VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID() + " AND VAM_PackagingLine_ID<>" + GetVAM_PackagingLine_ID()));
                }
                else if (GetVAM_InvTrf_Line_ID() > 0 && !(newRecord))
                {

                    // totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM VAM_PackagingLine WHERE VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID() + " AND VAM_PackagingLine_ID<>" + GetVAM_PackagingLine_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM VAM_PackagingLine WHERE VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID() + " AND VAM_PackagingLine_ID<>" + GetVAM_PackagingLine_ID()));
                }
                else if (!(GetVAM_InvTrf_Line_ID() > 0) && !(GetVAM_Inv_InOutLine_ID() > 0))
                {
                    difference = GetQty();
                    difference = Decimal.Subtract(difference, GetConfirmedQty());
                    difference = Decimal.Subtract(difference, GetScrappedQty());
                    SetDifferenceQty(difference);
                    return true;
                }
                else if (!(GetVAM_InvTrf_Line_ID() > 0) && !(GetVAM_Inv_InOutLine_ID() > 0) && !newRecord)
                {
                    totalPackQty = 0;
                }

                decimal totalPackQtyNew = totalPackQty + GetConfirmedQty();
                SetDTD001_AlreadyPackQty(totalPackQty);
                //commented by shivani
                //if (totalPackQty + GetQty() > GetDTD001_TotalQty())
                //{
                //    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_MoreThenTotalQty"));
                //    //  ShowMessage.Warn(Msg.GetMsg(GetCtx(), "DTD001_MoreThenTotalQty"), null, "", "");
                //    return false;
                //}
                //////
                //_sql = "SELECT COUNT(*) FROM   VAM_InvTrf_Line WHERE IsActive='Y' AND  VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID() + " AND VAM_PFeature_SetInstance_ID=" + GetVAM_PFeature_SetInstance_ID();
                //int MvAttribute = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM   VAM_InvTrf_Line WHERE IsActive='Y' AND  VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID() + " AND VAM_PFeature_SetInstance_ID=" + GetVAM_PFeature_SetInstance_ID()));
                //if (MvAttribute > 0)
                //{

                //}
                //else
                //{
                //    return false;
                //}
                difference = GetQty();
                difference = Decimal.Subtract(difference, GetConfirmedQty());
                difference = Decimal.Subtract(difference, GetScrappedQty());
                SetDifferenceQty(difference);

                int _pckgAttrbt = GetVAM_PFeature_SetInstance_ID();

                if (GetVAM_InvTrf_Line_ID() > 0)
                {
                    int MvAttribute = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_PFeature_SetInstance_ID  FROM   VAM_InvTrf_Line WHERE IsActive='Y' AND  VAM_InvTrf_Line_ID=" + GetVAM_InvTrf_Line_ID() + " AND VAM_Product_ID=" + GetVAM_Product_ID()));

                    if (MvAttribute == _pckgAttrbt)
                    {

                    }
                    else
                    {
                        log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_AttributeNotSetMvL"));
                        // ShowMessage.Warn(Msg.GetMsg(GetCtx(), "DTD001_AttributeNotSetMvL"), null, "", "");
                        return false;
                    }
                }

            }

            return true;
        }

        /// </summary>
        /// <param name="line"></param>
    }
}

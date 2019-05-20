/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPackageLine
 * Purpose        : Package Line Model
 * Class Used     : X_M_PackageLine
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
    public class MPackageLine : X_M_PackageLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PackageLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPackageLine(Ctx ctx, int M_PackageLine_ID, Trx trxName)
            : base(ctx, M_PackageLine_ID, trxName)
        {
            if (M_PackageLine_ID == 0)
            {
                //	setM_Package_ID (0);
                //	setM_InOutLine_ID (0);
                SetQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MPackageLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">header</param>
        public MPackageLine(MPackage parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_Package_ID(parent.GetM_Package_ID());
        }

        /// <summary>
        /// Set Shipment Line
        /// </summary>
        /// <param name="line">line</param>
        public void SetInOutLine(MInOutLine line, DateTime? moveDate, String DocumentNo, Int32 Client_ID, Int32 Org_ID)
        {
            SetM_InOutLine_ID(line.GetM_InOutLine_ID());
            SetQty(line.GetMovementQty());
            //Edited :Arpit Rai ,13 Sept,2017 
            //to Set Client,Org,Confirm Date,Confirm Date, Scrapped Qty, Difference Qty & Reference No
            SetAD_Client_ID(Client_ID);
            SetAD_Org_ID(Org_ID);
            SetM_Product_ID(line.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(line.GetM_AttributeSetInstance_ID());
            if (Util.GetValueOfInt(DB.ExecuteQuery("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'")) > 0
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
        public Decimal SetMovementLine(VAdvantage.Model.MMovementLine line)
        {

            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'"));
            if (_CountDTD001 > 0)
            {
                SetM_MovementLine_ID(line.GetM_MovementLine_ID());
                SetDTD001_TotalQty(line.GetMovementQty());
                SetM_Product_ID(line.GetM_Product_ID());
                SetM_AttributeSetInstance_ID(line.GetM_AttributeSetInstance_ID());
                decimal totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_MovementLine_ID=" + line.GetM_MovementLine_ID()));
                SetQty(line.GetMovementQty() - totalPackQty);
                _count = Util.GetValueOfDecimal(line.GetMovementQty()) - totalPackQty;
                SetDTD001_AlreadyPackQty(totalPackQty);
                SetConfirmedQty(line.GetMovementQty() - totalPackQty);
                SetDTD001_ConfirmDate(System.DateTime.Now);

            }
            return _count;
        }
        public Decimal SetInoutLine(VAdvantage.Model.MInOutLine line)
        {
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
            if (_CountDTD001 > 0)
            {
                SetM_InOutLine_ID(line.GetM_InOutLine_ID());
                SetDTD001_TotalQty(line.GetMovementQty());
                SetM_Product_ID(line.GetM_Product_ID());
                SetM_AttributeSetInstance_ID(line.GetM_AttributeSetInstance_ID());
                decimal totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID()));
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
            GetM_Package_ID();
            GetM_MovementLine_ID();
            GetM_InOutLine_ID();
            string sql = null;
            string DocS = null;

            if (Util.GetValueOfInt(GetM_MovementLine_ID()) != 0)
            {
                sql = "Select mv.docstatus FROM m_movementline ml   INNER JOIN m_movement mv   ON (ml.m_movement_id=mv.m_movement_id) where ml.m_movementline_id=" + GetM_MovementLine_ID();
                DocS = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (DocS == "CO")
                {
                }
                else if (DocS == "IP")
                {
                    SetDTD001_IsConfirm(true);
                }
            }
            if (Util.GetValueOfInt(GetM_InOutLine_ID()) != 0)
            {
                sql = "SELECT o.docstatus    FROM  M_InOutLine ol    INNER JOIN M_InOut o     ON (ol.M_InOut_ID=o.M_InOut_ID) where ol.M_InOutLine_ID=" + GetM_InOutLine_ID();
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
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'"));
            if (_CountDTD001 > 0)
            {
                decimal totalPackQty = 0;
                if (GetConfirmedQty() > GetQty())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ConfirmQtyNotGreater"));
                    return false;
                }
                if (GetM_InOutLine_ID() > 0 && newRecord)
                {
                    //totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                    //totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM M_PackageLine WHERE  M_Package_ID=" + GetM_Package_ID() + " AND M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                }
                else if (GetM_MovementLine_ID() > 0 && newRecord)
                {
                    // totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM M_PackageLine WHERE M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                }
                else if (GetM_InOutLine_ID() > 0 && !(newRecord))
                {
                    // totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_PackageLine_ID<>" + GetM_PackageLine_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_PackageLine_ID<>" + GetM_PackageLine_ID()));
                }
                else if (GetM_MovementLine_ID() > 0 && !(newRecord))
                {

                    // totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_PackageLine_ID<>" + GetM_PackageLine_ID()));
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(ConfirmedQty) FROM M_PackageLine WHERE M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_PackageLine_ID<>" + GetM_PackageLine_ID()));
                }
                else if (!(GetM_MovementLine_ID() > 0) && !(GetM_InOutLine_ID() > 0))
                {
                    difference = GetQty();
                    difference = Decimal.Subtract(difference, GetConfirmedQty());
                    difference = Decimal.Subtract(difference, GetScrappedQty());
                    SetDifferenceQty(difference);
                    return true;
                }
                else if (!(GetM_MovementLine_ID() > 0) && !(GetM_InOutLine_ID() > 0) && !newRecord)
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
                //_sql = "SELECT COUNT(*) FROM   M_MovementLine WHERE IsActive='Y' AND  M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID();
                //int MvAttribute = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM   M_MovementLine WHERE IsActive='Y' AND  M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID()));
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

                int _pckgAttrbt = GetM_AttributeSetInstance_ID();

                if (GetM_MovementLine_ID() > 0)
                {
                    int MvAttribute = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_AttributeSetInstance_ID  FROM   M_MovementLine WHERE IsActive='Y' AND  M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));

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

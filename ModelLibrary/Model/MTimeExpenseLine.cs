/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTimeExpenseLine
 * Purpose        : Time + Expense Line Model
 * Class Used     : X_VAS_ExpenseReportLine
 * Chronological    Development
 * Deepak          31-Dec-2009
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
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MTimeExpenseLine : X_VAS_ExpenseReportLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAS_ExpenseReportLine_ID"></param>
        /// <param name="trxName">transaction</param>
        public MTimeExpenseLine(Ctx ctx, int VAS_ExpenseReportLine_ID, Trx trxName)
            : base(ctx, VAS_ExpenseReportLine_ID, trxName)
        {
            //super (ctx, VAS_ExpenseReportLine_ID, trxName);
            if (VAS_ExpenseReportLine_ID == 0)
            {
                //	setVAS_ExpenseReportLine_ID (0);		//	PK
                //	setVAS_ExpenseReport_ID (0);			//	Parent
                SetQty(Env.ONE);
                SetQtyInvoiced(Env.ZERO);
                SetQtyReimbursed(Env.ZERO);
                //
                SetExpenseAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
                SetPriceReimbursed(Env.ZERO);
                SetInvoicePrice(Env.ZERO);
                SetPriceInvoiced(Env.ZERO);
                //
                //SetDateExpense (new Timestamp(System.currentTimeMillis()));
                SetDateExpense(DateTime.Now);
                SetIsInvoiced(false);
                SetIsTimeReport(false);
                SetLine(10);
                SetProcessed(false);
            }
        }	//	MTimeExpenseLine

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MTimeExpenseLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MTimeExpenseLine

        /**	Currency of Report			*/
        private int _VAB_Currency_Report_ID = 0;


        /// <summary>
        /// Get Qty Invoiced
        /// </summary>
        /// <returns>entered or qty</returns>
        public new Decimal GetQtyInvoiced()
        {
            Decimal bd = base.GetQtyInvoiced();
            if (Env.ZERO.CompareTo(bd) == 0)
            {
                return GetQty();
            }
            return bd;
        }	//	getQtyInvoiced

        /// <summary>
        /// Get Qty Reimbursed
        /// </summary>
        /// <returns>entered or qty</returns>
        public new Decimal GetQtyReimbursed()
        {
            Decimal bd = base.GetQtyReimbursed();
            if (Env.ZERO.CompareTo(bd) == 0)
            {
                return GetQty();
            }
            return bd;
        }	//	getQtyReimbursed


        /// <summary>
        /// Get Price Invoiced
        /// </summary>
        /// <returns>entered or invoice price</returns>
        public new Decimal GetPriceInvoiced()
        {
            Decimal bd = base.GetPriceInvoiced();
            if (Env.ZERO.CompareTo(bd) == 0)
            {
                return GetInvoicePrice();
            }
            return bd;
        }	//	getPriceInvoiced

        /// <summary>
        /// Get Price Reimbursed
        /// </summary>
        /// <returns>entered or converted amt</returns>
        public new Decimal GetPriceReimbursed()
        {
            Decimal bd = base.GetPriceReimbursed();
            if (Env.ZERO.CompareTo(bd) == 0)
            {
                return GetConvertedAmt();
            }
            return bd;
        }	//	getPriceReimbursed


        /// <summary>
        /// Get Approval Amt
        /// </summary>
        /// <returns>qty * converted amt</returns>
        public Decimal GetApprovalAmt()
        {
            //return getQty().multiply(getConvertedAmt());
            return Decimal.Multiply(GetQty(), GetConvertedAmt());
        }	//	getApprovalAmt

        /// <summary>
        /// Get VAB_Currency_ID of Report (Price List)
        /// </summary>
        /// <returns>currency</returns>
        public int GetVAB_Currency_Report_ID()
        {
            if (_VAB_Currency_Report_ID != 0)
            {
                return _VAB_Currency_Report_ID;
            }
            //	Get it from header
            MTimeExpense te = new MTimeExpense(GetCtx(), GetVAS_ExpenseReport_ID(), Get_TrxName());
            _VAB_Currency_Report_ID = te.GetVAB_Currency_ID();
            return _VAB_Currency_Report_ID;
        }	//	getVAB_Currency_Report_ID

        /// <summary>
        /// Set VAB_Currency_ID of Report (Price List)
        /// </summary>
        /// <param name="VAB_Currency_ID">currency</param>
        public void SetVAB_Currency_Report_ID(int VAB_Currency_ID)
        {
            _VAB_Currency_Report_ID = VAB_Currency_ID;
        }	//	getVAB_Currency_Report_ID


        /// <summary>
        /// Set Resource Assignment - Callout
        /// </summary>
        /// <param name="oldVAS_Res_Assignment_ID">old value</param>
        /// <param name="newVAS_Res_Assignment_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetVAS_Res_Assignment_ID(String oldVAS_Res_Assignment_ID,
               String newVAS_Res_Assignment_ID, int windowNo)
        {
            if (newVAS_Res_Assignment_ID == null || newVAS_Res_Assignment_ID.Length == 0)
            {
                return;
            }
            //int VAS_Res_Assignment_ID = Integer.parseInt(newVAS_Res_Assignment_ID);
            int VAS_Res_Assignment_ID = Util.GetValueOfInt(newVAS_Res_Assignment_ID);
            if (VAS_Res_Assignment_ID == 0)
            {
                return;
            }
            //
            base.SetVAS_Res_Assignment_ID(VAS_Res_Assignment_ID);

            int VAM_Product_ID = 0;
            String Name = null;
            String Description = null;
            Decimal? Qty = null;
            String sql = "SELECT p.VAM_Product_ID, ra.Name, ra.Description, ra.Qty "
                + "FROM VAS_Res_Assignment ra"
                + " INNER JOIN VAM_Product p ON (p.VAS_Resource_ID=ra.VAS_Resource_ID) "
                + "WHERE ra.VAS_Res_Assignment_ID=@param";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, VAS_Res_Assignment_ID);
                param[0] = new SqlParameter("@param", VAS_Res_Assignment_ID);
                //ResultSet rs = pstmt.executeQuery();
                dr = DB.ExecuteReader(sql, param, null);
                if (dr.Read())
                {
                    VAM_Product_ID = Util.GetValueOfInt(dr[0]);// rs.getInt (1);
                    Name = Util.GetValueOfString(dr[1]);// rs.getString(2);
                    Description = Util.GetValueOfString(dr[2]);// rs.getString(3);
                    Qty = Util.GetValueOfDecimal(dr[3]);// rs.getBigDecimal(4);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            log.Fine("VAS_Res_Assignment_ID=" + VAS_Res_Assignment_ID
                    + " - VAM_Product_ID=" + VAM_Product_ID);
            if (VAM_Product_ID != 0)
            {
                SetVAM_Product_ID(VAM_Product_ID);
                if (Description != null)
                {
                    Name += " (" + Description + ")";
                }
                if (!".".Equals(Name))
                {
                    SetDescription(Name);
                }
                if (Qty != null)
                {
                    SetQty(Qty);
                }
            }
        }	//	setVAS_Res_Assignment_ID

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldVAM_Product_ID">old value</param>
        /// <param name="newVAM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetVAM_Product_ID(String oldVAM_Product_ID,
                String newVAM_Product_ID, int windowNo)
        {
            if (newVAM_Product_ID == null || newVAM_Product_ID.Length == 0)
            {
                return;
            }
            //int VAM_Product_ID = Integer.parseInt(newVAM_Product_ID);
            int VAM_Product_ID = Util.GetValueOfInt(newVAM_Product_ID);
            base.SetVAM_Product_ID(VAM_Product_ID);
            if (VAM_Product_ID == 0)
            {
                return;
            }

            //	Employee
            MTimeExpense hdr = new MTimeExpense(GetCtx(), GetVAS_ExpenseReport_ID(), null);
            int VAB_BusinessPartner_ID = hdr.GetVAB_BusinessPartner_ID();
            Decimal Qty = GetQty();
            Boolean IsSOTrx = true;
            MProductPricing pp = new MProductPricing(GetVAF_Client_ID(), GetVAF_Org_ID(),
                    VAM_Product_ID, VAB_BusinessPartner_ID, Qty, IsSOTrx);
            //
            int VAM_PriceList_ID = hdr.GetVAM_PriceList_ID();
            pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
            DateTime? orderDate = GetDateExpense();
            pp.SetPriceDate(orderDate);
            //
            SetExpenseAmt(pp.GetPriceStd());
            SetVAB_Currency_ID(pp.GetVAB_Currency_ID());
            SetAmt(windowNo, "VAM_Product_ID");
        }	//	setVAM_Product_ID

        /// <summary>
        /// Set Currency - Callout
        /// </summary>
        /// <param name="oldVAB_Currency_ID">old value</param>
        /// <param name="newVAB_Currency_ID">new value</param>
        /// <param name="windowNo"> window</param>
        public void SetVAB_Currency_ID(String oldVAB_Currency_ID,
               String newVAB_Currency_ID, int windowNo)
        {
            if (newVAB_Currency_ID == null || newVAB_Currency_ID.Length == 0)
            {
                return;
            }
            int VAB_Currency_ID = Util.GetValueOfInt(newVAB_Currency_ID);
            base.SetVAB_Currency_ID(VAB_Currency_ID);
            SetAmt(windowNo, "VAB_Currency_ID");
        }	//	setVAB_Currency_ID

        /// <summary>
        /// Set ExpenseAmt - Callout
        /// </summary>
        /// <param name="oldExpenseAmt">old value</param>
        /// <param name="newExpenseAmt">new value</param>
        /// <param name="windowNo">window</param>
        public void SetExpenseAmt(String oldExpenseAmt,
               String newExpenseAmt, int windowNo)
        {
            if (newExpenseAmt == null || newExpenseAmt.Length == 0)
            {
                return;
            }
            //Decimal ExpenseAmt = new BigDecimal(newExpenseAmt);
            Decimal ExpenseAmt = Util.GetValueOfDecimal(newExpenseAmt);
            base.SetExpenseAmt(ExpenseAmt);
            SetAmt(windowNo, "ExpenseAmt");
        }	//	setExpenseAmt

        /// <summary>
        /// Set Amount (Callout)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">column name</param>
        private void SetAmt(int windowNo, String columnName)
        {
            //	get values
            Decimal ExpenseAmt = GetExpenseAmt();
            int VAB_Currency_From_ID = GetVAB_Currency_ID();
            int VAB_Currency_To_ID = GetCtx().GetContextAsInt("$VAB_Currency_ID");
            DateTime? DateExpense = GetDateExpense();
            //
            log.Fine("Amt=" + ExpenseAmt + ", VAB_Currency_ID=" + VAB_Currency_From_ID);
            //	Converted Amount = Unit price
            Decimal ConvertedAmt = ExpenseAmt;
            //	convert if required
            //if (ConvertedAmt.signum() != 0 && VAB_Currency_To_ID != VAB_Currency_From_ID)
            if (Env.Signum(ConvertedAmt) != 0 && VAB_Currency_To_ID != VAB_Currency_From_ID)
            {
                ConvertedAmt = VAdvantage.Model.MVABExchangeRate.Convert(GetCtx(),
                    ConvertedAmt, VAB_Currency_From_ID, VAB_Currency_To_ID,
                    DateExpense, 0, GetVAF_Client_ID(), GetVAF_Org_ID());
            }
            SetConvertedAmt(ConvertedAmt);
            log.Fine("ConvertedAmt=" + ConvertedAmt);
        }   //	setAmt

        /// <summary>
        /// Before Save.
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Calculate Converted Amount
            if (newRecord || Is_ValueChanged("ExpenseAmt") || Is_ValueChanged("VAB_Currency_ID") || Is_ValueChanged("DateExpense"))
            {
                if (GetVAB_Currency_ID() == GetVAB_Currency_Report_ID())
                {
                    SetConvertedAmt(GetExpenseAmt());
                }
                else
                {
                    // did changes to give error message when conversion is not found.-Mohit
                    decimal convertedAmt = VAdvantage.Model.MVABExchangeRate.Convert(GetCtx(),
                        GetExpenseAmt(), GetVAB_Currency_ID(), GetVAB_Currency_Report_ID(), 
                        GetDateExpense(), 0, GetVAF_Client_ID(), GetVAF_Org_ID());
                    if (convertedAmt.Equals(0))
                    {
                        log.SaveError("ConversionNotFound", "");
                        return false;
                    }
                    SetConvertedAmt(convertedAmt);
                }
            }
            if (IsTimeReport())
            {
                SetExpenseAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }

            if (IsBillToCustomer())
            {
                if (Util.GetValueOfInt(GetVAB_BusinessPartner_ID()) == 0)
                {
                    //throw new Exception("SelectBusinessPartner");
                    // ShowMessage.Info("SelectBusinessPartner", true, null, null);
                    return false;
                }
            }

            return true;
        }	//	beforeSave


        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns> success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (success)
            {
                UpdateHeader();
                if (newRecord || Is_ValueChanged("VAS_Res_Assignment_ID"))
                {
                    int VAS_Res_Assignment_ID = GetVAS_Res_Assignment_ID();
                    int old_VAS_Res_Assignment_ID = 0;
                    if (!newRecord)
                    {
                        Object ii = Get_ValueOld("VAS_Res_Assignment_ID");
                        //if (ii instanceof Integer)
                        if (ii is int)
                        {
                            //old_VAS_Res_Assignment_ID = ((Integer)ii).intValue();
                            old_VAS_Res_Assignment_ID = Util.GetValueOfInt((int)ii);
                            //	Changed Assignment
                            if (old_VAS_Res_Assignment_ID != VAS_Res_Assignment_ID
                                && old_VAS_Res_Assignment_ID != 0)
                            {
                                MResourceAssignment ra = new MResourceAssignment(GetCtx(),
                                    old_VAS_Res_Assignment_ID, Get_TrxName());
                                ra.Delete(false);
                            }
                        }
                    }
                    //	Sync Assignment
                    if (VAS_Res_Assignment_ID != 0)
                    {
                        MResourceAssignment ra = new MResourceAssignment(GetCtx(),
                            VAS_Res_Assignment_ID, Get_TrxName());
                        if (GetQty().CompareTo(ra.GetQty()) != 0)
                        {
                            ra.SetQty(GetQty());
                            if (GetDescription() != null && GetDescription().Length > 0)
                            {
                                ra.SetDescription(GetDescription());
                            }
                            ra.Save();
                        }
                    }
                }
            }
            return success;
        }	//	afterSave


        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override Boolean AfterDelete(Boolean success)
        {
            if (success)
            {
                UpdateHeader();
                //
                Object ii = Get_ValueOld("VAS_Res_Assignment_ID");
                if (ii is int)
                {
                    int old_VAS_Res_Assignment_ID = Util.GetValueOfInt((int)ii);
                    //	Deleted Assignment
                    if (old_VAS_Res_Assignment_ID != 0)
                    {
                        MResourceAssignment ra = new MResourceAssignment(GetCtx(),
                            old_VAS_Res_Assignment_ID, Get_TrxName());
                        ra.Delete(false);
                    }
                }
            }
            return success;
        }	//	afterDelete

        /// <summary>
        /// Update Header.	Set Approved Amount
        /// </summary>
        private void UpdateHeader()
        {
            String sql = "UPDATE VAS_ExpenseReport te"
                + " SET ApprovalAmt = "
                    + "(SELECT SUM(Qty*ConvertedAmt) FROM VAS_ExpenseReportLine tel "
                    + "WHERE te.VAS_ExpenseReport_ID=tel.VAS_ExpenseReport_ID) "
                + "WHERE VAS_ExpenseReport_ID=" + GetVAS_ExpenseReport_ID();
            DB.ExecuteQuery(sql, null, Get_TrxName());
        }	//	updateHeader


    }	//	MTimeExpenseLine

}

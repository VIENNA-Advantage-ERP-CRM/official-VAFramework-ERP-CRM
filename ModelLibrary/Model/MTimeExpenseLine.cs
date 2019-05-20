/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTimeExpenseLine
 * Purpose        : Time + Expense Line Model
 * Class Used     : X_S_TimeExpenseLine
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MTimeExpenseLine : X_S_TimeExpenseLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="S_TimeExpenseLine_ID"></param>
        /// <param name="trxName">transaction</param>
        public MTimeExpenseLine(Ctx ctx, int S_TimeExpenseLine_ID, Trx trxName)
            : base(ctx, S_TimeExpenseLine_ID, trxName)
        {
            //super (ctx, S_TimeExpenseLine_ID, trxName);
            if (S_TimeExpenseLine_ID == 0)
            {
                //	setS_TimeExpenseLine_ID (0);		//	PK
                //	setS_TimeExpense_ID (0);			//	Parent
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
        private int _C_Currency_Report_ID = 0;


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
        /// Get C_Currency_ID of Report (Price List)
        /// </summary>
        /// <returns>currency</returns>
        public int GetC_Currency_Report_ID()
        {
            if (_C_Currency_Report_ID != 0)
            {
                return _C_Currency_Report_ID;
            }
            //	Get it from header
            MTimeExpense te = new MTimeExpense(GetCtx(), GetS_TimeExpense_ID(), Get_TrxName());
            _C_Currency_Report_ID = te.GetC_Currency_ID();
            return _C_Currency_Report_ID;
        }	//	getC_Currency_Report_ID

        /// <summary>
        /// Set C_Currency_ID of Report (Price List)
        /// </summary>
        /// <param name="C_Currency_ID">currency</param>
        public void SetC_Currency_Report_ID(int C_Currency_ID)
        {
            _C_Currency_Report_ID = C_Currency_ID;
        }	//	getC_Currency_Report_ID


        /// <summary>
        /// Set Resource Assignment - Callout
        /// </summary>
        /// <param name="oldS_ResourceAssignment_ID">old value</param>
        /// <param name="newS_ResourceAssignment_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetS_ResourceAssignment_ID(String oldS_ResourceAssignment_ID,
               String newS_ResourceAssignment_ID, int windowNo)
        {
            if (newS_ResourceAssignment_ID == null || newS_ResourceAssignment_ID.Length == 0)
            {
                return;
            }
            //int S_ResourceAssignment_ID = Integer.parseInt(newS_ResourceAssignment_ID);
            int S_ResourceAssignment_ID = Util.GetValueOfInt(newS_ResourceAssignment_ID);
            if (S_ResourceAssignment_ID == 0)
            {
                return;
            }
            //
            base.SetS_ResourceAssignment_ID(S_ResourceAssignment_ID);

            int M_Product_ID = 0;
            String Name = null;
            String Description = null;
            Decimal? Qty = null;
            String sql = "SELECT p.M_Product_ID, ra.Name, ra.Description, ra.Qty "
                + "FROM S_ResourceAssignment ra"
                + " INNER JOIN M_Product p ON (p.S_Resource_ID=ra.S_Resource_ID) "
                + "WHERE ra.S_ResourceAssignment_ID=@param";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, S_ResourceAssignment_ID);
                param[0] = new SqlParameter("@param", S_ResourceAssignment_ID);
                //ResultSet rs = pstmt.executeQuery();
                dr = DB.ExecuteReader(sql, param, null);
                if (dr.Read())
                {
                    M_Product_ID = Util.GetValueOfInt(dr[0]);// rs.getInt (1);
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

            log.Fine("S_ResourceAssignment_ID=" + S_ResourceAssignment_ID
                    + " - M_Product_ID=" + M_Product_ID);
            if (M_Product_ID != 0)
            {
                SetM_Product_ID(M_Product_ID);
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
        }	//	setS_ResourceAssignment_ID

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetM_Product_ID(String oldM_Product_ID,
                String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
            {
                return;
            }
            //int M_Product_ID = Integer.parseInt(newM_Product_ID);
            int M_Product_ID = Util.GetValueOfInt(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            if (M_Product_ID == 0)
            {
                return;
            }

            //	Employee
            MTimeExpense hdr = new MTimeExpense(GetCtx(), GetS_TimeExpense_ID(), null);
            int C_BPartner_ID = hdr.GetC_BPartner_ID();
            Decimal Qty = GetQty();
            Boolean IsSOTrx = true;
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    M_Product_ID, C_BPartner_ID, Qty, IsSOTrx);
            //
            int M_PriceList_ID = hdr.GetM_PriceList_ID();
            pp.SetM_PriceList_ID(M_PriceList_ID);
            DateTime? orderDate = GetDateExpense();
            pp.SetPriceDate(orderDate);
            //
            SetExpenseAmt(pp.GetPriceStd());
            SetC_Currency_ID(pp.GetC_Currency_ID());
            SetAmt(windowNo, "M_Product_ID");
        }	//	setM_Product_ID

        /// <summary>
        /// Set Currency - Callout
        /// </summary>
        /// <param name="oldC_Currency_ID">old value</param>
        /// <param name="newC_Currency_ID">new value</param>
        /// <param name="windowNo"> window</param>
        public void SetC_Currency_ID(String oldC_Currency_ID,
               String newC_Currency_ID, int windowNo)
        {
            if (newC_Currency_ID == null || newC_Currency_ID.Length == 0)
            {
                return;
            }
            int C_Currency_ID = Util.GetValueOfInt(newC_Currency_ID);
            base.SetC_Currency_ID(C_Currency_ID);
            SetAmt(windowNo, "C_Currency_ID");
        }	//	setC_Currency_ID

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
            int C_Currency_From_ID = GetC_Currency_ID();
            int C_Currency_To_ID = GetCtx().GetContextAsInt("$C_Currency_ID");
            DateTime? DateExpense = GetDateExpense();
            //
            log.Fine("Amt=" + ExpenseAmt + ", C_Currency_ID=" + C_Currency_From_ID);
            //	Converted Amount = Unit price
            Decimal ConvertedAmt = ExpenseAmt;
            //	convert if required
            //if (ConvertedAmt.signum() != 0 && C_Currency_To_ID != C_Currency_From_ID)
            if (Env.Signum(ConvertedAmt) != 0 && C_Currency_To_ID != C_Currency_From_ID)
            {
                ConvertedAmt = VAdvantage.Model.MConversionRate.Convert(GetCtx(),
                    ConvertedAmt, C_Currency_From_ID, C_Currency_To_ID,
                    DateExpense, 0, GetAD_Client_ID(), GetAD_Org_ID());
            }
            SetConvertedAmt(ConvertedAmt);
            log.Fine("ConvertedAmt=" + ConvertedAmt);
        }	//	setAmt

        /// <summary>
        /// Before Save.
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Calculate Converted Amount
            if (newRecord || Is_ValueChanged("ExpenseAmt") || Is_ValueChanged("C_Currency_ID"))
            {
                if (GetC_Currency_ID() == GetC_Currency_Report_ID())
                {
                    SetConvertedAmt(GetExpenseAmt());
                }
                else
                {
                    SetConvertedAmt(VAdvantage.Model.MConversionRate.Convert(GetCtx(),
                        GetExpenseAmt(), GetC_Currency_Report_ID(), GetC_Currency_ID(), 
                        GetDateExpense(), 0, GetAD_Client_ID(), GetAD_Org_ID()));
                }
            }
            if (IsTimeReport())
            {
                SetExpenseAmt(Env.ZERO);
                SetConvertedAmt(Env.ZERO);
            }

            if (IsBillToCustomer())
            {
                if (Util.GetValueOfInt(GetC_BPartner_ID()) == 0)
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
                if (newRecord || Is_ValueChanged("S_ResourceAssignment_ID"))
                {
                    int S_ResourceAssignment_ID = GetS_ResourceAssignment_ID();
                    int old_S_ResourceAssignment_ID = 0;
                    if (!newRecord)
                    {
                        Object ii = Get_ValueOld("S_ResourceAssignment_ID");
                        //if (ii instanceof Integer)
                        if (ii is int)
                        {
                            //old_S_ResourceAssignment_ID = ((Integer)ii).intValue();
                            old_S_ResourceAssignment_ID = Util.GetValueOfInt((int)ii);
                            //	Changed Assignment
                            if (old_S_ResourceAssignment_ID != S_ResourceAssignment_ID
                                && old_S_ResourceAssignment_ID != 0)
                            {
                                MResourceAssignment ra = new MResourceAssignment(GetCtx(),
                                    old_S_ResourceAssignment_ID, Get_TrxName());
                                ra.Delete(false);
                            }
                        }
                    }
                    //	Sync Assignment
                    if (S_ResourceAssignment_ID != 0)
                    {
                        MResourceAssignment ra = new MResourceAssignment(GetCtx(),
                            S_ResourceAssignment_ID, Get_TrxName());
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
                Object ii = Get_ValueOld("S_ResourceAssignment_ID");
                if (ii is int)
                {
                    int old_S_ResourceAssignment_ID = Util.GetValueOfInt((int)ii);
                    //	Deleted Assignment
                    if (old_S_ResourceAssignment_ID != 0)
                    {
                        MResourceAssignment ra = new MResourceAssignment(GetCtx(),
                            old_S_ResourceAssignment_ID, Get_TrxName());
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
            String sql = "UPDATE S_TimeExpense te"
                + " SET ApprovalAmt = "
                    + "(SELECT SUM(Qty*ConvertedAmt) FROM S_TimeExpenseLine tel "
                    + "WHERE te.S_TimeExpense_ID=tel.S_TimeExpense_ID) "
                + "WHERE S_TimeExpense_ID=" + GetS_TimeExpense_ID();
            DB.ExecuteQuery(sql, null, Get_TrxName());
        }	//	updateHeader


    }	//	MTimeExpenseLine

}

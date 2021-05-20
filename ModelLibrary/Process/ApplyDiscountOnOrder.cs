/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : ApplyDiscountOrder
    * Purpose        : Apply Discount And Percentage Discount from button
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological  : Development
    * Amit Bansal    : 14/02/2018
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
using System.Data;
using System.Data.SqlClient;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;

namespace VAdvantage.Process
{
    public class ApplyDiscountOnOrder : SvrProcess
    {
        // Order			
        private String _IsCLearDiscount = "N";
        private Decimal _DiscountAmt = 0.0M;
        private Decimal _DiscountPercent = 0.0M;
        private Decimal subTotal = 0.0M;
        private Decimal discountPercentageOnTotalAmount = 0.0M;
        private Decimal discountAmountOnTotal = 0.0M;
        private int precision = 0;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("DiscountAmt"))
                {
                    _DiscountAmt = Util.GetValueOfDecimal(para[i].GetParameter());
                }
                else if (name.Equals("Discount"))
                {
                    _DiscountPercent = Util.GetValueOfDecimal(para[i].GetParameter());
                }
                else if (name.Equals("IsClearDiscount"))
                {
                    _IsCLearDiscount = Util.GetValueOfString(para[i].GetParameter());
                }
            }
        }

        protected override string DoIt()
        {

            MOrder obj = new MOrder(GetCtx(), GetRecord_ID(), Get_Trx());

            // get Precision for rounding
            precision = MPriceList.Get(GetCtx(), obj.GetM_PriceList_ID(), Get_Trx()).GetPricePrecision();

            MOrderLine[] lines = obj.GetLines();

            if (_IsCLearDiscount == "N")
            {
                if (_DiscountAmt == 0 && _DiscountPercent == 0)
                {
                    return Msg.GetMsg(GetCtx(), "PlsSelAtlstOneField");
                }

                if (_DiscountAmt != 0 && _DiscountPercent != 0)
                {
                    return Msg.GetMsg(GetCtx(), "PlsSelOneField");
                }

                // get amount on which we have to apply discount
                subTotal = obj.GetTotalLines();

                // when we are giving discount in terms of amount, then we have to calculate discount in term of percentage
                discountPercentageOnTotalAmount = GetDiscountPercentageOnTotal(subTotal, _DiscountAmt, 12);

                for (int i = 0; i < lines.Length; i++)
                {
                    MOrderLine ln = lines[i];
                    // this value represent discount on line net amount
                    discountAmountOnTotal = GetDiscountAmountOnTotal(ln.GetLineNetAmt(), discountPercentageOnTotalAmount != 0 ? discountPercentageOnTotalAmount : _DiscountPercent);

                    // this value represent discount on unit price of 1 qty
                    discountAmountOnTotal = Decimal.Round(Decimal.Divide(discountAmountOnTotal, ln.GetQtyEntered()), 12);

                    if (discountPercentageOnTotalAmount != 0 && _DiscountAmt != 0)
                    {
                        if (i != lines.Length - 1)
                        {
                            // reduce discounted amount from total discount
                            _DiscountAmt -= discountAmountOnTotal;
                        }
                        else if (i == lines.Length - 1)
                        {
                            // when last iteration, set remaning amount
                            discountAmountOnTotal = _DiscountAmt;
                        }
                    }

                    ln.SetAmountAfterApplyDiscount(Decimal.Round(Decimal.Add(ln.GetAmountAfterApplyDiscount(), discountAmountOnTotal), precision));
                    ln.SetPriceActual(Decimal.Round(Decimal.Subtract(ln.GetPriceActual(), discountAmountOnTotal), precision));
                    ln.SetPriceEntered(Decimal.Round(Decimal.Subtract(ln.GetPriceEntered(), discountAmountOnTotal), precision));
                    if (!ln.Save(Get_TrxName()))
                    {
                        Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Info("ApplyDiscountOnOrder : Not Saved. Error Value : " + pp.GetValue() + " , Error Name : " + pp.GetName());
                        throw new Exception(Msg.GetMsg(GetCtx(), "DiscNotApplied"));
                    }
                }
                return Msg.GetMsg(GetCtx(), "DiscAppliedSuccess");
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    MOrderLine ln = lines[i];
                    ln.SetPriceEntered(Decimal.Add(ln.GetPriceEntered(), ln.GetAmountAfterApplyDiscount()));
                    ln.SetPriceActual(Decimal.Add(ln.GetPriceActual(), ln.GetAmountAfterApplyDiscount()));
                    ln.SetAmountAfterApplyDiscount(0);
                    if (!ln.Save())
                    {
                        Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Info("ApplyDiscountOnOrder : Not Saved. Error Value : " + pp.GetValue() + " , Error Name : " + pp.GetName());
                        throw new Exception(Msg.GetMsg(GetCtx(), "DiscNotCleared"));
                    }
                }
                return Msg.GetMsg(GetCtx(), "DiscClearedSuccessfully");
            }
        }

        // call when we give discount in terms of Amount
        private Decimal GetDiscountPercentageOnTotal(Decimal grandTotal, Decimal discountAmount, int precision)
        {
            Decimal discountPercentageOnTotal = 0.0M;
            if (grandTotal != 0 && discountAmount != 0)
            {
                discountPercentageOnTotal = Decimal.Round(Decimal.Multiply(Decimal.Divide(discountAmount, grandTotal), 100), precision); ;

            }
            return discountPercentageOnTotal;
        }

        private Decimal GetDiscountAmountOnTotal(Decimal lineNetAmount, Decimal discountPercentageOnTotal)
        {
            Decimal lineDiscountAmount = 0.0M;
            if (lineNetAmount != 0 && discountPercentageOnTotal != 0)
            {
                lineDiscountAmount = Decimal.Divide(Decimal.Multiply(lineNetAmount, discountPercentageOnTotal), 100);
            }
            return lineDiscountAmount;
        }
    }
}

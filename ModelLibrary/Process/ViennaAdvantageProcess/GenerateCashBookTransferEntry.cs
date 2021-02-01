using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using ViennaAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Model;

namespace ViennaAdvantage.Process
{
    public class GenerateCashBookTransferEntry : SvrProcess
    {
        private int VAB_CashJRNL_ID = 0;
        MVABCashJRNLLine cashline = null;
        MVABCashJRNL cash = null;
        //   bool completed = false;
        string sql = "";
        Decimal convertedamount = 0;
        private int Currency_ID = 0;
        private Decimal _Curencyrate = 0;
        int cashtype = 0;
        List<int> _cashIds = new List<int>();
        int i;
        protected override string DoIt()
        {

            MVABCashJRNL cashheader = new MVABCashJRNL(GetCtx(), GetRecord_ID(), Get_Trx());
            sql = "select VAB_Currency_ID from  VAB_CashBook where VAB_CashBook_id=" + cashheader.GetVAB_CashBook_ID();
            int VAB_Currencyheader_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            sql = "select * from VAB_CashJRNLLine WHERE VAB_CashJRNL_ID=" + GetRecord_ID();
            DataSet dscashline = DB.ExecuteDataset(sql, null, Get_Trx());
            if (dscashline != null)
            {
                if (dscashline.Tables[0].Rows.Count > 0)
                {

                    for (i = 0; i < dscashline.Tables[0].Rows.Count; i++)
                    {
                        cashline = new MVABCashJRNLLine(GetCtx(), Util.GetValueOfInt(dscashline.Tables[0].Rows[i]["VAB_CashJRNLLine_ID"]), Get_Trx());
                        if (cashline.GetCashType().ToString() == "A" || cashline.GetCashType().ToString() == "F")
                        {
                            sql = "Select * from VAB_CashJRNL where VAB_CashBook_Id=" + cashline.GetVAB_CashBook_ID() + " and docstatus='DR' and  DateAcct=" + GlobalVariable.TO_DATE(DateTime.Now, true);
                            DataSet dscashbook = DB.ExecuteDataset(sql, null, Get_Trx());
                            if (dscashbook != null)
                            {
                                if (dscashbook.Tables[0].Rows.Count > 0)
                                {
                                    int j;
                                    for (j = 0; j < dscashbook.Tables[0].Rows.Count; j++)
                                    {

                                        cash = new MVABCashJRNL(GetCtx(), Util.GetValueOfInt(dscashbook.Tables[0].Rows[j]["VAB_CashJRNL_ID"]), Get_Trx());
                                        if (!_cashIds.Contains(cash.GetVAB_CashJRNL_ID()))
                                        {
                                            _cashIds.Add(cash.GetVAB_CashJRNL_ID());
                                        }
                                        //ViennaAdvantage.Model.MCashLine cashline1 = new ViennaAdvantage.Model.MCashLine(GetCtx(), 0, Get_Trx());
                                        MVABCashJRNLLine cashline1 = new MVABCashJRNLLine(GetCtx(), 0, Get_Trx());
                                        cashline1.SetVAB_CashJRNL_ID(cash.GetVAB_CashJRNL_ID());
                                        cashline1.SetVAF_Client_ID(cash.GetVAF_Client_ID());
                                        cashline1.SetVAF_Org_ID(cash.GetVAF_Org_ID());
                                        if (cashline.GetCashType().ToString() == "A")
                                        {
                                            cashline1.SetCashType("F");
                                        }
                                        if (cashline.GetCashType().ToString() == "F")
                                        {
                                            cashline1.SetCashType("A");
                                        }
                                        // Added by Bharat as discussed with Ravikant on 22 March 2017
                                        if (cashline1.Get_ColumnIndex("VAB_CurrencyType_ID") > 0)
                                        {
                                            cashline1.SetVAB_CurrencyType_ID(cashline.GetVAB_CurrencyType_ID());
                                        }
                                        cashline1.SetVAB_CashBook_ID(cashheader.GetVAB_CashBook_ID());
                                        cashline1.SetVAB_BusinessPartner_ID(cashline.GetVAB_BusinessPartner_ID());
                                        sql = "select VAB_Currency_ID from  VAB_CashBook where VAB_CashBook_id=" + cash.GetVAB_CashBook_ID();
                                        Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                        if (Currency_ID == VAB_Currencyheader_ID)
                                        {
                                            cashline1.SetVAB_Currency_ID(VAB_Currencyheader_ID);
                                            cashline1.SetAmount(Decimal.Negate(cashline.GetAmount()));
                                            cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                        }
                                        else
                                        {
                                            //cashline1.SetVAB_Currency_ID(Currency_ID);
                                            // Change by Bharat as discussed with Ravikant on 22 March 2017
                                            cashline1.SetVAB_Currency_ID(VAB_Currencyheader_ID);
                                            if (cashline1.Get_ColumnIndex("VAB_CurrencyType_ID") > 0)
                                            {
                                                //_Curencyrate = MConversionRate.GetRate(VAB_Currencyheader_ID, Currency_ID, cash.GetDateAcct(), cashline.GetVAB_CurrencyType_ID(),
                                                //    cash.GetVAF_Client_ID(), cash.GetVAF_Org_ID());

                                                convertedamount = Decimal.Negate(Util.GetValueOfDecimal(cashline.GetConvertedAmt()));

                                                // if converted amount not found then get amount based on currency conversion avaliable
                                                if (cashline.GetAmount() != 0 && convertedamount == 0)
                                                {
                                                    convertedamount = MVABExchangeRate.Convert(GetCtx(), Decimal.Negate(cashline.GetAmount()), Currency_ID, VAB_Currencyheader_ID,
                                                        cash.GetDateAcct(), cashline.GetVAB_CurrencyType_ID(), cash.GetVAF_Client_ID(), cash.GetVAF_Org_ID());
                                                    cashline.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(convertedamount)));
                                                }

                                                if (cashline.GetAmount() != 0 && convertedamount == 0)
                                                {
                                                    return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                                }
                                                else if (cashline.GetAmount() == 0)
                                                {
                                                    return "Amount Should be greater than zero";
                                                }
                                                cashline1.SetAmount(convertedamount);
                                                cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                            }
                                            else
                                            {
                                                sql = "select multiplyrate from VAB_ExchangeRate where isactive='Y' and VAB_Currency_id=" + VAB_Currencyheader_ID + " and VAB_Currency_to_id=" + Currency_ID;
                                                _Curencyrate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
                                                if (_Curencyrate == 0)
                                                {
                                                    return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                                }
                                                else
                                                {
                                                    convertedamount = Decimal.Round(Decimal.Multiply(_Curencyrate, Decimal.Negate(cashline.GetAmount())), 2);
                                                }
                                                cashline1.SetConvertedAmt(Util.GetValueOfString(convertedamount));
                                                cashline1.SetAmount(convertedamount);
                                            }
                                        }

                                        cashline1.SetVAB_Charge_ID(cashline.GetVAB_Charge_ID());
                                        if (cashline.GetCashType().ToString() == "A")
                                        {
                                            cashline1.SetVSS_PAYMENTTYPE("R");
                                        }
                                        if (cashline.GetCashType().ToString() == "F")
                                        {
                                            cashline1.SetVSS_PAYMENTTYPE("P");

                                        }
                                        cashline1.SetVAB_CashJRNLLine_ID_1(cashline.GetVAB_CashJRNLLine_ID());
                                        if (!cashline1.Save())
                                        {
                                            Rollback();
                                            log.Severe("NotSaved");
                                            return " NotSaved Cashline1 [ Header Found ]  ";

                                        }
                                        if (cashline.GetCashType().ToString() == "F")
                                        {
                                            cashline.SetVAB_CashJRNLLine_ID_1(cashline1.GetVAB_CashJRNLLine_ID());
                                        }
                                        if (!cashline.Save())
                                        {
                                            Rollback();
                                            log.Severe("Not Saved");
                                            return " NotSaved Cashline [ Header Found ]  ";
                                        }
                                        //change by Amit 1-june-2016 after discussion with ravikant

                                        //int BeginBal = Util.GetValueOfInt(DB.ExecuteScalar("select completedbalance from VAB_CashBook where VAB_CashBook_id=" + cash.GetVAB_CashBook_ID(), null, Get_Trx()));
                                        //cash.SetBeginningBalance(BeginBal);
                                        //if (!cash.Save(Get_Trx()))
                                        //{
                                        //    Rollback();
                                        //    log.Severe("Not Saved");
                                        //    return " NotSaved Cash [ Header Found ]  ";

                                        //}

                                        //end

                                        break;
                                    }


                                }
                                else
                                {
                                    dscashbook.Dispose();

                                    MVABCashJRNL cash1 = new MVABCashJRNL(GetCtx(), 0, Get_Trx());
                                    cash1.SetVAF_Client_ID(cashheader.GetVAF_Client_ID());
                                    cash1.SetVAF_Org_ID(cashheader.GetVAF_Org_ID());
                                    cash1.SetVAB_DocTypes_ID(cashheader.GetVAB_DocTypes_ID());
                                    cash1.SetName(Util.GetValueOfString(System.DateTime.Today.ToShortDateString()));
                                    cash1.SetVAB_CashBook_ID(cashline.GetVAB_CashBook_ID());
                                    cash1.SetDateAcct(DateTime.Now);
                                    cash1.SetStatementDate(DateTime.Now);
                                    //change by Amit 1-june-2016 after discussion with ravikant
                                    int BeginBal = Util.GetValueOfInt(DB.ExecuteScalar("select SUM(completedbalance + runningbalance) from VAB_CashBook where VAB_CashBook_id=" + cashline.GetVAB_CashBook_ID(), null, Get_Trx()));
                                    cash1.SetBeginningBalance(BeginBal);
                                    //end
                                    //Change by Bharat as discussed with Ravikant on 22 March 2017
                                    sql = "select VAB_Currency_ID from  VAB_CashBook where VAB_CashBook_id=" + cash1.GetVAB_CashBook_ID();
                                    Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                    if (cash1.Get_ColumnIndex("VAB_Currency_ID") > 0)
                                    {
                                        cash1.SetVAB_Currency_ID(Currency_ID);
                                    }
                                    //End
                                    if (!cash1.Save())
                                    {
                                        Rollback();

                                        VAdvantage.Model.ValueNamePair ppE = VLogger.RetrieveError();
                                        if (ppE != null)
                                        {
                                            return ppE.GetName() + "<--->" + ppE.GetValue();
                                        }

                                        return "cash1 not saved [header not found->]";
                                    }
                                    // return cash1.GetVAB_CashJRNL_ID() + "suc";
                                    if (!_cashIds.Contains(cash1.GetVAB_CashJRNL_ID()))
                                    {
                                        _cashIds.Add(cash1.GetVAB_CashJRNL_ID());
                                    }
                                    MVABCashJRNLLine cashline1 = new MVABCashJRNLLine(GetCtx(), 0, Get_Trx());
                                    cashline1.SetVAB_CashJRNL_ID(cash1.GetVAB_CashJRNL_ID());
                                    cashline1.SetVAF_Client_ID(cash1.GetVAF_Client_ID());
                                    cashline1.SetVAF_Org_ID(cash1.GetVAF_Org_ID());
                                    if (cashline.GetCashType().ToString() == "A")
                                    {
                                        cashline1.SetCashType("F");
                                    }
                                    if (cashline.GetCashType().ToString() == "F")
                                    {
                                        cashline1.SetCashType("A");
                                    }
                                    if (cashline1.Get_ColumnIndex("VAB_CurrencyType_ID") > 0)
                                    {
                                        cashline1.SetVAB_CurrencyType_ID(cashline.GetVAB_CurrencyType_ID());
                                    }
                                    cashline1.SetVAB_CashBook_ID(cashheader.GetVAB_CashBook_ID());
                                    cashline1.SetVAB_BusinessPartner_ID(cashline.GetVAB_BusinessPartner_ID());

                                    //Change by Bharat as discussed with Ravikant on 22 March 2017
                                    //sql = "select VAB_Currency_ID from  VAB_CashBook where VAB_CashBook_id=" + cash1.GetVAB_CashBook_ID();
                                    //Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                    if (Currency_ID == VAB_Currencyheader_ID)
                                    {
                                        cashline1.SetAmount(Decimal.Negate(cashline.GetAmount()));
                                        cashline1.SetVAB_Currency_ID(VAB_Currencyheader_ID);
                                        cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                    }
                                    else
                                    {
                                        //Change by Bharat as discussed with Ravikant on 22 March 2017
                                        cashline1.SetVAB_Currency_ID(VAB_Currencyheader_ID);
                                        if (cashline1.Get_ColumnIndex("VAB_CurrencyType_ID") > 0)
                                        {
                                            //_Curencyrate = MConversionRate.GetRate(VAB_Currencyheader_ID, Currency_ID, cashheader.GetDateAcct(), cashline.GetVAB_CurrencyType_ID(),
                                            //    cashheader.GetVAF_Client_ID(), cashheader.GetVAF_Org_ID());
                                            convertedamount = Decimal.Negate(Util.GetValueOfDecimal(cashline.GetConvertedAmt()));

                                            // if converted amount not found then get amount based on currency conversion avaliable
                                            if (cashline.GetAmount() != 0 && convertedamount == 0)
                                            {
                                                convertedamount = MVABExchangeRate.Convert(GetCtx(), Decimal.Negate(cashline.GetAmount()), Currency_ID, VAB_Currencyheader_ID,
                                                    cash1.GetDateAcct(), cashline.GetVAB_CurrencyType_ID(), cash1.GetVAF_Client_ID(), cash1.GetVAF_Org_ID());
                                                cashline.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(convertedamount)));
                                            }

                                            if (cashline.GetAmount() != 0 && convertedamount == 0)
                                            {
                                                return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                            }
                                            else if (cashline.GetAmount() == 0)
                                            {
                                                return "Amount Should be greater than zero";
                                            }
                                            cashline1.SetAmount(convertedamount);
                                            cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                        }
                                        else
                                        {
                                            sql = "select multiplyrate from VAB_ExchangeRate where isactive='Y' and VAB_Currency_id=" + VAB_Currencyheader_ID + " and VAB_Currency_to_id=" + Currency_ID;
                                            _Curencyrate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));

                                            if (_Curencyrate == 0)
                                            {
                                                return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                            }
                                            else
                                            {
                                                convertedamount = Decimal.Round(Decimal.Multiply(_Curencyrate, Decimal.Negate(cashline.GetAmount())), 2);
                                            }
                                            cashline1.SetConvertedAmt(Util.GetValueOfString(convertedamount));
                                            cashline1.SetAmount(convertedamount);
                                        }

                                    }

                                    cashline1.SetVAB_Charge_ID(cashline.GetVAB_Charge_ID());
                                    if (cashline.GetCashType().ToString() == "A")
                                    {
                                        cashline1.SetVSS_PAYMENTTYPE("R");
                                    }
                                    if (cashline.GetCashType().ToString() == "F")
                                    {
                                        cashline1.SetVSS_PAYMENTTYPE("P");
                                    }
                                    cashline1.SetVAB_CashJRNLLine_ID_1(cashline.GetVAB_CashJRNLLine_ID());
                                    if (!cashline1.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        log.Severe("NotSaved");
                                        return " NotSaved Cashline1 [ Header not Found ]  ";
                                    }
                                    if (cashline.GetCashType().ToString() == "F")
                                    {
                                        cashline.SetVAB_CashJRNLLine_ID_1(cashline1.GetVAB_CashJRNLLine_ID());
                                    }
                                    if (!cashline.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        log.Severe("NotSaved");
                                        return " NotSaved Cashline [ Header not Found ]  ";
                                    }
                                    //change by Amit 1-june-2016 after discussion with ravikant

                                    //int BeginBal = Util.GetValueOfInt(DB.ExecuteScalar("select completedbalance from VAB_CashBook where VAB_CashBook_id=" + cash1.GetVAB_CashBook_ID(), null, Get_Trx()));
                                    //cash1.SetBeginningBalance(BeginBal);
                                    //if (!cash1.Save())
                                    //{
                                    //    Rollback();
                                    //    log.Severe("NotSaved");
                                    //    return " NotSaved Cash1 [ Header not Found(bb) ]  ";
                                    //}

                                    //end
                                }

                            }
                            else
                            {
                                //dscashbook.Dispose();
                            }
                        }
                        else
                        {
                            cashtype++;



                        }
                    }
                    // Discussed with Ravikant it is re updating the Header so commented on 23-March-2017 by Bharat
                    //decimal OpenBal = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT completedbalance FROM  VAB_CashBook WHERE VAB_CashBook_id=" + cashheader.GetVAB_CashBook_ID() + "", null, Get_Trx()));
                    //cashheader.SetBeginningBalance(OpenBal);
                    //if (!cashheader.Save())
                    //{
                    //    Rollback();
                    //    log.Severe("Not Saved");
                    //}
                }
                else
                {
                    dscashline.Dispose();
                    cashheader.SetGenerateCashBookTransfer("Y");
                    if (!cashheader.Save())
                    {
                        Rollback();
                        log.Severe("NotSaved");
                    }
                    return Msg.GetMsg(GetCtx(), "NoCashLineExist");
                }
                if (_cashIds.Count > 0)
                {
                    for (int k = 0; k < _cashIds.Count; k++)
                    {
                        cash = new MVABCashJRNL(GetCtx(), _cashIds[k], Get_Trx());
                        cash.SetDocStatus(cash.CompleteIt());
                        if (!cash.Save())
                        {
                            Rollback();
                            log.Severe("NotSaved");
                        }
                    }

                }
            }

            cashheader.SetGenerateCashBookTransfer("Y");
            if (!cashheader.Save())
            {
                log.Severe("NotSaved");
            }
            if (cashtype == i)
            {
                return Msg.GetMsg(GetCtx(), "NoCashLineExist");

            }
            return "Completed";

        }

        protected override void Prepare()
        {
            VAB_CashJRNL_ID = GetRecord_ID();

        }
    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : BPGroupAcctCopy
 * Purpose        : Copy BP Group default Accounts
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           19-Dec-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BPGroupAcctCopy:ProcessEngine.SvrProcess
    {
           //BP Group					
            private int _VAB_BPart_Category_ID = 0;
          //Acct Schema					
            private int _VAB_AccountBook_ID = 0;

            /// <summary>
            /// Prepare - e.g., get Parameters.
            /// </summary>
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
                    else if (name.Equals("VAB_BPart_Category_ID"))
                    {
                        _VAB_BPart_Category_ID = para[i].GetParameterAsInt();
                    }
                    else if (name.Equals("VAB_AccountBook_ID"))
                    {
                        _VAB_AccountBook_ID = para[i].GetParameterAsInt();
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                    }
                }
            }	//	prepare


            /// <summary>
            /// Process
            /// </summary>
            /// <returns>message</returns>
            protected override String DoIt()
            {
                log.Info("VAB_AccountBook_ID=" + _VAB_AccountBook_ID);
                if (_VAB_AccountBook_ID == 0)
                {
                    throw new SystemException("VAB_AccountBook_ID=0");
                }
                MVABAccountBook ass = MVABAccountBook.Get(GetCtx(), _VAB_AccountBook_ID);
                if (ass.Get_ID() == 0)
                {
                    throw new SystemException("Not Found - VAB_AccountBook_ID=" + _VAB_AccountBook_ID);
                }
                //
                String sql = null;
                int _updated = 0;
                int _created = 0;
                int _updatedTotal = 0;
                int _createdTotal = 0;

                //	Update existing Customers
                sql = "UPDATE C_BP_Customer_Acct ca "
                    + "SET (C_Receivable_Acct,C_Receivable_Services_Acct,C_PrePayment_Acct)="
                     + " (SELECT C_Receivable_Acct,C_Receivable_Services_Acct,C_PrePayment_Acct "
                     + " FROM VAB_BPart_Category_Acct"
                     + " WHERE VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID
                     + " AND VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                    + "), Updated=SysDate, UpdatedBy=0 "
                    + "WHERE ca.VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                    + " AND EXISTS (SELECT * FROM VAB_BusinessPartner p "
                        + "WHERE p.VAB_BusinessPartner_ID=ca.VAB_BusinessPartner_ID"
                        + " AND p.VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ")";
                _updated = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_updated), "@Updated@ @VAB_BusinessPartner_ID@ @IsCustomer@");
                _updatedTotal += _updated;

                //	Insert new Customer
                sql = "INSERT INTO C_BP_Customer_Acct "
                    + "(VAB_BusinessPartner_ID, VAB_AccountBook_ID,"
                    + " VAF_Client_ID, VAF_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " C_Receivable_Acct, C_Receivable_Services_Acct, C_PrePayment_Acct) "
                    + "SELECT p.VAB_BusinessPartner_ID, acct.VAB_AccountBook_ID,"
                    + " p.VAF_Client_ID, p.VAF_Org_ID, 'Y', SysDate, 0, SysDate, 0,"
                    + " acct.C_Receivable_Acct, acct.C_Receivable_Services_Acct, acct.C_PrePayment_Acct "
                    + "FROM VAB_BusinessPartner p"
                    + " INNER JOIN VAB_BPart_Category_Acct acct ON (acct.VAB_BPart_Category_ID=p.VAB_BPart_Category_ID)"
                    + "WHERE acct.VAB_AccountBook_ID=" + _VAB_AccountBook_ID			//	#
                    + " AND p.VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID
                    + " AND NOT EXISTS (SELECT * FROM C_BP_Customer_Acct ca "
                        + "WHERE ca.VAB_BusinessPartner_ID=p.VAB_BusinessPartner_ID"
                        + " AND ca.VAB_AccountBook_ID=acct.VAB_AccountBook_ID)";
                _created = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_created), "@Created@ @VAB_BusinessPartner_ID@ @IsCustomer@");
                _createdTotal += _created;


                //	Update existing Vendors
                sql = "UPDATE C_BP_Vendor_Acct va "
                    + "SET (V_Liability_Acct,V_Liability_Services_Acct,V_PrePayment_Acct)="
                     + " (SELECT V_Liability_Acct,V_Liability_Services_Acct,V_PrePayment_Acct "
                     + " FROM VAB_BPart_Category_Acct"
                     + " WHERE VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID
                     + " AND VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                    + "), Updated=SysDate, UpdatedBy=0 "
                    + "WHERE va.VAB_AccountBook_ID=" + _VAB_AccountBook_ID
                    + " AND EXISTS (SELECT * FROM VAB_BusinessPartner p "
                        + "WHERE p.VAB_BusinessPartner_ID=va.VAB_BusinessPartner_ID"
                        + " AND p.VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ")";
                _updated = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_updated), "@Updated@ @VAB_BusinessPartner_ID@ @IsVendor@");
                _updatedTotal += _updated;

                //	Insert new Vendors
                sql = "INSERT INTO C_BP_Vendor_Acct "
                    + "(VAB_BusinessPartner_ID, VAB_AccountBook_ID,"
                    + " VAF_Client_ID, VAF_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " V_Liability_Acct, V_Liability_Services_Acct, V_PrePayment_Acct) "
                    + "SELECT p.VAB_BusinessPartner_ID, acct.VAB_AccountBook_ID,"
                    + " p.VAF_Client_ID, p.VAF_Org_ID, 'Y', SysDate, 0, SysDate, 0,"
                    + " acct.V_Liability_Acct, acct.V_Liability_Services_Acct, acct.V_PrePayment_Acct "
                    + "FROM VAB_BusinessPartner p"
                    + " INNER JOIN VAB_BPart_Category_Acct acct ON (acct.VAB_BPart_Category_ID=p.VAB_BPart_Category_ID)"
                    + "WHERE acct.VAB_AccountBook_ID=" + _VAB_AccountBook_ID			//	#
                    + " AND p.VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID
                    + " AND NOT EXISTS (SELECT * FROM C_BP_Vendor_Acct va "
                        + "WHERE va.VAB_BusinessPartner_ID=p.VAB_BusinessPartner_ID AND va.VAB_AccountBook_ID=acct.VAB_AccountBook_ID)";
                _created = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_created), "@Created@ @VAB_BusinessPartner_ID@ @IsVendor@");
                _createdTotal += _created;

                return "@Created@=" + _createdTotal + ", @Updated@=" + _updatedTotal;
            }	//	doIt

        }	
}

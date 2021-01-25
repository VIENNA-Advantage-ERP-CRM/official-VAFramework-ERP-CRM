using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;
using java.io;
using System.Data.SqlClient;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPaySelection : X_C_PaySelection
    {
        /**
         * 	Default Constructor
         *	@param ctx context
         *	@param C_PaySelection_ID id
         *	@param trxName transaction
         */
        public MPaySelection(Ctx ctx, int C_PaySelection_ID, Trx trxName):
            base(ctx, C_PaySelection_ID, trxName)
        {
            
            if (C_PaySelection_ID == 0)
            {
                //	SetVAB_Bank_Acct_ID (0);
                //	SetName (null);	// @#Date@
                //	SetPayDate (new Timestamp(System.currentTimeMillis()));	// @#Date@
                SetTotalAmt(Env.ZERO);
                SetIsApproved(false);
                SetProcessed(false);
                SetProcessing(false);
            }
        }	//	MPaySelection

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName transaction
         */
        public MPaySelection(Ctx ctx, DataRow dr, Trx trxName):
            base(ctx, dr, trxName)
        {
            
        }	//	MPaySelection

        /**	Lines						*/
        private MPaySelectionLine[] _lines = null;
        /**	Currency of Bank Account	*/
        private int _VAB_Currency_ID = 0;

        /**
         * 	Get Lines
         *	@param requery requery
         *	@return lines
         */
        public MPaySelectionLine[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MPaySelectionLine> list = new List<MPaySelectionLine>();
            String sql = "SELECT * FROM C_PaySelectionLine WHERE C_PaySelection_ID=" + GetC_PaySelection_ID() + " ORDER BY Line";
            DataTable dt;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MPaySelectionLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "GetLines", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            
            //
            _lines = new MPaySelectionLine[list.Count];
            _lines=list.ToArray();
            return _lines;
        }	//	GetLines

        /**
         * 	Get Currency of Bank Account
         *	@return VAB_Currency_ID
         */
        public int GetVAB_Currency_ID()
        {
            if (_VAB_Currency_ID == 0)
            {
                String sql = "SELECT VAB_Currency_ID FROM VAB_Bank_Acct "
                    + "WHERE VAB_Bank_Acct_ID=@param1";
                _VAB_Currency_ID = DataBase.DB.GetSQLValue(null, sql, GetVAB_Bank_Acct_ID());
            }
            return _VAB_Currency_ID;
        }	//	GetVAB_Currency_ID


        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaySelection[");
            sb.Append(Get_ID()).Append(",").Append(GetName())
                .Append("]");
            return sb.ToString();
        }	//	toString

    }
}

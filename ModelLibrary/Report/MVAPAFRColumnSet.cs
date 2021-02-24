/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAPAFRColumnSet
 * Purpose        : Report Column Set Model
 * Class Used     : X_VAPA_FR_ColumnSet
 * Chronological    Development
 * Deepak           18-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Report
{
    public class MVAPAFRColumnSet : X_VAPA_FR_ColumnSet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FR_ColumnSet_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAPAFRColumnSet(Ctx ctx, int VAPA_FR_ColumnSet_ID, Trx trxName)
            : base(ctx, VAPA_FR_ColumnSet_ID, trxName)
        {

            if (VAPA_FR_ColumnSet_ID == 0)
            {
            }
            else
            {
                LoadColumns();
            }
        }	//	MVAPAFRColumnSet

        /** Contained Columns		*/
        private MVAPAFRColumn[] _columns = null;

        /// <summary>
        /// Load contained columns
        /// </summary>
        private void LoadColumns()
        {
            List<MVAPAFRColumn> list = new List<MVAPAFRColumn>();
            String sql = "SELECT * FROM VAPA_FR_ColumnSet WHERE VAPA_FR_ColumnSet_ID=@param AND IsActive='Y' ORDER BY SeqNo";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                param[0] = new SqlParameter("@param", GetVAPA_FR_ColumnSet_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAPAFRColumn(GetCtx(), dr, null));
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
            }
            //
            _columns = new MVAPAFRColumn[list.Count];
            _columns = list.ToArray();
            log.Finest("ID=" + GetVAPA_FR_ColumnSet_ID()
                + " - Size=" + list.Count);
        }	//	loadColumns

        /// <summary>
        /// Get Columns
        /// </summary>
        /// <returns>columns</returns>
        public MVAPAFRColumn[] GetColumns()
        {
            return _columns;
        }	//	getColumns

        /// <summary>
        ///	List Info
        /// </summary>
        public void List()
        {
            //System.out.println(toString());
            System.Console.WriteLine(ToString());
            if (_columns == null)
            {
                return;
            }
            for (int i = 0; i < _columns.Length; i++)
            {
                //System.out.println("- " + _columns[i].toString());
                System.Console.WriteLine("- " + _columns[i].ToString());
            }
        }	//	list

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAPAFRColumnSet[")
                .Append(Get_ID()).Append(" - ").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }	//	MVAPAFRColumnSet

}

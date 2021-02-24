/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReportLineSet
 * Purpose        : Report Line Set Model
 * Class Used     : X_VAPA_FR_RowSet
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
    public class MVAPAFRRowSet : X_VAPA_FR_RowSet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FR_RowSet_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAPAFRRowSet(Ctx ctx, int VAPA_FR_RowSet_ID, Trx trxName):base(ctx, VAPA_FR_RowSet_ID, trxName)
        {
            
            if (VAPA_FR_RowSet_ID == 0)
            {
            }
            else
                LoadLines();
        }	//	MReportLineSet

        /**	Contained Lines			*/
        private MVAPARFRow[] _lines = null;

        /// <summary>
        ///	Load Lines
        /// </summary>
        private void LoadLines()
        {
            List<MVAPARFRow> list = new List<MVAPARFRow>();
            String sql = "SELECT * FROM VAPA_FR_Row "
                + "WHERE VAPA_FR_RowSet_ID=@param AND IsActive='Y' "
                + "ORDER BY SeqNo";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, get_TrxName());
                //pstmt.setInt(1, getVAPA_FR_RowSet_ID());
                param[0] = new SqlParameter("@param", GetVAPA_FR_RowSet_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAPARFRow(GetCtx(), dr, Get_TrxName()));
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
            _lines = new MVAPARFRow[list.Count];
            _lines=list.ToArray();
            log.Finest("ID=" + GetVAPA_FR_RowSet_ID()
                + " - Size=" + list.Count);
        }	//	loadColumns

        /// <summary>
        /// Get Liness
        /// </summary>
        /// <returns>array of lines</returns>
        public MVAPARFRow[] GetLiness()
        {
            return _lines;
        }	//	getLines

        /// <summary>
        ///	List Info
        /// </summary>
        public void List()
	{
		//System.out.println(toString());
        System.Console.WriteLine(ToString());
        if (_lines == null)
        {
            return;
        }
        for (int i = 0; i < _lines.Length; i++)
        {
            _lines[i].List();
        } 
	}	//	list

        /// <summary>
        ///	String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MReportLineSet[")
                .Append(Get_ID()).Append(" - ").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }	//	MReportLineSet

}

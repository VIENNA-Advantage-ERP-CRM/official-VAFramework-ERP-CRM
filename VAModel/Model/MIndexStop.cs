/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MIndexStop
 * Purpose        : Text Search Stop Keyword Model
 * Class Used     : X_K_IndexStop
 * Chronological    Development
 * Deepak           12-Feb-2010
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

namespace VAdvantage.Model
{
    public class MIndexStop : X_K_IndexStop
    {
        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="K_IndexStop_ID">id</param>
        /// <param name="trxName">trx</param>
        public MIndexStop(Ctx ctx, int K_IndexStop_ID, Trx trxName)
            : base(ctx, K_IndexStop_ID, trxName)
        {

        }

        /// <summary>
        /// 	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MIndexStop(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MIndexStop(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }
        /// <summary>
        /// Set Keyword & standardize
        /// </summary>
        /// <param name="Keyword">key</param>
        public new void SetKeyword(String Keyword)
        {
            String kw = MIndex.StandardizeKeyword(Keyword);
            if (kw == null)
            {
                kw = "?";
            }
            base.SetKeyword(kw);
        }	//	setKeyword
        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord || Is_ValueChanged("Keyword"))
                SetKeyword(GetKeyword());
            if (GetKeyword().Equals("?"))
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Keyword"));
                return false;
            }
            return true;
        }	//	beforeSave

    }	//	MIndexStop

}

using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Data;
using VAdvantage.DataBase;
using VIS.Areas.VIS.Models;
using System.Collections.Generic;
using VAdvantage.Utility;

namespace ViennaAdvantage.Tool.Model
{
    public class GenerateXModelModel
    {
        /// <summary>
        /// Data set for combo box
        /// </summary>
        /// <returns></returns>
        public DataSet GetTable()
        {
            string strQuery = "select Name, VAF_TABLEVIEW_ID,TableName from VAF_TABLEVIEW order by name";
            //execute
            return DB.ExecuteDataset(strQuery, null);
        }
        /// <summary>
        /// Dataset to bind checked list box
        /// </summary>
        /// <returns></returns>
        public DataSet GetEntity()
        {
            string strQuery = "select VAF_RecrodType_id, entitytype, name from VAF_RecrodType";
            //execute
            return DB.ExecuteDataset(strQuery, null);
        }

        /// <summary>
        /// Get Table details
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public List<GenXModelGetTableClass> GenXModelGetTable(Ctx ctx)
        {
            List<GenXModelGetTableClass> obj = new List<GenXModelGetTableClass>();

            string strQuery = "select Name, VAF_TABLEVIEW_ID,TableName from VAF_TABLEVIEW order by name";
            DataSet ds = DB.ExecuteDataset(strQuery, null);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GenXModelGetTableClass tb = new GenXModelGetTableClass();
                    tb.VAF_TableView_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TABLEVIEW_ID"]);
                    tb.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj.Add(tb);
                }
            }

            return obj;
        }

    }

    public class GenXModelGetTableClass
    {
        public int VAF_TableView_ID { get; set; }
        public string Name { get; set; }
    }


}

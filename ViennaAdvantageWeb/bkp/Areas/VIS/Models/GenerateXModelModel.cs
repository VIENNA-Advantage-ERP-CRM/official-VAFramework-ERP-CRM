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
            string strQuery = "select Name, AD_TABLE_ID,TableName from AD_TABLE order by name";
            //execute
            return DB.ExecuteDataset(strQuery, null);
        }
        /// <summary>
        /// Dataset to bind checked list box
        /// </summary>
        /// <returns></returns>
        public DataSet GetEntity()
        {
            string strQuery = "select ad_entitytype_id, entitytype, name from ad_entitytype";
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

            string strQuery = "select Name, AD_TABLE_ID,TableName from AD_TABLE order by name";
            DataSet ds = DB.ExecuteDataset(strQuery, null);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GenXModelGetTableClass tb = new GenXModelGetTableClass();
                    tb.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_TABLE_ID"]);
                    tb.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    obj.Add(tb);
                }
            }

            return obj;
        }

    }

    public class GenXModelGetTableClass
    {
        public int AD_Table_ID { get; set; }
        public string Name { get; set; }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdvantage.Utility
{
    public class QueryParams
    {
        public String sql = "";
        public List<Object> parameters;

        /// <summary>
        /// 
        /// </summary>
        public QueryParams()
        {
            parameters = new List<Object>();
        }

        /// <summary>
        /// </summary>
        /// <param name="sql">Sql</param>
        /// <param name="parameters">Param</param>
        public QueryParams(String sql, Object[] localparam)
        {
            this.sql = sql;
            //this.params = Arrays.asList(params);
            this.parameters = localparam.ToList();
            //this.parameters.AddRange(localparam.ToList());
        }
    }
}

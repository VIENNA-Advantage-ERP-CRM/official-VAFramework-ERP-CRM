using System;

using System.Data;
using System.Data.SqlClient;

using VAdvantage.Logging;

namespace VAdvantage.Classes
{
    /****************************************************************************?
    * 
    *     VLookUpInfo class
    *  Info Class for Lookup SQL (ValueObject)
    * 
   *****************************************************************************/
    public class VLookUpInfo
    {
        #region "Declaration"

        /** SQL Query       */
        public string query = null;

        public string queryAll = null;

        /** Table Name      */
        public string tableName = "";
        /** Key Column      */
        public string keyColumn = "";
        /** Zoom Window     */
        public int zoomWindow;
        /** Zoom Window     */
        public int zoomWindowPO;
        /** Zoom Query      */
        public IQuery zoomQuery = null;

        /** Direct Access Query 	*/
        public string queryDirect = "";
        /** Parent Flag     */
        public bool isParent = false;
        /** Key Flag     	*/
        public bool isKey = false;
        /** Validation code */
        public string validationCode = "";
        /** Validation flag */
        public bool isValidated = false;

        /**	AD_Column_Info or AD_Process_Para	*/
        public int column_ID;
        /** Real AD_Reference_ID				*/
        public int AD_Reference_Value_ID;
        /** CreadedBy?updatedBy					*/
        public bool isCreadedUpdatedBy = false;

        /* Display Column SQL Query*/
        public string displayColSubQ = null;

        /// <summary>
        /// If image is marked as identifier, then true otherwise false
        /// </summary>
        public bool hasImageIdentifier = false;

        #endregion

        /// <summary>
        /// Construtor
        /// </summary>
        public VLookUpInfo()
        {
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="qry">sql qry text</param>
        /// <param name="tablename">table name</param>
        /// <param name="keyColName">key col name</param>
        /// <param name="zoomWindowId">zoom win id</param>
        /// <param name="zoomWindowPOId">zoom windoe po id</param>
        /// <param name="zoomqry">zoom sql qury text</param>
        public VLookUpInfo(string qry, string tablename, string keyColName, int zoomWindowId, int zoomWindowPOId, IQuery zoomqry)
        {
            if (qry == null)
                throw new System.Exception("SqlQuery is null");
            query = qry;
            if (keyColName == null)
                throw new System.Exception("keyColumn is null");
            tableName = tablename;
            keyColumn = keyColName;
            zoomWindow = zoomWindowId;
            zoomWindowPO = zoomWindowPOId;
            zoomQuery = zoomqry;
        }

        /// <summary>
        /// Clone class object
        /// </summary>
        /// <returns></returns>
        public VLookUpInfo Clone()
        {
            VLookUpInfo clone = new VLookUpInfo(this.query, this.tableName, this.keyColumn, this.zoomWindow, this.zoomWindowPO, this.zoomQuery);
            return clone;
        }

        /// <summary>
        /// Get first AD_Reference_ID of a matching Reference Name.
        /// Can have SQL LIKE place holders.
        /// (This is more a development tool than used for production)
        /// </summary>
        /// <param name="ReferenceName">reference name</param>
        /// <returns>AD_Reference_ID</returns>
        public static int GetAD_Reference_ID(String ReferenceName)
        {
            int RetValue = 0;
            String sql = "SELECT AD_Reference_ID,Name,ValidationType,IsActive "
                + "FROM AD_Reference WHERE Name LIKE @name";

            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@name", ReferenceName);
                dr = DataBase.DB.ExecuteReader(sql, param);
                //
                int i = 0;
                int id = 0;
                String RefName = "";
                String ValidationType = "";
                bool IsActive = false;
                while (dr.Read())
                {
                    id = Utility.Util.GetValueOfInt(dr[0].ToString());
                    if (i == 0)
                        RetValue = id;
                    RefName = dr[1].ToString();
                    ValidationType = dr[2].ToString();
                    IsActive = dr[3].ToString().Equals("Y");
                    VLogger.Get().Config("AD_Reference Name=" + RefName + ", ID=" + id + ", Type=" + ValidationType + ", Active=" + IsActive);
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, sql, e);
            }
            return RetValue;
        }


        /// <summary>
        ///  Get first AD_Column_ID of matching ColumnName.
        ///  Can have SQL LIKE place holders.
        ///  (This is more a development tool than used for production)
        /// </summary>
        /// <param name="ColumnName">column name</param>
        /// <returns>AD_Column_ID</returns>
        public static int GetAD_Column_ID(String columnName)
        {
            int RetValue = 0;
            String sql = "SELECT c.AD_Column_ID,c.ColumnName,t.tableName "
                + "FROM AD_Column c, AD_Table t "
                + "WHERE c.ColumnName LIKE @name AND c.AD_Table_ID=t.AD_Table_ID";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@name", columnName);
                dr = DataBase.DB.ExecuteReader(sql, param);
                //
                int i = 0;
                int id = 0;
                String colName = "";
                String tabName = "";
                while (dr.Read())
                {
                    id = Utility.Util.GetValueOfInt(dr[0].ToString());
                    if (i == 0)
                        RetValue = id;
                    colName = dr[1].ToString();
                    tabName = dr[2].ToString();
                    VLogger.Get().Config("Name=" + colName + ", ID=" + id + ", Table=" + tabName);
                }
                dr.Close();
                dr = null;
                param = null;
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                VLogger.Get().Log(Level.SEVERE, sql, e);
            }
            return RetValue;
        }
    }
}

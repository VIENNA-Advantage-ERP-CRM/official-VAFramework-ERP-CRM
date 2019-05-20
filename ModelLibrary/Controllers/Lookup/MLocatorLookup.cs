/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLocatorLookup
 * Purpose        : Warehouse Locator Lookup model.
 * Chronological    Development
 * Harwinder      :  30-Jun-2009
 * RaguNandan     : 26-june 
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Threading;
using System.Globalization;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    [Serializable]
    public class MLocatorLookup : Lookup
    {
        #region Variables
        protected int C_Locator_ID;
        private Thread _loader;
        //	Only Warehouse	
        private int _only_Warehouse_ID = 0;
        //	Only Product				
        private int _only_Product_ID = 0;
        //	Only outgoing Trx			
        private bool? _only_Outgoing = null;
        // Storage of data  MLookups	
        //volatile Map<int, KeyNamePair> _lookup = new LinkedHashMap<int, KeyNamePair>();
        volatile Dictionary<int, KeyNamePair> _lookup = new Dictionary<int, KeyNamePair>();
        // Max Locators per Lookup		
        private static int _maxRows = 10000;	//	how many rows to read

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        public MLocatorLookup(Ctx ctx, int windowNo)
            : base(ctx, windowNo, DisplayType.TableDir)
        {
            //_loader = new Thread(new ThreadStart(Load));
            //_loader.IsBackground = true;
           
            //_loader.Start();
           // Load();
        }

        //public MLocatorLookup Initialize()
        //{
        //    _loader = new Thread(new ThreadStart(Load));
        //    _loader.IsBackground = true;
        
       
        //    _loader.Start();
        //    //Load();
        //    return this;
        //}

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            if (_loader != null)
            {
                while (_loader.IsAlive)
                {
                    _loader.Interrupt();
                }
            }
            _loader = null;
            if (_lookup != null)
                _lookup.Clear();
            _lookup = null;
            //
            base.Dispose();
        }

        /// <summary>
        ///Set Warehouse restriction
        /// </summary>
        /// <param name="only_Warehouse_ID"></param>
        public void SetOnly_Warehouse_ID(int only_Warehouse_ID)
        {
            _only_Warehouse_ID = only_Warehouse_ID;
        }

        /// <summary>
        ///Get Only Wahrehouse
        /// </summary>
        /// <returns></returns>
        public int GetOnly_Warehouse_ID()
        {
            return _only_Warehouse_ID;
        }

        /// <summary>
        /// Set Product restriction
        /// </summary>
        /// <param name="only_Product_ID"></param>
        public void SetOnly_Product_ID(int only_Product_ID)
        {
            _only_Product_ID = only_Product_ID;
        }

        /// <summary>
        ///Get Only Product
        /// </summary>
        /// <returns></returns>
        public int GetOnly_Product_ID()
        {
            return _only_Product_ID;
        }

        /// <summary>
        /// Set Only Outgoing Trx
        /// </summary>
        /// <param name="isOutgoing"></param>
        public void SetOnly_Outgoing(Boolean isOutgoing)
        {
            _only_Outgoing = isOutgoing;
        }

        /// <summary>
        ///Get Outgoing Trx
        /// </summary>
        /// <returns></returns>
        public bool IsOnly_Outgoing()
        {
            return (_only_Outgoing !=null && _only_Outgoing.Value != false);
        }

        /// <summary>
        /// Wait until async Load Complete
        /// </summary>
        public void LoadComplete()
        {
            if (_loader != null)
            {
                try
                {
                    _loader.Join();
                }
                catch (Exception ie)
                {
                    log.Log(Level.SEVERE, "Join interrupted", ie);
                }
            }
        }

        /// <summary>
        ///	Get value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override NamePair Get(Object key)
        {
            if (key == null || key == DBNull.Value)
                return null;
            int key1 = int.Parse(key.ToString());
            //	try cache
            KeyNamePair pp;

            _lookup.TryGetValue(key1, out pp);
            if (pp != null)
                return (NamePair)pp;

            //	Not found and waiting for loader
            if (_loader.IsAlive)
            {
                log.Fine("Waiting for Loader");
                LoadComplete();
                //	is most current
                _lookup.TryGetValue(key1, out pp);
            }
            if (pp != null)
                return (NamePair)pp;

            //	Try to get it directly
            return GetDirect(key, true, null);
        }

        /// <summary>
        /// Get Display value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override String GetDisplay(Object value)
        {
            if (value == null || value == DBNull.Value)
                return "";
            //
            NamePair display = Get(value);
            if (display == null)
                return "<" + value.ToString() + ">";
            return display.ToString();
        }

        /// <summary>
        /// The Lookup contains the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  bool ContainsKey(int key)
        {
            return _lookup.ContainsKey(key);
        }

        /// <summary>
        /// Get Data Direct from Table
        /// </summary>
        /// <param name="keyValue">integer key value</param>
        /// <param name="saveInCache">saveInCache save in cache</param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public NamePair GetDirect(Object keyValue, bool saveInCache, Trx trxName)
        {
            MLocator loc = GetMLocator(keyValue, trxName);
            if (loc == null)
                return null;
            //
            int key = loc.GetM_Locator_ID();
            KeyNamePair retValue = new KeyNamePair(key, loc.ToString());
            if (saveInCache)
                _lookup.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        ///Get Data Direct from Table
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public MLocator GetMLocator(Object keyValue, Trx trxName)
        {
            //	log.fine( "MLocatorLookup.getDirect " + keyValue.getClass() + "=" + keyValue);
            int M_Locator_ID = -1;
            try
            {
                M_Locator_ID = int.Parse(keyValue.ToString());
            }
            catch 
            { }
            if (M_Locator_ID == -1)
            {
                log.Log(Level.SEVERE, "Invalid key=" + keyValue);
                return null;
            }
            return new MLocator((Context)GetCtx(), M_Locator_ID, trxName);
        }

        /// <summary>
        ///  a string representation of the object.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "MLocatorLookup[Size=" + _lookup.Count + "]";
        }

        /// <summary>
        ///Is Locator with key valid (Warehouse)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsValid(Object key)
        {
            if (key == null)
                return true;
            //	try cache

            KeyNamePair pp ;
            _lookup.TryGetValue(Utility.Util.GetValueOfInt(key), out pp);
            //_lookup.TryGetValue(int.Parse(key.ToString()),out pp);
            return pp != null;
        }

     /// <summary>
     ///Load Lookup
     /// </summary>
        private void Load()
        {
            //	log.config("MLocatorLookup Loader.run " + m_AD_Column_ID);
            //	Set Info	- see VLocator.actionText

            int only_Warehouse_ID = GetOnly_Warehouse_ID();
            int only_Product_ID = GetOnly_Product_ID();
            bool? only_IsSOTrx = IsOnly_Outgoing();
            //int sqlParaCount = 0;
            StringBuilder sql = new StringBuilder("SELECT * FROM M_Locator ")
                .Append(" WHERE IsActive='Y'");
            if (only_Warehouse_ID != 0)
                sql.Append(" AND M_Warehouse_ID=@w");
            if (only_Product_ID != 0)
            {
                sql.Append(" AND (IsDefault='Y' ");	//	Default Locator
                //	Something already stored
                sql.Append("OR EXISTS (SELECT * FROM M_Storage s ")	//	Storage Locator
                    .Append("WHERE s.M_Locator_ID=M_Locator.M_Locator_ID AND s.M_Product_ID=@p)");

                if (only_IsSOTrx == null || !only_IsSOTrx.Value)
                {
                    //	Default Product
                    sql.Append("OR EXISTS (SELECT * FROM M_Product p ")	//	Default Product Locator
                    .Append("WHERE p.M_Locator_ID=M_Locator.M_Locator_ID AND p.M_Product_ID=@p)");
                    //	Product Locators
                    sql.Append("OR EXISTS (SELECT * FROM M_ProductLocator pl ")	//	Product Locator
                    .Append("WHERE pl.M_Locator_ID=M_Locator.M_Locator_ID AND pl.M_Product_ID=@p)");
                    // No locators defined for the warehouse
                    sql.Append("OR 0 = (SELECT COUNT(*) ");
                    sql.Append("FROM M_ProductLocator pl");
                    sql.Append(" INNER JOIN M_Locator l2 ON (pl.M_Locator_ID=l2.M_Locator_ID) ");
                    sql.Append("WHERE pl.M_Product_ID=@p AND l2.M_Warehouse_ID=M_Locator.M_Warehouse_ID )");
                }
                sql.Append(" ) ");
            }
            String finalSql = MRole.GetDefault((Context)GetCtx(), false).AddAccessSQL(
                sql.ToString(), "M_Locator", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            //if (_loader.ThreadState == ThreadState.Suspended)
            //{
            //    log.log(Level.SEVERE, "Interrupted");
            //    return;
            //}
            //	Reset
            _lookup.Clear();
            int rows = 0;
            try
            {
                List<System.Data.SqlClient.SqlParameter> para = new List<System.Data.SqlClient.SqlParameter>();

               // int index = 1;
                if (only_Warehouse_ID != 0)
                {
                    //	pstmt.setInt(index++, only_Warehouse_ID);
                    para.Add(new System.Data.SqlClient.SqlParameter("@w", only_Warehouse_ID));
                }
                if (only_Product_ID != 0)
                {
                    para.Add(new System.Data.SqlClient.SqlParameter("@p", only_Product_ID));
                   
                }
                DataSet ds = DataBase.DB.ExecuteDataset(finalSql, para.ToArray(),null);
                //
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //	Max out
                    if (rows++ > _maxRows)
                    {
                        log.Warning("Over Max Rows - " + rows);
                        break;
                    }
                    MLocator loc = new MLocator((Context)GetCtx(), dr, null);
                    int M_Locator_ID = loc.GetM_Locator_ID();
                    KeyNamePair pp = new KeyNamePair(M_Locator_ID, loc.ToString());
                    _lookup.Add(M_Locator_ID, pp);
                }
                ds.Dispose();
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, finalSql, e);
                
            }
            log.Fine("Complete #" + _lookup.Count);
            if (_lookup.Count == 0)
            {
                log.Finer(finalSql);
            }
        }

        /// <summary>
        ///  Return info as List containing Locator, waits for the loader to finish
        /// </summary>
        /// <returns></returns>
        public ICollection<KeyNamePair> GetData()
        {
            if (_loader!= null && _loader.IsAlive)
            {
                log.Fine("Waiting for Loader");
                try
                {
                    _loader.Join();
                }
                catch (Exception ie)
                {
                    log.Severe("Join interrupted - " + ie.Message);
                }
            }
            return _lookup.Values;
        }

        /// <summary>
        /// Return data as sorted List
        /// </summary>
        /// <param name="mandatory"></param>
        /// <param name="onlyValidated"></param>
        /// <param name="onlyActive"></param>
        /// <param name="temporary"></param>
        /// <returns></returns>
        public override List<NamePair> GetData(bool mandatory, bool onlyValidated, bool onlyActive, bool temporary)
        {
            //	create list
            //Collection<KeyNamePair> collection = GetData();
            ICollection<KeyNamePair> collection = GetData();
            List<NamePair> list = new List<NamePair>(collection.Count);
            //Iterator<KeyNamePair> it = collection.iterator();
            IEnumerator<KeyNamePair> it = collection.GetEnumerator();
            while (it.MoveNext())
            {
                list.Add(it.Current);
            }

            /**	Sort Data
            MLocator l = new MLocator (m_ctx, 0);
            if (!mandatory)
                list.add (l);
            Collections.sort (list, l);
            **/
            return list;
        }

        /// <summary>
        /// Refresh Values
        /// </summary>
        /// <returns></returns>
        public override int Refresh()
        {
            log.Fine("start");
            //_loader = new Thread(new ThreadStart(Load));
           
       
            //_loader.Start();
            try
            {
                Load();  //_loader.Join();
            }
            catch 
            {
            }
            log.Info("#" + _lookup.Count);
            return _lookup.Count;
        }

        /// <summary>
        ///Get underlying fully qualified Table.Column Name
        /// </summary>
        /// <returns></returns>
        public override String GetColumnName()
        {
            return "M_Locator.M_Locator_ID";
        }
    }
}

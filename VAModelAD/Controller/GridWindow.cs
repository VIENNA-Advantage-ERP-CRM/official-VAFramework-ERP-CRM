using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Controller;
namespace VAdvantage.Model
{
    public class GridWindow
    {

        /** Value Object                */
        public GridWindowVO _vo;
        /**	Tabs						*/
        public List<GridTab> _tabs = new List<GridTab>();
        /** Model last updated			*/
        private DateTime? _modelUpdated = null;

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(GridWindow).FullName);


        /// <summary>
        ///  Get Grid Window
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        /// <param name="VAF_Screen_ID"></param>
        /// <returns></returns>
        public static GridWindow Get(Context ctx, int windowNo, int VAF_Screen_ID)
        {
            log.Config("Window=" + windowNo + ", VAF_Screen_ID=" + VAF_Screen_ID);
            GridWindowVO mWindowVO = GridWindowVO.Create(ctx, windowNo, VAF_Screen_ID);
            if (mWindowVO == null)
                return null;
            return new GridWindow(mWindowVO);
        }	//	get

        /// <summary>
        ///Constructor
        /// </summary>
        /// <param name="vo"></param>
        public GridWindow(GridWindowVO vo)
        {
            _vo = vo;
            if (LoadTabData())
            {
                //enableEvents();
            }
        }	//	MWindow

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
    		log.Info("VAF_Screen_ID=" + _vo.VAF_Screen_ID);
		    for (int i = 0; i < _tabs.Count; i++)
            {
			 _tabs[i].Dispose();
            }
		  _tabs.Clear();
		  _tabs = null;
        }

        /// <summary>
        ///Get Tab data and create MTab(s)
        /// </summary>
        /// <returns></returns>
        private bool LoadTabData()
        {
            log.Config("");

            if (_vo.GetTabs() == null)
                return false;

            for (int t = 0; t < _vo.GetTabs().Count; t++)
            {
                GridTabVO mTabVO = _vo.GetTabs()[t];
                if (mTabVO != null)
                {
                    int onlyCurrentDays = 0;
                    if (t == 0 && IsTransaction())
                        onlyCurrentDays = 1;
                    GridTab mTab = new GridTab(mTabVO, onlyCurrentDays,_vo);
                    mTabVO.GetCtx().SetContext(mTabVO.windowNo, mTabVO.tabNo,
                        "KeyColumnName", mTab.GetKeyColumnName());
                    //	Set Link Column
                    if (mTab.GetLinkColumnName().Length == 0)
                    {
                        List<String> parents = mTab.GetParentColumnNames();
                        //	No Parent - no link
                        if (parents.Count == 0)
                        {
                            ;
                        }
                        //	Standard case
                        else if (parents.Count == 1)
                            mTab.SetLinkColumnName(parents[0]);
                        else
                        {
                            //	More than one parent.
                            //	Search prior tabs for the "right parent"
                            //	for all previous tabs
                            for (int i = 0; i < _tabs.Count; i++)
                            {
                                //	we have a tab
                                GridTab tab = _tabs[i];
                                String tabKey = tab.GetKeyColumnName();		//	may be ""
                                //	look, if one of our parents is the key of that tab
                                for (int j = 0; j < parents.Count; j++)
                                {
                                    String parent = parents[j];
                                    if (parent.Equals(tabKey))
                                    {
                                        mTab.SetLinkColumnName(parent);
                                        break;
                                    }
                                    //	The tab could have more than one key, look into their parents
                                    if (tabKey.Equals(""))
                                        for (int k = 0; k < tab.GetParentColumnNames().Count; k++)
                                            if (parent.Equals(tab.GetParentColumnNames()[k]))
                                            {
                                                mTab.SetLinkColumnName(parent);
                                                break;
                                            }
                                }	//	for all parents
                            }	//	for all previous tabs
                        }	//	parents.size > 1
                    }	//	set Link column
                    mTab.SetLinkColumnName(null);	//	overwrites, if VAF_Column_ID exists
                    //
                    _tabs.Add(mTab);
                }
            }	//  for all tabs
            LogAccess();
            return true;
        }	//	loadTabData

        /// <summary>
        /// log Access
        /// </summary>
        private void LogAccess()
        {
            MSession session = MSession.Get(_vo.GetCtx(), true);
            session.WindowLog(_vo.GetCtx().GetVAF_Client_ID(), _vo.GetCtx().GetVAF_Org_ID(),
                GetVAF_Screen_ID(), 0);
        }	//

       /// <summary>
       ///	Is Transaction Window
       /// </summary>
       /// <returns></returns>
        public bool IsTransaction()
        {
            return _vo.WindowType.Equals(GridWindowVO.WINDOWTYPE_TRX);
        }   //	

       /// <summary>
       ///  Get Window Icon
       /// </summary>
       /// <returns></returns>
        public System.Drawing.Image GetImage()
        {
            if (_vo.VAF_Image_ID == 0)
                return null;
            //
            Model.MVAFImage mImage = MVAFImage.Get((Context)_vo.GetCtx(), _vo.VAF_Image_ID);
            return mImage.GetImage();
            
        }   //  getImage

        /// <summary>
        /// Get Window Icon
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Icon GetIcon()
        {
            if (_vo.VAF_Image_ID == 0)
                return null;
            //
            //MImage mImage = MImage.get(m_vo.ctx, m_vo.VAF_Image_ID);
            //return mImage.getIcon();
            return null;
        }   //  

        /// <summary>
        /// SO Trx Window
        /// </summary>
        /// <returns></returns>
        public bool IsSOTrx()
        {
            return _vo.IsSOTrx;
        }

     /// <summary>
     ///Get number of Tabs
     /// </summary>
     /// <returns></returns>
        public int GetTabCount()
        {
            return _tabs.Count;
        }	//	getTabCount

        /// <summary>
        ///Get i-th MTab - null if not valid
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public GridTab GetTab(int i)
        {
            if (i < 0 || i + 1 > _tabs.Count)
                return null;
            return _tabs[i];
        }

        /// <summary>
        ///Get Window_ID
        /// </summary>
        /// <returns></returns>
        public int GetVAF_Screen_ID()
        {
            return _vo.VAF_Screen_ID;
        }

        public int GetWindowWidth()
        {
            return _vo.WinWidth;
        }

        public bool GetHasPanel()
        {
            return _vo.hasPanel;
        }
        /// <summary>
        ///Get WindowNo
        /// </summary>
        /// <returns></returns>
        public int GetWindowNo()
        {
            return _vo.windowNo;
        }

        /// <summary>
        ///Get Name
        /// </summary>
        /// <returns></returns>
        public String GetName()
        {
            return _vo.Name;
        }	//	getName


        /// <summary>
        ///Get Name
        /// </summary>
        /// <returns></returns>
        public String GetDisplayName()
        {
            return _vo.DisplayName;
        }	

        /**
         *	Get Description
         *  @return Description
         */
        public String GetDescription()
        {
            return _vo.Description;
        }	

        /// <summary>
        ///Get Help
        /// </summary>
        /// <returns></returns>
        public String GetHelp()
        {
            return _vo.Help;
        }

        /// <summary>
        ///Get Window Type
        /// </summary>
        /// <returns></returns>
        public String GetWindowType()
        {
            return _vo.WindowType;
        }	//	getWindowType

         //	isTransaction

        /**
         * 	Get Window Size
         *	@return window size or null if not set
         */
        public System.Drawing.Size? GetWindowSize()
        {
            if (_vo.WinWidth != 0 && _vo.WinHeight != 0)
                return new  System.Drawing.Size(_vo.WinWidth, _vo.WinHeight);
            return null;
        }	

        /// <summary>
        ///  To String
        /// </summary>
        /// <returns></returns>
        public override  String ToString()
        {
            return "MWindow[" + _vo.windowNo + "," + _vo.Name + " (" + _vo.VAF_Screen_ID + ")]";
        }  

       /// <summary>
       /// Get Model last Updated
       /// </summary>
       /// <param name="recalc"></param>
       /// <returns></returns>
        public DateTime GetModelUpdated(bool recalc)
        {
            if (recalc || _modelUpdated == null)
            {
                String sql = "SELECT MAX(w.Updated), MAX(t.Updated), MAX(tt.Updated), MAX(f.Updated), MAX(c.Updated) "
                    + "FROM VAF_Screen w"
                    + " INNER JOIN VAF_Tab t ON (w.VAF_Screen_ID=t.VAF_Screen_ID)"
                    + " INNER JOIN VAF_TableView tt ON (t.VAF_TableView_ID=tt.VAF_TableView_ID)"
                    + " INNER JOIN VAF_Field f ON (t.VAF_Tab_ID=f.VAF_Tab_ID)"
                    + " INNER JOIN VAF_Column c ON (f.VAF_Column_ID=c.VAF_Column_ID) "
                    + "WHERE w.VAF_Screen_ID=" + GetVAF_Screen_ID();
                try
                {
                    //System.Data.IDataReader dr = DataBase.DB.ExecuteReader(sql);
                    //if (dr.Read())
                    //{
                    //    _modelUpdated = dr.gett .GetDateTime(0);	//	Window
                    //    DateTime ts = dr.GetDateTime(1);		//	Tab
                    //    if (ts.after .after(m_modelUpdated))
                    //        m_modelUpdated = ts;
                    //    ts = rs.getTimestamp(3);				//	Table
                    //    if (ts.after(m_modelUpdated))
                    //        m_modelUpdated = ts;
                    //    ts = rs.getTimestamp(4);				//	Field
                    //    if (ts.after(m_modelUpdated))
                    //        m_modelUpdated = ts;
                    //    ts = rs.getTimestamp(5);				//	Column
                    //    if (ts.after(m_modelUpdated))
                    //        m_modelUpdated = ts;
                    //}
                    //rs.close();
                    //pstmt.close();
                    //pstmt = null;
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
            }
            return _modelUpdated.Value;
        }	//	getModelUpdated

        public GridWindowVO GetWindowVO()
        {
            return _vo;
        }

        /// <summary>
        ///Get i-th MTab - null if not valid
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public GridTab GetTabByID(int ID)
        {
            for (int i = 0; i <= _tabs.Count - 1; i++)
            {
                if (ID.Equals(_tabs[i].GetVAF_Tab_ID()))
                    return _tabs[i];
            }

            return null;
        }
    }
}


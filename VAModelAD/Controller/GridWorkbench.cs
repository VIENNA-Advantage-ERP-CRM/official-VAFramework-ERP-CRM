/**
 *  Window Workbench Model
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class GridWorkbench
    {

        /// <summary>
        ///  Workbench Model Constructor
        /// </summary>
        /// <param name="ctx"></param>
        public GridWorkbench(Ctx ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        ///  No Workbench - Just Frame for Window
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Window_ID"></param>

        public GridWorkbench(Ctx ctx, int AD_Window_ID)
        {
            _ctx = ctx;
            _windows.Add(new WBWindow(TYPE_WINDOW, AD_Window_ID));
        }   //  MWorkbench

        /** Properties      */
        private Ctx _ctx;

        /** List of windows */
        private List<WBWindow> _windows = new List<WBWindow>();

        private int AD_Workbench_ID = 0;
        private String Name = "";
        private String Description = "";
        private String Help = "";
        private int AD_Column_ID = 0;
        private int AD_Image_ID = 0;
        private int AD_Color_ID = 0;
        private int PA_Goal_ID = 0;
        private String ColumnName = "";

        /**	Logger			*/
        private static VLogger log = VLogger.GetVLogger(typeof(GridWorkbench).FullName);

        /// <summary>
        ///  Init Workbench
        /// </summary>
        /// <param name="ad_Workbench_ID"></param>
        /// <returns></returns>
        public bool InitWorkbench(int ad_Workbench_ID)
        {
            AD_Workbench_ID = ad_Workbench_ID;
            //  Get WB info
            String sql = null;
            if (Env.IsBaseLanguage(_ctx, "AD_Workbench"))
                sql = "SELECT w.Name,w.Description,w.Help,"                         //  1..3
                    + " w.AD_Column_ID,w.AD_Image_ID,w.AD_Color_ID,w.PA_Goal_ID,"   //  4..7
                    + " c.ColumnName "                                              //  8
                    + "FROM AD_Workbench w, AD_Column c "
                    + "WHERE w.AD_Workbench_ID=" + AD_Workbench_ID.ToString()                   //  #1
                    + " AND w.IsActive='Y'"
                    + " AND w.AD_Column_ID=c.AD_Column_ID";
            else
                sql = "SELECT t.Name,t.Description,t.Help,"
                    + " w.AD_Column_ID,w.AD_Image_ID,w.AD_Color_ID,w.PA_Goal_ID,"
                    + " c.ColumnName "
                    + "FROM AD_Workbench w, AD_Workbench_Trl t, AD_Column c "
                    + "WHERE w.AD_Workbench_ID=" + AD_Workbench_ID.ToString()                   //  #1
                    + " AND w.IsActive='Y'"
                    + " AND w.AD_Workbench_ID=t.AD_Workbench_ID"
                    + " AND t.AD_Language='" + Env.GetAD_Language(_ctx) + "'"
                    + " AND w.AD_Column_ID=c.AD_Column_ID";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null);
                if (dr.Read())
                {
                    Name = dr[0].ToString();
                    Description = dr[1].ToString();
                    if (Description == null)
                        Description = "";
                    Help = dr[2].ToString();
                    if (Help == null)
                        Help = "";
                    //
                    AD_Column_ID = Utility.Util.GetValueOfInt(dr[3]);
                    AD_Image_ID = Utility.Util.GetValueOfInt(dr[4]);
                    AD_Color_ID = Utility.Util.GetValueOfInt(dr[5]);
                    PA_Goal_ID = Utility.Util.GetValueOfInt(dr[6]);
                    ColumnName = Utility.Util.GetValueOfString(dr[7]);
                }
                else
                {
                    AD_Workbench_ID = 0;
                }
                dr.Close();
                dr = null;
            }
            catch (System.Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

            if (AD_Workbench_ID == 0)
                return false;
            return InitWorkbenchWindows();
        }

        /**
         *  String Representation
         *  @return info
         */
        public override  String ToString()
        {
            return "MWorkbench ID=" + AD_Workbench_ID + " " + Name
                + ", windows=" + _windows.Count.ToString() + ", LinkColumn=" + ColumnName;
        }

        /**
         *  Dispose
         */
        public void Dispose()
        {
            for (int i = 0; i < _windows.Count; i++)
            {
                Dispose(i);
            }
            _windows.Clear();
            _windows = null;
        }   //  dispose


        public Query GetQuery()
        {
            return Query.GetEqualQuery(ColumnName, "@#" + ColumnName + "@");
        }   //  getQuery

        /*************************************************************************/

        /// <summary>
        ///Get Workbench
        /// </summary>
        /// <returns></returns>
        public int GetAD_Workbench_ID()
        {
            return AD_Workbench_ID;
        }

        /// <summary>
        ///Get Name
        /// </summary>
        /// <returns></returns>
        public String GetName()
        {
            return Name;
        }

        /// <summary>
        ///Get Description
        /// </summary>
        /// <returns></returns>
        public String GetDescription()
        {
            return Description;
        }

        /// <summary>
        ///	Get Help
        /// </summary>
        /// <returns></returns>
        public String GetHelp()
        {
            return Help;
        }

        /// <summary>
        ///	Get Link AD_Column_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Column_ID()
        {
            return AD_Column_ID;
        }

        /// <summary>
        ///	Get AD_Image_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Image_ID()
        {
            return AD_Image_ID;
        }

        /// <summary>
        /// Get AD_Color_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Color_ID()
        {
            return AD_Color_ID;
        }

        /// <summary>
        ///Get PA_Goal_ID
        /// </summary>
        /// <returns></returns>
        public int GetPA_Goal_ID()
        {
            return PA_Goal_ID;
        }

        /*************************************************************************/

        /** Window          */
        public const int TYPE_WINDOW = 1;
        /** Form            */
        public const int TYPE_FORM = 2;
        /** Process         */
        public const int TYPE_PROCESS = 3;
        /** Task            */
        public const int TYPE_TASK = 4;

        /// <summary>
        /// Init Workbench Windows
        /// </summary>
        /// <returns></returns>
        private bool InitWorkbenchWindows()
        {
            String sql = "SELECT AD_Window_ID, AD_Form_ID, AD_Process_ID, AD_Task_ID "
                + "FROM AD_WorkbenchWindow "
                + "WHERE AD_Workbench_ID=" + AD_Workbench_ID.ToString() + " AND IsActive='Y'"
                + "ORDER BY SeqNo";
            IDataReader dr = null;
            try
            { 
                dr = DataBase.DB.ExecuteReader(sql, null);
                while (dr.Read())
                {
                    int AD_Window_ID = Utility.Util.GetValueOfInt(dr[0]);
                    int AD_Form_ID = Utility.Util.GetValueOfInt(dr[1]);
                    int AD_Process_ID = Utility.Util.GetValueOfInt(dr[2]);
                    int AD_Task_ID = Utility.Util.GetValueOfInt(dr[3]);
                    //
                    if (AD_Window_ID > 0)
                        _windows.Add(new WBWindow(TYPE_WINDOW, AD_Window_ID));
                    else if (AD_Form_ID > 0)
                        _windows.Add(new WBWindow(TYPE_FORM, AD_Form_ID));
                    else if (AD_Process_ID > 0)
                        _windows.Add(new WBWindow(TYPE_PROCESS, AD_Process_ID));
                    else if (AD_Task_ID > 0)
                        _windows.Add(new WBWindow(TYPE_TASK, AD_Task_ID));
                }
                dr.Close();
                dr = null;
            }
            catch (System.Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, sql, e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Window Count
        /// </summary>
        /// <returns></returns>
        public int GetWindowCount()
        {
            return _windows.Count;
        }

        /// <summary>
        ///Get Window Type of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetWindowType(int index)
        {
            if (index < 0 || index > _windows.Count)
                return -1;
            WBWindow win = _windows[index];
            return win.Type;
        }

        /// <summary>
        ///Get ID for Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetWindowID(int index)
	{
		if (index < 0 || index > _windows.Count)
			return -1;
		WBWindow win = _windows[index];
		return win.ID;
	}

      /// <summary>
      /// Set Window Model of Window
      /// </summary>
      /// <param name="index">index in workbench</param>
      /// <param name="mw">model window</param>
        public void SetMWindow(int index, GridWindow mw)
	{
		if (index < 0 || index > _windows.Count)
			throw new ArgumentException ("Index invalid: " + index);
		WBWindow win = _windows[index];
		if (win.Type != TYPE_WINDOW)
			throw new ArgumentException ("Not a MWindow: " + index);
		win.mWindow = mw;
	}

        /// <summary>
        ///Get Window Model of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GridWindow GetMWindow(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.Type != TYPE_WINDOW)
                throw new ArgumentException("Not a MWindow: " + index);
            return win.mWindow;
        }

        /// <summary>
        /// Get Name of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetName(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.mWindow != null && win.Type == TYPE_WINDOW)
                return win.mWindow.GetName();
            return null;
        }

        /// <summary>
        ///  Get Description of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetDescription(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.mWindow != null && win.Type == TYPE_WINDOW)
                return win.mWindow.GetDescription();
            return null;
        }

        /// <summary>
        /// Get Help of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public String GetHelp(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.mWindow != null && win.Type == TYPE_WINDOW)
                return win.mWindow.GetHelp();
            return null;
        }

        public System.Drawing.Icon GgetIcon(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.mWindow != null && win.Type == TYPE_WINDOW)
                return win.mWindow.GetIcon();
            return null;
        }

        /// <summary>
        /// Get Image Icon of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public System.Drawing.Image GetImage(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.mWindow != null && win.Type == TYPE_WINDOW)
                return win.mWindow.GetImage();
            return null;
        }

        /// <summary>
        /// Get AD_Color_ID of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetAD_Color_ID(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            int retValue = -1;
            //	if (win.mWindow != null && win.Type == TYPE_WINDOW)
            //		return win.mWindow.getAD_Color_ID();
            if (retValue == -1)
                return GetAD_Color_ID();
            return retValue;
        }

        /// <summary>
        /// Set WindowNo of Window
        /// </summary>
        /// <param name="index"></param>
        /// <param name="windowNo"></param>
        public void SetWindowNo(int index, int windowNo)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = (WBWindow)_windows[index];
            win.WindowNo = windowNo;
        }

        /// <summary>
        ///Get WindowNo of Window
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetWindowNo(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            return win.WindowNo;
        }   //  getWindowNo

        /// <summary>
        /// Dispose of Window
        /// </summary>
        /// <param name="index"></param>
        public void Dispose(int index)
        {
            if (index < 0 || index > _windows.Count)
                throw new ArgumentException("Index invalid: " + index);
            WBWindow win = _windows[index];
            if (win.mWindow != null)
                win.mWindow.Dispose();
            win.mWindow = null;
        }   //  dispose

        /// <summary>
        ///	Get Window Size
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Size? GetWindowSize()
        {
            return  null;
        }
    }
	
	
	/**************************************************************************
	 *  Window Type
	 */
	class WBWindow
	{
		/**
		 * 	WBWindow
		 *	@param type
		 *	@param id
		 */
		public WBWindow (int type, int id)
		{
			Type = type;
			ID = id;
		}
		/** Type			*/
		public int      Type = 0;
		/** ID				*/
		public int      ID = 0;
		/** Window No		*/
		public int      WindowNo = -1;
		/** Window Midel	*/
		public GridWindow  mWindow = null;
	//	public MFrame   mFrame = null;
	//	public MProcess mProcess = null;
	}   //  WBWindow
	
}   //  Workbench


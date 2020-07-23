/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : CTreeNode
 * Purpose        : Mutable Tree Node (not a PO).
 * Class Used     : X_PA_Hierarchy
 * Chronological    Development
 * Deepak           11-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Login;
using VAdvantage.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace VAdvantage.Model
{
     public class CTreeNode:TreeNode
    { 
    /**
	 *  Construct Model TreeNode
	 *  @param node_ID	node
	 *  @param seqNo sequence
	 *  @param name name
	 *  @param description description
	 *  @param parent_ID parent
	 *  @param isSummary summary
	 *  @param imageIndicator image indicator
	 *  @param onBar on bar
	 *  @param color color
	 */
	public CTreeNode (int node_ID, int seqNo, String name, String description,
		int parent_ID, Boolean isSummary, String imageIndicator, Boolean onBar, Color color):base()
	{
		//super(); 
	//	log.fine( "CTreeNode Node_ID=" + node_ID + ", Parent_ID=" + parent_ID + " - " + name);
		m_node_ID = node_ID;
		m_seqNo = seqNo;
		m_name = name;
		m_description = description;
		if (m_description == null)
			m_description = "";
		m_parent_ID = parent_ID;
		SetSummary(isSummary);
		SetImageIndicator(imageIndicator);
		m_onBar = onBar;
		m_color = color;
	}   //  CTreeNode


	/** Node ID         */
	private int     	m_node_ID;
	/**	SeqNo			*/
	private int     	m_seqNo;
	/** Name			*/
	private String  	m_name;
	/** Description		*/
	private String  	m_description;
	/**	Parent ID		*/
	private int     	m_parent_ID;
	/**	Summaty			*/
	private Boolean 	m_isSummary;
	/** Image Indicator				*/
	private String      m_imageIndicator;
	/** Window ID       */
    private int         AD_Window_ID;
    /** Process ID      */
    private int         AD_Process_ID;
    /** Form ID         */
    private int         AD_Form_ID;
    /** Workflow ID     */
    private int         AD_Workflow_ID;
    /** Task ID         */
    private int         AD_Task_ID;
    /** Workbench ID    */
    private int         AD_Workbench_ID;
	/** Index to Icon               */
	private int 		m_imageIndex = 0;
	/**	On Bar			*/
	private Boolean 	m_onBar;
	/**	Color			*/
	private Color 		m_color;
        
	/**	Logger			*/
	private static VLogger log = VLogger.GetVLogger(typeof(CTreeNode).FullName);//.class);
	
	/*************************************************************************/

	/**	Window - 1			*/
	public static int		TYPE_WINDOW = 1;
	/**	Report - 2			*/
	public static int		TYPE_REPORT = 2;
	/**	Process - 3			*/
	public static int		TYPE_PROCESS = 3;
	/**	Workflow - 4		*/
	public static int		TYPE_WORKFLOW = 4;
	/**	Workbench - 5		*/
	public static int		TYPE_WORKBENCH = 5;
	/**	Variable - 6		*/
	public static int		TYPE_SETVARIABLE = 6;
	/**	Choice - 7			*/
	public static int		TYPE_USERCHOICE = 7;
	/**	Action - 8			*/
	public static int		TYPE_DOCACTION = 8;

	/** 16* 16 Icons		*/
	public static Image[] IMAGES = new Image[]
	{
		null,

		Env.GetImageIcon("mWindow.gif"),
		Env.GetImageIcon("mReport.gif"),
		Env.GetImageIcon("mProcess.gif"),
		Env.GetImageIcon("mWorkFlow.gif"),
		Env.GetImageIcon("mWorkbench.gif"),
		Env.GetImageIcon("mSetVariable.gif"),
		Env.GetImageIcon("mUserChoice.gif"),
		Env.GetImageIcon("mDocAction.gif")
	};


	/**************************************************************************
	 *  Get Node ID
	 *  @return node id (e.g. AD_Menu_ID)
	 */
	public int GetNode_ID()
	{
		return m_node_ID;
	}   //  getID
	
	/**
     * Get Window ID
     */
    public int GetAD_Window_ID()
    {
        return AD_Window_ID;
    }
    
    /**
     * Set Window ID
     * @param int windowID
     */
    public void SetAD_Window_ID(int windowID) 
    {
        this.AD_Window_ID = windowID;
    }
    
    /**
     * Get Process ID
     */
    public int GetAD_Process_ID()
    {
        return AD_Process_ID;
    }
    
    /**
     * Set Process ID
     * @param int processID
     */
    public void SetAD_Process_ID(int processID)
    {
        this.AD_Process_ID = processID;
    }
    
    /**
     * Get Form ID
     */
    public int GetAD_Form_ID()
    {
        return AD_Form_ID;
    }
    
    /**
     * Set Form ID
     * @param int formID
     */
    public void SetAD_Form_ID(int formID)
    {
        this.AD_Form_ID = formID;
    }
    
    /**
     * Get WorkFlow ID
     */
    public int GetAD_Workflow_ID()
    {
        return AD_Workflow_ID;
    }
    
    /**
     * Set Workflow ID
     * @param int workflowID
     */
    public void SetAD_Workflow_ID(int workflowID)
    {
        this.AD_Workflow_ID = workflowID;
    }
    
    /**
     * Get Task ID
     */
    public int GetAD_Task_ID()
    {
        return AD_Task_ID;
    }
    
    /**
     * Set Task ID
     * @param int taskID
     */
    public void SetAD_Task_ID(int taskID)
    {
        this.AD_Task_ID = taskID;
    }
    
    /**
     * Get Workbench ID
     */
    public int GetAD_Workbench_ID()
    {
        return AD_Workbench_ID;
    }
    
    /**
     * Set Workbench ID
     * @param int workbenchID
     */
    public void SetAD_Workbench_ID(int workbenchID)
    {
        this.AD_Workbench_ID = workbenchID;
    }

	/**
	 *  Set Name
	 *  @param name name
	 */
	public void SetName (String name)
	{
		if (name == null)
			m_name = "";
		else
			m_name = name;
	}   //  setName

	/**
	 *  Get Name
	 *  @return name
	 */
	public String GetName()
	{
		return m_name;
	}   //  setName

	/**
	 *	Get SeqNo (Index) as formatted String 0000 for sorting
	 *  @return SeqNo as String
	 */
	public String GetSeqNo()
	{
		String retValue = "0000" + m_seqNo;	//	not more than 100,000 nodes
		if (m_seqNo > 99999)
           // log.Log(Level.SEVERE, "TreeNode Index is higher than 99999");
		if (retValue.Length > 5)
			retValue = retValue.Substring(retValue.Length-5);	//	last 5
		return retValue;
	}	//	getSeqNo

	/**
	 *	Return parent
	 *  @return Parent_ID (e.g. AD_Menu_ID)
	 */
	public int GetParent_ID()
	{
		return m_parent_ID;
	}	//	getParent

	/**
	 *  Print Name
	 *  @return info
	 */
	public override String ToString()
	{
		return //   m_node_ID + "/" + m_parent_ID + " " + m_seqNo + " - " +
			m_name;
	}   //  toString

	/**
	 *	Get Description
	 *  @return description
	 */
	public String GetDescription()
	{
		return m_description;
	}	//	getDescription

	
	/**************************************************************************
	 *  Set Summary (allow children)
	 *  @param isSummary summary node
	 */
	public void SetSummary (Boolean isSummary)
	{
		m_isSummary = isSummary;
		//super.setAllowsChildren(isSummary);
        
	}   //  setSummary

	/**
	 *  Set Summary (allow children)
	 *  @param isSummary true if summary
	 */
	public void setAllowsChildren (Boolean isSummary)
	{
		//super.setAllowsChildren (isSummary);
		m_isSummary = isSummary;
	}   //  setAllowsChildren

	/**
	 *  Allow children to be added to this node
	 *  @return true if summary node
	 */
	public Boolean IsSummary()
	{
		return m_isSummary;
	}   //  isSummary


	/**************************************************************************
	 *  Get Image Indicator/Index
	 *  @param imageIndicator image indicator (W/X/R/P/F/T/B) X_AD_WF_Node.ACTION_
	 *  @return index of image
	 */
	public static int GetImageIndex (String imageIndicator)
	{
		int imageIndex = 0;
        if (imageIndicator == null)
        {
            ;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_UserWindow)		//	Window 
            || imageIndicator.Equals(X_AD_WF_Node.ACTION_UserForm))
        {
            imageIndex = TYPE_WINDOW;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_AppsReport))		//	Report
        {
            imageIndex = TYPE_REPORT;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_AppsProcess)		//	Process
            || imageIndicator.Equals(X_AD_WF_Node.ACTION_AppsTask))
        {
            imageIndex = TYPE_PROCESS;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_SubWorkflow))		//	WorkFlow
        {
            imageIndex = TYPE_WORKFLOW;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_UserWorkbench))	//	Workbench
        {
            imageIndex = TYPE_WORKBENCH;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_SetVariable))		//	Set Variable
        {
            imageIndex = TYPE_SETVARIABLE;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_UserChoice))		//	User Choice
        {
            imageIndex = TYPE_USERCHOICE;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_DocumentAction))	//	Document Action
        {
            imageIndex = TYPE_DOCACTION;
        }
        else if (imageIndicator.Equals(X_AD_WF_Node.ACTION_WaitSleep))		//	Sleep
        {
            ;
        }
		return imageIndex;
	}   //  getImageIndex

	/**
	 *  Set Image Indicator and Index
	 *  @param imageIndicator image indicator (W/X/R/P/F/T/B) X_AD_WF_Node.ACTION_
	 */
	public void SetImageIndicator (String imageIndicator)
	{
		if (imageIndicator != null)
		{
			m_imageIndicator = imageIndicator;
			m_imageIndex = GetImageIndex(m_imageIndicator);
		}
	}   //  setImageIndicator

	/**
	 *  Get Image Indicator
	 *  @return image indicator
	 */
	public String GetImageIndiactor()
	{
		return m_imageIndicator;
	}   //  getImageIndiactor

	/**
	 *	Get Image Icon
	 *  @param index image index
	 *  @return Icon
	 */
	public static Image GetIcon (int index)
	{
		if (index == 0 || IMAGES == null || index > IMAGES.Length)
			return null;
		return IMAGES[index];
	}	//	getIcon

	/**
	 *	Get Image Icon
	 *  @return Icon
	 */
	public Image GetIcon()
	{ 
		return GetIcon(m_imageIndex);
	}	//	getIcon

	/**
	 *  Get Shortcut Bar info
	 *  @return true if node on bar
	 */
	public Boolean isOnBar()
	{
		return m_onBar;
	}   //  isOnBar
	
	/**
	 * 	Is Process
	 *	@return true if Process
	 */
	public Boolean isProcess()
	{
		return X_AD_Menu.ACTION_Process.Equals(m_imageIndicator);
	}	//	isProcess

	/**
	 * 	Is Report
	 *	@return true if report
	 */
	public Boolean isReport()
	{
		return X_AD_Menu.ACTION_Report.Equals(m_imageIndicator);
	}	//	isReport
	
	/**
	 * 	Is Window
	 *	@return true if Window
	 */
	public Boolean isWindow()
	{
		return X_AD_Menu.ACTION_Window.Equals(m_imageIndicator);
	}	//	isWindow
	
	/**
	 * 	Is Workbench
	 *	@return true if Workbench
	 */
	public Boolean isWorkbench()
	{
		return X_AD_Menu.ACTION_Workbench.Equals(m_imageIndicator);
	}	//	isWorkbench
	
	/**
	 * 	Is Workflow
	 *	@return true if Workflow
	 */
	public Boolean isWorkFlow()
	{
		return X_AD_Menu.ACTION_WorkFlow.Equals(m_imageIndicator);
	}	//	isWorkFlow

	/**
	 * 	Is Form
	 *	@return true if Form
	 */
	public Boolean isForm()
	{
		return X_AD_Menu.ACTION_Form.Equals(m_imageIndicator);
	}	//	isForm

	/**
	 * 	Is Task
	 *	@return true if Task
	 */
	public Boolean isTask()
	{
		return X_AD_Menu.ACTION_Task.Equals(m_imageIndicator);
	}	//	isTask

	/**
	 * 	Get Color
	 *	@return color or black if not set
	 */
	public Color GetColor()
	{
        if (m_color != null)
        {
            return m_color;
        }
		return Color.Black;
	}	//	getColor
	
	/*************************************************************************/

	/**	Last found ID				*/
	private int                 m_lastID = -1;
	/** Last found Node				*/
	private CTreeNode           m_lastNode = null;




	/**
	 *	Return the Node with ID in list of children
	 *  @param ID id
	 *  @return VTreeNode with ID or null
	 */
	public CTreeNode FindNode (int ID)
	{
		if (m_node_ID == ID)
        {
			return this;
        }
		//
		if (ID == m_lastID && m_lastNode != null)
        {
			return m_lastNode;
        }
        		//
        //Enumeration<?> en = preorderEnumeration();
         ////
        TreeNode[] node = this.Nodes.Find(ID.ToString(), true);
        if (node.GetLength(0) > 0)
        {
            m_lastID = ID;
            m_lastNode = (CTreeNode)node[0];
            return m_lastNode;

        }
        return null;
        //IEnumerator<Object> iterator = cache.Keys.GetEnumerator();
        //while (en.hasMoreElements())
      
        //{
        //    CTreeNode nd = (CTreeNode)en.nextElement();
        //    if (ID == nd.GetNode_ID())
        //    {
        //        m_lastID = ID;
        //        m_lastNode = nd;
        //        return nd;
        //    }
        //}
		//return null;
	}   //  findNode

}   //  CTreeNode

}

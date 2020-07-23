/********************************************************
 * Module Name    : Show Tree(menu and favorite) 
 * Purpose        : used to Get Tree Node Data(eg menu , org etc) 
 * Class Used     : GlobalVariable.cs, TreeNode(System default)
 * Created By     : Harwinder 
 * Date           : 24 nov 2008
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;


namespace VAdvantage.Classes
{
    public class VTreeNode : TreeNode
    {
        #region "Declare Variable"
        /** Node ID         */
        private int _Node_ID;
        /**	SeqNo			*/
        private int _seqNo;
        /** Name			*/
        private string _name;
        /** Description		*/
        private string _description;
        /**	Parent ID		*/
        private int _Parent_ID;
        /**	Summaty			*/
        private bool _isSummary;
        /** Image Indicator				*/
        private string _imageIndicator;
        /** Window ID       */
        private int _AD_Window_ID;
        /** Process ID      */
        private int _AD_Process_ID;
        /** Form ID         */
        private int _AD_Form_ID;
        /** Workflow ID     */
        private int _AD_Workflow_ID;
        /** Task ID         */
        private int _AD_Task_ID;
        /** Workbench ID    */
        private int _AD_Workbench_ID;
        /** Index to Icon               */
        private int _imageIndex = 0;
        /**	On Bar			*/
        private bool _onBar;
        /**	Color			*/
        //private color m_color;

        public const string ACTION_WORKBENCH = "B";
        /** WorkFlow = F */
        public const string ACTION_WORKFLOW = "F";
        /** Process = P */
        public const string ACTION_PROCESS = "P";
        /** Report = R */
        public const string ACTION_REPORT = "R";
        /** Task = T */
        public const string ACTION_TASK = "T";
        /** Window = W */
        public const string ACTION_WINDOW = "W";
        /** Form = X */
        public const string ACTION_FORM = "X";



        /*************************************************************************/

        /**	Window - 1			*/
        public const int TYPE_WINDOW = 1;
        /**	Report - 2			*/
        public const int TYPE_REPORT = 2;
        /**	Process - 3			*/
        public const int TYPE_PROCESS = 3;
        /**	Workflow - 4		*/
        public const int TYPE_WORKFLOW = 4;
        /**	Workbench - 5		*/
        public const int TYPE_WORKBENCH = 5;
        /**	Variable - 6		*/
        public const int TYPE_SETVARIABLE = 6;
        /**	Choice - 7			*/
        public const int TYPE_USERCHOICE = 7;
        /**	Action - 8			*/
        public const int TYPE_DOCACTION = 8;

        /**************************************************************************/

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="seqNo"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="parentId"></param>
        /// <param name="isSummary"></param>
        /// <param name="imageIndicator"></param>
        /// <param name="onBar"></param>
        public VTreeNode(int nodeId, int seqNo, string name, string description,
        int parentId, bool isSummary, string imageIndicator, bool onBar)
        {
            if (imageIndicator == null)
            {
                imageIndicator = "";

            }
            _Node_ID = nodeId;
            _seqNo = seqNo;
            _name = name;
            if (description == null)
                description = "";
            _description = description;
            _Parent_ID = parentId;
            this.Text = name;
            this.Name = nodeId.ToString();
            this.ToolTipText = description;
            _imageIndicator = imageIndicator;
            if (isSummary == true) //Set Manually If summary node
            {
                this.ImageKey = "C";
                this.SelectedImageKey = "C";
            }
            else
            {
                this.ImageKey = imageIndicator;
                this.SelectedImageKey = imageIndicator;
            }
            SetSummary(isSummary);
            _onBar = onBar;
            //_color = color;
        }  //  VTreeNode


        /// <summary>
        /// copy Node Attribute(clone) 
        /// </summary>
        /// <param name="node">TreeNode</param>
        public VTreeNode(VTreeNode node)
        {

            _Node_ID = node.Node_ID;
            this.Name = node.Node_ID.ToString();
            _seqNo = int.Parse(node.GetSeqNo());
            _name = node.Text;
            this.Text = node.Text;

            this.ImageKey = node.ImageKey;
            this.SelectedImageKey = node.SelectedImageKey;

            _description = node.GetDescription;
            this.ToolTipText = node.ToolTipText;

            _Parent_ID = node.Parent_ID;

            _isSummary = node.IsSummary;        /** Image Indicator				*/

            _imageIndicator = node.ImageKey;

            _AD_Window_ID = node.AD_Window_ID;

            _AD_Process_ID = node.AD_Process_ID;

            _AD_Form_ID = node.AD_Form_ID;

            _AD_Workflow_ID = node.AD_Workflow_ID;

            _AD_Task_ID = node.AD_Task_ID;

            _AD_Workbench_ID = node.AD_Workflow_ID;

            _imageIndex = 0;
            /**	On Bar			*/
            _onBar = node.OnBar;
            if (_imageIndex < 0)
                _imageIndex = 0;
        }

        /// <summary>
        /// Constructor copy node and its child nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="copyChild"></param>
        public VTreeNode(VTreeNode node, bool copyChild)
        {

            _Node_ID = node.Node_ID;
            this.Name = node.Node_ID.ToString();
            _seqNo = int.Parse(node.GetSeqNo());
            _name = node.Text;
            this.Text = node.Text;

            this.ImageKey = node.ImageKey;
            this.SelectedImageKey = node.SelectedImageKey;

            _description = node.GetDescription;
            this.ToolTipText = node.ToolTipText;

            _Parent_ID = node.Parent_ID;

            _isSummary = node.IsSummary;        /** Image Indicator				*/

            _imageIndicator = node.ImageKey;

            _AD_Window_ID = node.AD_Window_ID;

            _AD_Process_ID = node.AD_Process_ID;

            _AD_Form_ID = node.AD_Form_ID;

            _AD_Workflow_ID = node.AD_Workflow_ID;

            _AD_Task_ID = node.AD_Task_ID;

            _AD_Workbench_ID = node.AD_Workflow_ID;

            _imageIndex = 0;
            /**	On Bar			*/
            _onBar = node.OnBar;

            //  TreeNodeCollection myTreeNodeCollection = node.Nodes;
            // Create an array of 'TreeNodes'.
            TreeNode[] myTreeNodeArray = new TreeNode[node.Nodes.Count];
            // Copy the tree nodes to the 'myTreeNodeArray' array.
            node.Nodes.CopyTo(myTreeNodeArray, 0);
            // Remove all the tree nodes from the 'myTreeViewBase' TreeView.

            try
            {
                node.Nodes.Clear();
                this.Nodes.AddRange(myTreeNodeArray);
            }
            catch 
            {

            }
        }

        //Satandard Constructor
        public VTreeNode()
        {
        }

        #region Properties
        /// <summary>
        /// returm node id
        /// </summary>
        public int Node_ID
        {
            get
            {
                return _Node_ID;
            }
            set
            {
                _Node_ID = value;
            }
        }   //  getID

        /// <summary>
        /// returm SeqNo id
        /// </summary>
        public int SeqNo
        {
            get
            {
                return _seqNo;
            }
            set
            {
                _seqNo = value;
            }
        }

        /// <summary>
        /// get and set  window id
        /// </summary>
        public int AD_Window_ID
        {
            get
            {
                return _AD_Window_ID;
            }
            set
            {
                _AD_Window_ID = value;
            }
        }

        /// <summary>
        /// get and set process Id
        /// </summary>
        public int AD_Process_ID
        {
            get
            {
                return _AD_Process_ID;
            }
            set
            {
                _AD_Process_ID = value;
            }
        }

        /// <summary>
        /// get and set form id
        /// </summary>
        public int AD_Form_ID
        {
            get
            {
                return _AD_Form_ID;
            }
            set
            {
                this._AD_Form_ID = value;
            }
        }

        /// <summary>
        /// Get and set workflow id
        /// </summary>
        public int AD_Workflow_ID
        {
            get
            {
                return _AD_Workflow_ID;
            }
            set
            {
                this._AD_Workflow_ID = value;
            }
        }

        /// <summary>
        /// get and set task id
        /// </summary>
        public int AD_Task_ID
        {
            get
            {
                return _AD_Task_ID;
            }
            set
            {
                this._AD_Task_ID = value;
            }
        }

        /// <summary>
        /// Get and set workBench id
        /// </summary>
        public int AD_Workbench_ID
        {
            get
            {
                return _AD_Workbench_ID;
            }
            set
            {
                this._AD_Workbench_ID = value;
            }
        }
        /// <summary>
        /// get and set Parent id
        /// </summary>
        public int Parent_ID
        {
            get
            {
                return _Parent_ID;
            }
            set
            {
                _Parent_ID = value;
            }
        }
        /// <summary>
        /// true if node is on bar(favorite)
        /// </summary>
        /// <returns></returns>
        public bool OnBar
        {
            get
            {
                return _onBar;
            }
            set
            {
                _onBar = value;
            }

        }
        #endregion

        /// <summary>
        /// Get and set name of node
        /// </summary>
        public string SetName
        {
            set
            {
                if (value == null)
                    _name = "";
                else
                    _name = value;
                this.Text = _name;

            }
            get
            {
                return _name;
            }
        }   //  setName





        //   /**
        //*  Set Summary (allow children)
        //*  @param isSummary true if summary
        //*/
        //   public void setAllowsChildren(boolean isSummary)
        //   {
        //       super.setAllowsChildren(isSummary);
        //       m_isSummary = isSummary;
        //   }   //  setAllowsChildren


        /// <summary>
        ///Get SeqNo (Index) as formatted String 0000 for sorting
        /// </summary>
        /// <returns></returns>
        public string GetSeqNo()
        {
            string retValue = "0000" + _seqNo;	//	not more than 100,000 nodes
            if (_seqNo > 99999)
                if (retValue.Length > 5)
                    retValue = retValue.Substring(retValue.Length - 5);	//	last 5
            return retValue;
        }

        /// <summary>
        /// Get description 
        /// </summary>
        public string GetDescription
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Set Summary property
        /// </summary>
        /// <param name="isSummary"></param>
        public void SetSummary(bool isSummary)
        {
            _isSummary = isSummary;

        }   //  setSummary

        /// <summary>
        /// true if summary node
        /// </summary>
        public bool IsSummary
        {
            get
            {
                return _isSummary;
            }
        }

        /// <summary>
        /// return true if process
        /// </summary>
        /// <returns></returns>
        public bool IsProcess()
        {
            return ACTION_PROCESS.Equals(_imageIndicator);
        }

        /// <summary>
        /// return true if report
        /// </summary>
        /// <returns></returns>
        public bool IsReport()
        {
            return ACTION_REPORT.Equals(_imageIndicator);
        }	//	isReport

        /// <summary>
        ///return true if Window
        /// </summary>
        /// <returns></returns>
        public bool IsWindow()
        {
            return ACTION_WINDOW.Equals(this.ImageKey);
        }	//	isWindow

        /// <summary>
        /// return true if workbench
        /// </summary>
        /// <returns></returns>
        public bool IsWorkbench()
        {
            return ACTION_WORKBENCH.Equals(_imageIndicator);
        }	//	isWorkbench

        /// <summary>
        /// return true if Workflow
        /// </summary>
        /// <returns>return true if workflow else false</returns>
        public bool IsWorkFlow()
        {
            return ACTION_WORKFLOW.Equals(_imageIndicator);
        }

        /// <summary>
        ///return true if Form
        /// </summary>
        /// <returns>true if Form else false</returns>
        public bool IsForm()
        {
            return ACTION_FORM.Equals(_imageIndicator);
        }	//	isForm

        /// <summary>
        /// return true if Task
        /// </summary>
        /// <returns>true if task else false</returns>
        public bool IsTask()
        {
            return ACTION_TASK.Equals(_imageIndicator);
        }

        // imageIndicator image indicator (W/X/R/P/F/T/B)
        //  Get Node ID
        //  @return node id (e.g. AD_Menu_ID)

        public int GetNode_ID()
        {
            return Node_ID;// node_ID;
        }

        /**	Last found ID				*/
        private int _lastID = -1;
        /** Last found Node				*/
        private VTreeNode _lastNode = null;

        /// <summary>
        ///Return the Node with ID in list of children
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public VTreeNode FindNode(int ID)
        {
            if (_Node_ID == ID)
                return this;
            ////
            if (ID == _lastID && _lastNode != null)
                return _lastNode;
            ////
            TreeNode[] node = this.Nodes.Find(ID.ToString(), true);
            if (node.GetLength(0) > 0)
            {
                _lastID = ID;
                _lastNode = (VTreeNode)node[0];
                return _lastNode;

            }
            return null;
        }

        public string GetAction()
        {
            return _imageIndicator;
        }

        public int GetActionID()
        {
            int id = 0;


            if (ACTION_WINDOW == _imageIndicator)
            {
                id = AD_Window_ID;
            }
            else if (ACTION_FORM == _imageIndicator)
            {
                id = AD_Form_ID;
            }

            else if (ACTION_PROCESS == _imageIndicator)
            {
                id = AD_Process_ID;
            }
            else if (ACTION_REPORT == _imageIndicator)
            {
                id = AD_Process_ID;
            }

            else if (ACTION_WORKFLOW == _imageIndicator)
            {
                id = AD_Workflow_ID;
            }
            else if (ACTION_TASK == _imageIndicator)
            {
                id = AD_Task_ID;
            }
            else if (ACTION_WORKBENCH == _imageIndicator)
            {
                id = AD_Workbench_ID;
            }

            return id;
        }

        public IEnumerator preorderEnumeration()
        {
            return new PreorderEnumerator(this);
        }
    }

    public class PreorderEnumerator : System.Collections.Generic.IEnumerator<object>
    {

        protected Stack stack;

        public PreorderEnumerator(TreeNode rootNode)
        {
            //Vector v = new Vector(1);
            //v.addElement(rootNode);	// PENDING: don't really need a vector
            stack = new Stack();
            stack.Push(rootNode);
        }

        //public bool HasMoreElements()
        //{
        //    return (stack.Count != 0 && (((TreeNode)stack.Peek()).Nodes.Count > 0));    //hasMoreElements());
        //}
        //IEnumerator children;
        public object Current
        {
            //  IEnumerator enumer = (IEnumerator)stack.Peek();\
            get
            {
                //IEnumerator enumer = (IEnumerator)stack.Peek();

                TreeNode node = (TreeNode)stack.Peek();//
                TreeNodeCollection nodes = node.Nodes; ;

                //if (node.Nodes.Count > 0)
                //{
                //    //children = node.Nodes.GetEnumerator();
                //    nodes = node.Nodes;
                //}
                //else 
                // {
                stack.Pop();
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    //stack.Push(children.Current);
                    stack.Push(nodes[i]);
                }
                return node;
            }
        }
        // Summary:
        //     Advances the enumerator to the next element of the collection.
        //
        // Returns:
        //     true if the enumerator was successfully advanced to the next element; false
        //     if the enumerator has passed the end of the collection.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The collection was modified after the enumerator was created.
        public bool MoveNext()
        {
            return stack.Count != 0; //&& (((TreeNode)stack.Peek()).Nodes.Count > 0)    //hasMoreElements());
        }
        //
        // Summary:
        //     Sets the enumerator to its initial position, which is before the first element
        //     in the collection.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The collection was modified after the enumerator was created.
        public void Reset()
        {
        }
        public void Dispose()
        {

        }

        //public TreeNode NextElement()
        //{
        //    IEnumerator enumer = (IEnumerator)stack.Peek();
        //    TreeNode node = (TreeNode)enumer.Current;
        //    IEnumerator children = node.Nodes;

        //    if (!enumer.MoveNext())
        //    {
        //        stack.Pop();
        //    }
        //    if (children.MoveNext())
        //    {
        //        stack.Push(children);
        //    }
        //    return node;
        //}
    }
}


/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : WebProjectDeploy
 * Purpose        : Deploy Web Project
 * Class Used     : X_CM_Container
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
using VAdvantage.Print;
using System.Net;
using VAdvantage.ProcessEngine;

namespace VAdvantage.CM
{
    public class WebProjectDeploy : ProcessEngine.SvrProcess
    {
        /**	WebProject					*/
        private int _CM_WebProject_ID = 0;
        /** Full Redeploy				*/
        private bool _isRedeploy = false;
        private VConnection _apps_host=null;
        /** Project						*/
        private MWebProject _project = null;
        /**	Stage Hash Map				*/
        private HashMap<int, MCStage> _map = new HashMap<int, MCStage>();
        /** List of IDs					*/
        private List<int> _idList = new List<int>();


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("CM_WebProject_ID"))
                {
                    _CM_WebProject_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("ReDeploy"))
                    if (para[i].GetParameter().ToString().Equals("Y"))
                        _isRedeploy = true;
                    else
                        _isRedeploy = false;
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }	//	prepare

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            _apps_host = new VConnection();
            CacheHandler thisHandler = new CacheHandler
                //(CacheHandler.ConvertJNPURLToCacheURL(GetCtx().GetContext("java.naming.provider.url")), log, GetCtx(), Get_Trx());
            (CacheHandler.ConvertJNPURLToCacheURL((String)_apps_host.Apps_host), log, GetCtx(), Get_Trx());
           
            log.Info("CM_WebProject_ID=" + _CM_WebProject_ID);
            _project = new MWebProject(GetCtx(), _CM_WebProject_ID, Get_Trx());
            if (_project.Get_ID() != _CM_WebProject_ID)
            {
                throw new Exception("@NotFound@ @CM_WebProject_ID@ " + _CM_WebProject_ID);
            }
            log.Log(Level.INFO, "Starting media deployment");
            //	Deploy Media
            MMediaServer[] mserver = MMediaServer.GetMediaServer(_project);

            for (int i = 0; i < mserver.Length; i++)
            {
                log.Log(Level.INFO, "Media Server deployment started on: " + mserver.ToString());
                if (_isRedeploy)
                    mserver[i].ReDeployAll();
                mserver[i].Deploy();
                log.Log(Level.INFO, "Media Server deployment finished on: " + mserver.ToString());
            }

            //	Stage
            MCStage[] stages = MCStage.GetStages(_project);
            for (int i = 0; i < stages.Length; i++)
                _map.Add(Utility.Util.GetValueOfInt(stages[i].GetCM_CStage_ID()), stages[i]);

            //	Copy Stage Tree
            MTree treeS = new MTree(GetCtx(), _project.GetAD_TreeCMS_ID(), false, false, Get_Trx());
            VTreeNode root = treeS.GetRootNode();
            CopyStage(root, "/", _isRedeploy);

            //	Delete Inactive Containers
            MContainer[] containers = MContainer.GetContainers(_project);
            for (int i = 0; i < containers.Length; i++)
            {
                MContainer container = containers[i];
                if (!_idList.Contains(Utility.Util.GetValueOfInt(container.GetCM_Container_ID())))
                {
                    String name = container.GetName();
                    if (container.Delete(true))
                        log.Fine("Deleted: " + name);
                    else	//	e.g. was referenced
                    {
                        log.Warning("Failed Delete: " + name);
                        AddLog(0, null, null, "@Error@ @Delete@: " + name);
                    }
                }
                // Remove Container from cache
                thisHandler.CleanContainer(container.Get_ID());
            }	//	Delete Inactive

            //	Sync Stage & Container Tree
            MTreeNodeCMS[] nodesCMS = MTreeNodeCMS.GetTree(GetCtx(), _project.GetAD_TreeCMS_ID(), Get_Trx());
            MTreeNodeCMC[] nodesCMC = MTreeNodeCMC.GetTree(GetCtx(), _project.GetAD_TreeCMC_ID(), Get_Trx());
            for (int s = 0; s < nodesCMS.Length; s++)
            {
                MTreeNodeCMS nodeCMS = nodesCMS[s];
                int Node_ID = nodeCMS.GetNode_ID();
                for (int c = 0; c < nodesCMC.Length; c++)
                {
                    MTreeNodeCMC nodeCMC = nodesCMC[c];
                    if (nodeCMC.GetNode_ID() == Node_ID)
                    {
                        //if (nodeCMS.getParent_ID()!=0) 
                        nodeCMC.setParent_ID(nodeCMS.GetParent_ID());
                        nodeCMC.SetSeqNo(nodeCMS.GetSeqNo());
                        nodeCMC.Save();
                        break;
                    }
                }
            }	//	for all stage nodes
            // Clean ContainerTree Cache
            thisHandler.CleanContainerTree(_CM_WebProject_ID);

            return "@Copied@ @CM_Container_ID@ #" + _idList.Count;
        }	//	doIt


        /// <summary>
        /// Copy Stage
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="path">path</param>
        /// <param name="isRedeploy">is dedeploy</param>
        private void CopyStage(VTreeNode node, String path, bool isRedeploy)
        {
            CacheHandler thisHandler = new CacheHandler
            (CacheHandler.ConvertJNPURLToCacheURL
                (_apps_host.Apps_host), log, GetCtx(), Get_Trx());
            int? ID = Utility.Util.GetValueOfInt(node.GetNode_ID());
            MCStage stage = _map.Get(ID.Value);
            //	
            //int size = node.getChildCount();
            int size = node.Nodes.Count;
            for (int i = 0; i < size; i++)
            {
                VTreeNode child = (VTreeNode)node.Nodes[i];// .getChildAt(i);
                int? ID1 = Utility.Util.GetValueOfInt(child.Node_ID);//.getNode_ID());
                stage = _map.Get(ID1);
                if (stage == null)
                {
                    log.Warning("Not Found ID=" + ID);
                    continue;
                }
                if (!stage.IsActive())
                    continue;
                // If we have a stage and it is modified we will update!
                if (stage != null)
                {
                    if (isRedeploy || stage.IsModified() || stage.IsSummary())
                    {
                        log.Log(Level.INFO, "Deploying container: " + path + stage.ToString());
                        MContainer cc = MContainer.Deploy(_project, stage, path);
                        if (cc != null)
                        {
                            AddLog(0, null, null, "@Updated@: " + cc.GetName());
                            _idList.Add(ID1.Value);
                        }
                        // Remove Container from cache
                        thisHandler.CleanContainer(cc.Get_ID());
                        // Reset Modified flag...
                        stage.SetIsModified(false);
                        stage.Save(stage.Get_Trx());
                    }
                    else
                    {
                        // If not modified we should check update status...
                        // But even if updtodate we need to add it to the list, because otherwise it will get deleted!
                        _idList.Add(ID1.Value);
                    }
                }
                if (child.IsSummary)
                {
                    CopyStage(child, path + stage.GetRelativeURL() + "/", isRedeploy);
                }
            }
        }	//	copyStage



    }	
}

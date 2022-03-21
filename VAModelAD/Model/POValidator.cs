using BaseLibrary.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

using VAModelAD.Model;

namespace VAModelAD.Model
{
    public class POValidator : POAction
    {

        private void RegisterPORecordList()
        {
            PORecord.AddParent(X_C_Order.Table_ID, X_C_Order.Table_Name);
            PORecord.AddParent(X_CM_Container.Table_ID, X_CM_Container.Table_Name);

            //parent child7
            PORecord.AddParentChild(X_C_OrderLine.Table_ID, X_C_OrderLine.Table_Name);
            PORecord.AddParentChild(X_CM_Container_Element.Table_ID, X_CM_Container_Element.Table_Name);

            //cascade
            PORecord.AddCascade(X_AD_Attachment.Table_ID, X_AD_Attachment.Table_Name);
            PORecord.AddCascade(X_AD_Archive.Table_ID, X_AD_Archive.Table_Name);
            PORecord.AddCascade(X_AD_Note.Table_ID, X_AD_Note.Table_Name);
            PORecord.AddCascade(X_MailAttachment1.Table_ID, X_MailAttachment1.Table_Name);
            PORecord.AddCascade(X_AppointmentsInfo.Table_ID, X_AppointmentsInfo.Table_Name);
            PORecord.AddCascade(X_K_Index.Table_ID, X_K_Index.Table_Name);

            //Restricts
            PORecord.AddRestricts(X_CM_Chat.Table_ID, X_CM_Chat.Table_Name);
            PORecord.AddRestricts(X_R_Request.Table_ID, X_R_Request.Table_Name);
        }

        public POValidator()
        {
            RegisterPORecordList();
        }

        /// <summary>
        ///Insert id data into Tree
        /// </summary>
        /// <returns></returns>
        private bool InsertTreeNode(PO po)
        {
            int AD_Table_ID = po.Get_Table_ID();
            if (!MTree.HasTree(AD_Table_ID, po.GetCtx()))
                return false;
            int id = po.Get_ID();
            int AD_Client_ID = po.GetAD_Client_ID();
            String treeTableName = MTree.GetNodeTableName(AD_Table_ID, po.GetCtx());
            int C_Element_ID = 0;
            if (AD_Table_ID == X_C_ElementValue.Table_ID)
            {
                int? ii = (int?)po.Get_Value("C_Element_ID");
                if (ii != null)
                    C_Element_ID = ii.Value;
            }
            //
            StringBuilder sb = new StringBuilder("INSERT INTO ")
                .Append(treeTableName)
                .Append(" (AD_Client_ID,AD_Org_ID, IsActive,Created,CreatedBy,Updated,UpdatedBy, ")
                .Append("AD_Tree_ID, Node_ID, Parent_ID, SeqNo) ")
                //
                .Append("SELECT t.AD_Client_ID,0, 'Y', SysDate, 0, SysDate, 0,")
                .Append("t.AD_Tree_ID, ").Append(id).Append(", 0, 999 ")
                .Append("FROM AD_Tree t ")
                .Append("WHERE t.AD_Client_ID=").Append(AD_Client_ID).Append(" AND t.IsActive='Y'");
            //	Account Element Value handling
            if (C_Element_ID != 0)
                sb.Append(" AND EXISTS (SELECT * FROM C_Element ae WHERE ae.C_Element_ID=")
                    .Append(C_Element_ID).Append(" AND t.AD_Tree_ID=ae.AD_Tree_ID)");
            else	//	std trees
                sb.Append(" AND t.IsAllNodes='Y' AND t.AD_Table_ID=").Append(AD_Table_ID);
            //	Duplicate Check
            sb.Append(" AND NOT EXISTS (SELECT * FROM ").Append(treeTableName).Append(" e ")
                .Append("WHERE e.AD_Tree_ID=t.AD_Tree_ID AND Node_ID=").Append(id).Append(")");
            //
            // Check applied to insert the node in treenode from organization units window in only default tree - Changed by Mohit asked by mukesh sir and ashish
            if (AD_Table_ID == X_AD_Org.Table_ID)
            {
                X_AD_Org Org = new X_AD_Org(po.GetCtx(), id, null);
                if (Org.Get_ColumnIndex("IsOrgUnit") > -1)
                {
                    if (Org.IsOrgUnit())
                    {
                        int DefaultTree_ID = MTree.GetDefaultAD_Tree_ID(po.GetAD_Client_ID(), AD_Table_ID);
                        sb.Append(" AND t.AD_Tree_ID=").Append(DefaultTree_ID);
                    }
                }
            }
            int no = DB.ExecuteQuery(sb.ToString(), null, po.Get_Trx());
            if (no > 0)
            {
                po.GetLog().Fine("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
            }
            else
            {
                po.GetLog().Warning("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
            }
            return no > 0;
        }

        /// <summary>
        /// Delete ID Tree Nodes
        /// </summary>
        /// <returns>true if actually deleted (could be non existing)</returns>
        private bool DeleteTreeNode(PO po)
        {
            int id = po.Get_ID();
            if (id == 0)
                id = po.Get_IDOld();
            int AD_Table_ID = po.Get_Table_ID();
            if (!MTree.HasTree(AD_Table_ID, po.GetCtx()))
                return false;
            String treeTableName = MTree.GetNodeTableName(AD_Table_ID, po.GetCtx());
            if (treeTableName == null)
                return false;
            //
            StringBuilder sb = new StringBuilder("DELETE FROM ")
                .Append(treeTableName)
                .Append(" n WHERE Node_ID=").Append(id)
                .Append(" AND EXISTS (SELECT * FROM AD_Tree t ")
                .Append("WHERE t.AD_Tree_ID=n.AD_Tree_ID AND t.AD_Table_ID=")
                .Append(AD_Table_ID).Append(")");
            //
            int no = DB.ExecuteQuery(sb.ToString(), null, po.Get_Trx());
            if (no > 0)
                po.GetLog().Fine("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
            else
                po.GetLog().Warning("#" + no.ToString() + " - AD_Table_ID=" + AD_Table_ID);
            return no > 0;
        }

        public bool BeforeSave(PO po)
        {

            if (po.GetAD_Org_ID() == 0
                && (po.GetAccessLevel() == PO.ACCESSLEVEL_ORG
                    || (po.GetAccessLevel() == PO.ACCESSLEVEL_CLIENTORG
                        && MClientShare.IsOrgLevelOnly(po.GetAD_Client_ID(), po.Get_Table_ID()))))
            {
                po.GetLog().SaveError("FillMandatory", Msg.GetElement(po.GetCtx(), "AD_Org_ID"));
                return false;
            }

            //	Should be Org 0
            if (po.GetAD_Org_ID() != 0)
            {
                bool reset = po.GetAccessLevel() == PO.ACCESSLEVEL_SYSTEM;
                if (!reset && MClientShare.IsClientLevelOnly(po.GetAD_Client_ID(), po.Get_Table_ID()))
                {
                    reset = po.GetAccessLevel() == PO.ACCESSLEVEL_CLIENT
                        || po.GetAccessLevel() == PO.ACCESSLEVEL_SYSTEMCLIENT
                        || po.GetAccessLevel() == PO.ACCESSLEVEL_CLIENTORG;
                }
                if (reset)
                {
                    po.GetLog().Warning("Set Org to 0");
                    po.SetAD_Org_ID(0);
                }
            }

            //	Before Save
            MAssignSet.Execute(po, po.Is_New());	//	Automatic Assignment
            return true;
        }

        public bool AfterSave(bool newRecord, bool success, PO po)
        {
            if (success && newRecord)
                InsertTreeNode(po);

            // Case for Master Data Versioning, check if the record being saved is in Version table

            string tableName = GetTable(po.Get_TableName());
            Ctx p_ctx = po.GetCtx();
            Trx trx = po.Get_Trx();
            if (tableName.ToLower().EndsWith("_ver"))
            {
                // Get Master Data Properties
                var MasterDetails = po.GetMasterDetails();
                // check if Record has any Workflow (Value Type) linked, or Is Immediate save etc
                if (MasterDetails != null && MasterDetails.AD_Table_ID > 0 && MasterDetails.ImmediateSave && !MasterDetails.HasDocValWF)
                {
                    // create object of parent table
                    MTable tbl = MTable.Get(p_ctx, MasterDetails.AD_Table_ID);
                    PO _po = null;
                    bool updateMasID = false;
                    // check if Master table has single key or multiple keys or single key
                    // then create object 
                    if (tbl.IsSingleKey())
                    {
                        _po = tbl.GetPO(p_ctx, MasterDetails.Record_ID, trx);
                        if (_po.Get_ID() <= 0)
                            updateMasID = true;
                    }
                    else
                    {
                        // fetch key columns for parent table
                        string[] keyCols = tbl.GetKeyColumns();
                        StringBuilder whereCond = new StringBuilder("");
                        for (int w = 0; w < keyCols.Length; w++)
                        {
                            if (w == 0)
                            {
                                if (keyCols[w] != null)
                                    whereCond.Append(keyCols[w] + " = " + po.Get_Value(keyCols[w]));
                                else
                                    whereCond.Append(" NVL(" + keyCols[w] + ",0) = 0");
                            }
                            else
                            {
                                if (keyCols[w] != null)
                                    whereCond.Append(" AND " + keyCols[w] + " = " + po.Get_Value(keyCols[w]));
                                else
                                    whereCond.Append(" AND NVL(" + keyCols[w] + ",0) = 0");
                            }
                        }
                        _po = tbl.GetPO(p_ctx, whereCond.ToString(), trx);
                    }
                    if (_po != null)
                    {
                        _po.SetAD_Window_ID(MasterDetails.AD_Window_ID);
                        // copy date from Version table to Master table
                        bool saveSuccess = CopyVersionToMaster(_po, po);
                        if (!saveSuccess)
                        {
                            if (trx != null)
                            {
                                trx.Rollback();
                                trx.Close();
                                return false;
                            }
                        }
                        else
                        {
                            if (updateMasID)
                            {
                                MasterDetails.Record_ID = _po.Get_ID();
                                // set new values in MaserDetails Object
                                po.SetMasterDetails(MasterDetails);
                                // Update key column in version table against Master table 
                                // only in case of single key column and in case of new record
                                string sqlQuery = "UPDATE " + tableName + " SET " + _po.Get_TableName() + "_ID = " + _po.Get_ID() + " WHERE " + tableName + "_ID = " + po.Get_ID();
                                int count = DB.ExecuteQuery(sqlQuery, null, trx);
                            }
                        }
                    }
                }
            }
            return success;
        }

        public bool BeforeDelete(PO po)
        {
            return true;
            //throw new NotImplementedException();
        }

        public bool AfterDelete(PO po, bool success)
        {
            if (success)
                DeleteTreeNode(po);

            if (po.Get_Table_ID() == X_AD_Attachment.Table_ID)
            {
                MAttachment.DeleteFileData(po.Get_Table_ID().ToString() + "_" + po.Get_ID().ToString());
            }
            return success;
        }

        public bool IsAutoUpdateTrl(Ctx ctx, string tableName)
        {
            MClient client = MClient.Get(ctx);
            return client.IsAutoUpdateTrl(tableName);
        }

        public string GetDocumentNo(int dt, PO po)
        {
            dynamic masDet = po.GetMasterDetails();
            string value = null;
            if (dt != -1)       //	get based on Doc Type (might return null)
                value = MSequence.GetDocumentNo(dt, po.Get_Trx(), po.GetCtx(), false, po);
            if (value == null)  //	not overwritten by DocType and not manually entered
            {
                if (masDet != null && masDet.TableName != null && masDet.TableName != "")
                    value = MSequence.GetDocumentNo(po.GetAD_Client_ID(), masDet.TableName, po.Get_Trx(), po.GetCtx());
                else
                    value = MSequence.GetDocumentNo(po.GetAD_Client_ID(), po.GetTableName(), po.Get_Trx(), po.GetCtx());
            }
            return value;
        }

        public int GetNextID(int AD_Client_ID, string TableName, Trx trx)
        {
            return MSequence.GetNextID(AD_Client_ID, TableName, trx);
        }

        public string GetDocumentNo(PO po)
        {
            dynamic masDet = po.GetMasterDetails();
            string value = null;
            if (masDet != null && masDet.TableName != null && masDet.TableName != "")
                value = MSequence.GetDocumentNo(masDet.TableName, po.Get_Trx(), po.GetCtx(), po);
            else
            {
                // Handled to get Search Key based on Organization same as Document No.
                value = MSequence.GetDocumentNo(GetTable(po.Get_TableName()), po.Get_Trx(), po.GetCtx(), po);

            }
            return value;
        }

        /// <summary>
        /// Copy record from Version table to Master table
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        private bool CopyVersionToMaster(PO poMaster, PO po)
        {
            int count = po.Get_ColumnCount();
            for (int i = 0; i < count; i++)
            {
                string columnName = po.Get_ColumnName(i);
                // skip column if column name is either "Created" or "CreatedBy"
                if (columnName.Trim().ToLower() == "created" || columnName.Trim().ToLower() == "createdby")
                    continue;
                if (poMaster.Get_ColumnIndex(columnName) < 0)
                    continue;

                if (poMaster.Get_Value(columnName) != po.Get_ValueOld(columnName))
                {
                    poMaster.Set_ValueNoCheck(columnName, po.Get_Value(columnName));
                }
            }

            if (!poMaster.Save())
                return false;

            return true;
        }

        private String GetTable(string tblName)
        {
            String tableName = tblName;
            if (!tableName.StartsWith("I_"))
            {
                return tblName;
            }

            tableName = tblName.Replace("I_", "AD_");
            int tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "M_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "C_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "R_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "MRP_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "A_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "B_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "CM_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "GL_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "K_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "PA_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "S_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "T_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            tableName = tblName.Replace("I_", "W_");
            tableID = MTable.Get_Table_ID(tableName);
            if (tableID > 0)
                return tableName;

            return tblName;

        }

        public Lookup GetLookup(Ctx ctx, POInfoColumn colInfo)
        {
            return Common.GetColumnLookup(ctx, colInfo);
        }

        public dynamic GetAttachment(Ctx ctx, int aD_Table_ID, int id)
        {
            return MAttachment.Get(ctx, aD_Table_ID, id);
        }

        public dynamic CreateAttachment(Ctx ctx, int aD_Table_ID, int id, Trx trx)
        {
            return new MAttachment(ctx, aD_Table_ID, id, trx);
        }
    }
}
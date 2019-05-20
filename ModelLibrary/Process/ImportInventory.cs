/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportInventory
 * Purpose        : Import Physical Inventory fom I_Inventory
 * Class Used     : ProcessEngine.SvrProcess
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
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportInventory : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /**	Organization to be imported to	*/
        private int _AD_Org_ID = 0;
        /**	Locator to be imported to		*/
        private int _M_Locator_ID = 0;
        /**	Default Date					*/
        private DateTime? _MovementDate = null;
        /**	Delete old Imported				*/
        private bool _DeleteOldImported = false;

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
                else if (name.Equals("AD_Client_ID"))
                {
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("M_Locator_ID"))
                {
                    _M_Locator_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("MovementDate"))
                    _MovementDate = (DateTime?)para[i].GetParameter();
                else if (name.Equals("DeleteOldImported"))
                    _DeleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            log.Info("M_Locator_ID=" + _M_Locator_ID + ",MovementDate=" + _MovementDate);
            //
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_DeleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_Inventory "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, Location, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_Inventory "
                  + "SET AD_Client_ID = COALESCE (AD_Client_ID,").Append(_AD_Client_ID).Append("),"
                  + " AD_Org_ID = COALESCE (AD_Org_ID,").Append(_AD_Org_ID).Append("),");
            if (_MovementDate != null)
            {
                sql.Append(" MovementDate = COALESCE (MovementDate,").Append(DataBase.DB.TO_DATE(_MovementDate)).Append("),");
            }
            sql.Append(" IsActive = COALESCE (IsActive, 'Y'),"
                  + " Created = COALESCE (Created, SysDate),"
                  + " CreatedBy = COALESCE (CreatedBy, 0),"
                  + " Updated = COALESCE (Updated, SysDate),"
                  + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                  + " I_ErrorMsg = NULL,"
                  + " M_Warehouse_ID = NULL,"	//	reset
                  + " I_IsImported = 'N' "
                  + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_Inventory o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0"
                + " OR EXISTS (SELECT * FROM AD_Org oo WHERE o.AD_Org_ID=oo.AD_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
            {
                log.Warning("Invalid Org=" + no);
            }

            // gwu: bug 1703137
            // if Warehouse key provided, get Warehouse ID
            sql = new StringBuilder("UPDATE I_Inventory i "
                    + "SET M_Warehouse_ID=(SELECT MAX(M_Warehouse_ID) FROM M_Warehouse w"
                    + " WHERE i.WarehouseValue=w.Value AND i.AD_Client_ID=w.AD_Client_ID) "
                    + "WHERE M_Warehouse_ID IS NULL AND WarehouseValue IS NOT NULL"
                    + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Warehouse from Value =" + no);

            //	Location
            sql = new StringBuilder("UPDATE I_Inventory i "
                + "SET M_Locator_ID=(SELECT MAX(M_Locator_ID) FROM M_Locator l"
                + " WHERE i.LocatorValue=l.Value AND COALESCE (i.M_Warehouse_ID, l.M_Warehouse_ID)=l.M_Warehouse_ID AND i.AD_Client_ID=l.AD_Client_ID) "
                + "WHERE M_Locator_ID IS NULL AND LocatorValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Locator from Value =" + no);
            sql = new StringBuilder("UPDATE I_Inventory i "
                + "SET M_Locator_ID=(SELECT MAX(M_Locator_ID) FROM M_Locator l"
                + " WHERE i.X=l.X AND i.Y=l.Y AND i.Z=l.Z AND COALESCE (i.M_Warehouse_ID, l.M_Warehouse_ID)=l.M_Warehouse_ID AND i.AD_Client_ID=l.AD_Client_ID) "
                + "WHERE M_Locator_ID IS NULL AND X IS NOT NULL AND Y IS NOT NULL AND Z IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Locator from X,Y,Z =" + no);
            if (_M_Locator_ID != 0)
            {
                sql = new StringBuilder("UPDATE I_Inventory "
                    + "SET M_Locator_ID = ").Append(_M_Locator_ID).Append(
                    " WHERE M_Locator_ID IS NULL"
                    + " AND I_IsImported<>'Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Set Locator from Parameter=" + no);
            }
            sql = new StringBuilder("UPDATE I_Inventory "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Location, ' "
                + "WHERE M_Locator_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Location=" + no);


            //	Set M_Warehouse_ID 
            sql = new StringBuilder("UPDATE I_Inventory i "
                + "SET M_Warehouse_ID=(SELECT M_Warehouse_ID FROM M_Locator l WHERE i.M_Locator_ID=l.M_Locator_ID) "
                + "WHERE M_Locator_ID IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Warehouse from Locator =" + no);
            sql = new StringBuilder("UPDATE I_Inventory "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Warehouse, ' "
                + "WHERE M_Warehouse_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Warehouse=" + no);


            //	Product
            sql = new StringBuilder("UPDATE I_Inventory i "
                  + "SET M_Product_ID=(SELECT MAX(M_Product_ID) FROM M_Product p"
                  + " WHERE i.Value=p.Value AND i.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_Product_ID IS NULL AND Value IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from Value=" + no);
            sql = new StringBuilder("UPDATE I_Inventory i "
                  + "SET M_Product_ID=(SELECT MAX(M_Product_ID) FROM M_Product p"
                  + " WHERE i.UPC=p.UPC AND i.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_Product_ID IS NULL AND UPC IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from UPC=" + no);
            sql = new StringBuilder("UPDATE I_Inventory "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Product, ' "
                + "WHERE M_Product_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Product=" + no);

            //	No QtyCount
            sql = new StringBuilder("UPDATE I_Inventory "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Qty Count, ' "
                + "WHERE QtyCount IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No QtyCount=" + no);

            Commit();

            /*********************************************************************/

            MInventory inventory = null;

            int noInsert = 0;
            int noInsertLine = 0;

            //	Go through Inventory Records
            sql = new StringBuilder("SELECT * FROM I_Inventory "
                + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY M_Warehouse_ID, TRUNC(MovementDate,'DD'), I_Inventory_ID");
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement (sql.ToString (), Get_TrxName());
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                //
                int x_M_Warehouse_ID = -1;
                DateTime? x_MovementDate = null;
                while (idr.Read())
                {
                    X_I_Inventory imp = new X_I_Inventory(GetCtx(), idr, Get_TrxName());
                    DateTime? MovementDate = TimeUtil.GetDay(imp.GetMovementDate());

                    if (inventory == null
                        || imp.GetM_Warehouse_ID() != x_M_Warehouse_ID
                        || !MovementDate.Equals(x_MovementDate))
                    {
                        inventory = new MInventory(GetCtx(), 0, Get_TrxName());
                        inventory.SetClientOrg(imp.GetAD_Client_ID(), imp.GetAD_Org_ID());
                        inventory.SetDescription("I " + imp.GetM_Warehouse_ID() + " " + MovementDate);
                        inventory.SetM_Warehouse_ID(imp.GetM_Warehouse_ID());
                        inventory.SetMovementDate(MovementDate);
                        //
                        if (!inventory.Save())
                        {
                            log.Log(Level.SEVERE, "Inventory not saved");
                            break;
                        }
                        x_M_Warehouse_ID = imp.GetM_Warehouse_ID();
                        x_MovementDate = MovementDate;
                        noInsert++;
                    }

                    //	Line
                    int M_AttributeSetInstance_ID = 0;
                    if (imp.GetLot() != null || imp.GetSerNo() != null)
                    {
                        MProduct product = MProduct.Get(GetCtx(), imp.GetM_Product_ID());
                        if (product.IsInstanceAttribute())
                        {
                            MAttributeSet mas = product.GetAttributeSet();
                            MAttributeSetInstance masi = new MAttributeSetInstance(GetCtx(), 0, mas.GetM_AttributeSet_ID(), Get_TrxName());
                            if (mas.IsLot() && imp.GetLot() != null)
                                masi.SetLot(imp.GetLot(), imp.GetM_Product_ID());
                            if (mas.IsSerNo() && imp.GetSerNo() != null)
                                masi.SetSerNo(imp.GetSerNo());
                            masi.SetDescription();
                            masi.Save();
                            M_AttributeSetInstance_ID = masi.GetM_AttributeSetInstance_ID();
                        }
                    }
                    MInventoryLine line = new MInventoryLine(inventory,
                        imp.GetM_Locator_ID(), imp.GetM_Product_ID(), M_AttributeSetInstance_ID,
                        imp.GetQtyBook(), imp.GetQtyCount());
                    if (line.Save())
                    {
                        imp.SetI_IsImported(X_I_Inventory.I_ISIMPORTED_Yes);
                        imp.SetM_Inventory_ID(line.GetM_Inventory_ID());
                        imp.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                        imp.SetProcessed(true);
                        if (imp.Save())
                            noInsertLine++;
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                { idr.Close(); }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_Inventory "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@M_Inventory_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertLine), "@M_InventoryLine_ID@: @Inserted@");
            return "";
        }	//	doIt

    }
}

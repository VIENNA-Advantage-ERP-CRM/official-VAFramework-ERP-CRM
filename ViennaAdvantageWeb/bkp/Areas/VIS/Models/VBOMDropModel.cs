/********************************************************
 * Module Name    : VIS
 * Purpose        : Model class for VBOMDrop Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     27 May 2015
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class VBOMDropModel
    {
        //Logger			
        private static VLogger log = VLogger.GetVLogger(typeof(VBOMDropModel).FullName);
        //	Product to create BOMs from	
        private MProduct _product;
        // BOM Qty						
        private Decimal _qty = 1;
        //	Line Counter				
        private int _bomLine = 0;
        List<BOMLines> lstBOMLines =null;
        private List<BOMLines> _selectionList = new List<BOMLines>();
        public BOMDrop GetDetail(Ctx ctx)
        {
            BOMDrop objBOMDrop = new BOMDrop();
            objBOMDrop.Product = GetProduct(ctx);
            objBOMDrop.Invoice = GetInvoice(ctx);
            objBOMDrop.Order = GetOrder(ctx);
            objBOMDrop.Opportunity = GetOpportunity(ctx);
            return objBOMDrop;
        }

        private List<Product> GetProduct(Ctx ctx)
        {
            List<Product> lstProduct = new List<Product>();
            DataSet ds = new DataSet();
            string sql = "SELECT M_Product_ID, Name "
                    + "FROM M_Product "
                    + "WHERE IsBOM='Y' AND IsVerified='Y' AND IsActive='Y' "
                    + "ORDER BY Name";

            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Product", true, false);    
            ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {                   
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstProduct.Add(new Product()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_Product_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                            
                        });
                    }
                }
            }
            return lstProduct;
        }

        private List<Invoice> GetInvoice(Ctx ctx)
        {
            List<Invoice> lstInvoice = new List<Invoice>();
            DataSet ds = new DataSet();
            string sql = "SELECT C_Invoice_ID, DocumentNo || '_' || GrandTotal AS Name "
                   + "FROM C_Invoice "
                   + "WHERE Processed='N' AND DocStatus='DR' "
                   + "ORDER BY DocumentNo";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_Invoice", true, false);    
            ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstInvoice.Add(new Invoice()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_Invoice_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])

                        });
                    }
                }
            }
            return lstInvoice;
        }

        private List<Order> GetOrder(Ctx ctx)
        {
            List<Order> lstOrder = new List<Order>();
            DataSet ds = new DataSet();
            string sql = "SELECT C_Order_ID, DocumentNo || '_' || GrandTotal as Name "
                  + "FROM C_Order "
                  + "WHERE IsActive='Y' AND IsSOTrx='N' AND IsReturnTrx='N' AND Processed='N' AND DocStatus='DR' "
                  + "ORDER BY DocumentNo";
            // Added Check By Pratap 30-12-15  -- IsActive='Y' AND IsSOTrx='N' AND IsReturnTrx='N'
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_Order", true, false);     
            ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstOrder.Add(new Order()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_Order_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])

                        });
                    }
                }
            }
            return lstOrder;
          
        }

        private List<Opportunity> GetOpportunity(Ctx ctx)
        {
            List<Opportunity> lstOpportunity = new List<Opportunity>();
            DataSet ds = new DataSet();
            string sql = "SELECT C_Project_ID, Name "
                 + "FROM C_Project "
                 + "WHERE Processed='N' AND IsSummary='N' AND IsActive='Y'"
                 + " AND ProjectCategory<>'S' "
                 + "ORDER BY Name";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_Project", true, false);          
            ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstOpportunity.Add(new Opportunity()
                        {
                            ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_Project_ID"]),
                            Value = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])

                        });
                    }
                }
            }
            CreateMainPanel(ctx, 0);
            return lstOpportunity;
     
        }

        /// <summary>
        /// Create Main Panel.
        /// Called when changing Product 	
        /// </summary>
        public List<BOMLines> CreateMainPanel(Ctx ctx, int productID)
        {
           
            String title = Msg.GetMsg(ctx, "SelectProduct");
            _product = new MProduct(ctx, productID, null);
            if (_product != null && _product.Get_ID() > 0)
            {
                title = _product.GetName();
                if (_product.GetDescription() != null && _product.GetDescription().Length > 0)
                {
                    //this.setToolTipText(_product.getDescription());
                }
                _bomLine = 0;
                BomLines(ctx,_product, _qty);
            }
            return lstBOMLines;
        }

        private void BomLines(Ctx ctx,MProduct product, Decimal qty)
        {
                lstBOMLines = new List<BOMLines>();
                MProductBOM[] bomLines = null;          
                bomLines = MProductBOM.GetBOMLines(product);
                MProduct objproduct=null;
                for (int i = 0; i < bomLines.Length; i++)
                {
                    AddBOMLine(ctx,bomLines[i], qty);
                    objproduct = new MProduct(ctx, Util.GetValueOfInt(bomLines[i].GetM_ProductBOM_ID()), null);
                    lstBOMLines.Add(new BOMLines()
                    {
                            BOMType=Util.GetValueOfString(bomLines[i].GetBOMType()),
                            BOMTypeName=GetBOMType(Util.GetValueOfString(bomLines[i].GetBOMType())),
                            BOMQty=Util.GetValueOfString(bomLines[i].GetBOMQty()),
                            ProductID= Util.GetValueOfString(bomLines[i].GetM_ProductBOM_ID()),
                            ProductName = Util.GetValueOfString(objproduct.GetName()),
                            ProductBOMID=Util.GetValueOfString(bomLines[i].GetM_Product_BOM_ID()),
                            LineNo =Util.GetValueOfString(bomLines[i].GetLine()),
                            Description =Util.GetValueOfString(bomLines[i].GetDescription())
                    });
                }
              

        }        
        /// <summary>
        /// Add BOM Line to this.
        /// Calls addBOMLines if added product is a BOM
        /// </summary>
        /// <param name="line">BOM Line</param>
        /// <param name="qty">quantity</param>
        private void AddBOMLine(Ctx ctx,MProductBOM line, Decimal qty)
        {
            
            // Envs.SetBusyIndicator(true);
           log.Fine(line.ToString());
            String bomType = line.GetBOMType();
            if (bomType == null)
            {
                bomType = MProductBOM.BOMTYPE_StandardPart;
            }
            //
            Decimal lineQty = Decimal.Multiply(line.GetBOMQty(), qty);
            MProduct product = line.GetProduct();
            if (product == null)
            {
                return;
            }
            if (product.IsBOM() && product.IsVerified())
            {

                BomLines(ctx,product, lineQty);

            }
            else
            {
              //  GetDisplay(line.GetM_Product_ID(),
                //    product.GetM_Product_ID(), bomType, product.GetName(), lineQty);
            }

        }
        /// <summary>
        /// Save Selection
        /// </summary>
        /// <returns>true if saved</returns>
        public bool Cmd_Save(Ctx ctx,string[] param,List<BOMLines> lstBOMLines)
        {
            int productID = Util.GetValueOfInt(param[0]);
            int invoiceID = Util.GetValueOfInt(param[1]);
            int orderID = Util.GetValueOfInt(param[2]);
            int projectID = Util.GetValueOfInt(param[3]);
            _selectionList = lstBOMLines;
            if (orderID != null && orderID > 0)
            {
                return Cmd_SaveOrder(ctx,orderID);
            }
            //
            // pp = (KeyNamePair)vcmbInvoiceField.SelectedItem;
            if (invoiceID != null && invoiceID > 0)
            {
                return Cmd_SaveInvoice(ctx,invoiceID);
            }
            //
            // pp = (KeyNamePair)vcmbProjectField.SelectedItem;
            if (projectID != null && projectID > 0)
            {
                return Cmd_SaveProject(ctx,projectID);
            }
            //
            log.Log(Level.SEVERE, "Nothing selected");
            return false;
        }
        /// <summary>
        /// Save to Order
        /// </summary>
        /// <param name="C_Order_ID">id</param>
        /// <returns>true if saved</returns>
        private bool Cmd_SaveOrder(Ctx ctx,int C_Order_ID)
        {
            log.Config("C_Order_ID=" + C_Order_ID);
            MOrder order = new MOrder(ctx, C_Order_ID, null);
            if (order.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "Not found - C_Order_ID=" + C_Order_ID);
                
            }
            int lineCount = 0;
            bool chk = false;
            Decimal qty = 0;
            //	for all bom lines
            if (_selectionList != null)
            {
                bool isQtySet = false;
                for (int i = 0; i < _selectionList.Count; i++)
                {
                    
                    if (_selectionList[i].IsSelected)
                    {
                        int M_Product_ID = Util.GetValueOfInt(_selectionList[i].ProductID);//.intValue();
                        //	Create Line
                        MOrderLine ol = new MOrderLine(order);
                        ol.SetM_Product_ID(Util.GetValueOfInt(_selectionList[i].ProductID));
                        ol.SetQty(Util.GetValueOfInt(_selectionList[i].BOMQty));
                        ol.SetPrice();
                        ol.SetTax();
                        if (ol.Save())
                        {
                            lineCount++;
                        }
                        else
                        {
                            log.Log(Level.SEVERE, "Line not saved");
                        }
                    }	//	line selected
                }	//	for all bom lines
            }
            log.Config("#" + lineCount);           
            return true;
        }
        /// <summary>
        /// Save to Invoice
        /// </summary>
        /// <param name="C_Invoice_ID">id</param>
        /// <returns>true if saved</returns>
        private bool Cmd_SaveInvoice(Ctx ctx,int C_Invoice_ID)
        {
            Decimal qty = 0;
            log.Config("C_Invoice_ID=" + C_Invoice_ID);
            MInvoice invoice = new MInvoice(ctx, C_Invoice_ID, null);
            if (invoice.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "Not found - C_Invoice_ID=" + C_Invoice_ID);
                
            }
            int lineCount = 0;
            bool chk = false;
            //	for all bom lines
            if (_selectionList != null)
            {
                bool isQtySet = false;
                for (int i = 0; i < _selectionList.Count; i++)
                {
                    if (_selectionList[i].IsSelected)
                    {
                        int M_Product_ID = Util.GetValueOfInt(_selectionList[i].ProductID);//.intValue();
                        //	Create Line
                        MInvoiceLine il = new MInvoiceLine(invoice);
                        il.SetM_Product_ID(Util.GetValueOfInt(_selectionList[i].ProductID));
                        il.SetQty(Util.GetValueOfInt(_selectionList[i].BOMQty));
                        il.SetPrice();
                        il.SetTax();
                        if (il.Save())
                        {
                            lineCount++;
                        }
                        else
                        {
                            log.Log(Level.SEVERE, "Line not saved");
                        }
                    }	//	line selected
                }	//	for all bom lines

                log.Config("#" + lineCount);
            }            

            return true;
        }
        /// <summary>
        /// Save to Project
        /// </summary>
        /// <param name="C_Project_ID">id</param>
        /// <returns>true if saved</returns>
        private bool Cmd_SaveProject(Ctx ctx,int C_Project_ID)
        {
            Decimal qty = 0;        
            log.Config("C_Project_ID=" + C_Project_ID);
            MProject project = new MProject(ctx, C_Project_ID, null);
            if (project.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "Not found - C_Project_ID=" + C_Project_ID);
                
            }
            int lineCount = 0;
            bool chk = false;
            //	for all bom lines
            if (_selectionList != null)
            {
                bool isQtySet = false;
                for (int i = 0; i < _selectionList.Count; i++)
                {
                    if (_selectionList[i].IsSelected)
                    {
                        int M_Product_ID = Util.GetValueOfInt(_selectionList[i].ProductID);//.intValue();
                        //	Create Line
                        MProjectLine pl = new MProjectLine(project);
                        pl.SetM_Product_ID(Util.GetValueOfInt(_selectionList[i].ProductID));
                        pl.SetPlannedQty(Util.GetValueOfInt(_selectionList[i].BOMQty));
                        if (pl.Save())
                        {
                            lineCount++;
                        }
                        else
                        {
                            log.Log(Level.SEVERE, "Line not saved");
                        }
                    }	//	line selected
                }	//	for all bom lines

                log.Config("#" + lineCount);
            }          
            return true;
        }


        private string GetBOMType(string BOMTypeValue)
        {
            string sql = "select lst.Name from ad_ref_list  lst "
                       + " INNER JOIN ad_reference re"
                       + " ON re.ad_reference_id=lst.ad_reference_id"
                       + " where re.name='M_Product BOM Product TypeX' and lst.value='" + BOMTypeValue + "'";
            return Util.GetValueOfString(DB.ExecuteScalar(sql));
        }
    }
    //*******************
    //Properties Classes for VBOM Drop
    //*******************
    public class BOMDrop
    {
        public List<Product> Product { get; set; }
        public List<Invoice> Invoice { get; set; }
        public List<Order> Order { get; set; }
        public List<Opportunity> Opportunity { get; set; }
    }
    public class Product
    {
        public int ID { get; set; }
        public string Value { get; set; }
       
    }
    public class Invoice
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
    public class Order
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
    public class Opportunity
    {
        public int ID { get; set; }
        public string Value { get; set; }
    }
    public class BOMLines
    {
        public string BOMType { get; set; }
        public string BOMTypeName { get; set; }
        public string BOMQty { get; set; }
        public string ProductID { get; set; }
        public string ProductBOMID { get; set; }
        public string ProductName { get; set; }
        public string LineNo { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }

}
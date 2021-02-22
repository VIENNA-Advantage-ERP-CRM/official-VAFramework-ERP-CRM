using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Logging;

namespace VAdvantage.VOS
{

    public class ProductInfo : NamePair
    {

        #region Private Variables
        //private static long SERIALVERSIONUID = 1L;
        public int VAM_Product_ID;
       // public String Name;
        public String title;
        public String groupName;
        public String bomType;
        public String quantity;
        public String supplyType;
        public String fieldIdentifier; // identifer composed of bom component ID + bom level
        public int VAM_BOMVAMProduct_ID;
        public int operationSeqNo;
        private static VLogger log = VLogger.GetVLogger(typeof(ProductInfo).FullName);
        #endregion

        public ProductInfo() { }

        /// <summary>
        /// ProductInfo
        /// </summary>
        /// <param name="newVAM_Product_ID"></param>
        /// <param name="newName"></param>
        /// <param name="newTitle"></param>
        /// <param name="newGroupName"></param>
        /// <param name="newBomType"></param>
        /// <param name="newQuantity"></param>
        /// <param name="newSupplyType"></param>
        /// <param name="newVAM_BOMVAMProduct_ID"></param>
        /// <param name="newOperationSeqNo"></param>
        /// <param name="newFieldIdentifier"></param>
        public ProductInfo(int newVAM_Product_ID,
            String newName, String newTitle, String newGroupName, String newBomType, String newQuantity,
            String newSupplyType, int newVAM_BOMVAMProduct_ID, int newOperationSeqNo, String newFieldIdentifier)
            : base(newName)
        {
            VAM_Product_ID = newVAM_Product_ID;
            Name = newName;
            title = newTitle;
            groupName = newGroupName;
            bomType = newBomType;
            quantity = newQuantity;
            supplyType = newSupplyType;
            VAM_BOMVAMProduct_ID = newVAM_BOMVAMProduct_ID;
            operationSeqNo = newOperationSeqNo;
            fieldIdentifier = newFieldIdentifier;
        }

        public ProductInfo(int newVAM_Product_ID,
                String newName, String newTitle, String newGroupName, String newBomType, String newQuantity,
                String newSupplyType, int newOperationSeqNo, String newFieldIdentifier)
            : this(newVAM_Product_ID,
                newName, newTitle, newGroupName, newBomType, newQuantity,
                newSupplyType, 0, newOperationSeqNo, newFieldIdentifier)
        {

        }

        public int GetOperationSeqNo()
        {
            return operationSeqNo;
        }

        public int GetVAM_BOMVAMProduct_ID()
        {
            return VAM_BOMVAMProduct_ID;
        }

        /// <summary>
        /// to String
        /// </summary>
        /// <returns>infoint</returns>
        public override String ToString()
        {
            return Name;
        }

        public override String GetID()
        {
            // TODO Auto-generated method stub
            return Convert.ToString(VAM_Product_ID);
        }

        public int GetVAM_Product_ID()
        {
            return VAM_Product_ID;
        }

        public String GetGroupName()
        {
            return groupName;
        }

        public String GetTitle()
        {
            return title;
        }

        public String GetBomType()
        {
            return bomType;
        }

        public String GetQuantity()
        {
            return quantity;
        }

        public String GetSupplyType()
        {
            return supplyType;
        }

        public String GetFieldIdentifier()
        {
            return fieldIdentifier;
        }

        public override object GetKeyID()
        {
            log.SaveError("GetKeyID", "GetKeyID fuction doesn't contain code");
            return null;
        }
    }
}

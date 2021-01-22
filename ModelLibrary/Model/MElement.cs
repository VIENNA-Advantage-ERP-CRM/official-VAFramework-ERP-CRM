/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MElement
 * Purpose        : Accounting Element model.
 * Class Used     : MElement inherits from X_C_Element class
 * Chronological    Development
 * Raghunandan      08-May-2009
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
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MElement : X_C_Element
    {
        //Cache						
        private static CCache<int, MElement> s_cache = new CCache<int, MElement>("VAF_ColumnDic", 20);
        // Tree Used		
        private X_VAF_TreeInfo _tree = null;

        /// <summary>
        ///Get Accounting Element from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_ColumnDic_ID">id</param>
        /// <returns>MElement</returns>
        public static MElement Get(Ctx ctx, int VAF_ColumnDic_ID)
        {
            int key = (int)VAF_ColumnDic_ID;
            MElement retValue = (MElement)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MElement(ctx, VAF_ColumnDic_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        ///Standard Accounting Element Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Element_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MElement(Ctx ctx, int C_Element_ID, Trx trxName)
            : base(ctx, C_Element_ID, trxName)
        {
            if (C_Element_ID == 0)
            {
                //	setName (null);
                //	setVAF_TreeInfo_ID (0);
                //	setElementType (null);	// A
                SetIsBalancing(false);
                SetIsNaturalAccount(false);
            }
        }

        /// <summary>
        ///	Accounting Element Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MElement(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        ///Full Constructor
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="Name">name</param>
        /// <param name="ElementType">type</param>
        /// <param name="VAF_TreeInfo_ID">tree</param>
        public MElement(MClient client, string name, string elementType, int VAF_TreeInfo_ID)
            : this(client.GetCtx(), 0, client.Get_TrxName())
        {
            SetClientOrg(client);
            SetName(name);
            SetElementType(elementType);	// A
            SetVAF_TreeInfo_ID(VAF_TreeInfo_ID);
            SetIsNaturalAccount(ELEMENTTYPE_Account.Equals(elementType));
        }

        /// <summary>
        ///Get Tree
        /// </summary>
        /// <returns>tree</returns>
        public X_VAF_TreeInfo GetTree()
        {
            if (_tree == null)
                _tree = new X_VAF_TreeInfo(GetCtx(), GetVAF_TreeInfo_ID(), Get_TrxName());
            return _tree;
        }

        /// <summary>
        ///	Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            String elementType = GetElementType();
            //	Natural Account
            if (ELEMENTTYPE_UserDefined.Equals(elementType) && IsNaturalAccount())
                SetIsNaturalAccount(false);
            //	Tree validation
            X_VAF_TreeInfo tree = GetTree();
            if (tree == null)
                return false;
            String treeType = tree.GetTreeType();
            if (ELEMENTTYPE_UserDefined.Equals(elementType))
            {
                if (X_VAF_TreeInfo.TREETYPE_User1.Equals(treeType) || X_VAF_TreeInfo.TREETYPE_User2.Equals(treeType))
                {
                    ;
                }
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@TreeType@ <> @ElementType@ (U)"), false);
                    return false;
                }
            }
            else
            {
                if (!X_VAF_TreeInfo.TREETYPE_ElementValue.Equals(treeType))
                {
                   log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@TreeType@ <> @ElementType@ (A)"), false);
                    return false;
                }
            }
            return true;
        }

    }
}

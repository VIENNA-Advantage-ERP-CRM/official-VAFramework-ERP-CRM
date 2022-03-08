using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace BaseLibrary.Model
{

    public interface IModelFactory
    {

        Type GetClass(String tableName);


        PO GetPO(String tableName, Ctx ctx, int Record_ID, Trx trxName);


        PO GetPO(String tableName, Ctx ctx, DataRow dr, Trx trxName);
    }

}
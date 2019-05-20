using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using VAdvantage.DataBase;

namespace VAdvantage.Server
{
    [ServiceContract(Namespace = "")]
    public interface IDSPosting
    {
        [OperationContract]
        String PostImmediate(IDictionary<string, string> ctx, int AD_Client_ID, int AD_Table_ID, int Record_ID, bool force, Trx trxName);

        [OperationContract]
        bool OnServer(IDictionary<string, string> ctxDic, int AD_Client_ID, string applicationURL,
           List<string> processOption, bool allChecked, bool traceFile, string accesskey);

        [OperationContract]
        bool StopServer(bool req, int AD_Client_ID, string applicationURL, string accesskey);

        [OperationContract]
        List<String> IsServerRun(int AD_Client_ID, string applicationURL, string accesskey);

        [OperationContract]
        string GetCloudURL();

        [OperationContract]
        bool InvokeIISService();
    }
}

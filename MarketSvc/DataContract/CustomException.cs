using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MarketSvc
{
    [DataContract]
    public class CustomException
    {

        [DataMember(Order = 0)]
        public string Message { get; set; }
        [DataMember(Order = 1)]
        public CustomException InnerException;

        public CustomException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            this.Message = ex.Message + ex.StackTrace;
        }
        public Exception ToException()
        {
            Exception e;
            CustomException ce = this;
            if (ce.InnerException != null)
            {
                Exception inner = ce.InnerException.ToException();
                e = new Exception(ce.Message, inner);
            }
            else
                e = new Exception(ce.Message);
            return e;
        }
    }

}

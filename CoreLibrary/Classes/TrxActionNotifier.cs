using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Classes
{



    public interface ITrxAction
    {
        bool OnRollBack(string trxName);
        bool OnCommit(string trxName);
        bool OnClose(string trxName);
    }

    public sealed class TrxActionNotifier
    {

        private static TrxActionNotifier _single = null;
        public static TrxActionNotifier Get()
        {
            if (_single == null)
                _single = new TrxActionNotifier();
            return _single;
        }

        private TrxActionNotifier()
        {
        }

        public void Register(ITrxAction observer)
        {
            if (!_listner.Contains(observer))
                _listner.Add(observer);
        }


        public List<ITrxAction> _listner = new List<ITrxAction>();

        public bool OnClose(string trxName)
        {
            foreach (ITrxAction action in _listner)
            {
                action.OnClose(trxName);
            }
            return true;
        }

        public bool OnCommit(string trxName)
        {
            foreach (ITrxAction action in _listner)
            {
                action.OnCommit(trxName);
            }
            return true;
        }

        public bool OnRollBack(string trxName)
        {
            foreach (ITrxAction action in _listner)
            {
                action.OnRollBack(trxName);
            }
            return true;
        }
    }
}
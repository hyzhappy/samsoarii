using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    public class BrowsingHistoryManager
    {
        public Stack<Tuple<int, List<int>>> backHistoryStack;
        public Stack<Tuple<int, List<int>>> aheadHistoryStack;
        public BrowsingHistoryManager()
        {
            backHistoryStack = new Stack<Tuple<int, List<int>>>();
            aheadHistoryStack = new Stack<Tuple<int, List<int>>>();
        }
        public void ClearAheadHistoryStack()
        {
            aheadHistoryStack.Clear();
        }
        public void aheadHistoryStackPush(Tuple<int, List<int>> status)
        {
            aheadHistoryStack.Push(status);
        }
        public void backHistoryStackPush(Tuple<int, List<int>> oldStatus)
        {
            backHistoryStack.Push(oldStatus);
        }
        public Tuple<int, List<int>> backHistoryStackPop()
        {
            return backHistoryStack.Pop();
        }
        public Tuple<int, List<int>> aheadHistoryStackPop()
        {
            return aheadHistoryStack.Pop();
        }
        public bool CanBack()
        {
            return backHistoryStack.Count > 0;
        }
        public bool CanAhead()
        {
            return aheadHistoryStack.Count > 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MatchingEngine
{
    public static class Extensions
    {
        public static void Replace<T>(this LinkedListNode<T> toReplaceNode, LinkedListNode<T> replacerNode)
        {
            var lkList = toReplaceNode.List;
            lkList.AddBefore(toReplaceNode, replacerNode);
            lkList.Remove(toReplaceNode);
        }
    }
}

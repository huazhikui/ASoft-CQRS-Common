using System;
namespace ASoft.Model
{
   public  interface ITreeNode
    {
        bool Checked { get; set; }
        System.Collections.Generic.List<ITreeNode> Child { get; set; }
        DateTime CreateDate { get; set; }
        bool Expanded { get; set; }
        string IconCls { get; set; }
        string Id { get; set; }
        bool IsLeaf { get; set; }
        short Lvl { get; set; }
        ITreeNode Parent { get; set; }
        string ParentId { get; set; }
        string Path { get; set; }
        short Sort { get; set; }
        string Title { get; set; }
        string Url { get; set; }
    }
}

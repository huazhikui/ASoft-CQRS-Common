using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Db;

namespace ASoft.Model
{
    public enum NodeRelationship
    {
        Parent = 0,
        Child = 1,
        Brother = 2,
        BrotherParent = 3,
        BrotherChild = 4
    }
    public class TreeNode
    {
        #region

        /// <summary>
        /// 根据PATH判断是否是是指定ID的子节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsMyChild(String id, String path)
        {
            return path.IndexOf(String.Format(".{0}.", id)) > -1;
        }

        /// <summary>
        /// 判断PATH是否以ID结尾（）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsMyself(String id, String path)
        {
            return path.EndsWith(String.Format(".{0}", id));
        }

        /// <summary>
        /// 获取父节点的PATH
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String GetParentPath(String path)
        {
            var lastIndex = path.LastIndexOf('.');
            if (lastIndex > -1)
            {
                return path.Substring(0, lastIndex);
            }
            return "";
        }

        public static String GetId(String path)
        {
            var lastIndex = path.LastIndexOf('.');
            if (lastIndex == -1)
            {
                return path;
            }
            else
            {
                return path.Substring(lastIndex + 1);
            }
        }

        public static int GetLvlByPath(String path)
        {
            var arr = path.Split('.');
            if (arr != null && arr.Length > 0)
            {
                return arr.Length - 1;
            }
            return 0;
        }
        #endregion

        /// <summary>
        /// 返回父节点ID的数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String[] GetParentIDs(String path)
        {
            String[] nodePaths = path.Split('.');
            if (nodePaths != null)
            {
                var count = nodePaths.Length;
                //倒叙后跳过第一个
                return nodePaths.Reverse().Skip(1).ToArray();
            }
            return null;
        }

        public static String GetParentIDByPath(String path)
        {
            String[] ids = GetParentIDs(path);
            if (ids != null)
            {
                return ids[0];
            }
            return null;
        }




        /// <summary>
        /// 返回路径中的所有ID
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String[] GetPathIDs(String path)
        {
            String[] nodePaths = path.Split('.');
            return nodePaths;
        }

        public static NodeRelationship Compute(String path, String nodeId)
        {
            String[] nodePaths = path.Split('.');
            if (nodePaths != null)
            {
                int lvl = nodePaths.Length - 1;
                foreach (var item in nodePaths)
                {

                }
            }

            return NodeRelationship.Brother;
        }

        /// <summary>
        /// 从nodePaths中获取离nodePath最近的节点的索引 , 搜索方式 平级->父级->平级->父级
        /// </summary>
        /// <param name="nodePath"></param>
        /// <param name="nodePaths"></param>
        /// <returns></returns>
        public static int GetShortestIndex(String nodePath, String[] nodePaths)
        {
            String result = "";
            int index = -1;
            if (nodePaths != null && nodePath.IndexOf(".") > -1)
            {
                int length = nodePaths.Length;
                String nodeParentPath = GetParentPath(nodePath);
                if (!String.IsNullOrEmpty(nodeParentPath))
                {
                    String[] nodeParentPaths = new String[length];
                    for (var i = 0; i < length; i++)
                    {
                        //nodeParentPaths[i] = GetParentPath(nodePaths[i]);
                        if (nodePaths[i].StartsWith(nodeParentPath))
                        {
                            index = i;
                            return index;
                        }
                        //if (nodeParentPath.StartsWith(nodeParentPaths[i]))
                        //{
                        //    index = i;
                        //    return index;
                        //}
                    }
                    if (index == -1)
                    {
                        return GetShortestIndex(nodeParentPath, nodePaths);
                    }
                }
            }
            return index;
        }
    }


    public class TreeNode<E> : BaseModel
         where E : ASoft.Model.TreeNode<E>, new()
    {

        [DataProperty(IsIdentifier = true, IsPrimaryKey = true)]
        public virtual String Id { set; get; }

        public virtual String Title { set; get; }

        public virtual String Url { set; get; }

        public virtual String IconCls { set; get; }

        //public String IconCls { set; get; }
        [DataProperty(Field = "PARENT_ID")]
        public virtual String ParentId { set; get; }
        [DataProperty]
        public virtual short Sort { set; get; }
        [DataProperty]
        public virtual short Lvl { set; get; }
        [DataProperty(Field = "IS_LEAF")]
        public virtual bool IsLeaf { set; get; }
        [DataProperty(Field = "CREATE_DATE" )]
        public virtual DateTime CreateDate { set; get; } 
        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "EXPANDED")]
        public virtual bool Expanded { set; get; }

        /// <summary>
        /// 节点路径 path =  parent.Path+ entity.Sort.ToString() 
        /// </summary>
        [DataProperty(Field = "PATH")]
        public virtual string Path { set; get; }
        public bool Checked { set; get; }
        public List<E> Child { set; get; }
        public E Parent { set; get; }
    }
}

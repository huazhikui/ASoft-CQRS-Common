using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Db;
using ASoft.Dal;
using ASoft.Model;
using ASoft.Text;

namespace ASoft.Dal
{
    public class TreeDal<E> : BaseDal<E>
           where E : ASoft.Model.TreeNode<E>, new()
    {
        /// <summary>
        /// 获取菜单节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public E Get(String id)
        {
            E entity = new E();
            entity.Id = id;
            db.LoadObject(entity);
            return entity;
        }


        /// <summary>
        /// 根据ID获取菜单节点，并可根据深度加载子级，默认深度0
        /// </summary>
        /// <param name="id"></param>
        /// <param name="depth">深度（递归次数） 0：加载1级子级，-1：加载所有子级，-2：不加载子级</param>
        /// <returns></returns>
        public E Get(string id, short depth = 0)
        {
            E result = this.Get(id);
            if (depth > -2)
            {
                this.GetChild(result, depth);
            }
            return result;
        }

        public String[] GetPaths(String[] orgIDs)
        {
            List<String> paths = new List<string>();
    
            DataCommand command = this.db.CreateSQLCommand("select * from SYS_ORGAN where org_id in ({0})", orgIDs);
            using (DataReader dr = db.ExecuteReader(command))
            {
                while (dr.Read()) {
                    paths.Add(dr.GetString("path"));
                }
                dr.Close();
            }

            return paths.ToArray(); 
        }

        public override E Insert(E entity)
        {
            this.Insert(entity, entity.ParentId);
            return entity;
        }

        /// <summary>
        /// 插入一条新记录，并作为指定ID的节点的子项（末尾）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public E Insert(E entity, String parentId)
        {
            if (String.IsNullOrEmpty(parentId))
            {
                parentId = "0";
            }
            entity.Parent = this.Get(parentId);
            this.Insert(entity, (entity.Parent) as E);
            return entity;
        }

        /// <summary>
        /// 插入一条新记录，并作为指定节点的子项（末尾）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public void Insert(E entity, E parent)
        {
            List<String> sqls = new List<string>();
            entity.Id = Sequence.GetInstance(this.baseDb).NextLuhmValue;
            if (entity.Parent != null)
            {
                entity.Lvl = (short)(entity.Parent.Lvl + 1);
                if (entity.Sort == 0)
                {
                    entity.Sort = (short)(GetSortByParent(parent.Id) + 1);
                    entity.Path = String.Format("{0}.{1}", parent.Path, entity.Id);
                }
                entity.IsLeaf = true;
                entity.ParentId = parent.Id;
                //更新父节点
                if (entity.Parent.IsLeaf)
                {
                    entity.Parent.IsLeaf = false;
                    (entity.Parent as BaseModel).PropertyUpdate("IsLeaf");
                    sqls.Add(db.GetSqlByUpdate((entity.Parent as BaseModel)));
                }
            }
            entity.CreateDate = DateTime.Now;
            sqls.Add(db.GetSqlByInsert(entity));
            db.ExecuteTransaction(sqls);
        }

        /// <summary>
        /// 更新节点，如果有子菜单则更新子节点的级别
        /// </summary>
        /// <param name="e"></param>
        public void Update(E e)
        {
            List<String> sqls = new List<string>();
            try
            {
                var old = this.Get(e.Id);
                e.Path = old.Path;
                e.IsLeaf = old.IsLeaf;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("指定的节点不存在,Id:{0}", e.Id));
            }
            var oldE = this.Get(e.Id);
            var oldPath = oldE.Path;
            var oldParentPath = TreeNode.GetParentPath(oldPath);
            var oldParentId = TreeNode.GetId(oldParentPath);
            //父节点发生改变
            if (e.ParentId != oldParentId)
            {
                //获取节点新的父节点
                E parent = this.Get(e.ParentId);
                //节点新的路径
                e.Path = String.Format("{0}.{1}", parent.Path.Replace("." + e.Id + ".", "."), e.Id);
                //更新节点新的父节点是被其原先子节点，则置换
                if (TreeNode.IsMyChild(e.Id, parent.Path))
                {
                    sqls.Add(String.Format("update {0} set Is_Leaf=0 where {1}='{2}'", this.Table, this.KeyField, e.Id));
                    sqls.Add(String.Format("update {0} set parent_id={2} where parent_id='{3}'", this.Table, oldParentPath, oldParentId, e.Id));
                    //更新历史子节点的路径
                    sqls.Add(String.Format("update {0} set path=replace(path,'.{1}.','.'), lvl=lvl-1 where path like '%.{1}.%'", this.Table, e.Id));

                    e.IsLeaf = true;
                }
                else
                {
                    //更新子节点的级别
                    if (e.IsLeaf == false && ExistsChild(e.Id))
                    {
                        //更新子节点的路径
                        sqls.Add(String.Format("update {0} set path='{1}.'||{2}, lvl=lvl-1 where path like '%.{3}.%'", this.Table, e.Path, this.KeyField, e.Id));
                        sqls.Add(String.Format("update {0} set lvl=nvl(length(translate(path,'#0123456789','#')),0) where path like '%.{1}.%'", this.Table, e.Id));
                    }
                }
                //更新新的父节点的叶子状态
                if (!String.IsNullOrEmpty(e.ParentId))
                {
                    sqls.Add(String.Format("update {0} set is_leaf=0 where {1}='{2}'", this.Table, this.KeyField, e.ParentId));
                }
            }
            e.Lvl = Convert.ToInt16(TreeNode.GetLvlByPath(e.Path));
            e.PropertyUpdate("Path");
            e.PropertyUpdate("ParentId");
            e.PropertyUpdate("Lvl");
            e.PropertyUpdate("IsLeaf");
            e.PropertyUpdate("Sort");
            e.PropertyUpdate("Title");
            sqls.Add(db.GetSqlByUpdate(e));
            //更新节点
            db.ExecuteTransaction(sqls);
            //判断老的父节点是否是叶子如果是则更新状态
            if (!this.ExistsChild(oldParentId))
            {
                db.ExecuteNonQuery(String.Format("update {0} set is_leaf=1 where {1}='{2}'", this.Table, this.KeyField, oldParentId));
            }
        }

        /// <summary>
        /// 获取子项的Sort最大值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public short GetSortByParent(String id)
        {
            DataCommand  sql = this.db.CreateSQLCommand("select max(sort) from "+this.Table+" where parent_id={0}", id);
            using (ScalerResult sr = db.ExecuteScalar(sql))
            {
                if (sr.IsDBNull)
                {
                    return 0;
                }
                else
                {
                    return (short)sr.IntValue;
                }
            }
        }

        /// <summary>
        /// 获取叶子
        /// </summary>
        /// <returns></returns>
        public List<E> GetByLeaf()
        {
            List<E> result = this.Where("isleaf =1 and state=0")
                                       .OrderBy(e => e.Sort)
                                       .ToList<E>();
            return result;
        }

        public List<E> GetByLvl(int lvl)
        {
            List<E> result = this.Where("lvl={0} and state=0", lvl)
                                       .OrderBy(e => e.Sort)
                                       .ToList<E>();
            return result;
        }


        /// <summary>
        /// 是否存在子菜单 true:有子菜单 false:无子菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsChild(String id)
        {

            DataCommand sql = this.db.CreateSQLCommand("select count(1) from "+this.Table+" where parent_id={0}", id);
            using (ScalerResult sr = db.ExecuteScalar(sql))
            {
                if (sr.IsDBNull)
                {
                    return false;
                }
                else
                {
                    return sr.IntValue > 0;
                }
            }
        }

        /// <summary>
        /// 根据ID获取子项
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public List<E> GetChild(String id)
        {
            List<E> result = this.Where("PARENT_ID = '{0}'", id);
            return result;
        }

        /// <summary>
        /// 根据深度，获取子项
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="depth">深度（递归次数）：0则只返回一级子集, -1返回所有子级</param> 
        /// <returns>树形菜单列表</returns>
        public List<E> GetChild(E parent, short depth)
        {
            List<E> result = null;
            if (String.IsNullOrEmpty(parent.Id))
            {
                return result;
            }
            result = this.GetChild(parent.Id);
            //遍历结果并递归加载子项
            result.ForEach(delegate(E e)
            {
                e.Parent = parent;
                //判断是否加载子级
                if ((depth == -1 || depth > 0))
                {
                    if (depth > 0)
                    {
                        e.Child = this.GetChild(e, (short)(depth - 1)).ToList<E>();
                    }
                    else if (depth == 0)
                    {
                        e.Child = this.GetChild(e, depth).ToList<E>();
                    }
                }
            });
            return result;

        }

        /// <summary>
        /// 获取ID节点的所有子项(列表)
        /// </summary>
        /// <returns></returns>
        public List<E> GetAllChild(String id)
        {
            var organ = this.Get(id);
            return this.Where("Path like {0}%'   order by sort", organ.Path+".");
        }


        ///// <summary>
        /////  获取ID节点的所有子项(树形)
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="filterIds"></param>
        ///// <returns>指定ID的子项列表（树形）</returns>
        //public List<E> GetAllChildTree(String id)
        //{
        //    List<E> result = new List<E>();
        //    List<E> childNode = null;
        //    id = String.IsNullOrEmpty(id) ? "0" : id;
        //    E parent = this.Get(id);
        //    if (String.IsNullOrEmpty(id))
        //    {
        //         childNode  =this.Where(" 1= 1 order by sort");
        //    }
        //    else
        //    {
        //         childNode  =this.Where(String.Format(" path like '%{0}.%' order by sort", parent.Path)); 
        //    }  
        //    childNode = BuildChildTree(childNode, "0", -1);
        //    parent.Child = childNode;
        //    result.Add(parent);
        //    return result;
        //}




        /// <summary>
        ///  获取ID节点的所有子项(树形)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filterIds"></param>
        /// <returns>指定ID的子项列表（树形）</returns>
        public List<E> GetAllChildTree(String id)
        {
            List<E> result = null;
            DataCommand sql;

            if (String.IsNullOrEmpty(id))
            {
                 sql = this.db.CreateSQLCommand("SELECT * FROM "+this.Table+" where (Parent_ID is null or Parent_ID ='')   order by sort ");
            }
            else
            {
                  sql = this.db.CreateSQLCommand("SELECT * FROM "+this.Table+" WHERE Parent_ID = {0}   order by sort", id);
            }
            using (DataReader dr = db.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    if (result == null)
                    {
                        result = new List<E>();
                    }
                    E entity = db.SetObjectProperty<E>(dr);

                    if (entity.IsLeaf != true)
                    {
                        var query = this.GetAllChildTree(entity.Id);
                        if (query != null && query.Any())
                        {
                            entity.Child = query.ToList<E>();
                        }
                    }
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }
         


        /// <summary>
        ///  获取ID节点的树形子项，并返回筛选指定范围的节点(树形)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filterIds">筛选条件</param>
        /// <returns>指定ID的子项列表（树形）</returns>
        public List<E> GetAllChildTree(String id, String[] filterIds)
        {
            List<E> result = null;
            DataCommand sql;
          
            if (String.IsNullOrEmpty(id))
            {
                  sql = this.db.CreateSQLCommand("SELECT * FROM "+this.Table+" where (Parent_ID is null or Parent_ID ='') and Menu_ID in ({0}) order by sort ", filterIds);
            }
            else
            {
                  sql = this.db.CreateSQLCommand("SELECT * FROM "+this.Table+" WHERE Parent_ID = {0} and Menu_ID in ({1}) order by sort", id, filterIds);
            }
            using (DataReader dr = db.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                    if (entity.IsLeaf != true)
                    {
                        entity.Child = this.GetAllChildTree(entity.Id, filterIds).ToList<E>();
                    }
                    result.Add(entity);
                }

                dr.Close();
            }
            return result;
        }

        public List<E> GetChild(String[] roleIds, short depth)
        {
            return this.GetChild(roleIds, null, depth);
        }
        /// <summary>
        /// 根据父节点的ID设置新的菜单级别
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="lvl"></param>
        /// <returns></returns>
        public bool SetLvlByParent(String parentId, short lvl)
        {
            DataCommand sql = this.db.CreateSQLCommand("update "+this.Table+" set lvl={0} where parent_id={1}", lvl, parentId);
            return db.ExecuteNonQuery(sql).Value > 0;
        }


        #region 根据权限获取树形节点
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="parentId"></param>
        /// <param name="depth">深度（递归次数）：0则只返回一级子集, -1返回所有子级</param> 
        /// <returns></returns>
        public virtual List<E> GetChild(String[] roleIds, String parentId, short depth)
        {
            List<E> result = null;
            var input = this.GetChildByRoles(roleIds, parentId);
            result = BuildChildTree(input, parentId, depth);
            return result;
        }

        public virtual List<E> GetChildList(String[] roleIds, String parentId)
        {
            List<E> result = this.GetChildByRoles(roleIds, parentId); 
  
            return result;
        }

        public List<E> BuildChildTree(List<E> input, String parentID, short depth)
        {
            List<E> result = null;
            if (input == null)
            {
                return input;
            }
           // var query = input.Where(item => item.ParentId == parentID || item.Path.StartsWith(parentID));
            var query = input.Where(item => item.ParentId == parentID);
            //;
            if (query != null && query.Any())
            {
                result = query.ToList();
            }
            if (result != null)
            {
                //遍历结果并递归加载子项
                result.ForEach(delegate(E e)
                {
                    //判断是否加载子级
                    if ((depth == -1 || depth > 0))
                    {
                        if (depth > 0)
                        {
                            e.Child = this.BuildChildTree(input, e.Id, (short)(depth - 1));
                        }
                        else
                        {
                            e.Child = this.BuildChildTree(input, e.Id, depth);
                        }
                    }
                });
            } 
            return result;
        } 
       

        /// <summary>
        /// 根据角色获取菜单
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public List<E> GetChild(String[] roleIds, String parentId)
        {
            List<E> result = null;
            DataCommand sql;
      
            if (String.IsNullOrEmpty(parentId))
            {
                sql = this.db.CreateSQLCommand("select *  from "+this.Table+" where  (Parent_ID is null or Parent_ID ='')  and exists (select * from SYS_PRIVILEGE t where  t.privilege_access = 'SYS_MENU' and t.privilege_access_key = sys_menu.menu_id  and t.privilege_master='SYS_ROLE' and t.privilege_master_key in ({1})) order by sort", parentId, roleIds);
            }
            else
            {
                sql = this.db.CreateSQLCommand("select *  from "+this.Table+" where parent_id={0}   and exists (select * from SYS_PRIVILEGE t where  t.privilege_access = 'SYS_MENU' and t.privilege_access_key = sys_menu.menu_id  and t.privilege_master='SYS_ROLE' and t.privilege_master_key in ({1})) order by sort", parentId, roleIds);
            }
            using (DataReader dr = db.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    if (result == null)
                    {
                        result = new List<E>();
                    }
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }


        public List<E> GetChildByRoles(String[] roleIds, String parentId)
        {
            List<E> result = null;
            DataCommand sql;
            if (!this.Exists(parentId))
            {
                return result;
            }
            var parent = this.Get(parentId);
      
            //包含系统管理员角色
            if (roleIds.Contains(Config.GetAppSettings("ASoft.SystemAdministrator.RoleID")))
            {
                  sql = this.db.CreateSQLCommand("select *  from "+this.Table+ " where   Instr(SYS_MENU.path, {0}) > 0 order by sort", parent.Path+".", roleIds);
            }
            else
            {

                  sql = this.db.CreateSQLCommand("select *  from "+this.Table+" where   Instr(SYS_MENU.path, {0}) > 0   and exists (select * from SYS_PRIVILEGE t where  t.privilege_access = 'SYS_MENU' and t.privilege_access_key = sys_menu.menu_id  and t.privilege_master='SYS_ROLE' and t.privilege_master_key in ({1})) order by sort", parent.Path+".", roleIds);
            }
            using (DataReader dr = baseDb.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    if (result == null)
                    {
                        result = new List<E>();
                    }
                    E entity = baseDb.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// 返回节点的全名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public String GetFullName(String id)
        {
            String result = "";
            var node = this.Get(id);
            if (node != null)
            {
                String[] ids = TreeNode.GetPathIDs(node.Path);
                if (ids != null)
                {
                    foreach (var item in ids)
                    {
                        if (item != id)
                        {
                            result += this.Get(item).Title;
                        }
                        else
                        {
                            result += node.Title;
                        }
                    }
                }
            }
            return result;
        }

        #endregion


        #region


        #endregion

        #region 其他
        /// <summary>
        /// 设置tree的checked状态
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="checkIds"></param>
        public static void SetNodeChecked(List<E> tree, String[] checkIds)
        {
            if (tree != null && checkIds != null && checkIds.Length > 0)
            {
                tree.ForEach(delegate(E e)
                {
                    if (checkIds.Contains(e.Id))
                    {
                        e.Checked = true;
                    }
                    if (e.Child != null)
                    {
                        SetNodeChecked(e.Child, checkIds);
                    }
                });
            }
        }
        #endregion
    }
}

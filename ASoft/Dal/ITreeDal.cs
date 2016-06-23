using System;
namespace ASoft.Dal
{
    interface ITreeDal<E>
     where E : ASoft.Model.TreeNode<E>, new()
    {
        System.Collections.Generic.List<E> BuildChildTree(System.Collections.Generic.List<E> input, string parentID, short depth);
        bool ExistsChild(string id);
        E Get(string id);
        E Get(string id, short depth = 0);
        System.Collections.Generic.List<E> GetAllChild(string id);
        System.Collections.Generic.List<E> GetAllChildTree(string id);
        System.Collections.Generic.List<E> GetAllChildTree(string id, string[] filterIds);
        System.Collections.Generic.List<E> GetByLeaf();
        System.Collections.Generic.List<E> GetByLvl(int lvl);
        System.Collections.Generic.List<E> GetChild(E parent, short depth);
        System.Collections.Generic.List<E> GetChild(string id);
        System.Collections.Generic.List<E> GetChild(string[] roleIds, short depth);
        System.Collections.Generic.List<E> GetChild(string[] roleIds, string parentId);
        System.Collections.Generic.List<E> GetChild(string[] roleIds, string parentId, short depth);
        string GetFullName(string id);
        short GetSortByParent(string id);
        E Insert(E entity);
        void Insert(E entity, E parent);
        E Insert(E entity, string parentId);
        bool SetLvlByParent(string parentId, short lvl);
        void Update(E e);
    }
}

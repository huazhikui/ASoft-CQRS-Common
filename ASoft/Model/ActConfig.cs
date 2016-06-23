using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Db;
namespace ASoft.Model
{

    public class ActConfig : BaseModel
    {
        [DataProperty(IsIdentifier = true, IsPrimaryKey = true)]
        public String ID { set; get; }

        [DataProperty]
        public String Name { set; get; }

        [DataProperty]
        public String Config { set; get; }

        [DataProperty]
        public String Code { set; get; }

        public static Act ToAct(ActConfig config)
        { 
            Act result = Serialize.JsonDeserilize<Act>(config.Config);
            result.ActDict = new Dictionary<String, Act>();
            result.ActRolesDict = new Dictionary<string, List<string>>();
            result.BCode = config.Code;

            if (result.ActType == ActType.Sequence && result != null && result.Sequence != null)
            {
                var count = result.Sequence.Count();
                //Act pre = null;
                //Act next = null;
                for (var i = 0; i < count; i++)
                {
                    var item = result.Sequence[i];
                    if (i == 0)
                    {
                        item.IsStart = true;
                    }
                    item.BCode = config.Code;
                    item.AutoDispatch = result.AutoDispatch;
                    item.RequireFirstRead = result.RequireFirstRead;
                    if (String.IsNullOrEmpty(item.ApiName))
                    {
                        item.ApiName = result.ApiName;
                    }

                    if (String.IsNullOrEmpty(item.Form))
                    {
                        item.Form = "NormalAction";
                    }
                    if (item.Parallel != null)
                    {
                        //并行活动中最大时限
                        int maxParallelTime = 0;
                        foreach (var pItem in item.Parallel)
                        {
                            pItem.BCode = config.Code;
                            if (String.IsNullOrEmpty(pItem.ApiName))
                            {
                                pItem.ApiName = result.ApiName;
                            }
                            if (String.IsNullOrEmpty(pItem.Form))
                            {
                                pItem.Form = "NormalAction";
                            }
                            maxParallelTime = maxParallelTime < pItem.TimeLimit ? pItem.TimeLimit : maxParallelTime;
                            result.ActDict.Add(pItem.Code, pItem);
                            if (pItem.Switch != null)
                            {
                                pItem.Router = new Dictionary<int, Act>();
                                pItem.SwitchDict = new Dictionary<int, Router>();
                                foreach (var router in pItem.Switch)
                                {
                                    pItem.RoleIDs = pItem.RoleIDs == null ? new List<String>() : pItem.RoleIDs;

                                    if (router.RoleIDs != null)
                                    {
                                        foreach (var routeRole in router.RoleIDs)
                                        {
                                            if (!pItem.RoleIDs.Contains(routeRole))
                                            {
                                                pItem.RoleIDs.Add(routeRole);
                                            }
                                        }
                                    }
                                    //设置路由字典
                                    pItem.SwitchDict[router.ID] = router;
                                    pItem.Router[router.ID] = result.Sequence.Where(a => a.Code == router.RouterTo).First();
                                    router.OutRouter = router.Method;
                                    if (router.TableActityOfRouters != null)
                                    {
                                        foreach (var myTableActity in router.TableActityOfRouters)
                                        {
                                            if (String.IsNullOrEmpty(myTableActity.TableName) || myTableActity.SynchCaseState)
                                            {
                                                myTableActity.TableName = result.Table;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        result.SumTimeLimit += maxParallelTime;
                    }
                    else
                    {
                        //
                        result.SumTimeLimit += item.TimeLimit;
                        result.ActDict.Add(item.Code, item);
                        //路由列表
                        if (item.Switch != null)
                        {
                            item.Router = new Dictionary<int, Act>();
                            item.SwitchDict = new Dictionary<int, Router>();
                            foreach (var router in item.Switch)
                            {
                                item.RoleIDs = item.RoleIDs == null ? new List<String>() : item.RoleIDs;

                                if (router.RoleIDs != null)
                                {
                                    foreach (var routeRole in router.RoleIDs)
                                    {
                                        if (!item.RoleIDs.Contains(routeRole))
                                        {
                                            item.RoleIDs.Add(routeRole);
                                        }
                                    }
                                }
                                //设置路由字典
                                item.SwitchDict[router.ID] = router;
                                item.Router[router.ID] = result.Sequence.Where(a => a.Code == router.RouterTo).First();
                                router.OutRouter = router.Method;
                                if (router.TableActityOfRouters != null)
                                {
                                    foreach (var myTableActity in router.TableActityOfRouters)
                                    {
                                        if (String.IsNullOrEmpty(myTableActity.TableName) || myTableActity.SynchCaseState)
                                        {
                                            myTableActity.TableName = result.Table;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //
                   result.ActRolesDict.Add(item.Code, item.RoleIDs);
                   item.ActRolesDict = result.ActRolesDict;
                }
            }
            //总时限
            if (result.SumTimeLimit > 0)
            {
                foreach (var myItem in result.ActDict)
                {
                    myItem.Value.SumTimeLimit = result.SumTimeLimit;
                }
            }
            return result;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class Router
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int Value { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String ActionUrl { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String ActivityUrl { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String ReturnUrl { set; get; }

        /// <summary>
        /// 验证函数
        /// </summary>
        public String VaildFuncName { set; get; }

        /// <summary>
        /// 引导至详细页面（优先级高于returnUrl）
        /// </summary>
        public bool ToShowDetails { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String PerVaildAction { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String RouterTo { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public String Method { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public String Name { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public String OutRouter { set; get; }
        /// <summary>
        /// 
        /// </summary>
        //执行的活动名
        public String Activity { set; get; }

        /// <summary>
        /// 设置CASE的状态
        /// </summary>
        public int CaseStatus { set; get; }
        /// <summary>
        /// 下一个办理者(部门，其中：Self_自己, Parent_父, Brother_旁系, Last_最后一次相同活动)
        /// </summary>
        public String OrganTo { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String[] Paths { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public String[] SwitchBrother { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Act ToAct { set; get; }

        /// <summary>
        /// 路由角色
        /// </summary>
        public String[] RoleIDs { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<TableActityOfRouter> TableActityOfRouters { set; get; }
    }

    public enum ActCurStatus
    {
        正常 = 0,
        退回 = 2
    }

    public class Act : BaseModel
    {
        public virtual int ActCurStatus { set; get; }

        public ActType ActType { set; get; }
        public virtual String ActID { set; get; }
        public virtual String Code { set; get; }
        public virtual List<String> RoleIDs { set; get; }
        /// <summary>
        /// 业务代码
        /// </summary>
        public virtual String BCode { set; get; }

        /// <summary>
        /// 必须的（并行流程中）
        /// </summary>
        public virtual bool Required { set; get; }

        /// <summary>
        /// 必须先读取后办理(独占模式)
        /// </summary>
        public virtual bool RequireFirstRead { set; get; }

        /// <summary>
        /// 自动派发（）
        /// </summary>
        public virtual bool AutoDispatch { set; get; }

        /// <summary>
        /// 活动的处理表单名称
        /// </summary>
        public virtual String Form { set; get; }

        public String Table { set; get; }

        /// <summary>
        /// 指定活动的显示表单（）
        /// </summary>
        public virtual String DisplayForm { set; get; }

        /// <summary>
        /// 活动的处理表单数据的控制器（接口）名称
        /// </summary>
        public virtual String ApiName { set; get; }


        /// <summary>
        /// 可办理活动的角色字典 key=actcode value=roleList
        /// </summary>
        public virtual Dictionary<String, List<String>> ActRolesDict { set; get; }

        private bool _check = true;
        /// <summary>
        /// 是否生成（并行流程中）
        /// </summary>
        public virtual bool Checked
        {
            set
            {
                _check = value;
            }
            get
            {
                if (Required)
                {
                    return true;
                }
                else
                {
                    return _check;
                }
            }
        }

        /// <summary>
        /// 活动名
        /// </summary>
        public virtual String Name { set; get; }


        /// <summary>
        /// 活动时限
        /// </summary>
        public int TimeLimit { set; get; }

        /// <summary>
        /// 活动总时限
        /// </summary>
        public int SumTimeLimit { set; get; }
        /// <summary>
        /// 可办理的部门
        /// </summary>
        public String[] Targets { set; get; }
        /// <summary>
        /// 可办理部门的父级部门
        /// </summary>
        public String[] TargetParentIDs { set; get; }
        /// <summary>
        /// 可办理的部门等于指定节点
        /// </summary>
        public String TargetEqualNode { set; get; }
        /// <summary>
        /// 与To关联的目标ID(OrgId)
        /// </summary>
        public String Target { set; get; }

        public String Url { set; get; }
        public bool IsEnd { set; get; }
        public bool IsStart { set; get; }
        public List<Router> Switch { set; get; }

        /// <summary>
        /// 合并
        /// </summary>
        public String[] Merge { set; get; }

        /// <summary>
        /// 下一个ACT
        /// </summary>
        public Act NextAct
        {
            get
            {
                try
                {
                    return this.Router[this.CurrentRouterID];
                }
                catch
                {
                    return null;
                }
            }
        }

        private String _outRouter;
        /// <summary>
        /// 
        /// </summary>
        public String CurrentOutRouter
        {
            get
            {
                try
                {
                    if (this.IsEnd)
                    {
                        return "已结束";
                    }
                    return this.CurrentRouter.Method + this.NextAct.Name;
                }
                catch
                {
                    return "";
                }
            }
        }

        public int CurrentRouterID { set; get; }

        public Router CurrentRouter
        {
            get
            {
                try
                {
                    return this.SwitchDict[this.CurrentRouterID];
                }
                catch
                {
                    return null;
                }
            }
        }

        public Act Next
        {
            set;
            get;
        }
        //public Act Pre { set; get; }   

        /// <summary>
        /// 
        /// </summary>
        public List<Act> Sequence { set; get; }

        public Dictionary<String, Act> ActDict { set; get; }

        /// <summary>
        /// 并列
        /// </summary>
        public List<Act> Parallel { set; get; }

        /// <summary>
        /// 路由
        /// </summary>
        public Dictionary<int, Act> Router { set; get; }

        /// <summary>
        /// 路由字典
        /// </summary>
        public Dictionary<int, Router> SwitchDict { set; get; }

        /// <summary>
        /// 只显示编辑不加载显示表单
        /// </summary>
        public bool OnlyEditForm { get; set; }

        /// <summary>
        /// 从router中获取下一个活动的NAME
        /// </summary>
        /// <param name="router"></param>
        /// <returns></returns>
        public static String GetToActNameFromRouter(String router)
        {
            if (String.IsNullOrEmpty(router))
            {
                return "";
            }
            if (router.Contains("[TO]"))
            {
                return router.Replace("[TO]", "");
            }
            if (router.Contains("[驳回到]"))
            {
                return router.Replace("[驳回到]", "");
            }
            throw new Exception("router 的格式不正确，既不包含[TO]又不包含[驳回到]");
        }

    }

    public enum ActType
    {
        Sequence = 0,
        Parallel = 1
    }

    public enum ActOperate
    {
        未办理 = 0,
        办理中 = 1,
        已办理 = 2
    }

    /// <summary>
    /// 路由相应的表活动
    /// </summary>
    public class TableActityOfRouter
    {
        /// <summary>
        /// 设置表名
        /// </summary>
        public String TableName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool SynchCaseState { set; get; }

        /// <summary>
        /// 设置列值
        /// </summary>
        public List<SetColumnValue> SetColumnValues { set; get; }
    }

    public class SetColumnValue
    {
        public String Name { set; get; }

        public String Value { set; get; }
    }
}

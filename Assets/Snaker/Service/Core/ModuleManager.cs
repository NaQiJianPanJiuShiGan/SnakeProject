using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;

namespace Snaker.Service.Core
{
    /// <summary>
    /// 模块管理器模块
    /// </summary>
    public class ModuleManager :ServiceModule<ModuleManager>
    {
        //消息辅助类
        class MessageObject
        {
            public string target;//消息发送到目标模块的名字
            public string msg;//消息名字
            public object[] args;//消息参数
        }
        //管理所有已经被创建过的模块<string 模块的名字, 继承BusinessModule的模块>
        private Dictionary<string, BusinessModule> m_mapModule;
        //预监听事件表 A模块监听B模块的事件，但是B模块还没有被创建
        private Dictionary<string, EventTable> m_mapPreListenEvents;
        //预监听保存 缓存要发送的消息
        private Dictionary<string, List<MessageObject>> m_mapCacheMessage;

        private string m_domain;
        public ModuleManager()
        {
            m_mapModule = new Dictionary<string, BusinessModule>();
            m_mapPreListenEvents = new Dictionary<string, EventTable>();
            m_mapCacheMessage = new Dictionary<string, List<MessageObject>>();
        }
        //初始化时传入域名 （默认值"Snaker.Module"）
        public void Init(string domain = "Snaker.Module")
        {
            CheckSingleton();
            m_domain = domain;
        }
        public T CreateModule<T>(object args = null) where T:BusinessModule
        {
            return (T) CreateModule(typeof(T).Name, args);
        }
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="name">模块类名</param>
        /// <param name="args">参数可为空</param>
        /// <returns></returns>
        public BusinessModule CreateModule(string name,object args = null)
        {
            if (m_mapModule.ContainsKey(name))//存在就不能重复创建
            {
                return null;
            }
            BusinessModule module = null;
            Type type = Type.GetType(m_domain + "." + name);//通过名字反射获得这个类型    
            if (type!=null)//类型存在
            {
                module = Activator.CreateInstance(type) as BusinessModule;//创建这个实例
            }
            else
            {
                module = new LuaModule(name);//不存在可能是lua模块
            }
            m_mapModule.Add(name, module);//添加进管理器
            //处理预监听的事件
            if (m_mapPreListenEvents.ContainsKey(name))
            {
                EventTable tblEvent = m_mapPreListenEvents[name];
                m_mapPreListenEvents.Remove(name);//取出这个预监听事件
                module.SetEventTable(tblEvent);//设置给这个模块
            }
            module.Create(args);
            //处理缓存的消息
            if (m_mapCacheMessage.ContainsKey(name))
            {
                List<MessageObject> list = m_mapCacheMessage[name];
                for (int i = 0; i < list.Count; i++)
                {
                    MessageObject msgobj = list[i];
                    module.HandleMessage(msgobj.msg, msgobj.args);//发送这个模块中预缓存所有消息
                }
                m_mapCacheMessage.Remove(name);//移除这个模块的消息
            }
            return module;
        }
        public void ReleaseModule(BusinessModule module)
        {
            if (module!=null)
            {
                if (m_mapModule.ContainsKey(module.Name))
                {
                    this.Log("ReleaseModule() name = " + module.Name);
                    m_mapModule.Remove(module.Name);
                    module.Release();
                }
                else
                {
                    this.LogError("ReleaseModule() 模块不是由ModuleManager创建的！ name = " + module.Name);
                }
            }
            else
            {
                this.LogError("ReleaseModule() module = null!");
            }
        }
        /// <summary>
        /// 释放所有模块
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var item in m_mapPreListenEvents)//清空预监听事件
            {
                item.Value.Clear();
            }
            m_mapPreListenEvents.Clear();
            m_mapCacheMessage.Clear();
            foreach (var item in m_mapModule)//清空监听事件
            {
                item.Value.Release();
            }
            m_mapModule.Clear();
        }
        public T GetModule<T>() where T:BusinessModule
        {
            return (T)GetModule(typeof(T).Name);
        }
        /// <summary>
        /// 通过名字获取一个模块
        /// 如果为创建过该模块，则返回null
        /// </summary>
        /// <param name="name">模块名</param>
        /// <returns></returns>
        public BusinessModule GetModule(string name)
        {
            if (m_mapModule.ContainsKey(name))
            {
                return m_mapModule[name];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 向指定模块发送消息
        /// </summary>
        /// <param name="target">发送到目标模块的名</param>
        /// <param name="msg">消息名</param>
        /// <param name="args">消息参数</param>
        public void SendMessage(string target,string msg,params object[] args)
        {
            BusinessModule module = GetModule(target);
            if (module!=null)//如果模块已经加载了就发送消息
            {
                module.HandleMessage(msg,args);
            }
            else//模块没有加载就保存到缓存一下等待加载的时候再发送
            {
                List<MessageObject> list = GetCacheMessageList(target);
                MessageObject msgobj = new MessageObject();
                msgobj.target = target;
                msgobj.msg = msg;
                msgobj.args = args;
                list.Add(msgobj);
            }
        }
        /// <summary>
        /// 得到预监听消息集合
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private List<MessageObject> GetCacheMessageList(string target)
        {
            List<MessageObject> list = null;
            if (m_mapCacheMessage.ContainsKey(target))
            {
                list = m_mapCacheMessage[target];
            }
            else
            {
                list = new List<MessageObject>();
                m_mapCacheMessage.Add(target,list);
            }
            return list;
        }
        /// <summary>
        /// 某个模块中的某个事件
        /// </summary>
        /// <param name="target">目标模块</param>
        /// <param name="type">目标事件</param>
        /// <returns></returns>
        public ModuleEvent Event(string target,string type)
        {
            ModuleEvent evt = null;
            BusinessModule module = GetModule(target);
            if (module!=null)//如果模块已经呗加载就返回事件
            {
                evt = module.Event(type);
            }
            else//没有被加载就在预监听表中创建或返回一个
            {
                EventTable table = GetPreListenEventTable(target);
                evt = table.GetEvent(type);
            }
            return evt;
        }
        private EventTable GetPreListenEventTable(string target)
        {
            EventTable table = null;
            if (m_mapPreListenEvents.ContainsKey(target))
            {
                table = m_mapPreListenEvents[target];
            }
            else
            {
                table = new EventTable();
                m_mapPreListenEvents.Add(target,table);
            }
            return table;
        }
    }
}

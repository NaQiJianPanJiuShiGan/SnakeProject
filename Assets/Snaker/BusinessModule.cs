using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;
using System.Reflection;
namespace Assets.Snaker
{
    /// <summary>
    /// 业务模块的基类
    /// </summary>
    public abstract class BusinessModule : Module
    {
        private string m_name = "";

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_name))
                {
                    m_name = this.GetType().Name;//返回当前类的名字
                }
                return m_name;
            }
        }
        public string Title;
        public BusinessModule()
        {

        }
        //当lua模块时，操作是在lua中进行的类的名字就是lua中传过来的而不是使用C#类型的名字
        internal BusinessModule(string name)
        {
            m_name = name;
        }
        /// <summary>
        /// 业务逻辑模块中的事件表
        /// </summary>
        private EventTable m_tblEvent;
        /// <summary>
        /// 由外部设置事件表
        /// </summary>
        /// <param name="tblEvent"></param>
        internal void SetEventTable(EventTable tblEvent)
        {
            m_tblEvent = tblEvent;
        }
        /// <summary>
        /// 模块的一个事件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ModuleEvent Event(string type)
        {
            return GetEventTable().GetEvent(type);
        }
        /// <summary>
        /// 事件表
        /// </summary>
        /// <returns></returns>
        protected EventTable GetEventTable()
        {
            if (m_tblEvent==null)
            {
                m_tblEvent = new EventTable();
            }
            return m_tblEvent;
        }
        /// <summary>
        /// 执行消息
        /// </summary>
        /// <param name="msg">方法名</param>
        /// <param name="args">方法参数</param>
        internal void HandleMessage(string msg,object[] args)
        {
            this.Log("HandleMessage() msg{0},arge{1}", msg, args);
            MethodInfo mi = this.GetType().GetMethod(msg);
            if (mi!=null)
            {
                mi.Invoke(this,args);
            }
            else
            {
                OnModuleMessage(msg,args);
            }
        }
        protected virtual void OnModuleMessage(string msg,object[] args)
        {
            this.Log("OnModuleMessage() msg{0},args{1}", msg, args);
        }
        public virtual void Create(object args = null)
        {
            this.Log("Create args ={0}",args);
        }
        /// <summary>
        /// 清空模块事件
        /// </summary>
        public override void Release()
        {
            if (m_tblEvent!=null)
            {
                m_tblEvent.Clear(); //清空事件表
                m_tblEvent = null;
            }
            base.Release();
        }
        public virtual void Show()
        {
            this.Log("Show()");
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.Snaker
{
    public class ModuleEvent : UnityEvent<object>
    {

    }
    public class ModuleEvent<T> : UnityEvent<T>
    {

    }
    public class EventTable
    {
        private Dictionary<string, ModuleEvent> m_mapEvent;
        /// <summary>
        /// 获取一个ModuleEvent(它其实是一个EventTable事件表)
        /// 如果不存在，则实例化一个
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        
        public ModuleEvent GetEvent(string type)
        {
            if (m_mapEvent==null)
            {
                m_mapEvent = new Dictionary<string, ModuleEvent>();
            }
            if (!m_mapEvent.ContainsKey(type))
            {
                m_mapEvent.Add(type, new ModuleEvent());
            }
            return m_mapEvent[type];
        }
        /// <summary>
        /// 移除所有监听事件
        /// </summary>
        public void Clear()
        {
            if (m_mapEvent!=null)
            {
                foreach (var item in m_mapEvent)
                {
                    item.Value.RemoveAllListeners();//先移除所有监听
                }
                m_mapEvent.Clear();
            }
        }
    }
   
}

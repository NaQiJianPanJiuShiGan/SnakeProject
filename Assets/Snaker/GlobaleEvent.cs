using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Snaker
{
    /// <summary>
    /// 全局事件
    /// </summary>
    public static class GlobaleEvent
    {
        public static ModuleEvent<bool> onLogin = new ModuleEvent<bool>();
        public static ModuleEvent<bool> onPlay = new ModuleEvent<bool>();

        #region 使用方法
        public static void Foo()
        {
            GlobaleEvent.onLogin.AddListener(OnLogin);//事件监听
            GlobaleEvent.onLogin.Invoke(true);//发送事件
        }
        private static void OnLogin(bool args)
        {

        }
        #endregion
    }
}

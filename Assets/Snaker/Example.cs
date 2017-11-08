using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;
using UnityEngine;

namespace Assets.Snaker
{
    public class Example : MonoBehaviour
    {
        void Start()
        {
            Debuger.EnableLog = true;

            ModuleC.GetInstance.Init();
            ModuleManager.GetInstance.Init("Snaker.Service.Core.Example");

            ModuleManager.GetInstance.CreateModule("ModuleA");
            ModuleManager.GetInstance.CreateModule("ModuleB");

        }

    }
    public class ModuleA : BusinessModule
    {
        public override void Create(object args = null)
        {
            base.Create(args);

            //业务层模块之间，通过Message进行通讯
            ModuleManager.GetInstance.SendMessage("ModuleB", "MessageFromA_1", 1, 2, 3);
            ModuleManager.GetInstance.SendMessage("ModuleB", "MessageFromA_2", "abc", 123);

            //业务层模块之间，通过Event进行通讯 
            ModuleManager.GetInstance.Event("ModuleB", "onModuleEventB").AddListener(OnModuleEventB);

            //业务层调用服务层，通过事件监听回调
            ModuleC.GetInstance.onEvent.AddListener(OnModuleEventC);
            ModuleC.GetInstance.DoSomething();

            //全局事件
            GlobaleEvent.onLogin.AddListener(OnLogin);
        }

        private void OnModuleEventC(object args)
        {
            this.Log("OnModuleEventC() args:{0}", args);
        }

        private void OnModuleEventB(object args)
        {
            this.Log("OnModuleEventB() args:{0}", args);
        }

        private void OnLogin(bool args)
        {
            this.Log("OnLogin() args:{0}", args);
        }
    }

    public class ModuleB : BusinessModule
    {
        public ModuleEvent onModuleEventB { get { return Event("onModuleEventB"); } }

        public override void Create(object args = null)
        {
            base.Create(args);
            onModuleEventB.Invoke("aaaa");
        }

        protected void MessageFromA_2(string args1, int args2)
        {
            this.Log("MessageFromA_2() args:{0},{1}", args1, args2);
        }

        protected override void OnModuleMessage(string msg, object[] args)
        {
            base.OnModuleMessage(msg, args);
            this.Log("OnModuleMessage() msg:{0}, args:{1},{2},{3}", msg, args[0], args[1], args[2]);
        }
    }

    public class ModuleC : ServiceModule<ModuleC>
    {
        public ModuleEvent onEvent = new ModuleEvent();
        public void Init()
        {

        }
        public void DoSomething()
        {
            onEvent.Invoke(null);
        }
    }
}

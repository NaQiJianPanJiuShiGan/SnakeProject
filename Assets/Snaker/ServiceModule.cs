using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Snaker
{

    public abstract class ServiceModule<T> : Module where T : new() //限制传入的类并且必须带有公共无参构造函数的非抽象类型
    {
        //抽象类不能被直接实例化所以这里不需要构造器
        private static T ms_instance = default (T);
        public static T GetInstance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new T();
                }
                return ms_instance;
            }
        }
        /// <summary>
        /// 检测是否是单例
        /// </summary>
        protected void CheckSingleton()
        {
            if (ms_instance==null)
            {
                var exp = new Exception("ServiceModule<"+typeof(T).Name+">无法直接实例化，因为他是一个单例！");
                throw exp;
            }
        }
    }
}

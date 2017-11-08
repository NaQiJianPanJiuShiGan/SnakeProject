using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Snaker
{
    public  class LuaModule :BusinessModule
    {
        private object m_args = null;
        internal LuaModule(string name) : base(name)
        {

        }
        public override void Create(object args = null)
        {
            base.Create(args);
            m_args = args;
            //TODO加载Name所对应的lua脚本！   
        }
        public override void Release()
        {
            base.Release();
            //TODO释放Name所对应的lua脚本！
        }
    }
}

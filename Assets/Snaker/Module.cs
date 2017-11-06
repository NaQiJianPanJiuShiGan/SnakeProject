using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;

namespace Assets.Snaker
{
    public abstract class Module
    {
        public virtual void Release()
        {
            this.Log("Release");
        }
    }
}

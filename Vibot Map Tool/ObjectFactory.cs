using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ObjectFactory
{
    using OBJECTMAP = Dictionary<string, ConstructorFuntor>;

    public delegate IObject ConstructorFuntor();
    
    public abstract class IObject
    {
        private string m_ClassName = "";
        public abstract IObject Instance();
        public bool IsClassName(string ClassName)
        {
            if (m_ClassName == ClassName)
                return true;

            return false;
        }
    }

    public abstract class CObjectFactory
    {
        public OBJECTMAP ObjectMap = new OBJECTMAP();

        public void AddObjectClass(string ClassName, ConstructorFuntor Functor)
        {
            if (!ObjectMap.ContainsKey(ClassName))
            {
                ObjectMap.Add(ClassName, Functor);
            }
            else
            {
                MessageBox.Show("이미 등록된 Object입니다");
            }
        }

        public IObject CreateObjectClass(string ClassName)
        {
            foreach (KeyValuePair<string, ConstructorFuntor> Object in ObjectMap)
            {
                if (Object.Key == ClassName)
                {
                    ConstructorFuntor Functor = Object.Value;
                    return Functor();
                }
            }
            return null;
        }

        public abstract void RegisterObject();
    }
}

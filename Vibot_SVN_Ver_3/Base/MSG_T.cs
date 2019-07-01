using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vibot.Base
{
    public class CMsg<T>
    {
        public T m_Msg;
        public object m_Arg1;
        public object m_Arg2;
        public object m_Arg3;
        public object m_Arg4;

        public CMsg(T Msg)
        {
            m_Msg = Msg;
        }

        public CMsg(T Msg, object Arg1)
        {
            m_Msg = Msg;
            m_Arg1 = Arg1;
        }

        public CMsg(T Msg, object Arg1, object Arg2)
        {
            m_Msg = Msg;
            m_Arg1 = Arg1;
            m_Arg2 = Arg2;
        }

        public CMsg(T Msg, IntPtr Arg1, IntPtr Arg2, IntPtr Arg3)
        {
            m_Msg = Msg;
            m_Arg1 = Arg1;
            m_Arg2 = Arg2;
            m_Arg3 = Arg3;
        }

        public CMsg(T Msg, IntPtr Arg1, IntPtr Arg2, IntPtr Arg3, IntPtr Arg4)
        {
            m_Msg = Msg;
            m_Arg1 = Arg1;
            m_Arg2 = Arg2;
            m_Arg3 = Arg3;
            m_Arg4 = Arg4;
        }

    }
}

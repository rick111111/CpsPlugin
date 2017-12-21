using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ProjectSystem.VS;

namespace DesktopProjectDebug
{
    internal static class Assumes
    {
        public static bool Verify(bool condition)
        {
            if (!condition)
            {
                throw new Exception();
            }

            return condition;
        }

        public static bool Verify(bool condition, string msg)
        {
            if (!condition)
            {
                throw new Exception(msg);
            }

            return condition;
        }

        public static void ThrowIfNull(object obj, string msg)
        {
            if (obj == null)
            {
                throw new Exception(msg);
            }            
        }

        public static void Assert(bool assertion, string msg)
        {
            if (!assertion)
            {
                throw new Exception(msg);
            }
        }

        public static void Fail(string msg)
        {
            throw new Exception(msg);
        }


    }
}
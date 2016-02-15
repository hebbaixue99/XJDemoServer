namespace Youkia
{
    using System;

    public class DomainAccess
    {
        public virtual void Dispose()
        {
        }

        ~DomainAccess()
        {
        }

        public static object getObject(string classStr)
        {
            return null;
        }

        private static Type getType(string classStr)
        {
            return null;
        }

        private static object loadObject(Type type)
        {
            return null;
        }
    }
}


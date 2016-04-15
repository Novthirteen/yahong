using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace com.Sconit.Utility
{
    public static class LogHelper
    {
        public static void LogEntityField(log4net.ILog log, object obj)
        {
            PropertyInfo[] propertyInfoAry = obj.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfoAry)
            {
                log.Debug(obj.GetType() + "." + propertyInfo.Name + " : " + propertyInfo.GetValue(obj, null));
            }
        }
    }
}

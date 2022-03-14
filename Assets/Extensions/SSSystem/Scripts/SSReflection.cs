using UnityEngine;
using System.Collections;
using System.Reflection;
#if NETFX_CORE
using System.Linq;
#endif

public class SSReflection
{
    public static object GetPropValue(object src, string propName)
    {
#if NETFX_CORE
            return src.GetType().GetTypeInfo().GetDeclaredProperty(propName).GetValue(src, null);
#else
        return src.GetType().GetProperty(propName).GetValue(src, null);
#endif
    }

    public static void SetPropValue(object src, string propName, object data)
    {

#if NETFX_CORE
       //src.GetType().InvokeMember(
       //    propName,
       //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
       //    System.Type.DefaultBinder,
       //    src,
       //    new object[] { data }
       //);
       src.GetType().GetTypeInfo()
        .DeclaredMethods
        .Where((methodInfo) => methodInfo.IsPublic && methodInfo.Name.Equals(propName))
        .FirstOrDefault()
        .Invoke(src, new[] {data});
#else
        src.GetType().InvokeMember(
           propName,
           BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
           System.Type.DefaultBinder,
           src,
           new object[] { data }
       );
#endif
    }
}

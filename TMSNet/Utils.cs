using System.Reflection;

namespace TMSNet
{
    public class Utils
    {
        public static TOut CopyCast<TIn, TOut>(TIn tin) where TOut: new()
        {
            var o = new TOut();

            foreach (var propertyInfo in typeof(TIn).GetProperties(BindingFlags.Public))
            {
                var p = typeof(TOut).GetProperty(propertyInfo.Name);

                if (p == null)
                    continue;
                
                p.SetValue(propertyInfo.GetValue(tin), o);
            }

            return o;
        }

        public static void CopyCast<TIn, TOut>(TIn tin, TOut o) where TOut : new()
        {
            foreach (var propertyInfo in typeof(TIn).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var p = typeof(TOut).GetProperty(propertyInfo.Name);

                if (p == null)
                    continue;

                var get = propertyInfo.GetValue(tin);
                p.SetValue(o, get);
            }
        }
    }
}
using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    public class JSFloat : JSSerializable
    {
        float a;

        public JSFloat(float a)
        {
            this.a = a;
        }

        public float GetFloat()
        {
            return a;
        }

        public static JSFloat GetJSFloat(float a)
        {
            return new JSFloat(a);
        }

        public override string ToString()
        {
            return "" + a;
        }
    }

}

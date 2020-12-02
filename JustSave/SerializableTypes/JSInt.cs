using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    public class JSInt : JSSerializable
    {
        int a;

        public JSInt(int a)
        {
            this.a = a;
        }

        public int GetInt() {
            return a;
        }

        public static JSInt GetJSSingle(int a) {
            return new JSInt(a);
        }

        public override string ToString()
        {
            return "" +a;
        }
    }

}

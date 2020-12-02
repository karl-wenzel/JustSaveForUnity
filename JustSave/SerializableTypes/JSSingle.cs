using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    public class JSSingle : JSSerializable
    {
        int a;

        public JSSingle(int a)
        {
            this.a = a;
        }

        public int GetInt() {
            return a;
        }

        public static JSSingle GetJSSingle(int a) {
            return new JSSingle(a);
        }

        public override string ToString()
        {
            return "" +a;
        }
    }

}

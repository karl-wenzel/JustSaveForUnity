using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    public class JSQuad : JSSerializable
    {
        double a;
        double b;
        double c;
        double d;

        public JSQuad(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
    }

}

using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    public class JSTriple : JSSerializable
    {
        double a;
        double b;
        double c;

        public JSTriple(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }

}

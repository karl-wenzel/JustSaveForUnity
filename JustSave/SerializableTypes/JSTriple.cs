using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    public class JSTriple : JSSerializable
    {
        float a;
        float b;
        float c;

        public JSTriple(float a, float b, float c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public static JSTriple GetJSTriple(UnityEngine.Vector3 Vector) {
            return new JSTriple(Vector.x, Vector.y, Vector.z);
        }

        public UnityEngine.Vector3 GetVector3() {
            return new UnityEngine.Vector3(a, b, c);
        }

        public override string ToString()
        {
            return "JSTriple: " + a + ", " + b + ", " + c;
        }
    }

}

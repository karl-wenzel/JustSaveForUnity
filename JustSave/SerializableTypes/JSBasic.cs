using System;
using JustSave;

namespace JustSave
{
    [Serializable]
    class JSBasic : JSSerializable
    {
        object SerializableObject;

        public JSBasic(object value) {
            SerializableObject = value;
        }

        public object GetValue() {
            return SerializableObject;
        }

        public static JSBasic GetFromObject(object value) {
            return new JSBasic(value);
        }

        public override string ToString()
        {
            return SerializableObject.ToString();
        }
    }
}

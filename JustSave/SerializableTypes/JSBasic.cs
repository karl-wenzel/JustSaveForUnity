using System;
using JustSave;
using System.Runtime.Serialization;

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
            if (SerializableObject is string) {
                return ((string)SerializableObject).ToString();
            }
            return base.ToString();
        }
    }
}

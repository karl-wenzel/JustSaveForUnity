using System;
using JustSave;
using UnityEngine;

namespace JustSave
{
    [Serializable]
    public class JSNTuple : JSSerializable
    {
        float[] fields;

        public JSNTuple(params float[] fields) {
            this.fields = fields;
        }

        public Vector2 GetVector2() {
            if (fields.Length < 2) {
                Debug.LogError("JSNTuple has not enough fields to support Vector2. Returning default vector (0,0).");
                return new Vector2(0, 0);
            }
            return new Vector2(fields[0], fields[1]);
        }

        public static JSNTuple GetFromVector2(Vector2 input) {
            return new JSNTuple(input.x, input.y);
        }

        public Vector3 GetVector3()
        {
            if (fields.Length < 3)
            {
                Debug.LogError("JSNTuple has not enough fields to support Vector3. Returning default vector (0,0,0).");
                return new Vector3(0, 0, 0);
            }
            return new Vector3(fields[0], fields[1], fields[2]);
        }

        public static JSNTuple GetFromVector3(Vector3 input)
        {
            return new JSNTuple(input.x, input.y, input.z);
        }

        public Vector4 GetVector4()
        {
            if (fields.Length < 4)
            {
                Debug.LogError("JSNTuple has not enough fields to support Vector4. Returning default vector (0,0,0,0).");
                return new Vector4(0, 0, 0, 0);
            }
            return new Vector4(fields[0], fields[1], fields[2], fields[3]);
        }

        public static JSNTuple GetFromVector4(Vector4 input)
        {
            return new JSNTuple(input.x, input.y, input.z, input.w);
        }

        public Quaternion GetQuaternion()
        {
            if (fields.Length < 4)
            {
                Debug.LogError("JSNTuple has not enough fields to support Quaternion. Returning identity.");
                return Quaternion.identity;
            }
            return new Quaternion(fields[0], fields[1], fields[2], fields[3]);
        }

        public static JSNTuple GetFromQuaternion(Quaternion input)
        {
            return new JSNTuple(input.x, input.y, input.z, input.w);
        }
    }

}

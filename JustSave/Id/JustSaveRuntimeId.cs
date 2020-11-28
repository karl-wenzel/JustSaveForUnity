using System;

namespace JustSave
{
    public class JustSaveRuntimeId : JustSaveId
    {
        Guid id;
        ObjectPool ObjectPoolReference;
        int ObjectPoolIndex;

        public void SetId(Guid newId) {
            id = newId;
        }

        public Guid GetId() {
            return id;
        }

        public void SetUp(int newIndex, ObjectPool newPool) {
            ObjectPoolIndex = newIndex;
            ObjectPoolReference = newPool;
        }

        public void Despawn() {
            ObjectPoolReference.Despawn(ObjectPoolIndex);
        }

        public override string ToString()
        {
            return id.ToString();
        }
    }

}

namespace JustSave
{

    public interface ISavable
    {
        void JSOnSave();

        void JSOnLoad();

        void JSOnSpawned();

        void JSOnDespawned();

        void JSOnPooled();
    }

}

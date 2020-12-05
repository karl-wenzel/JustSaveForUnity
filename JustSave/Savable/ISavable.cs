namespace JustSave
{

    /// <summary>
    /// implement this interface to be notified, when something JustSave-related happens
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// this is called, right before the savedata is gathered
        /// </summary>
        void JSOnSave();

        /// <summary>
        /// this is called after the savedata is applied
        /// </summary>
        void JSOnLoad();

        /// <summary>
        /// this is called, when the object leaves the pool and is activated
        /// </summary>
        void JSOnSpawned();

        /// <summary>
        /// this is called, when the object is deactivated
        /// </summary>
        void JSOnDespawned();

        /// <summary>
        /// this is called, when the object enters the pool
        /// </summary>
        void JSOnPooled();

        /// <summary>
        /// this is called, when the pool needs more objects. The object should try to despawn itself.
        /// </summary>
        void JSOnNeeded();
    }

}

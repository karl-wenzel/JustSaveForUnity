namespace JustSave
{
    /// <summary>
    /// Decides, how the pool should operate if it's empty.
    /// <para>ForceDespawn: The pool will despawn and recycle the oldest object.</para>
    /// ReturnNull: The pool will return null and do nothing.
    /// <para>OnDemand: The pool will instantiate a new object and therefore change it's size.</para>
    /// </summary>
    public enum PoolingMode
    {
        ForceDespawn,
        ReturnNull,
        OnDemand
    }
}

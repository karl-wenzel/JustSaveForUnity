# More About Object Pooling

Just Save comes with an object pooling system included. [What this means](#WhatIs), [how it works](#HowItWorks) and [what you should know about](#poolingModes).

## What is object pooling <a name="WhatIs"></a>

Object pooling means that if you want to spawn an object, instead of creating a new one, the object is taken from a pool of inactive objects.
If you don't need the spawned object anymore, instead of just destroying it, you can return it to the pool to reuse it later.
In most of the cases, this has a positive impact on performance.
If you want to know more, read [the wikipedia article](https://en.wikipedia.org/wiki/Object_pool_pattern).

## How does it work in JustSave <a name="HowItWorks"></a>

When dealing with object pools, JustSave will do almost anything for you. You just need a reference to your prefab, create an object pool with it using a simple method call, and then you can spawn your objects conveniently.  

*Here is an code example of how to create an object pool and spawn an object from it:*

	using JustSave

	class ExampleClass : MonoBehaviour
	{
		//assign your prefab in the unity inspector
		public GameObject MyPrefab;
		public Vector3 SpawnPosition;

		public void Start () 
		{
			JustSaveManager.Instance.CreateObjectPool(MyPrefab, "ExamplePrefab", 5, PoolingMode.OnDemand, 2);
		}

		public void SpawnMyPrefab ()
		{
			JustSaveManager.Instance.Spawn("ExamplePrefab", SpawnPosition);
		}
	}

(Read about the general approach in ["Getting Started"](./GETTINGSTARTED.md#runtimeObjects).)

When creating an object pool with `JustSaveManager.Instance.CreateObjectPool(YourPrefab, "YourPrefabId", SizeOfThePool, PoolingMode, NotifyToDespawn);`, you can set several parameters in the arguments of the method.

| parameter name | type | example value | description |
| -------------- | --------- | ------------ | --------------------------------------------------------- |
| PrefabToSpawn | GameObject | YourPrefab | This should be a reference to the prefab you want to spawn |
| PrefabId | String | "YourPrefabId" | Assign a unique Id to your prefab here. This can be anything you want. Use this for spawning the prefab later. |
| BasePoolSize | Integer | 5 | This is the default size of the pool. JustSave will create a new pool and fill it with as many objects as you specify here. |
| Mode | PoolingMode | PoolingMode.OnDemand | The pooling mode specifies, how the pool will react if you want to spawn something and it is empty. |
| NotifyToDespawn | Integer | 2 | If you already spawned 3 of the 5 objects in your pool and spawn a 4-th object, the pool will notify the first object you spawned to despawn itself. |

### Pooling modes <a name="poolingModes"></a>

JustSave supports 3 different pooling modes:

- PoolingMode.ForceDespawn
- PoolingMode.ReturnNull
- PoolingMode.OnDemand

*ForceDespawn:* If you want to spawn something and the pool is empty, the oldest object will automatically be recycled (despawned and instantly spawned again).

*ReturnNull:* If you want to spawn something and the pool is empty, nothing will happen. The Reference returned by `JustSaveManager.Instance.Spawn(...)` will be a null-Pointer.

*OnDemand:* This mode imitates the "classic" system, which means if the pool is empty and a new object is needed, this new object will be instantiated, added to the pool and then spawned. 
Please be aware, that this mode can lead to lag spikes, when spawning large quantities of objects. It is usefull though, if you just want the same functionality as a classic `Instantiate(object)`.

### Notify to despawn <a name="notifyToDespawn"></a>

This is an usefull setting to integrate better Despawning into your application. Notify to despawn sets how many objects ahead an object in the pool should be informed before it is spawned. 
The **ISavable**-interface has a JSOnNeeded()-method, which is called, when the Runtime Object will soon be despawned. You can use this to gain more control over the despawning process and implement custom despawns.
For an example implementation on how to code a custom despawn, see the **JustSaveNiceDespawn** example class, which is included in the JustSave package.

### Resetting runtime objects <a name="resettingRuntimeObjects"></a>

When using object pooling, you have to be especially carefull about the lifecycle of your prefabs. When they are recycled (when they are spawned after beeing despawned), their fields should be reset.
Use the method `JSOnSpawned()` from the **ISavable**-interface for that. This method is called every time, an object is spawned. In most cases, use `JSOnSpawned()` instead of `Start()` or `Awake()`.
In the example below, the lifetimeCounter should not count how long the object lived in earlier lifecycles. Therefore, it must be reset, when the object is spawned.

*how to write a class, counting the lifetime of a spawned prefab*

	using UnityEngine;
	using JustSave;

	public class ClockOfLife : Savable
	{
		[Autosaved]
		public float lifetimeCounter;

		private void Update()
		{
			lifetimeCounter += Time.deltaTime;
		}

		public override void JSOnSpawned()
		{
			base.JSOnSpawned();
			lifetimeCounter = 0;
		}
	}

> Note: If you want your lifetimeCounter to be consistent when saving an loading again, you have to use the **[Autosaved]**-attribute on the field `public float lifetimeCounter`.

### Returning objects to the pool whenever you want

If you just want to despawn an object, because you dont need it anymore, feel free to call the `Despawn()`-method on its **JustSaveId**. The Object will then be despawned and returned to it's pool.

*example implementation:*

	using JustSave;
	using UnityEngine;

	public class DespawnThis : MonoBehaviour
	{
		public void DespawnMePlease()
		{
			GetComponent<JustSaveId>().Despawn();
		}
	}

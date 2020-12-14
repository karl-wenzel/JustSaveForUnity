# Getting Started

This document will guide you through the first steps of using the JustSave package in unity.

## Table of contents
1. [How to install this package in unity](#install)
2. [How to use the JustSaveManager](#use)
3. [How to prepare your game for loading and saving](#prepare)
	1. [Preparing Scene Objects](#sceneObjects)
	2. [Preparing Runtime Objects](#runtimeObjects)
	3. [The Autosaved-Attribute](#attribute)
	4. [The ISavable interface and the Savable class](#savables)
4. [How to actually load and save](#loadAndSave)

## How to install this package in unity <a name="install"></a>

Just import it and make sure that the JustSave folder, containing all the files from this package, is saved somewhere in your Asset-folder.

## How to use the JustSaveManager <a name="use"></a>

First note, that everything in this package is contained in the JustSave-namespace.
So in order to use anything from this package you must include the namespace in your classes. Add `using JustSave;` at the beginning of your script.

To use JustSave, you should first get a reference to the **JustSaveManager**. The **JustSaveManager** is the central class of this package and controls everything.
With `JustSaveManager.Instance` you can get a reference to the singleton-instance of this manager class. Use this to access all the functionality of this manager. You can store this reference, if you like, or use it directly.

*An example, how to use `JustSaveManager.Instance`:*

	using JustSave;

	class ExampleClass
	{
		private void ExampleSave1()
		{
			JustSaveManager.Instance.Save();
		}


		JustSaveManager SavedReferenceToTheManager;

		private void ExampleSave2()
		{
			SavedReferenceToTheManager = JustSaveManager.Instance;
			SavedReferenceToTheManager.Save();
		}
	}
	
## How to prepare your game for loading and saving <a name="prepare"></a>

Although, the package will assemble the save automatically, when you call `JustSaveManager.Instance.Save()`, you have to specify, which data the save should include.

### Preparing Scene Objects <a name="sceneObjects"></a>

In the context of JustSave, scene objects refers to objects, which are present at the start of the game. So every object, which is not spawned at runtime.
Every scene object, which should be managed by JustSave, needs a **JustSaveSceneId**-component on it.
You can add this component by navigating to your object in the unity editor, pressing "Add Component" and searching for *JustSaveSceneId*.
Every component on an object with a **JustSaveSceneId**-component and every component on every child object of this object will be searched for savable data.

### Preparing Runtime Objects <a name="runtimeObjects"></a>

A runtime object on the other hand is an object, which is not present at start and will be spawned into the scene at runtime. Luckily, JustSave comes with a convenient solution for runtime objects, called object pooling.

When you spawn an object in unity, you probably use `Instantiate(ObjectToSpawn)` or any overload of this method. Do not use this method in combination with JustSave, except you dont want your spawned object to be saved.  
Spawning objects in JustSave works only slightly different. You prepare your prefabs in unity as usual (Just drag and drop an object into the project-panel), but make sure, that your Prefab has a **JustSaveRuntimeId**-component on it.
To add this component to your object, press "Add Component" in the unity inspector and search for *JustSaveRuntimeId*. Note, that the **JustSaveRuntimeId** should be on the parent object of your prefab.  

Now you have to create an objectpool for this object. The object pool will do the spawning for you and can easily be created from script. Just use `JustSaveManager.Instance.CreateObjectPool(YourPrefab, "YourPrefabId", SizeOfThePool, PoolingMode, NotifyToDespawn);`.
The first argument should be reference to the prefab you created and want to spawn. The second argument is the id, you assign for this prefab. You will need this id later to actually spawn the prefab.

To spawn a prefab with JustSave, use `JustSaveManager.Instance.Spawn("YourPrefabId", SpawnPosition);`. This will also return a reference of type GameObject of the spawned Prefab, so use this reference to your liking.

*code example of how to create an object pool and spawn an object from it:*

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

[Read more...](./OBJECTPOOLING.md)

### The Autosaved Attribute <a name="attribute"></a>

To specify the exact fields, which should be saved and loaded, use the `[Autosaved]`-Attribute. Also make sure, that the field you want to save is `public`, else the **SaveAssembler** can not acess it.
If you don't want your public fields to show up in the unity inspector, I suggest also using the `[HideInInspector]` attribute.

*code example:*

	using JustSave

	class ExampleClass : MonoBehaviour
	{
		[Autosaved]
		public int a;
	}

And that is it. Provided that the object this component is on has a **JustSaveSceneId** or a **JustSaveRuntimeId** attached to it, the integer *a* will be included in the savefile and automatically be synchronised on load.

### The ISavable interface and the Savable class <a name="savables"></a>

The **ISavable**-interface allows you to implement methods, which will be called when certain events related to saving occur. You can add it to any of your classes, that are managed by JustSave.  
This interface is exceptionally important on runtime objects (prefabs, spawned with object pooling), because you have to reset this object every time it is spawned.

The **ISavable**-interface includes the following events:

- void JSOnSave();
- void JSOnLoad();
- void JSOnSpawned();
- void JSOnDespawned();
- void JSOnPooled();
- void JSOnNeeded();

> [How to use JSOnSpawned() to properly spawn runtime objects](./OBJECTPOOLING.md#resettingRuntimeObjects)  
  
> [Learn about custome despawns and how to use JSOnNeeded() to archieve them](./OBJECTPOOLING.md#notifyToDespawn)

*example implementation using the **ISavable**-interface (This class saves and loads the position of an object in unity):*

	using JustSave;
	using UnityEngine;

	public class SaveTransform : MonoBehaviour, ISavable
	{
		[Autosaved]
		[HideInInspector]
		public Vector3 Position;

		public void JSOnSave() {
			Position = transform.position;
		}

		public void JSOnLoad() {
			transform.position = Position;
		}

		public void JSOnSpawned() {}

		public void JSOnDespawned() {}

		public void JSOnPooled() {}

		public void JSOnNeeded() {}
	}


If you do not want to implement all the methods, for example you just need `JSOnSpawned` to reset your prefab when it is spawned, you can also derive your classes from **Savable**.  
The **Savable**-class is an class without functionality which implements **ISavable**. You can then overwrite the methods you need. Note that Savable derives from MonoBehaviour, so you can use Start() and Update() etc. as usual.  

*example implementation using the **Savable**-class (This class saves and loads the position of an object in unity):*

	using JustSave;
	using UnityEngine;

	public class SaveTransform : Savable
	{
		[Autosaved]
		[HideInInspector]
		public Vector3 Position;

		public override void JSOnSave()
		{
			base.JSOnSave();
			Position = transform.position;
		}

		public override void JSOnLoad()
		{
			base.JSOnLoad();
			transform.position = Position;
		}
	}

## How to actually load and save <a name="loadAndSave"></a>

When it comes to performing the load or save, JustSave is very simple. Just call `Save()` or `Load()` in the **JustSaveManager**.

*example implementation:*

	using UnityEngine;
	using JustSave;

	public class ScriptManager : MonoBehaviour
	{
		public void Save()
		{
			JustSaveManager.Instance.Save();
		}

		public void Load()
		{
			JustSaveManager.Instance.Load();
		}
	}

By default, your savefile is saved to the persistent datapath of your application. If you don't know, where that is, don't worry: everything should work just fine.

If you want to specify, where the savefile should be saved to or loaded from, you can change  the filepath, the filename and even set your custome fileending:

- JustSaveManager.SetFileName() sets the name of the savefile or the name of the file to be loaded
- JustSaveManager.SetFileEnding() sets the fileending to be saved or loaded
- JustSaveManager.SetFilePath() sets the path from which the savefile will be loaded or to which it will be saved

*example configuration:*

	using UnityEngine;
	using JustSave;

	public class JustSaveConfiguration : MonoBehaviour
	{
		public void Start()
		{
			JustSaveManager JS = JustSaveManager.Instance;

			JS.SetFileName("MySave");
			JS.SetFileEnding(".supersave");
			JS.SetFilePath(Application.persistentDataPath + "\");
		}
	}



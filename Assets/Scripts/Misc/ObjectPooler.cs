using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
	private static ObjectPoolerPrewarmer prewarmerInstance;

    //private class for organising different pools of gameobjects
    private class Pool
    {
        //List of all gameobjects in the pool
        private List<GameObject> pooledObjects = new List<GameObject>();

        //The prefab that new gameobjects will be instantiated from
        public GameObject prefab;

        public Pool(GameObject prefab)
        {
            //New pools must have a prefab
            this.prefab = prefab;
        }

		public GameObject GetPooledObject(bool register = true)
        {
            //Search for inactive object in pool
            foreach (GameObject o in pooledObjects)
            {
                if (!o.activeSelf)
                {
                    //Activate and return any object found
                    o.SetActive(true);
                    return o;
                }
            }

            if (prefab)
            {
                //If no object was found, instantiate a new one
                GameObject obj = GameObject.Instantiate(prefab);
                //Set name to that of prefab for comparisons
                obj.name = prefab.name;
                //Organised under pooled object "maintenance" gameobject
                obj.transform.SetParent(poolObject.transform);
                //Add new gameobject to pool and return
                pooledObjects.Add(obj);

                if (register && prewarmerInstance)
                    prewarmerInstance.Register(prefab);

                return obj;
            }

            GameObject empty = new GameObject();

            return empty;
        }

        public void Purge()
        {
            //Destroy every gameobject in this pool
            for (int i = 0; i < pooledObjects.Count; i++)
                GameObject.Destroy(pooledObjects[i]);

            pooledObjects.Clear();
        }

        public void ReturnAll()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
                pooledObjects[i].SetActive(false);
        }
    }

    //List of all object pools
    private static List<Pool> objectPools = new List<Pool>();
    //An empty gameobject for organising pooled objects in the scene
    private static GameObject poolObject;

	public static GameObject GetPooledObject(GameObject prefab, bool register = true)
    {
        //Make sure there is a gameobject for organising pooled objects in the scene
        if(!poolObject)
        {
            poolObject = GameObject.Find("PooledObjects");

            //If there was no pre-existing object to hold them, create a new one
            if (!poolObject)
                poolObject = new GameObject("PooledObjects");

			GameObject.DontDestroyOnLoad(poolObject);
        }

		if(!prewarmerInstance)
		{
			prewarmerInstance = GameObject.FindObjectOfType<ObjectPoolerPrewarmer>();
		}

        //Pool starts as null, since one will either be found or created
        Pool pool = null;

        //Attempt to find a pool with the same prefab
        foreach (Pool p in objectPools)
            if (p.prefab == prefab)
                pool = p;

        //If no pool was found...
        if (pool == null)
        {
            //Create a new pool and add it to the list of pools
            pool = new Pool(prefab);
            objectPools.Add(pool);
        }

        //Get a pooled object from the pool and return it
        return pool.GetPooledObject(register);
    }

    //Clears all object pools
    public static void PurgePools()
    {
        //Eacg pool handles its own purging
        foreach (Pool pool in objectPools)
            pool.Purge();

        //Once pools have been purged, clear them
        objectPools.Clear();
    }

    public static void ReturnAll()
    {
        foreach (Pool pool in objectPools)
            pool.ReturnAll();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
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

		public GameObject GetPooledObject(Vector3 position)
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

            //If no object was found, instantiate a new one
            GameObject obj = GameObject.Instantiate(prefab, position, Quaternion.identity);
            //Set name to that of prefab for comparisons
            obj.name = prefab.name;
            //Organised under pooled object "maintenance" gameobject
            obj.transform.SetParent(poolObject.transform);
            //Add new gameobject to pool and return
            pooledObjects.Add(obj);
            return obj;
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

	//Gets a pooled object from one of the object pools, or creates a new pool if one does not exist for this prefab
	public static GameObject GetPooledObject(GameObject prefab)
	{
		return GetPooledObject(prefab, Vector3.zero);
	}


	public static GameObject GetPooledObject(GameObject prefab, Vector3 position)
    {
        //Make sure there is a gameobject for organising pooled objects in the scene
        if(!poolObject)
        {
            poolObject = GameObject.Find("PooledObjects");

            //If there was no pre-existing object to hold them, create a new one
            if (!poolObject)
                poolObject = new GameObject("PooledObjects");
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
        return pool.GetPooledObject(position);
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

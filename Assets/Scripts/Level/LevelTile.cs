using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public List<Transform> doors = new List<Transform>();
	public BoxCollider layoutCollider;

	[Space()]
	[Tooltip("The prefab to spawn when blocking off doors on this tile.")]
	public GameObject blockedDoorPrefab;
	public int minBlocks = 1;
	public int maxBlocks = 2;

	[Space()]
	public Transform tileOrigin;

	[Space()]
	public GameObject walls;

    [Header("Alternate Graphics")]
    [Tooltip("The graphic that will be replaced by the below prefabs.")]
    public GameObject defaultGraphic;

    [Space()]
    public GameObject grassGraphicPrefab;
    public GameObject desertGraphicPrefab;
    public GameObject fireGraphicPrefab;
	public GameObject iceGraphicPrefab;
	public GameObject forestGraphicPrefab;

	public enum Biomes
    {
        Grass,
        Fire,
        Desert,
		Ice,
		Forest
    }

    public bool IsIntersecting(LayerMask layerMask)
	{
		bool intersected = false;

		BoxCollider col = layoutCollider;

		//Get list of all colliders in layer overlapping this one
		List<Collider> others = new List<Collider>(Physics.OverlapBox(col.center + col.transform.position, col.size / 2, col.transform.rotation, layerMask));

		//Remove self from list
		others.Remove(col);

		//If there are colliders in list, it is intersecting
		if (others.Count > 0)
		{
			intersected = true;
		}

		return intersected;
	}

	public Vector3 GetPosInTile(float width, float height)
	{
		BoxCollider col = layoutCollider;

		Vector3 pos = Vector3.zero;

		pos.x = Random.Range(col.bounds.min.x + width / 2, col.bounds.max.x - width / 2);
		pos.z = Random.Range(col.bounds.min.z + height / 2, col.bounds.max.z - height / 2);

		return pos;
	}

	public void BlockDoors()
	{
		if (maxBlocks > doors.Count)
			maxBlocks = doors.Count;

		int blockAmount = Random.Range(minBlocks, maxBlocks + 1);

		int spawnedBlockAmount = 0;

		while(spawnedBlockAmount < blockAmount)
		{
			int index = Random.Range(0, doors.Count);

            if (blockedDoorPrefab)
            {
                GameObject doorObj = Instantiate(blockedDoorPrefab, transform);
                doorObj.transform.position = doors[index].position;
                doorObj.transform.rotation = doors[index].rotation;
            }

			DestroyImmediate(doors[index].gameObject);
			doors.RemoveAt(index);

			spawnedBlockAmount++;
		}
	}

	public void ReplaceDoors()
	{
		for(int i = 0; i < doors.Count; i++)
		{
			GameObject oldObj = doors[i].gameObject;

			ReplaceWithPrefab replace = doors[i].GetComponent<ReplaceWithPrefab>();

			if(replace)
			{
				GameObject newObj = replace.Replace();

				if(newObj)
				{
					doors[i] = newObj.transform;

					DestroyImmediate(oldObj);
				}
			}
		}
	}

    public void Replace(Biomes biome)
    {
        if(defaultGraphic)
        {
            GameObject newGraphic = null;

            //Select prefab to replace based on biome
            switch(biome)
            {
                case Biomes.Grass:
                    newGraphic = grassGraphicPrefab;
                    break;
                case Biomes.Desert:
                    newGraphic = desertGraphicPrefab;
                    break;
                case Biomes.Fire:
                    newGraphic = fireGraphicPrefab;
                    break;
				case Biomes.Ice:
					newGraphic = iceGraphicPrefab;
					break;
				case Biomes.Forest:
					newGraphic = forestGraphicPrefab;
					break;
			}

            //If prefab exists then replace with it
            if(newGraphic)
            {
                GameObject obj = Instantiate(newGraphic, transform);
                obj.transform.localPosition = defaultGraphic.transform.localPosition;
                obj.transform.localRotation = defaultGraphic.transform.localRotation;

                //Destroy layout graphic
                DestroyImmediate(defaultGraphic);
            }
        }

        LevelBlock[] levelBlocks = GetComponentsInChildren<LevelBlock>();

        for(int i = 0; i < levelBlocks.Length; i++)
        {
            levelBlocks[i].Replace(biome);
        }
    }

	public void SetCurrent(LevelTile oldTile)
	{
		if (oldTile)
			StartCoroutine(FadeWalls(CameraFollow.Instance.wallFadeOutTime, CameraFollow.Instance.wallFadeInTime, this, oldTile));
		else
		{
			walls.SetActive(true);

			if (layoutCollider)
				CameraFollow.Instance.UpdateCameraBounds(layoutCollider.bounds);
		}
	}

	IEnumerator FadeWalls(float fadeOutTime, float fadeInTime, LevelTile newTile, LevelTile oldTile)
	{
		GameObject newWalls = newTile.walls;
		GameObject oldWalls = oldTile.walls;

		//Get mesh renderers
		MeshRenderer[] newRends = newWalls.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] oldRends = oldWalls.GetComponentsInChildren<MeshRenderer>();

		//Cache shared material to return at end
		Material sharedMaterial = newRends[0].sharedMaterial;

		//Fade old out
		{
			Material oldMat = oldRends[0].material;

			for (int i = 0; i < oldRends.Length; i++)
				oldRends[i].sharedMaterial = oldMat;

			Color startColor = sharedMaterial.GetColor("_TintColor");
			Color endColor = oldMat.GetColor("_TintColor");
			endColor.a = 0;

			float elapsedTime = 0;

			while (elapsedTime <= fadeOutTime)
			{
				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;

				oldMat.SetColor("_TintColor", Color.Lerp(startColor, endColor, elapsedTime / fadeOutTime));
			}

			oldWalls.SetActive(false);
		}

		//Fade new in
		{
			Material newMat = newRends[0].material;

			for (int i = 0; i < newRends.Length; i++)
				newRends[i].sharedMaterial = newMat;

			Color startColor = sharedMaterial.GetColor("_TintColor");
			startColor.a = 0;
			Color endColor = newMat.GetColor("_TintColor");

			newMat.SetColor("_TintColor", startColor);

			newWalls.SetActive(true);

			if(newTile.layoutCollider)
				CameraFollow.Instance.UpdateCameraBounds(newTile.layoutCollider.bounds);

			float elapsedTime = 0;

			while (elapsedTime <= fadeInTime)
			{
				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;

				newMat.SetColor("_TintColor", Color.Lerp(startColor, endColor, elapsedTime / fadeInTime));
			}

		}

		//Restore shared material
		foreach(MeshRenderer rend in oldRends)
			rend.sharedMaterial = sharedMaterial;

		foreach (MeshRenderer rend in newRends)
			rend.sharedMaterial = sharedMaterial;
	}
}

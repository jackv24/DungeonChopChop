using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepParticles : MonoBehaviour
{
	public Transform leftFoot;
	public Transform rightFoot;

	[Space()]
	public LayerMask groundLayer;

	[System.Serializable]
	public class MaterialParticlePair
	{
		public Material groundMaterial;
		public GameObject particlePrefab;
	}

	[Space()]
	public MaterialParticlePair[] materialPairs;

	private MeshRenderer rend;
	private Collider lastHit;

	public void ParticleFootLeft()
	{
		SpawnParticles(leftFoot);
	}

	public void ParticleFootRight()
	{
		SpawnParticles(rightFoot);
	}

	void SpawnParticles(Transform point)
	{
		if(point)
		{
			RaycastHit hit;

			//Raycast down from foot
			if(Physics.Raycast(point.position, Vector3.down, out hit, 1.0f, groundLayer))
			{
				if (hit.collider != lastHit)
				{
					rend = hit.collider.GetComponent<MeshRenderer>();

					lastHit = hit.collider;
				}

				//If hit a mesh renderer
				if(rend)
				{
					//Get shared mesh renderer material (no need to get instance)
					Material mat = rend.sharedMaterial;

					//Loop through all material pair
					foreach(MaterialParticlePair pair in materialPairs)
					{
						//If a match was found...
						if(pair.groundMaterial == mat && pair.particlePrefab)
						{
							//Spawn particles at foot position
							GameObject obj = ObjectPooler.GetPooledObject(pair.particlePrefab);
							obj.transform.position = hit.point;

							return;
						}
					}
				}
			}
		}
	}
}

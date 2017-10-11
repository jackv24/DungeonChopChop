using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ParticleType
{
    Hit,
    Death
};

[System.Serializable]
public class ParticleAndColor
{
    public GameObject particle;
    public Gradient gradient;
}

[System.Serializable]
public class AmountOfParticleTypes
{
    public ParticleType particleType;
    [Tooltip("Amount of particles in type to chose from randomly")]
    public ParticleAndColor[] particles;
}

public class SpawnEffects : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EffectOnDeath(AmountOfParticleTypes[] typeList, Vector3 spawnPosition)
    {
        //loop through each amount of particles list
        foreach (AmountOfParticleTypes particleType in typeList)
        {
            //check if the particle is a hit type
            if (particleType.particles.Length > 0)
            {
                if (particleType.particleType == ParticleType.Death)
                {
                    //get a random number max being amount of particles
                    int randomParticleNumber = Random.Range(0, particleType.particles.Length);
                    //spawn the particle
                    GameObject particle = ObjectPooler.GetPooledObject(particleType.particles[randomParticleNumber].particle);
                    //sets the particle colors
                    ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = particleType.particles[randomParticleNumber].gradient;
                    //sets the position
                    particle.transform.position = spawnPosition;
                }
            }
        }
    }

    public GameObject GetEffectOnDeath(AmountOfParticleTypes[] typeList)
    {
        foreach (AmountOfParticleTypes particleType in typeList)
        {
            //check if the particle is a hit type
            if (particleType.particles.Length > 0)
            {
                if (particleType.particleType == ParticleType.Death)
                {
                    //get a random number max being amount of particles
                    int randomParticleNumber = Random.Range(0, particleType.particles.Length);
                    //get particle
                    return particleType.particles[randomParticleNumber].particle;
                }
            }
        }
        return null;
    }

    public void EffectOnHit(AmountOfParticleTypes[] typeList, Vector3 spawnPosition)
    {
        //loop through each amount of particles list
        foreach (AmountOfParticleTypes particleType in typeList)
        {
            //check if the particle is a hit type
            if (particleType.particles.Length > 0)
            {
                if (particleType.particleType == ParticleType.Hit)
                {
                    //get a random number max being amount of particles
                    int randomParticleNumber = Random.Range(0, particleType.particles.Length);
                    //spawn the particle
                    GameObject particle = ObjectPooler.GetPooledObject(particleType.particles[randomParticleNumber].particle);
                    //sets the particle colors
                    ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
                    main.startColor = particleType.particles[randomParticleNumber].gradient;
                    //sets the position
                    particle.transform.position = spawnPosition;
                }
            }
        }
    }

    public GameObject GetEffectOnHit(AmountOfParticleTypes[] typeList)
    {
        foreach (AmountOfParticleTypes particleType in typeList)
        {
            //check if the particle is a hit type
            if (particleType.particles.Length > 0)
            {
                if (particleType.particleType == ParticleType.Hit)
                {
                    //get a random number max being amount of particles
                    int randomParticleNumber = Random.Range(0, particleType.particles.Length);
                    //get particle
                    return particleType.particles[randomParticleNumber].particle;
                }
            }
        }
        return null;
    }
}

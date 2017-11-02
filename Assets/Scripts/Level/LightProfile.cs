using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lighting Profile", menuName = "Data/Lighting Profile")]
public class LightProfile : ScriptableObject
{
    public Color sunColor = Color.white;
    public float sunIntensity = 1;

    [Space()]
    public Material skyboxMaterial;
    public float ambientIntensity = 1.0f;
    public Color ambientColour = Color.black;

    [Space()]
    public float playerLightIntensity = 0.0f;
}

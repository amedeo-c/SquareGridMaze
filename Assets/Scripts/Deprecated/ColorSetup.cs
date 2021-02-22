using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSetup : ScriptableObject
{
    [SerializeField]
    public List<Color> cellColors;

    [SerializeField]
    public Color openWallColor;

    [SerializeField]
    public Color closedWallColor;
}

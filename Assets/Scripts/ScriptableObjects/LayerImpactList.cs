using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LayerImpactList", menuName = "Create LayerImpactList", order = 1)]
public class LayerImpactList : ScriptableObject
{
    public List<LayerImpactPair> layerImpactPairs;
}

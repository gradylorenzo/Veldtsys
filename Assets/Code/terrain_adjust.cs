using UnityEngine;
using System.Collections;

public class terrain_adjust : MonoBehaviour {

    public TerrainData uterrain;
    public float[,] theights;
	// Use this for initialization
	void Start ()
    {
        if (uterrain)
        {
            theights = uterrain.GetHeights(0, 0, 2049, 2049);
        }
	}
}

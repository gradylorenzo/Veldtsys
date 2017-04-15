using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreFragmentBehavior : MonoBehaviour {

    bool fallThrough = false;
    public float pauseTime;
	// Use this for initialization
	void Start ()
    {
        StartCoroutine("ClearFragment");
	}
	
	// Update is called once per frame
	void Update ()
    {
        foreach(MeshCollider mc in GetComponentsInChildren<MeshCollider>())
        {
            mc.isTrigger = fallThrough;
        }
    }

    IEnumerator ClearFragment()
    {
        yield return new WaitForSeconds(pauseTime);
        fallThrough = true;
        yield return new WaitForSeconds(pauseTime);
        DestroyObject(gameObject);
    }
}

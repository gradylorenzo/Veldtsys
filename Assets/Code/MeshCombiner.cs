using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour {

	public void CombineMeshes()
    {
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Debug.Log(name + " is combining " + filters.Length + " meshes");

        Mesh finalMesh = new Mesh();

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        for(int a = 0; a< filters.Length; a++)
        {
            if (filters[a].transform == transform)
                continue;
            combiners[a].subMeshIndex = 0;
            combiners[a].mesh = filters[a].sharedMesh;
            combiners[a].transform = filters[a].transform.localToWorldMatrix;
        }

        foreach(MeshFilter mf in GetComponentsInChildren<MeshFilter>())
        {
            if (mf.gameObject != this.gameObject)
            {
                mf.gameObject.SetActive(false);
            }
        }

        finalMesh.CombineMeshes(combiners);
        finalMesh.name = "combinedMesh";

        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        GetComponent<MeshCollider>().sharedMesh = finalMesh;

        transform.rotation = oldRot;
        transform.position = oldPos;
    }

    public void DecombineMeshes()
    {
        this.GetComponent<MeshFilter>().mesh = null;
        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
        {
            mf.gameObject.SetActive(true);
        }
    }
}

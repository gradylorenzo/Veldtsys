using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Core;

public class Mineable : NetworkBehaviour {



    public GameObject brokenPrefab;
    public GameObject[] lootPrefabs;
    public int dropOdds = 25;

    [SyncVar]
    public float strength = 1;

    public void TakeDamage(float d)
    {
        if(strength > 0)
        {
            strength -= d;
        }
    }

    private void Update()
    {
        if(strength <= 0)
        {
            Instantiate(brokenPrefab, transform.position, transform.rotation);

            if (Random.Range(0, 100) <= dropOdds)
            {
                int objToSpawn = Random.Range(0, lootPrefabs.Length);
                var go = Instantiate(lootPrefabs[objToSpawn], transform.position, transform.rotation);
                CmdNetSpawn(go);
            }

            CmdNetDestroy(this.GetComponent<NetworkIdentity>().netId);

            //DestroyObject(gameObject);
        }
    }

    private void OnMouseUpAsButton()
    {
        strength -= .5f;
    }

    [Command]
    public void CmdNetSpawn(GameObject go)
    {
        NetworkServer.Spawn(go);
    }

    [Command]
    public void CmdNetDestroy(NetworkInstanceId obj)
    {
        NetworkServer.Destroy(NetworkServer.FindLocalObject(obj).gameObject);
    }
}

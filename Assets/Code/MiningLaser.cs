using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MiningLaser : NetworkBehaviour {

    public float speed = 0.1f;
    public float range = 14.0f;
    public GameObject hitParticles;

    private RaycastHit hit;

    private void Start()
    {
        hitParticles.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("MOUSE_L"))
        {
            if (Physics.Raycast(transform.position, this.transform.up, out hit, range))
            {
                if (hit.collider.gameObject.tag == "Mineable")
                {
                    hit.collider.gameObject.GetComponent<Mineable>().strength -= speed;
                }

                hitParticles.transform.position = hit.point;
                hitParticles.SetActive(true);
            }
            else
            {
                hitParticles.SetActive(false);
            }
        }
        else
        {
            hitParticles.SetActive(false);
        }


        Debug.DrawRay(transform.position, this.transform.up * range, Color.cyan);
    }
}

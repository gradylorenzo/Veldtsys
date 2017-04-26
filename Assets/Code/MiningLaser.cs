using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningLaser : MonoBehaviour {

    public float speed = 0.1f;
    public float range = 14.0f;
    public GameObject hitParticles;
    public GameObject laserScarDecal;

    public GameObject prongs;
    public GameObject prongsBlur;

    private RaycastHit hit;

    private float prongSpeed = 0.0f;

    private Player playerCore;

    private Vector3 previousHitPoint;

    private void Start()
    {
        hitParticles.SetActive(false);
        playerCore = transform.parent.transform.parent.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (playerCore.fireCont)
        {
            if (Physics.Raycast(transform.position, this.transform.up, out hit, range))
            {
                if (hit.collider.gameObject.tag == "Mineable")
                {
                    hit.collider.gameObject.GetComponent<Mineable>().strength -= speed;
                }

                hitParticles.transform.position = hit.point;
                hitParticles.SetActive(true);


                /*if (QualitySettings.GetQualityLevel() >= 4)
                {
                    if (hit.collider.gameObject.tag != "Water")
                    {
                        if (laserScarDecal)
                        {
                            if (Vector3.Distance(hit.point, previousHitPoint) > 0.01f)
                            {
                                previousHitPoint = hit.point;
                                var go = Instantiate(laserScarDecal, hit.point, Quaternion.Euler(hit.normal));
                                go.transform.parent = hit.collider.transform;
                            }
                        }
                    }
                }*/
            }
            else
            {
                hitParticles.SetActive(false);
            }

            prongSpeed = 1;
        }
        else
        {
            if (prongSpeed > 0)
            {
                prongSpeed -= .02f;
            }
            else
            {
                prongSpeed = 0;
            }

            hitParticles.SetActive(false);
        }

        

        prongs.transform.Rotate(0, prongSpeed * 15, 0);

        Color blurA = new Color(0.5f, 0.5f, 0.5f, Mathf.Clamp((prongSpeed / 3) - 0.25f, 0.0f, 1.0f));
        Color prongsA = new Color(1, 1, 1, 1 - prongSpeed);
        prongsBlur.GetComponent<Renderer>().material.SetColor("_TintColor", blurA);
        prongs.GetComponent<Renderer>().material.SetColor("_Color", prongsA);

        Debug.DrawRay(transform.position, this.transform.up * range, Color.cyan);
    }
}

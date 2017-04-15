using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class miningLaserEffect : MonoBehaviour {

    public float speed = 0.3f;

    private bool showLaser = false;

    private void FixedUpdate()
    {
        this.GetComponent<LineRenderer>().material.SetTextureOffset(5, new Vector2(Time.time * speed, 0));
    }

    private void Update()
    {
        if (Input.GetButton("MOUSE_L"))
        {
            showLaser = true;
        }
        else
        {
            showLaser = false;
        }

        this.GetComponent<LineRenderer>().enabled = showLaser;
    }

}

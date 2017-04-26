using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScar : MonoBehaviour {

    private float progress = 0;
    public float speed = 0.001f;
    public AnimationCurve cooldownCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    public float Lifetime = 20;

    private void Start()
    {
        progress = 0;
    }

    private void Update()
    {
        progress += speed;

        float a = cooldownCurve.Evaluate(progress);

        Color col = new Color(1, 1, 1, 1) * a;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", col);

        if(progress > 2)
        {
            DestroyObject(gameObject);
        }
    }
}

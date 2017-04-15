using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class sky_controller : MonoBehaviour {

    public GameObject playerCamera;

    public Light ChildDL;
    public AnimationCurve BrightnessCurve = new AnimationCurve(new Keyframe(-1, 0), new Keyframe(-0.1f, 0), new Keyframe(0.1f, 1), new Keyframe(1, 1));
    public float sunBrightness = 0.8f;
    public GameObject starSphere;
    public AnimationCurve StarCurve = new AnimationCurve(new Keyframe(-500, 1), new Keyframe(-45, 1), new Keyframe(-20, 0), new Keyframe(500, 0));
    public float starBrightness = 0.35f;
    public GameObject HorizonGlow;
    public bool disableSkyFX;

    private float newBrightness = 0.0f;

    void Update()
    {
        if (ChildDL)
        {
            newBrightness = BrightnessCurve.Evaluate(ChildDL.transform.position.y / 500) * sunBrightness;
            ChildDL.intensity = newBrightness;
        }

        if (starSphere)
        {
            starSphere.transform.rotation = this.transform.rotation;

            float starBrightCo = 0.0f;
            if (ChildDL)
            {
                starBrightCo = StarCurve.Evaluate(ChildDL.transform.position.y);
            }
            starSphere.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color.white * starBrightCo * starBrightness);

            if (playerCamera)
            {
                starSphere.transform.position = playerCamera.transform.position;
            }

            if (!disableSkyFX)
            {
                starSphere.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                starSphere.GetComponent<Renderer>().enabled = false;
            }
        }

        if(HorizonGlow)
        {
            if (playerCamera)
            {
                HorizonGlow.transform.position = playerCamera.transform.position;
            }

            if (!disableSkyFX)
            {
                HorizonGlow.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                HorizonGlow.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}

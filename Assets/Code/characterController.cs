using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class characterController : MonoBehaviour {

    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;

    [System.Serializable]
    public class JumpSystem
    {
        public float Fuel = 100.0f;
        public float Recover = 0.1f;
        public float Usage = 0.5f;
        public float Force = 25.0f;
        public AudioSource[] jetSounds;
        public AnimationCurve jetPitch = new AnimationCurve(new Keyframe(0, .01f), new Keyframe(60, 1), new Keyframe(100, 1));
    }

    public JumpSystem Jump;

    public GameObject aiFollowPoint;

    Rigidbody rbody;
    bool isGrounded;

    float translation;
    float strafe;

    // Use this for initialization
    void Start ()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        rbody = this.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetButton("SPRINT"))
        {
            translation = Input.GetAxis("WALK") * runSpeed * Time.deltaTime;
            strafe = Input.GetAxis("STRAFE") * runSpeed * Time.deltaTime;
        }
        else
        {
            translation = Input.GetAxis("WALK") * walkSpeed * Time.deltaTime;
            strafe = Input.GetAxis("STRAFE") * walkSpeed * Time.deltaTime;
        }

        transform.Translate(strafe, 0, translation);

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
  
        foreach (AudioSource a in Jump.jetSounds)
        {
            a.pitch = Jump.jetPitch.Evaluate(Jump.Fuel);
        }

        if (Jump.Fuel > 0)
        {
            if (Input.GetButton("JUMP"))
            {
                rbody.AddForce((Vector3.up * (Jump.Force * ((Jump.Fuel / 100) + .25f))));
                Jump.Fuel -= Jump.Usage;
                foreach (AudioSource a in Jump.jetSounds)
                {
                    a.mute = false;
                }
            }
            else
            {
                foreach (AudioSource a in Jump.jetSounds)
                {
                    a.mute = true;
                }
            }
        }
        else
        {
            foreach (AudioSource a in Jump.jetSounds)
            {
                a.mute = true;
            }
        }

        if (Jump.Fuel < 100)
        {
            if (!Input.GetButton("JUMP"))
            {
                Jump.Fuel += Jump.Recover;
            }
        }

        if(Jump.Fuel < 0)
        {
            Jump.Fuel = 0.0f;
        }

        if(Jump.Fuel > 100)
        {
            Jump.Fuel = 100.0f;
        }
    }
}

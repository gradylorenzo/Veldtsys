using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEditor;

namespace Core
{
    public class vcUnetCommon : NetworkBehaviour
    {
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

    public class vcPlayer : NetworkBehaviour
    {
        //Core Data Class for unchanging player information
        [System.Serializable]
        public class PlayerData
        {
            public string Name;
            public string ID;
        }

        //Core behaviour class for player objects
        [RequireComponent(typeof(Rigidbody))]
        public class PlayerBehaviour : NetworkBehaviour
        {
            public PlayerData Data;
            public List<vcItems.Item> Inventory;
            public float Health;
            public int credits = 0;

            private bool enableLocalControl;

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

            Rigidbody rbody;
            float translation;
            float strafe;

            public bool fireFirst;
            public bool fireLast;
            public bool fireCont;

            void Start()
            {
                if (isLocalPlayer)
                {
                    foreach(Camera c in GetComponentsInChildren<Camera>())
                    {
                        c.enabled = true;
                    }
                    this.GetComponentInChildren<AudioListener>().enabled = true;
                    this.GetComponentInChildren<camMouseLook>().enabled = true;
                    this.GetComponent<PlayerInterface>().enabled = true;
                    //this.GetComponent<FirstPersonController>().enabled = true;
                    GameObject sc = GameObject.FindGameObjectWithTag("SkyController");
                    if (sc)
                    {
                        sc.GetComponent<sky_controller>().playerCamera = this.gameObject;
                        sc.GetComponent<sky_controller>().disableSkyFX = false;
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("StartCam"))
                    {
                        go.GetComponent<Camera>().enabled = false;
                    }
                }
                else
                {

                }

                rbody = this.GetComponent<Rigidbody>();
            }

            private void FixedUpdate()
            {
                if (isLocalPlayer) {
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

                    if (Jump.Fuel < 0)
                    {
                        Jump.Fuel = 0.0f;
                    }

                    if (Jump.Fuel > 100)
                    {
                        Jump.Fuel = 100.0f;
                    }

                    //Core boolean for controlling weapons
                    fireFirst = Input.GetButtonDown("MOUSE_L");
                    fireLast = Input.GetButtonUp("MOUSE_L");
                    fireCont = Input.GetButton("MOUSE_L");
                }
                else
                {
                    return;
                }
            }

            void OnTriggerStay(Collider other)
            {
                if (other.gameObject.tag == "Pickup")
                {
                    if (Input.GetButtonDown("INTERACT"))
                    {
                        if (other.GetComponent<Pickup>().Type == vcPickup.Pickup.PickupType.item)
                        {
                            addItemToInventory(other.GetComponent<Pickup>().Item);
                            print("picked up " + other.GetComponent<Pickup>().Item.displayedName+"!");
                        }
                        else if (other.GetComponent<Pickup>().Type == vcPickup.Pickup.PickupType.credit)
                        {
                            addCredits(other.GetComponent<Pickup>().Credits);
                            print("picked up credits!");
                        }
                        CmdNetDestroy(other.GetComponent<NetworkIdentity>().netId);
                    }
                }
            }

            public void addItemToInventory(vcItems.Item newItem)
            {
                foreach(vcItems.Item i in Inventory)
                {
                    if(i.ID == newItem.ID)
                    {
                        i.quantity += newItem.quantity;
                        return;
                    }
                }

                Inventory.Add(newItem);
            }

            public void addCredits(vcItems.Credit newCredit)
            {
                credits += newCredit.amount;
            }

            [Command]
            public void CmdNetDestroy(NetworkInstanceId obj)
            {
                NetworkServer.Destroy(NetworkServer.FindLocalObject(obj).gameObject);
            }
        }
    }

    public class vcItems
    {
        [System.Serializable]
        public class Item
        {
            public string ID = "ID_NOT_SET";
            public string displayedName = "ID Not Set";
            public int quantity;
            public bool isConsumable = false;
            public int value = 0;
            public Texture2D icon;
        }

        [System.Serializable]
        public class Credit
        {
            public int amount = 0;
        }
    }

    public class vcPickup : NetworkBehaviour
    {

        [RequireComponent (typeof(Rigidbody))]
        public class Pickup : NetworkBehaviour
        {
            public enum PickupType
            {
                item,
                credit
            }

            public PickupType Type;

            public vcItems.Credit Credits;
            public vcItems.Item Item;

            public void Awake()
            {
                if(this.tag != "Pickup")
                {
                    Debug.LogWarning("Pickup not tagged properly!");
                }
                
            }
        }
    }

    public class vcInterface : NetworkBehaviour
    {
        public class Notification
        {

        }

        public class PlayerInterface : MonoBehaviour
        {
            public Texture2D healthBar;
            public Texture2D jumpBar;

            void OnGUI()
            {
                var Jump = this.GetComponent<Player>().Jump;
                var Health = this.GetComponent<Player>().Health;

                GUI.Label(new Rect(5, Screen.height - 50, 100, 50), "Health:" + Health.ToString("n2"));
                GUI.DrawTexture(new Rect(100, Screen.height - 40, Health * 4, 10), healthBar);
                GUI.Label(new Rect(5, Screen.height - 30, 100, 50), "Fuel:" + Jump.Fuel.ToString("n2"));
                GUI.DrawTexture(new Rect(100, Screen.height - 20, Jump.Fuel * 4, 10), jumpBar);

                if (Input.GetKey(KeyCode.Tab))
                {
                    GUILayout.BeginArea(new Rect(5, 5, 200, Screen.width / 2));
                    GUILayout.BeginVertical();
                    GUILayout.Label("Credits: " + GetComponent<Player>().credits);
                    GUILayout.Space(10);
                    foreach (vcItems.Item i in GetComponent<Player>().Inventory)
                    {
                        GUILayout.Label(i.displayedName + " x" + i.quantity);
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
            }
        }
    }

    public class vcGeneration : NetworkBehaviour
    {
        [CustomEditor(typeof(OreDistributionEditor))]
        public class OreDistributionEditor : Editor
        {
            
        }

        public void FindOreDistributionPoints()
        {
            
        }
    }
}

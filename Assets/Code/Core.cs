using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

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
        public class PlayerBehaviour : NetworkBehaviour
        {
            public PlayerData Data;
            public List<vcItems.Item> Inventory;
            public float Health;
            public int credits = 0;

            private vcUnetCommon ucommon;

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
                    this.GetComponent<characterController>().enabled = true;
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
                
                print(this.GetComponent<NetworkIdentity>().netId);


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
                var Jump = this.GetComponent<characterController>().Jump;
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
}

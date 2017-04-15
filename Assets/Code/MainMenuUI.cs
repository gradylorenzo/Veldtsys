using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(NetworkManager))]
public class MainMenuUI : MonoBehaviour
{
    private NetworkManager manager;
    
    [System.Serializable]
    public enum MenuStates
    {
        Login,
        Main,
        Play,
        Settings,
        Exit,
        Game
    }


    public MenuStates State = MenuStates.Main;
    public Texture2D logo;
    public Texture2D loading;
    public GUISkin Skin;

    private bool showLoading = false;

    private void Awake()
    {
        manager = this.GetComponent<NetworkManager>(); 
    }

    private void OnGUI()
    {
        if (Skin)
        {
            GUI.skin = Skin;
        }

        if (logo && State != MenuStates.Game)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), logo);
        }

        if (showLoading)
        {
            GUI.DrawTexture(new Rect(0, Screen.height - loading.height, loading.width, loading.height), loading);
            showLoading = false;
        }


        GUILayout.BeginArea(new Rect(10, 10, Screen.width / 10, Screen.height - 10));
        switch (State)
        {
            //Login screen
            case MenuStates.Login:
                State = MenuStates.Main;
                break;

            //Main menu
            case MenuStates.Main:
                if (GUILayout.Button("Play"))
                {
                    State = MenuStates.Play;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Settings"))
                {
                    State = MenuStates.Settings;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Exit"))
                {
                    State = MenuStates.Exit;
                }
                break;

            //Play Menu
            case MenuStates.Play:
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                if (GUILayout.Button("Start as Client"))
                {
                    State = MenuStates.Game;
                    showLoading = true;
                    manager.StartClient();
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Start as Host"))
                {
                    State = MenuStates.Game;
                    showLoading = true;
                    manager.StartHost();
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Back"))
                {
                    State = MenuStates.Main;
                }
                break;

            //Settings Menu
            case MenuStates.Settings:
                if (GUILayout.Button("Back"))
                {
                    State = MenuStates.Main;
                }
                break;

            //Exit confirmation
            case MenuStates.Exit:
                GUILayout.Label("Are you sure you want to exit?");

                if (GUILayout.Button("Yes"))
                {
                    print("closing");
                    Application.Quit();
                }
                GUILayout.Space(10);
                if (GUILayout.Button("No"))
                {
                    State = MenuStates.Main;
                }
                break;

            //idek
            default:
                break;
        }

        GUILayout.EndArea();

        
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.InputSystem.XR;
using UnityEditor;
using UnityEngine.SceneManagement;

public class RoleGiver : AttributesSync, IInteractable
{
    public GameObject hunterCanvas;
    public GameObject preyCanvas;

    public GameObject newPrefab;

    public PlayerStates playerStates;
    public Multiplayer networkManager;

    public WinTrigger wt;
    public MapMover mm;



    int hunterIndex = 0;

    public GameObject GiveObject()
    {
        return gameObject;
    }

    public void InitInteract(string interactor)
    {
        //Calls interact method, resets all player values. Ensures game restarts don't end due to leftover variables from last game. - Ibrahim
        playerStates.StateReset();
        BroadcastRemoteMethod("Interact", interactor);
    }

    [SynchronizableMethod]
    public void Interact(string interactor)
    {
        //On interact with the GameObject that houses this script: - Ibrahim
        if (playerStates.Players.Count > 1)
        {
            
           
                //The players and all networking components move between the game map and the lobby. - Ibrahim
                mm.MoveMaps();
            

            for (int i = 0; i < playerStates.Players.Count; i++)
            {
                Alteruna.Avatar avatar = playerStates.Players[i].GetComponent<Alteruna.Avatar>();
                //The host gets turned into a hunter via the SwitchPrefab script. - Ibrahim
                if (i == hunterIndex)
                {
                    if (!avatar.IsMe)
                        return;
                    SwitchPrefab(hunterIndex);
                    //Hunter UI is turned on. - Ibrahim
                    hunterCanvas.SetActive(true);
                }
                else
                {
                    if (!avatar.IsMe)
                        return;
                    //For all other players, the prey UI is turned on instead. - Ibrahim
                    preyCanvas.SetActive(true);
                }
            }
        }
    }



    [SynchronizableMethod]

    public void SwitchPrefab(int i)
    {
        //Disables the prey and enables the hunter components of the player. - Ibrahim
        Transform parentTransform = playerStates.Players[i].transform;

        // Find the child GameObjects by name - Ibrahim
        Transform firstChild = parentTransform.Find("PreyComponent");
        Transform secondChild = parentTransform.Find("HunterComponent");


        // Transfer the position from the first child to the second child - Ibrahim

        secondChild.position = Vector3.zero;
        secondChild.rotation = firstChild.rotation;

        //Enable the hunter, then disable the prey component. - Ibrahim
        if (firstChild != null && secondChild != null)
        {
            secondChild.gameObject.SetActive(true);
            firstChild.gameObject.SetActive(false);
        }
    }

    

    public void Tag(GameObject tagger, GameObject tagged)
    {
        //If a player is tagged, they are sent to the jail's co-ordinates. - Ibrahim
        tagged.transform.position = new Vector3(63.7f, 10.58f, -17.28f);
    }

    void Start()
    {
        //Both UI canvases are turned off at the beginning to avoid overlap between the two. - Ibrahim
        hunterCanvas.SetActive(false);
        preyCanvas.SetActive(false);
        //Scene scene = SceneManager.GetActiveScene();
        
        //if (mm==null&&scene.name=="Game_Map")
        //{
        //    mm = wt.mapMover;
        //}
    }

    void Update()
    {   
        networkManager = FindAnyObjectByType<Multiplayer>();
        playerStates = networkManager.GetComponent<PlayerStates>();
    }


    public void ResetAllPrefabs()
    {
        //resets positions of players - Ibrahim
        //List<GameObject> _players = playerStates.Players;
        networkManager = FindAnyObjectByType<Multiplayer>();

        playerStates = networkManager.GetComponent<PlayerStates>();

        foreach (var obj in playerStates.Players)
        {
            
            //Debug.Log("reseting"+obj.name);
            Transform parentTransform = obj.transform;

            Transform firstChild = parentTransform.Find("PreyComponent");
            Transform secondChild = parentTransform.Find("HunterComponent");

            // Find the child GameObjects' positions by name
            Transform firstChildPosition = parentTransform.Find("PreyComponent").Find("PlayerAndBody");

            Transform secondChildPosition = parentTransform.Find("HunterComponent").Find("PlayerAndBody");


            if (secondChild != null && secondChild.gameObject.activeSelf)
            {
                // Transfer the position from the first child to the second child
                secondChildPosition.position = firstChildPosition.position;
                secondChildPosition.rotation = firstChildPosition.rotation;
            
                if (!firstChild.gameObject.activeSelf)
                {

                    // Turn the first GameObject on
                    firstChild.gameObject.SetActive(true);

                    // Turn the second GameObject off
                    secondChild.gameObject.SetActive(false);
                    firstChildPosition.transform.position = new Vector3(84f, 16.44f, 128);
                }
            }else if (firstChild != null && firstChild.gameObject.activeSelf)
            {

                if (!secondChild.gameObject.activeSelf)
                {
                    firstChildPosition.transform.position = new Vector3(64.5f, 16.44f, 100);
                }
            }


        }
    }

}
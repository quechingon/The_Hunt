using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using System;

public class PlayerStates : MonoBehaviour
{
    //Global values. Gather number of players based on their states. Holds info for spawn locations. Resets all states on new game start. - Ibrahim
    public List<GameObject> escapedPlayers = new List<GameObject>();
    public List<GameObject> taggedPlayers = new List<GameObject>();

    //Legacy code for random  hunter spawns. Removed due to the game becoming unfair. - Ibrahim
    //public List<Vector3> spawns = new List<Vector3>()
    //{
    //    new Vector3(5,1,28),
    //    new Vector3(-11,1, 27),
    //    new Vector3(-14.5f,1, 4),
    //    new Vector3(9,1, 6) 
    //};

    //Lists for all the types of players. Used repetitively through the stages of gameplay. - Ibrahim
    public List<GameObject> players;
    public List<GameObject> prey;
    public List<GameObject> hunters;

    public RoleGiver roleGiver;
    public AttributesSync sync;
    //public EndGameController endGameController;

    public bool gameStarted;
    public bool gameEnded;
    public bool allPlayersTagged;
    public bool hasReset = false;

    public int lastPlayerIndex;

    public void StateReset()
    {
        //Resets all common values except for the diamonds. Happens before game begins and after it ends. - Ibrahim
        foreach (GameObject prey in prey)
        {
            P_StateManager state = prey.GetComponentInChildren<P_StateManager>();
            state.Escaped = false;
            state.Caught = false;
        }
        escapedPlayers.Clear();
        taggedPlayers.Clear();        
         
        
        allPlayersTagged = false;
        gameStarted = false;
        gameEnded = false;
        hasReset = true;
        //PlayerForceSync();
    }

    public void playerEscaped(GameObject player)
    {
        if (!escapedPlayers.Contains(player))
        {
            escapedPlayers.Add(player);
            Debug.Log(player + " Escaped");
        }
        
    }

    public void playerTagged(GameObject player)
    {
        if(!taggedPlayers.Contains(player)) 
        { 
            taggedPlayers.Add(player);
        }
    }

    public void PlayerForceSync()
    {
        //Used to force all clients to sync together. - Ibrahim
        sync = roleGiver.Multiplayer.gameObject.GetComponent<AttributesSync>();

        sync.ForceSync();
        
        //for (int i = 0; i < Players.Count; i++)
        //{
        //    if ((i == lastPlayerIndex))
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        Alteruna.Avatar avatar = Players[i].GetComponent<Alteruna.Avatar>();
        //        Debug.Log("HunterForceSync worked up until the Sync");
        //        sync.ForceSync();
        //        Debug.Log("ForceSync was called.");
        //    }

        //}
    }

    public void Update()
    { 
        players = FindObjectsOnLayer(9);
        prey = FindObjectsOnLayer(7);
        hunters = FindObjectsOnLayer(6);
        roleGiver = FindObjectOfType<RoleGiver>();

        if (!gameEnded && !gameStarted && !hasReset)
        {
            if (roleGiver == null)
            {
                Debug.Log("rolgiver  null");
            }
            StateReset();
        }

    }



    public List<GameObject> Players { get { return players; } }
    public List<GameObject> Prey { get { return prey; } }
    public List<GameObject> Hunters { get { return hunters; } }

    public List<GameObject> FindObjectsOnLayer(int layer)
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> objectsInLayer = new List<GameObject>();

        foreach (var obj in allPlayers)
        {
            if (obj.layer == layer)
            {
                objectsInLayer.Add(obj);
            }
        }
        return objectsInLayer;
    }
}

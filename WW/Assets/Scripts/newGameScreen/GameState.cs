﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{

    //Attributes
    private int turnOfPlayer; //turn of: P1 or P2
    private bool turnComplete;
    public static bool tileTaken;
    public GameObject PlayerScrolls; //Access to scroll gameobjects
    public GameObject TilePositions; //Access to gameObject that is the parent of all tile placement positions;
    public List<GameObject> P1Barriers; //Access to the Player's light barriers
    public List<GameObject> P2Barriers; //Access to the Enemy's light barriers
    public List<GameObject> TilePrefabs; //List of 4 elements prefabs 
    public List<GameObject> P1HandTiles;
    public List<GameObject> P2HandTiles;
    private int p1BarrierCount, p2BarrierCount; /*Barrier functionality to be added.*/
    
    
    //Functions
    void Start()
    {
        tileTaken = false;
        turnOfPlayer = 1;  
        turnComplete = false;

        p1BarrierCount = p2BarrierCount = 5;
        
        HandSetup(1); 
        HandSetup(2);
        
        ShowTiles(turnOfPlayer);
    }
    void Update()
    {
        if (turnComplete)
        {
            turnOver();
        }
    }

    public void addTile()
    { //Called by the tilePouch
        if (!tileTaken)
        {
            ScrollClick.selectedScroll = "none"; //to unselect any selected scroll when pouch is pressed
            int temp = Random.Range(0, 9999) % 4;
            if (turnOfPlayer == 1 && P1HandTiles.Count < 4)
            {
                P1HandTiles.Add(Instantiate(TilePrefabs[temp]));
                P1HandTiles[P1HandTiles.Count - 1].name = TilePrefabs[temp].name + (P1HandTiles.Count - 1).ToString();
                P1HandTiles[P1HandTiles.Count - 1].SetActive(true);
            }
            else if (turnOfPlayer == 2 && P2HandTiles.Count < 4)
            {
                P2HandTiles.Add(Instantiate(TilePrefabs[temp]));
                P2HandTiles[P2HandTiles.Count - 1].name = TilePrefabs[temp].name + (P2HandTiles.Count - 1).ToString();
                P2HandTiles[P2HandTiles.Count - 1].SetActive(true);
            }

            ShowTiles(turnOfPlayer);
            tileTaken = true;
        }
    }
    
    void HandSetup(int playerNumber)
    { //to set tiles in each player's hand. Called in the Start function above.
        
        int initial_tiles = 3;
        for (int i = 0; i < initial_tiles; i++)
        {
            int temp = Random.Range(0, 9999) % 4;

            if (playerNumber == 1)
            {
                P1HandTiles.Add(Instantiate(TilePrefabs[temp]));
                P1HandTiles[i].name = TilePrefabs[temp].name + i.ToString(); //appends the index to the element name. To be used in deselecting all other tiles.
                P1HandTiles[i].SetActive(false);
                
            }
            else if (playerNumber == 2)
            {
                P2HandTiles.Add(Instantiate(TilePrefabs[temp]));
                P2HandTiles[i].name = TilePrefabs[temp].name + i.ToString(); //appends the index to the element name. To be used in deselecting all other tiles.
                P2HandTiles[i].SetActive(false);
            }
        }
    }
    void ShowTiles(int playerNumber)
    {//To display the tiles in hand of the player whose turn it is. Called in the Start function and turnOver function
        if (playerNumber == 1)
        {
            //Hide Tiles of the opponent
            for (int i = 0; i < P2HandTiles.Count; i++)
                P2HandTiles[i].SetActive(false);

            //Show tiles of the player
            for (int i = 0; i < P1HandTiles.Count; i++)
            {
                P1HandTiles[i].transform.position = TilePositions.transform.GetChild(i).transform.position;
                P1HandTiles[i].SetActive(true);
            }
        }
        else if (playerNumber == 2)
        {
            //Hide Tiles of the opponent
            for (int i = 0; i < P1HandTiles.Count; i++)
                P1HandTiles[i].SetActive(false);
            
            //Show tiles of the player
            for (int i = 0; i < P2HandTiles.Count; i++)
            {
                P2HandTiles[i].transform.position = TilePositions.transform.GetChild(i).transform.position;
                P2HandTiles[i].SetActive(true);
            }
        }
    }

    public static void addToScroll(GameObject tile)
    {//Called by the Select function in tileSpriteChanger
        FindObjectOfType<GameState>().tileToScroll(tile);
    }
    void tileToScroll(GameObject tile)
    {//Called by the static function addToScroll
        int temp = System.Convert.ToInt32(ScrollClick.selectedScroll);
        SpellContainer scrollToAddTo = PlayerScrolls.transform.GetChild(temp).GetComponent<SpellContainer>();
        bool added = scrollToAddTo.addTile(turnOfPlayer, tile.name);
        if (added)
        {
            if (turnOfPlayer == 1)
            {
                P1HandTiles.Remove(tile);
                Destroy(tile);
            }
            else
            {
                P2HandTiles.Remove(tile);
                Destroy(tile);
            }
            turnComplete = true; //Once a tile is placed, switch players.
        }
    }

    public void turnOver()
    {//Called in Update when turnOver bool is true

        //reset values: 
        tileTaken = false;
        turnComplete = false;
        ScrollClick.selectedScroll = "none";
        tileSpriteChanger.selectedTile = "none";

        //Set turn to the other Player
        turnOfPlayer = (turnOfPlayer == 1) ? 2 : 1;

        for (int i = 0; i < PlayerScrolls.transform.childCount; i++)
        {
            PlayerScrolls.transform.GetChild(i).GetComponent<SpellContainer>().switchPlayers(turnOfPlayer);
        }
        ShowTiles(turnOfPlayer);
    }
}

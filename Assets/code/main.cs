﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class HelperMethods
{
    public static void updateForces(GameObject gameObject, KeyInputType keyPressed, float horizontal)
    {
        // Vector2 position = gameObject.transform.position;
        float jumpVel = 0;
        
        var controllerScript = gameObject.GetComponent<CharacterController>();
        if (controllerScript.isGrounded && keyPressed == KeyInputType.Jump)
        {
            jumpVel = 600;
            controllerScript.isGrounded = false;
        }
        Vector2 force = new Vector2(horizontal * Time.deltaTime * 1000, jumpVel);
        gameObject.GetComponent<Rigidbody2D>().AddForce(force);
        // position.x = position.x + 5.0f * horizontal * Time.deltaTime;
        // position.y = position.y + 5.0f * vertical * Time.deltaTime;
        // gameObject.transform.position = position;
    }
}

public class main : MonoBehaviour
{
    private GameObject currCharacter;
    private GameObject nextCharacter;
    private CharacterLife currCharacterLife;

    public Texture2D lincolnIcon;
    public Texture2D grumpyIcon;

    private GameObject luigiPrefab;
    private GameObject lincolnPrefab;
    private GameObject grumpyPrefab;

    private List<CharacterLife> characterLives;

    void Start()
    {
        characterLives = new List<CharacterLife>();
        luigiPrefab = Resources.Load<GameObject>("very_important_asset");
        lincolnPrefab = Resources.Load<GameObject>("LincolnPrefab");
        grumpyPrefab = Resources.Load<GameObject>("GrumpyPrefab");
        // Debug.Log("truly what");

        nextCharacter = lincolnPrefab;
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (currCharacter != null && currCharacterLife != null)
            {
                characterLives.Add(currCharacterLife);
                foreach (CharacterLife life in characterLives)
                {
                    life.ResetToSpawn();
                }
            }

            //currCharacter = Instantiate(luigiPrefab);
            currCharacter = Instantiate(nextCharacter);
            currCharacterLife = new CharacterLife(currCharacter);
        }
        else if (currCharacter) 
        {
            KeyInputType keyPressed = KeyInputType.None;
            if (Input.GetKey("up"))
            {
                keyPressed = KeyInputType.Jump;
            }
            if (Input.GetKey("left"))
            {
                keyPressed = KeyInputType.Left;
            }
            if (Input.GetKey("right"))
            {
                keyPressed = KeyInputType.Right;
            }

            float horizontal = Input.GetAxis("Horizontal");
            // float vertical = Input.GetAxis("Vertical");
            HelperMethods.updateForces(currCharacter, keyPressed, horizontal);
            currCharacterLife.TrackInput(Time.deltaTime, keyPressed, horizontal);

            foreach (CharacterLife life in characterLives)
            {
                life.UpdateFromHistory(Time.deltaTime);
            }
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), lincolnIcon))
        {
            nextCharacter = lincolnPrefab;
            print("EMANCIPATION TIME");
        }

        if (GUI.Button(new Rect(10, 70, 100, 50), grumpyIcon))
        {
            nextCharacter = grumpyPrefab;
            print("Feelin' Grompy");
        }
    }
}

public enum KeyInputType
{
    None,
    Left,
    Right,
    Jump
}


public class CharacterLife
{
    public List<Tuple<float, KeyInputType, float>> history;
    private int currentPositionInArray;
    private GameObject unityObject;
    Vector3 initTransformPosition; 

    public CharacterLife(GameObject obj)
    {
        history = new List<Tuple<float, KeyInputType, float>>();
        currentPositionInArray = 0;
        unityObject = obj;
        initTransformPosition = unityObject.transform.position;
    }

    public void TrackInput(float timeDelta, KeyInputType keyPressed, float horizontal)
    {
        history.Add(new Tuple<float, KeyInputType, float>(timeDelta, keyPressed, horizontal));
    }

    public void UpdateFromHistory(float timeDelta)
    {
        if (currentPositionInArray < history.Count)
        {
            HelperMethods.updateForces(unityObject, 
                history[currentPositionInArray].Item2, 
                history[currentPositionInArray].Item3
            );
            currentPositionInArray++;
        }
    }

    public void ResetToSpawn()
    {
        unityObject.transform.position = initTransformPosition;
        currentPositionInArray = 0;
    }
}
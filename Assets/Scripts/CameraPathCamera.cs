﻿using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraPathCamera : MonoBehaviour {
    //List<GameObject> nodes;
	[SerializeField]
	public CameraPathNode currentNode;
	[SerializeField]
	float speedMultiplier;
	[SerializeField]
	float accelerationDampener = 0.01f; 
	[SerializeField]
	bool changeDirection = false; 
	[SerializeField]
	float minY, maxY;
    [SerializeField]
    float maxOrthographicSize;
    [SerializeField]
    float minOrthographicSize;
    [SerializeField]
    float distBetweenPlayers;
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    float minSpeed;
    [SerializeField]
    float drag;

    [SerializeField]
    float speed;

    [SerializeField]
    Vector3 direction;

    GameObject[] players;
    int i = 0; 
    // Use this for initialization
    void Start ()
    {
        //nodes = GameObject.FindGameObjectsWithTag("Node");
		players = GameObject.FindGameObjectsWithTag ("Player"); 
		//Array.Sort (nodes, delegate(GameObject node1, GameObject node2) { return node1.name.CompareTo(node2.name); }); 
		//currentNode = nodes[1];
	}
	
	// Update is called once per frame
	void Update ()
    {
        float minPlayerPos = float.MaxValue;
        float maxPlayerPos = float.MinValue;

        //Increase Speed Multiplier
        foreach (GameObject x in players) {
			if (x.transform.position.x > transform.position.x + 3 && speedMultiplier < maxSpeed) {
				speedMultiplier = speedMultiplier + (x.transform.position.x - transform.position.x) * accelerationDampener;
                
            }
            if (x.transform.position.y < minPlayerPos)
            {
                minPlayerPos = x.transform.position.y;
            }
            if (x.transform.position.y > maxPlayerPos)
            {
                maxPlayerPos = x.transform.position.y;
            }
        }

        //find distance between players
        distBetweenPlayers = maxPlayerPos - minPlayerPos;

        if (distBetweenPlayers >= GetComponent<Camera>().orthographicSize)
        {
			if (GetComponent<Camera> ().orthographicSize < maxOrthographicSize)
				GetComponent<Camera> ().orthographicSize *= 1.01f;
        }
		else if (GetComponent<Camera>().orthographicSize > minOrthographicSize && GetComponent<Camera>().orthographicSize * 0.97 > distBetweenPlayers )
        {
            GetComponent<Camera>().orthographicSize *= 0.99f;
        }

		//Add Drag to Speed Multiplier
		if (speedMultiplier > minSpeed) {
			speedMultiplier = speedMultiplier - drag;
		}

		//Desired Next position
		Vector3 nextPosition = new Vector3(calculateXPos(), calculateYPos(), transform.position.z);

		direction = nextPosition - transform.position;
		direction.Normalize ();
        direction.y += (nextPosition.y - transform.position.y)*5;

		//Move
		speed = speedMultiplier * Time.deltaTime;
		transform.position += direction *  speed;


		//Did we reach the node?
		if (currentNode.transform.position.x > transform.position.x - 2 && currentNode.transform.position.x < transform.position.x + 2)
		{
			if (currentNode.nextNode != null) {
				currentNode = currentNode.nextNode;
			}
		}
    }

	float calculateYPos()
	{
		float averageYpos = currentNode.transform.position.y; 
		foreach(GameObject x in players)
			averageYpos += x.transform.position.y;

		averageYpos /= players.Length + 1;
		averageYpos = Mathf.Min (maxY - GetComponent<Camera>().orthographicSize, Mathf.Max (minY + GetComponent<Camera>().orthographicSize, averageYpos));
		return (averageYpos);
	}

	float calculateXPos()
	{
		return currentNode.transform.position.x;
	}

    public void reset()
    {
        speedMultiplier = 1.0f;
    }
}

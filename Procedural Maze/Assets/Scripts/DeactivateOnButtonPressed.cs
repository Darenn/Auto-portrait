﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnButtonPressed : MonoBehaviour {

    public GameObject ToDeactivate;
    public KeyCode Key;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(Key))
        {
            ToDeactivate.SetActive(!ToDeactivate.active);
        }	
	}
}

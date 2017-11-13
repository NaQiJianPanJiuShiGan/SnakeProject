using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using SGF;
using Snaker.Service.Core.Example;

public class AppMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debuger.EnableLog = true;
        Example exa = new Example();
        exa.Init();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
   
}

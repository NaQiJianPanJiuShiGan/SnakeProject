using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGF;

public class TestLog : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debuger.EnableLog = true;
        Debuger.EnableSave = true;
        this.Log("调试测试");
        Debuger.Log(Debuger.LogFileDir);
        this.LogError("调试测试");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

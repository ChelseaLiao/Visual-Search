using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRception;

public class NextScene : MonoBehaviour
{

    public string blockName;
    public DataLogger logger;

    public string nextScene = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            DataManager dm = GameObject.Find("DataManager").GetComponent<DataManager>();
            dm.nextScene(blockName, nextScene);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class WaitBlock : MonoBehaviour
    {
        public float Duration = 360.0f;
        public string blockName;
        public DataLogger logger;
        public float timer = 0.0f;
        enum STATES { ready, start, wait, end };
        STATES state_block = STATES.ready;

        public string nextScene = "";

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            LogData();
            long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (state_block == STATES.start)
            {
                logger.writeState(now, blockName, "start");
                state_block = STATES.wait;
            }
            else if(state_block == STATES.wait)
            {
                timer += Time.deltaTime;
            }
            if(state_block == STATES.wait && timer >= Duration)
            {
                logger.writeState(now, blockName, "end");

                DataManager dm = GameObject.Find("DataManager").GetComponent<DataManager>();
                dm.nextScene(blockName, nextScene);
            }

        }
        public void LogData()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (state_block == STATES.ready)
                {
                    state_block = STATES.start;
                }
            }
            
        }
    }
}


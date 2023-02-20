using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class TheFirstTwoBlocks : MonoBehaviour
    {
        public float Duration = 360.0f;
        public string blockName;
        public CSVWriter writer;
        float timer = 0.0f;
        enum STATES { ready, start, wait, end };
        STATES state_block = STATES.ready;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            LogData();

            if (state_block == STATES.start)
            {
                Debug.Log(blockName + "start");
                long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                writer.writeState(blockName, "start", timestamp);
                state_block = STATES.wait;
            }
            else if(state_block == STATES.wait)
            {
                timer += Time.deltaTime;
            }
            if(state_block == STATES.wait && timer >= Duration)
            {
                Debug.Log(blockName + "over");
                long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                writer.writeState(blockName, "end", timestamp);
                state_block = STATES.end;
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
                else if (state_block == STATES.end)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
            
        }
    }
}


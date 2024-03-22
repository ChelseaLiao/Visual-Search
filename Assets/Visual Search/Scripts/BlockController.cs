using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static VRception.DataManager;

namespace VRception
{
    public class BlockController : MonoBehaviour
    {
        //public static int reality_counter = -1;
        //public static int difficulty_counter = -2;
        //public static string reality_name = "";
        //public static string difficulty_name = "";
        //public static string blockName="";

        public string blockName = "";

        public enum DIFFICULTY { none, easy, difficult };

        private SCENE currentScene = SCENE.None;

        public int currentBlockNumber;
        public int currentCondition;
        public DIFFICULTY currentDifficulty;
        public string currentActuality;

        public DataManager dataManager;
        GameObject virtualScene;

        // Start is called before the first frame update
        void Start()
        {
            prepare();
        }


        // Update is called once per frame
        void Update()
        {
            if (currentScene == SCENE.None) {
                prepare();
            }
        }

        private void prepare() {
            currentScene = dataManager.getCurrentScene();


            if (currentScene == SCENE.Task | currentScene == SCENE.Questionnaire)
            {
                currentBlockNumber = dataManager.getBlockCounter();
               
                List<int> order = dataManager.getOrder();
                if (order.Count == 0)
                {
                    //Debug.LogError("BlockController: The DataManager has no Order set.");
                    currentScene = SCENE.None;
                }
                else
                {
                    currentCondition = order[currentBlockNumber];

                    switch (currentCondition)
                    {
                        case 0:
                        case 1:
                            currentActuality = "AR";
                            if (Settings.instance != null)
                            {
                                Settings.instance.crossfader = -0.3f;
                            }
                            break;
                        case 2:
                        case 3:
                            currentActuality = "AV";
                            if (Settings.instance != null)
                            {
                                Settings.instance.crossfader = 0.3f;
                            }
                            break;
                        case 4:
                        case 5:
                            currentActuality = "VR";
                            if (Settings.instance != null)
                            {
                                Settings.instance.crossfader = 1.0f;
                            }
                            break;
                        default:
                            currentActuality = "nan";
                            break;
                    }

                    switch (currentCondition)
                    {
                        case 0:
                        case 2:
                        case 4:
                            currentDifficulty = DIFFICULTY.easy;
                            break;
                        case 1:
                        case 3:
                        case 5:
                            currentDifficulty = DIFFICULTY.difficult;
                            break;
                        default:
                            currentDifficulty = DIFFICULTY.none;
                            break;
                    }


                    blockName = currentActuality + "-" + currentDifficulty;
                    //Debug.Log("BlockController: blockName:" + blockName);
                }
            }
            else if (currentScene == SCENE.Training)
            {
                blockName = "training";
                //Debug.Log("BlockController: blockName:" + blockName);
            }
            else if (currentScene == SCENE.None)
            {
                Debug.LogError("BlockController: The DataManager has no SCENE set.");
                currentScene = SCENE.None;
            }
            else
            {
                Debug.LogError("BlockController: Block Name Undefind");
                currentScene = SCENE.None;
            }

            if (currentScene == SCENE.Task)
            {
                virtualScene = GameObject.Find("studio");
                if (currentActuality == "AR")
                    virtualScene.SetActive(false);
                else
                    virtualScene.SetActive(true);

            }

        }

        internal string getBlockName()
        {
            return blockName;
        }

        internal DIFFICULTY getCurrentDifficulty()
        {
            return currentDifficulty;
        }
    }
}



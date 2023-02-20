using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class CSVWriter : MonoBehaviour
    {
        // string fileName ="";
        public string rootFolder = "./LogData/";
        //public EnvironmentController env;
        public string playerID = "";
        public static string ParticipantId = "";
        StreamWriter swSelection;
        StreamWriter swState;
        StreamWriter swFixation;
        StreamWriter swItem;

        StreamWriter swSelection_Training;
        StreamWriter swFixation_Training;
        StreamWriter swItem_Training;

        // Start is called before the first frame update
        void Start()
        {
            ////        The Start function cannot be called but only init()

            rootFolder = rootFolder + "./ID-" + playerID + "/";
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }

            string filepath = rootFolder + "experiment_data.csv";

            if (File.Exists(filepath))
            {
                Debug.LogError("The file is exist " + filepath);
#if UNITY_EDITOR
                //UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif

            }

            init();
            // fileName= Application.dataPath + "/experiment_data.csv";
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        private void init()
        {
            ParticipantId = playerID;
            Debug.Log(ParticipantId);

            Debug.Log("Init Files");
            string filepath = rootFolder + "experiment_data.csv";

            if(swState == null)
            {
                filepath = rootFolder + "/" + "ID-" + playerID + "-state.csv";
                swState = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                swState.WriteLine("Block,State,Time");
                swState.Flush();
            }

            if(SceneManager.GetActiveScene().buildIndex == 3)
            {
                if (swSelection == null)
                {
                    filepath = rootFolder + "/" + "ID-" + playerID + "-performance.csv";
                    swSelection = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                    swSelection.WriteLine("SpawnTime,Target,isTarget,PressTime,ClickTime,EndTime,Block");
                    swSelection.Flush();
                }

                if (swFixation == null)
                {
                    filepath = rootFolder + "/" + "ID-" + playerID + "-cross.csv";
                    swFixation = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                    swFixation.WriteLine("Block,Duration,X_Coordination,Y_Coordination,Z_Coordination,SpawnTime");
                    swFixation.Flush();
                }

                if (swItem == null)
                {
                    filepath = rootFolder + "/" + "ID-" + playerID + "-item.csv";
                    swItem = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                    swItem.WriteLine("Block,Pattern,Item,Tag,X_Coordination,Y_Coordination,Z_Coordination");
                    swItem.Flush();
                }
            }

            else if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                if (swSelection_Training == null)
                {
                    filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-performance.csv";
                    swSelection_Training = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                    swSelection_Training.WriteLine("SpawnTime,Target,isTarget,PressTime,ClickTime,EndTime");
                    swSelection_Training.Flush();
                }

                if (swFixation_Training == null)
                {
                    filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-cross.csv";
                    swFixation_Training = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                    swFixation_Training.WriteLine("Duration,X_Coordination,Y_Coordination,Z_Coordination,SpawnTime");
                    swFixation_Training.Flush();
                }

                if (swItem_Training == null)
                {
                    filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-item.csv";
                    swItem_Training = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                    swItem_Training.WriteLine("Pattern,Item,Tag,X_Coordination,Y_Coordination,Z_Coordination");
                    swItem_Training.Flush();
                }
            }
            

        }

        public void writeSelection(long startTimestamp, string targetName, bool isTarget, long pressTimestamp, long clickTimestamp, long endTimestamp, string blockName)
        {
            
            if (swSelection == null)
            {
                init();
            }
            swSelection.WriteLine(startTimestamp + "," + targetName + "," + isTarget  + "," + pressTimestamp  + "," + clickTimestamp  + ","+ endTimestamp + ","+ blockName);
            swSelection.Flush();
        }

        public void writeState(string blockName, string stateName, long Timestamp)
        {
            
            if (swState == null)
            {
                init();
            }
            swState.WriteLine(blockName + "," + stateName+ "," + Timestamp);
            swState.Flush();
        }

        public void writeCross(string blockName, int duration, Vector3 crossPosition, long Timestamp)
        {
            
            if (swFixation == null)
            {
                init();
            }
            swFixation.WriteLine(blockName + "," + duration + "," + crossPosition + "," + Timestamp);
            swFixation.Flush();
        }

        public void writeItems(string blockName, int pattern, string itemName, string itemTag, Vector3 itemPosition)
        {
            
            if (swItem == null)
            {
                init();
            }
            swItem.WriteLine(blockName + "," + pattern + "," + itemName + "," + itemTag + "," + itemPosition);
            swItem.Flush();
        }

        public void writeSelection_Training(long startTimestamp, string targetName, bool isTarget, long pressTimestamp, long clickTimestamp, long endTimestamp)
        {
            
            if (swSelection_Training == null)
            {
                init();
            }
            swSelection_Training.WriteLine(startTimestamp + "," + targetName + "," + isTarget  + "," + pressTimestamp  + "," + clickTimestamp  + ","+ endTimestamp);
            swSelection_Training.Flush();
        }

        public void writeCross_Training(int duration, Vector3 crossPosition, long Timestamp)
        {
            
            if (swFixation_Training == null)
            {
                init();
            }
            swFixation_Training.WriteLine(duration + "," + crossPosition + "," + Timestamp);
            swFixation_Training.Flush();
        }

        public void writeItems_Training(int pattern, string itemName, string itemTag, Vector3 itemPosition)
        {
            
            if (swItem_Training == null)
            {
                init();
            }
            swItem_Training.WriteLine(pattern + "," + itemName + "," + itemTag + "," + itemPosition);
            swItem_Training.Flush();
        }
        
    }
}


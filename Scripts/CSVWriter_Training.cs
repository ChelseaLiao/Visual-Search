using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace VRception
{
    public class CSVWriter_Training : MonoBehaviour
    {
        // string fileName ="";
        public string rootFolder = "./LogData/";
        //public static string playerID = "";
        //public EnvironmentController env;
        public string playerID = "";
        StreamWriter swSelection;
        StreamWriter swState;
        StreamWriter swFixation;
        StreamWriter swItem;

        // Start is called before the first frame update
        void Start()
        {
            rootFolder = rootFolder + "./ID-" + playerID + "/"; 
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }

            string filepath = rootFolder + "experiment_data.csv";

            if (File.Exists(filepath)) {
                Debug.LogError("The file is exist "+filepath);
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
            Debug.Log("Init Files");
            string filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-experiment_data.csv";

            if (swSelection == null)
            {
                filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-performance.csv";
                swSelection = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                swSelection.WriteLine("SpawnTime,Target,isTarget,PressTime,ClickTime,EndTime");
                swSelection.Flush();
            }

            //if (swState == null)
            //{
            //    filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-state.csv";
            //    swState = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
            //    swState.WriteLine("State,Time");
            //    swState.Flush();
            //}
            if (swState == null)
            {
                filepath = rootFolder + "/" + "ID-" + playerID + "-state.csv";
                swState = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                swState.WriteLine("Block,State,Time");
                swState.Flush();
            }

            if (swFixation == null)
            {
                filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-cross.csv";
                swFixation = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                swFixation.WriteLine("Duration,X_Coordination,Y_Coordination,Z_Coordination,SpawnTime");
                swFixation.Flush();
            }

            if (swItem == null)
            {
                filepath = rootFolder + "/" + "ID-" + playerID + "-training" + "-item.csv";
                swItem = (!File.Exists(filepath)) ? File.CreateText(filepath) : File.AppendText(filepath);
                swItem.WriteLine("Pattern,Item,Tag,X_Coordination,Y_Coordination,Z_Coordination");
                swItem.Flush();
            }
            

        }

        public void writeSelection(long startTimestamp, string targetName, bool isTarget, long pressTimestamp, long clickTimestamp, long endTimestamp)
        {
            
            if (swSelection == null)
            {
                init();
            }
            swSelection.WriteLine(startTimestamp + "," + targetName + "," + isTarget  + "," + pressTimestamp  + "," + clickTimestamp  + ","+ endTimestamp);
            swSelection.Flush();
        }

        public void writeState(string blockName, string stateName, long Timestamp)
        {
            
            if (swState == null)
            {
                init();
            }
            swState.WriteLine(blockName + "," + stateName + "," + Timestamp);
            swState.Flush();
        }

        public void writeCross(int duration, Vector3 crossPosition, long Timestamp)
        {
            
            if (swFixation == null)
            {
                init();
            }
            swFixation.WriteLine(duration + "," + crossPosition + "," + Timestamp);
            swFixation.Flush();
        }

        public void writeItems(int pattern, string itemName, string itemTag, Vector3 itemPosition)
        {
            
            if (swItem == null)
            {
                init();
            }
            swItem.WriteLine(pattern + "," + itemName + "," + itemTag + "," + itemPosition);
            swItem.Flush();
        }
        
    }
}


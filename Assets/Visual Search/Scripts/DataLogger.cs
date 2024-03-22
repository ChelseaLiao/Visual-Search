using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VRception
{
    public class DataLogger : MonoBehaviour
    {
        public static string rootFolder = "./LogData/";

        StreamWriter swState;

        StreamWriter swTask;
        StreamWriter swSelection;
        StreamWriter swCross;
        //StreamWriter swIsi;
        StreamWriter swItem;

        StreamWriter swGaze;
        StreamWriter swGazeSR;
        StreamWriter swEeg;

        StreamWriter swQuestionnaire;

        StringBuilder stringbuilderEeg = new StringBuilder();
        StringBuilder stringbuilderGazeSR = new StringBuilder();

        private int countedEeg = 0;
        private int countedGazeSR = 0;

        private int participantId;

        // Start is called before the first frame update
        void Start()
        {
            participantId = DataManager.ParticipantIdSTATIC;

            rootFolder = rootFolder.Replace(" ", "");
            if (!rootFolder.EndsWith("/")) {
                rootFolder = rootFolder + "/";
            }
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }

            Debug.Log("Start logging Files at: " + rootFolder + "ID-" + participantId);
            init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private StreamWriter open(string filepath, string header) {
            StreamWriter sw;
            if (!File.Exists(filepath)) {
                sw = File.CreateText(filepath);
                sw.WriteLine(header);
                sw.Flush();
            }
            else {
                sw = File.AppendText(filepath);
            }
            return sw;
        }

        private void init()
        {
            
            string filepath;
            if (swState == null)
            {
                filepath =  rootFolder + "ID-" + participantId + "-state.csv";
                swState = open(filepath, "Time,Block,State");
            }

            if (swTask == null)
            {
                filepath = rootFolder + "ID-" + participantId + "-task.csv";
                swTask = open(filepath, "Time,Block,TaskCount,Pattern,State");
            }
            
            if (swSelection == null)
            {
                filepath =  rootFolder + "ID-" + participantId + "-performance.csv";
                swSelection = open(filepath, "Time,Block,TaskCount,Target,ClickedItem,IsTarget");
            }

            if (swCross == null)
            {
                filepath =  rootFolder + "ID-" + participantId + "-cross.csv";
                swCross = open(filepath, "Time,Block,TaskCount,Duration,X,Y,Z");
            }

            // if (swIsi == null)
            // {
            //     filepath =  rootFolder + "ID-" + participantId + "-isi.csv";
            //     swIsi = open(filepath, "Time,Block,TaskCount,State");
            // }

            if (swItem == null)
            {
                filepath =  rootFolder + "ID-" + participantId + "-item.csv";
                swItem = open(filepath, "Time,Block,TaskCount,Item,Tag,X,Y,Z");
            }

            if (swGaze == null)
            {
                filepath = rootFolder + "ID-" + participantId + "-gaze.csv";
                swGaze = open(filepath, "Time,Block,TaskCount,Item,Tag,X,Y,Z");
            }


            /*if (swGazeSR == null) { 
                filepath = rootFolder + "ID-" + participantId + "-gazeSR.csv";
                swGazeSR = open(filepath, string.Join(",", DataLoggerEye.ColumnNames));
            }*/
            

            if (swQuestionnaire == null)
            {
                filepath = rootFolder + "ID-" + participantId + "-questionnaire.csv";
                swQuestionnaire = open(filepath, "Time,Block,QuestionType,Question,QuestionID,Answer");
            }

            if (swEeg == null)
            {
                filepath = rootFolder + "ID-" + participantId + "-EEG.csv";
                swEeg = open(filepath, "Time,TimeLsl,Fp1,Fz,F3,F7,F9,FC5,FC1,C3,T7,CP5,CP1,Pz,P3,P7,P9,O1,Oz,O2,P10,P8,P4,CP2,CP6,T8,C4,Cz,FC2,FC6,F10,F8,F4,Fp2");
            }
        }

        public void writeSelection(long timeStamp, string blockName, int taskCount, string targetName, string clickedItem, bool isTarget)
        {

            if (swSelection == null)
            {
                Debug.LogWarning("swSelection is null");
                init();
            }
            swSelection.WriteLine(timeStamp + "," + blockName + "," + taskCount + "," + targetName + "," + clickedItem + "," + isTarget);
            swSelection.Flush();
        }

        public void writeState(long timeStamp, string blockName, string stateName)
        {

            if (swState == null)
            {
                Debug.LogWarning("swState is null");
                init();
            }
            swState.WriteLine(timeStamp + "," + blockName + "," + stateName);
            swState.Flush();
        }

        public void writeTask(long timeStamp, string blockName, int taskCount, string patternName, string stateName)
        {

            if (swTask == null)
            {
                Debug.LogWarning("swState is null");
                init();
            }
            swTask.WriteLine(timeStamp + "," + blockName + "," + taskCount + "," + patternName + "," + stateName);
            swTask.Flush();
        }
        public void writeCross(long timeStamp, string blockName, int taskCount, float duration, Vector3 crossPosition)
        {

            if (swCross == null)
            {
                Debug.LogWarning("swFixation is null");
                init();
            }
            swCross.WriteLine(timeStamp + "," + blockName + "," + taskCount + "," + duration + "," + crossPosition.x + "," + crossPosition.y + "," + crossPosition.z);
            swCross.Flush();
        }
        // public void writeISI(long timeStamp, string blockName, int taskCount, string stateName)
        // {

        //     if (swIsi == null)
        //     {
        //         Debug.LogWarning("swIsi is null");
        //         init();
        //     }
        //     swIsi.WriteLine(timeStamp + "," + blockName + "," + taskCount + "," + stateName);
        //     swIsi.Flush();
        // }

        public void writeItems(long timeStamp, string blockName, int taskCount, string itemName, string itemTag, Vector3 itemPosition)
        {

            if (swItem == null)
            {
                Debug.LogWarning("swItem is null");
                init();
            }
            swItem.WriteLine(timeStamp + "," + blockName + "," + taskCount + "," + itemName + "," + itemTag + "," + itemPosition.x + "," + itemPosition.y + "," + itemPosition.z);
            swItem.Flush();
        }

        public void writeGaze(long timeStamp, string blockName, int pattern, string itemName, string itemTag, Vector3 itemPosition)
        {

            if (swGaze == null)
            {
                init();
            }
            swGaze.WriteLine(timeStamp + "," + blockName + "," + pattern + "," + itemName + "," + itemTag + "," + itemPosition.x + "," + itemPosition.y + "," + itemPosition.z);
            swGaze.Flush();
        }

        /*public void writeGazeSR(string[] logData)
        {
            Debug.Log("writeGazeSR");
            if (swGazeSR == null)
            {
                init();
            }

            stringbuilderGazeSR.AppendLine(string.Join(",", logData));

            countedGazeSR++;
            Debug.Log("write data " + countedGazeSR);
            if (countedGazeSR % 1000 == 0)
            {
                swGazeSR.WriteLine(stringbuilderGazeSR);
                stringbuilderGazeSR.Clear();
                swGazeSR.Flush();
            }
        }*/


        
        internal void write(string name, SignalSample1D s)
        {
            if (swEeg == null)
            {
                init();
            }
            if (name.ToLower() == "eeg")
            {

                if (s.values.Length == 33)
                {
                    stringbuilderEeg.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}{35}", s.time, s.timeLsl, s.values[0], s.values[1], s.values[2], s.values[3], s.values[4], s.values[5], s.values[6], s.values[7], s.values[8], s.values[9], s.values[10], s.values[11], s.values[12], s.values[13], s.values[14], s.values[15], s.values[16], s.values[17], s.values[18], s.values[19], s.values[20], s.values[21], s.values[22], s.values[23], s.values[24], s.values[25], s.values[26], s.values[27], s.values[28], s.values[29], s.values[30], s.values[31], Environment.NewLine);
                }
                else if (s.values.Length == 32)
                {
                    //Debug.Log("electrode count:" + s.values.Length);
                    stringbuilderEeg.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33}{34}", s.time, s.timeLsl, s.values[0], s.values[1], s.values[2], s.values[3], s.values[4], s.values[5], s.values[6], s.values[7], s.values[8], s.values[9], s.values[10], s.values[11], s.values[12], s.values[13], s.values[14], s.values[15], s.values[16], s.values[17], s.values[18], s.values[19], s.values[20], s.values[21], s.values[22], s.values[23], s.values[24], s.values[25], s.values[26], s.values[27], s.values[28], s.values[29], s.values[30], s.values[31], Environment.NewLine);
                }
                else if (s.values.Length == 65)
                {
                    stringbuilderEeg.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66}{67}", s.time, s.timeLsl, s.values[0], s.values[1], s.values[2], s.values[3], s.values[4], s.values[5], s.values[6], s.values[7], s.values[8], s.values[9], s.values[10], s.values[11], s.values[12], s.values[13], s.values[14], s.values[15], s.values[16], s.values[17], s.values[18], s.values[19], s.values[20], s.values[21], s.values[22], s.values[23], s.values[24], s.values[25], s.values[26], s.values[27], s.values[28], s.values[29], s.values[30], s.values[31], s.values[32], s.values[33], s.values[34], s.values[35], s.values[36], s.values[37], s.values[38], s.values[39], s.values[40], s.values[41], s.values[42], s.values[43], s.values[44], s.values[45], s.values[46], s.values[47], s.values[48], s.values[49], s.values[50], s.values[51], s.values[52], s.values[53], s.values[54], s.values[55], s.values[56], s.values[57], s.values[58], s.values[59], s.values[60], s.values[61], s.values[62], s.values[63], Environment.NewLine);
                }
                else if (s.values.Length == 64)
                {
                    stringbuilderEeg.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65}{66}", s.time, s.timeLsl, s.values[0], s.values[1], s.values[2], s.values[3], s.values[4], s.values[5], s.values[6], s.values[7], s.values[8], s.values[9], s.values[10], s.values[11], s.values[12], s.values[13], s.values[14], s.values[15], s.values[16], s.values[17], s.values[18], s.values[19], s.values[20], s.values[21], s.values[22], s.values[23], s.values[24], s.values[25], s.values[26], s.values[27], s.values[28], s.values[29], s.values[30], s.values[31], s.values[32], s.values[33], s.values[34], s.values[35], s.values[36], s.values[37], s.values[38], s.values[39], s.values[40], s.values[41], s.values[42], s.values[43], s.values[44], s.values[45], s.values[46], s.values[47], s.values[48], s.values[49], s.values[50], s.values[51], s.values[52], s.values[53], s.values[54], s.values[55], s.values[56], s.values[57], s.values[58], s.values[59], s.values[60], s.values[61], s.values[62], s.values[63], Environment.NewLine);
                }
                else
                {

                    throw new NotImplementedException("Your electrode count is not 32 please ajust the script");
                }

                countedEeg++;
                //Debug.Log("write data " + countedEeg);
                if (countedEeg % 1000 == 0)
                {
                    swEeg.WriteLine(stringbuilderEeg);

                    //swEeg.WriteLine(stringbuilderEeg);
                    stringbuilderEeg.Clear();
                    swEeg.Flush();
                }
            }

            else
            {
                Debug.LogWarning("Logger Data Dropped");
            }
        }

        void OnDestroy()
        {
            List<StreamWriter> lst = new List<StreamWriter> { swSelection, swState, swTask, swCross, swItem, swGaze, swEeg, swQuestionnaire }; //swGazeSR

            foreach (StreamWriter sw in lst)
            {
                sw.Flush();
                sw.Close();
            }
            
        }

        internal void writeQuestionnaire(StringBuilder contentOfResult)
        {
            swQuestionnaire.Write(contentOfResult);
            swQuestionnaire.Flush();
        }
    }
}
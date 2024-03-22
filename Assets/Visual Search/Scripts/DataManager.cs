using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class DataManager : MonoBehaviour
    {
        public enum SCENE { None, Validation, ClosedEye, Resting, Training, Task, Questionnaire };
        public SCENE currentScene;
        public int currentBlock;

        private List<List<int>> latinSquare;
        public List<int> order;

        public static int ParticipantIdSTATIC = 1;


        public int participantId = 1;

        public bool ResetParticipantId;

        // Start is called before the first frame update
        void Start()
        {

            latinSquare = getLatinSquareDesign6();

            if (ResetParticipantId == true) {
                PlayerPrefs.SetInt("ParticipantId", 0);
                PlayerPrefs.SetInt("BlockCounter", 0);
            }

            int pid = PlayerPrefs.GetInt("ParticipantId");
            
            if (pid == 0)
            {
                participantId = getValidityOfParticipantId(participantId);
                
                order = latinSquare[participantId % 6];
                PlayerPrefs.SetInt("ParticipantId", participantId);
                ParticipantIdSTATIC = participantId;
            }
            else {
                participantId = pid;
                order = latinSquare[participantId % 6];
                ParticipantIdSTATIC = participantId;
            }
            Debug.Log("Current PlayerPrefs ParticipantId" + participantId);
        }

        private List<List<int>> getLatinSquareDesign6()

        {
            List<List<int>> latinSquare = new List<List<int>>();
            latinSquare.Add(new List<int>{ 0, 1, 5, 2, 4, 3});
            latinSquare.Add(new List<int>{ 1, 2, 0, 3, 5, 4});
            latinSquare.Add(new List<int>{ 2, 3, 1, 4, 0, 5});
            latinSquare.Add(new List<int>{ 3, 4, 2, 5, 1, 0});
            latinSquare.Add(new List<int>{ 4, 5, 3, 0, 2, 1});
            latinSquare.Add(new List<int>{ 5, 0, 4, 1, 3, 2});
            return latinSquare;
        }

        internal List<int> getOrder()
        {
            return order;
        }

        internal SCENE getCurrentScene()
        {
            return currentScene;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private int getValidityOfParticipantId(int startId) {
            string filepath = DataLogger.rootFolder + "ID-" + startId + "-state.csv";

            if (!File.Exists(filepath))
            {
                return startId; 
            }

            int newID = startId;
            while (true) {
                newID++;
                filepath = DataLogger.rootFolder + "ID-" + newID + "-state.csv";

                if (!File.Exists(filepath))
                {
                    Debug.LogWarning("The ParticipantId " + startId + " did exist. Changed to" + newID);
                    return newID;
                }
            }
        }

        public int getBlockCounter()
        {
            currentBlock = PlayerPrefs.GetInt("BlockCounter");
            return currentBlock;
        }

        private void nextBlock() {
            PlayerPrefs.SetInt("BlockCounter", PlayerPrefs.GetInt("BlockCounter") + 1);
            currentBlock = PlayerPrefs.GetInt("BlockCounter");
        }

        internal int getParticipantId()
        {
            return participantId;
        }

        public void nextScene(string currentScene, string nextScene) {
            Debug.Log("DataManager: currect: " + currentScene + " and next: " + nextScene);

            DataLoggerEye dataLoggerEye = GameObject.Find("Logger").GetComponent<DataLoggerEye>();
            if (dataLoggerEye != null) {
                dataLoggerEye.StopLogging();
            }

            if (currentScene == "Questionnaire")
            {
                if (getBlockCounter() >= 5)
                {
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                }
                else
                {
                    this.nextBlock();
                    SceneManager.LoadScene(nextScene);
                }

            }
            else if (currentScene == "Task")
            {
                SceneManager.LoadScene(nextScene);
            }
            else if (currentScene == "ClosingEyes")
            {
                SceneManager.LoadScene(nextScene);
            }
            else if (currentScene == "Resting")
            {
                SceneManager.LoadScene(nextScene);
            }
            else if (currentScene == "Training")
            {
                SceneManager.LoadScene(nextScene);
            }
            else if (currentScene == "Validation")
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.LogError("DataManager: Current scene not handled, currect: " + currentScene + " and next: " + nextScene);
            }
        }
    }
}
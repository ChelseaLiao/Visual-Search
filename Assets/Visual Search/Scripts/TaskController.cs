using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class TaskController : MonoBehaviour
    {
        [SerializeField] public int ItemCount_Easy;
        [SerializeField] public int ItemCount_Difficult;
        [SerializeField] public float[] fixationDurationList;

        public GameObject startMenu;
        public GameObject visualSearchTask;
        public GameObject fixationCross;
        public BlockController blockController;

        public DataLogger dataLogger;

        public float patternDuration = 4.0f;

        public enum STATES {running, wait, search, found, searchDone, isi, fixation, end };
        
        STATES state_block = STATES.wait;
        public STATES state_pattern = STATES.running;
                        
        public int maxNumerOfTasks;
        public float maxFixationDuration;

        public long now;
        public long startTime;
        public long startFixation;
        public long endFixation;
        public long startISI;
        public long endISI;

        float isiTimer;
        public int itemCount = 0;
        public int taskCount = -1;
        public bool isTraning = false;


        // Start is called before the first frame update
        void Start()
        {
            visualSearchTask.SetActive(false);
            fixationCross.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (itemCount == 0)
            {
                if (isTraning == false)
                {
                    if (blockController.getCurrentDifficulty() == BlockController.DIFFICULTY.easy)
                        itemCount = ItemCount_Easy;
                    else if (blockController.getCurrentDifficulty() == BlockController.DIFFICULTY.difficult)
                        itemCount = ItemCount_Difficult;
                }
                else {
                    itemCount = ItemCount_Easy;
                }
            }
            else
            {
                if (state_block == STATES.running)
                {
                    if (state_pattern == STATES.fixation)
                    {
                        if (fixationCross.activeSelf == false)
                        {
                            startFixation = now;
                            maxFixationDuration = fixationDurationList[Random.Range(0, 3)];
                            fixationCross.SetActive(true);
                            dataLogger.writeCross(startFixation, blockController.getBlockName(), getTaskCount(), maxFixationDuration, fixationCross.transform.position);
                            dataLogger.writeTask(startFixation, blockController.getBlockName(), getTaskCount(), "cross", "start");

                        }
                        else
                        {
                            if ((now - startFixation) >= maxFixationDuration)
                            {
                                endFixation = now;
                                fixationCross.SetActive(false);
                                dataLogger.writeCross(endFixation, blockController.getBlockName(), getTaskCount(), maxFixationDuration, fixationCross.transform.position);
                                dataLogger.writeTask(endFixation, blockController.getBlockName(), getTaskCount(), "cross", "end");
                                startVisualSearchTask();
                                state_pattern = STATES.search;
                            }
                        }
                    }

                    if (state_pattern == STATES.search)
                    {
                        //When player clicks item during the pattern
                        if (SelectRayCaster.IsInteracted == true)
                        {
                            dataLogger.writeSelection(SelectRayCaster.pressTime, blockController.getBlockName(), getTaskCount(), visualSearchTask.GetComponent<VisualSearchTask>().target.name, SelectRayCaster.clickedItem, SelectRayCaster.IsTarget);

                            resetVisualSearchTask();
                            state_pattern = STATES.found;

                        }
                    }

                    if (state_pattern == STATES.search && (now - startTime) >= patternDuration)
                    {
                        state_pattern = STATES.searchDone;

                        dataLogger.writeSelection(now, blockController.getBlockName(), getTaskCount(), visualSearchTask.GetComponent<VisualSearchTask>().target.name, "Missed", false);
                    }
                    else if (state_pattern == STATES.found && (now - startTime) >= patternDuration)
                    {
                        state_pattern = STATES.searchDone;
                    }

                    if (state_pattern == STATES.searchDone)
                    {
                        SelectRayCaster.IsTarget = false;
                        SelectRayCaster.pressTime = 0;
                        SelectRayCaster.clickedItem = "NULL";
                        resetVisualSearchTask();
                        resetGame();
                        //state_pattern = STATES.isi;
                        
                        
                    }

                    if (state_pattern == STATES.isi)
                    {
                        endISI = now;
                        //isiTimer += Time.deltaTime;
                        if (endISI - startISI >= 1000)
                        {
                            dataLogger.writeTask(endISI, blockController.getBlockName(), getTaskCount(), "isi", "end");
                            taskCount += 1;
                            if (maxNumerOfTasks < getTaskCount())
                            {
                                state_block = STATES.end;
                            }
                            else
                            {
                                state_pattern = STATES.fixation;
                            }
                        }
                    }
                }
                else
                {

                    StartButtonEvent();
                }

                if (state_block == STATES.end)
                {
                    dataLogger.writeState(now, blockController.getBlockName(), "end");
                    DataManager dm = GameObject.Find("DataManager").GetComponent<DataManager>();
                    if (isTraning == false) { 
                        dm.nextScene("Task", "09_Questionnaire");
                    } 
                    else
                    {
                        float accuracy = (float)SelectRayCaster.accuracy/maxNumerOfTasks*100.0f;
                        if(accuracy< 80.0){
                            startMenu.SetActive(true);
                            startMenu.GetComponentInChildren<TextMesh>().text = "Training is over! The accuracy is " + accuracy +"%";
                            state_block = STATES.wait;
                            state_pattern = STATES.running;
                            taskCount = 1;
                        }
                        else
                            dm.nextScene("Training", "05_VRception");
                    }
                }
            }
        }

        private void StartButtonEvent()
        {
            if (Input.GetKeyDown(KeyCode.S)){
                long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                dataLogger.writeState(now, blockController.getBlockName(), "start");
                state_block = STATES.running;
                state_pattern = STATES.fixation;
                startMenu.SetActive(false);
                SelectRayCaster.accuracy = 0;
                //visualSearchTask.GetComponent<VisualSearchTask>().taskCount = 0;

            }
        }

        private void startVisualSearchTask() { 
            startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            dataLogger.writeTask(startTime, blockController.getBlockName(), getTaskCount(), "vs", "start");
            visualSearchTask.SetActive(true);
            visualSearchTask.GetComponent<VisualSearchTask>().start(itemCount);
        }

        private void resetVisualSearchTask()
        {
            visualSearchTask.GetComponent<VisualSearchTask>().end();
            visualSearchTask.SetActive(false);

        }
        private void resetGame()
        {
            long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            state_pattern = STATES.end;

            dataLogger.writeTask(now, blockController.getBlockName(), getTaskCount(), "vs", "end");

            SelectRayCaster.IsInteracted = false;
            //Log the time when the trial end
            
            isiTimer = 0.0f;

            if (state_block != STATES.end)
            {
                state_pattern = STATES.isi;
                startISI = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                dataLogger.writeTask(startISI, blockController.getBlockName(), getTaskCount(), "isi", "start");
            }

        }
        internal int getTaskCount()
        {
            return taskCount;
        }
    }
}


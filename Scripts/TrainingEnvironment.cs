using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class TrainingEnvironment : MonoBehaviour
    {
            [SerializeField] public GameObject[] Target;
            public int CurrentDifficulty;
            [SerializeField] public int ItemCount_Easy = 20;
            [SerializeField] public int ItemCount_Difficult  = 30;
            [SerializeField] List<Vector3> SpawnPosition;
            [SerializeField] public float[] fixationDurationList;
            
            GameObject startMenu;
            GameObject tipMenu;
            GameObject fixationCross;

            ItemController item;
            public TargetMenu_Training tm;
            public CSVWriter writer;
            public float patternDuration = 4.0f;

            enum STATES { ready, start, wait, operate, refresh, end };
            STATES state_block = STATES.ready;
            STATES state_pattern = STATES.start;

            private long startTime; 
            private long endTime;
            
            public static GameObject target;
         
            //public static string playerID="001";
            public int maxPatternNumber=50;
            //Defination of the item spawn range 
            public float minSpawnRange_x=-1.0f;
            public float minSpawnRange_y=0.7f;
            public float minSpawnRange_z=-1.5f;
            public float maxSpawnRange_x=1.0f;
            public float maxSpawnRange_y=2.7f;
            public float maxSpawnRange_z=1.5f;
            
            public float fixationDuration;
            public float min_dis2fixation=0.5f;

            long fixationTime;
            float timer = 0.0f;
            float patternTimer;
            float fixationTimer;
            int patternID=0;
            

            // Start is called before the first frame update
            void Start()
            {   
                target = Target[1];

                startMenu = GameObject.Find("Menu");
                tipMenu = GameObject.Find("TipMenu");
                tipMenu.SetActive(false);
                fixationCross = GameObject.Find("Fixation Cross");
                fixationCross.SetActive(false);
                // if(SelectRayCaster.IsStart==true)
                //     StartButtonEvent();
                Vector3 pos = new Vector3(0.08715498f,0.8926537f,0.090873f);
                Vector3 pos2 = new Vector3(-0.02474797f,0.8714609f,-0.1071777f);
                Debug.Log(Vector3.Distance(pos,pos2));
            
            }

            // Update is called once per frame
            void Update()
            {
                StartButtonEvent();
                if (state_block == STATES.wait)
                {
                    if (SelectRayCaster_Training.IsInteracted == true)
                    {
                        resetGame();
                    }
                    if (SelectRayCaster_Training.IsInteracted == false && patternTimer > patternDuration)
                    {
                        SelectRayCaster_Training.IsTarget = false;
                        SelectRayCaster_Training.pressTime = 0;
                        SelectRayCaster_Training.clickTime = 0;
                        resetGame();
                    }
                    if (state_pattern == STATES.refresh)
                    {
                        fixationTimer += Time.deltaTime;
                        fixationCross.SetActive(true);

                        if (fixationTimer > fixationDuration)
                        {
                            fixationCross.SetActive(false);
                            generateObject();
                        }
                    }
                    if (state_pattern == STATES.operate)
                    {
                        patternTimer += Time.deltaTime;
                    }

                }

                if(state_block == STATES.end)
                {
                        //fixationCross.SetActive(false);
                        startMenu.SetActive(true);
                        startMenu.GetComponentInChildren<TextMesh>().text = "Training is over! The accuracy is " + (float)SelectRayCaster_Training.accuracy/maxPatternNumber +"%";

                        //SceneManager.LoadScene("End");
                }
        


            }
            public void StartButtonEvent()
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (state_block == STATES.ready)
                    {
                        state_block = STATES.start;
                        startMenu.SetActive(false);
                        tipMenu.SetActive(true);
                        long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        writer.writeState("Training", "start", timestamp);
                        generateObject();
                    }
                    else if (state_block == STATES.end)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }

                }
                    
                
            }
            public void resetGame()
            {
                destroyObject();
                SelectRayCaster_Training.IsInteracted=false;
                writer.writeSelection_Training(startTime,target.name, SelectRayCaster_Training.IsTarget, SelectRayCaster_Training.pressTime, SelectRayCaster_Training.clickTime,endTime);
                fixationTimer = 0.0f;
                patternTimer = 0.0f;
                if(state_block != STATES.end)
                {
                    state_pattern = STATES.refresh;
                    Debug.Log("refresh");
                    //Set duration of fixation cross to 1250, 1500, or 1750 ​ms (randomized)
                    fixationDuration = fixationDurationList[Random.Range(0, 3)];
                    long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    writer.writeCross_Training((int)(fixationDuration * 1000), fixationCross.transform.position, timestamp);
                }

            }

            public List<Vector3> generateSpawnPosition(int itemCount)
            {
                SpawnPosition = new List<Vector3>();
                for(int i=0; i<itemCount; i++)
                {
                    Vector3 Position = new Vector3(Random.Range(minSpawnRange_x,maxSpawnRange_x),Random.Range(minSpawnRange_y,maxSpawnRange_y),Random.Range(minSpawnRange_z,maxSpawnRange_z));
                    float dis2fixation = Vector3.Distance(Position,fixationCross.transform.position);
                    bool found = false;
                    for(int index = 0; index<SpawnPosition.Count; index++)
                    {
                        float dis = Vector3.Distance(Position,SpawnPosition[index]);
                        
                        if( dis < 0.5f || dis2fixation<min_dis2fixation){
                        //Position = new Vector3(Random.Range(minSpawnRange_x,maxSpawnRange_x),Random.Range(minSpawnRange_y,maxSpawnRange_y),Random.Range(minSpawnRange_z,maxSpawnRange_z));
                        found = true;
                        break;
                        }
                        
                        
                    }
                    if(found)
                        i--;
                    else
                        SpawnPosition.Add(Position);
                    
                }

                
                return SpawnPosition;
                
            }


            public void destroyObject()
            {
                
                endTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                foreach(Transform child in this.transform)
                {   
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
                tm.hideTarget();
                tipMenu.SetActive(false);
                if (patternID >= maxPatternNumber)
                {
                    Debug.Log("block end");
                    state_pattern = STATES.end;
                    state_block = STATES.end;
                    long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    writer.writeState("Training", "end", timestamp);
                }
            }

            public void generateObject()
            {
                patternID += 1; 
                state_block = STATES.wait;
                state_pattern = STATES.operate;
                Debug.Log("operate");
                startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                tipMenu.SetActive(true);

                //Spawn objects front of camera
                //Easy level
                if(CurrentDifficulty==1)
                {
                    //Targets 0,1 are for easy and 2,3,4 are for difficult
                    target = Target[Random.Range(0,2)];
                    item = (ItemController)target.GetComponent(typeof(ItemController));
                    tm.ShowTarget();

                    
                    List<Vector3> pos = generateSpawnPosition(ItemCount_Easy);
                    //Spawn target
                    Instantiate(target,pos[0],Quaternion.identity, transform);
                    writer.writeItems_Training(patternID,target.name,"Target",pos[0]);
                    
                    //Spawn distractors
                    //1:same color 2:same shape
                    int featureID = Random.Range(1,3);
                    for(int i=1; i<(int)(0.1f*ItemCount_Easy)+1;i++) 
                    {
                        GameObject d;
                        switch(featureID)
                        {
                            case 1:
                                d = item.Distractor[Random.Range(0,2)];
                                Instantiate(d,pos[i],Quaternion.identity, transform);
                                writer.writeItems_Training(patternID,d.name,"Distractor",pos[i]);
                                break;
                            case 2:
                                d = item.Distractor[Random.Range(2,4)];
                                Instantiate(d,pos[i],Quaternion.identity, transform);
                                writer.writeItems_Training(patternID,d.name,"Distractor",pos[i]);
                                break;
                            default:
                                break;
                        }

                    }
                    //Spawn other objects
                    for(int i=(int)(0.1f*ItemCount_Easy)+1; i<ItemCount_Easy;i++)
                    {
                        GameObject o = item.Others[Random.Range(0,2)];
                        Instantiate(o,pos[i],Quaternion.identity, transform);
                        writer.writeItems_Training(patternID,o.name,"Other Object",pos[i]);
                    }
                }
                //Difficult level
                else if(CurrentDifficulty==2)
                {
                    // //Targets 0,1 are for easy and 2,3,4 are for difficult
                    target = Target[Random.Range(2,5)];
                    item = (ItemController)target.GetComponent(typeof(ItemController));
                    //tipMenu.GetComponent<TextMesh>().text = "Click The "+ target.name;
                    tm.ShowTarget();
                    
                    List<Vector3> pos = generateSpawnPosition(ItemCount_Difficult);
                    //Spawn target
                    Instantiate(target,pos[0],Quaternion.identity, transform);
                    writer.writeItems_Training(patternID,target.name,"Target",pos[0]);

                    //Spawn distractors with same color
                    for(int i=1; i<(int)(ItemCount_Difficult/3)+1;i++)
                    {
                        GameObject d = item.Distractor[Random.Range(0,3)];
                        Instantiate(d,pos[i],Quaternion.identity, transform);
                        writer.writeItems_Training(patternID,d.name,"Distractor",pos[i]);

                    }
                    //Spawn distractors with same shape
                    for(int i=(int)(ItemCount_Difficult/3)+1; i<(int)(2*ItemCount_Difficult/3)+1;i++)
                    {
                        GameObject d = item.Distractor[Random.Range(3,6)];
                        Instantiate(d,pos[i],Quaternion.identity, transform);
                        writer.writeItems_Training(patternID,d.name,"Distractor",pos[i]);

                    }
                    //Spawn other objects
                    for(int i=(int)(2*ItemCount_Difficult/3)+1; i<ItemCount_Difficult;i++)
                    {
                        GameObject o = item.Others[Random.Range(0,2)];
                        Instantiate(o,pos[i],Quaternion.identity, transform);
                        writer.writeItems_Training(patternID,o.name,"Other Object",pos[i]);
                    }
                }


            }

    }

}

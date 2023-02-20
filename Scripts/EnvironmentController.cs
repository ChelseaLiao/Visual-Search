using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRception
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] public GameObject[] Target;
        [SerializeField] public int ItemCount_Easy = 20;
        [SerializeField] public int ItemCount_Difficult  = 30;
        [SerializeField] List<Vector3> SpawnPosition;
        [SerializeField] public float[] fixationDurationList;
        
        GameObject startMenu;
        GameObject tipMenu;
        GameObject fixationCross;

        ItemController item;
        public TargetMenu tm;
        public CSVWriter writer;
        //public BlockController bc;//public int patternCounter = 1;
        public float patternDuration = 4.0f;

        enum STATES { start, wait, operate, refresh, end };
        STATES state_block = STATES.start;
        STATES state_pattern = STATES.start;

        private long startTime; 
        private long endTime;
        
        public static GameObject target;
        //string blockName="";
        //public string playerID="";
        
        //Defination of the item spawn range 
        public float minSpawnRange_x=-1.0f;
        public float minSpawnRange_y=0.7f;
        public float minSpawnRange_z=-1.5f;
        public float maxSpawnRange_x=1.0f;
        public float maxSpawnRange_y=2.7f;
        public float maxSpawnRange_z=1.5f;
        
        public int maxPatternNumber=10;
        public float fixationDuration;
        public float min_dis2fixation=0.5f;

        long fixationTime;
        //float timer = 0.0f;
        float patternTimer;
        float fixationTimer;
        int patternID=0;
        

        // Start is called before the first frame update
        void Start()
        {   
            target = Target[1];
            // if(BlockController.CurrentDifficulty==1)
            //     blockName= BlockController.reality_name + "-easy";
            // else if(BlockController.CurrentDifficulty==2)
            //     blockName= BlockController.reality_name + "-difficult";
            // Debug.Log("blockName:"+blockName);
            startMenu = GameObject.Find("Menu");
            tipMenu = GameObject.Find("TipMenu");
            tipMenu.SetActive(false);
            fixationCross = GameObject.Find("Fixation Cross");
            fixationCross.SetActive(false);
            // if(SelectRayCaster.IsStart==true)
            //     StartButtonEvent();
        
        }

        // Update is called once per frame
        void Update()
        {  
        
        if(SelectRayCaster.IsInteracted==true)
        {
            Debug.Log("select");
            resetGame();
        }
        if(SelectRayCaster.IsInteracted==false && patternTimer>patternDuration)
        {
            Debug.Log("miss");
            SelectRayCaster.IsTarget = false;
            SelectRayCaster.pressTime = 0;
            SelectRayCaster.clickTime = 0;
            resetGame();
        }


        if (state_pattern == STATES.refresh)
        {
            
            fixationTimer += Time.deltaTime;
            fixationCross.SetActive(true);
        
            if(fixationTimer>fixationDuration)
                {
                    fixationCross.SetActive(false);
                    generateObject();
                }
        }
        if(state_pattern == STATES.operate)
        {
            patternTimer += Time.deltaTime;          
        }



        if (state_block == STATES.end)
        {
                Debug.Log("game over");
                long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                writer.writeState(BlockController.blockName,"end",timestamp);
                SceneManager.LoadScene("Questionnaire");
        }
    


        }
        public void StartButtonEvent()
        {
                state_block = STATES.start;
                Debug.Log("game startes");
                startMenu.SetActive(false);
                tipMenu.SetActive(true);
                long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();          
                writer.writeState(BlockController.blockName,"start",timestamp);
                
                generateObject(); 
            
        }
        public void resetGame()
        {
            Debug.Log("game is reset");
            destroyObject();
            SelectRayCaster.IsInteracted=false;
            writer.writeSelection(startTime,target.name,SelectRayCaster.IsTarget,SelectRayCaster.pressTime,SelectRayCaster.clickTime,endTime,BlockController.blockName);
            fixationTimer = 0.0f;
            patternTimer = 0.0f;
            
            //Set duration of fixation cross to 1250, 1500, or 1750 ​ms (randomized)
            fixationDuration = fixationDurationList[Random.Range(0,3)];
            long timestamp = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            writer.writeCross(BlockController.blockName,(int)(fixationDuration*1000),fixationCross.transform.position,timestamp);
            Debug.Log("pattern refresh");
            state_pattern = STATES.refresh;

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
            Debug.Log("pattern end");
            if (patternID >= maxPatternNumber)
            {
                Debug.Log("block end");
                state_block = STATES.end;
            }
        }

        public void generateObject()
        {
            Debug.Log("pattern begins");
            patternID += 1; 
            state_block = STATES.wait;
            state_pattern = STATES.operate;       
            startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

            List<Vector3> tipMenuSpawnPosition = new List<Vector3>() { new Vector3(-0.5f, 1.5f, -1.7f), new Vector3(-0.5f, 1.5f, 1.7f) };

            List<Quaternion> tipMenuSpawnRotation = new List<Quaternion>() { Quaternion.Euler(0, 160, 0), Quaternion.Euler(0, 20, 0) };

            int rand = Random.Range(0, 2);
            tipMenu.transform.position = tipMenuSpawnPosition[rand];
            tipMenu.transform.rotation = tipMenuSpawnRotation[rand];
            tipMenu.SetActive(true);
            //Spawn objects front of camera
            //Easy level
            if (BlockController.CurrentDifficulty==1)
            {
                //Targets 0,1 are for easy and 2,3,4 are for difficult
                target = Target[Random.Range(0,2)];
                item = (ItemController)target.GetComponent(typeof(ItemController));
                tm.ShowTarget();

                
                List<Vector3> pos = generateSpawnPosition(ItemCount_Easy);
                //Spawn target
                Instantiate(target,pos[0],Quaternion.identity, transform);
                writer.writeItems(BlockController.blockName,patternID,target.name,"Target",pos[0]);
                
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
                            writer.writeItems(BlockController.blockName,patternID,d.name,"Distractor",pos[i]);
                            break;
                        case 2:
                            d = item.Distractor[Random.Range(2,4)];
                            Instantiate(d,pos[i],Quaternion.identity, transform);
                            writer.writeItems(BlockController.blockName,patternID,d.name,"Distractor",pos[i]);
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
                    writer.writeItems(BlockController.blockName,patternID,o.name,"Other Object",pos[i]);
                }
            }
            //Difficult level
            else if(BlockController.CurrentDifficulty==2)
            {
                // //Targets 0,1 are for easy and 2,3,4 are for difficult
                target = Target[Random.Range(2,5)];
                item = (ItemController)target.GetComponent(typeof(ItemController));
                //tipMenu.GetComponent<TextMesh>().text = "Click The "+ target.name;
                tm.ShowTarget();
                
                List<Vector3> pos = generateSpawnPosition(ItemCount_Difficult);
                //Spawn target
                Instantiate(target,pos[0],Quaternion.identity, transform);
                writer.writeItems(BlockController.blockName,patternID,target.name,"Target",pos[0]);

                //Spawn distractors with same color
                for(int i=1; i<(int)(ItemCount_Difficult/3)+1;i++)
                {
                    GameObject d = item.Distractor[Random.Range(0,3)];
                    Instantiate(d,pos[i],Quaternion.identity, transform);
                    writer.writeItems(BlockController.blockName,patternID,d.name,"Distractor",pos[i]);

                }
                //Spawn distractors with same shape
                for(int i=(int)(ItemCount_Difficult/3)+1; i<(int)(2*ItemCount_Difficult/3)+1;i++)
                {
                    GameObject d = item.Distractor[Random.Range(3,6)];
                    Instantiate(d,pos[i],Quaternion.identity, transform);
                    writer.writeItems(BlockController.blockName,patternID,d.name,"Distractor",pos[i]);

                }
                //Spawn other objects
                for(int i=(int)(2*ItemCount_Difficult/3)+1; i<ItemCount_Difficult;i++)
                {
                    GameObject o = item.Others[Random.Range(0,2)];
                    Instantiate(o,pos[i],Quaternion.identity, transform);
                    writer.writeItems(BlockController.blockName,patternID,o.name,"Other Object",pos[i]);
                }
            }


        }

    }
}


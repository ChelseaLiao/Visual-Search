using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRception
{
    public class VisualSearchTask : MonoBehaviour
    {

        [SerializeField] public GameObject[] possibleObjects;

        [SerializeField] List<Vector3> SpawnPosition;
        public GameObject spawnArea;
        public TargetMenu targetMenu;
        public BlockController blockController;
        public TaskController taskController;
        public DataLogger dataLogger;

        public float min_dis2fixation = 0.5f;

        //public int taskCount = -1;

        //ItemController item;

        public GameObject objectHolder;
        public GameObject target;

        public GameObject[] targetPanels;
        public GameObject targetPanel;

        // Start is called before the first frame update
        void OnEnable()
        {
            objectHolder = GameObject.Find("ObjectHolder");

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void start(int itemCount)
        {
            Debug.Log(itemCount);
            foreach (GameObject obj in targetPanels)
            {
                obj.GetComponent<TargetMenu>().addTargets(possibleObjects);
                obj.SetActive(false);
            }

            generateObject(itemCount);
            spawnArea.GetComponent<MeshCollider>().enabled = false;


            targetPanel.GetComponent<TargetMenu>().ShowTarget(target.name);

        }
        public void end()
        {
            foreach (Transform child in objectHolder.transform)
            {
               child.gameObject.SetActive(false);
               Destroy(child.gameObject);
            }

            spawnArea.GetComponent<MeshCollider>().enabled = true;
            targetPanel.GetComponent<TargetMenu>().hideTarget();
        }

        private void generateObject(int itemCount)
        {
            long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (GameObject obj in targetPanels) {
                obj.SetActive(false);
            }

            int rand = Random.Range(0, targetPanels.Length);
            targetPanel = targetPanels[rand];
            targetPanel.SetActive(true);
            dataLogger.writeItems(now, blockController.getBlockName(), taskController.getTaskCount(), targetPanel.name, "Panel", targetPanel.transform.position);

            target = possibleObjects[Random.Range(0, possibleObjects.Length)];

            string targetColor = target.name.Split(' ')[0];
            string targetShape = target.name.Split(' ')[1];

            //Debug.Log("Target: " + target.name + " Color: " + targetColor + " Shape: " + targetShape);

            List<Vector3> pos = generateSpawnPosition(itemCount);

            int posCounter = 0;
            //Spawn target
            GameObject gTarget = Instantiate(target, pos[posCounter], Quaternion.identity, objectHolder.transform);
            gTarget.name = target.name + " Target";
            gTarget.tag = "Target";
            if (gTarget.transform.childCount > 0)
            {
                Transform child = gTarget.transform.GetChild(0);
                //child.name = child.name + " Target";
                child.tag = "Target";
            }
            else {
                Debug.LogError("Structure of targets changed.");
            }
            dataLogger.writeItems(now, blockController.getBlockName(), taskController.getTaskCount(), target.name, "Target", pos[posCounter]);
            posCounter++;

            //Spawn distractors with same color
            for (int i = 0; i < (int)(itemCount / 3); i++)
            {
                GameObject obj = getObjectWithSameColor(targetColor, targetShape);
                GameObject g = Instantiate(obj, pos[posCounter], Quaternion.identity, objectHolder.transform);
                g.name = obj.name;
                //g.target = "Same Color";
                if (g.transform.childCount > 0)
                {
                    Transform child = g.transform.GetChild(0);
                    child.tag = "Same Color";
                }
                dataLogger.writeItems(now, blockController.getBlockName(), taskController.getTaskCount(), g.name, "Same Color", pos[posCounter]);
                posCounter++;

            }
            //Spawn distractors with same shape
            for (int i = 0; i < (int)(itemCount / 3); i++)
            {
                GameObject obj = getObjectWithSameShape(targetColor, targetShape);
                GameObject g = Instantiate(obj, pos[posCounter], Quaternion.identity, objectHolder.transform);
                g.name = obj.name;
                //g.target = "Same Shape";
                if (g.transform.childCount > 0)
                {
                    Transform child = g.transform.GetChild(0);
                    child.tag = "Same Shape";
                }
                dataLogger.writeItems(now, blockController.getBlockName(), taskController.getTaskCount(), g.name, "Same Shape", pos[posCounter]);
                posCounter++;

            }
            //Spawn other objects
            for (int i = 0; i < (int)(itemCount / 3)-1; i++)
            {
                GameObject obj = getObjectDifferent(targetColor, targetShape);
                GameObject g = Instantiate(obj, pos[posCounter], Quaternion.identity, objectHolder.transform);
                g.name = obj.name;
                //g.target = "Different";
                if (g.transform.childCount > 0)
                {
                    Transform child = g.transform.GetChild(0);
                    child.tag = "Different";
                }
                dataLogger.writeItems(now, blockController.getBlockName(), taskController.getTaskCount(), g.name, "Different", pos[posCounter]);
                posCounter++;
            }
        }

        public GameObject getObjectWithSameColor(string color, string shape)
        {
            List<GameObject> options = new List<GameObject>();
            foreach (GameObject obj in possibleObjects)
            {
                if (obj.name.Contains(color) && !obj.name.Contains(shape)) {
                    options.Add(obj);
                }
            }
            return options[Random.Range(0, options.Count)];
        }

        public GameObject getObjectWithSameShape(string color, string shape)
        {
            List<GameObject> options = new List<GameObject>();
            foreach (GameObject obj in possibleObjects)
            {
                if ((!obj.name.Contains(color)) && obj.name.Contains(shape))
                {
                    options.Add(obj);
                }
            }
            return options[Random.Range(0, options.Count)];
        }

        public GameObject getObjectDifferent(string color, string shape)
        {
            List<GameObject> options = new List<GameObject>();
            foreach (GameObject obj in possibleObjects)
            {
                if ((!obj.name.Contains(color)) && !(obj.name.Contains(shape)))
                {
                    options.Add(obj);
                }
            }
            return options[Random.Range(0, options.Count)];
        }


        public static bool IsPointWithinCollider(Collider collider, Vector3 point)
        {
            return collider.ClosestPoint(point) == point;
        }

        private Vector3 getPointInSpawnArea()
        {
            MeshCollider mc = spawnArea.GetComponent<MeshCollider>();
            Vector3 point;

            for (int i = 0; i < 100; i++)
            {
                Vector3 tempPoint = new Vector3(Random.Range(mc.bounds.min.x, mc.bounds.max.x), Random.Range(mc.bounds.min.y, mc.bounds.max.y), Random.Range(mc.bounds.min.z, mc.bounds.max.z));
                if (IsPointWithinCollider(mc, tempPoint))
                {
                    //Debug.Log(tempPoint);
                    return tempPoint;
                }
            }
            Debug.LogError("Could not find Vector3 in spawn area");
            return new Vector3(Random.Range(mc.bounds.min.x, mc.bounds.max.x), Random.Range(mc.bounds.min.y, mc.bounds.max.y), Random.Range(mc.bounds.min.z, mc.bounds.max.z));
        }

        public List<Vector3> generateSpawnPosition(int itemCount)
        {
            SpawnPosition = new List<Vector3>();
            for (int i = 0; i < itemCount; i++)
            {
                //RandomPointInBounds(myCollider.bounds);

                Vector3 Position = getPointInSpawnArea();
                //float dis2fixation = Vector3.Distance(Position, fixationCross.transform.position);
                bool found = false;
                for (int index = 0; index < SpawnPosition.Count; index++)
                {
                    float dis = Vector3.Distance(Position, SpawnPosition[index]);

                    if (dis < 0.5f)// || dis2fixation < min_dis2fixation)
                    {
                        //Position = new Vector3(Random.Range(minSpawnRange_x,maxSpawnRange_x),Random.Range(minSpawnRange_y,maxSpawnRange_y),Random.Range(minSpawnRange_z,maxSpawnRange_z));
                        found = true;
                        break;
                    }
                }
                if (found)
                    i--;
                else
                    SpawnPosition.Add(Position);
            }

            return SpawnPosition;
        }


        internal string getTargetName()
        {
            return target.name;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRception
{
    public class TargetMenu : MonoBehaviour
    {
        public GameObject targetContainer;

        // Start is called before the first frame update
        void Start()
        {
            // foreach(Transform child in transform)
            // {
            //     child.gameObject.SetActive(false);
            // }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void addTargets(GameObject[] possibleObjects) {

            foreach (Transform child in targetContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (GameObject obj in possibleObjects)
            {
                GameObject g = Instantiate(obj, targetContainer.transform.position, targetContainer.transform.rotation, targetContainer.transform);
                g.name = obj.name;
                Destroy(g.transform.GetChild(0).GetComponent<BoxCollider>());
                Destroy(g.transform.GetChild(0).GetComponent<MeshCollider>());
                Destroy(g.transform.GetChild(0).GetComponent<SphereCollider>());
                Destroy(g.transform.GetChild(0).GetComponent<CapsuleCollider>());
                Destroy(g.transform.GetChild(0).GetComponent<HighlightAtGazeSR>());

            }
        }


        public void ShowTarget(string target)
        {
        
            foreach(Transform child in targetContainer.transform)
            {
                if(child.gameObject.name == target)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }

        }
        public void hideTarget()
        {

            foreach(Transform child in targetContainer.transform)
            {
                child.gameObject.SetActive(false);
            }

        }
    }    
}


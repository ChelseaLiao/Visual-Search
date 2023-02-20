using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRception
{
    public class TargetMenu_Training : MonoBehaviour
    {
        //[SerializeField] public EnvironmentController env;

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
        public void ShowTarget()
        {
        
            foreach(Transform child in transform)
            {
                if(child.gameObject.name == TrainingEnvironment.target.name)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }

        }
        public void hideTarget()
        {

            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

        }
    }    
}


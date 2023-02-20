using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using Valve.VR;

namespace VRception
{
    public class SelectRayCaster : MonoBehaviour
    {
        [SerializeField]
        float maxRaycasterLength = 12.0f;
        [SerializeField] EnvironmentController env;
        public static bool IsInteracted = false;
        public static bool IsTarget;
        public static long clickTime;
        public static long pressTime;
        public SteamVR_Input_Sources targetSource;
        public SteamVR_Action_Boolean clickAction;

        //public static bool IsStart=false;
        //public float delay = 0.1f;
        LineRenderer lr;

        int layerNumber;
        LayerMask ignoreLayer;

        // DateTime localDate = DateTime.Now;
        // CultureInfo culture = new CultureInfo("de-DE");

        // Start is called before the first frame update
        void Start()
        {
            lr = GetComponent<LineRenderer>();


            IsTarget = false;
            layerNumber = LayerMask.NameToLayer("Ignore Raycast");
            ignoreLayer = 1 << layerNumber;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 endPoint = transform.position + (transform.forward * maxRaycasterLength);


            RaycastHit hit = CreatRaycast(maxRaycasterLength);

            if (hit.collider != null)
            {
                endPoint = hit.point;
                //TO DO find the better way to aviod logging data when click th button
                if (clickAction.GetStateDown(targetSource))
                {
                    if (hit.transform.name != "StartButton")
                    {
                        pressTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        Debug.Log("log press time at" + pressTime);
                    }
                        
                }
                if (clickAction.GetStateUp(targetSource))
                {

                    if (hit.transform.CompareTag("Target"))
                    {
                        clickTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        Debug.Log("log click time at" + clickTime);
                        IsTarget = true;
                        IsInteracted = true;
                    }
                    else if (hit.transform.name == "StartButton")
                    {
                        env.StartButtonEvent();
                    }

                    else
                    {
                        clickTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        Debug.Log("log click time at" + clickTime);
                        IsTarget = false;
                        IsInteracted = true;
                    }

                }
            }

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, endPoint);


        }
        private RaycastHit CreatRaycast(float length)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            Physics.Raycast(ray, out hit, length, ~ignoreLayer);
            return hit;
        }
    }
}


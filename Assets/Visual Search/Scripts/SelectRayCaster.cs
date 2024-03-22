using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

namespace VRception
{
    public class SelectRayCaster : MonoBehaviour
    {
        [SerializeField]
        float maxRaycasterLength = 12.0f;
        public static bool IsInteracted = false;
        public static bool IsTarget;
        //public static long clickTime;
        public static long pressTime;
        public SteamVR_Input_Sources targetSource;
        public SteamVR_Action_Boolean clickAction;
        public static int accuracy;
        public static string clickedItem;
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
            clickedItem = "Null";

            lr = GetComponent<LineRenderer>();

            accuracy = 0;
            IsTarget = false;
            layerNumber = LayerMask.NameToLayer("Ignore Raycast");
            ignoreLayer = 1 << layerNumber;
        }

        // Update is called once per frame
        void Update()
        {
            long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Vector3 endPoint = transform.position + (transform.forward * maxRaycasterLength);


            RaycastHit hit = CreatRaycast(maxRaycasterLength);

            if (hit.collider != null)
            {
                endPoint = hit.point;
                //TO DO find the better way to aviod logging data when click th button
                if (clickAction.GetStateDown(targetSource))
                {
                    clickedItem = hit.collider.name;
                    //Debug.Log("HIT:" + hit.transform.name + "  " + hit.transform.tag);


                    if (hit.transform.name.Contains("FeedbackElement") || hit.transform.tag.Contains("FeedbackElement"))
                    {
                        //GameObject vrqt = GameObject.Find("VRQuestionnaireToolkit");
                        //EventConrtoller ec = vrqt.GetComponent<EventConrtoller>();
                        if (hit.transform.name == "ButtonNext")
                        {
                            //ec.next();
                            Button b = hit.transform.GetComponent<Button>();
                            b.onClick.Invoke();
                        }
                        else if (hit.transform.name == "Radio")
                        {
                            Toggle t = hit.transform.GetComponent<Toggle>();
                            t.isOn = !t.isOn;
                        }
                        else if (hit.transform.name == "ButtonToTask")
                        {
                            Button b = hit.transform.GetComponent<Button>();
                            b.onClick.Invoke();
                        }
                        else if (hit.transform.name == "ButtonPrevious")
                        {
                            Button b = hit.transform.GetComponent<Button>();
                            b.onClick.Invoke();
                        }
                        
                    }
                    else if (hit.transform.name.Contains("Target") || hit.transform.CompareTag("Target"))
                    {
                        pressTime = now;
                        Debug.Log("log press on correct item " + clickedItem + " at" + pressTime);
                        IsTarget = true;
                        accuracy++;
                        IsInteracted = true;

                    }
                    else
                    {
                        pressTime = now;
                        Debug.Log("log press time to wrong item at" + pressTime);
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


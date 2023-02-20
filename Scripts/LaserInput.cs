using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

namespace VRception
{
    public class LaserInput : MonoBehaviour
    {
        public SteamVR_LaserPointer laserPointer;
        [SerializeField] EnvironmentController env;
        float maxRaycasterLength = 12.0f;
        public static bool IsInteracted = false;
        public static bool IsTarget;
        public static long clickTime;
        public static long pressTime;

        LineRenderer lr;

        int layerNumber;
        LayerMask ignoreLayer;
        // Start is called before the first frame update
        void Awake()
        {

            //laserPointer.PointerIn += PointerInside;
            //laserPointer.PointerOut += PointerOutside;
            laserPointer.PointerClick += PointerClick;
        }

        public void PointerClick(object sender, PointerEventArgs e)
        {
            if (e.target.name == "Target")
            {
                IsTarget = true;
                IsInteracted = true;
            }
            else if (e.target.name == "StartButton")
            {
                env.StartButtonEvent();
            }

            else
            {
                IsTarget = false;
                IsInteracted = true;
            }
        }


        void Start()
        {
            IsTarget = false;
            layerNumber = LayerMask.NameToLayer("Ignore Raycast");
            ignoreLayer = 1 << layerNumber;

        }

        // Update is called once per frame
        void Update()
        {


        }
    }
}


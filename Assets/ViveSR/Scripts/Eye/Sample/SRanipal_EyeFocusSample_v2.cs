//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR.anipal.Eye
{
    public class SRanipal_EyeFocusSample_v2 : MonoBehaviour
    {
        private FocusInfo FocusInfo;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static EyeData_v2 eyeData = new EyeData_v2();
        private bool eye_callback_registered = false;
        private void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                Debug.Log("callback:" + SRanipal_Eye_Framework.Instance.EnableEyeDataCallback + "register:" + eye_callback_registered);
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                Debug.Log("callback:" + SRanipal_Eye_Framework.Instance.EnableEyeDataCallback + "register:" + eye_callback_registered);
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }

            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                int dart_board_layer_id = LayerMask.NameToLayer("NoReflection");
                bool eye_focus;
                if (eye_callback_registered)
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                else
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));
               
                if (eye_focus)
                {
                    if (FocusInfo.collider != null)
                        Debug.Log(FocusInfo.collider.name);
                    else
                        Debug.Log("FocusInfo is null");
                    DartBoard dartBoard = FocusInfo.transform.GetComponent<DartBoard>();
                    if (dartBoard != null) dartBoard.Focus(FocusInfo.point);
                    break;
                }
            }
        }
        private void Release()
        {
            if (eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
                Debug.Log("release");
            }
        }
        private static void EyeCallback(ref EyeData_v2 eye_data)
        {
            eyeData = eye_data;
            //Vector3 combinedEyeLocalOrigin;
            //Vector3 combinedEyeLocalForward;
            //bool isCombinedEyeGazeRayValid;
            //isCombinedEyeGazeRayValid = SRanipal_Eye_v2.GetGazeRay((GazeIndex)2, out combinedEyeLocalOrigin, out combinedEyeLocalForward, eyeData);
            //if(isCombinedEyeGazeRayValid == true)
            //    Debug.Log(combinedEyeLocalOrigin.x.ToString());
        }
    }
}
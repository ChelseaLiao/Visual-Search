using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using VRception;

public class DataLoggerEye : MonoBehaviour {

    public DataLogger dataLogger;

    //private static EyeData_v2 eyeData = new EyeData_v2 ();
    private static bool eye_callback_registered = false;
    private static Matrix4x4 cameraTransformLocalToWorldMatrix; //used in python to convert from local to world corrdinates
    private static Vector3 hmdWorldOrigin;
    private static Vector3 hmdWorldForward;
    private static Quaternion hmdWorldQuaternion;

    private static bool logging = false;

    public static float headVel_Az;
    public static float headVel_Pol;
    
    private static long startTime;
    private static float unityTime;

    public Camera camera; 
    public long now;
    public Vector3 combinedEyeLocalForward;
    public Vector3 combinedEyeLocalOrigin;
    private static Vector3 combinedEyeWorldForward;
    public Vector3 combinedEyeWorldOrigin;

    private EyeParameter eye_parameter;
    private float[] headAzs = new float[5];
    private float[] headPols = new float[5];
    private float dt = 1 / 90.0f;

    public static string[] ColumnNames = {
                //timestamp
                "Time",
                "frameSequence",
                "systemTimestamp(ms)",
                "unityTimestamp(s)",
                "deviceTimestamp(ms)",

                //event
                "trialIndex",

                //Combined eye
                "combinedEyeLocalOrigin_X",
                "combinedEyeLocalOrigin_Y",
                "combinedEyeLocalOrigin_Z",
                "combinedEyeLocalForward_X",
                "combinedEyeLocalForward_Y",
                "combinedEyeLocalForward_Z",
                "combinedEyeWorldForward_X",
                "combinedEyeWorldForward_Y",
                "combinedEyeWorldForward_Z",
                "isCombinedEyeGazeRayValid",

                //Left eye
                "leftEyeLocalOrigin_X",
                "leftEyeLocalOrigin_Y",
                "leftEyeLocalOrigin_Z",
                "leftEyeLocalForward_X",
                "leftEyeLocalForward_Y",
                "leftEyeLocalForward_Z",
                "leftEyePupilDiameter",
                "leftEyePupilPositionInSensorArea_X",
                "leftEyePupilPositionInSensorArea_Y",
                "leftEyePupilPosition_X",
                "leftEyePupilPosition_Y",
                "leftEyeOpenness",
                "leftEyeOpennessReadSuccess",
                "leftEyeFrown",
                "leftEyeSqueeze",
                "leftEyeWide",
                "isLeftEyeGazeRayValid",

                //Right eye
                "rightEyeLocalOrigin_X",
                "rightEyeLocalOrigin_Y",
                "rightEyeLocalOrigin_Z",
                "rightEyeLocalForward_X",
                "rightEyeLocalForward_Y",
                "rightEyeLocalForward_Z",
                "rightEyePupilDiameter",
                "rightEyePupilPositionInSensorArea_X",
                "rightEyePupilPositionInSensorArea_Y",
                "rightEyePupilPosition_X",
                "rightEyePupilPosition_Y",
                "rightEyeOpenness",
                "rightEyeOpennessReadSuccess",
                "rightEyeFrown",
                "rightEyeSqueeze",
                "rightEyeWide",
                "isRightEyeGazeRayValid",

                //HMD
                "hmdWorldOrigin_X",
                "hmdWorldOrigin_Y",
                "hmdWorldOrigin_Z",
                "hmdWorldForward_X",
                "hmdWorldForward_Y",
                "hmdWorldForward_Z",

                //HMD quaternion
                "hmdWorldQuaternion_W",
                "hmdWorldQuaternion_X",
                "hmdWorldQuaternion_Y",
                "hmdWorldQuaternion_Z",

                //HMD localToWorldMatrix (Matrix4X4)
                "hmdL2W_m00",
                "hmdL2W_m01",
                "hmdL2W_m02",
                "hmdL2W_m03",

                "hmdL2W_m10",
                "hmdL2W_m11",
                "hmdL2W_m12",
                "hmdL2W_m13",

                "hmdL2W_m20",
                "hmdL2W_m21",
                "hmdL2W_m22",
                "hmdL2W_m23",

                "hmdL2W_m30",
                "hmdL2W_m31",
                "hmdL2W_m32",
                "hmdL2W_m33",

                //head velocity
                "headVel_Az",
                "headVel_Pol",

                "stimLocalPos_X",
                "stimLocalPos_Y",
                "stimLocalPos_Z"};

    // Start is called before the first frame update
    void Start () {

        //if (!SRanipal_Eye_Framework.Instance.EnableEye)
        //{
        //    enabled = false;
        //    return;
        //}
        //camera = Camera.main;
        startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        StartLogging();
    }

    // Update is called once per frame
    void Update () {

        if (SRanipal_Eye_Framework.Instance == null)
        {
            Debug.Log("SRanipal_Eye_Framework is null");
            return;
        }

        if (!SRanipal_Eye_Framework.Instance.EnableEye) {
            Debug.Log("SRanipal_Eye_Framework is EnableEye not enabled");
            return;
        }

        if (camera == null) {
            Debug.Log("camera is null");

            camera = Camera.main;
            if (camera == null)
            {
                camera = GameObject.Find("Camera").GetComponent<Camera>();
            }
            
            return;
        }

        now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        dt = Time.deltaTime;
        unityTime = Time.time;

        //Debug.Log ("Status: " + SRanipal_Eye_Framework.Status);
        //Debug.Log("EnableEyeDataCallback: " + SRanipal_Eye_Framework.Instance.EnableEyeDataCallback);
        //Debug.Log("eye_callback_registered: " + eye_callback_registered);

        cameraTransformLocalToWorldMatrix = camera.transform.localToWorldMatrix;
        hmdWorldOrigin = camera.transform.position;
        hmdWorldForward = camera.transform.forward;
        hmdWorldQuaternion = camera.transform.rotation;

        //only continue if eye tracking is working 
        if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
        {
            //Debug.Log("callback: " + SRanipal_Eye_Framework.Instance.EnableEyeDataCallback + " register: " + eye_callback_registered);
            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                Debug.Log("Status: " + SRanipal_Eye_Framework.Status);
                var a = SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                Debug.Log("callback success?: " + a);
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
                Debug.Log("Status: " + SRanipal_Eye_Framework.Status);
            }
        }
        else
        {
            Debug.LogWarning("eye tracking doesnot work");
            return;
        }

       //Get curretn-frame gaze ray
       Vector3 combinedEyeLocalForwardTemp;
       bool isCombinedEyeGazeRayValid;
       isCombinedEyeGazeRayValid = SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out combinedEyeLocalOrigin, out combinedEyeLocalForwardTemp);
       if (isCombinedEyeGazeRayValid == true) {
            combinedEyeLocalForward = combinedEyeLocalForwardTemp;
            combinedEyeWorldOrigin = camera.transform.TransformPoint (combinedEyeLocalOrigin);
            combinedEyeWorldForward = camera.transform.TransformDirection (combinedEyeLocalForward);
       }
        //gazePoint.transform.position = cam.transform.TransformPoint (combinedEyeLocalForward * 0.75f);

        //get head ficks angles
        Vector2 headFicks = vec2ficks (camera.transform.forward.x, camera.transform.forward.y, camera.transform.forward.z);
       //update last 5 samples
       for (int i = 0; i < 4; i++) {
           headAzs[i] = headAzs[i + 1];
           headPols[i] = headPols[i + 1];
       }
       headAzs[4] = headFicks.x;
       headPols[4] = headFicks.y;
       //5 point velocity calculation
       headVel_Az = (-headAzs[0] - headAzs[1] + headAzs[3] + headAzs[4]) / (6 * dt);
       headVel_Pol = (-headPols[0] - headPols[1] + headPols[3] + headPols[4]) / (6 * dt);
    }

    public void StartLogging () {
        if (logging) {
            Debug.LogWarning ("Logging was on when StartLogging was called. No new log was started.");
            return;
        }
        
        logging = true;
    }

    public void StopLogging () {
        Debug.Log("DataLoggingEye Stopped");
        if (!logging)
            return;

        Release ();
        if (swGazeSR != null)
        {
            swGazeSR.Flush();
            swGazeSR.Close();
            swGazeSR = null;
            //try
            //{
            //    swGazeSR.Flush();
            //    swGazeSR.Close();
            //}
            //catch (Exception e)
            //{
            //    Debug.LogWarning(e.ToString());
            //}
        }
        logging = false;
        Debug.Log ("Logging ended");

    }

    void OnDestroy()
    {
        Release();
    }

    private void OnDisable () {
        Release ();
    }

    void OnApplicationQuit () {
        Release ();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus == true) { 
            Release();
        }
    }

    static int GetPupilDiameter (EyeIndex eyeIndex, out float pupilDiameter, EyeData_v2 eyeData) {
        VerboseData verboseData;
        SRanipal_Eye_v2.GetVerboseData (out verboseData, eyeData);
        if (eyeIndex == ViveSR.anipal.Eye.EyeIndex.LEFT) {
            pupilDiameter = verboseData.left.pupil_diameter_mm;
            return 1;
        } else if (eyeIndex == ViveSR.anipal.Eye.EyeIndex.RIGHT) {
            pupilDiameter = verboseData.right.pupil_diameter_mm;
            return 1;
        } else {
            pupilDiameter = -1;
            return 0;
        }
    }

    static int GetPupilPositionInSensorArea (EyeIndex eyeIndex, out Vector2 pupilPositionInSensorArea, EyeData_v2 eyeData) {
        VerboseData verboseData;
        SRanipal_Eye_v2.GetVerboseData (out verboseData, eyeData);
        if (eyeIndex == ViveSR.anipal.Eye.EyeIndex.LEFT) {
            pupilPositionInSensorArea = verboseData.left.pupil_position_in_sensor_area;
            return 1;
        } else if (eyeIndex == ViveSR.anipal.Eye.EyeIndex.RIGHT) {
            pupilPositionInSensorArea = verboseData.right.pupil_position_in_sensor_area;
            return 1;
        } else {
            pupilPositionInSensorArea = new Vector2 ();
            pupilPositionInSensorArea.x = -1;
            pupilPositionInSensorArea.y = -1;
            return 0;
        }
    }

    static int GetEyeExpression (EyeIndex eyeIndex, out float eyeFrown, out float eyeSqueeze, out float eyeWide, EyeData_v2 eyeData) {
        EyeExpression expressionData = eyeData.expression_data;
        if (eyeIndex == ViveSR.anipal.Eye.EyeIndex.LEFT) {
            eyeFrown = expressionData.left.eye_frown;
            eyeSqueeze = expressionData.left.eye_squeeze;
            eyeWide = expressionData.left.eye_wide;
            return 1;
        } else if (eyeIndex == ViveSR.anipal.Eye.EyeIndex.RIGHT) {
            eyeFrown = expressionData.right.eye_frown;
            eyeSqueeze = expressionData.right.eye_squeeze;
            eyeWide = expressionData.right.eye_wide;
            return 1;
        } else {
            eyeFrown = -1;
            eyeSqueeze = -1;
            eyeWide = -1;
            return 0;
        }

    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private void Release () {

        //if (swGazeSR != null) {
        //    swGazeSR.Flush();
        //    swGazeSR.Close();

        //    //try
        //    //{
        //    //    swGazeSR.Flush();
        //    //    swGazeSR.Close();
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Debug.LogWarning(e.ToString());
        //    //}
        //}

        Debug.Log("DataLoggingEye Release");
        if (eye_callback_registered == true) {
            Debug.Log ("try to release");
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback (Marshal.GetFunctionPointerForDelegate ((SRanipal_Eye_v2.CallbackBasic) EyeCallback));
            Debug.Log ("release complete");
            eye_callback_registered = false;
        }
    }

    /// <summary>
    /// Required class for IL2CPP scripting backend support
    /// </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute {
        public MonoPInvokeCallbackAttribute () { }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    [MonoPInvokeCallback]
    private void EyeCallback(ref EyeData_v2 eyeData)
    {
        long nowCallback = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //eyeData = eye_data;
        Vector3 combinedEyeLocalOrigin;
        Vector3 combinedEyeLocalForward;
        Vector3 combinedEyeWorldOrigin;
        Vector3 leftEyeLocalOrigin;
        Vector3 leftEyeLocalForward;
        float leftEyePupilDiameter;
        Vector2 leftEyePupilPositionInSensorArea;
        Vector2 leftEyePupilPosition;
        float leftEyeOpenness;
        float leftEyeFrown;
        float leftEyeSqueeze;
        float leftEyeWide;
        Vector3 rightEyeLocalOrigin;
        Vector3 rightEyeLocalForward;
        float rightEyePupilDiameter;
        Vector2 rightEyePupilPositionInSensorArea;
        Vector2 rightEyePupilPosition;
        float rightEyeOpenness;
        float rightEyeFrown;
        float rightEyeSqueeze;
        float rightEyeWide;

        //eye local origin and gaze direction
        bool isLeftEyeGazeRayValid = SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out leftEyeLocalOrigin, out leftEyeLocalForward, eyeData);
        bool isRightEyeGazeRayValid = SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out rightEyeLocalOrigin, out rightEyeLocalForward, eyeData);
        bool isCombinedEyeGazeRayValid = SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out combinedEyeLocalOrigin, out combinedEyeLocalForward, eyeData);
        var combinedEyeWorldForward_ = combinedEyeWorldForward;
        //if (isCombinedEyeGazeRayValid == true)
        //{
        //    //combinedEyeLocalForward = combinedEyeLocalForwardTemp;
        //    combinedEyeWorldOrigin = camera.transform.TransformPoint(combinedEyeLocalOrigin);
        //    combinedEyeWorldForward = camera.transform.TransformDirection(combinedEyeLocalForward);
        //}
        //Debug.Log("combinedEyeWorldForward: " + combinedEyeWorldForward);
        if (isLeftEyeGazeRayValid == false & isRightEyeGazeRayValid == false)
        {
            Debug.LogWarning("No valid Eye Data");
            return;
        }

        //Debug.Log(isLeftEyeGazeRayValid + " " + isRightEyeGazeRayValid);
        //pupil diameter
        int leftPupilDiameterReadSuccess = GetPupilDiameter(EyeIndex.LEFT, out leftEyePupilDiameter, eyeData);
        int rightPupilDiameterReadSuccess = GetPupilDiameter(EyeIndex.RIGHT, out rightEyePupilDiameter, eyeData);

        //pupil position in sensor area
        int leftEyePupilPositionInSensorAreaReadSuccess = GetPupilPositionInSensorArea(EyeIndex.LEFT, out leftEyePupilPositionInSensorArea, eyeData);
        int rightEyePupilPositionInSensorAreaReadSuccess = GetPupilPositionInSensorArea(EyeIndex.RIGHT, out rightEyePupilPositionInSensorArea, eyeData);
        //pupil position GetPupilPosition ? how is it differenct from pupil_position_in_sensor_area?
        var leftEyePupilPositionReadSuccess = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out leftEyePupilPosition, eyeData);
        var rightEyePupilPositionReadSuccess = SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out rightEyePupilPosition, eyeData);
        //openness ?should be clamped between 0,1, but its not?
        var leftEyeOpennessReadSuccess = SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.LEFT, out leftEyeOpenness, eyeData);
        var rightEyeOpennessReadSuccess = SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.RIGHT, out rightEyeOpenness, eyeData);

        //Eye expression
        var leftEyeExpressionReadSuccess = GetEyeExpression(EyeIndex.LEFT, out leftEyeFrown, out leftEyeSqueeze, out leftEyeWide, eyeData);
        var rightEyeExpressionReadSuccess = GetEyeExpression(EyeIndex.RIGHT, out rightEyeFrown, out rightEyeSqueeze, out rightEyeWide, eyeData);

        //hmd
        var hmdTransformLocalToWorldMatrix = cameraTransformLocalToWorldMatrix;
        var hmdWorldOrigin_ = hmdWorldOrigin;
        var hmdWorldForward_ = hmdWorldForward;
        var hmdWorldQuaternion_ = hmdWorldQuaternion;

        string[] logData = new string[ColumnNames.Length];

        //Debug.LogError ("frameSequence: " + eyeData.frame_sequence.ToString ());
        //timestamp

        logData[Array.IndexOf(ColumnNames, "Time")] = nowCallback.ToString();
        logData[Array.IndexOf(ColumnNames, "frameSequence")] = eyeData.frame_sequence.ToString();
        logData[Array.IndexOf(ColumnNames, "systemTimestamp(ms)")] = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - startTime).ToString();
        logData[Array.IndexOf(ColumnNames, "unityTimestamp(s)")] = unityTime.ToString();
        logData[Array.IndexOf(ColumnNames, "deviceTimestamp(ms)")] = eyeData.timestamp.ToString(); //5000000. ToString ();
                                                                                                     //logData[Array.IndexOf (ColumnNames, "trialIndex")] = gameStates.trialIndex.ToString ();

        //Combined eye
        logData[Array.IndexOf(ColumnNames, "combinedEyeLocalOrigin_X")] = combinedEyeLocalOrigin.x.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeLocalOrigin_Y")] = combinedEyeLocalOrigin.y.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeLocalOrigin_Z")] = combinedEyeLocalOrigin.z.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeLocalForward_X")] = combinedEyeLocalForward.x.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeLocalForward_Y")] = combinedEyeLocalForward.y.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeLocalForward_Z")] = combinedEyeLocalForward.z.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeWorldForward_X")] = combinedEyeWorldForward_.x.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeWorldForward_Y")] = combinedEyeWorldForward_.y.ToString();
        logData[Array.IndexOf(ColumnNames, "combinedEyeWorldForward_Z")] = combinedEyeWorldForward_.z.ToString();
        logData[Array.IndexOf(ColumnNames, "isCombinedEyeGazeRayValid")] = isCombinedEyeGazeRayValid.ToString();

        //Left eye
        logData[Array.IndexOf(ColumnNames, "leftEyeLocalOrigin_X")] = leftEyeLocalOrigin.x.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeLocalOrigin_Y")] = leftEyeLocalOrigin.y.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeLocalOrigin_Z")] = leftEyeLocalOrigin.z.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeLocalForward_X")] = leftEyeLocalForward.x.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeLocalForward_Y")] = leftEyeLocalForward.y.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeLocalForward_Z")] = leftEyeLocalForward.z.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyePupilDiameter")] = leftEyePupilDiameter.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyePupilPositionInSensorArea_X")] = leftEyePupilPositionInSensorArea.x.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyePupilPositionInSensorArea_Y")] = leftEyePupilPositionInSensorArea.y.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyePupilPosition_X")] = leftEyePupilPosition.x.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyePupilPosition_Y")] = leftEyePupilPosition.y.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeOpenness")] = leftEyeOpenness.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeOpennessReadSuccess")] = leftEyeOpennessReadSuccess.ToString();
        logData[Array.IndexOf(ColumnNames, "leftEyeFrown")] = leftEyeFrown.ToString("F5");
        logData[Array.IndexOf(ColumnNames, "leftEyeSqueeze")] = leftEyeSqueeze.ToString("F5");
        logData[Array.IndexOf(ColumnNames, "leftEyeWide")] = leftEyeWide.ToString("F5");
        logData[Array.IndexOf(ColumnNames, "isLeftEyeGazeRayValid")] = isLeftEyeGazeRayValid.ToString();

        //Right eye
        logData[Array.IndexOf(ColumnNames, "rightEyeLocalOrigin_X")] = rightEyeLocalOrigin.x.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeLocalOrigin_Y")] = rightEyeLocalOrigin.y.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeLocalOrigin_Z")] = rightEyeLocalOrigin.z.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeLocalForward_X")] = rightEyeLocalForward.x.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeLocalForward_Y")] = rightEyeLocalForward.y.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeLocalForward_Z")] = rightEyeLocalForward.z.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyePupilDiameter")] = rightEyePupilDiameter.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyePupilPositionInSensorArea_X")] = rightEyePupilPositionInSensorArea.x.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyePupilPositionInSensorArea_Y")] = rightEyePupilPositionInSensorArea.y.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyePupilPosition_X")] = rightEyePupilPosition.x.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyePupilPosition_Y")] = rightEyePupilPosition.y.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeOpenness")] = rightEyeOpenness.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeOpennessReadSuccess")] = rightEyeOpennessReadSuccess.ToString();
        logData[Array.IndexOf(ColumnNames, "rightEyeFrown")] = rightEyeFrown.ToString("F5");
        logData[Array.IndexOf(ColumnNames, "rightEyeSqueeze")] = rightEyeSqueeze.ToString("F5");
        logData[Array.IndexOf(ColumnNames, "rightEyeWide")] = rightEyeWide.ToString("F5");
        logData[Array.IndexOf(ColumnNames, "isRightEyeGazeRayValid")] = isRightEyeGazeRayValid.ToString();

        //HMD
        logData[Array.IndexOf(ColumnNames, "hmdWorldOrigin_X")] = hmdWorldOrigin_.x.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldOrigin_Y")] = hmdWorldOrigin_.y.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldOrigin_Z")] = hmdWorldOrigin_.z.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldForward_X")] = hmdWorldForward_.x.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldForward_Y")] = hmdWorldForward_.y.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldForward_Z")] = hmdWorldForward_.z.ToString();

        logData[Array.IndexOf(ColumnNames, "hmdWorldQuaternion_W")] = hmdWorldQuaternion_.w.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldQuaternion_X")] = hmdWorldQuaternion_.x.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldQuaternion_Y")] = hmdWorldQuaternion_.y.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdWorldQuaternion_Z")] = hmdWorldQuaternion_.z.ToString();

        //HMD localToWorldMatrix(Matrix4X4)
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m00")] = hmdTransformLocalToWorldMatrix.m00.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m01")] = hmdTransformLocalToWorldMatrix.m01.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m02")] = hmdTransformLocalToWorldMatrix.m02.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m03")] = hmdTransformLocalToWorldMatrix.m03.ToString();

        logData[Array.IndexOf(ColumnNames, "hmdL2W_m10")] = hmdTransformLocalToWorldMatrix.m10.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m11")] = hmdTransformLocalToWorldMatrix.m11.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m12")] = hmdTransformLocalToWorldMatrix.m12.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m13")] = hmdTransformLocalToWorldMatrix.m13.ToString();

        logData[Array.IndexOf(ColumnNames, "hmdL2W_m20")] = hmdTransformLocalToWorldMatrix.m20.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m21")] = hmdTransformLocalToWorldMatrix.m21.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m22")] = hmdTransformLocalToWorldMatrix.m22.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m23")] = hmdTransformLocalToWorldMatrix.m23.ToString();

        logData[Array.IndexOf(ColumnNames, "hmdL2W_m30")] = hmdTransformLocalToWorldMatrix.m30.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m31")] = hmdTransformLocalToWorldMatrix.m31.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m32")] = hmdTransformLocalToWorldMatrix.m32.ToString();
        logData[Array.IndexOf(ColumnNames, "hmdL2W_m33")] = hmdTransformLocalToWorldMatrix.m33.ToString();

        logData[Array.IndexOf(ColumnNames, "headVel_Az")] = headVel_Az.ToString();
        logData[Array.IndexOf(ColumnNames, "headVel_Pol")] = headVel_Pol.ToString();

        // logData[Array.IndexOf (ColumnNames, "stimLocalPos_X")] = gameStates.holeLocalPos_X.ToString ();
        // logData[Array.IndexOf (ColumnNames, "stimLocalPos_Y")] = gameStates.holeLocalPos_Y.ToString ();
        // logData[Array.IndexOf (ColumnNames, "stimLocalPos_Z")] = gameStates.holeLocalPos_Z.ToString ();
        Log(logData);

    }
    Vector2 vec2ficks (float x, float y, float z) {
        float r = Mathf.Sqrt (Mathf.Pow (x, 2) + Mathf.Pow (y, 2) + Mathf.Pow (z, 2));
        float az = Mathf.Atan2 (x, z);
        float pol = Mathf.Acos (y / r);
        return new Vector2 (rad2deg (az), 90 - rad2deg (pol));
    }

    float rad2deg (float rad) {
        return (180 / Mathf.PI) * rad;
    }

    static StringBuilder stringbuilderGazeSR = new StringBuilder();
    static StreamWriter swGazeSR = null;
    static int countedGazeSR = 0;

    // Write given values in the log file
    static void Log(string[] values)
    {
        if (swGazeSR == null)
        {
            string filepath = DataLogger.rootFolder + "ID-" + DataManager.ParticipantIdSTATIC + "-gazeSR.csv";
            swGazeSR = open(filepath, string.Join(",", DataLoggerEye.ColumnNames));
        }

        stringbuilderGazeSR.AppendLine(string.Join(",", values));

        countedGazeSR++;
        if (countedGazeSR % 100 == 0)
        {
            swGazeSR.Write(stringbuilderGazeSR);
            stringbuilderGazeSR.Clear();
            swGazeSR.Flush();
        }
    }

    static private StreamWriter open(string filepath, string header)
    {
        StreamWriter sw;
        if (!File.Exists(filepath))
        {
            sw = File.CreateText(filepath);
            sw.WriteLine(header);
            sw.Flush();
        }
        else
        {
            sw = File.AppendText(filepath);
        }
        return sw;
    }
}
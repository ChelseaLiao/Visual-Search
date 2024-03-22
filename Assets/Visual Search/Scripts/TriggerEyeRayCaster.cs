using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using ViveSR.anipal.Eye;

public class TriggerEyeRayCaster : MonoBehaviour
{
    [SerializeField]
    float maxRaycasterLength = 12.0f;
    public static long pressTime;
    public static string lookedItem;


    int layerNumber;
    LayerMask ignoreLayer;

    HighlightAtGazeSR highlight;

    public Camera cam;
    [SerializeField] private LineRenderer GazeRayRenderer;
    private static EyeData_v2 eyeData = new EyeData_v2();
    private bool eye_callback_registered = false;

    // DateTime localDate = DateTime.Now;
    // CultureInfo culture = new CultureInfo("de-DE");

    // Start is called before the first frame update
    void Start()
    {
        if(!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }

        lookedItem = "Null";

        if (GazeRayRenderer == null)
        {
            GazeRayRenderer = GetComponent<LineRenderer>();
        }


        layerNumber = LayerMask.NameToLayer("Ignore Raycast");
        ignoreLayer = 1 << layerNumber;
    }

    private void Update()
    {
        long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }

        Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

        if (eye_callback_registered)
        {
            if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
            else return;
        }
        else
        {
            if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
            else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
            else return;
        }

        Vector3 GazeDirectionCombined = cam.transform.TransformDirection(GazeDirectionCombinedLocal);
        Vector3 endPoint = cam.transform.position + GazeDirectionCombined * maxRaycasterLength;//transform.position + (transform.forward * maxRaycasterLength);
        //RaycastHit hit = CreatRaycast(maxRaycasterLength);

        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, GazeDirectionCombined * maxRaycasterLength);
        Physics.Raycast(ray, out hit, maxRaycasterLength, ~ignoreLayer);

        if (hit.collider != null)
        {
            endPoint = hit.point;
            lookedItem = hit.collider.name;
            Debug.Log("HIT:" + hit.transform.name + "  " + hit.transform.tag);

            highlight = hit.transform.GetComponent<HighlightAtGazeSR>();
            if (highlight != null)
            {
                highlight.GazeFocusChanged(true);
                Debug.Log("looked at " + lookedItem + " at" + now);
            }
        }
        else
        {
            if (highlight != null)
            {
                Debug.Log("Defocus");
                highlight.GazeFocusChanged(false);
                highlight = null;
            }

            GazeRayRenderer.SetPosition(0, cam.transform.position);
            GazeRayRenderer.SetPosition(1, cam.transform.position + GazeDirectionCombined * maxRaycasterLength);
        }
    }

    void OnDestroy()
    {
        Release();
    }

    private void Release()
    {                                                                             
        if (eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        eyeData = eye_data;
    }

    private RaycastHit CreatRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, length, ~ignoreLayer);
        return hit;
    }
}

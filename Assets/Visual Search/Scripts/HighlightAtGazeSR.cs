using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRception;

public class HighlightAtGazeSR : MonoBehaviour
{
    private static readonly int _baseColor = Shader.PropertyToID("_BaseColor");
    public Color highlightColor = Color.red;
    public float animationTime = 0.1f;
    DataLogger dataLogger;

    BlockController blockController;
    TaskController taskController;
    private Renderer _renderer;
    private Color _originalColor;
    private Color _targetColor;

    public void GazeFocusChanged(bool hasFocus)
    {
        //If this object received focus, fade the object's color to highlight color
        if (hasFocus)
        {
            Debug.Log(gameObject.name);
            
            long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (SceneManager.GetActiveScene().buildIndex == 8)
            {
                _targetColor = highlightColor;
                //dataLogger.writeGaze(now, "Unknown", -1, gameObject.name, gameObject.tag, gameObject.transform.position);
            }
            else
                dataLogger.writeGaze(now, blockController.getBlockName(), taskController.taskCount, gameObject.name, gameObject.tag, gameObject.transform.position);
        }
        //If this object lost focus, fade the object's color to it's original color
        else
        {
            _targetColor = _originalColor;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 8)
        {
            dataLogger = GameObject.Find("Logger").GetComponent<DataLogger>();
            blockController = GameObject.Find("BlockController").GetComponent<BlockController>();
            taskController = GameObject.Find("TaskController").GetComponent<TaskController>();

        }
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
        _targetColor = _originalColor;

    }

    private void Update()
    {
        //This lerp will fade the color of the object
        if (_renderer.material.HasProperty(_baseColor)) // new rendering pipeline (lightweight, hd, universal...)
        {
            _renderer.material.SetColor(_baseColor, Color.Lerp(_renderer.material.GetColor(_baseColor), _targetColor, Time.deltaTime * (1 / animationTime)));
        }
        else // old standard rendering pipline
        {
            _renderer.material.color = Color.Lerp(_renderer.material.color, _targetColor, Time.deltaTime * (1 / animationTime));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRception
{
    public class PatternController : MonoBehaviour
{
    enum Reality { AR, AV, VR };
    public List<int> pattern = new List<int>{ 1, 2 };
    // public int patternCounter = 1;
    // Start is called before the first frame update
    void Start()
    {
        Settings.instance.crossfader = 0.4f;
        //pattern.Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // public void goToNextPattern()
    // {
    //     if(patternCounter<pattern.Count)
    //         patternCounter++;
    // }
    // public void goToPreviousPattern()
    // {
    //     if(patternCounter>1)
    //         patternCounter--;
    // }
    // public int getCurrentPattern()
    // {
    //     if(patternCounter==1)
    //         return 1;
    //     else
    //         return 2;
    // }
}
}


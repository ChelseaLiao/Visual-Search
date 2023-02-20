using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VRception
{
    public class BlockController : MonoBehaviour
    {
        public static List<int> reality_order = new List<int>{ 1, 2, 3 };
        public static List<int> difficulty_order = new List<int>{ 1, 2 };
        public static int reality_counter = -1;
        public static int difficulty_counter = -2;
        public static int CurrentDifficulty;
        public static string reality_name = "";
        public static string difficulty_name = "";
        public static string blockName="";
        public static int blockNumber;
        // Start is called before the first frame update
        void Start()
        {           
            if(reality_counter == -1)
            {
                reality_order = reality_order.OrderBy(i => Random.value).ToList();
                foreach(int x in reality_order)
                    Debug.Log(x);
            }
            
            difficulty_counter++;
            
            CurrentDifficulty = getCurrentDifficulty();

            getCurrentCrossfader();
            Debug.Log(Settings.instance.crossfader);
            if(CurrentDifficulty==1)
                blockName= reality_name + "-easy";
            else if(CurrentDifficulty==2)
                blockName= reality_name + "-difficult";
            Debug.Log("blockName:"+blockName);
            //Doesn't work
            //reality_order.Shuffle();
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public int getCurrentReality()
        {
            if (reality_counter == -1) {
                return -1;
            }
            else if (reality_counter >= reality_order.Count) {
                return 1;
            }
            else
            {
                return reality_order[reality_counter];
            }
        }

        public void getCurrentCrossfader()
        {
            int order = getCurrentReality();
            switch(order)
            {
                case 1:
                    reality_name = "AR";
                    Settings.instance.crossfader = -1.0f;
                    break;
                case 2:
                    reality_name = "AV";
                    Settings.instance.crossfader = 0.3f;
                    break;
                case 3:
                    reality_name = "VR";
                    Settings.instance.crossfader = 1.0f;
                    break;
                default:
                    break;
            }
        }

        public int getCurrentDifficulty()
        {
            if (difficulty_counter == -1 || difficulty_counter >= difficulty_order.Count) {
                reality_counter++;
                difficulty_order = difficulty_order.OrderBy(i => Random.value).ToList();
                foreach(int x in difficulty_order)
                    Debug.Log("difficulty order:"+x);
                difficulty_counter = 0;
                return difficulty_order[0];
            }
            else
            {
                return difficulty_order[difficulty_counter];
            }
        }
        // public int getNextReality()
        // {
        //     int c = counter + 1;
        //     if (c == -1)
        //     {
        //         return -1;
        //     }
        //     else if (c >= reality_order.Count)
        //     {
        //         return -2;
        //     }
        //     else
        //     {
        //         return reality_order[c];
        //     }
        // }
    }
}



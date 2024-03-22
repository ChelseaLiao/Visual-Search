using UnityEngine;
namespace VRception
{
    /// <summary>
    /// The script cloneable is required for prefabs that are instantiated during runtime.
    /// If the script is not attached and the prefab is instatiated during runtime, it will cause errors on other network clients as they cannot properly handle the instantiated prefab.
    /// </summary>
    public class Cloneable : MonoBehaviour
    {
        //// SECTION "Cloneable Settings"
        [Header("Cloneable Settings", order = 0)]
        [Helpbox("The script cloneable is required for prefabs that are instantiated during runtime. If the script is not attached and the prefab is instatiated during runtime, it will cause errors on other network clients as they cannot properly handle the instantiated prefab. Thus, this script should be attached to the prefabs offered in the menu, marker prefabs, and every other prefab/gameobject that implements the 'Interactable' script with at least one of the following module: 'ModuleDuplicate' and 'ModuleInterface.' Below, please specify the exact name of the prefab and make sure it is in a 'Resources' folder. Make sure the name of the prefab is unique.", order = 1)]
        [Tooltip("Name of prefab in a 'Resources' folder.", order = 2)]
        public string prefabName = null;

        // Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
        void OnValidate()
        {
            // In case the name of the prefab has not been set yet, try to come up with the most reasonable one
            if(string.IsNullOrEmpty(this.prefabName))
                this.prefabName = this.gameObject.name;
        }
    }
}
using UnityEngine;

namespace VRception
{
	[RequireComponent(typeof(References))]
	public class ControllerPlayer : MonoBehaviour
	{
		// Stores a reference to the references script
        private References references;

		// Awake is called when the script instance is being loaded
        void Awake()
        {
            // Get a reference on the references script
            this.references = this.GetComponent<References>();
	    }

		// Start is called before the first frame update
        void Start()
        {
			// Initalize settings with references
            	Settings.instance.Initalize(this.references);
            	// Register for crossfader change
            	//Settings.instance.onCrossfaderChange += this.OnCrossfaderChange;
            	//this.OnCrossfaderChange();

        }
	}
}
			
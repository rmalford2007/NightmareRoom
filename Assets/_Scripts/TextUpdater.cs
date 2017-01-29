using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/****************************************
 * This is a UI helper class. Used with the Stats class. We can subscribe to a stat and watch it for changes.
 *  Each time it changes we update the UI display in some special way. Whether it's updating text as health goes down,
 *  or resizing a visual bar like a health bar.
 * *************************************/

public class TextUpdater : MonoBehaviour {

    private Text displayText;
    public enum StatTypeDisplay
    {
        HEALTH,
        RESOURCE,
        GOAL
    }
    public StatTypeDisplay displayType = StatTypeDisplay.RESOURCE; //the type of stat we are tracking
    public GameObject trackObject; //the game object that contains the stat component script to watch
    public GameObject statResizeBar; //the game object that contains the image bar to resize 
    private Stats statObject;
	// Use this for initialization
	void Awake () {
        displayText = GetComponent<Text>();
	}

    //Susbscribe to stat object we are watching, based on type of stat we are watching
    void OnEnable()
    {
        if (trackObject == null)
            trackObject = GameObject.FindGameObjectWithTag("Player");
        if (trackObject != null)
        {
            statObject = trackObject.GetComponent<Stats>();
            
            
            if (displayType == StatTypeDisplay.RESOURCE)
            {
                statObject.StatChanged_Resource += new StatInteractionHandler(OnStatChanged);
                SetStatDisplay(statObject.resources);
            }
            else if (displayType == StatTypeDisplay.HEALTH)
            {
                statObject.StatChanged_Health += new StatInteractionHandler(OnStatChanged);
                SetStatDisplay(statObject.health);
            }
            else if (displayType == StatTypeDisplay.GOAL)
            {
                statObject.StatChanged_Goal += new StatInteractionHandler(OnStatChanged);
                SetStatDisplay(statObject.goal);
            }
        }
    }

    //Unsusbscribe from stat object we are watching
    void OnDisable()
    {
        if(statObject != null)
        {
            if (displayType == StatTypeDisplay.HEALTH)
                statObject.StatChanged_Health -= new StatInteractionHandler(OnStatChanged);
            if (displayType == StatTypeDisplay.RESOURCE)
                statObject.StatChanged_Resource -= new StatInteractionHandler(OnStatChanged);
            if (displayType == StatTypeDisplay.GOAL)
                statObject.StatChanged_Goal -= new StatInteractionHandler(OnStatChanged);
        }
    }

    //Setup the display based on the type of stat we are watching
    void SetStatDisplay(Stat s)
    {
        if (s != null)
        {
            if (displayType == StatTypeDisplay.RESOURCE)
                displayText.text = "Resouces : " + ((int)s.current).ToString();
            else if (displayType == StatTypeDisplay.HEALTH)
            {
                // print("updating health");
                displayText.text = "Health";
                if (statResizeBar != null)
                {
                    //Resize bar to percentage of current / max
                    Vector3 resizeScale = statResizeBar.transform.localScale;
                    resizeScale.x = s.percentage();
                    statResizeBar.transform.localScale = resizeScale;
                }
            }
            else if(displayType == StatTypeDisplay.GOAL)
            {
                if (statResizeBar != null)
                {
                    //Resize bar to percentage of current / max
                    Vector3 resizeScale = statResizeBar.transform.localScale;
                    resizeScale.x = s.percentage();
                    statResizeBar.transform.localScale = resizeScale;
                }
            }
        }
    }

    //Each time the watched stat is changed, call SetStatDisplay
    protected virtual void OnStatChanged(object sender, Stat s)
    {
        SetStatDisplay(s);
    }
}

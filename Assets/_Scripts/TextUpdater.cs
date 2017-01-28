using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUpdater : MonoBehaviour {

    private Text displayText;
    public enum StatTypeDisplay
    {
        HEALTH,
        RESOURCE,
        GOAL
    }
    public StatTypeDisplay displayType = StatTypeDisplay.RESOURCE;
    public GameObject trackObject;
    public GameObject statResizeBar;
    private Stats statObject;
	// Use this for initialization
	void Awake () {
        displayText = GetComponent<Text>();
	}

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

    protected virtual void OnStatChanged(object sender, Stat s)
    {
        SetStatDisplay(s);
    }
}

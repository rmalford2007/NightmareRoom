using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat
{
    public float current;
    public float max;
    public float percentage()
    {
        if (max > 0)
        {
            return current / max;
        }
        else
            return 0.0f;
    }
}

public delegate void StatInteractionHandler(object sender, Stat s);

public class Stats : MonoBehaviour {

    public Stat health;
    public Stat resources;
    public Stat goal;
    //public Stat armor;
    public event StatInteractionHandler StatChanged_Health;
    public event StatInteractionHandler StatChanged_Resource;
    public event StatInteractionHandler StatChanged_Goal;

    public virtual void OnStatChanged_Health(Stat s)
    {
        if (StatChanged_Health != null)
        {
            StatChanged_Health(this, s);
        }
    }

    public virtual void OnStatChanged_Resource(Stat s)
    {
        if (StatChanged_Resource != null)
        {
            StatChanged_Resource(this, s);
        }
    }

    public virtual void OnStatChanged_Goal(Stat s)
    {
        if (StatChanged_Goal != null)
        {
            StatChanged_Goal(this, s);
        }
    }

    // Use this for initialization
    void Start () {
	    
	}

    public void ApplyDamage(float amount)
    {
        health.current -= amount;
        OnStatChanged_Health(health);
    }

    public void SetHealthMax(float value)
    {
        health.max = value;
        OnStatChanged_Health(health);
    }

    public void AddResources(float amount)
    {
        resources.current += amount;
        OnStatChanged_Resource(resources);
    }

    public float GiveResource(float gatherRate, float timeGathering)
    {
        if (resources.current == 0)
            return 0.0f;
        float giveBack = timeGathering * gatherRate;
        if (resources.current > giveBack)
        {
            resources.current -= giveBack;
            OnStatChanged_Resource(resources);
            return giveBack;
        }
        giveBack = resources.current;
        resources.current -= resources.current;
        OnStatChanged_Resource(resources);
        return giveBack;
    }

    public void AddGoal(float amount)
    {
        goal.current += amount;
        OnStatChanged_Goal(goal);
    }

    public void SetGoal(float amount)
    {
        goal.current = amount;
        OnStatChanged_Goal(goal);
    }

    public void SetResource(float amount)
    {
        resources.current = amount;
        OnStatChanged_Resource(resources);
    }

    public void SetHealth(float amount)
    {
        health.current = amount;
        OnStatChanged_Health(health);
    }
}

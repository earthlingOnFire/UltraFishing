using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UltraFishing;

public class ZoneBuilder
{
    private bool onenter;
    private SetFishZone SetFishZone;
    private GameObject gameObject;
    
    public ZoneBuilder(SetFishZone Zone)
    {
        this.gameObject = Zone.gameObject;
        SetFishZone = Zone;
        SetFishZone.customMinDistance = false;
        SetFishZone.onEnter = true;
        SetFishZone.restorePreviousOnExit = true;
    }
    public static ZoneBuilder CreateZone()
    {
        GameObject zone = new GameObject("FishZone", typeof(SetFishZone));

        BoxCollider collider = zone.AddComponent<UnityEngine.BoxCollider>();
        collider.isTrigger = true;
        return new ZoneBuilder(zone.GetComponent<SetFishZone>());
    }
    public ZoneBuilder SetPosition(float x, float y, float z)
    {
        gameObject.transform.position = new Vector3(x, y, z);

        return this;
    }
    public ZoneBuilder SetLocalScale(float x, float y, float z)
    {
        gameObject.transform.localScale = new Vector3(x, y, z);

        return this;
    }
    public ZoneBuilder SuggestedDistance(float x)
    {
        SetFishZone.suggestedFishingDistance = x;

        return this;
    }
    public ZoneBuilder CustomMinDistance(float x)
    {
        SetFishZone.customMinDistance = true;
        SetFishZone.minDistance = x;
        return this;
    }
}

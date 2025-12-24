using System.Collections.Generic;
using UnityEngine;

namespace UltraFishing;
public class LateSetWater : MonoBehaviour {

  public List<string> Fish = new List<string>();
  public string WaterName = "ERROR: NAME UNASSIGNED";
  public Color color = Color.magenta;
  public List<Transform> Water = new List<Transform>();
  private bool triggered = false;

  public LateSetWater Addfish (string fish) {
    Fish.Add(fish);
    return this;
  }

  public LateSetWater AddObject(Transform water) {
    Water.Add(water);

    return this;
  }

  void OnEnable() {
    if (triggered == false) {
      foreach (var child in Water) {
        // Plugin.logger.LogWarning($"Currently adding to: {child.name}");

        WaterBuilder.SetWater(child.transform).AddFishes(Fish).SetUp(WaterName, color);
        // Plugin.logger.LogWarning("Completed loop");
      }

      triggered = true;
    }
  }
}

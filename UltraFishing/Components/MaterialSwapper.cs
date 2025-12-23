using UnityEngine;
using System.Collections.Generic;

namespace UltraFishing;

public class MaterialSwapper : MonoBehaviour {
  public Material mat;
  public List<string> ignoreLevels;
  public int layer;

  void Start() {
    Renderer rend = GetComponent<Renderer>();
    if (gameObject.layer == layer && !ignoreLevels.Exists(s => s == SceneHelper.CurrentScene)) {
      rend.material = mat;
    }
  }
}

using HarmonyLib;
using UnityEngine;

namespace UltraFishing;

[HarmonyPatch]
public static class OtherPatches {

  [HarmonyPostfix]
  [HarmonyPatch(typeof(Glass), "Shatter")]
  private static void Glass_Shatter_Postfix(Glass __instance) {
    if (SceneHelper.CurrentScene != "Level 0-1" && SceneHelper.CurrentScene != "Level 0-2") return;
    
    BoxCollider[] boxColliders = __instance.gameObject.GetComponents<BoxCollider>();
    if (boxColliders == null) return;

    foreach(var col in boxColliders) {
      col.enabled = false;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(typeof(FishManager), "UnlockFish")]
  private static void FishManager_UnlockFish_Postfix(ref FishObject fish) {
    GlobalFishManager.UnlockFish(fish);
  }
  
  [HarmonyPostfix]
  [HarmonyPatch(typeof(ItemIdentifier), "PutDown")]
  private static void ItemIdentifier_PutDown_Postfix(ItemIdentifier __instance) {
    FishObjectReference fishRef = __instance.GetComponent<FishObjectReference>();
    if (fishRef == null) return;
    FishObject fish = fishRef.fishObject;
    switch (fish.fishName) {
      case "Coin":
        GameObject coin = __instance.transform.Find("Coin").gameObject;
        Camera cam = CameraController.Instance.GetComponent<Camera>();
        GameObject camObj = cam.gameObject;
        GunControl gc = GunControl.Instance;
        FistControl fc = FistControl.Instance;

        fc.currentPunch.CoinFlip();

        GameObject obj = GameObject.Instantiate(coin, camObj.transform.position + camObj.transform.up * -0.5f, camObj.transform.rotation);
        obj.SetActive(true);
        obj.GetComponent<Coin>().sourceWeapon = gc.currentWeapon;
        MonoSingleton<RumbleManager>.Instance.SetVibration(RumbleProperties.CoinToss);
        obj.GetComponent<Rigidbody>().AddForce(camObj.transform.forward * 20f + Vector3.up * 15f + MonoSingleton<PlayerTracker>.Instance.GetPlayerVelocity(trueVelocity: true), ForceMode.VelocityChange);

        GameObject.Destroy(__instance.gameObject);
        break;
      case "Flying Demon Fish":
        GameObject proj = Plugin.bundle.LoadAsset<GameObject>("assets/bundles/fishingstuff/projectiles/flying demon fish/projectile.prefab");
        Object.Instantiate(proj, NewMovement.Instance.transform.position, CameraController.Instance.transform.rotation);

        GameObject.Destroy(__instance.gameObject);
        break;
    }
  }
}

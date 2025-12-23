using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TMPro;

namespace UltraFishing;

[HarmonyPatch]
public static class ComponentReplacementPatches {

  // Replace FishingRodWeapon with NewFishingRod
  [HarmonyPrefix]
  [HarmonyPatch(typeof(FishingRodWeapon), "Awake")]
  private static bool FishingRodWeapon_Awake_Prefix(FishingRodWeapon __instance) {
    if (__instance is NewFishingRod) {
      return true;
    }

    FishingRodWeapon rod = __instance;
    GameObject gameObject = rod.gameObject;
    gameObject.SetActive(false);
    NewFishingRod newRod = gameObject.AddComponent<NewFishingRod>();

    newRod.animator = rod.animator;
    newRod.targetPrefab = rod.targetPrefab;
    newRod.baitPrefab = rod.baitPrefab;
    newRod.rodTip = rod.rodTip;
    newRod.fishPickupTemplate = rod.fishPickupTemplate;
    newRod.pullSound = rod.pullSound;
    newRod.targetingCircle = rod.targetingCircle;
    newRod.spawnedBaitCon = rod.spawnedBaitCon;
    newRod.state = rod.state;
    newRod.selectedPower = rod.selectedPower;
    newRod.climaxed = rod.climaxed;
    newRod.baitThrown = rod.baitThrown;
    newRod.distanceAfterThrow = rod.distanceAfterThrow;
    newRod.fishHooked = rod.fishHooked;
    newRod.currentFishPool = rod.currentFishPool;
    newRod.currentWater = rod.currentWater;
    newRod.hookedFishe = rod.hookedFishe;
    newRod.fishTolerance = rod.fishTolerance;
    newRod.fishDesirePosition = rod.fishDesirePosition;
    newRod.playerProvidedPosition = rod.playerProvidedPosition;
    newRod.playerPositionVelocity = rod.playerPositionVelocity;
    newRod.timeSinceBaitInWater = rod.timeSinceBaitInWater;
    newRod.timeSinceAction = rod.timeSinceAction;
    newRod.noFishErrorDisplayed = rod.noFishErrorDisplayed;
    gameObject.GetComponentInChildren<FishingRodAnimEvents>().weapon = newRod;

    Object.Destroy(rod);
    gameObject.SetActive(true);

    return false;
  }

  [HarmonyPrefix]
  [HarmonyPatch(typeof(FishingRodWeapon), "Update")]
  private static bool FishingRodWeapon_Update_Prefix(FishingRodWeapon __instance) {
    if (__instance is NewFishingRod) {
      NewFishingRod newRod = (NewFishingRod)__instance;
      newRod.NewUpdate();
      return false;
    }
    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(typeof(FishingRodWeapon), "FishCaughtAndGrabbed")]
  private static bool FishingRodWeapon_FishCaughtAndGrabbed_Prefix(FishingRodWeapon __instance) {
    if (__instance is NewFishingRod) {
      NewFishingRod newRod = (NewFishingRod)__instance;
      newRod.FishCaughtAndGrabbed();
      return false;
    }
    return true;
  }

  [HarmonyPrefix]
  [HarmonyPatch(typeof(FishingRodWeapon), "ThrowBaitEvent")]
  private static bool FishingRodWeapon_ThrowBaitEvent_Prefix(FishingRodWeapon __instance) {
    if (__instance is NewFishingRod) {
      NewFishingRod newRod = (NewFishingRod)__instance;
      newRod.ThrowBaitEvent();
      return false;
    }
    return true;
  }

  // Replace FishEncyclopedia with GlobalFishEncyclopedia
  [HarmonyPrefix]
  [HarmonyPatch(typeof(FishEncyclopedia), "Start")]
  private static bool FishEncyclopedia_Start_Prefix(FishEncyclopedia __instance) {
    if (__instance is GlobalFishEncyclopedia) {
      GlobalFishEncyclopedia globalFishEncyclopedia = (GlobalFishEncyclopedia)__instance;
      globalFishEncyclopedia.StartEncyclopedia();
      return false;
    }
    FishEncyclopedia enc = __instance;
    GameObject gameObject = enc.gameObject;
    GlobalFishEncyclopedia newEnc = gameObject.AddComponent<GlobalFishEncyclopedia>();

    newEnc.fishPicker = enc.fishPicker;
    newEnc.fishInfoContainer = enc.fishInfoContainer;
    newEnc.fishName = enc.fishName;
    newEnc.fishDescription = enc.fishDescription;
    newEnc.fishGrid = enc.fishGrid;
    newEnc.fishButtonTemplate = enc.fishButtonTemplate;
    newEnc.fish3dRenderContainer = enc.fish3dRenderContainer;
    newEnc.fishButtons  = enc.fishButtons;

    Transform backButton = newEnc.fishInfoContainer.transform.Find("Window/Back Button");

    GameObject previousButton = Object.Instantiate(backButton.gameObject, newEnc.fishPicker.transform.parent);
    previousButton.name = "Previous Button";
    previousButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "<<";
    previousButton.transform.localScale = new Vector3(1.4f, 1.4f, 1);
    previousButton.transform.position += Vector3.down * 0.0425f;  
    previousButton.GetComponent<Button>().onClick.AddListener(delegate {
      newEnc.PreviousPage();
    });

    GameObject nextButton = Object.Instantiate(backButton.gameObject, newEnc.fishPicker.transform.parent);
    previousButton.name = "Next Button";
    nextButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ">>";
    nextButton.transform.localScale = previousButton.transform.localScale;
    if (SceneHelper.CurrentScene.Contains("construct") || SceneHelper.CurrentScene.Contains("5-S")) {
      nextButton.transform.position = previousButton.transform.position + Vector3.left * 0.8313f;
    }
    else {
      nextButton.transform.position = previousButton.transform.position + Vector3.right * 0.8313f;
    }
    nextButton.GetComponent<Button>().onClick.AddListener(delegate {
      newEnc.NextPage();
    });
    newEnc.fishInfoContainer.transform.SetAsLastSibling();

    backButton.GetComponent<ShopButton>().toActivate = new GameObject[]{ newEnc.fishPicker };

    GameObject manneFishDescPrefab = Plugin.bundle.LoadAsset<GameObject>("Assets/Bundles/fishingstuff/HOUSE OF FINS.prefab");
    newEnc.mannequinFishDescription = GameObject.Instantiate(manneFishDescPrefab, newEnc.fishDescription.transform.parent);
    newEnc.mannequinFishDescription.transform.localPosition += Vector3.right*2.5f;
    newEnc.mannequinFishDescription.SetActive(false);

    Object.Destroy(enc);
    return false;
  }
}

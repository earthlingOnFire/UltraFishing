using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;
using Object = UnityEngine.Object;
using TMPro;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.XR;
using System.Runtime.CompilerServices;

namespace UltraFishing;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin {	
  public const string PLUGIN_GUID = "com.earthlingOnFire.UltraFishing";
  public const string PLUGIN_NAME = "UltraFishing";
  public const string PLUGIN_VERSION = "1.0.0";
  public static AssetBundle bundle;
  public static ManualLogSource logger;
  public static string modDir;
  public static bool linked = false;
  public static GameObject fishingRod;
  public static GameObject fishingCanvas;
  public static GameObject baitConsumedSound;
  public static GameObject terminal;
  private static Shader MainShader;
  public static readonly string[] NoRodLevels = [ "Level 1-S" ];

  private void Awake() {
    MainShader = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/MasterShader/ULTRAKILL-Standard.shader").WaitForCompletion();

    gameObject.hideFlags = HideFlags.HideAndDontSave;
    logger = Logger;
  }

  public static void ReplaceShader(Material mat, Shader shader) {
    if (mat == null || mat.shader == null)  return;

    int renderQueue = mat.renderQueue;
    Shader shader2 = mat.shader;
    if (Shader.Find(shader2.name) != null) {
      if (mat.shader.name != "Standard") {
        mat.shader = Shader.Find(shader2.name);
      }
      else {
        mat.shader = shader;
      }
      mat.renderQueue = renderQueue;
    }
    else if (shader2.name == shader.name) {
      mat.shader = shader;
      mat.renderQueue = renderQueue;
    }
    else {
      mat.renderQueue = renderQueue;
    }
  }

  public static void ReplaceAssets() {

    List<Material> Materials = [];
    Dictionary<string, AudioMixer> Sounds = [];
    void Add(string name) => Sounds.Add(name, Addressables.LoadAssetAsync<AudioMixer>(name).WaitForCompletion());
    Add("AllAudio");
    Add("DoorAudio");
    Add("GoreAudio");
    Add("MusicAudio");
    Add("UnfreezeableAudio");
    
    List<GameObject> gameObjects = [.. bundle.LoadAllAssets<GameObject>()];

    FishObject[] customFishes = bundle.LoadAllAssets<FishObject>();
    for (int i = 0; i < customFishes.Length; i++) {
      gameObjects.Add(customFishes[i].worldObject);
      if (customFishes[i].customPickup != null) {
        gameObjects.Add(customFishes[i].customPickup.gameObject);
      }
    }
    
    Material[] sharedMaterials;
    foreach (GameObject val in gameObjects) {
      if (val.GetComponentsInChildren<AudioSource>(true) != null) {
        AudioSource[] componentsInChildren = val.GetComponentsInChildren<AudioSource>(true);
        foreach (AudioSource val2 in componentsInChildren) {
          if (val2.outputAudioMixerGroup != null && Sounds.TryGetValue(val2.outputAudioMixerGroup.audioMixer.name, out var value))
            val2.outputAudioMixerGroup.audioMixer.outputAudioMixerGroup = value.FindMatchingGroups("Master").FirstOrDefault();
        }
      }

      //study this
      if (val.GetComponentsInChildren<Renderer>(true) != null) {
        Renderer[] componentsInChildren2 = val.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer val3 in componentsInChildren2) {
          if (val3.sharedMaterial != null) {
            ReplaceShader(val3.sharedMaterial, MainShader);
          }
          if (val3.sharedMaterials != null && val3.sharedMaterials.Length != 0) {
            sharedMaterials = val3.sharedMaterials;
            foreach (Material val4 in sharedMaterials) {
              Materials.Add(val4);
              ReplaceShader(val4, MainShader);
            }
          }
        }
      }
      if (val.GetComponentsInChildren<ParticleSystemRenderer>(true) == null) {
        continue;
      }
      ParticleSystemRenderer[] componentsInChildren3 = val.GetComponentsInChildren<ParticleSystemRenderer>(true);
      foreach (ParticleSystemRenderer val5 in componentsInChildren3) {
        if (val5.sharedMaterial != null) {
          ReplaceShader(val5.sharedMaterial, MainShader);
        }
        if (val5.sharedMaterials != null && val5.sharedMaterials.Length != 0) {
          sharedMaterials = val5.sharedMaterials;
          foreach (Material val6 in sharedMaterials) {
            Materials.Add(val6);
            ReplaceShader(val6, MainShader);
          }
        }
      }
    }
    sharedMaterials = bundle.LoadAllAssets<Material>();
    foreach (Material val7 in sharedMaterials) {
      if (!Materials.Contains(val7)) {
        Materials.Add(val7);
        ReplaceShader(val7, MainShader);
      }
    }
  }

  private void Start() {
    string modPath = Assembly.GetExecutingAssembly().Location.ToString();
    modDir = Path.GetDirectoryName(modPath);

    LoadBundle();
    LoadAssets();

    GlobalFishManager.Start();

    new Harmony(PLUGIN_GUID).PatchAll();

    logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
  }

  private void LoadBundle() {
    string bundlePath = Path.Combine(modDir, "fishingstuff.fishbundle");
    bundle = AssetBundle.LoadFromFile(bundlePath);
    if (bundle == null) {
      logger.LogError("Bundle could not be loaded");
    }
  }

  private void LoadAssets() {
    if (linked == false) {
      ReplaceAssets();
      linked = true;
    }

    fishingCanvas = AssetHelper.LoadPrefab("Assets/Prefabs/UI/FishingCanvas.prefab");
    fishingRod = AssetHelper.LoadPrefab("Assets/Prefabs/Fishing/Fishing Rod Weapon.prefab");
    baitConsumedSound = AssetHelper.LoadPrefab("Assets/Particles/SoundBubbles/Bait Consumed Sound.prefab");

    if (bundle != null) {
      WeaponIcon rodIcon = fishingRod.AddComponent<WeaponIcon>();
      rodIcon.weaponDescriptor = bundle.LoadAsset<WeaponDescriptor>("assets/bundles/fishingstuff/rod descriptor.asset");

      terminal = bundle.LoadAsset<GameObject>("assets/bundles/fishingstuff/fishing enc terminal.prefab");
    }
  }
}

[HarmonyPatch]
public static class Patches {

  [HarmonyPostfix]
  [HarmonyPatch(typeof(GunControl), "Start")]
  private static void GunControl_Start_Postfix() {
    if (Object.FindObjectOfType<FishingHUD>() != null || Plugin.NoRodLevels.Contains(SceneHelper.CurrentScene)) return;

    GameObject fishManagerObj = new GameObject("FishManager");
    fishManagerObj.SetActive(value: false);
    fishManagerObj.AddComponent<FishManager>().fishDbs = new FishDB[]{};
    fishManagerObj.SetActive(value: true);

    SetupWaters();

    GameObject fishingCanvasClone = Object.Instantiate(Plugin.fishingCanvas);
   
    AddWeapon(5, Plugin.fishingRod);

    LoadFishTerminal();
  }

  [HarmonyPostfix]
  [HarmonyPatch(typeof(FishManager), "UnlockFish")]
  private static void FishManager_UnlockFish_Postfix(ref FishObject fish) {
    GlobalFishManager.UnlockFish(fish);
  }
  
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

    Object.Destroy(enc);

    return false;
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
    }
  }

  private static void LoadFishTerminal() {
    string scene = SceneHelper.CurrentScene;

    if (!(scene.Contains("Level") || scene.Contains("construct") || scene.Contains("Museum"))) return;

    GameObject terminalClone;

    if (scene.Contains("construct")) {
      terminalClone = Object.Instantiate(Plugin.terminal);
      terminalClone.transform.position = new Vector3(-37, -10, 335.125f);
      terminalClone.transform.localEulerAngles = new Vector3(0, 0, 180);
      return;
    }

    GameObject firstRoom;
    switch (scene) {
      case "Level 6-1":
        firstRoom = GenericHelper.FindGameObject("Interiors/FirstRoom");
        break;
      default:
        firstRoom = GenericHelper.FindGameObjectContaining("FirstRoom");
        break;
    }

    if (firstRoom == null) {
      Plugin.logger.LogError("No FirstRoom could be found!");
      return;
    }

    terminalClone = Object.Instantiate(Plugin.terminal, firstRoom.transform.GetChild(0));
    terminalClone.transform.localPosition = new Vector3(-6.5f, 2, 32);
    terminalClone.transform.localEulerAngles = Vector3.zero;
  }

  private static void AddWeapon(int slot, GameObject weapon) {
    GunControl gunControl = GunControl.Instance;

    if (slot >= gunControl.slots.Count) return;

    if (gunControl.slots[slot].Exists(w => w.name == weapon.name + "Clone")) return;

    GameObject weaponClone = Object.Instantiate(weapon, gunControl.transform);
    gunControl.slots[slot].Add(weaponClone);
    gunControl.UpdateWeaponList(false);
    weaponClone.SetActive(value: false);
  }

  private static void SetupWaters() {
    switch (SceneHelper.CurrentScene) {
      case "uk_construct":
        WaterBuilder.SetWater("Water Tri")
          .AddFish("Funny Stupid Fish (Friend)")
          .AddFish("PITR Fish")
          .AddFish("Trout")
          .AddFish("Metal Fish")
          .AddFish("Chomper")
          .AddFish("Bomb Fish")
          .AddFish("Eyeball")
          .AddFish("Frog (?)")
          .AddFish("Dope Fish")
          .AddFish("Stickfish")
          .AddFish("Cooked Fish")
          .AddFish("Shark")
          .SetUp("Garry's Lake", Color.green);
        break;
      case "CreditsMuseum2":
        WaterBuilder.SetWater("__Room_Aquarium/", 8)
          .AddFish("Funny Stupid Fish (Friend)")
          .AddFish("PITR Fish")
          .AddFish("Trout")
          .AddFish("Metal Fish")
          .AddFish("Chomper")
          .AddFish("Bomb Fish")
          .AddFish("Eyeball")
          .AddFish("Frog (?)")
          .AddFish("Dope Fish")
          .AddFish("Stickfish")
          .AddFish("Cooked Fish")
          .AddFish("Shark")
          .SetUp("Aquarium", Color.cyan);
        WaterBuilder.SetWater("__Room_Courtyard/__Level Geo/Water Fountain/Water fountain_water_1")
          .AddFish("Mannequin Fish")
          .SetUp("Fountain", Color.cyan);
        WaterBuilder.SetWater("__Room_FrontDesk_1/__Level geo/Cube (3)")
          .AddFish("Wise Fish")
          .SetUp("Credits", Color.magenta);
        WaterBuilder.SetWater("__Room_Large_Lower/__Level Geo/water")
          .AddFish("Wise Fish")
          .SetUp("Credits", Color.magenta);
        break;
      case "Level 0-2":
        WaterBuilder.SetWater("3 - Blood Room/3 Nonstuff/Decorations/Mulchflow")
          .AddFish("Filthy Screaming Fish (Filsh)")
          .SetUp("Meat", Color.red);
        WaterBuilder.SetWater("3 - Blood Room/3 Nonstuff/Decorations/Mulchflow/Cube")
          .AddFish("Filthy Screaming Fish (Filsh)")
          .SetUp("Meat", Color.red);
        WaterBuilder.SetWater("3 - Blood Room/3 Nonstuff/Decorations/Mulchflow (1)")
          .AddFish("Filthy Screaming Fish (Filsh)")
          .SetUp("Meat", Color.red);
        WaterBuilder.SetWater("3 - Blood Room/3 Nonstuff/Decorations/Mulchflow (1)/Cube")
          .AddFish("Filthy Screaming Fish (Filsh)")
          .SetUp("Meat", Color.red);
        WaterBuilder.SetWater("6 - Crusher Arena/6 Nonstuff/Floor", 4)
          .AddFish("Filthy Screaming Fish (Filsh)")
          .AddMeshCollider()
          .SetUp("Meat", Color.red);
        WaterBuilder.SetWater("7 - Crusher Hallway/7 Nonstuff/Floor/Blood/")
          .AddFish("Filthy Screaming Fish (Filsh)")
          .SetUp("Meat", Color.red);
        foreach (int childIndex in new int[] { 0, 1, 3, 5, 7 }) {
          WaterBuilder.SetWater("9-9B Tunnel/BloodRiver/", childIndex)
            .AddFish("Filthy Screaming Fish (Filsh)")
            .SetUp("Meat", Color.red);
        }
        break;
      case "Level 0-5":
        WaterBuilder.SetWater("2 - Lava Foundry/Lava/", 0)
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("2 - Lava Foundry/Lava/", 1)
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        break;
      case "Level 1-1":

        WaterBuilder.SetWater("7 - Castle Entrance/7 Nonstuff/Ground/Cube/")
          .AddFish("Nil")
          .SetUp("Steam", Color.magenta);
        WaterBuilder.SetWater("6 - Waterfall Arena/6 Nonstuff/Cliff and Waterfall", 0, "GameObject")
          .AddFish("null")
          .SetUp("Waterfall", Color.magenta);
        WaterBuilder.SetWater("6 - Waterfall Arena/6 Nonstuff/Cliff and Waterfall", 2, "GameObject")
          .AddFish("null")
          .SetUp("Waterfall", Color.magenta);
        // shitty fix for deltakill compat
        GameObject fountain = GenericHelper.FindGameObject("1 - First Field/1 Stuff/1 - Darker_Fountain(Clone)/fountain/Cylinder (1)");
        if (fountain == null) {
          fountain = GenericHelper.FindGameObject("1 - First Field/1 Stuff/Fountain/Cylinder"); // works but not after coin
        }
        WaterBuilder.SetWater(fountain)
          .AddFish("Coin")
          .SetUp("Fountain", Color.cyan);
        break;
      case "Level 1-2":

        WaterBuilder.SetWater(GenericHelper.FindGameObject("5 - Double Hallway/5 Nonstuff/Floor/").transform.GetChild(8).gameObject)
          .AddFish("null")
          .SetUp("Castle Water", Color.magenta);

        //sewer?

        WaterBuilder.SetWater(GenericHelper.FindGameObject("3 - Stairs Room/3 Nonstuff/Floor/").transform.GetChild(7).gameObject)
          .AddFish("Nil")
          .SetUp("Castle Water", Color.magenta);


        foreach (Transform objects in GenericHelper.FindGameObject("7 - Castle Entrance/7 Nonstuff/Sewer/Water").transform) {
          WaterBuilder.SetWater(objects.gameObject)
            .AddFish("Nil")
            .SetUp("Castle Water", Color.magenta);
        }

        WaterBuilder.SetWater("7 - Castle Entrance/7 Nonstuff/Sewer/GreenWater")
          .AddFish("Cancerous Fish")
          .SetUp("Cancerous Water", Color.green);
        WaterBuilder.SetWater("7B - Lava Room/Floor/Lava/")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        break;
      case "Level 1-3":

        //water
        foreach (Transform objects in GenericHelper.FindGameObject("R1 - Courtyard/R1 Nonstuff/Decorations/Water").transform) {
          WaterBuilder.SetWater(objects.gameObject)
            .AddFish("Nil")
            .SetUp("Castle Water", Color.magenta);
        }

        foreach (Transform objects in GenericHelper.FindGameObject("R3 - Final Arena/R3 Nonstuff/Water/Water (Colliders)").transform) {
          WaterBuilder.SetWater(objects.gameObject)
            .AddFish("Nil")
            .SetUp("Castle Water", Color.magenta);
        }

        foreach (Transform objects in GenericHelper.FindGameObject("B2-B Stairs Hallway/B2-B Nonstuff/Water").transform) {
          WaterBuilder.SetWater(objects.gameObject)
            .AddFish("Nil")
            .SetUp("Castle Water", Color.magenta);
        }
        foreach (Transform objects in GenericHelper.FindGameObject("B2 -> B2-B Water/Water/").transform) {
          WaterBuilder.SetWater(objects.gameObject)
            .AddFish("Nil")
            .SetUp("Castle Water", Color.magenta);
        }
        foreach (Transform objects in GenericHelper.FindGameObject("B2 -> B2-B Water/Water/Cube (2)/").transform) {
          WaterBuilder.SetWater(objects.gameObject)
            .AddFish("Nil")
            .SetUp("Castle Water", Color.magenta);
        }
        WaterBuilder.SetWater(GenericHelper.FindGameObject("B2-A Water Hallway/B2-A Nonstuff/Floor/").transform.GetChild(5).gameObject)
          .AddFish("Nil")
          .SetUp("Castle Water", Color.magenta);
        WaterBuilder.SetWater(GenericHelper.FindGameObject("B2-A Water Hallway/B2-A Nonstuff/Floor/").transform.GetChild(6).gameObject)
          .AddFish("Nil")
          .SetUp("Castle Water", Color.magenta);
        WaterBuilder.SetWater(GenericHelper.FindGameObject("B2-A Water Hallway/B2-A Nonstuff/Floor/").transform.GetChild(7).gameObject)
          .AddFish("Nil")
          .SetUp("Castle Water", Color.magenta);
        WaterBuilder.SetWater(GenericHelper.FindGameObject("S - Secret Fight/S Nonstuff/Water/Cube/"))
          .AddFish("Nil")
          .SetUp("Castle Water", Color.magenta);
        //waterend

        WaterBuilder.SetWater("R2 - Second Arena/R2 Nonstuff/Lava")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("B1-C Lava Staircase/B1-C Nonstuff/Floor/Cube")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("B1-D Lava Hallway/B1-D Nonstuff/Lava/Cube-clone")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        break;

      case "Level 1-4":
        WaterBuilder.SetWater("V2 - Arena/V2 Nonstuff/Floor/Water/")
          .AddFish("NaN")
          .SetUp("Pond", Color.magenta);
        WaterBuilder.SetWater("2 - Bridge/2 Nonstuff/Start Side/Plane/")
          .AddFish("NaN")
          .SetUp("Pond", Color.magenta);
        BoxCollider col = GenericHelper.FindGameObject("2 - Bridge/2 Nonstuff/Start Side/Plane/").gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        break;

      case "Level 2-3":
        WaterBuilder.SetWater("1 - Main Hall/1 Nonstuff/Water/", 3)
          .AddFish("Koi Fish")
          .SetUp("Pond", Color.magenta);
        /*WaterBuilder.SetWater("5 - Final Arena/5 Nonstuff/Water (Controlled)/")*/
        /*  .AddFish("")*/
        /*  .SetUp("", Color.magenta);*/
        break;
      case "Level 3-1":
        WaterBuilder.SetWater("2 - Tallway/2 Nonstuff/Floor/Water/")
          .AddFish("Eyeball")
          .SetUp("Blood", Color.red);
        WaterBuilder.SetWater("7 - Bridge Arena/7 Nonstuff/Water/")
          .AddFish("Eyeball")
          .SetUp("Blood", Color.red);
        // needs work
        WaterBuilder.SetWater("7 - Bridge Arena/7 Nonstuff/Water (1)/")
          .AddFish("Eyeball")
          .SetUp("Blood", Color.red);
        WaterBuilder.SetWater("5 - Circular Arena/5 Nonstuff/Water/")
          .AddFish("Frog (?)")
          .SetUp("Blood", Color.red);
        WaterBuilder.SetWater("3 - Big Arena/3 Nonstuff/Floor/Acid/")
          .AddFish("Melted Fish")
          .SetUp("Acid", Color.green);
        WaterBuilder.SetWater("9 - Uphill Battle/9 Nonstuff/Floor/Acid/Cube")
          .AddFish("Melted Fish")
          .SetUp("Acid", Color.green);
        WaterBuilder.SetWater("9 - Uphill Battle/9 Nonstuff/Floor/Acid/Cube (1)")
          .AddFish("Melted Fish")
          .SetUp("Acid", Color.green);
        WaterBuilder.SetWater("10 - Structure/10 Stuff/AcidRaiser (1)/AcidRaiser/Acid/")
          .AddFish("Melted Fish")
          .SetUp("Acid", Color.green);
        break;
      case "Level 3-2":
        WaterBuilder.SetWater("3 - Other Room/3 Nonstuff/Water/")
          .AddFish("Melted Fish")
          .SetUp("Acid", Color.green);
        break;
        //do something in 4-1 (pool of water beginning, lava later)
        //do something in 4-3 pool of water
        //4-4 chamber of the feline and the rodent
      case "Level 4-2":
        for (int i = 0; i < 7; i++) {
          WaterBuilder.SetWater("Dunes", i)
            .AddFish("Coin")
            .SetUp("Sand", Color.yellow);
        }
        break;
            case "Level 4-1":


                foreach (Transform objects in GenericHelper.FindGameObject("6 - Staircase Arena/6 Nonstuff/Pit/Lava/").transform)
                {
                    WaterBuilder.SetWater(objects.gameObject)
                      .AddFish("Overcooked Fish")
                      .SetUp("Lava", Color.red);
                }
                foreach (Transform objects in GenericHelper.FindGameObject("Exterior Areas/1 Nonstuff/Water/").transform)
                {
                    WaterBuilder.SetWater(objects.gameObject)
                      .AddFish("Ancient Fish")
                      .SetUp("Pond", Color.cyan);
                }
                break;

            case "Level 4-3":
        WaterBuilder.SetWater("3 - Traitor Hallway/3B - Tomb of Kings/3B Nonstuff/Entrance/Walls/Cube (99)")
          .AddFish("Coin")
          .SetUp("Gold", Color.yellow);
        WaterBuilder.SetWater("3 - Traitor Hallway/3B - Tomb of Kings/3B Nonstuff/Entrance/Walls/Cube (100)")
          .AddFish("Coin")
          .SetUp("Gold", Color.yellow);

                foreach (Transform objects in GenericHelper.FindGameObject("5 - Cerberus Room/5 Nonstuff/Water/").transform)
                {
                    WaterBuilder.SetWater(objects.gameObject)
                      .AddFish("Ancient Fish")
                      .SetUp("Pond", Color.cyan);
                }
                break;
      case "Level 4-4":
        WaterBuilder.CreateWater("8 - Outro/8 Stuff/Landing (Broken) (1)")
          .SetPosition(1065, 255, 692)
          .SetLocalScale(9, 0, 9)
          .AddFish("Eyeball")
          .SetUp("\"V2\"", Color.red);
        WaterBuilder.SetWater("8 - Outro/8 Nonstuff/Untilted (Outro)/Cube(Clone) (1)/")
          .AddFish("Coin")
          .SetUp("Sand", Color.yellow);
                foreach (Transform objects in GenericHelper.FindGameObject("3 - Ground Floor/Hallway/Sands/").transform)
                {
                    WaterBuilder.SetWater(objects.gameObject)
                  .AddFish("Coin")
                    .SetUp("Sand", Color.yellow);
                }
                WaterBuilder.SetWater("3 - Ground Floor/Sand Hall/Sand/")
                        .AddFish("Coin")
                .SetUp("Sand", Color.yellow);

                WaterBuilder.SetWater(GenericHelper.FindGameObject("5 - Window Hallway/Floor/").transform.GetChild(5).gameObject)
              .AddFish("Ancient Fish")
              .SetUp("Pond", Color.cyan);
                WaterBuilder.SetWater(GenericHelper.FindGameObject("5 - Window Hallway/Floor/").transform.GetChild(6).gameObject)
              .AddFish("Ancient Fish")
              .SetUp("Pond", Color.cyan);
                WaterBuilder.SetWater(GenericHelper.FindGameObject("3 - Ground Floor/Secret Hall/SuperSecretActivator/"))
            .AddFish("Ancient Fish")
            .SetUp("Pond", Color.cyan);
                break;
      case "Level 5-1":
        WaterBuilder.SetWater("Underwaters/All Waters/Cube (3)")
          .AddFish("Funny Stupid Fish (Friend)")
          .AddFish("PITR Fish")
          .SetUp("Cave Lake", Color.cyan);
        WaterBuilder.SetWater("IntroParent/Intro/Intro A - First Cave/Plane/Cube")
          .AddFish("Chomper")
          .SetUp("Cave Pool", Color.gray);
        WaterBuilder.SetWater("IntroParent/Intro/Intro A - First Cave/Plane (1)/Cube")
          .AddFish("Chomper")
          .SetUp("Cave Pool", Color.gray);
        WaterBuilder.SetWater("IntroParent/Intro/Intro C - Second Cave/Plane (2)/Cube")
          .AddFish("Chomper")
          .SetUp("Cave Pool", Color.gray);
        WaterBuilder.SetWater("2B - Arena B/B Nonstuff/Water/Cube")
          .AddFish("Chomper")
          .SetUp("Cave Pool", Color.gray);
        WaterBuilder.SetWater("1 - Main Cave/1 Nonstuff/Drained/Cube")
          .AddFish("Dope Fish")
          .SetUp("Cave Lake", Color.cyan);
        WaterBuilder.SetWater("2A - Arena A/A Nonstuff/Drained (1)/Cube")
          .AddFish("PITR Fish")
          .AddFish("Funny Stupid Fish (Friend)")
          .SetUp("Cave Lake", Color.cyan);
        break;
      case "Level 5-2":
        WaterBuilder.SetWater("Sea/Sea Itself/Filler/WaterTrigger")
          .AddFish("Nerd Shark", 0)
          .AddBait("3 - Ferryman's Cabin/3 Nonstuff/Interior/Book with Stand/Book", "Nerd Shark")
          .SetUp("The Ocean Styx", Color.blue);
        break;
      case "Level 5-4":
        WaterBuilder.SetWater("Surface/Stuff/Watersurface/Cube")
          .AddFish("Eel (?)")
          .SetUp("The Ocean Styx", Color.blue);
        WaterBuilder.SetWater("Surface/Stuff/Watersurface/Cube (1)")
          .AddFish("Eel (?)")
          .SetUp("The Ocean Styx", Color.blue);
        WaterBuilder.SetWater("Surface/Stuff/Watersurface/Cube (2)")
          .AddFish("Eel (?)")
          .SetUp("The Ocean Styx", Color.blue);
        WaterBuilder.SetWater("Surface/Stuff/Watersurface (Sunken)/NewDeath/Anti-diver Colliders/Cube")
          .AddFish("Eel (?)")
          .SetUp("The Ocean Styx", Color.blue);
        for (int i = 0; i < 8; i++) {
          WaterBuilder.SetWater("Surface/Stuff/Watersurface (Sunken)/NewWater/", i)
            .AddFish("Eel (?)")
            .SetUp("The Ocean Styx", Color.blue);
        }
        break;
      case "Level 6-1":
        WaterBuilder.SetWater("Interiors/6 - Lava Chasm/6 Nonstuff/Lava")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("10 - Chapel/10 Nonstuff/Pit")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("14 - Hall of Sacreligious Remains/14 Nonstuff/Lava Rim/Lava/Cube (9)")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("14 - Hall of Sacreligious Remains/14 Nonstuff/Lava Rim/Lava/Cube (9)/Cube (7)")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("14 - Hall of Sacreligious Remains/14 Nonstuff/Lava Rim/Lava/Cube (6)")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("14 - Hall of Sacreligious Remains/14 Nonstuff/Lava Rim/Lava/Cube (6)/Cube (7)")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        break;
      case "Level 7-2":
        //optionally, Outdoors/12 - Red Skull Trench/12 Nonstuff/Water
        WaterBuilder.SetWater("Outdoors/Decorations/Ground/Blood")
          .AddFish("Bomb Fish")
          .SetUp("The River Phlegethon", Color.black);
        break;
      case "Level 7-4":
        WaterBuilder.SetWater("Main/Interior/InteriorStuff/BoilingBlood")
          .AddFish("Melted Fish")
          .SetUp("Earthmover Insides", Color.black);
        WaterBuilder.SetWater("Main/Interior/InteriorStuff/BoilingBlood (Return)")
          .AddFish("Melted Fish")
          .SetUp("Earthmover Insides", Color.black);
        break;
      case "Level 7-S":
        WaterBuilder.SetWater("Pond/Pond Underwater")
          .AddFish("Koi Fish")
          .SetUp("Pond", Color.white);
        WaterBuilder.SetWater("Pit/PitDestroyer")
          .AddFish("Wise Fish")
          .SetUp("Depths Of The Library", Color.gray);
        WaterBuilder.SetWater("Curved Pit Destroyer")
          .AddFish("Wise Fish")
          .SetUp("Depths Of The Library", Color.gray);
        WaterBuilder.SetWater("Curved Pit Destroyer/GameObject")
          .AddFish("Wise Fish")
          .SetUp("Depths Of The Library", Color.gray);
        WaterBuilder.SetWater("7-S_Unpaintable/Exterior/The Water Ups_Todo/The Water Ups/Water Ups Ocean")
          .AddFish("\"size 2\"", GlobalFishManager.CanCatchSize2())
          .AddMeshCollider(false)
          .SetUp("The Water Ups", Color.blue);
        break;
      case "Level P-2":
        WaterBuilder.SetWater("Shortcut/Deathzones/Deathzone")
          .AddFish("Metal(?) Fish")
          .SetUp("Scrindonguloded Souls", Color.black);
        WaterBuilder.SetWater("Shortcut/Deathzones", 2)
          .AddFish("Metal(?) Fish")
          .SetUp("Scrindonguloded Souls", Color.black);
        WaterBuilder.SetWater("Main Section/Outside/2 - Bridge Street/Floor/Plane (2)/Plane")
          .AddFish("Metal(?) Fish")
          .SetUp("Scrongled Souls", Color.black);
        WaterBuilder.SetWater("Main Section/Outside/2 - Bridge Street/Floor/Plane (3)/Plane (1)")
          .AddFish("Metal(?) Fish")
          .SetUp("Scrongled Souls", Color.black);
        WaterBuilder.SetWater("Main Section/Inside/6 - Soul Tunnel/6 Nonstuff (1)/Soulwalls/Cube(Clone)")
          .AddFish("Metal(?) Fish")
          .SetUp("Damned Souls", Color.black);
        WaterBuilder.SetWater("Main Section/Inside/6 - Soul Tunnel/6 Nonstuff (1)/Soulwalls", 2)
          .AddFish("Metal(?) Fish")
          .SetUp("Damned Souls", Color.black);
        WaterBuilder.SetWater("Main Section/Inside/6 - Soul Tunnel/6 Nonstuff/Soulwalls/Cube(Clone)")
          .AddFish("Metal(?) Fish")
          .SetUp("Damned Souls", Color.black);
        WaterBuilder.SetWater("Main Section/Inside/6 - Soul Tunnel/6 Nonstuff/Soulwalls", 2)
          .AddFish("Metal(?) Fish")
          .SetUp("Damned Souls", Color.black);
        break;
      case "Level 0-E":
        WaterBuilder.SetWater("6 - Crossroads/6 Nonstuff/6 Hot Only/Blood")
          .AddFish("Filthy Screaming Fish (Filsh)")
          .SetUp("Meat", Color.red);
        WaterBuilder.SetWater("8 - Lava Foundry/8 Hot Only/Lava (1)/Cube")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("5-6 Water")
          .AddFish("Frozen Fish")
          .SetUp("Freezing Water", Color.white);
        break;
      case "Level 1-E":
        WaterBuilder.SetWater("2 - Skull Field % Blue Skull Room/2 Nonstuff/Return Trip Nonstuff/Lava/")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("1 - First Field % Skylight Hallway/1 Nonstuff/1 Lava/Cube")
          .AddFish("Overcooked Fish")
          .SetUp("Lava", Color.red);
        WaterBuilder.SetWater("4 - Bridge/4 Nonstuff/4 Unburned/Plane/")
          .AddFish("NaN")
          .SetUp("Glitchy Pond", Color.magenta);

        BoxCollider coll = GenericHelper.FindGameObject("4 - Bridge/4 Nonstuff/4 Unburned/Plane/").gameObject.AddComponent<BoxCollider>();
        coll.isTrigger = true;
        break;
    }
  }
}


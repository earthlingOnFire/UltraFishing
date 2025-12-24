using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using System.Linq;

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
  public static readonly string[] NoRodLevels = [ "Level 1-S", "Level 2-S", "Level 0-S", "Level 4-S" ];

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

      RuntimeAnimatorController animControl = bundle.LoadAsset<RuntimeAnimatorController>("assets/bundles/fishingstuff/CustomFishing Rod Animator.controller");
      fishingRod.GetComponent<FishingRodWeapon>().animator.runtimeAnimatorController = animControl;

      terminal = bundle.LoadAsset<GameObject>("assets/bundles/fishingstuff/fishing enc terminal.prefab");

      string[] splashes = {
        "assets/bundles/fishingstuff/splashes/NoSplash.prefab",
        "assets/bundles/fishingstuff/splashes/Electricity_FishingBaitSplash1.prefab",
        "assets/bundles/fishingstuff/splashes/Sand_FishingBaitSplash.prefab",
        "assets/bundles/fishingstuff/splashes/book/Book_FishingBaitSplash.prefab",
      };
      WaterBuilder.customSplashes.Add("None", bundle.LoadAsset<GameObject>(splashes[0]));
      WaterBuilder.customSplashes.Add("Electricity", bundle.LoadAsset<GameObject>(splashes[1]));
      WaterBuilder.customSplashes.Add("Sand", bundle.LoadAsset<GameObject>(splashes[2]));
      WaterBuilder.customSplashes.Add("Books", bundle.LoadAsset<GameObject>(splashes[3]));
    }
  }
}


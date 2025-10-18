using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using System;

namespace UltraFishing;

public static class GlobalFishManager {
  private static FishObject[] fishes;
  private static bool[] foundFishes;

  public static void Start() {
    string[] defaultFishes = {
      "Assets/Data/Fishing/Fishes/Funny Stupid Fish.asset", //Funny Stupid Fish (Friend)
      "Assets/Data/Fishing/Fishes/pitr fish.asset", //PITR Fish
      "Assets/Data/Fishing/Fishes/Trout.asset", //Trout
      "Assets/Data/Fishing/Fishes/Amid Evil Fish.asset", //Metal Fish
      "Assets/Data/Fishing/Fishes/Chomper.asset", //Chomper
      "Assets/Data/Fishing/Fishes/Bomb Fish.asset", //Bomb Fish
      "Assets/Data/Fishing/Fishes/Gib Eye.asset", //Eyeball
      "Assets/Data/Fishing/Fishes/Iron Lung Fish.asset", //Frog (?)
      "Assets/Data/Fishing/Fishes/Dope Fish.asset", //Dope Fish
      "Assets/Data/Fishing/Fishes/Stickfish.asset", //Stickfish
      "Assets/Data/Fishing/Fishes/Cooked Fish.asset", //Cooked Fish
      "Assets/Data/Fishing/Fishes/Shark.asset", //Shark
    };
    string[] customFishes = {
      "assets/bundles/fishingstuff/fishes/filth fish.asset", // Filthy Screaming Fish (Filsh)
      "assets/bundles/fishingstuff/fishes/missing fish.asset", // null
      "assets/bundles/fishingstuff/fishes/cancer fish.asset", // Cancerous Fish
      "assets/bundles/fishingstuff/fishes/koi fish.asset", // Koi Fish
      "assets/bundles/fishingstuff/fishes/melted fish.asset", // Melted Fish
      "assets/bundles/fishingstuff/fishes/nerd shark.asset", // Nerd Shark
      "assets/bundles/fishingstuff/fishes/leviathan fish.asset", // Eel (?)
      "assets/bundles/fishingstuff/fishes/death metal fish.asset", // Metal(?) Fish
      "assets/bundles/fishingstuff/fishes/overcooked fish.asset", // Overcooked Fish
      "assets/bundles/fishingstuff/fishes/frozen fish.asset", // Frozen Fish
      "assets/bundles/fishingstuff/fishes/coin fish.asset", // Coin
      "assets/bundles/fishingstuff/fishes/book fish.asset", // Wise Fish
      "assets/bundles/fishingstuff/fishes/png fish.asset", // "size 2"
      "assets/bundles/fishingstuff/fishes/mannequin fish.asset", // Mannequin Fish
      "assets/bundles/fishingstuff/fishes/nil fish.asset", // Nil
      "assets/bundles/fishingstuff/fishes/nan fish.asset", // NaN
      "assets/bundles/fishingstuff/fishes/ancient fish.asset", // Ancient Fish
      "assets/bundles/fishingstuff/fishes/tasty fish.asset", // Tasty Fish
      "assets/bundles/fishingstuff/fishes/wine fish.asset", // Wine Fish
      "assets/bundles/fishingstuff/fishes/vapor fish.asset", // Vapor Fish
      "assets/bundles/fishingstuff/fishes/wire shark.asset", // Wire Shark
    };

    fishes = new FishObject[defaultFishes.Length + customFishes.Length];
    foundFishes = new bool[fishes.Length];
    for (int i = 0; i < defaultFishes.Length; i++) {
      fishes[i] = Addressables.LoadAssetAsync<FishObject>(defaultFishes[i]).WaitForCompletion();
      foundFishes[i] = false;
    }
    for (int i = 0; i < customFishes.Length; i++) {
      FishObject fish = Plugin.bundle.LoadAsset<FishObject>(customFishes[i]);
      fishes[i + defaultFishes.Length] = PrepareFish(fish);
      foundFishes[i + defaultFishes.Length] = false;
    }
    

    string savePath = Path.Combine(Plugin.modDir, "fish.save");
    if (File.Exists(savePath)) {
      byte[] saveData = File.ReadAllBytes(savePath);
      for (int i = 0; i < saveData.Length; i++) {
        if (saveData[i] == 1) {
          foundFishes[i] = true;
        }
      }
    }
  }

  private static FishObject PrepareFish(FishObject fish) {
    switch (fish.fishName) {
      case "Wise Fish":
        fish.customPickup.gameObject.AddComponent<BookRandomizer>();
        break;
      case "Wine Fish":
        fish.worldObject.transform.Find("Liquid").gameObject.AddComponent<Liquid>();
        break;
    }
    return fish;
  }

  public static void UnlockFish(FishObject fish) {
    int fishIndex = Array.FindIndex(fishes, f => f == fish);
    if (fishIndex == -1) {
      Plugin.logger.LogError($"Fish {fish.fishName} could not be found!");
      return;
    }
    Plugin.logger.LogInfo($"Fish {fish.fishName} was found!");
    if (foundFishes[fishIndex] != true) {
      foundFishes[fishIndex] = true;
      WriteToSaveFile();
      UpdateSize2();
    }
  }

  public static int FishCount() {
    return fishes.Length;
  }

  public static FishObject GetFish(int index) {
    return fishes[index];
  }

  public static FishObject GetFish(string fishName) {
    return Array.Find(fishes, fish => fish.fishName == fishName);
  }

  public static bool GetFishValue(int index) {
    return foundFishes[index];
  }

  public static bool FoundFish(FishObject fish) {
    int fishIndex = Array.FindIndex(fishes, f => f == fish);
    if (fishIndex == -1) return false;

    return GetFishValue(fishIndex);
  }

  public static string GetFishDescription(FishObject fish) {
    if (FoundFish(fish)) {
      return fish.description;
    }

    switch (fish.fishName) {
      case "null":
        return """"
An ordinary and very real fish. Native to the beautiful paradise known as Limbo.

Usually found where the water falls.
"""";
      case "\"size 2\"":
        return """"
The legendary fish. The dream of all fishers, yet none have ever caught it. It is said to only appear to expert fishers who have caught every kind of fish.

The waterfall conceals the water UPS. Agnes Gorge Trail. Use your ability and fulfill your destiny.
"""";
      default:
        return fish.description;
    }
  }

  private static void WriteToSaveFile() {
    string savePath = Path.Combine(Plugin.modDir, "fish.save");
    byte[] saveData = new byte[foundFishes.Length];

    for (int i = 0; i < foundFishes.Length; i++) {
      if (foundFishes[i] == true) {
        saveData[i] = 1;
      }
      else {
        saveData[i] = 0;
      }
    }

    File.WriteAllBytes(savePath, saveData);
  }

  public static int CanCatchSize2() {
    for (int i = 0; i < foundFishes.Length - 1; i++) {
      if (foundFishes[i] != true) return 0;
    }
    return 1;
  }

  public static void UpdateSize2() {
    if (SceneHelper.CurrentScene == "Level 7-S" && CanCatchSize2() == 1) {
      string path = "7-S_Unpaintable/Exterior/The Water Ups_Todo/The Water Ups/Water Ups Ocean";
      GameObject waterUpsOcean = GenericHelper.FindGameObject(path);
      if (waterUpsOcean == null) return;
      FakeWater fakeWater = waterUpsOcean.GetComponent<FakeWater>();
      if (fakeWater == null) return;
      FishDescriptor[] foundFishes = fakeWater.fishDB.foundFishes;
      foundFishes[0].chance = 1;
    }
  }
}

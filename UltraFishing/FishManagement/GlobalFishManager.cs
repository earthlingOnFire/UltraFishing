using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.IO;

namespace UltraFishing;

public static class GlobalFishManager {

  private static Dictionary<string, FishData> fishes = new Dictionary<string, FishData>();
  private static List<FishCollection> collections = new List<FishCollection>();

  public static void RegisterCollection(FishCollection collection) {
    collections.Add(collection);
    foreach (FishData fishData in collection.fishes) {
      fishes.Add(fishData.fish.fishName, fishData);
    }
  }

  public static void Start() {
    string savePath = Path.Combine(Plugin.modDir, "fish.save");

    (string, int)[] defaultFishes = {
      ("Assets/Data/Fishing/Fishes/Funny Stupid Fish.asset", 0), // Funny Stupid Fish (Friend)
      ("Assets/Data/Fishing/Fishes/pitr fish.asset", 1), // PITR Fish
      ("Assets/Data/Fishing/Fishes/Trout.asset", 2), // Trout
      ("Assets/Data/Fishing/Fishes/Amid Evil Fish.asset", 3), // Metal Fish
      ("Assets/Data/Fishing/Fishes/Chomper.asset", 4), // Chomper
      ("Assets/Data/Fishing/Fishes/Bomb Fish.asset", 5), // Bomb Fish
      ("Assets/Data/Fishing/Fishes/Gib Eye.asset", 6), // Eyeball
      ("Assets/Data/Fishing/Fishes/Iron Lung Fish.asset", 7), // Frog (?)
      ("Assets/Data/Fishing/Fishes/Dope Fish.asset", 8), // Dope Fish
      ("Assets/Data/Fishing/Fishes/Stickfish.asset", 9), // Stickfish
      ("Assets/Data/Fishing/Fishes/Cooked Fish.asset", 10), // Cooked Fish
      ("Assets/Data/Fishing/Fishes/Shark.asset", 11), // Shark
    };

    (string, int)[] customFishes = {
      ("assets/bundles/fishingstuff/fishes/filth fish.asset", 12), // Filthy Screaming Fish (Filsh)
      ("assets/bundles/fishingstuff/fishes/sword fish.asset", 25), // Scraphead Fish
      ("assets/bundles/fishingstuff/fishes/wire shark.asset", 26), // Wire Shark
      ("assets/bundles/fishingstuff/fishes/overcooked fish.asset", 20), // Overcooked Fish
      ("assets/bundles/fishingstuff/fishes/missing fish.asset", 13), // null
      ("assets/bundles/fishingstuff/fishes/nil fish.asset", 27), // Nil
      ("assets/bundles/fishingstuff/fishes/nan fish.asset", 28), // NaN
      ("assets/bundles/fishingstuff/fishes/coin fish.asset", 22), // Coin
      ("assets/bundles/fishingstuff/fishes/cancer fish.asset", 14), // Cancerous Fish
      ("assets/bundles/fishingstuff/fishes/flying demon fish.asset", 29), // Flying Demon Fish
      ("assets/bundles/fishingstuff/fishes/vapor fish.asset", 30), // Vapor Fish
      ("assets/bundles/fishingstuff/fishes/plastic fish.asset", 31), // Plastic Fish
      ("assets/bundles/fishingstuff/fishes/koi fish.asset", 15), // Koi Fish
      ("assets/bundles/fishingstuff/fishes/melted fish.asset", 16), // Melted Fish
      ("assets/bundles/fishingstuff/fishes/ancient fish.asset", 32), // Ancient Fish
      ("assets/bundles/fishingstuff/fishes/nerd shark.asset", 17), // Nerd Shark
      ("assets/bundles/fishingstuff/fishes/wine fish.asset", 33), // Poisson de Vin
      ("assets/bundles/fishingstuff/fishes/leviathan fish.asset", 18), // Eel (?)
      ("assets/bundles/fishingstuff/fishes/mannequin fish.asset", 34), // Mannequin Fish
      ("assets/bundles/fishingstuff/fishes/tasty fish.asset", 35), // Tasty Fish
      ("assets/bundles/fishingstuff/fishes/book fish.asset", 23), // Wise Fish
      ("assets/bundles/fishingstuff/fishes/frozen fish.asset", 21), // Frozen Fish
      ("assets/bundles/fishingstuff/fishes/death metal fish.asset", 19), // Metal(?) Fish
      ("assets/bundles/fishingstuff/fishes/prime fish.asset", 36), // Prime Fish
    };

    string size2 = "assets/bundles/fishingstuff/fishes/png fish.asset"; // "size 2"
    int size2SaveSlot = 24;

    FishCollection defaultCollection = new FishCollection("ULTRAKILL");
    for (int i = 0; i < defaultFishes.Length; i++) {
      FishObject fish = Addressables.LoadAssetAsync<FishObject>(defaultFishes[i].Item1).WaitForCompletion();
      int saveSlot = defaultFishes[i].Item2;
      defaultCollection.RegisterFish(fish, savePath, saveSlot);
    }

    FishCollection ultrafishingCollection = new FishCollection("ULTRAFISHING");
    for (int i = 0; i < customFishes.Length; i++) {
      FishObject fish = Plugin.bundle.LoadAsset<FishObject>(customFishes[i].Item1);
      int saveSlot = customFishes[i].Item2;
      ultrafishingCollection.RegisterFish(PrepareFish(fish), savePath, saveSlot);
    }

    FishCollection size2Collection = new FishCollection("???");
    FishObject size2Fish = Plugin.bundle.LoadAsset<FishObject>(size2);
    size2Collection.RegisterFish(size2Fish, savePath, size2SaveSlot);

    RegisterCollection(defaultCollection);
    RegisterCollection(ultrafishingCollection);
    RegisterCollection(size2Collection);
  }

  private static FishObject PrepareFish(FishObject fish) {
    switch (fish.fishName) {
      case "Wise Fish":
        fish.customPickup.gameObject.AddComponent<BookRandomizer>();
        break;
      case "Poisson de Vin":
        fish.worldObject.transform.Find("Liquid").gameObject.AddComponent<Liquid>();
        break;
      case "NaN":
        MaterialSwapper matSwap = fish.worldObject.transform.GetChild(1).gameObject.AddComponent<MaterialSwapper>();
        matSwap.mat = Plugin.bundle.LoadAsset<Material>("Assets/Bundles/fishingstuff/Skyboxes/FakeOldScreenField.mat");
        matSwap.layer = 28;
        matSwap.ignoreLevels = new List<string>(new string[]{
            "Level 1-1", "Level 1-2", "Level 1-3", "Level 1-4", "Level 1-E"
        });
        break;
      case "Prime Fish":
        MaterialSwapper matSwap1 = fish.worldObject.transform.GetChild(0).gameObject.AddComponent<MaterialSwapper>();
        matSwap1.mat = Plugin.bundle.LoadAsset<Material>("Assets/Bundles/fishingstuff/MinosPrimeBody.mat");
        matSwap1.layer = -1;
        matSwap1.ignoreLevels = new List<string>(new string[]{"Level P-2"});
        break;
    }
    return fish;
  }

  public static void UnlockFish(FishObject fish) {
    if (!fishes.ContainsKey(fish.fishName)) {
      Plugin.logger.LogError($"Fish {fish.fishName} could not be found!");
      return;
    }

    Plugin.logger.LogInfo($"Fish {fish.fishName} was found!");
    FishData fishData = fishes[fish.fishName];

    fishData.Unlock();
    UpdateSize2();
  }

  public static FishObject GetFish(string fishName) {
    return fishes[fishName].fish;
  }

  public static bool FoundFish(FishObject fish) {
    return fishes[fish.fishName].found;
  }

  public static FishCollection[] GetFishCollections() {
    return collections.ToArray();
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
      case "NaN":
        return """"
A local delicacy, enjoyed by the happy residents of the peaceful Limbo layer.

Usually found where the water pools.
"""";
      case "Nil":
        return """"
A very normal fish. You could find similar fish to this one anywhere. Resides in the idyllic and luxurious Limbo layer.

Usually found where the water flows.
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

  public static int Size2Chance() {
    if (collections[0].FoundAll() && collections[1].FoundAll()) return 1;
    else return 0;
  }

  public static void UpdateSize2() {
    if (SceneHelper.CurrentScene == "Level 7-S" && Size2Chance() == 1) {
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

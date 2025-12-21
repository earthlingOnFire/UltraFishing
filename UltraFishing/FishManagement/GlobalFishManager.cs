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
    string[] customFishesPage1 = {
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
    };

    string[] customFishesPage2 = {
      "assets/bundles/fishingstuff/fishes/wire shark.asset", // Wire Shark
      "assets/bundles/fishingstuff/fishes/nil fish.asset", // Nil
      "assets/bundles/fishingstuff/fishes/nan fish.asset", // NaN
      "assets/bundles/fishingstuff/fishes/vapor fish.asset", // Vapor Fish
      "assets/bundles/fishingstuff/fishes/ancient fish.asset", // Ancient Fish
      "assets/bundles/fishingstuff/fishes/wine fish.asset", // Wine Fish
      "assets/bundles/fishingstuff/fishes/mannequin fish.asset", // Mannequin Fish
      "assets/bundles/fishingstuff/fishes/tasty fish.asset", // Tasty Fish
      "assets/bundles/fishingstuff/fishes/flying demon fish.asset", // Flying Demon Fish
      "assets/bundles/fishingstuff/fishes/plastic fish.asset", // Plastic Fish
      "assets/bundles/fishingstuff/fishes/prime fish.asset", // Prime Fish
      "assets/bundles/fishingstuff/fishes/sword fish.asset", // Scraphead Fish
    };

    string size2 = "assets/bundles/fishingstuff/fishes/png fish.asset"; // "size 2"

    FishCollection defaultCollection = new FishCollection("ULTRAKILL");
    for (int i = 0; i < defaultFishes.Length; i++) {
      FishObject fish = Addressables.LoadAssetAsync<FishObject>(defaultFishes[i]).WaitForCompletion();
      defaultCollection.RegisterFish(fish, savePath, i);
    }

    FishCollection ultrafishingCollection = new FishCollection("ULTRAFISHING");
    for (int i = 0; i < customFishesPage1.Length; i++) {
      FishObject fish = Plugin.bundle.LoadAsset<FishObject>(customFishesPage1[i]);
      int saveSlot = i + defaultFishes.Length;
      ultrafishingCollection.RegisterFish(PrepareFish(fish), savePath, saveSlot);
    }

    for (int i = 0; i < customFishesPage2.Length; i++) {
      FishObject fish = Plugin.bundle.LoadAsset<FishObject>(customFishesPage2[i]);
      int saveSlot = i + defaultFishes.Length + customFishesPage1.Length + 1;
      ultrafishingCollection.RegisterFish(PrepareFish(fish), savePath, saveSlot);
    }

    FishCollection size2Collection = new FishCollection("???");
    FishObject size2Fish = Plugin.bundle.LoadAsset<FishObject>(size2);
    int size2SaveSlot = defaultFishes.Length + customFishesPage1.Length;
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
      case "Wine Fish":
        fish.worldObject.transform.Find("Liquid").gameObject.AddComponent<Liquid>();
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

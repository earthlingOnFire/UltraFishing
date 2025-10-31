using UnityEngine;

namespace UltraFishing;

public static class FishHelper {

  public static FishObject GetFish(string fishName) {
    return GlobalFishManager.GetFish(fishName);
  }

  private static FishDB GetFishDB(FishDescriptor[] fishes) {
    FishDB fishDB = ScriptableObject.CreateInstance<FishDB>();
    fishDB.foundFishes = fishes;

    return fishDB;
  }

  private static FishDB GetFishDB(string[] fishes, int[] chances) {
    FishDescriptor[] foundFishes = new FishDescriptor[fishes.Length];
    
    for (int i = 0; i < fishes.Length; i++) {
      foundFishes[i] = new FishDescriptor();
      foundFishes[i].fish = GetFish(fishes[i]);
      foundFishes[i].chance = chances[i];
    }

    return GetFishDB(foundFishes);
  }

  private static FishDB GetFishDB(string[] fishes) {
    int[] chances = new int[fishes.Length];
    for (int i = 0; i < chances.Length; i++) {
      chances[i] = 1;
    }

    return GetFishDB(fishes, chances);
  }

  public static FishDB GetFishDB(string fish, int chance = 1) {
    return GetFishDB(new string[]{fish}, new int[]{chance});
  }

  public static FishDB AddFishToDB(FishDB fishDB, string fish, int chance = 1) {
    FishDescriptor newFish = new FishDescriptor();
    newFish.fish = GetFish(fish);
    newFish.chance = chance;
    
    FishDescriptor[] fishes = GenericHelper.AppendToArray(fishDB.foundFishes, newFish);

    FishDB newFishDB = GetFishDB(fishes);
    newFishDB.fullName = fishDB.fullName;
    newFishDB.symbolColor = fishDB.symbolColor;

    return newFishDB;
  }

  public static void UpdateFishManager(FishDB fishDB) {
    FishManager.Instance.fishDbs = GenericHelper.AppendToArray(FishManager.Instance.fishDbs, fishDB);

    for (int i = 0; i < fishDB.foundFishes.Length; i++) {
      FishObject fish = fishDB.foundFishes[i].fish;
      if (!FishManager.Instance.recognizedFishes.ContainsKey(fish)) {
        FishManager.Instance.recognizedFishes.Add(fish, value: false);
      }
    }
  }

}

using System.Collections.Generic;

namespace UltraFishing;

public class FishCollection {

  public string name;

  public List<FishData> fishes;

  public FishCollection(string name) {
    this.name = name;
    this.fishes = new List<FishData>();
  }

  public void RegisterFish(FishObject fish, string savePath, int saveSlot) {
    FishData fishData = new FishData(fish, savePath, saveSlot);
    fishes.Add(fishData);
  }

  public bool FoundAll() {
    foreach (FishData fish in fishes) {
      if (!fish.found) return false;
    }

    return true;
  }
}

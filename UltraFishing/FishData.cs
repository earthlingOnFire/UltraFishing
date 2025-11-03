
namespace UltraFishing;

public class FishData {

  public FishObject fish;
  
  public string savePath;

  public int saveSlot;

  public bool found;

  public FishData(FishObject fish, string savePath, int saveSlot) {
    this.fish = fish;
    this.savePath = savePath;
    this.saveSlot = saveSlot;
    this.found = (SaveHelper.ReadFromSave(savePath, saveSlot) == 1);
  }

  public void Unlock() {
    if (found) return;

    found = true;

    SaveHelper.WriteToSave(savePath, saveSlot, 1);
  }
}

using UnityEngine;
using Object = UnityEngine.Object;

namespace UltraFishing;

public class WaterBuilder {

  private bool isFake;
  private FakeWater fakeWater;
  private Water water;
  private FishDB fishDB;
  private GameObject gameObject;

  public WaterBuilder(Water water) {
    this.gameObject = water.gameObject;
    this.water = water;
    this.isFake = false;
  }

  public WaterBuilder(FakeWater fakeWater) {
    this.gameObject = fakeWater.gameObject;
    this.fakeWater = fakeWater;
    this.isFake = true;
  }

  public static WaterBuilder SetWater(GameObject gameObject) {
    if (gameObject == null) {
      return null;
    }

    var water = gameObject.GetComponent<Water>();

    if (water == null) {
      return new WaterBuilder(gameObject.AddComponent<FakeWater>());
    }

    return new WaterBuilder(water);
  }

  public static WaterBuilder SetWater(string gameObjectPath) {
    GameObject gameObject = GenericHelper.FindGameObject(gameObjectPath);
    if (gameObject == null) {
      Plugin.logger.LogError($"Could not find GameObject at path {gameObjectPath}");
      return null;
    }

    return SetWater(gameObject);
  }

  public static WaterBuilder SetWater(string parentPath, int childIndex) {
    GameObject parent = GenericHelper.FindGameObject(parentPath);
    if (parent == null) {
      Plugin.logger.LogError($"Could not find GameObject at path {parentPath}");
      return null;
    }

    if (childIndex >= parent.transform.childCount) {
      Debug.Log($"GameObject at path {parentPath} only has {parent.transform.childCount} children!");
      return null;
    }

    GameObject child = parent.transform.GetChild(childIndex).gameObject;

    return SetWater(child);
  }

  public static WaterBuilder SetWater(string parentPath, int childIndex, string childPath) {
    GameObject parent = GenericHelper.FindGameObject(parentPath);
    if (parent == null) {
      Plugin.logger.LogError($"Could not find GameObject at path {parentPath}");
      return null;
    }

    if (childIndex >= parent.transform.childCount) {
      Debug.Log($"GameObject at path {parentPath} only has {parent.transform.childCount} children!");
      return null;
    }

    GameObject child = parent.transform.GetChild(childIndex).gameObject;

    GameObject gameObject = child.transform.Find(childPath).gameObject;
    if (gameObject == null) {
      Plugin.logger.LogError($"Could not find GameObject at path {parentPath}, {childIndex}, {childPath}");
      return null;
    }

    return SetWater(gameObject);
  }

  public static WaterBuilder CreateWater() { // creates FAKE water
    GameObject water = new GameObject("fakewater", typeof(FakeWater));
    
    BoxCollider collider = water.AddComponent<UnityEngine.BoxCollider>();
    collider.isTrigger = true;

    return new WaterBuilder(water.GetComponent<FakeWater>());
  }

  public static WaterBuilder CreateWater(string parentPath) { // creates FAKE water
    WaterBuilder waterBuilder = CreateWater();
    GameObject water = waterBuilder.gameObject;

    GameObject parent = GenericHelper.FindGameObject(parentPath);
    if (parent != null) {
      water.transform.SetParent(parent.transform);
      water.transform.localPosition = Vector3.zero;
    }

    return waterBuilder;
  }

  public WaterBuilder SetPosition(float x, float y, float z) {
    gameObject.transform.position = new Vector3(x, y, z);

    return this;
  }

  public WaterBuilder SetLocalScale(float x, float y, float z) {
    gameObject.transform.localScale = new Vector3(x, y, z);

    return this;
  }

  public WaterBuilder AddFish(string fish, int chance = 1) {
    if (fishDB == null) {
      fishDB = FishHelper.GetFishDB(fish, chance);
    } else {
      fishDB = FishHelper.AddFishToDB(fishDB, fish, chance);
    }

    return this;
  }

  public WaterBuilder RaiseFishingPoint(float amount) {
    Transform originalTransform = gameObject.transform;
    Transform newTransform = Object.Instantiate(originalTransform);
    newTransform.localPosition = new Vector3(
        originalTransform.localPosition.x,
        originalTransform.localPosition.y + amount,
        originalTransform.localPosition.z
    );

    if (isFake) {
      fakeWater.overrideFishingPoint = newTransform;
    }
    else {
      water.overrideFishingPoint = newTransform;
    }

    return this;
  }

  public WaterBuilder AddMeshCollider(bool isTrigger = true) {
    if (gameObject != null) {
      MeshCollider col = gameObject.AddComponent<MeshCollider>();
      if (isTrigger) {
        col.convex = true;
        col.isTrigger = true;
      }
    }

    return this;
  }

  public WaterBuilder AddBait(string baitPath, string fish) {
    GameObject bait = GenericHelper.FindGameObject(baitPath);

    FishObject fishObject = FishHelper.GetFish(fish);
    BaitItem baitItem = bait.GetComponent<BaitItem>();

    if (baitItem == null) {
      baitItem = bait.AddComponent<BaitItem>();
    }

    baitItem.consumedPrefab = Plugin.baitConsumedSound;
    baitItem.attractFish = GenericHelper.AppendToArray(baitItem.attractFish, fishObject);
    baitItem.supportedWaters = GenericHelper.AppendToArray(baitItem.supportedWaters, fishDB);

    return this;
  }

  public WaterBuilder SetName(string name) {
    if (fishDB == null) fishDB = FishHelper.GetFishDB(null);
    fishDB.fullName = name;
    
    return this;
  }

  public WaterBuilder SetColor(Color color) {
    if (fishDB == null) fishDB = FishHelper.GetFishDB(null);
    fishDB.symbolColor = color;
    
    return this;
  }

  public void SetUp() {
    if (isFake) {
      fakeWater.fishDB = fishDB;
      fakeWater.SetupFishDB(fishDB);
    } else {
      water.fishDB = fishDB;
      fishDB.SetupWater(water);
    }

    FishHelper.UpdateFishManager(fishDB);
  }

  public void SetUp(string name, Color color) {
    fishDB.fullName = name;
    fishDB.symbolColor = color;

    SetUp();
  }

}

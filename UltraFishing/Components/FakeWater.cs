using UnityEngine;

namespace UltraFishing;

public class FakeWater : MonoBehaviour {

	public FishDB fishDB;

	public Transform overrideFishingPoint;

	public FishObject[] attractFish;

  public void SetupFishDB(FishDB fishDB) {
		if (fishDB.fishGhostPrefab != null) {
			Bounds bounds = this.GetComponent<Collider>().bounds;
			int num = (int)(bounds.size.x * bounds.size.y / 100f);
			for (int i = 0; i < num; i++) {
				GameObject gameObject = Object.Instantiate(fishDB.fishGhostPrefab, this.transform, worldPositionStays: true);
				gameObject.transform.position = new Vector3(Random.Range((0f - bounds.size.x) / 4f, bounds.size.x / 4f) + bounds.center.x, 0f, Random.Range((0f - bounds.size.z) / 4f, bounds.size.z / 4f) + bounds.center.z);
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, bounds.center.y + Random.Range(-1f, 1f) * (bounds.size.y / 2f - 0.2f), gameObject.transform.position.z);
				gameObject.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
			}
		}
  }
}

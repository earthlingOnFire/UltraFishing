using UnityEngine;

namespace UltraFishing;

public class BookRandomizer : MonoBehaviour {
  private Readable readable;

  private void Start() {
    readable = this.gameObject.GetComponent<Readable>();
    RandomizeText();
  }

  public void RandomizeText() {
    readable.content = RandomBookTextProvider.GetRandomText();
  }

  public void GetNextText() {
    readable.content = RandomBookTextProvider.GetNextText();
    readable.StartScan();
  }
}

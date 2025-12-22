using UnityEngine;

//made by Gabriel Aguiar Prod.

public class TerninalFix : MonoBehaviour
{
    Renderer rend;
    public Material mat;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void OnEnable()
    {
        if (gameObject.layer == 8)
        {
            rend.material = mat;
        }
    }
}

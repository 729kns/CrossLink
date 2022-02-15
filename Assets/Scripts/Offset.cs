using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Offset : MonoBehaviour
{
    public Shader shader;
    public Vector2 speed;
    private Image si;
    private Vector2 offset;
    // Start is called before the first frame update
    void Awake()
    {
        si = GetComponent<Image>();
        si.material = new Material(shader);
        si.material.SetTextureOffset("_MainTex", Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        offset += speed;
        si.material.SetTextureOffset("_MainTex", offset);
    }
}

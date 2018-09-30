
using UnityEngine;
using UnityEngine.UI;

public class HueShifter : MonoBehaviour
{
    public float Speed = 1;
    private Renderer[] rend;
    public Image image;
    public bool isUI;

    void Start()
    {
        if(!isUI)
        {
            rend = GetComponentsInChildren<Renderer>();
        }

    }

    void Update()
    {
        if(isUI)
        {
            image.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1)));
        }
        else
        {
            foreach (Renderer r in rend)
            {
                r.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1)));
            }
        }
    }
}

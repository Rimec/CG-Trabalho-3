
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    Ray ray;
    Texture2D tex;
    public float cameraSize;
    public Transform light1;
    [Range(0, 30)]
    public float specular;
    [Range(0, 1)]
    public float ambient;
    public Color lightColor;
    public Color backgroundColor;
    void Start()
    {
        ray = new Ray(transform.position, Vector3.forward);
        Renderer rend = GetComponent<Renderer>();
        tex = new Texture2D(120, 120);
        tex.filterMode = FilterMode.Bilinear;
        rend.material.mainTexture = tex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("RenderScene");
        }
        Debug.DrawRay(ray.origin, ray.direction * 5.0f, Color.red);
    }

    IEnumerator RenderScene()
    {
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float px = ((float)x / tex.width) * cameraSize - cameraSize * .5f;
                float py = ((float)y / tex.height) * cameraSize - cameraSize * .5f;
                ray.origin = new Vector3(px, py, 0);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Color c = BlinnPhong(hit);
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, backgroundColor);
                }
                tex.Apply();
            }
        }
        yield return new WaitForSeconds(0.0001f);
    }

    Color Phong(RaycastHit hit)
    {
        Color hitColor = hit.transform.GetComponent<MeshRenderer>().material.color;
        Vector3 L = (light1.position - hit.point).normalized;
        Vector3 N = hit.normal;
        Vector3 V = (transform.position - hit.point).normalized;
        Vector3 R = Vector3.Reflect(-L, N);
        float diff = Mathf.Max(0, Vector3.Dot(N, L));
        float spec = Mathf.Max(0, Mathf.Pow(Vector2.Dot(V, R), specular * 2 + 1));
        return hitColor * ambient + hitColor * diff + spec * Color.white;
    }

    Color BlinnPhong(RaycastHit hit)
    {
        Color hitColor = hit.transform.GetComponent<MeshRenderer>().material.color;
        Vector3 L = (light1.position - hit.point).normalized;
        Vector3 N = hit.normal;
        Vector3 V = (transform.position - hit.point).normalized;
        Vector3 H = (L + V).normalized;
        float NdotH = Mathf.Max(0, Vector3.Dot(N, H));
        float diff = Mathf.Max(0, Vector3.Dot(N, L));
        float spec = Mathf.Pow(NdotH, (specular * 2 + 1));
        return ambient * hitColor + diff * hitColor + spec * lightColor;
    }
}

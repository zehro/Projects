using UnityEngine;
using System.Collections;

public class Raymarch : MonoBehaviour
{
    public Camera cam;
    Renderer render;
    Collider collide;

    void Awake()
    {
        render = GetComponent<Renderer>();
        collide = GetComponent<Collider>();
        //render.material.SetVector("_Color", new Vector3(Random.Range(0.8f, 1), Random.Range(0.8f, 1), Random.Range(0.8f, 1)));
        render.material.SetVector("_LocalExtents", collide.bounds.extents);
    }

    void LateUpdate()
    {
        render.material.SetMatrix("_CameraMatrix", Matrix4x4.TRS(Vector3.zero, convertRot(cam.transform.rotation, true), Vector3.one));
        render.material.SetMatrix("_LocalMatrix", Matrix4x4.TRS(convertPos(transform.position), convertRot(transform.rotation, false), Vector3.one).inverse);
        render.material.SetFloat("_AspectRatio", ((float)Screen.width) / Screen.height);
    }

    Vector3 convertPos(Vector3 o)
    {
        Vector3 t = Vector3.zero;
        t.x = -o.x;
        t.y = -o.z;
        t.z = +o.y;
        return t;
    }

    Quaternion convertRot(Quaternion o, bool sign)
    {
        Quaternion t = Quaternion.identity;
        t.w = (sign ? +o.w : -o.w);
        t.x = -o.x;
        t.y = -o.z;
        t.z = +o.y;
        return t;
    }
}
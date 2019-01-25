using UnityEngine;
using System.Collections;

public class CorpseCleanUp : MonoBehaviour
{
    float Timer = 5.00f;
    float MaxTimer;
    float CleanUpStartsAt = 0.25f;
    Bounds RenderBounds;
	public void SetTimer(float NewTimer, float WaitPercentage)
    {
        Timer = NewTimer;
        MaxTimer = NewTimer;
        CleanUpStartsAt = 1.00f - WaitPercentage;
    }
    void Start()
    {
        SkinnedMeshRenderer SkinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        if (SkinnedRenderer != null)
            RenderBounds = SkinnedRenderer.bounds;
        else
        {
            MeshRenderer Renderer = GetComponent<MeshRenderer>();
            if (Renderer != null)
                RenderBounds = Renderer.bounds;
        }
    }
	void Update()
    {
        if (CleanUpStartsAt < 0.00f)
            CleanUpStartsAt = 0.00f;
        if (CleanUpStartsAt > 1.00f)
            CleanUpStartsAt = 1.00f;

	    if (Timer > 0.00f)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0.00f)
            {
                Destroy(GetComponent<CorpseCleanUp>());
                Destroy(gameObject);
            }
            else if (Timer <= MaxTimer * CleanUpStartsAt)
            {
                //if (GetComponent<Rigidbody>() != null)
                    //Destroy(GetComponent<Rigidbody>());

                //transform.Translate(Vector3.down * Time.deltaTime * Mathf.Max(RenderBounds.extents.x, RenderBounds.extents.y, RenderBounds.extents.z) * 2
                    /// (MaxTimer * CleanUpStartsAt), Space.World);

                /*Color hostColor = GetComponent<MeshRenderer>().material.color;
                Color newColor = new Color();
                newColor.r = hostColor.r;
                newColor.g = hostColor.g;
                newColor.b = hostColor.b;
                newColor.a = (float)System.Math.Min(1.00, Timer / (MaxTimer * CleanUpStartsAt));
                GetComponent<MeshRenderer>().material.SetColor("_Color", newColor);*/
            }
        }
	}
}

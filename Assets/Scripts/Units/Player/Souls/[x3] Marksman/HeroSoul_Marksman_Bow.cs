using UnityEngine;
using System.Collections;

public class HeroSoul_Marksman_Bow : MonoBehaviour
{
    public Mesh bowMesh;
    public Mesh bowDrawnMesh;

    float limitAlpha;
    bool isFadingIn = true;
    bool isFadingOut = false;

    bool instaFade = false;

	public void Start()
    {
        // Save max alpha and reset the initial value
        Color color = GetComponent<SkinnedMeshRenderer>().material.color;
        limitAlpha = color.a;
        if (!instaFade) { color.a = 0.00f; }
        GetComponent<SkinnedMeshRenderer>().material.color = color;
    }
	
	void Update()
    {
	    if (isFadingIn)
        {
            bool isDone = AdjustAlpha(Time.deltaTime * 2.00f);
            if (isDone) { isFadingIn = false; }
        }
        else if (isFadingOut)
        {
            bool isDone = AdjustAlpha(-Time.deltaTime * 1.00f);
            if (isDone) { Destroy(gameObject); }
        }
	}

    bool AdjustAlpha(float delta)
    {
        bool limitReached = false;
        Color color = GetComponent<SkinnedMeshRenderer>().material.color;
        color.a += delta;
        if (color.a >= limitAlpha)
        {
            color.a = limitAlpha;
            limitReached = true;
        }
        else if (color.a <= 0f)
        {
            color.a = 0f;
            limitReached = true;
        }
        GetComponent<SkinnedMeshRenderer>().material.color = color;
        return limitReached;
    }

    public void Ressurect()
    {
        GetComponent<SkinnedMeshRenderer>().sharedMesh = bowMesh;
        isFadingIn = true;
        isFadingOut = false;
    }

    public void Draw()
    {
        GetComponent<SkinnedMeshRenderer>().sharedMesh = bowDrawnMesh;
    }

    // Call this right after the object is instantiated to make sure it is started with maximum alpha;
    public void InstaFade()
    {
        if (instaFade)
        {
            AdjustAlpha(1f);
        }
        instaFade = true;
    }

    public void Kill()
    {
        //gameObject.AddComponent<TimedLife>().Timer = 1.00f;
        isFadingOut = true;
        GetComponent<SkinnedMeshRenderer>().sharedMesh = bowMesh;
    }
}

using UnityEngine;
using System.Collections;

public class RemoveMeshInGame : MonoBehaviour
{
	void Start()
    {
        MeshRenderer Mesh = GetComponent<MeshRenderer>();
        if (Mesh != null) { Destroy(Mesh); }

        SkinnedMeshRenderer SkinnedMesh = GetComponent<SkinnedMeshRenderer>();
        if (SkinnedMesh != null) { Destroy(SkinnedMesh); }

        Destroy(this);
	}
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorchMountedAutoEnable : MonoBehaviour
{
    bool IsEnabled = true;
    GameObject Player;
	void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        //DisableTorch();

        EnableTorch();
    }

	void Update()
    {
        //if (IsEnabled == false && Vector3.Distance(transform.position, Player.transform.position) <= Player.GetComponentInChildren<LightSourceController>().GetLightRange() / 1.5f)
            //EnableTorch();

	}
    void EnableTorch()
    {
        if (IsEnabled == true)
            return;
        List<ParticleSystem> Emitters = new List<ParticleSystem>();
        GetComponentsInChildren<ParticleSystem>(Emitters);
        for (int i = 0; i < Emitters.Count; i++)
        {
            Emitters[i].Play();
        }
        GetComponentInChildren<LightSourceController>().Enable(1.00f);
        IsEnabled = true;
    }
    void DisableTorch()
    {
        if (IsEnabled == false)
            return;
        List<ParticleSystem> Emitters = new List<ParticleSystem>();
        GetComponentsInChildren<ParticleSystem>(Emitters);
        for (int i = 0; i < Emitters.Count; i++)
        {
            Emitters[i].Stop();
        }
        GetComponentInChildren<LightSourceController>().Disable();
        IsEnabled = false;
    }
}

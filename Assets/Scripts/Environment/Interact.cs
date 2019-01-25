﻿using UnityEngine;
using System.Collections;

public abstract class Interact : MonoBehaviour
{
    public float interactRange = 3.00f;
    public Vector3 transformOffset = Vector3.zero;

    protected bool isInRange = false;

    protected GameObject player;
    protected PlayerInteractor interactor;

    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        interactor = player.GetComponent<PlayerInteractor>();
    }

    protected virtual void Update()
    {
        float distanceToPlayer = Math.GetDistance2D(player.transform.position, transform.position);
        if (distanceToPlayer <= interactRange && !isInRange)
        {
            isInRange = true;
            OnRangeEnter();
            interactor.Notify(gameObject);
        }
        else if (distanceToPlayer > interactRange && isInRange)
        {
            isInRange = false;
            OnRangeLeave();
        }
    }

    protected virtual void DisableInteraction()
    {
        interactor.NotifyOnDestroy(gameObject);
    }

    public virtual void OnInteract() {}
    protected virtual void OnRangeEnter() {}
    protected virtual void OnRangeLeave() {}
}

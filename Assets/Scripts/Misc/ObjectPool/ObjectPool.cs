using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Misc.ObjectPool {
	public class ObjectPool {

		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		
		private readonly Dictionary<Resource, ConcurrentBag<GameObject>> Data = new Dictionary<Resource, ConcurrentBag<GameObject>>();
		
		public GameObject Obtain(Resource resource) {
			return FetchOrCreateObject(resource, FetchOrCreateBag(resource));
		}
		
		public void Return(Resource resource, GameObject gameObject) {
			FetchOrCreateBag(resource).Add(gameObject);
		}

		private ConcurrentBag<GameObject> FetchOrCreateBag(Resource resource) {
			ConcurrentBag<GameObject> objectBag;
			if (!Data.TryGetValue(resource, out objectBag)) {
				objectBag = new ConcurrentBag<GameObject>();
				Data.Add(resource, objectBag);
			}

			return objectBag;
		}
		
		private GameObject FetchOrCreateObject(Resource resource, ConcurrentBag<GameObject> bag) {
			GameObject fetchedObject;
			if (!bag.TryTake(out fetchedObject)) {
				fetchedObject = (GameObject) Object.Instantiate(Assets.Get(resource)); 
			}

			return fetchedObject;
		}
	}
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Misc.ObjectPool {
	[UsedImplicitly]
	public class ObjectPool {
		private readonly Assets Assets = AutowireFactory.GetInstanceOf<Assets>();
		private readonly Dictionary<Prefab, GameObject> Singletons = new Dictionary<Prefab, GameObject>();
		private readonly Dictionary<Prefab, ConcurrentBag<GameObject>> Data = new Dictionary<Prefab, ConcurrentBag<GameObject>>();
		
		public GameObject Obtain(Prefab prefab) {
			var gameObject = FetchOrCreateObject(prefab, FetchOrCreateBag(prefab));
			gameObject.transform.position = Vector3.zero;
			gameObject.SetActive(true);
			return gameObject;
		}
		
		public GameObject ObtainSingleton(Prefab prefab) {
			if (!Singletons.TryGetValue(prefab, out var singleton)) {
				singleton = (GameObject) Object.Instantiate(Assets.Get(prefab));
				Singletons.Add(prefab, singleton);
			}
			return singleton;
		}

		public GameObject ObtainForDuration(Prefab prefab, float duration) {
			var gameObject = Obtain(prefab);
			var timer = new Timer();
			timer.Start(duration);
			timer.SetOnDoneAction(() => Return(prefab, gameObject));
			return gameObject;
		}
		
		public void Return(Prefab prefab, GameObject gameObject) {
			gameObject.SetActive(false);
			FetchOrCreateBag(prefab).Add(gameObject);
		}

		private ConcurrentBag<GameObject> FetchOrCreateBag(Prefab prefab) {
			if (!Data.TryGetValue(prefab, out var objectBag)) {
				objectBag = new ConcurrentBag<GameObject>();
				Data.Add(prefab, objectBag);
			}

			return objectBag;
		}
		
		private GameObject FetchOrCreateObject(Prefab prefab, ConcurrentBag<GameObject> bag) {
			if (!bag.TryTake(out var fetchedObject)) {
				fetchedObject = (GameObject) Object.Instantiate(Assets.Get(prefab)); 
			}

			return fetchedObject;
		}
	}
}
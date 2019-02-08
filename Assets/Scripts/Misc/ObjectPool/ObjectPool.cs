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
		private readonly Dictionary<Prefab, ConcurrentBag<GameObject>> Data = new Dictionary<Prefab, ConcurrentBag<GameObject>>();
		
		public GameObject Obtain(Prefab prefab) {
			var gameObject = FetchOrCreateObject(prefab, FetchOrCreateBag(prefab));
			gameObject.transform.position = Vector3.zero;
			gameObject.SetActive(true);
			return gameObject;
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
			ConcurrentBag<GameObject> objectBag;
			if (!Data.TryGetValue(prefab, out objectBag)) {
				objectBag = new ConcurrentBag<GameObject>();
				Data.Add(prefab, objectBag);
			}

			return objectBag;
		}
		
		private GameObject FetchOrCreateObject(Prefab prefab, ConcurrentBag<GameObject> bag) {
			GameObject fetchedObject;
			if (!bag.TryTake(out fetchedObject)) {
				fetchedObject = (GameObject) Object.Instantiate(Assets.Get(prefab)); 
			}

			return fetchedObject;
		}
	}
}
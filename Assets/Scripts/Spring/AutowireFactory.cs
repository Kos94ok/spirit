using System;
using System.Collections.Generic;
using UnityEngine;

public static class AutowireFactory {
	private static readonly Dictionary<Type, object> library = new Dictionary<Type, object>();

	public static T GetInstanceOf<T>() {
		object instance;
		if (library.TryGetValue(typeof(T), out instance)) {
			return (T)instance;
		}

		instance = Activator.CreateInstance(typeof(T));
		library.Add(typeof(T), instance);
		return (T)instance;
	}
}


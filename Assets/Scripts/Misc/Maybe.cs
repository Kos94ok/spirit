using System;
using System.Collections.Generic;
using System.Linq;

namespace Misc {
	public struct Maybe<T> {
		private readonly IEnumerable<T> Values;

		public static Maybe<T> Some(T value) {
			if (value == null) {
				throw new InvalidOperationException();
			}

			return new Maybe<T>(new [] { value });
		}

		public static Maybe<T> None => new Maybe<T>(new T[0]);

		private Maybe(IEnumerable<T> values) {
			Values = values;
		}

		public bool HasValue => Values != null && Values.Any();

		public T Value {
			get {
				if (!HasValue) {
					throw new InvalidOperationException("Maybe does not have a value");
				}

				return Values.Single();
			}
		}
	}
}
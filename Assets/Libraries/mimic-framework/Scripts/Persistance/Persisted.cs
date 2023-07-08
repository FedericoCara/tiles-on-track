using System;

namespace Mimic.Persistance
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class Persisted : Attribute {
	}
}


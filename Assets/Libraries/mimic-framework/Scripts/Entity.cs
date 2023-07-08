using System;
using System.Collections.Generic;

using Mimic.Persistance;

namespace Mimic
{
	[Serializable]
	public class Entity : CustomEqualsObject, ICloneable {

		protected int id;
		public int ID {
			get => id;
			set => id = value;
		}

		protected Entity(){}

		protected Entity(int id){
			this.id = id;
		}

		public override bool Equals (object obj) {
			if (obj is Entity otherEntity) {
				return otherEntity.id == id;
			} else
				return false;
		}

		public override int GetHashCode () {
			return 17 * id;
		}

		#region ICloneable implementation

		public virtual object Clone ()
		{
			Entity newEntity = new Entity ();
			newEntity.CopyFrom (this);
			return newEntity;
		}

		public virtual void CopyFrom(Entity refEntity){
			this.id = refEntity.id;
		}

		#endregion

	}
}


using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Tuple
{
	/// <summary>
	/// Represents a basic property within the Hibernate runtime-metamodel.
	/// </summary>
	/// <remarks>
	/// Author: Steve Ebersole
	/// </remarks>
	public class StandardProperty : Property
	{
		private bool lazy;
		private bool insertable;
		private bool updateable;
		private bool insertGenerated;
		private bool updateGenerated;
		private bool nullable;
		private bool dirtyCheckable;
		private bool versionable;
		private Cascades.CascadeStyle cascadeStyle;

		/// <summary>
		/// Constructs StandardProperty instances.
		/// </summary>
		/// <param name="name">The name by which the property can be referenced within
		/// its owner.</param>
		/// <param name="node">The node name to use for XML-based representation of this
		/// property.</param>
		/// <param name="type">The Hibernate Type of this property.</param>
		/// <param name="lazy">Should this property be handled lazily?</param>
		/// <param name="insertable">Is this property an insertable value?</param>
		/// <param name="updateable">Is this property an updateable value?</param>
		/// <param name="insertGenerated">Is this property generated in the database on insert?</param>
		/// <param name="updateGenerated">Is this property generated in the database on update?</param>
		/// <param name="nullable">Is this property a nullable value?</param>
		/// <param name="checkable">Is this property a checkable value?</param>
		/// <param name="versionable">Is this property a versionable value?</param>
		/// <param name="cascadeStyle">The cascade style for this property's value.</param>
		public StandardProperty(
			String name,
			String node,
			IType type,
			bool lazy,
			bool insertable,
			bool updateable,
			bool insertGenerated,
			bool updateGenerated,
			bool nullable,
			bool checkable,
			bool versionable,
			Cascades.CascadeStyle cascadeStyle)
			: base(name, node, type)
		{
			this.lazy = lazy;
			this.insertable = insertable;
			this.updateable = updateable;
			this.insertGenerated = insertGenerated;
			this.updateGenerated = updateGenerated;
			this.nullable = nullable;
			this.dirtyCheckable = checkable;
			this.versionable = versionable;
			this.cascadeStyle = cascadeStyle;
		}

		public bool IsLazy
		{
			get { return lazy; }
		}

		public bool IsInsertable
		{
			get { return insertable; }
		}

		public bool IsUpdateable
		{
			get { return updateable; }
		}

		public bool IsInsertGenerated
		{
			get { return insertGenerated; }
		}

		public bool IsUpdateGenerated
		{
			get { return updateGenerated; }
		}

		public bool IsNullable
		{
			get { return nullable; }
		}

		public bool IsDirtyCheckable(bool hasUninitializedProperties)
		{
			return IsDirtyCheckable() && (!hasUninitializedProperties || !IsLazy);
		}

		public bool IsDirtyCheckable()
		{
			return dirtyCheckable;
		}

		public bool IsVersionable
		{
			get { return versionable; }
		}

		public Cascades.CascadeStyle CascadeStyle
		{
			get { return cascadeStyle; }
		}
	}
}
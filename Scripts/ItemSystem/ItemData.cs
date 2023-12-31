using Godot;

namespace Game.ItemSystem;

[Tool]
internal abstract partial class ItemData : Resource {
	public static readonly ShaderMaterial s_HighlightOverlay = GD.Load<ShaderMaterial>("res://Materials/Highlight.tres");

	[Export] public StringName Name { get; set; }
	[Export] public float Mass { get; private set; } = 1.0f;
	[Export] public Mesh Mesh { get; private set; }
	[Export()] public Shape3D CollisionShape { 
		get {
			return m_CollisionShape ??= Mesh.CreateConvexShape(true, true);
		}
		private set {
			m_CollisionShape = value;
		}
	} // NOTE: Leave empty to auto generate CollisionShape from Mesh
	private Shape3D m_CollisionShape = null;

	public abstract Item Spawn();
}

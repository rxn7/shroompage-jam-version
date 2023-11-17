using System.Collections.Generic;
using Godot;
using Game.Player;
using Game.Utils;

namespace Game.ItemSystem; 

internal abstract partial class Item : RigidBody3D {
	protected const float DropForce = 2f;
	
	public ItemData Data { get; set; }
	
	private MeshInstance3D m_Mesh;
	private static readonly Material HighlightShaderMaterial = GD.Load<Material>("res://Materials/Highlight.tres");

	public override void _Ready() {
		m_Mesh = GetChild<MeshInstance3D>(0);
		DisableMode = DisableModeEnum.Remove;
	}

	public virtual void Highlight() {
		m_Mesh.MaterialOverlay = HighlightShaderMaterial;
	}

	public virtual void Unhighlight() {
		m_Mesh.MaterialOverlay = null;
	}
	
	public virtual void Equip(PlayerManager player) { }
}

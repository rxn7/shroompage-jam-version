using System.Collections.Generic;
using Godot;
using Game.Player;
using Game.Utils;

namespace Game.ItemSystem; 

internal abstract partial class Item : RigidBody3D {
	protected const float DropForce = 2f;
	
	public ItemData Data { get; set; }
	
	public MeshInstance3D MeshInstance { get; set; }

	public override void _Ready() {
		MeshInstance = GetChild<MeshInstance3D>(0);
		DisableMode = DisableModeEnum.Remove;
		ApplyHighlightShader();
	}

	public virtual void Highlight() {
	}

	public virtual void Unhighlight() {
	}
	
	public virtual void Equip(PlayerManager player) { 
		MeshInstance.MaterialOverlay = null;
	}

	public void ApplyHighlightShader() {
		MeshInstance.MaterialOverlay = ItemData.s_HighlightOverlay;
	}
}

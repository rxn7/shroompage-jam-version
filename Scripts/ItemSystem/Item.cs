using System.Collections.Generic;
using Godot;
using Game.Player;
using Game.Utils;

namespace Game.ItemSystem; 

internal partial class Item : RigidBody3D {
	private const float DropForce = 2f;

	private static readonly List<AudioStream> PickupSounds = ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/Item/Pickup");
	private static readonly List<AudioStream> DropSounds = ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/Item/Drop");
	
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
	
	public virtual void Equip(PlayerManager player) {
		ProcessMode = ProcessModeEnum.Disabled;
		SoundManager.Play3D(player.GlobalPosition, PickupSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
		player.Viewmodel.AttachToHandSlot(this, Data.HandSlotOffset, Data.HandSlotRotation);
        // TODO: Play equip anim first
        player.Viewmodel.PlayAnimation(Data.IdleAnimationName);
	}

	public virtual void Drop(Vector3 playerVelocity, Vector3 forward) {
		SoundManager.Play3D(GlobalPosition, DropSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
		ProcessMode = ProcessModeEnum.Inherit;
		Reparent(GetTree().CurrentScene);
		ApplyImpulse(forward * DropForce + playerVelocity);
	}
}

using Godot;
using Game.ItemSystem;

namespace Game.Player; 

internal class PlayerItemManager {
	public static readonly StringName PickupInputName = "ItemPickup";
	public static readonly StringName DropInputPickup = "ItemDrop";
	
	public Item HeldItem { get; private set; }
	private readonly PlayerManager m_Player;

	public PlayerItemManager(PlayerManager player) {
		m_Player = player;
	}

	public void Update() {
		if (m_Player.ItemRaycast.HighlightedItem is not null && Input.IsActionJustPressed(PickupInputName)) {
			DropHeldItem();
			
			HeldItem = m_Player.ItemRaycast.HighlightedItem;
			m_Player.ItemRaycast.ResetHighlightedItem();
			
			HeldItem.Equip(m_Player);
		} else if (HeldItem is not null && Input.IsActionJustPressed(DropInputPickup)) {
			DropHeldItem();
		}
	}

	public void DropHeldItem() {
		HeldItem?.Drop(m_Player.Controller.Velocity, -m_Player.Head.Camera.GlobalTransform.Basis.Z);
		HeldItem = null;
	}
}
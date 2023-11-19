using Godot;
using Game.ItemSystem;
using Game.Utils;
using System.Collections.Generic;

namespace Game.Player; 

internal class PlayerItemManager {
	public static readonly HoldableItemData MacheteItemData = GD.Load<HoldableItemData>("res://Items/MacheteItem.tres");
	public static readonly StringName PickupInputName = "ItemPickup";
	public static readonly StringName DropInputPickup = "ItemDrop";
	
	public HoldableItem HeldItem { get; set; }
	private readonly PlayerManager m_Player;

	public PlayerItemManager(PlayerManager player) {
		m_Player = player;
	}

	public void Update() {
		if (m_Player.ItemRaycast.HighlightedItem is not null && Input.IsActionJustPressed(PickupInputName)) {
			Item highligtedItem = m_Player.ItemRaycast.HighlightedItem;

			if(highligtedItem is HoldableItem highlitedHoldableItem) {
				DropHeldItem();
				HeldItem = highlitedHoldableItem;
			}

			highligtedItem.Equip(m_Player);
			m_Player.ItemRaycast.ResetHighlightedItem();
		} else if (HeldItem is not null && Input.IsActionJustPressed(DropInputPickup)) {
			// DropHeldItem(true);
			// NOTE!: Turn off, because we dont have more weapons anyway
		}
	}

	public void DropHeldItem(bool playEquipAnim = false) {
		HeldItem?.Drop(m_Player.Controller.Velocity, -m_Player.Head.Camera.GlobalTransform.Basis.Z);
		HeldItem = null;

		if(playEquipAnim)
			m_Player.Viewmodel.PlayEquipAnimation();
	}
}

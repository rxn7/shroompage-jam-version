using Godot;
using Godot.Collections;
using Game.ItemSystem;

namespace Game.Player; 

internal partial class PlayerItemRaycast : Node {
	private const float Reach = 2.5f;

	[Export(PropertyHint.Layers3DPhysics)] private uint m_CollisionMask;
	[Export] private Label m_PickupLabel;
	private PlayerManager m_Player;
	
	private Item m_HighlightedItem;
	private PhysicsRayQueryParameters3D m_RayQuery;
	
	public Item HighlightedItem {
		get => m_HighlightedItem;
		private set {
			m_HighlightedItem = value;

			if (m_HighlightedItem is null) {
				if(m_PickupLabel is not null) 
					m_PickupLabel.Visible = false;
				return;
			}

			m_PickupLabel.Visible = true;

			using InputEventKey keyEvent = InputMap.ActionGetEvents(PlayerItemManager.PickupInputName)[0] as InputEventKey;
			m_PickupLabel.Text = $"Press [{keyEvent.PhysicalKeycode}] to pickup {m_HighlightedItem.Data.Name}";
		}
	}

	public override void _Ready() {
		m_Player = GetParent<PlayerManager>();
		HighlightedItem = null;
		m_RayQuery = new PhysicsRayQueryParameters3D();;
		m_RayQuery.CollisionMask = m_CollisionMask;
	}

	public override void _PhysicsProcess(double delta) {
		Transform3D cameraTransform = m_Player.Head.Camera.GlobalTransform;
		Vector3 from = cameraTransform.Origin;
		m_RayQuery.From = from;
		m_RayQuery.To = from - cameraTransform.Basis.Z * Reach;

		using PhysicsDirectSpaceState3D spaceState = m_Player.GetWorld3D().DirectSpaceState;
		using Dictionary result = spaceState.IntersectRay(m_RayQuery);

		if (result.Count == 0 || result["collider"].AsGodotObject() is not Item item) {
			ResetHighlightedItem();
			return;
		}
			
		if (item != HighlightedItem)
			HighlightedItem = item;
	}
	
	public void ResetHighlightedItem() {
		HighlightedItem = null;
	}
}

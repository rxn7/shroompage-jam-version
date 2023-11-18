using Godot;
using Game.Utils;
using Game.ItemSystem;

namespace Game.Player;

internal partial class PlayerManager : CharacterBody3D {
	private const float KickCooldown = 0.5f;
	private const float HighLevelReduceRate = 0.05f;

	private static readonly StringName AttackInputAction = "Attack";
	private static readonly StringName KickInputAction = "Kick";
	private static readonly AudioStream PunchSound = GD.Load<AudioStream>("res://Audio/Punch.wav");
	
	public PlayerHead Head { get; private set; }
	public PlayerViewmodel Viewmodel { get; private set; }
	public PlayerController Controller { get; private set; }
	public PlayerItemManager ItemManager { get; private set; }
	public PlayerBobbing Bobbing { get; private set; }
	public PlayerItemRaycast ItemRaycast { get; private set; }
	public Headlight Headlight { get; private set; }
	public ScreenEffect ScreenEffect { get; private set; }

	[Export] private AudioStream[] m_KickSounds = new AudioStream[0];
	[Export] private AudioStream[] m_KickImpactSounds = new AudioStream[0];

	public float Health {
		get => m_Health;
		set { 
			m_Health = Mathf.Clamp(value, 0.0f, 100.0f); 
			if(m_Health == 0.0f) {
				// TODO:die
				//Die();
			}
		}
	}
	private float m_Health = 10.0f;

	public float HighLevel { 
		get => m_HighLevel;
		set { m_HighLevel = Mathf.Clamp(value, 0.0f, 1.0f); }
	}
	private float m_HighLevel = 0.0f;
	
	private float m_KickCooldownTimer = 0.0f;

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;

		Head = GetNode<PlayerHead>("Head");
		Viewmodel = Head.Camera.GetNode<PlayerViewmodel>("Viewmodel");
		Viewmodel.Player = this; 
		ItemRaycast = GetNode<PlayerItemRaycast>("ItemRaycast");
		Headlight = Head.Camera.GetNode<Headlight>("Headlight");

		ScreenEffect = GetNode("HUD").GetNode<ScreenEffect>("Screen");
		ScreenEffect.Player = this;

		Controller = new PlayerController(this);
		Bobbing = new PlayerBobbing(this);
		ItemManager = new PlayerItemManager(this);

		ItemManager.HeldItem = PlayerItemManager.MacheteItemData.Spawn() as HoldableItem;
		AddChild(ItemManager.HeldItem);
		ItemManager.HeldItem.Equip(this);
	}

	public override void _Process(double dt) {
		HighLevel -= (float)dt * HighLevelReduceRate;
		m_KickCooldownTimer += (float)dt;

		Controller.Update((float)dt);
		Bobbing.Update((float)dt);
		
		ItemManager.Update();
		
		if(Input.IsActionPressed(AttackInputAction))
			Attack();

		if(Input.IsActionJustPressed(KickInputAction))
			Kick();
	}

	public override void _PhysicsProcess(double dt) {
		Controller.PhysicsUpdate((float)dt);
	}

	public override void _Input(InputEvent e) {
		switch(e) {
			case InputEventMouseMotion motion:
				Controller.HandleMouseMotionEvent(motion);
				break;
		}
	}

	private void Attack() {
		if (!Viewmodel.AnimPlayer.CurrentAnimation.EndsWith("Idle"))
			return;

		Viewmodel.PlayAttackAnimation();
		SoundManager.Play3D(GlobalPosition, PunchSound, (float)GD.RandRange(0.8f, 1.2f));
	}

	private void Kick() {
		if (m_KickCooldownTimer < KickCooldown)
			return;

		m_KickCooldownTimer = 0.0f;

		Viewmodel.PlayLegKickAnimation();
		SoundManager.Play3D(GlobalPosition, m_KickSounds.GetRandomItem(), (float)GD.RandRange(0.8f, 1.2f));
	}
}

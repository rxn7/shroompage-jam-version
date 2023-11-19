using Godot;
using Game.Utils;
using Game.ItemSystem;
using System;
using Game.Story;

namespace Game.Player;

internal partial class PlayerManager : CharacterBody3D, IHealth {
	private const float KickCooldown = 0.5f;
	private const float KickRange = 3.0f;
	private const float KickDamage = 5.0f;
	private const float KickKnockback = 6.0f;
	private const float MeleeRange = 4.0f;
	private const float MeleeKnockback = 2.0f;
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
	public PlayerNotificationDisplay NotificationDisplay { get; private set; }
	public Node HUD;
	public Action<float> OnHealthChanged { get; set; }

	[Export] private AudioStream[] m_KickSounds = new AudioStream[0];
	[Export] private AudioStream[] m_KickImpactSounds = new AudioStream[0];
	[Export] private AudioStream[] m_HurtSounds = new AudioStream[0];

	public float Health {
		get => m_Health;
		set { 
			m_Health = Mathf.Clamp(value, 0.0f, 100.0f); 
			OnHealthChanged?.Invoke(m_Health); 
		}
	}
	private float m_Health = 40.0f;

	public float HighLevel { 
		get => m_HighLevel;
		set { m_HighLevel = Mathf.Clamp(value, 0.0f, 1.0f); }
	}
	private float m_HighLevel = 0.0f;

	public Action OnDied { get; set; }
	public Action<float> OnDamage { get; set; }
	public bool ViewmodelDisabled { get; set; } = false;
	public bool IsDead { get; set; } = false;

	private float m_KickCooldownTimer = 0.0f;

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;

		HUD =  GetNode("HUD");
		Head = GetNode<PlayerHead>("Head");
		Viewmodel = Head.Camera.GetNode<PlayerViewmodel>("Viewmodel");
		Viewmodel.Player = this; 
		ItemRaycast = GetNode<PlayerItemRaycast>("ItemRaycast");
		Headlight = Head.Camera.GetNode<Headlight>("Headlight");
		NotificationDisplay = HUD.GetNode<PlayerNotificationDisplay>("Notification");

		ScreenEffect = HUD.GetNode<ScreenEffect>("Screen");
		ScreenEffect.Player = this;

		Controller = new PlayerController(this);
		Bobbing = new PlayerBobbing(this);
		ItemManager = new PlayerItemManager(this);

		ItemManager.HeldItem = PlayerItemManager.MacheteItemData.Spawn() as HoldableItem;
		AddChild(ItemManager.HeldItem);
		ItemManager.HeldItem.Equip(this);

		OnDied += () => {
			GetTree().ChangeSceneToFile("res://Scenes/UI/SplashScreen.tscn");
		};

		OnDamage += (float dmg) => {
			SoundManager.Play3D(GlobalPosition, m_HurtSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
		};
	}

	public override void _Process(double dt) {
		HighLevel -= (float)dt * HighLevelReduceRate;
		m_KickCooldownTimer += (float)dt;

		Controller.Update((float)dt);
		Bobbing.Update((float)dt);
		
		ItemManager.Update();

		if (ViewmodelDisabled) {
			Viewmodel.Hide();
			return;
		} else {
			Viewmodel.Show();
		}
		
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
		SoundManager.Play3D(GlobalPosition, ItemManager.HeldItem?.HoldableData.AttackSounds.GetRandomItem() ?? PunchSound, (float)GD.RandRange(0.8f, 1.2f));

		if(Raycast(MeleeRange, ItemManager.HeldItem?.HoldableData.Damage ?? 5, MeleeKnockback, out IHealth health)) {
			SoundManager.Play3D(GlobalPosition, ItemManager.HeldItem?.HoldableData.AttackHitSounds.GetRandomItem(), (float)GD.RandRange(0.8f, 1.2f));
		}
	}

	private void Kick() {
		if (m_KickCooldownTimer < KickCooldown)
			return;

		m_KickCooldownTimer = 0.0f;

		Viewmodel.PlayLegKickAnimation();
		SoundManager.Play3D(GlobalPosition, m_KickSounds.GetRandomItem(), (float)GD.RandRange(0.8f, 1.2f));

		if(Raycast(KickRange, KickDamage, KickKnockback, out IHealth health)) {
			SoundManager.Play3D(GlobalPosition, m_KickImpactSounds.GetRandomItem(), (float)GD.RandRange(0.8f, 1.2f), 50.0f);
		}
	}

	private bool Raycast(float range, float damage, float knockback, out IHealth health) {
		PhysicsRayQueryParameters3D query = new() {
			CollideWithAreas = false,
			CollideWithBodies = true,
			From = Head.Camera.GlobalPosition,
			To = Head.Camera.GlobalPosition - Head.Camera.GlobalTransform.Basis.Z * range,
		};

		Godot.Collections.Dictionary raycastResults = PhysicsServer3D.SpaceGetDirectState(GetWorld3D().Space).IntersectRay(query);
		if(raycastResults.Count == 0) {
			health = null;
			return false;
		}

		GodotObject collider = raycastResults["collider"].As<GodotObject>();
		if(collider is not IHealth h) {
			health = null;
			return false;
		}

		if(collider is Enemy.Enemy enemy) {
			enemy.ApplyImpulse(-Head.Camera.GlobalTransform.Basis.Z * knockback + Vector3.Up * 5);
		}

		health = h;
		health.Damage(damage);
		return true;
	}
}

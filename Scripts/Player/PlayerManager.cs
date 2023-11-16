using System;
using Godot;
using Game.Utils;

namespace Game.Player;

internal partial class PlayerManager : CharacterBody3D {
	private static readonly StringName AttackInputAction = "Attack";
	private static readonly StringName DefaultAttackAnimation = "FistsAttack";
	private static readonly AudioStream PunchSound = GD.Load<AudioStream>("res://Audio/Punch.wav");
	
	public PlayerHead Head { get; private set; }
	public PlayerViewmodel Viewmodel { get; private set; }
	public PlayerController Controller { get; private set; }
	public PlayerItemManager ItemManager { get; private set; }
	public PlayerBobbing Bobbing { get; private set; }
	public PlayerItemRaycast ItemRaycast { get; private set; }

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;

		Head = GetNode<PlayerHead>("Head");
		Viewmodel = Head.Camera.GetNode<PlayerViewmodel>("Viewmodel");
		ItemRaycast = GetNode<PlayerItemRaycast>("ItemRaycast");

		Controller = new PlayerController(this);
		Bobbing = new PlayerBobbing(this);
		ItemManager = new PlayerItemManager(this);
	}

	public override void _Process(double dt) {
		Controller.Update((float)dt);
		Bobbing.Update((float)dt);
		
		ItemManager.Update();
		
		if(Input.IsActionPressed(AttackInputAction))
			Attack();
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

		Viewmodel.PlayAnimation(DefaultAttackAnimation);
		SoundManager.Play3D(GlobalPosition, PunchSound, (float)GD.RandRange(0.8f, 1.2f));
	}
}

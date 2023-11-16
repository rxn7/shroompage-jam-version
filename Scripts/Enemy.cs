using Godot;
using Game.Surface;
using Game.Utils;

namespace Game {
	internal partial class Enemy : CharacterBody3D {
		private float m_MoveSpeed = 1.0f;
		[Export] private float MoveSpeed {
			get => m_MoveSpeed;
			set {
				m_MoveSpeed = value;
				m_AnimationPlayer.SpeedScale = m_MoveSpeed * 2.0f;
			}
		}
		[Export] private float m_Acceleration = 1.5f;
		private AnimationPlayer m_AnimationPlayer;
		private NavigationAgent3D m_Agent;
		private Vector3 m_Velocity;
		private float m_TargetAngle;
		private ESurfaceMaterial m_SurfaceMaterial;
		
		public override void _Ready() {
			m_Agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
			m_AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			m_Agent.TargetDesiredDistance = 1.0f;
			
			m_Agent.VelocityComputed += (Vector3 safeVelocity) => {
				m_Velocity = safeVelocity;
			};

			MoveSpeed = m_MoveSpeed; // Trigger the animation player speed scale update
		}

		public override void _PhysicsProcess(double dt) {
			Vector3 distance = (m_Agent.GetNextPathPosition() - GlobalPosition);
			Vector3 direction = new Vector3(distance.X, 0.0f, distance.Z).Normalized();
			
			m_Agent.TargetPosition = World.Instance.Player.GlobalPosition;
			m_Agent.Velocity =  direction * MoveSpeed;
			m_SurfaceMaterial = this.GetSurfaceMaterial();

			m_TargetAngle = MathUtils.MoveTowardAngle(m_TargetAngle, Mathf.Atan2(direction.X, direction.Z) - Mathf.Pi, m_Acceleration * (float)dt);
			Rotation = new Vector3(0.0f, m_TargetAngle, 0.0f);

			if (distance.Length() > m_Agent.TargetDesiredDistance) {
				Velocity = Velocity.MoveToward(m_Velocity, m_Acceleration * (float)dt);
				MoveAndSlide();
			}
		}

		public void PlayFootstep() {
			SurfaceManager.PlayRandomFootstep(m_SurfaceMaterial, GlobalPosition, (float)GD.RandRange(0.9f, 1.1f), -20.0f);
		}
	}
}

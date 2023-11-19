using Godot;

namespace Game;

internal partial class ParticleFx : CpuParticles3D {
	public static readonly PackedScene s_SpawnParticles = GD.Load<PackedScene>("res://Scenes/Particles/Spawn.tscn");
	public static readonly PackedScene s_DeathParticles = GD.Load<PackedScene>("res://Scenes/Particles/Death.tscn");

	public override void _Ready() {
		OneShot = true;
		Emitting = false;
	}

	public override void _PhysicsProcess(double delta) {
		if(!Emitting) QueueFree();
	}
}

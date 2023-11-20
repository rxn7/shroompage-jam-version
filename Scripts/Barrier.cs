using System.Collections.Generic;
using Game.Utils;
using Godot;

namespace Game;

internal partial class Barrier : Node3D {
	private static readonly List<AudioStream> DestructSounds = ResourceUtils.LoadAll<AudioStream>("res://Audio/BarrierDestruct/barrier_destruct1.wav", "res://Audio/BarrierDestruct/barrier_destruct2.wav");

	public void Destruct() {
		SoundManager.Play3D(GlobalPosition, DestructSounds.GetRandomItem(), (float)GD.RandRange(0.8f, 1.2f));
		QueueFree();
	}
}

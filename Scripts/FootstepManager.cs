using System.Collections.Generic;
using Godot;
using Game.Utils;

namespace Game.Surface;

internal struct SoundData {
	public List<AudioStream> Sounds;
	public int LastPlayedIdx; 

	public SoundData(List<AudioStream> sounds) {
		Sounds = sounds;
		LastPlayedIdx = -1;
	}

	public AudioStream GetRandomSound() {
		int idx = 0;
		do { idx = (int)(GD.Randi() % Sounds.Count); } while(idx == LastPlayedIdx);
		
		LastPlayedIdx = idx;

		return Sounds[idx];
	}
}

internal static class FootstepManager {
	public static readonly StringName SurfaceMetaName = "Surface";

	private static SoundData s_Footsteps;
	private static SoundData s_Jumps;
	private static SoundData s_Lands;

	public static void Init() {
		s_Footsteps = new(ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/FootStep"));
		s_Jumps = new(ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/Jump"));
		s_Lands = new(ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/Land"));
		GD.Print("FootstepManager initialized");
	}
	
	public static void PlayFootstep(Vector3 position, float pitch = 1.0f, float volumeDb = 0.0f) {
		SoundManager.Play3D(position, s_Footsteps.GetRandomSound(), pitch, volumeDb);
	}

	public static void PlayJump(Vector3 position, float pitch = 1.0f, float volumeDb = 0.0f) {
		SoundManager.Play3D(position, s_Jumps.GetRandomSound(), pitch, volumeDb);
	}

	public static void PlayLand(Vector3 position, float pitch = 1.0f, float volumeDb = 0.0f) {
		SoundManager.Play3D(position, s_Lands.GetRandomSound(), pitch, volumeDb);
	}
}

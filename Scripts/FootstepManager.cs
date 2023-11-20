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
		s_Footsteps = new(ResourceUtils.LoadAll<AudioStream>("res://Audio/FootStep/player_fstep1.wav", "res://Audio/FootStep/player_fstep2.wav", "res://Audio/FootStep/player_fstep3.wav", "res://Audio/FootStep/player_fstep4.wav", "res://Audio/FootStep/player_fstep5.wav", "res://Audio/FootStep/player_fstep6.wav", "res://Audio/FootStep/player_fstep7.wav", "res://Audio/FootStep/player_fstep8.wav"));
		s_Jumps = new(ResourceUtils.LoadAll<AudioStream>("res://Audio/Jump/player_fstep_jump1.wav", "res://Audio/Jump/player_fstep_jump2.wav", "res://Audio/Jump/player_fstep_jump3.wav", "res://Audio/Jump/player_fstep_jump4.wav"));
		s_Lands = new(ResourceUtils.LoadAll<AudioStream>("res://Audio/Land/player_fstep_land1.wav", "res://Audio/Land/player_fstep_land2.wav", "res://Audio/Land/player_fstep_land3.wav", "res://Audio/Land/player_fstep_land4.wav"));
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

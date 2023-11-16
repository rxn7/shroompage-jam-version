using System.Collections.Generic;
using Godot;
using Game.Utils;

namespace Game.Surface;

internal static class SurfaceManager {
	private static readonly Dictionary<ESurfaceMaterial, SurfaceData> SurfaceDataMap = new();
	private static readonly StringName SurfaceMetaName = "Surface";

	public static void Init() {
		foreach(ESurfaceMaterial mat in (ESurfaceMaterial[])System.Enum.GetValues(typeof(ESurfaceMaterial)))
			if(mat != ESurfaceMaterial.None)
				SurfaceDataMap.Add(mat, new SurfaceData(mat));

		GD.Print("FootstepManager initialized");
	}
	
	public static ESurfaceMaterial GetSurfaceMaterial(this CharacterBody3D body) {
		for (int i = 0; i < body.GetSlideCollisionCount(); ++i) {
			KinematicCollision3D collision = body.GetSlideCollision(i);

			if (collision.GetNormal() != body.GetFloorNormal())
				continue;

			GodotObject collider = collision.GetCollider();
			if (!collider.HasMeta(SurfaceMetaName))
				continue;
			
			if (System.Enum.TryParse(collider.GetMeta(SurfaceMetaName).AsString(), out ESurfaceMaterial mat))
				return mat;
		}

		return ESurfaceMaterial.None;
	}

	public static void PlayRandomFootstep(ESurfaceMaterial surfaceMaterial, Vector3 position, float pitch = 1.0f, float volumeDb = 0.0f) {
		if(!SurfaceDataMap.ContainsKey(surfaceMaterial))
			return;

		SurfaceData data = SurfaceDataMap[surfaceMaterial];
		SoundManager.Play3D(position, data.GetRandomFootstepSound(), pitch, volumeDb);
	}

	public class SurfaceData {
		public ESurfaceMaterial Material { get; private set; }
		public List<AudioStream> FootstepSounds { get; private set; }

		private int m_LastIndex;

		public SurfaceData(ESurfaceMaterial material) {
			Material = material;
			string path = $"res://Audio/FootSteps/{Material}";
			FootstepSounds = ResourceUtils.LoadAllAudioStreamsFromDirectory(path);
		}

		public AudioStream GetRandomFootstepSound() {
			if(FootstepSounds.Count == 1) return FootstepSounds[0];

			int idx = 0;
			do { idx = (int)(GD.Randi() % FootstepSounds.Count); } while(idx == m_LastIndex);
			
			m_LastIndex = idx;
			return FootstepSounds[idx];
		}
	}
}

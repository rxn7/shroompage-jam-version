

using Godot;
using Game;

namespace Game;

struct audioVolumeTargets {
	public AudioStreamPlayer[] m_tracks;
	public float[] m_volumes;
}

// SHIT CODE!!!!!! but im too lazy to fix the new code
internal partial class GameSoundtrack : Node {

	[Export] private AudioStreamPlayer[] m_TracksIntro = new AudioStreamPlayer[1];
	[Export] private AudioStreamPlayer[] m_TracksIntensity_0 = new AudioStreamPlayer[5];
	[Export] private AudioStreamPlayer[] m_TracksIntensity_1 = new AudioStreamPlayer[5];
	[Export] private AudioStreamPlayer[] m_TracksIntensity_2 = new AudioStreamPlayer[5];

	private int m_QueuedIntensityLevel = 0; // max 4
	private int m_IntensityLevel = 0; // max 4
	private int m_Stage = 0;
	private int m_QueuedStage = 0;
	private double m_UpdateTimer = 0;
	private double m_TimeUntilUpdate = 0;
	private bool m_Muted = true;
	private bool m_usingIntroMusic = false;

	public override void _Ready() {
		ApplyVolumeTargets(GetVolumeTargets(), 1);
		
		m_UpdateTimer = m_TracksIntensity_0[0].Stream.GetLength();
		m_TimeUntilUpdate = m_UpdateTimer;
	}

	public override void _Process(double delta_time) {

		int enemyCount = GameManager.Singleton.GetEnemyCount();
		if (enemyCount == 0) m_QueuedIntensityLevel = 0;
		else if (enemyCount <= 2) m_QueuedIntensityLevel = 1;
		else if (enemyCount <= 5) m_QueuedIntensityLevel = 3;
		else m_QueuedIntensityLevel = 4;

		if (m_QueuedIntensityLevel > m_IntensityLevel) {
			m_IntensityLevel = m_QueuedIntensityLevel;
		}

		m_TimeUntilUpdate -= delta_time;
		if (m_TimeUntilUpdate <= 0) {
			m_IntensityLevel = m_QueuedIntensityLevel;
			m_Stage = m_QueuedStage;
			m_TimeUntilUpdate += m_UpdateTimer;
		}

		ApplyVolumeTargets(GetVolumeTargets(), delta_time * 6);
	}

	// exists so that you can have the same audio file play at 2 different stages
	private audioVolumeTargets GetVolumeTargets() {
		audioVolumeTargets targets = new audioVolumeTargets();
		AudioStreamPlayer[][] allStreams = {
			m_TracksIntensity_0,
			m_TracksIntensity_1,
			m_TracksIntensity_2
		};

		AudioStreamPlayer[] targetStreams = new AudioStreamPlayer[16];
		float[] volumeTargets = new float[16];
		int listedTracks = 0;  

		AudioStreamPlayer[] currentStage = allStreams[m_Stage];
		if (m_usingIntroMusic) currentStage = m_TracksIntro;
		for (int i = 0; i < currentStage.Length; i++) {
			targetStreams[listedTracks] = currentStage[i];
			volumeTargets[listedTracks] = i <= m_IntensityLevel && !m_Muted ? 0 : -100;
			listedTracks++;
		}
		
		// compute volume for current state, and then mute all other tracks
		foreach (AudioStreamPlayer[] streamList in allStreams) {
			if (streamList == allStreams[m_Stage]) continue;
			for (int streamPosition = 0; streamPosition < streamList.Length; streamPosition++) {
				AudioStreamPlayer stream = streamList[streamPosition];

				if (!m_usingIntroMusic) {
					// check if volume was already assigned for the previous track
					bool changedVolume = false;
					for (int i = 0; i < targetStreams.Length; i++) {
						if (targetStreams[i] != stream) continue;
						changedVolume = true; 
						break;
					}

					if (changedVolume) continue;
				}
		
				targetStreams[listedTracks] = stream;
				volumeTargets[listedTracks] = -100;
				listedTracks++;
			}            
		}
	

		targets.m_tracks = targetStreams;
		targets.m_volumes = volumeTargets;

		return targets;
	}

	private static void ApplyVolumeTargets(audioVolumeTargets targets, double alpha) {
		for (int i = 0; i < targets.m_tracks.Length; i++) {
			AudioStreamPlayer track = targets.m_tracks[i];
			if (track == null) continue;
			if (!track.Playing) track.Play();
			track.VolumeDb = Mathf.Lerp(targets.m_tracks[i].VolumeDb, targets.m_volumes[i], (float)alpha);
		}
	}

	public void IncreaseStage() {
		m_QueuedStage++;
	}

	public void SetMuted(bool mute) {
		m_Muted = mute;
	}

	public void SetIntroMusic(bool enable) {
		m_usingIntroMusic = enable;
	}
}

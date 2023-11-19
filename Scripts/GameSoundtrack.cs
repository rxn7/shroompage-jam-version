using Godot;

namespace Game;

internal struct TrackList {
	[Export] public AudioStreamPlayer[] Tracks;
}

internal struct AudioVolumeTarget {
	public AudioStreamPlayer Track;
	public float Volume;
}

internal partial class GameSoundtrack : Node {
	private TrackList[] m_TrackLists;
	private int m_QueuedIntensityLevel = 0; // max 4
	private int m_IntensityLevel = 0; // max 4
	private int m_Stage = 0;
	private int m_QueuedStage = 0;
	private double m_UpdateTimer = 0;
	private double m_TimeUntilUpdate = 0;

	public override void _Ready() {
		InitTrackLists();
		ApplyVolumeTargets(GetVolumeTargets(), 1);
		
		m_UpdateTimer = m_TrackLists[0].Tracks[0].Stream.GetLength();
		m_TimeUntilUpdate = m_UpdateTimer;
	}

	public override void _Process(double deltaTime) {
		int enemyCount = GameManager.Singleton.GetEnemyCount();

		m_QueuedIntensityLevel = enemyCount switch {
			0    => 0,
			<= 2 => 1,
			<= 5 => 3,
			_    => 4
		};

		if(m_QueuedIntensityLevel > m_IntensityLevel)
			m_IntensityLevel = m_QueuedIntensityLevel;

		m_TimeUntilUpdate -= deltaTime;
		if(m_TimeUntilUpdate <= 0) {
			m_IntensityLevel = m_QueuedIntensityLevel;
			m_Stage = m_QueuedStage;
			m_TimeUntilUpdate += m_UpdateTimer;
		}

		ApplyVolumeTargets(GetVolumeTargets(), (float)deltaTime * 10.0f);
	}

	// exists so that you can have the same audio file play at 2 different stages
	private AudioVolumeTarget[] GetVolumeTargets() {
		AudioVolumeTarget[] targets = new AudioVolumeTarget[16];
		int listedTracks = 0;  

		TrackList currentStageTrackList = m_TrackLists[m_Stage];

		for(int i=0; i<currentStageTrackList.Tracks.Length; ++i) {
			targets[listedTracks] = new() {
				Volume = i <= m_IntensityLevel ? 0 : -100,
				Track = currentStageTrackList.Tracks[i]
			};

			listedTracks++;
		}
		
		// compute volume for current state, and then mute all other tracks
		for(int streamListPosition = 0; streamListPosition < m_TrackLists.Length; ++streamListPosition) {   
			if (streamListPosition == m_Stage) 
				continue;

			TrackList trackList = m_TrackLists[streamListPosition];
			foreach(AudioStreamPlayer track in trackList.Tracks) {
				bool changedVolume = false;

				foreach(AudioVolumeTarget target in targets) {
					if(target.Track == track) {
						changedVolume = true; 
						break;
					}
				}

				if (changedVolume) 
					continue;
		
				targets[listedTracks] = new() {
					Track = track,
					Volume = -100,
				};

				listedTracks++;
			}            
		}

		return targets;
	}

	private static void ApplyVolumeTargets(AudioVolumeTarget[] targets, float alpha) {
		foreach(AudioVolumeTarget target in targets) {
			if(target.Track is null) 
				continue;

			if(!target.Track.Playing) 
				target.Track.Play();

			target.Track.VolumeDb = Mathf.Lerp(target.Track.VolumeDb, target.Volume, alpha);
		}
	}

	private void InitTrackLists() {
		m_TrackLists = new TrackList[GetChildCount() - 1];

		for(int i=0; i<GetChildCount()-1; ++i) {
			// Starting from 1, 0 is intro
			Node child = GetChild(i+1);

			m_TrackLists[i] = new TrackList() {
				Tracks = new AudioStreamPlayer[child.GetChildCount()]
			};

			for(int j=0; j<child.GetChildCount(); ++j)
				m_TrackLists[i].Tracks[j] = (AudioStreamPlayer)child.GetChild(j);
		}
	}
}

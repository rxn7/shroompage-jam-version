using Godot;

namespace Game;

internal partial class MasterAudioBusManager : Node {
	private AudioEffectReverb m_Reverb;
	private AudioEffectChorus m_Chorus;

	public MasterAudioBusManager() {
		m_Reverb = AudioServer.GetBusEffect(0, 0) as AudioEffectReverb;
		m_Chorus = AudioServer.GetBusEffect(0, 1) as AudioEffectChorus;
	}

	public override void _Process(double delta) {
		float factor = GameManager.Singleton.Player.HighLevel * 0.5f;

		m_Reverb.Wet = factor;
		m_Reverb.Dry = 1.0f - factor * 0.5f;

		m_Chorus.Wet = factor;
		m_Chorus.Dry = 1.0f - factor * 0.5f;
	}
}
 

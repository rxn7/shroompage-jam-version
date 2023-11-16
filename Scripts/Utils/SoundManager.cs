using Godot;

namespace Game.Utils;
internal static class SoundManager {
	private const int StartPoolSize = 10;
	private const int MaxPoolSize = 30;
	private static ObjectPool<AudioStreamPlayer3D> s_Pool;
	private static SceneTree s_Tree;

	public static int PlayingSfx { get; private set; }

	public static void Init(SceneTree tree) {
		s_Tree = tree;
		s_Pool = new ObjectPool<AudioStreamPlayer3D>(CreatePoolObject, DeletePoolObject, StartPoolSize, MaxPoolSize);
		GD.Print("SoundManager initialized");
	}

	public static void Play3D(Vector3 position, AudioStream stream, float pitch = 1.0f, float volume = 0.0f, Node3D parent = null) {
		AudioStreamPlayer3D streamPlayer = s_Pool.Get();

		if(parent != null) {
			streamPlayer.GetParent()?.RemoveChild(streamPlayer);
			parent.AddChild(streamPlayer);
		} else if(streamPlayer.GetParent() is null)
			s_Tree.Root.AddChild(streamPlayer);

		streamPlayer.GlobalPosition = position;
		streamPlayer.Stream = stream;
		streamPlayer.PitchScale = pitch;
		streamPlayer.VolumeDb = volume;
		streamPlayer.Play();

		++PlayingSfx;
	}

	private static AudioStreamPlayer3D CreatePoolObject() {
		AudioStreamPlayer3D streamPlayer = new AudioStreamPlayer3D();
		streamPlayer.Finished += () => OnStreamPlayerFinished(streamPlayer);
		return streamPlayer;
	}

	private static void DeletePoolObject(AudioStreamPlayer3D obj) {
		obj.QueueFree();
	}

	private static void OnStreamPlayerFinished(AudioStreamPlayer3D player) {
		--PlayingSfx;
		s_Pool.Release(player);
	}
}
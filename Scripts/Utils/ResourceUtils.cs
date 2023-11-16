using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Utils;

internal static class ResourceUtils {
	private static readonly string[] AudioExtensions = { "wav", "ogg", "mp3" };

	public static List<AudioStream> LoadAllAudioStreamsFromDirectory(string path) => LoadAllFromDirectory<AudioStream>(path, AudioExtensions);
	
	public static List<T> LoadAllFromDirectory<T>(string path, string[] allowedExtensions) where T : Resource {
		using DirAccess dir = DirAccess.Open(path);
	
		List<T> list = new();
		foreach (string fileName in dir.GetFiles()) {
			string extension = fileName.GetExtension();
			if (!allowedExtensions.Contains(extension))
				continue;

			list.Add(ResourceLoader.Load<T>(path.PathJoin(fileName), extension));
		}

		return list;
	}
}
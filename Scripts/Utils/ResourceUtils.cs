using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Utils;

internal static class ResourceUtils {
	public static List<T> LoadAll<T>(params string[] paths) where T : Resource {
		List<T> list = new();
		foreach (string path in paths) {
			list.Add(ResourceLoader.Load<T>(path));
		}

		return list;
	}
}

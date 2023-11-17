using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Godot;

namespace Game.Utils;

public static class MathUtils {
	public const float Pi2 = Mathf.Pi * 2.0f;
	public const float PiHalf = Mathf.Pi * 0.5f;
	
	public static float AngleDelta(float a1, float a2) {
		float d = (a2 - a1 + 180.0f) % 360.0f - 180.0f;
		return d < -180 ? d + 360 : d;
	}

	public static float SafeLerp(float from, float to, float t) {
		if(Mathf.Abs(to-from) < 0.0001f)
			return to;

		return Mathf.Lerp(from, to, t);
	}

	public static Vector2 SafeLerp(this Vector2 from, Vector2 to, float t) {
		if(Mathf.Abs(to.X - from.X) < 0.0001f && Mathf.Abs(to.Y - from.Y) < 0.0001f)
			return to;

		return from.Lerp(to, t);
	}

	public static Vector3 SafeLerp(this Vector3 from, Vector3 to, float t) {
		if(Mathf.Abs(to.X - from.X) < 0.0001f && Mathf.Abs(to.Y - from.Y) < 0.0001f && Mathf.Abs(to.Z - from.Z) < 0.0001f)
			return to;

		return from.Lerp(to, t);
	}

	public static float NormalizeAngle(float angle) {
		return angle - Mathf.Round(angle / Mathf.Tau) * Mathf.Tau;
	}

	public static float MoveTowardAngle(float from, float to, float delta) {
		float diff = NormalizeAngle(to - from);
		float sign = Mathf.Sign(diff);
		return NormalizeAngle(from + Mathf.Min(delta, diff * sign) * sign);
	}

	public static T GetRandomItem<T>(this T[] array) {
		if (array.Length == 0) return default;
		return array[(int)(GD.Randi() % array.Length)];
	}

	public static T GetRandomItem<T>(this List<T> list) {
		if (list.Count == 0) return default;
		return list[(int)(GD.Randi() % list.Count)];
	}
}

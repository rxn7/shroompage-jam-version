using System;
using Godot;

namespace Game.UI; 

internal partial class DebugManager : Control {
	public enum DebugMode { None, Performance, All, }

	public DebugMode Mode {
		get => m_Mode;
		set {
			m_Mode = value;
			
			SetActive(m_PerformanceDebugLabel, value switch {
				DebugMode.None => false,
				_ => true,
			});
			
			SetActive(m_PlayerDebugLabel, value switch {
				DebugMode.All => true,
				_ => false,
			});
		}
	}

	private static readonly StringName ToggleDebugInputName = "ToggleDebug";

	private DebugMode m_Mode;
	[Export] private Label m_PerformanceDebugLabel;
	[Export] private Label m_PlayerDebugLabel;

	public override void _Ready() {
		Mode = DebugMode.None;
	}

	public override void _Process(double dt) {
		if(Input.IsActionJustPressed(ToggleDebugInputName))
			ToggleDebug();
	}

	private static void SetActive(Label label, bool active) {
		label.Visible = active;
		label.ProcessMode = active ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

	private void ToggleDebug() => Mode = (int)m_Mode == Enum.GetValues<DebugMode>().Length - 1 ? 0 : Mode + 1;
}

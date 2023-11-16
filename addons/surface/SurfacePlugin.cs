#if TOOLS
using Godot;

namespace Game.Surface.Plugin;

[Tool]
internal partial class SurfacePlugin : EditorPlugin {
	private SurfaceInspectorPlugin m_SurfaceInspector;

	public override void _EnterTree() {
		m_SurfaceInspector = new();
		AddInspectorPlugin(m_SurfaceInspector);
	}

	public override void _ExitTree() {
		RemoveInspectorPlugin(m_SurfaceInspector);
	}
}
#endif

using System.Text;
using Godot;
using Game.Utils;

namespace Game.UI;

internal partial class PerformanceDebug : Label {
    private readonly StringBuilder m_StringBuilder = new();

    public override void _PhysicsProcess(double dt) {
        m_StringBuilder.Clear();

        m_StringBuilder.Append("OS: ").AppendLine(OS.GetDistributionName())
                       .Append("FPS: ").AppendLine(Engine.GetFramesPerSecond().ToString())
                       .Append("PPS: ").AppendLine(Engine.PhysicsTicksPerSecond.ToString())
                       .Append("RAM: ").AppendLine((OS.GetStaticMemoryUsage() * 0.001f).ToString("#.00 KB"))
                       .Append("Peak RAM: ").AppendLine((OS.GetStaticMemoryPeakUsage() * 0.001f).ToString("#.00 KB"))
                       .Append("SFX: ").AppendLine(SoundManager.PlayingSfx.ToString());

        Text = m_StringBuilder.ToString();
    }
}
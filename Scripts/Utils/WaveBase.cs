using Godot;

namespace Game.Utils;

internal abstract class WaveBase {
    public float Timer { get; set; }
    public float Offset { get; set; }
    private float m_Frequency = 0.0f;
    private float m_Phase = 0.0f;

    public void UpdateTimer(float dt) {
        Timer += dt;
    }

    public void Reset() {
        Timer = 0.0f;
        m_Phase = 0.0f;
        m_Frequency = 0.0f;
    }

    public float GetValue(float frequency) {
        if(frequency != m_Frequency) {
            float curr = (Timer * m_Frequency + m_Phase + Offset) % MathUtils.Pi2;
            float next = (Timer * frequency + Offset) % MathUtils.Pi2;
            m_Phase = curr - next;
            m_Frequency = frequency;
        }

        return TrigFunc(Timer * m_Frequency + m_Phase + Offset);
    } 

    protected abstract float TrigFunc(float x);
}

internal class SineWaveBase : WaveBase { protected override float TrigFunc(float x) => Mathf.Sin(x); }
internal class CosineWaveBase : WaveBase { protected override float TrigFunc(float x) => Mathf.Cos(x); }
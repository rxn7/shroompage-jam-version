using Godot;

namespace Game;

internal partial class BarUI : ColorRect {
	protected ColorRect m_FillRect;
	protected Label m_Label;

	public virtual string Text => "Example";
	public virtual Color FillColor => Colors.Magenta;

	public override void _Ready() {
		m_FillRect = GetNode<ColorRect>("Fill");
		m_FillRect.Color = FillColor;
		m_Label = GetNode<Label>("Label");
		m_Label.Text = Text;
	}

	public void SetFill(float percentage) {
		Vector2 size = m_FillRect.Size;
		size.X = Size.X * Mathf.Clamp(percentage, 0.0f, 1.0f);
		m_FillRect.Size = size;
	}
}

#if TOOLS
using Godot;
using System;

namespace Game.Surface.Plugin;

internal partial class SurfaceInspectorPlugin : EditorInspectorPlugin {
	public override bool _CanHandle(GodotObject obj) =>  obj is StaticBody3D;
	private UndoRedo m_ChangeSurfaceUndoRedo = new();

	public override void _ParseBegin(GodotObject obj) {
		OptionButton surfaceOption = new OptionButton();
		foreach(ESurfaceMaterial mat in (ESurfaceMaterial[])Enum.GetValues(typeof(ESurfaceMaterial)))
			surfaceOption.AddItem(mat.ToString());

		if(obj.HasMeta("Surface")) {
			string surfaceMaterialStr = obj.GetMeta("Surface").ToString();

			if(!Enum.TryParse<ESurfaceMaterial>(surfaceMaterialStr, out ESurfaceMaterial mat))
				mat = ESurfaceMaterial.None;

			surfaceOption.Select((int)mat);
		}

		surfaceOption.ItemSelected += (long idx) => {
			m_ChangeSurfaceUndoRedo.CreateAction("Change the surface");
			m_ChangeSurfaceUndoRedo.CommitAction();

            ESurfaceMaterial surface = (ESurfaceMaterial)idx;
            if(surface == ESurfaceMaterial.None)
                return;

			string surfaceName = Enum.GetName<ESurfaceMaterial>(surface);
			obj.SetMeta("Surface", surfaceName);

			StaticBody3D body = (StaticBody3D)obj;
			body.Name = $"{surfaceName}Tile";
			body.GetParent().SetEditableInstance(body, true);
			foreach(Node child in body.GetChildren()) {
				if (child is MeshInstance3D meshInstance) {
					meshInstance.MaterialOverride = GD.Load<Material>($"res://Materials/Surfaces/{surfaceName}.tres");
					break;
				}
			}
		};

		AddCustomControl(surfaceOption);
	}
}
#endif

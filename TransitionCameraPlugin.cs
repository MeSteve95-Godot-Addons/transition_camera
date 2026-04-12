#if TOOLS
using Godot;

[Tool]
public partial class TransitionCameraPlugin : EditorPlugin
{
	private const string TRANSITION_CAMERA_AUTOLOAD_NAME = "TransitionCamera";

	public override void _EnablePlugin()
	{
		AddAutoloadSingleton(TRANSITION_CAMERA_AUTOLOAD_NAME, "res://addons/transition_camera/TransitionCamera.cs");
	}

	public override void _DisablePlugin()
	{
		RemoveAutoloadSingleton(TRANSITION_CAMERA_AUTOLOAD_NAME);
	}
}
#endif

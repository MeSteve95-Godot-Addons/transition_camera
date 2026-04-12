using Godot;
using System;

public partial class CameraTransitionData : RefCounted
{
	[Signal] public delegate void TransitionCompleteEventHandler();

	public Camera3D StartCamera { get; private set; }
	public Transform3D StartGlobalTransform { get; private set; }
	public float StartFov { get; private set; }
	
	public Camera3D EndCamera { get; private set; }
	public Transform3D EndGlobalTransform { get; private set; }
	public float EndFov { get; private set; }
	
	public Action TransitionCompleteCallback { get; private set; }

	public float TransitionSpeed { get; private set; }
	public float Distance { get; private set; }
	public float Progress { get; private set; }

	public float StepMultiplier => Distance * TransitionSpeed;

	public CameraTransitionData() {}
	public CameraTransitionData(
		Camera3D startCamera,
		Camera3D endCamera,
		float transitionSpeed,
		float startProgress = 0.0f,
		Action transitionCompleteCallback = null
	)
	{
		if (startProgress is < 0.0f or > 1.0f)
			throw new ArgumentOutOfRangeException(nameof(transitionCompleteCallback));
		
		StartCamera = startCamera;
		StartGlobalTransform = startCamera.GlobalTransform;
		StartFov = startCamera.Fov;
		EndCamera = endCamera;
		EndGlobalTransform = endCamera.GlobalTransform;
		EndFov = endCamera.Fov;
		
		TransitionSpeed = transitionSpeed;
		TransitionCompleteCallback = transitionCompleteCallback;

		Distance = startCamera.GlobalPosition.DistanceTo(endCamera.GlobalPosition);
		Progress = startProgress;
	}

	public void PerformTransitionStep(Camera3D transitionCamera, float delta)
	{
		float step = delta * StepMultiplier;
		Progress = Mathf.Clamp(Progress + step, 0, 1);

		transitionCamera.GlobalTransform = StartGlobalTransform.InterpolateWith(EndGlobalTransform, Progress);
		transitionCamera.Fov = Mathf.Lerp(StartFov, EndFov, Progress);

		if (Mathf.Abs(Progress - 1) < 0.0001f)
			EndTransition();
	}

	private void EndTransition()
	{
		EndCamera.MakeCurrent();
		TransitionCompleteCallback?.Invoke();
		EmitSignal(SignalName.TransitionComplete);
	}
}

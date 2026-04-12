using Godot;
using System;

public partial class TransitionCamera : Camera3D
{
	public static TransitionCamera Instance { get; private set; }

	public CameraTransitionData TransitionData { get; private set; }

	public TransitionCamera()
	{
		if (Instance is not null)
			throw new Exception("Multiple instances of singleton.");

		Instance = this;
	}

	public override void _PhysicsProcess(double delta)
	{
		TransitionData?.PerformTransitionStep(this, (float)delta);
	}

	public void TransitionTo(
		Camera3D targetCamera,
		float transitionSpeed = 1.0f,
		Action transitionCompleteCallback = null
	)
	{
		Camera3D startCamera = GetViewport().GetCamera3D();
		float startProgress = 0.0f;
		if (TransitionData is not null)
		{
			startCamera = TransitionData.EndCamera;
			startProgress = 1 - TransitionData.Progress;
			ClearTransitionData();
		}

		if (targetCamera is null)
			throw new NullReferenceException(nameof(targetCamera));

		if (transitionSpeed <= 0.0f)
			throw new ArgumentException("Transition speed must be greater than zero.", nameof(transitionSpeed));

		TransitionData = new CameraTransitionData(
			startCamera,
			targetCamera,
			transitionSpeed,
			startProgress,
			transitionCompleteCallback
		);
		TransitionData.TransitionComplete += OnTransitionComplete;
		
		// Initialize camera to prevent flicker before first frame of transition.
		TransitionData.PerformTransitionStep(this, 0);
		MakeCurrent();
	}

	private void OnTransitionComplete()
	{
		ClearTransitionData();
	}

	private void ClearTransitionData()
	{
		TransitionData.TransitionComplete -= ClearTransitionData;
		TransitionData = null;
	}
}

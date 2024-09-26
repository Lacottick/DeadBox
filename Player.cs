using Godot;
using System;

public partial class Player : CharacterBody3D
{
    public Camera3D cam;
    
    public Gun gun;
    public float speed = 10.0f;
    private Vector2 camDir = Vector2.Zero;
	public Vector2 CameraDirection;
    public float Sensitivity = 50.0f;

    public override void _Ready()
    {
        cam = GetNode<Camera3D>("Camera3D");
        gun = cam.GetNode<Gun>("Gun");
    }
﻿    public override void _Process(double delta)
    {
        UpdateMove(delta);
        if (Input.IsActionPressed("ui_cancel"))
        {
            Input.SetMouseMode(Input.MouseModeEnum.Visible);
        }
        else
        {
            Input.SetMouseMode(Input.MouseModeEnum.Captured);
        }

        if (Input.IsActionPressed("shoot"))
        {
            Dispare();
        }
    }
	public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseMotion && 
            Input.GetMouseMode() == Input.MouseModeEnum.Captured)
        {
			CameraDirection = (@event as InputEventMouseMotion).Relative;
            UpdateRotation();
		}
        else if(@event is InputEventKey && Input.IsKeyPressed(Key.R)){
            Reload();
        }
    }
    private void Dispare()
    {
        gun?.Dispare();
    }
    private void Reload()
    {
        gun?.Reload();
    }
    private void UpdateMove(double delta)
    {
        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        
        Vector3 dir = GlobalTransform.Basis.Z * input.Y + GlobalTransform.Basis.X * input.X;
        dir = dir.Normalized();

        Vector3 velocity = dir * speed;
		Velocity = velocity;
        MoveAndSlide();
    }

    private void UpdateRotation()
    {
        if (Input.GetMouseMode() != Input.MouseModeEnum.Captured)
            return;

        camDir -= CameraDirection * Sensitivity / 1000;
        camDir.Y = Mathf.Clamp(camDir.Y, -90, 90);

        RotationDegrees = new Vector3(0, camDir.X, 0);
        cam.RotationDegrees = new Vector3(camDir.Y, 0, 0);
    }

}
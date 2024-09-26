using Godot;
using System;

public partial class Gun : MeshInstance3D
{

    [Export(PropertyHint.Range, "1,20,0.5")]
    public float FireRate = 5f;
    
	public Timer FireRateTimer;
    
	private bool CanDispare {
        get {
            return FireRateTimer.IsStopped() && !_reloading;
        }
    }

    [ExportGroup("BulletConfifg")]
	[Export]
    public PackedScene bullet;

	[Export]
    public float ForceDispare = 25.0f;

    [Export]
    public int MagSize = 30;
    private int _ammo;
    public int Ammo {
        get {
            return _ammo;
        }
        set {
            _ammo = value;
            if(value == 0){
                Reload();
            }
            UpdateUI();
        }
    }
    
    private bool _reloading = false;

    private Transform3D _baseTransform;

    public AudioStreamPlayer3D SoundPlayer;

	public override void _Ready()
	{
        FireRateTimer = new()
        {
            WaitTime = 0.00000001
        };
        FireRateTimer.Timeout += FireRateTimer_Timeout;
        AddChild(FireRateTimer);
        Ammo = MagSize;
        SoundPlayer = GetNode<AudioStreamPlayer3D>("SoundPlayer");
        _baseTransform = Transform;
	}

	public void Dispare(){
		if(CanDispare){
            CreateBullet();
            HandleShootAnimation();
            FireRateTimer.Start();
            SoundPlayer.Play();
            Ammo--;
        }
	}

    private void CreateBullet(){
        var tiro = bullet.Instantiate<RigidBody3D>();
        Timer timer = new();
        timer.Timeout += tiro.QueueFree;
        tiro.AddChild(timer);
        GetParent().GetParent().GetParent().AddChild(tiro);
        tiro.GlobalTransform = GlobalTransform;
        tiro.GlobalPosition -= GlobalTransform.Basis.Z;
        tiro.ApplyCentralImpulse(-GlobalTransform.Basis.Z * ForceDispare);
        timer.Start(5.0);
    }

    public void HandleShootAnimation(){
        Transform3D newTransform = _baseTransform;
        using(Tween tween = CreateTween()){
					tween.SetParallel(true);
					tween.SetTrans(Tween.TransitionType.Sine);
					tween.SetEase(Tween.EaseType.Out);
					tween.TweenProperty(
						this,
						"transform",
                        newTransform.Translated(new Vector3(0, 0.5f, 0.25f)),
						0.25
					);
        };
        using(Tween tween = CreateTween()){
					tween.SetParallel(true);
					tween.SetTrans(Tween.TransitionType.Sine);
					tween.SetEase(Tween.EaseType.Out);
					tween.TweenProperty(
						this,
						"transform",
                        _baseTransform,
						0.5
					);
        };
    }
    public void Reload(){
        if(!_reloading){
            _reloading = true;
            Timer reloadTimer = new(){
                WaitTime = 1.5,
            };
            var meshMaterial = Mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
            meshMaterial.AlbedoColor = Colors.Yellow;
            reloadTimer.Timeout += ()=>{
                Ammo = MagSize;
                meshMaterial.AlbedoColor = Colors.Green;
                _reloading = false;
                reloadTimer.QueueFree();
            };
            AddChild(reloadTimer);
            reloadTimer.Start();
            HandleReloadAnimation();    
        }
    }

    private void HandleReloadAnimation(){
        using(Tween tween = CreateTween()){
            tween.SetParallel(false);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(
                this,
                "transform",
                Transform
                .Translated(new Vector3(0,-0.25f, 0))
                .Rotated(Vector3.Left, Mathf.Pi/4)
                .Rotated(Vector3.Up, Mathf.Pi/2),
                0.75
            );
        };
        /* using(Tween tween = CreateTween()){
            tween.SetParallel(false);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(
                this,
                "transform",
                _baseTransform,
                0.75
            );
        }; */
    }

    private void UpdateUI(){
        Node uiLayer = GetTree().GetFirstNodeInGroup("ui");
        Label ammoLabel = uiLayer.GetNode<Label>("%AmmoLabel");
        ammoLabel.Text = $"{Ammo}/{MagSize}"; 
    }

    public void FireRateTimer_Timeout(){
        FireRateTimer.Stop();
    }
}

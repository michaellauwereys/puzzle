using Sandbox;
using System;

partial class PuzzlePlayer : Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;
	private DamageInfo lastDamage;

	[Net] public PawnController VehicleController { get; set; }
	[Net] public PawnAnimator VehicleAnimator { get; set; }
	[Net, Predicted] public ICamera VehicleCamera { get; set; }
	[Net, Predicted] public Entity Vehicle { get; set; }
	[Net, Predicted] public ICamera MainCamera { get; set; }

	public ICamera LastCamera { get; set; }

	public bool cpSet = false;
	public float[] cpPosition;

	public static int countdown = 0;

	public static int Countdown
	{
		get { return countdown; }
		set { countdown = value; }

	}

	/****************************************************************************
	 * PuzzlePlayer
	 ***************************************************************************/
	public PuzzlePlayer()
	{
		Inventory = new Inventory( this );
	}

	/****************************************************************************
	 * Spawn
	 ***************************************************************************/
	public override void Spawn()
	{
		MainCamera = new FirstPersonCamera();
		LastCamera = MainCamera;

		base.Spawn();
	}

	/****************************************************************************
	 * Respwn
	 ***************************************************************************/
	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController{
			SprintSpeed = 250.0f, // 320.0f
			WalkSpeed = 180.0f, // 150.0f
			DefaultSpeed = 180.0f, // 190.0f
			Acceleration = 10.0f, // 10.0f
			AirAcceleration = 100.0f, // 50.0f
			FallSoundZ = -30.0f, // -30.0f
			GroundFriction = 4.0f, // 4.0f
			StopSpeed = 100.0f, // 100.0f
			Size = 20.0f, // 20.0f
			DistEpsilon = 0.03125f, // 0.03125f
			GroundAngle = 46.0f, // 46.0f
			Bounce = 0.0f, // 0.0f
			MoveFriction = 1.0f, // 1.0f
			StepSize = 18.0f, // 18.0f
			MaxNonJumpVelocity = 140.0f, // 140.0f
			BodyGirth = 32.0f, // 32.0f
			BodyHeight = 72.0f, // 72.0f
			EyeHeight = 64.0f, // 64.0f
			Gravity = 800.0f, // 800.0f
			AirControl = 30.0f, // 30.0f
		};

		Animator = new StandardPlayerAnimator();

		MainCamera = LastCamera;
		Camera = MainCamera;

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Dress();

		Inventory.Add( new GravGun(), true );
		Inventory.Add( new Flashlight() );

		base.Respawn();

		// If the player has a checkpoint set = teleport the player
		if ( cpSet == true )
		{
			Velocity = Vector3.Zero;
			Position = new Vector3( cpPosition[0], cpPosition[1], cpPosition[2] );
			EyeRot = Rotation.From( cpPosition[3], cpPosition[4], cpPosition[5] );

			Log.Warning( $"Teleported to: {cpPosition[0]} {cpPosition[1]} {cpPosition[2]} {cpPosition[3]} {cpPosition[4]} {cpPosition[5]}" );
		}
	}

	/****************************************************************************
	 * OnKilled
	 ***************************************************************************/
	public override void OnKilled()
	{
		base.OnKilled();

		PlaySound( "kersplat" );

		if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
		{
			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
		}

		VehicleController = null;
		VehicleAnimator = null;
		VehicleCamera = null;
		Vehicle = null;

		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
		LastCamera = MainCamera;
		MainCamera = new SpectateRagdollCamera();
		Camera = MainCamera;
		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		Inventory.DropActive();
		Inventory.DeleteContents();
	}

	/****************************************************************************
	 * TakeDamage
	 ***************************************************************************/
	public override void TakeDamage( DamageInfo info )
	{
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage *= 10.0f;
		}

		lastDamage = info;

		TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

		base.TakeDamage( info );
	}

	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	public override PawnController GetActiveController()
	{
		if ( VehicleController != null ) return VehicleController;
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}

	public override PawnAnimator GetActiveAnimator()
	{
		if ( VehicleAnimator != null ) return VehicleAnimator;

		return base.GetActiveAnimator();
	}

	public ICamera GetActiveCamera()
	{
		if ( VehicleCamera != null ) return VehicleCamera;

		return MainCamera;
	}

	/****************************************************************************
	 * Simulate
	 ***************************************************************************/
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		// Checkpoint
		var pos = Position;
		var ang = EyeRot.Angles();

		if ( countdown > 0 )
		{
			countdown--;
		}

		// Save
		if ( Input.Pressed( InputButton.Slot1 ) )
		{
			if ( countdown == 0 )
			{
				cpPosition = new[] { pos.x, pos.y, pos.z, ang.pitch, ang.yaw, ang.roll };
				Log.Warning( $"Checkpoint saved: {pos.x} {pos.y} {pos.z} {ang.pitch} {ang.yaw} {ang.roll}" );
				PlaySound( "cp-save" );
				cpSet = true;
				countdown = 5 * 3600; 
			}
			else
			{
				PlaySound( "cp-error" );
				Log.Error( "5 min cooldown active!" );
			}
		}

		// Teleport
		if ( Input.Pressed( InputButton.Slot2 ) )
		{
			// Check if player has a checkpoint set
			if ( cpSet == true )
			{
				Velocity = Vector3.Zero;
				Position = new Vector3( cpPosition[0], cpPosition[1], cpPosition[2] );
				EyeRot = Rotation.From( cpPosition[3], cpPosition[4], cpPosition[5] );

				Log.Warning( $"Teleported to: {cpPosition[0]} {cpPosition[1]} {cpPosition[2]} {cpPosition[3]} {cpPosition[4]} {cpPosition[5]}" );
				PlaySound( "cp-teleport" );
			}
		}

		// Remove
		if ( Input.Pressed( InputButton.Slot9 ) )
		{
			cpPosition = Array.Empty<float>();
			Log.Warning( $"Checkpoint removed" );
			PlaySound( "cp-remove" );
			cpSet = false;
		}

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		if ( VehicleController != null && DevController is NoclipController )
		{
			DevController = null;
		}

		var controller = GetActiveController();
		if ( controller != null )
			EnableSolidCollisions = !controller.HasTag( "noclip" );

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( MainCamera is not FirstPersonCamera )
			{
				MainCamera = new FirstPersonCamera();
			}
			else
			{
				MainCamera = new ThirdPersonCamera();
			}
		}
	
		Camera = GetActiveCamera();

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRot.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				Game.Current?.DoPlayerNoclip( cl );
			}

			timeSinceJumpReleased = 0;
		}

		if ( Input.Left != 0 || Input.Forward != 0 )
		{
			timeSinceJumpReleased = 1;
		}
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
	}

	[ServerCmd( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( !slot.ClassInfo.IsNamed( entName ) )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}
}

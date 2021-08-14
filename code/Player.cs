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

	public int checkpointCounter = 0;
	public float posX = 0f;
	public float posY = 0f;
	public float posZ = 0f;
	public float pitch = 0f;
	public float yaw = 0f;
	public float roll = 0f;

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
		Log.Info( "---------------------- SPAWN ----------------------" );

		MainCamera = new FirstPersonCamera();
		LastCamera = MainCamera;

		base.Spawn();
	}

	/****************************************************************************
	 * Respwn
	 ***************************************************************************/
	public override void Respawn()
	{
		Log.Info( "---------------------- RESPAWN ----------------------" );
		
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();

		MainCamera = LastCamera;
		Camera = MainCamera;

		EnableAllCollisions = false;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Dress();

		Inventory.Add( new GravGun(), true );
		Inventory.Add( new Flashlight() );

		base.Respawn();

		if ( checkpointCounter == 0 )
		{
			var player = Input.ActiveChild;
			var pos = Position;
			var ang = EyeRot.Angles();

			posX = pos.x;
			posY = pos.y;
			posZ = pos.z;
			pitch = ang.pitch;
			yaw = ang.yaw;
			roll = ang.roll;

			Log.Info( $"Position: {posX} {posY} {posZ} {pitch} {yaw} {roll}" );
			checkpointCounter = 1;
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

		// Checkpoint
		if ( Input.Pressed( InputButton.Slot1 ) )
		{
			Log.Info( "---------------------- CHECKPOINT SAVE BUTTON IS PRESSED ----------------------" );

			PlaySound( "save-cp" );

			var player = ActiveChild;
			var pos = Position;
			var ang = EyeRot.Angles();

			posX = pos.x;
			posY = pos.y;
			posZ = pos.z;
			pitch = ang.pitch;
			yaw = ang.yaw;
			roll = ang.roll;

			Log.Info( $"Position: {posX} {posY} {posZ} {pitch} {yaw} {roll}" );
		}

		if ( Input.Pressed( InputButton.Slot2 ) )
		{
			Log.Info( "---------------------- CHECKPOINT GO BUTTON IS PRESSED ----------------------" );

			PlaySound( "go-cp" );

			Velocity = Vector3.Zero;

			posX = posX;

			Position = new Vector3( posX, posY, posZ );
			EyeRot = Rotation.From( pitch, yaw, roll );

			Log.Info( $"Position: {posX} {posY} {posZ} {pitch} {yaw} {roll}" );
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
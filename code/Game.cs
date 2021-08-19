using Sandbox;

[Library( "puzzle", Title = "Puzzle" )]
partial class PuzzleGame : Game
{
	public PuzzleGame()
	{
		if ( IsServer )
		{
			// new PuzzleHud();
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new PuzzlePlayer();
		player.Respawn();

		cl.Pawn = player;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
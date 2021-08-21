using Sandbox;
using Sandbox.UI;

[Library("puzzle_total_levels", Description = "Define the total amount of levels in the map")]
[Hammer.EntityTool( "Total levels", "Puzzle", "Define the total amount of levels in the map" )]
public partial class Testing : Entity
{
  [Property( Title = "Total levels" ) ]
  [Hammer.MinMax( 1, 50 )]
  public int TotalLevels { get; set; } = 1;

  [Event( "mygame.gameover" )]
  protected virtual void TotalLevelss()
  {
    Log.Warning( $"Total levels: {TotalLevels}" );
  }
}

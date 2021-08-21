using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class TotalLevels : Panel
{
	public Panel LevelBox;
	public Label LevelNumber;

	public TotalLevels()
	{
		Panel totalLevelsBox = Add.Panel( "totalLevelsBox" );

		for ( int i = 1; i < 13; i++ )
		{
			Panel levelBox = totalLevelsBox.Add.Panel( "levelBox" );
			LevelNumber = levelBox.Add.Label( i.ToString(), "levelNumber" );
		}
	}

	//[Event( "mygame.gameover" )]
	//protected virtual void Testing()
	//{
	//	Log.Warning( $"Total levels: {TotalLevels}" );
	//	ChatBox.Say( "TEST" );
	//}
}

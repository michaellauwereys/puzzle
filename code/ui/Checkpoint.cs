using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Checkpoint : Panel
{
	public Panel CpBar;
	public Label CpValue;

	public Checkpoint()
	{
		Panel cpBox = Add.Panel( "cpBox" );
		Panel cpBar = cpBox.Add.Panel( "cpBar" );

		CpBar = cpBar.Add.Panel( "cpBar" );
		CpValue = cpBox.Add.Label( "0", "cpValue" );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn;

		int countdown = PuzzlePlayer.Countdown;

		if ( countdown == 0 )
		{
			CpValue.Text = "5:00";
		}
		else
		{
			CpValue.Text = countdown.ToString();
		}

		CpBar.Style.Dirty();
		CpBar.Style.Width = Length.Percent( 100 );
	}
}

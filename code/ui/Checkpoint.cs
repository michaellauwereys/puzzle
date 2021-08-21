using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

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
		float cdPercentage = (countdown / 18000f) * 100f;

		if ( countdown == 0 )
		{
			CpValue.Text = "5:00";
			CpBar.Style.Width = Length.Percent( 100 );
		}
		else
		{
			TimeSpan time = TimeSpan.FromSeconds( countdown );
			string str = time.ToString( @"hh\:mm" );

			CpValue.Text = str;

			CpBar.Style.Dirty();
			CpBar.Style.Width = Length.Percent( cdPercentage );
		}
	}
}

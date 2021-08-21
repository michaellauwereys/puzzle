using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{
	public Panel HealthBar;
	public Label HealthValue;

	public Health()
	{
		Panel healthBox = Add.Panel( "healthBox" );
		Panel healthBar = healthBox.Add.Panel( "healthBar" );

		HealthBar = healthBar.Add.Panel( "healthBar" );
		HealthValue = healthBox.Add.Label( "0", "healthValue" );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn;

		HealthValue.Text = $"{player.Health.CeilToInt()}";

		HealthBar.Style.Dirty();
		HealthBar.Style.Width = Length.Percent( player.Health );
	}
}

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Message : Panel
{
	public Panel TextBox;
	public Label TextBoxText;

	public Panel Options;
	public Label OptionsText;

	public Message()
	{
		TextBox = Add.Panel( "testbox" );
		TextBoxText = TextBox.Add.Label( "Features in the works:\n    - Change player speeds\n    - Three random items you can spawn\n    - Checkpoint cooldown\n    - Show when levels are finished on the hud\n    - Custom gravgun model\n    - Point system", "value" );

		Options = Add.Panel( "options" );
		OptionsText = Options.Add.Label( "Options:\n    - Set checkpoint: 1\n    - Teleport to checkpoint: 2\n    - Remove checkpoint: 9\n    - Thirdperson: c", "value" );
	}
}
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
		Options = Add.Panel( "options" );
		OptionsText = Options.Add.Label( "1 - Set checkpoint\n2 - Teleport to checkpoint\n9 - Remove checkpoint\nc - Thirdperson", "value" );
	}
}
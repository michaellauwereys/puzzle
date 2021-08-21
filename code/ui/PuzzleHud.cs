using Sandbox;
using Sandbox.UI;

[Library]
public partial class PuzzleHud : HudEntity<RootPanel>
{
	public PuzzleHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/PuzzleHud.scss" );

		RootPanel.AddChild<NameTags>();

		RootPanel.AddChild<CrosshairCanvas>();

		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();

		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();

		RootPanel.AddChild<TotalLevels>();

		RootPanel.AddChild<Checkpoint>();
		RootPanel.AddChild<Health>();

		RootPanel.AddChild<Message>();

		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<CurrentTool>();
	}
}

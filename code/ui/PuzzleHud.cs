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
		// RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();

		RootPanel.AddChild<Checkpoint>();
		RootPanel.AddChild<HealthBox>();

		RootPanel.AddChild<Message>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<CurrentTool>();
	}
}
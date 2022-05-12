using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace HICIG 
{
	public partial class TeamSelectScreen : Panel 
	{
		private bool IsOpen = false;
		private TimeSince TimeSinceLastOpened;

		private Label BlebsBtn;
		private Label RudsBtn;

		public TeamSelectScreen() 
		{
			StyleSheet.Load("/ui/screens/TeamSelectScreen.scss");

			Panel BlebsPnl = Add.Panel("BlebsTeamMenu");
			Panel RudsPnl = Add.Panel("RudsTeamMenu");

			BlebsBtn = BlebsPnl.Add.Label("Join the Blebs Team", "BlebsBtn");
			BlebsBtn.AddEventListener("onclick", () => 
			{
				ConsoleSystem.Run("HICIG_SetTeam", HICIGPlayer.TeamList.Blebs);

				IsOpen = false;
			});

			RudsBtn = RudsPnl.Add.Label("Join the Ruds Team", "RudsBtn");
			RudsBtn.AddEventListener("onclick", () => 
			{
				ConsoleSystem.Run("HICIG_SetTeam", HICIGPlayer.TeamList.Ruds);

				IsOpen = false;
			});
		}

		public override void Tick()
		{
			base.Tick();

			if (Input.Pressed(InputButton.Menu) && TimeSinceLastOpened >= 0.1f) 
			{
				IsOpen = !IsOpen;
				TimeSinceLastOpened = 0.0f;
			}

			SetClass("open", IsOpen);
		}
	}
}

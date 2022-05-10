using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace HICIG 
{
	public enum TeamList 
	{
		Unassigned = 0,
		Ruds,
		Blebs
	}

	partial class HICIGPlayer 
	{
		public TeamList CurrTeam;
	
		private TimeSince TimeSinceSwitchedTeam;

		public void SetTeam(HICIGPlayer player, TeamList newTeam) 
		{
			if (CurrTeam != TeamList.Unassigned) return;

			if (CurrTeam == newTeam) 
			{
				using (Prediction.Off())
					ChatBox.AddInformation(To.Single(this), $"You are already on the {newTeam} team.");

				return;
			}

			if ((newTeam == TeamList.Ruds && GetRudsMembers().Count > GetBlebsMembers().Count) 
				|| (newTeam == TeamList.Blebs && GetBlebsMembers().Count > GetRudsMembers().Count))
			{
				using (Prediction.Off())
					ChatBox.AddInformation(To.Single(this), $"Please select a different team, {newTeam} is currently larger than the other.");

				return;
			}

			if (TimeSinceSwitchedTeam < 15.0f) 
			{
				using (Prediction.Off())
					ChatBox.AddInformation(To.Single(this), $"Please wait {MathF.Round(15.0f - TimeSinceSwitchedTeam, 1)}s before switching to the {newTeam} team.");

				return;
			}

			CurrTeam = newTeam;

			using (Prediction.Off()) 
			{
				UpdateTeamClient(To.Single(this), newTeam);
				ChatBox.AddInformation(To.Everyone, $"{player.Client.Name} has joined the {newTeam} team.", $"avatar: {player.Client.PlayerId}");
			}

			TimeSinceSwitchedTeam = 0.0f;

			OnKilled();

			if (HICIGGame.CanStartGame())
				Event.Run("HICIG_BeginGame");
		}

		[ServerCmd("HICIG_SetTeam")]
		public static void SetTeamClientToServer(TeamList newTeam) 
		{
			var user = ConsoleSystem.Caller;

			if (user.Pawn is HICIGPlayer player)
				player.SetTeam(player, newTeam);
		}

		public static List<HICIGPlayer> GetBlebsMembers() 
		{
			List<HICIGPlayer> curBlebsMembers = new();

			foreach (var client in Client.All) 
			{
				if (client.Pawn is HICIGPlayer player) 
				{
					if (player.CurrTeam == TeamList.Blebs)
						curBlebsMembers.Add(player);
				}
			}

			return curBlebsMembers;
		}

		public static List<HICIGPlayer> GetRudsMembers() 
		{
			List<HICIGPlayer> curRudsMembers = new();

			foreach (var client in Client.All) 
			{
				if (client.Pawn is HICIGPlayer player) 
				{
					if (player.CurrTeam == TeamList.Ruds)
						curRudsMembers.Add(player);
				}
			}

			return curRudsMembers;
		}

		[ClientRpc]
		private void UpdateTeamClient(TeamList newTeamClient) 
		{
			CurrTeam = newTeamClient;
		}
	}
}

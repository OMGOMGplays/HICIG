using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace HICIG 
{
	public partial class HICIGGame 
	{
		public static TimeSince TimeToBegin;
		public static TimeSince TimeToEnd;
		
		[Net]
		public static bool RudsWinner {get; set;} = false;

		[Net]
		public static bool BlebsWinner {get; set;} = false;

		[Net]
		public static int WinGoal {get;} = 3;

		[Net]
		public static int RudsTotalCaps {get; set;} = 0;

		[Net]
		public static int BlebsTotalCaps {get; set;} = 0;

		public enum GameStatus 
		{
			Idle,
			Start,
			Active,
			Post
		}

		public static GameStatus CurrGameStatus;

		public static bool CanStartGame() 
		{
			if (HICIGPlayer.GetRudsMembers().Count >= 1 && HICIGPlayer.GetBlebsMembers().Count >= 1)
				return true;

			return false;
		}

		[Event("HICIG_StopGame")]
		public void StopGame() 
		{
			if (CurrGameStatus == GameStatus.Idle)
				return;

			CurrGameStatus = GameStatus.Idle;

			foreach (var client in Client.All) 
			{
				if (client.Pawn is HICIGPlayer player) 
				{
					player.LockControls = false;
				}
			}

			foreach (var oldSkel in All.OfType<SkelHead>())
				oldSkel.Delete();

			RudsTotalCaps = 0;
			BlebsTotalCaps = 0;

			using (Prediction.Off()) 
			{
				ChatBox.AddInformation(To.Everyone, "Game Stopped.");
				UpdateGameStateClient(To.Everyone, GameStatus.Idle);

				UpdateTeamScoreClient(To.Everyone, TeamList.Ruds, RudsTotalCaps);
				UpdateTeamScoreClient(To.Everyone, TeamList.Blebs, BlebsTotalCaps);
			}
		}

		[ServerCmd("HICIG_AdminStartGame")]
		public static void StartGameCMD() 
		{
			Event.Run("HICIG_BeginGame");
		}

		[ServerCmd("HICIG_AdminStopGame")]
		public static void StopGameCMD() 
		{
			Event.Run("HICIG_StopGame");
		}

		public static bool ShouldStopGame() 
		{
			if (HICIGPlayer.GetBlebsMembers().Count < 1 || HICIGPlayer.GetRudsMembers().Count < 1)
				return true;

			return false;
		}

		[Event("HICIG_BeginGame")]
		public void StartGame() 
		{
			if (CurrGameStatus != GameStatus.Idle) return;

			RudsTotalCaps = 0;
			BlebsTotalCaps = 0;

			CurrGameStatus = GameStatus.Start;

			TimeToBegin = 0;

			foreach (var client in Client.All) 
			{
				if (client.Pawn is HICIGPlayer player) 
				{
					player.Respawn();
					player.LockControls = true;
				}
			}

			foreach (var oldSkel in All.OfType<SkelHead>())
				oldSkel.Delete();

			// foreach (var point in All.OfType<SkelHeadSpawnpoint>()) 
			// {
			// 	if (point.IsFromMap) 
			// 	{
			// 		spawnPoints.Add(point);
			// 	}
			// }

			using (Prediction.Off()) 
			{
				ChatBox.AddInformation(To.Everyone, "Game is now starting.");
				UpdateGameStateClient(To.Everyone, CurrGameStatus);

				SetTeamScoreClient(To.Everyone, TeamList.Ruds, 0);
				SetTeamScoreClient(To.Everyone, TeamList.Blebs, 0);
			}
		}

		[Event.Tick.Server]
		public void ServerTick() 
		{
			if (CurrGameStatus == GameStatus.Start && TimeToBegin > 10) 
			{
				foreach (var client in Client.All) 
				{
					if (client.Pawn is HICIGPlayer player)
						player.LockControls = false;
				}

				CurrGameStatus = GameStatus.Active;

				using (Prediction.Off())
					UpdateGameStateClient(To.Everyone, CurrGameStatus);
			}

			// if (CurrGameStatus == GameStatus.Active && TimeSinceSkelCapped >= 5.0f) 
			// {
				// TimeSinceSkelSpawned = 0;

				// var skelNew = new SkelHead();
				// skelNew.Position = skelTeamSpawn;
			// } Uncomment whenevr brain work

			if (CurrGameStatus == GameStatus.Post && TimeToEnd >= 10.0f) 
			{
				CurrGameStatus = GameStatus.Start;

				foreach (var client in Client.All) 
				{
					if (client.Pawn is HICIGPlayer player)
						player.Respawn();
				}

				using (Prediction.Off()) 
				{
					SetTeamScoreClient(To.Everyone, TeamList.Ruds, 0);
					SetTeamScoreClient(To.Everyone, TeamList.Blebs, 0);
				}
			}
		}

		[ClientRpc]
		private void UpdateGameStateClient(GameStatus status) 
		{
			CurrGameStatus = status;
		}

		[Event("HICIG_EndGame")]
		public void EndGame() 
		{
			RudsTotalCaps = 0;
			BlebsTotalCaps = 0;

			TimeToEnd = 0;
			CurrGameStatus = GameStatus.Post;

			using (Prediction.Off())
				UpdateGameStateClient(To.Everyone, CurrGameStatus);
			foreach (var oldSkel in All.OfType<SkelHead>())
				oldSkel.Delete();
		}

		public static void DeclareWinner(TeamList winningTeam) 
		{
			Event.Run("HICIG_EndGame");

			if (winningTeam == TeamList.Ruds)
				using (Prediction.Off())
					SetWinnerClient(To.Everyone, true, false);

			else if (winningTeam == TeamList.Blebs)
				using (Prediction.Off())
					SetWinnerClient(To.Everyone, false, true);

			foreach (var client in Client.All) 
			{
				if (client.Pawn is HICIGPlayer player) 
				{
					player.LockControls = true;
				}
			}
		}

		[ClientRpc]
		public static void SetWinnerClient(bool rudsWon, bool blebsWon) 
		{
			RudsWinner = rudsWon;
			BlebsWinner = blebsWon;
		}

		[ClientRpc]
		public static void SetTeamScoreClient(TeamList teamUpdate, int amount) 
		{
			if (teamUpdate == TeamList.Ruds)
				RudsTotalCaps = amount;
			else if (teamUpdate == TeamList.Blebs)
				BlebsTotalCaps = amount;
			else 
			{
				RudsTotalCaps = amount;
				BlebsTotalCaps = amount;
			}
		}

		[ClientRpc]
		public static void UpdateTeamScoreClient(TeamList teamUpdate, int newAmount) 
		{
			if (teamUpdate == TeamList.Ruds)
				RudsTotalCaps += newAmount;
			else if (teamUpdate == TeamList.Blebs)
				BlebsTotalCaps += newAmount;
		}
	}
}

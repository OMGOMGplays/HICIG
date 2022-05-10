using Sandbox;

namespace HICIG
{
	public partial class HICIGGame : Game
	{
		// private HICIGHud hud;

		public HICIGGame()
		{
			if (IsServer) 
			{
			}

			if (IsClient) 
			{
				// hud = new HICIGHud();
			}
		}

		[Event.Hotload]
		public void UpdateHud() 
		{
			if (!IsClient) return;

			// hud?.Delete();
			// hud = new HICIGHud();
		}

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var player = new HICIGPlayer();
			player.Spawn();

			cl.Pawn = player;
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );

			if (ShouldStopGame())
				StopGame();
		}
	}
}

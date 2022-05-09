using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HICIG
{
	public partial class HICIGGame : Game
	{
		public HICIGGame()
		{
			if (IsServer) 
			{
				// _ = new HICIGHud();
			}
		}

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var player = new HICIGPlayer();
			player.Respawn();

			cl.Pawn = player;
		}
	}

}

using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace HICIG 
{
	public partial class HICIGPlayer : Player 
	{
		public bool LockControls = false;

		public bool IsHoldingSkel = false;

		public TimeSince TimeSinceKilled;

		public Clothing.Container Clothing = new();

		public void SetPositionSpawn() 
		{
			if (CurrTeam != TeamList.Unassigned) 
			{
				if (CurrTeam == TeamList.Blebs) 
				{
					List<BlebsSpawnpoint> spawnpoints = new List<BlebsSpawnpoint>();

					foreach (var blebPoint in All.OfType<BlebsSpawnpoint>())
						spawnpoints.Add(blebPoint);

					int checkedIndex = 0;
					int randomIndex = Rand.Int(checkedIndex, spawnpoints.Count - 1);

					while (spawnpoints[randomIndex].Position.IsNaN && checkedIndex < spawnpoints.Count) 
					{
						checkedIndex += 1;
						randomIndex = Rand.Int(checkedIndex, spawnpoints.Count - 1);
					}

					Position = spawnpoints[randomIndex].Position;
				}
				else if (CurrTeam == TeamList.Ruds) 
				{
					List<RudsSpawnpoint> spawnpoints = new List<RudsSpawnpoint>();

					foreach (var rudsPoint in All.OfType<RudsSpawnpoint>())
						spawnpoints.Add(rudsPoint);

					int checkedIndex = 0;
					int randomIndex = Rand.Int(checkedIndex, spawnpoints.Count - 1);

					while (spawnpoints[randomIndex].Position.IsNaN && checkedIndex < spawnpoints.Count) 
					{
						checkedIndex += 1;
						randomIndex = Rand.Int(checkedIndex, spawnpoints.Count - 1);
					}

					Position = spawnpoints[randomIndex].Position;
				}
			}
		}

		public HICIGPlayer() 
		{
			Inventory = new Inventory(this);
		}

		public HICIGPlayer(Client cl) : this() 
		{
			Clothing.LoadFromClient(cl);
		}

		public void InitialSpawn() 
		{
			TimeSinceSwitchedTeam = 15.0f;
			CurrTeam = TeamList.Unassigned;
			Respawn();
		}

		public override void Respawn()
		{
			base.Respawn();

			SetModel("models/citizen/citizen.vmdl");

			Inventory.Add(new TestWeapon(), true);

			Clothing.DressEntity(this);

			Animator = new StandardPlayerAnimator();
			CameraMode = new FirstPersonCamera();
			Controller = new WalkController();

			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			EnableDrawing = true;

			SetPositionSpawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if (LockControls && IsServer)
				return;

			SimulateActiveChild(cl, ActiveChild);

			if (LifeState == LifeState.Dead) 
			{
				if (HICIGGame.CurrGameStatus == HICIGGame.GameStatus.Idle && TimeSinceKilled > 2 && IsServer) 
				{
					Respawn();
					return;
				}

				if (TimeSinceKilled > 15 && IsServer) 
				{
					Respawn();
				}

				return;
			}

			TickPlayerUse();
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );
		}

		public override void TakeDamage( DamageInfo info )
		{
			base.TakeDamage( info );
		}

		public override void OnKilled()
		{
			TimeSinceKilled = 0;
			base.OnKilled();

			// BecomeRagdollOnClient();
			CameraMode = new SpectateRagdollCamera();
			EnableDrawing = false;
			EnableAllCollisions = false;
		}
	}
}

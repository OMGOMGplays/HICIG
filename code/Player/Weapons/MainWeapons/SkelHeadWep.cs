using Sandbox;

namespace HICIG 
{
	partial class SkelHeadWep : WeaponBase 
	{
		public override string ViewModelPath => "models/skelhead/skelhead.vmdl";
		public override string WorldModelPath => "models/skelhead/skelhead.vmdl";

		public override int ClipSize => -1;

		public override void Spawn() 
		{
			AmmoClip = -1;

			base.Spawn();
		}

		public override bool CanReload()
		{
			return false;
		}

		public override void AttackPrimary()
		{
			if (MeleeAttack()) 
			{
				OnMeleeHit();
			}
			else 
			{
				OnMeleeMiss();
			}
		}

		public override void AttackSecondary()
		{
			return;
		}

		private bool MeleeAttack() 
		{
			var forward = Owner.EyeRotation.Forward;
			forward = forward.Normal;

			bool hit = false;

			foreach (var tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * 80, 20.0f)) 
			{
				if (!tr.Entity.IsValid()) continue;

				tr.Surface.DoBulletImpact(tr);

				hit = true;

				using (Prediction.Off()) 
				{
					var damageInfo = DamageInfo.FromBullet(tr.EndPosition, forward * 100, 25)
						.UsingTraceResult(tr)
						.WithAttacker(Owner)
						.WithWeapon(this);

					tr.Entity.TakeDamage(damageInfo);
				}
			}

			return hit;
		}

		[ClientRpc]
		private void OnMeleeHit() 
		{
			Host.AssertClient();

			if (IsLocalPawn) 
			{
				_ = new Sandbox.ScreenShake.Perlin(1.0f, 1.0f, 3.0f);
			}

			(Owner as AnimEntity).SetAnimParameter("b_attack", true);

			ViewModelEntity?.SetAnimParameter("attack", true);
		}

		[ClientRpc]
		private void OnMeleeMiss() 
		{
			Host.AssertClient();

			if (IsLocalPawn) 
			{
				_ = new Sandbox.ScreenShake.Perlin();
			}

			(Owner as AnimEntity).SetAnimParameter("b_attack", true);

			ViewModelEntity?.SetAnimParameter("attack", true);
		}

		public void DeleteSkel() 
		{
			var player = Local.Pawn as HICIGPlayer;

			if (player.IsHoldingSkel == false) 
			{
				Delete();

				player.Inventory.Add(new TestWeapon(), true); // Ifall klasser kommer göras, kom ihåg att ändra så att dem får de vapen dem ska ha!
			} 
		}
	}
}

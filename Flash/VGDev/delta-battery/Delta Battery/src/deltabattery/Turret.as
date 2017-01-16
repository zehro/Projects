package deltabattery 
{
	import cobaltric.ContainerGame;
	import deltabattery.weapons.ABST_Weapon;
	import deltabattery.weapons.Weapon_Chain;
	import deltabattery.weapons.Weapon_DeltaStrike;
	import deltabattery.weapons.Weapon_Flak;
	import deltabattery.weapons.Weapon_RAAM;
	import deltabattery.weapons.Weapon_SAM;
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	
	/**	Controls aspects of the player's turret
	 *
	 * @author Alexander Huynh
	 */
	public class Turret extends ABST_Base
	{
		public var cg:ContainerGame;
		public var turret:MovieClip;			// the actual Turret MovieClip
		
		public var rightMouseDown:Boolean;		// TRUE if the right mouse is currently down
		
		// array of objects extending ABST_Weapons
		public var weaponPrimary:Array = [null, null, null];
		public var weaponSecondary:Array = [null, null, null];
		public var weaponSpecial:ABST_Weapon;
		
		private var weaponMC:Array;				// array of GUI objects (weapon icons)

		public var activePrimary:int = 0;		// index in array for LEFT
		public var activeSecondary:int = 0;		// index in array for RIGHT
		
		// turret rotation limits
		public const TURRET_LIMIT_R:Number = 170;
		public const TURRET_LIMIT_L:Number = 280;
		
		private var prevX:Number = 0;
		private var prevY:Number = 0;
		
		private var noAmmoGrace:int = 0;

		public function Turret(_cg:ContainerGame, _turret:MovieClip) 
		{
			cg = _cg;
			turret = _turret;
			turret.addEventListener(Event.ADDED_TO_STAGE, init);
			
			// setup default weapons
			weaponPrimary[0] = new Weapon_SAM(cg, 0);
			weaponSecondary[0] = new Weapon_Chain(cg, 3);
			weaponSpecial = new Weapon_DeltaStrike(cg);
		}
		
		
		/**
		 * 
		 * @param	weapon		the weapon to use
		 * @param	slot		0 <= slot <= 7
		 */
		public function enableWeapon(weapon:ABST_Weapon, slot:int):void
		{
			if (slot == 7)
				weaponSpecial = weapon;
			else if (slot == 6)
				return;
			else if (slot > 2)
				weaponSecondary[slot - 3] = weapon;
			else
				weaponPrimary[slot] = weapon;
			
			weaponMC[slot].visible = true;
		}
		
		private function init(e:Event):void
		{
			turret.removeEventListener(Event.ADDED_TO_STAGE, init);
			turret.stage.addEventListener(MouseEvent.MOUSE_DOWN, onMouseLeftDown);
			turret.stage.addEventListener(MouseEvent.RIGHT_MOUSE_DOWN, onMouseRightDown);
			turret.stage.addEventListener(MouseEvent.RIGHT_MOUSE_UP, onMouseRightUp);
			turret.stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyboard);
			turret.addEventListener(Event.REMOVED_FROM_STAGE, destroy);
			
			var gui:MovieClip = cg.game.mc_gui;
			weaponMC = [gui.wep_1, gui.wep_2, gui.wep_3, gui.wep_4, gui.wep_5, gui.wep_6, gui.wep_7, gui.wep_8];
			
			// setup weapons
			weaponMC[1].gotoAndStop("raam");
			weaponMC[2].gotoAndStop("big");
			weaponMC[3].gotoAndStop("chain");
			weaponMC[4].gotoAndStop("flak");
			weaponMC[5].gotoAndStop("laser");
			weaponMC[7].gotoAndStop("delta");
			for (var i:int = 0; i < weaponMC.length; i++)
				if (weaponMC[i])
				{
					weaponMC[i].visible = false;
					weaponMC[i].selected.visible = false;
					weaponMC[i].tf_number.text = (i + 1);
					weaponMC[i].mc_noAmmo.visible = false;
				}
			weaponMC[0].visible = true;
			weaponMC[3].visible = true;
			weaponMC[7].visible = true;
			weaponMC[0].selected.visible = true;
			weaponMC[3].selected.visible = true;
			weaponMC[7].selected.visible = true;
			weaponMC[7].tf_number.text = "S";

			cg.game.mc_gui.tf_ammoP.text = weaponPrimary[0].ammo;
			cg.game.mc_gui.tf_ammoS.text = weaponSecondary[0].ammo;
			
			cg.cursor.tf_pr.text = weaponPrimary[activePrimary].ammo;
			cg.cursor.tf_se.text = weaponSecondary[activeSecondary].ammo;
			cg.cursor.tf_sp.text = weaponSpecial.ammo;
			trace(weaponMC[7].ammo);
		}
		
		// called by ContainerGame
		public function step():void 
		{
			// update the mouse
			var currX:Number = turret.mouseX;
			var currY:Number = turret.mouseY;
			
			if (currX != prevX || currY != prevY)
			{
				prevX = currX;
				prevY = currY;
				
				// face the turret towards the mouse
				var rot:Number = getAngle(0, 0, turret.mouseX, turret.mouseY);
				

				var r:Number = (rot + 360) % 360;
				if (r > TURRET_LIMIT_L)
					rot = TURRET_LIMIT_L;
				else if (r < TURRET_LIMIT_R)
					rot = TURRET_LIMIT_R;
					
				turret.mc_cannon.rotation = rot;
				//trace(TURRET_LIMIT_R + " | " + (rot + 360) % 360 + " | " + TURRET_LIMIT_L);
			}
			// end updating mouse
			
			var i:int;
			if (rightMouseDown)
			{
				if (cg.game.mc_gui.newEnemy.visible) return;
				var newAmmo:int = weaponSecondary[activeSecondary].fire();
				weaponMC[activeSecondary + 3].ammo.y = 16.65 + (35 * (1 - (weaponSecondary[activeSecondary].ammo / weaponSecondary[activeSecondary].ammoMax)));

				if (newAmmo != -1)
				{
					cg.game.mc_gui.tf_ammoS.text = newAmmo;
					cg.cursor.tf_se.text = newAmmo;
					if (activeSecondary != 2)
						cg.manPart.spawnParticle("shell", new Point(turret.x + getRand(15, 10), turret.y + getRand( -20, -26)), 0, getRand(0, 1), getRand( -4, -3), .5);
				}
				else if (noAmmoGrace == 0 && weaponSecondary[activeSecondary].ammo == 0)
				{
					SoundPlayer.play("sfx_no_ammo");
					noAmmoGrace = 15;
				}
				
				weaponMC[activeSecondary + 3].mc_noAmmo.visible = weaponSecondary[activeSecondary].ammo == 0;
			}

			// update all weapons
			for (i = 0; i < 3; i++)
			{
				if (weaponPrimary[i])
				{
					weaponPrimary[i].step();
					weaponMC[i].reload.y = -20 + ((weaponPrimary[i].cooldownCounter / weaponPrimary[i].cooldownReset) * 40);
				}
				if (weaponSecondary[i])
				{
					weaponSecondary[i].step();
					weaponMC[i + 3].reload.y = -20 + ((weaponSecondary[i].cooldownCounter / weaponSecondary[i].cooldownReset) * 40);
				}
			}
			
			if (weaponSpecial)
			{
				weaponSpecial.step();
				weaponMC[7].reload.y = -20 + ((weaponSpecial.cooldownCounter / weaponSpecial.cooldownReset) * 40);
			}
			
			cg.cursor.tf_pr.alpha = weaponPrimary[activePrimary].ammo == 0 ? .4 : (weaponPrimary[activePrimary].cooldownCounter == 0 ? 1 : .5);
			cg.cursor.tf_se.alpha = weaponSecondary[activeSecondary].ammo == 0 ? .4 : (weaponSecondary[activeSecondary].cooldownCounter == 0 ? 1 : .5);
			cg.cursor.tf_sp.alpha = weaponSpecial.ammo == 0 ? .4 : (weaponSpecial.cooldownCounter == 0 ? 1 : .5);
			
			if (noAmmoGrace > 0)
				noAmmoGrace--;
		}

		private function onMouseLeftDown(e:MouseEvent):void
		{
			if (!cg) return;
			if (!cg.gameActive || cg.game.mouseY > 175) return;
			if (cg.game.mc_gui.newEnemy.visible) return;
			var newAmmo:int = weaponPrimary[activePrimary].fire();
			if (newAmmo != -1)
			{
				cg.game.mc_gui.tf_ammoP.text = newAmmo;
				cg.cursor.tf_pr.text = newAmmo;
				weaponMC[activePrimary].ammo.y = 16.65 + (35 * (1 - (weaponPrimary[activePrimary].ammo / weaponPrimary[activePrimary].ammoMax)));
				for (var i:int = 0; i < int(getRand(2, 4)) + 2; i++)
				{
					cg.manPart.spawnParticle("", new Point(turret.x + getRand(-5, 5), turret.y + getRand(-5, 5)), 0, getRand(0, 1), getRand(0, 1), .05);
				}
			}
			else
			{
				SoundPlayer.play("sfx_no_ammo");
			}
			
			weaponMC[activePrimary].mc_noAmmo.visible = weaponPrimary[activePrimary].ammo == 0;
		}
		
		// TEMP
		/*public function autoFire(tgtX:Number, tgtY:Number):void
		{
			var newAmmo:int = weaponPrimary[activePrimary].fire(tgtX, tgtY);
			if (newAmmo != -1)
				cg.game.mc_gui.tf_ammoP.text = newAmmo;
		}*/
		
		private function onMouseRightDown(e:MouseEvent):void
		{
			rightMouseDown = true;
		}
		
		private function onMouseRightUp(e:MouseEvent):void
		{
			rightMouseDown = false;
		}
		
		public function reloadAll():void
		{
			// reload all weapons
			for (var i:int = 0; i < 3; i++)
			{
				if (weaponPrimary[i])
				{
					weaponPrimary[i].reset();
					weaponMC[i].reload.y = 20;
					weaponMC[i].ammo.y = 16.65;
					weaponMC[i].mc_noAmmo.visible = false;
				}
				if (weaponSecondary[i])
				{
					weaponSecondary[i].reset();
					weaponMC[i + 3].reload.y = 20;
					weaponMC[i + 3].ammo.y = 16.65;
					weaponMC[i + 3].mc_noAmmo.visible = false;
				}
			}
			
			if (weaponSpecial)
			{
				weaponSpecial.reset();
				weaponMC[7].reload.y = 20;
				weaponMC[7].ammo.y = 16.65;
				weaponMC[7].mc_noAmmo.visible = false;
			}
			
			cg.game.mc_gui.tf_ammoP.text = weaponPrimary[activePrimary].ammo;
			cg.game.mc_gui.tf_ammoS.text = weaponSecondary[activeSecondary].ammo;
			
			cg.cursor.tf_pr.text = weaponPrimary[activePrimary].ammo;
			cg.cursor.tf_se.text = weaponSecondary[activeSecondary].ammo;
			cg.cursor.tf_sp.text = weaponSpecial.ammo;
			
			cg.cursor.tf_pr.alpha = 1;
			cg.cursor.tf_se.alpha = 1;
			cg.cursor.tf_sp.alpha = 1;
		}
		
		public function upgradeAll():void
		{
			// upgrade all weapons
			for (var i:int = 0; i < 3; i++)
			{
				if (weaponPrimary[i])
					weaponPrimary[i].setUpgrades();
				if (weaponSecondary[i])
					weaponSecondary[i].setUpgrades();
			}
			if (weaponSpecial)
				weaponSpecial.setUpgrades();
		}
		
		private function onKeyboard(e:KeyboardEvent):void
		{
			if (!cg.gameActive) return;
			
			var changed:int = -1;
			var old:int;
			
			switch (e.keyCode)
			{
				case 32:	// SPACE
					var newAmmo:int = weaponSpecial.fire();
					weaponMC[7].ammo.y = 16.65 + (35 * (1 - (weaponSpecial.ammo / weaponSpecial.ammoMax)));
					weaponMC[7].mc_noAmmo.visible = weaponSpecial.ammo == 0;
					cg.cursor.tf_sp.text = weaponSpecial.ammo;
				break;
				case 49:	// 1
					if (weaponPrimary[0])
					{
						old = activePrimary;
						activePrimary = 0;
						changed = 0;
						cg.game.mc_gui.tf_weaponP.text = weaponPrimary[activePrimary].name;
						cg.game.mc_gui.tf_ammoP.text = weaponPrimary[activePrimary].ammo;
						cg.cursor.tf_pr.alpha = 1;
						cg.cursor.tf_pr.text = weaponPrimary[activePrimary].ammo;
					}
				break;
				case 50: 	// 2
					if (weaponPrimary[1])
					{
						old = activePrimary;
						activePrimary = 1;
						changed = 1;
						cg.game.mc_gui.tf_weaponP.text = weaponPrimary[activePrimary].name;
						cg.game.mc_gui.tf_ammoP.text = weaponPrimary[activePrimary].ammo;
						cg.cursor.tf_pr.alpha = 1;
						cg.cursor.tf_pr.text = weaponPrimary[activePrimary].ammo;
					}
				break;
				case 51:	// 3
					if (weaponPrimary[2])
					{
						old = activePrimary;
						activePrimary = 2;
						changed = 2;
						cg.game.mc_gui.tf_weaponP.text = weaponPrimary[activePrimary].name;
						cg.game.mc_gui.tf_ammoP.text = weaponPrimary[activePrimary].ammo;
						cg.cursor.tf_pr.alpha = 1;
						cg.cursor.tf_pr.text = weaponPrimary[activePrimary].ammo;
					}
				break;
				case 52:	// 4
					if (weaponSecondary[0])
					{
						old = activeSecondary + 3;
						activeSecondary = 0;
						changed = 3;
						cg.game.mc_gui.tf_weaponS.text = weaponSecondary[activeSecondary].name;
						cg.game.mc_gui.tf_ammoS.text = weaponSecondary[activeSecondary].ammo;
						cg.cursor.tf_se.alpha = 1;
						cg.cursor.tf_se.text = weaponSecondary[activeSecondary].ammo;
					}
				break;
				case 53:	// 5
					if (weaponSecondary[1])
					{
						old = activeSecondary + 3;
						activeSecondary = 1;
						changed = 4;
						cg.game.mc_gui.tf_weaponS.text = weaponSecondary[activeSecondary].name;
						cg.game.mc_gui.tf_ammoS.text = weaponSecondary[activeSecondary].ammo;
						cg.cursor.tf_se.alpha = 1;
						cg.cursor.tf_se.text = weaponSecondary[activeSecondary].ammo;
					}
				break;
				case 54:	// 6
					if (weaponSecondary[2])
					{
						old = activeSecondary + 3;
						activeSecondary = 2;
						changed = 5;
						cg.game.mc_gui.tf_weaponS.text = weaponSecondary[activeSecondary].name;
						cg.game.mc_gui.tf_ammoS.text = weaponSecondary[activeSecondary].ammo;
						cg.cursor.tf_se.alpha = 1;
						cg.cursor.tf_se.text = weaponSecondary[activeSecondary].ammo;
					}
				break;
			}
		
			if (changed == -1) return;

			weaponMC[old].selected.visible = false;
			weaponMC[changed].selected.visible = true;
		}
		
		public function destroy(e:Event):void
		{
			cg = null;
			if (turret)
			{
				if (turret.stage)
				{
					turret.stage.removeEventListener(MouseEvent.CLICK, onMouseLeftDown);
					turret.stage.removeEventListener(MouseEvent.RIGHT_MOUSE_DOWN, onMouseRightDown);
					turret.stage.removeEventListener(MouseEvent.RIGHT_MOUSE_UP, onMouseRightUp);
					turret.stage.removeEventListener(KeyboardEvent.KEY_DOWN, onKeyboard);
				}
				turret.removeEventListener(Event.REMOVED_FROM_STAGE, destroy);
			}
			cg = null;
			turret = null;
		}
	}
}
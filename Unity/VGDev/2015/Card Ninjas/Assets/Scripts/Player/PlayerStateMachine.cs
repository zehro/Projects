using Assets.Scripts.Util;

namespace Assets.Scripts.Player
{
    /* This file controls all of the transitions between states*/
    class PlayerStateMachine
    {
        private Enums.PlayerState currState;
        private float hold = 0;//used for delays
        private bool die = false;

        public PlayerStateMachine()
        {
            currState = Enums.PlayerState.Idle;
        }

        public Enums.PlayerState update(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            switch (currState)
            {
                case Enums.PlayerState.Idle: currState = Idle(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.MoveBegining: currState = MoveBegining(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.MoveEnding: currState = MoveEnding(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.Hit: currState = Hit(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.Dead: currState = Dead(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.BasicAttack: currState = BasicAttack(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.HoriSwingMid: currState = HoriSwingMid(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.VertiSwingHeavy: currState = VertiSwingHeavy(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.ThrowLight: currState = ThrowLight(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.ThrowMid: currState = ThrowMid(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.Shoot: currState = Shoot(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.ChiAttack: currState = ChiAttack(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.ChiStationary: currState = ChiStationary(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.TauntGokuStretch: currState = TauntGokuStretch(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.TauntPointPoint: currState = TauntPointPoint(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.TauntThumbsDown: currState = TauntThumbsDown(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.TauntWrasslemania: currState = TauntWrasslemania(hit, animDone, direction, type, handEmpty, playerNumber); break;
                case Enums.PlayerState.TauntYaMoves: currState = TauntYaMoves(hit, animDone, direction, type, handEmpty, playerNumber); break;
            }
            return currState;
        }


        //The following methods control when and how you can transition between states

        private Enums.PlayerState Idle(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (!handEmpty && CustomInput.BoolFreshPress(CustomInput.UserInput.UseCard, playerNumber))
            {
                switch (type)
                {
                    case Enums.CardTypes.SwordVert: return Enums.PlayerState.VertiSwingHeavy;
                    case Enums.CardTypes.SwordHori: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.WideSword: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.NaginataVert: return Enums.PlayerState.VertiSwingHeavy;
                    case Enums.CardTypes.NaginataHori: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.HammerVert: return Enums.PlayerState.VertiSwingHeavy;
                    case Enums.CardTypes.HammerHori: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.ThrowLight: return Enums.PlayerState.ThrowLight;
                    case Enums.CardTypes.ThrowMid: return Enums.PlayerState.ThrowMid;
                    case Enums.CardTypes.Shoot: return Enums.PlayerState.Shoot;
                    case Enums.CardTypes.ChiAttack: return Enums.PlayerState.ChiAttack;
                    case Enums.CardTypes.ChiStationary: return Enums.PlayerState.ChiStationary;
                    case Enums.CardTypes.Error: return Enums.PlayerState.ChiAttack;
                    case Enums.CardTypes.Fan: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.Kanobo: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.Tanto: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.Wakizashi: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.Tonfa: return Enums.PlayerState.HoriSwingMid;
                    case Enums.CardTypes.BoStaff: return Enums.PlayerState.HoriSwingMid;
                }
            }
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Attack, playerNumber))
                return Enums.PlayerState.BasicAttack;
            if (direction != Enums.Direction.None)
                return Enums.PlayerState.MoveBegining;
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Taunt, playerNumber))
            {
                float chance = UnityEngine.Random.Range(0f, 1f);
                if (chance < .225f)
                    return Enums.PlayerState.TauntPointPoint;
                else if (chance < .45f)
                    return Enums.PlayerState.TauntThumbsDown;
                else if (chance < .675f)
                    return Enums.PlayerState.TauntWrasslemania;
                else if (chance < .9f)
                    return Enums.PlayerState.TauntYaMoves;
                else
                    return Enums.PlayerState.TauntGokuStretch;
            }
            return Enums.PlayerState.Idle;
        }

        private Enums.PlayerState MoveBegining(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.MoveEnding;
            return Enums.PlayerState.MoveBegining;
        }

        private Enums.PlayerState MoveEnding(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.MoveEnding;
        }

        private Enums.PlayerState Hit(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            hold += UnityEngine.Time.deltaTime;
            if (hold > .4f)
            {
                hold = 0;
                if (die)
                    return Enums.PlayerState.Dead;
                return Enums.PlayerState.Idle;
            }
            return Enums.PlayerState.Hit;
        }

        //this is used to prevent the player character from doing any thing while dead
        private Enums.PlayerState Dead(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            return Enums.PlayerState.Dead;
        }

        private Enums.PlayerState BasicAttack(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (CustomInput.BoolFreshPress(CustomInput.UserInput.Attack, playerNumber))
                hold = 1;
            if (animDone)
            {
                if (hold == 0)
                    return Enums.PlayerState.Idle;
                hold = 0;
                return Enums.PlayerState.BasicAttack;
            }
            return Enums.PlayerState.BasicAttack;
        }

        private Enums.PlayerState HoriSwingMid(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.HoriSwingMid;
        }
        private Enums.PlayerState VertiSwingHeavy(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.VertiSwingHeavy;
        }
        private Enums.PlayerState ThrowLight(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.ThrowLight;
        }
        private Enums.PlayerState ThrowMid(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.ThrowMid;
        }
        private Enums.PlayerState Shoot(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.Shoot;
        }
        private Enums.PlayerState ChiAttack(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.ChiAttack;
        }
        private Enums.PlayerState ChiStationary(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.ChiStationary;
        }
        private Enums.PlayerState TauntGokuStretch(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.TauntGokuStretch;
        }
        private Enums.PlayerState TauntPointPoint(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.TauntPointPoint;
        }
        private Enums.PlayerState TauntThumbsDown(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.TauntThumbsDown;
        }
        private Enums.PlayerState TauntWrasslemania(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.TauntWrasslemania;
        }
        private Enums.PlayerState TauntYaMoves(bool hit, bool animDone, Enums.Direction direction, Enums.CardTypes type, bool handEmpty, int playerNumber)
        {
            if (hit)
                return Enums.PlayerState.Hit;
            if (animDone)
                return Enums.PlayerState.Idle;
            return Enums.PlayerState.TauntYaMoves;
        }

        internal void Die()
        {
            die = true;
        }

        internal void Revive()
        {
            currState = Enums.PlayerState.Idle;
            die = false;
        }
    }
}

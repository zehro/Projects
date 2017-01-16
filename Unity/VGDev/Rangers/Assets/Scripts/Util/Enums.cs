namespace Assets.Scripts.Util
{
    /// <summary>
    /// Holds all of the enums used in the game
    /// </summary>
    public static class Enums
    {
        public enum Arrows { Normal, Fireball, Ice, Thunder, Acid, Ricochet, Ghost, ZeroGravity, Tracking, Lifesteal, Virus, Splitting, HeavyKnockback, RapidFire, Grappling, Teleporting, NumTypes }
        public enum Tokens { Fireball, Ice, Thunder, Acid, Ricochet, Ghost, ZeroGravity, Tracking, Lifesteal, Virus, Splitting, HeavyKnockback, RapidFire, Grappling, Teleporting, Health, NumTypes }
        public enum GameType { Deathmatch, Stock, NumTypes }
        public enum RepetitionTimerSettings { Limited, Unlimited }
        public enum Frequency { None, Sparce, Infrequent, Average, Frequent, Abundant, NumTypes }        
        public enum MenuDirections { Up, Down, Left, Right}
        public enum UIStates { Splash, Main, SinglePlayer, Multiplayer, Settings, Audio, Video, Signin, ArenaStandard, LevelSelect, ValueModifier, TargetLevelSelect, None}
        public enum BattleStages { ProLeagueStandard, RangerDreams, TheOrbitalBelt, SteamLords, FireOnMountFuji, FinalDestination, NumStages }
		public enum TargetPracticeStages { Intro, MagneticDistortion, NumStages }
    }
}

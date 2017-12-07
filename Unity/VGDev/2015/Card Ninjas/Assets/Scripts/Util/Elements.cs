using Assets.Scripts.Util;

namespace Assets.Scripts.Util
{
    public class Elements
    {
		//Thunder == Metal
		//Fire -> Wood -> Water -> Thunder -> Earth -> Fire
		//Fire -> Thunder -> Wood -> Earth -> Water


        private static float[,] damageValues = new float[,] {
            //	Fire,	Earth,	Thund,	Water,	Wood,	None
            { 	1,		.5f,	1.5f,  	.75f,	2,		1 },    	//Fire
            {   2,		1,		.5f,	1.5f,	.75f,	1 },		//Earth
            {  	.75f,	2,		1,		.5f,	1.5f,	1 },		//Thunder
            {   1.5f,	.75f,	2,		1,		.5f,	1 },		//Water
            {  	.5f,	1.5f,  	.75f,	2,		1,		1 },		//Wood
            {   1,		1,    	1,		1,    	1, 		1 }  };		//None

        public static float GetDamageMultiplier(Enums.Element me, Enums.Element them)
        {
            return damageValues[(int)me, (int)them];
        }
    }
}

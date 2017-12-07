namespace Assets.Scripts.Util
{
    /// <summary>
    /// Handles any bitwise type operations for treating ints as registers
    /// </summary>
    public static class Bitwise
    {
        /// <summary>
        /// Checks to see if a certain bit is on
        /// </summary>
        /// <param name="reg">The register to check</param>
        /// <param name="bitPlace">The place in the register to check</param>
        /// <returns>Whether or not the specified bit is on</returns>
        public static bool IsBitOn(int reg, int bitPlace)
        {
            return (((1 << bitPlace) & reg) > 0);
        }

        /// <summary>
        /// Turns a specific bit on
        /// </summary>
        /// <param name="reg">The register to modify</param>
        /// <param name="bitPlace">The place in the register to modify</param>
        /// <returns>The new modified register</returns>
        public static int SetBit(int reg, int bitPlace)
        {
            return (reg |= (1 << bitPlace));
        }
        /// <summary>
        /// Turns a specific bit off
        /// </summary>
        /// <param name="reg">The register to modify</param>
        /// <param name="bitPlace">The place in the register to modify</param>
        /// <returns></returns>
        public static int ClearBit(int reg, int bitPlace)
        {
            reg &= ~(1 << bitPlace);
            return reg;
        }
    }
}
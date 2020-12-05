
namespace JustSave
{
    public enum DebugMode
    {
        OFF, FATAL, ERROR, WARN, INFO, DEBUG, EXTENSIVE, ALL
    }

    public class Dbug
    {
        public static DebugMode GlobalLevel = DebugMode.WARN;

        /// <summary>
        /// returns true if the log message, which requires at least the minimum level, should be printed
        /// </summary>
        /// <param name="MinNeededLevel">the level of the message you want to print</param>
        /// <returns>true if the message should be printed</returns>
        public static bool Is(DebugMode MinNeededLevel)
        {
            return GlobalLevel >= MinNeededLevel ? true : false;
        }
    }
}

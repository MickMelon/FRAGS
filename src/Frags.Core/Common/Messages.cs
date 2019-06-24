using System;

namespace Frags.Core.Common
{
    public static class Messages
    {
        #region Character
        public static readonly string CHAR_NOT_FOUND = "Character not found.";
        public static readonly string CHAR_NAME_EXISTS = "You already have a character with that name.";
        public static readonly string CHAR_CREATE_SUCCESS = "Character created successfully.";
        public static readonly string CHAR_UPDATE_SUCCESS = "Character updated successfully.";
        public static readonly string CHAR_ACTIVE = "Character activated successfully.";
        public static readonly string CHAR_ALREADY_ACTIVE = "Character is already active.";
        public static readonly string CHAR_LEVEL_TOO_LOW = "Character level too low.";
        public static readonly string CHAR_LEVEL_TOO_HIGH = "Character level too high.";
        public static readonly string CHAR_TOO_MANY = "You have reached the limit of characters.";
        #endregion

        #region Game
        public static readonly string ROLL_FAILED = "Error rolling character statistic.";
        public static readonly string ROLL_DICE = "Rolled: {0}.";
        #endregion

        #region NPC
        public static readonly string NPC_NOT_FOUND = "NPC not found.";
        #endregion

        #region Statistics
        public static readonly string STAT_CREATE_SUCCESS = "Statistic created successfully.";
        public static readonly string STAT_UPDATE_SUCCESS = "Statistic updated successfully.";
        public static readonly string STAT_CREATE_FAILURE = "Statistic creation failed.";
        public static readonly string STAT_DELETE_SUCCESS = "Statistic deleted successfully.";
        public static readonly string STAT_NAME_EXISTS = "Statistic with that name already exists.";
        public static readonly string STAT_NOT_FOUND = "Statistic not found.";
        public static readonly string STAT_SET_SUCCESS = "Statistic set sucessfully.";
        public static readonly string STAT_TOO_MANY_AT_MAX = "Too many statistics were set to their max value. (Limit {0})";
        public static readonly string STAT_RESET = "Statistics reset.";
        public static readonly string STAT_CHECK = "{0}'s {1} is set to: {2}.";

        public static readonly string STAT_NOT_ENOUGH_POINTS = "Not enough points to set that statistic. Attempted: {0}/{1}.";
        #endregion

        #region Effects
        public static readonly string EFFECT_CREATE_SUCCESS = "Effect created successfully.";
        public static readonly string EFFECT_CREATE_FAILURE = "Effect creation failed.";
        public static readonly string EFFECT_UPDATE_SUCCESS = "Effect updated successfully.";
        public static readonly string EFFECT_DELETE_SUCCESS = "Effect deleted successfully.";
        public static readonly string EFFECT_NAME_EXISTS = "Effect with that name already exists.";
        public static readonly string EFFECT_NOT_FOUND = "Effect not found.";
        public static readonly string EFFECT_ADDED = "Effect added sucessfully.";
        public static readonly string EFFECT_REMOVED = "Effect removed sucessfully.";
        public static readonly string EFFECT_ALREADY_ADDED = "Effect is already added.";
        public static readonly string EFFECT_TOO_MANY = "You have reached the limit of effects.";
        #endregion

        #region Other
        public static readonly string REQUIRE_ROLE_FAIL = "You must be have the {0} role to use this command.";
        public static readonly string NOT_IN_GUILD = "You must be in a server to use this command.";
        public static readonly string INVALID_INPUT = "Input was invalid.";
        public static readonly string NOT_ENOUGH_POINTS = "Not enough points.";
        public static readonly string TOO_LOW = "The value was too low.";
        public static readonly string TOO_HIGH = "The value was too high.";
        #endregion
    }
}
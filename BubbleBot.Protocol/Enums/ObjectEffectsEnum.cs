namespace BubbleBot.Protocol.Enums
{
    public enum ObjectEffectsEnum
    {
        /// <summary>
        /// Steals #1{~1~2 to X} MP
        /// </summary>
        ACTION_CHARACTER_MOVEMENT_POINTS_STEAL = 77,
        /// <summary>
        /// Adds #1{~1~2 to }#2 MP
        /// </summary>
        ACTION_CHARACTER_MOVEMENT_POINTS_WIN = 78,
        /// <summary>
        /// #3% damage received x#1, or else healed by x#2
        /// </summary>
        ACTION_CHARACTER_MULTIPLY_RECEIVED_DAMAGE_OR_GIVE_LIFE_WITH_RATIO = 79,
        /// <summary>
        /// WARNING DON'T SET
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_FROM_PUSH = 80,
        /// <summary>
        /// HP restored #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_WIN_WITHOUT_ELEMENT = 81,
        /// <summary>
        /// Steals #1{~1~2 to }#2 HP (fixed)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_STEAL_WITHOUT_BOOST = 82,
        /// <summary>
        /// Ste_Steals #1{~1~2 to }#2 AP
        /// </summary>
        ACTION_CHARACTER_ACTION_POINTS_STEAL = 84,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's life (water)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_FROM_WATER = 85,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's life (earth)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_FROM_EARTH = 86,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's life (air)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_FROM_AIR = 87,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's life (fire)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_FROM_FIRE = 88,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's life (neutral)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE = 89,
        /// <summary>
        /// Gives #1{~1~2 to }#2 % of his own life
        /// </summary>
        ACTION_CHARACTER_DISPATCH_LIFE_POINTS_PERCENT = 90,
        /// <summary>
        /// Steals #1{~1~2 to }#2 HP (water)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_STEAL_FROM_WATER = 91,
        /// <summary>
        /// Steals #1{~1~2 to }#2 HP (earth)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_STEAL_FROM_EARTH = 92,
        /// <summary>
        /// Steals #1{~1~2 to }#2 HP (air)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_STEAL_FROM_AIR = 93,
        /// <summary>
        /// Steals #1{~1~2 to }#2 HP (fire)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_STEAL_FROM_FIRE = 94,
        /// <summary>
        /// Steals #1{~1~2 to }#2 HP (neutral)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_STEAL = 95,
        /// <summary>
        /// Damage: #1{~1~2 to }#2 (water)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_FROM_WATER = 96,
        /// <summary>
        /// Damage: #1{~1~2 to }#2 (earth)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_FROM_EARTH = 97,
        /// <summary>
        /// Damage: #1{~1~2 to }#2 (air)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_FROM_AIR = 98,
        /// <summary>
        /// Damage: #1{~1~2 to }#2 (fire)   
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_FROM_FIRE = 99,
        /// <summary>
        /// Damage: #1{~1~2 to }#2 (neutral)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST = 100,
        /// <summary>
        /// Lost AP for the target: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_ACTION_POINTS_LOST = 101,
        /// <summary>
        /// Reflects a spell, max. of level #2
        /// </summary>
        ACTION_CHARACTER_SPELL_REFLECTOR = 106,
        /// <summary>
        /// Reflects #1{~1~2 to }#2 damage
        /// </summary>
        ACTION_CHARACTER_LIFE_LOST_REFLECTOR = 107,
        /// <summary>
        /// HP restored #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_WIN = 108,
        /// <summary>
        /// Damage to the caster: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_CASTER = 109,
        /// <summary>
        /// +#1{~1~2 to }#2 life
        /// </summary>
        ACTION_CHARACTER_BOOST_LIFE_POINTS = 110,
        /// <summary>
        /// +#1{~1~2 to }#2 AP
        /// </summary>
        ACTION_CHARACTER_BOOST_ACTION_POINTS = 111,
        /// <summary>
        /// +#1{~1~2 to }#2 damage
        /// </summary>v
        ACTION_CHARACTER_BOOST_DAMAGES = 112,
        /// <summary>
        /// Multiply damage by #1
        /// </summary>
        ACTION_CHARACTER_MULTIPLY_DAMAGES = 114,
        /// <summary>
        /// +#1{~1~2 to }#2 critical hits
        /// </summary>
        ACTION_CHARACTER_BOOST_CRITICAL_HIT = 115,
        /// <summary>
        /// -#1{~1~2 to }#2 range
        /// </summary>
        ACTION_CHARACTER_DEBOOST_RANGE = 116,
        /// <summary>
        /// +#1{~1~2 to }#2 range
        /// </summary>
        ACTION_CHARACTER_BOOST_RANGE = 117,
        /// <summary>
        /// +#1{~1~2 to }#2 strength
        /// </summary>
        ACTION_CHARACTER_BOOST_STRENGTH = 118,
        /// <summary>
        /// +#1{~1~2 to }#2 agility
        /// </summary>
        ACTION_CHARACTER_BOOST_AGILITY = 119,
        /// <summary>
        /// Adds +#1{~1~2 to }#2 AP
        /// </summary>
        ACTION_CHARACTER_ACTION_POINTS_WIN = 120,
        /// <summary>
        /// +#1{~1~2 to }#2 damage
        /// </summary>
        ACTION_CHARACTER_BOOST_DAMAGES_FOR_ALL_GAME = 121,
        /// <summary>
        /// Adds #1{~1~2 to }#2 to critical failures
        /// </summary>
        ACTION_CHARACTER_BOOST_CRITICAL_MISS = 122,
        /// <summary>
        /// +#1{~1~2 to }#2 chance
        /// </summary>
        ACTION_CHARACTER_BOOST_CHANCE = 123,
        /// <summary>
        /// +#1{~1~2 to }#2 wisdom
        /// </summary>
        ACTION_CHARACTER_BOOST_WISDOM = 124,
        /// <summary>
        /// +#1{~1~2 to }#2 vitality
        /// </summary>
        ACTION_CHARACTER_BOOST_VITALITY = 125,
        /// <summary>
        /// +#1{~1~2 to }#2 intelligence
        /// </summary>
        ACTION_CHARACTER_BOOST_INTELLIGENCE = 126,
        /// <summary>
        /// MP lost: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_MOVEMENT_POINTS_LOST = 127,
        /// <summary>
        /// +#1{~1~2 to }#2 MP
        /// </summary>
        ACTION_CHARACTER_BOOST_MOVEMENT_POINTS = 128,
        /// <summary>
        /// Steals #1{~1~2 to }#2 Kamas
        /// </summary>
        ACTION_CHARACTER_STEAL_GOLD = 130,
        /// <summary>
        /// Lost AP for caster: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_ACTION_POINTS_LOST_CASTER = 133,
        /// <summary>
        /// Lost MP for caster: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_MOVEMEMT_POINTS_LOST_CASTER = 134,
        /// <summary>
        /// Caster's range reduced by: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_DEBOOST_RANGE_CASTER = 135,
        /// <summary>
        /// Caster's range increased by: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_BOOST_RANGE_CASTER = 136,
        /// <summary>
        /// Caster's physical damage increased by : #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_BOOST_DAMAGES_CASTER = 137,
        /// <summary>
        /// Increases damage by #1{~1~2 to }#2%
        /// </summary>
        ACTION_CHARACTER_BOOST_DAMAGES_PERCENT = 138,
        /// <summary>
        /// Restores #1{~1~2 to }#2 energy points
        /// </summary>
        ACTION_CHARACTER_ENERGY_POINTS_WIN = 139,
        /// <summary>
        /// +#1{~1~2 to }#2 to physical damage
        /// </summary>
        ACTION_CHARACTER_BOOST_PHYSICAL_DAMAGES = 142,
        /// <summary>
        /// HP restored: #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_WIN_WITHOUT_BOOST = 143,
        /// <summary>
        /// Damage: #1{~1~2 to }#2 (unboosted)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_WITHOUT_BOOST = 144,
        /// <summary>
        /// -#1{~1~2 to }#2 to damage
        /// </summary>
        ACTION_CHARACTER_DEBOOST_DAMAGES = 145,
        /// <summary>
        /// -#1{~1~2 to }#2 chance
        /// </summary>
        ACTION_CHARACTER_DEBOOST_CHANCE = 152,
        /// <summary>
        /// -#1{~1~2 to }#2 vitality
        /// </summary>
        ACTION_CHARACTER_DEBOOST_VITALITY = 153,
        /// <summary>
        /// -#1{~1~2 to }#2 agility
        /// </summary>
        ACTION_CHARACTER_DEBOOST_AGILITY = 154,
        /// <summary>
        /// -#1{~1~2 to }#2 intelligence
        /// </summary>
        ACTION_CHARACTER_DEBOOST_INTELLIGENCE = 155,
        /// <summary>
        /// -#1{~1~2 to }#2 wisdom
        /// </summary>
        ACTION_CHARACTER_DEBOOST_WISDOM = 156,
        /// <summary>
        /// -#1{~1~2 to }#2 strength
        /// </summary>
        ACTION_CHARACTER_DEBOOST_STRENGTH = 157,
        /// <summary>
        /// Increases load weight by #1{~1~2 to }#2 pods
        /// </summary>
        ACTION_CHARACTER_BOOST_MAXIMUM_WEIGHT = 158,
        /// <summary>
        /// Decreases load weight by #1{~1~2 to }#2 pods
        /// </summary>
        ACTION_CHARACTER_DEBOOST_MAXIMUM_WEIGHT = 159,
        /// <summary>
        /// Increases chance of avoiding AP loss by #1{~1~2 to }#2%
        /// </summary>
        ACTION_CHARACTER_BOOST_ACTION_POINTS_LOST_DODGE = 160,
        /// <summary>
        /// Increases chance of avoiding MP loss by #1{~1~2 to }#2%
        /// </summary>
        ACTION_CHARACTER_BOOST_MOVEMENT_POINTS_LOST_DODGE = 161,
        /// <summary>
        /// -#1{~1~2 to}#2 chance of avoiding AP losses
        /// </summary>
        ACTION_CHARACTER_DEBOOST_ACTION_POINTS_LOST_DODGE = 162,
        /// <summary>
        /// -#1{~1~2 to}#2 chance of avoiding MP losses
        /// </summary>
        ACTION_CHARACTER_DEBOOST_MOVEMENT_POINTS_LOST_DODGE = 163,
        /// <summary>
        /// Increases (#1) damage by #2%
        /// </summary>
        ACTION_CHARACTER_BOOST_WEAPON_DAMAGE_PERCENT = 165,
        /// <summary>
        /// -#1{~1~2 to }#2 AP
        /// </summary>
        ACTION_CHARACTER_DEBOOST_ACTION_POINTS = 168,
        /// <summary>
        /// -#1{~1~2 to }#2 MP
        /// </summary>
        ACTION_CHARACTER_DEBOOST_MOVEMENT_POINTS = 169,
        /// <summary>
        /// -#1{~1~2 to }#2 critical hits
        /// </summary>
        ACTION_CHARACTER_DEBOOST_CRITICAL_HIT = 171,
        /// <summary>
        /// Magic reduction decreased by #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_DEBOOST_MAGICAL_REDUCTION = 172,
        /// <summary>
        /// Physical reduction decreased by #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_DEBOOST_PHYSICAL_REDUCTION = 173,
        /// <summary>
        /// +#1{~1~2 to }#2 initiative
        /// </summary>
        ACTION_CHARACTER_BOOST_INITIATIVE = 174,
        /// <summary>
        /// -#1{~1~2 to }#2 initiative
        /// </summary>
        ACTION_CHARACTER_DEBOOST_INITIATIVE = 175,
        /// <summary>
        /// +#1{~1~2 to }#2 prospecting
        /// </summary>
        ACTION_CHARACTER_BOOST_MAGIC_FIND = 176,
        /// <summary>
        /// -#1{~1~2 to }#2 prospecting
        /// </summary>
        ACTION_CHARACTER_DEBOOST_MAGIC_FIND = 177,
        /// <summary>
        /// +#1{~1~2 to }#2 heals
        /// </summary>
        ACTION_CHARACTER_BOOST_HEAL_BONUS = 178,
        /// <summary>
        /// -#1{~1~2 to }#2 heals
        /// </summary>
        ACTION_CHARACTER_DEBOOST_HEAL_BONUS = 179,
        /// <summary>
        /// Creates a double of the caster
        /// </summary>
        ACTION_CHARACTER_ADD_DOUBLE = 180,
        /// <summary>
        /// +#1{~1~2 to }#2 to summonable creatures
        /// </summary>
        ACTION_CHARACTER_BOOST_MAXIMUM_SUMMONED_CREATURES = 182,
        /// <summary>
        /// Magic reduction of #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_BOOST_MAGICAL_REDUCTION = 183,
        /// <summary>
        /// Physical reduction of #1{~1~2 to }#2
        /// </summary>
        ACTION_CHARACTER_BOOST_PHYSICAL_REDUCTION = 184,
        /// <summary>
        /// Decreases damage by #1{~1~2 to }#2%
        /// </summary>
        ACTION_CHARACTER_DEBOOST_DAMAGES_PERCENT = 186,
        /// <summary>
        /// #1{~1~2 to }#2 % earth resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_EARTH_ELEMENT_PERCENT = 210,
        /// <summary>
        /// #1{~1~2 to }#2 % water resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_WATER_ELEMENT_PERCENT = 211,
        /// <summary>
        /// #1{~1~2 to }#2 % air resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_AIR_ELEMENT_PERCENT = 212,
        /// <summary>
        /// #1{~1~2 to }#2 % fire resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_FIRE_ELEMENT_PERCENT = 213,
        /// <summary>
        /// #1{~1~2 to }#2 % neutral resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_NEUTRAL_ELEMENT_PERCENT = 214,
        /// <summary>
        /// #1{~1~2 to }#2 % earth weakness
        /// </summary>
        ACTION_CHARACTER_DEBOOST_EARTH_ELEMENT_PERCENT = 215,
        /// <summary>
        /// #1{~1~2 to }#2 % water weakness
        /// </summary>
        ACTION_CHARACTER_DEBOOST_WATER_ELEMENT_PERCENT = 216,
        /// <summary>
        /// #1{~1~2 to }#2 % air weakness
        /// </summary>
        ACTION_CHARACTER_DEBOOST_AIR_ELEMENT_PERCENT = 217,
        /// <summary>
        /// #1{~1~2 to }#2 % fire weakness
        /// </summary>
        ACTION_CHARACTER_DEBOOST_FIRE_ELEMENT_PERCENT = 218,
        /// <summary>
        /// #1{~1~2 to }#2 % neutral weakness
        /// </summary>
        ACTION_CHARACTER_DEBOOST_NEUTRAL_ELEMENT_PERCENT = 219,
        /// <summary>
        /// Reflects #1 damage
        /// </summary>
        ACTION_CHARACTER_REFLECTOR_UNBOOSTED = 220,
        /// <summary>
        /// Adds #1{~1~2 to }#2 to trap damage
        /// </summary>
        ACTION_CHARACTER_BOOST_TRAP = 225,
        /// <summary>
        /// +#1{~1~2 to }#2% damage to traps
        /// </summary>
        ACTION_CHARACTER_BOOST_TRAP_PERCENT = 226,
        /// <summary>
        /// +#1{~1~2 to }#2 earth resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_EARTH_ELEMENT_RESIST = 240,
        /// <summary>
        /// +#1{~1~2 to }#2 water resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_WATER_ELEMENT_RESIST = 241,
        /// <summary>
        /// +#1{~1~2 to }#2 air resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_AIR_ELEMENT_RESIST = 242,
        /// <summary>
        /// +#1{~1~2 to }#2 fire resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_FIRE_ELEMENT_RESIST = 243,
        /// <summary>
        /// +#1{~1~2 to }#2 neutral resistance
        /// </summary>
        ACTION_CHARACTER_BOOST_NEUTRAL_ELEMENT_RESIST = 244,
        /// <summary>
        /// -#1{~1~2 to }#2 earth resistance
        /// </summary>
        ACTION_CHARACTER_DEBOOST_EARTH_ELEMENT_RESIST = 245,
        /// <summary>
        /// -#1{~1~2 to }#2 water resistance
        /// </summary>
        ACTION_CHARACTER_DEBOOST_WATER_ELEMENT_RESIST = 246,
        /// <summary>
        /// -#1{~1~2 to }#2 air resistance
        /// </summary>
        ACTION_CHARACTER_DEBOOST_AIR_ELEMENT_RESIST = 247,
        /// <summary>
        /// -#1{~1~2 to }#2 fire resistance
        /// </summary>
        ACTION_CHARACTER_DEBOOST_FIRE_ELEMENT_RESIST = 248,
        /// <summary>
        /// -#1{~1~2 to }#2 neutral resistance
        /// </summary>
        ACTION_CHARACTER_DEBOOST_NEUTRAL_ELEMENT_RESIST = 249,
        /// <summary>
        /// #1{~1~2 to }#2% earth resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_EARTH_ELEMENT_PVP_PERCENT = 250,
        /// <summary>
        /// #1{~1~2 to }#2 % water resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_WATER_ELEMENT_PVP_PERCENT = 251,
        /// <summary>
        /// #1{~1~2 to }#2 % air resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_AIR_ELEMENT_PVP_PERCENT = 252,
        /// <summary>
        /// #1{~1~2 to }#2 % fire resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_FIRE_ELEMENT_PVP_PERCENT = 253,
        /// <summary>
        /// #1{~1~2 to }#2 % neutral resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_NEUTRAL_ELEMENT_PVP_PERCENT = 254,
        /// <summary>
        /// #1{~1~2 to }#2 % earth weakness against fighters
        /// </summary>
        ACTION_CHARACTER_DEBOOST_EARTH_ELEMENT_PVP_PERCENT = 255,
        /// <summary>
        /// #1{~1~2 to }#2 % water weakness against fighters
        /// </summary>
        ACTION_CHARACTER_DEBOOST_WATER_ELEMENT_PVP_PERCENT = 256,
        /// <summary>
        /// #1{~1~2 to }#2 % air weakness against fighters
        /// </summary>
        ACTION_CHARACTER_DEBOOST_AIR_ELEMENT_PVP_PERCENT = 257,
        /// <summary>
        /// #1{~1~2 to }#2 % fire weakness against fighters
        /// </summary>
        ACTION_CHARACTER_DEBOOST_FIRE_ELEMENT_PVP_PERCENT = 258,
        /// <summary>
        /// #1{~1~2 to }#2 % neutral weakness against fighters
        /// </summary>
        ACTION_CHARACTER_DEBOOST_NEUTRAL_ELEMENT_PVP_PERCENT = 259,
        /// <summary>
        /// +#1{~1~2 to }#2 earth resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_EARTH_ELEMENT_PVP_RESIST = 260,
        /// <summary>
        /// +#1{~1~2 to }#2 water resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_WATER_ELEMENT_PVP_RESIST = 261,
        /// <summary>
        /// Adds #1{~1~2 to }#2 air resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_AIR_ELEMENT_PVP_RESIST = 262,
        /// <summary>
        /// +#1{~1~2 to }#2 fire resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_FIRE_ELEMENT_PVP_RESIST = 263,
        /// <summary>
        /// +#1{~1~2 to }#2 neutral resistance against fighters
        /// </summary>
        ACTION_CHARACTER_BOOST_NEUTRAL_ELEMENT_PVP_RESIST = 264,
        /// <summary>
        /// #1{~1~2 to }#2 Chance theft
        /// </summary>
        ACTION_CHARACTER_STEAL_CHANCE = 266,
        /// <summary>
        /// #1{~1~2 to }#2 Vitality theft
        /// </summary>
        ACTION_CHARACTER_STEAL_VITALITY = 267,
        /// <summary>
        /// #1{~1~2 to }#2 Agility theft
        /// </summary>
        ACTION_CHARACTER_STEAL_AGILITY = 268,
        /// <summary>
        /// #1{~1~2 to }#2 Intelligence theft
        /// </summary>
        ACTION_CHARACTER_STEAL_INTELLIGENCE = 269,
        /// <summary>
        /// #1{~1~2 to }#2 Wisdom theft
        /// </summary>
        ACTION_CHARACTER_STEAL_WISDOM = 270,
        /// <summary>
        /// #1{~1~2 to }#2 Strength theft
        /// </summary>
        ACTION_CHARACTER_STEAL_STRENGTH = 271,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's lost HP (water)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_MISSING_FROM_WATER = 275,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's lost HP (earth)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_MISSING_FROM_EARTH = 276,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's lost HP (air)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_MISSING_FROM_AIR = 277,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's lost HP (fire)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_MISSING_FROM_FIRE = 278,
        /// <summary>
        /// Damage: #1{~1~2 to }#2% of the attacker's lost HP (neutral)
        /// </summary>
        ACTION_CHARACTER_LIFE_POINTS_LOST_BASED_ON_CASTER_LIFE_MISSING = 279,
        /// <summary>
        /// +#1{~1~2 to }#2 pushback damage
        /// </summary>
        Effect_AddPushDamageBonus = 414,
        /// <summary>
        /// -#1{~1~2 to }#2 pushback damage
        /// </summary>
        Effect_SubPushDamageBonus = 415,
        /// <summary>
        /// +#1{~1~2 to }#2 pushback resistance
        /// </summary>
        Effect_AddPushDamageReduction = 416,
        /// <summary>
        /// -#1{~1~2 to }#2 pushback resistance
        /// </summary>
        Effect_SubPushDamageReduction = 417,
        /// <summary>
        /// +#1{~1~2 to }#2 critical damage
        /// </summary>
        Effect_AddCriticalDamageBonus = 418,
        /// <summary>
        /// -#1{~1~2 to }#2 critical damage
        /// </summary>
        Effect_SubCriticalDamageBonus = 419,
        /// <summary>
        /// +#1{~1~2 to }#2 critical resistance
        /// </summary>
        Effect_AddCriticalDamageReduction = 420,
        /// <summary>
        /// -#1{~1~2 to }#2 critical resistance
        /// </summary>
        Effect_SubCriticalDamageReduction = 421,
        /// <summary>
        /// +#1{~1~2 to }#2 Earth damage
        /// </summary>
        Effect_AddEarthDamageBonus = 422,
        /// <summary>
        /// -#1{~1~2 to }#2 Earth damage
        /// </summary>
        Effect_SubEarthDamageBonus = 423,
        /// <summary>
        /// +#1{~1~2 to }#2 Fire damage
        /// </summary>
        Effect_AddFireDamageBonus = 424,
        /// <summary>
        /// -#1{~1~2 to }#2 Fire damage
        /// </summary>
        Effect_SubFireDamageBonus = 425,
        /// <summary>
        /// +#1{~1~2 to }#2 Water damage
        /// </summary>
        Effect_AddWaterDamageBonus = 426,
        /// <summary>
        /// -#1{~1~2 to }#2 Water damage
        /// </summary>
        Effect_SubWaterDamageBonus = 427,
        /// <summary>
        /// +#1{~1~2 to }#2 Air damage
        /// </summary>
        Effect_AddAirDamageBonus = 428,
        /// <summary>
        /// -#1{~1~2 to }#2 Air damage
        /// </summary>
        Effect_SubAirDamageBonus = 429,
        /// <summary>
        /// +#1{~1~2 to }#2 Neutral damage
        /// </summary>
        Effect_AddNeutralDamageBonus = 430,
        /// <summary>
        /// -#1{~1~2 to }#2 Neutral damage
        /// </summary>
        Effect_SubNeutralDamageBonus = 431,
        /// <summary>
        /// Steals #1{~1~2 to }#2 AP
        /// </summary>
        Effect_StealAP_440 = 440,
        /// <summary>
        /// Steals #1{~1~2 to }#2 MP
        /// </summary>
        Effect_StealMP_441 = 441,
        ACTION_CHARACTER_BOOST_DODGE = 752,
        ACTION_CHARACTER_BOOST_TACKLE = 753,
        ACTION_CHARACTER_DEBOOST_DODGE = 754,
        ACTION_CHARACTER_DEBOOST_TACKLE = 755,
        /// <summary>
        /// Created X day(s) ago
        /// </summary>
        ACTION_CREATED_SINCE = 963,
        /// <summary>
        /// Modified by: #4
        /// </summary>
        ACTION_MODIFIED_BY = 985,
        /// <summary>
        /// Made by: #4
        /// </summary>
        ACTION_MADE_BY = 988,
        /// <summary>
        /// !! Invalid Certificate !!
        /// </summary>
        ACTION_INVALID_CERTIFICATE = 994,

        ACTION_CHARACTER_BOOST_SHIELD_BASED_ON_CASTER_LIFE = 1039,
        ACTION_CHARACTER_BOOST_SHIELD = 1040,
        ACTION_CHARACTER_LIFE_POINTS_MALUS = 1047,
        ACTION_CHARACTER_LIFE_POINTS_MALUS_PERCENT = 1048,
        ACTION_BOOST_GLOBAL_RESISTS_BONUS = 1076,
        ACTION_BOOST_GLOBAL_RESISTS_MALUS = 1077
    }
}

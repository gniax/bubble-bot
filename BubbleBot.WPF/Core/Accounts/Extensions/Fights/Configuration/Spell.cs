using System.IO;
using BubbleBot.Core.Accounts.Extensions.Fights.Configuration.Enums;
using Newtonsoft.Json;

namespace BubbleBot.Core.Accounts.Extensions.Fights.Configuration
{
    public class Spell
    {
        // Constructor
        public Spell(int spellId, string spellName, SpellTargets target, byte turns, byte relaunchs, byte targetHp,
            byte characterHp, SpellResistances resistance,
            byte resistanceValue, byte distanceToClosestMonster, bool handToHand, bool aoe, bool carefulAOE,
            bool avoidAllies)
        {
            SpellId = spellId;
            SpellName = spellName;
            Target = target;
            Turns = turns;
            LastTurn = 0;
            Relaunchs = relaunchs;
            RemainingRelaunchs = Relaunchs;
            TargetHp = targetHp;
            CharacterHp = characterHp;
            Resistance = resistance;
            ResistanceValue = resistanceValue;
            DistanceToClosestMonster = distanceToClosestMonster;
            HandToHand = handToHand;
            AOE = aoe;
            CarefulAOE = carefulAOE;
            AvoidAllies = avoidAllies;
        }

        // Properties
        [JsonProperty("SpellId")]
        public int SpellId { get; }

        [JsonProperty("SpellName")]
        public string SpellName { get; }

        [JsonProperty("Target")]
        public SpellTargets Target { get; }

        [JsonProperty("Turns")]
        public byte Turns { get; }

        [JsonProperty("LastTurn")]
        public byte LastTurn { get; set; }

        [JsonProperty("Relaunchs")]
        public byte Relaunchs { get; }

        [JsonProperty("RemainingRelaunchs")]
        public byte RemainingRelaunchs { get; set; }

        [JsonProperty("TargetHp")]
        public byte TargetHp { get; }

        [JsonProperty("CharacterHp")]
        public byte CharacterHp { get; }

        [JsonProperty("Resistance")]
        public SpellResistances Resistance { get; }

        [JsonProperty("ResistanceValue")]
        public byte ResistanceValue { get; }

        [JsonProperty("DistanceToClosestMonster")]
        public byte DistanceToClosestMonster { get; }

        [JsonProperty("HandToHand")]
        public bool HandToHand { get; }

        [JsonProperty("AOE")]
        public bool AOE { get; }

        [JsonProperty("CarefulAOE")]
        public bool CarefulAOE { get; }

        [JsonProperty("AvoidAllies")]
        public bool AvoidAllies { get; }



        public void Save(BinaryWriter bw)
        {
            bw.Write(SpellId);
            bw.Write(SpellName);
            bw.Write((byte) Target);
            bw.Write(Turns);
            bw.Write(Relaunchs);
            bw.Write(TargetHp);
            bw.Write(CharacterHp);
            bw.Write((byte) Resistance);
            bw.Write(ResistanceValue);
            bw.Write(DistanceToClosestMonster);
            bw.Write(HandToHand);
            bw.Write(AOE);
            bw.Write(CarefulAOE);
            bw.Write(AvoidAllies);
        }

        public static Spell Load(BinaryReader br)
        {
            return new Spell(br.ReadInt32(), br.ReadString(), (SpellTargets) br.ReadByte(),
                br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),
                (SpellResistances) br.ReadByte(), br.ReadByte(), br.ReadByte(),
                br.ReadBoolean(), br.ReadBoolean(), br.ReadBoolean(), br.ReadBoolean());
        }
    }
}
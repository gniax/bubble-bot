using System.IO;
using BubbleBot.Core.Accounts.Extensions.Fights.Configuration.Enums;

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
        public int SpellId { get; }
        public string SpellName { get; }
        public SpellTargets Target { get; }
        public byte Turns { get; }
        public byte LastTurn { get; set; }
        public byte Relaunchs { get; }
        public byte RemainingRelaunchs { get; set; }
        public byte TargetHp { get; }
        public byte CharacterHp { get; }
        public SpellResistances Resistance { get; }
        public byte ResistanceValue { get; }
        public byte DistanceToClosestMonster { get; }
        public bool HandToHand { get; }
        public bool AOE { get; }
        public bool CarefulAOE { get; }
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
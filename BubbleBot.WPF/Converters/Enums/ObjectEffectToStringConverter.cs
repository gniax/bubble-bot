using System;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Converters.Enums
{
    public static class ObjectEffectToStringConverter
    {
        public static string Convert(ObjectEffect objectEffect)
        {
            var actionId = objectEffect.ActionId;
            var context = Enum.GetName(typeof(ObjectEffectsInFrenchEnum), actionId);

            if (context == null)
                return "";

            context = context.Replace("_", " ");

            if (actionId == 193 || actionId == 222 || actionId == 620 || actionId == 622 || actionId == 648 ||
                actionId == 795 || actionId == 826 ||
                actionId == 948 || actionId == 994 || actionId == 1153 || actionId == 940)
                return context;

            if (objectEffect is ObjectEffectInteger oei)
            {
                if (actionId != 1151 && actionId != 800 && actionId != 806 && actionId != 807)
                {
                    context = context.Replace("PERCENT", "%");
                    context = context.Replace("LESS", "-");

                    if (context.Contains("-"))
                        context = context.Insert(context.IndexOf("-") + 2, oei.Value + " ");
                    else
                        context = context.Insert(0, oei.Value + " ");
                }
                else if (actionId == 1151 || actionId == 807)
                {
                    string item = null;
                    item = ObjectEnumFinder.GetObjectNameById((int) oei.Value);
                    if (item != null)
                        context += ": " + item;
                    else
                        context += ": " + "??";
                }
                else if (actionId == 800)
                {
                    context += ": " + oei.Value;
                }
                else if (actionId == 806)
                {
                    if (oei.Value < 6)
                        context += ": Normal";
                    else if (oei.Value > 5) context += ": Maigrichon (-" + oei.Value + " repas)";
                }
            }
            else if (objectEffect is ObjectEffectString oes)
            {
                context += ": " + oes.Value;
            }
            else if (objectEffect is ObjectEffectDuration oed)
            {
                context += " " + oed.Days + "j " + oed.Hours + "h " + oed.Minutes + "m";
            }
            else if (objectEffect is ObjectEffectDate oedd)
            {
                var zerom = oedd.Minute > 9 ? "" : "0";
                var zeroD = oedd.Day > 9 ? "" : "0";
                var zeroM = oedd.Month > 9 ? "" : "0";
                context += " " + zeroD + oedd.Day + "/" + zeroM + oedd.Month + "/" + oedd.Year + " " + oedd.Hour + ":" +
                           zerom + oedd.Minute;
            }
            else if (objectEffect is ObjectEffectMinMax oemm)
            {
                context = context.Replace("PERCENT", "%");
                context = context.Replace("LESS", "-");

                if (context.Contains("-"))
                    context = context.Insert(context.IndexOf("-") + 1, oemm.Min + " à " + oemm.Max + " ");
                else
                    context = context.Insert(0, oemm.Min + " à " + oemm.Max + " ");
            }
            else if (objectEffect is ObjectEffectLadder oel)
            {
                if (actionId == 717)
                {
                    var nameMonster = MonstersEnumFinder.GetMonsterNameById((int) oel.MonsterFamilyId);
                    if (nameMonster != null)
                        context = nameMonster + " : " + oel.MonsterCount;
                    else
                        context = "(MonstreID)" + oel.MonsterFamilyId + " : " + oel.MonsterCount;
                }
            }
            else if (objectEffect is ObjectEffectDice oedice)
            {
                if (actionId == 806) context += ": Obèse (+" + oedice.DiceSide + " repas)";
            }

            return context;
        }
    }
}
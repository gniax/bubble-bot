using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Protocol.Enums
{
    public enum ChallengesEnum
    {
        Zombie = 1,
        Statue = 2,
        Désigné_volontaire = 3,
        Sursis = 4,
        Économe = 5,
        Versatile = 6,
        Jardinier = 7,
        Nomade = 8,
        Barbare = 9,
        Cruel = 10,
        Mystique = 11,
        Fossoyeur = 12,
        Casino_Royal = 14,
        Araknophile = 15,
        Intouchable = 17,
        Incurable = 18,
        Mains_propres = 19,
        Elémentaire = 20,
        Circulez_741 = 21,
        Le_temps_qui_court = 22,
        Perdu_de_vue = 23,
        Ordonné = 25,
        Ni_pioutes_ni_soumises = 28,
        Ni_pious_ni_soumis = 29,
        Les_petits_d733abord = 30,
        Focus = 31,
        Elitiste = 32,
        Survivant = 33,
        Imprévisible = 34,
        Tueur_à_gages = 35,
        Hardi = 36,
        Collant = 37,
        Blitzkrieg = 38,
        Anachorète = 39,
        Pusillanime = 40,
        Pétulant = 41,
        Deux_pour_le_prix_d733un = 42,
        Abnégation = 43,
        Partage = 44,
        Duel = 45,
        Chacun_son_monstre = 46,
        Contamination = 47,
        Les_mules_d733abord = 48,
        Protégez_vos_mules = 49,
        Le_cheat_des_devs = 50
    }
    public static class ChallengesEnumFinder
    {
        public static string GetChallengeById(int key)
        {
            string result = null;
            result = Enum.GetName(typeof(ChallengesEnum), key);
            if (result == null)
                return null;

            result = result.Replace("733", "\'");
            result = result.Replace("734", "%");
            result = result.Replace("735", "*");
            result = result.Replace("736", ":");
            result = result.Replace("737", "/");
            result = result.Replace("738", "\"");
            result = result.Replace("739", "(");
            result = result.Replace("740", ")");
            result = result.Replace("741", "!");

            while (result.EndsWith("_")) // Remove this for duplicate spells
                result = result.Remove(result.Length - 1);

            result = result.Replace("_", " ");

            return result;
        }
    }
}

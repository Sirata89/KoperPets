using System;
using Server;
using Server.Mobiles;
using Server.Misc;
using System.Collections.Generic;

namespace Server.Custom.KoperPets
{
    public static class KoperPetNaming
    {
        private static readonly Random getrandom = new Random();
        public static Dictionary<int, KeyValuePair<string, int[]>> AdjectiveModifiers
        {
            get { return _adjectiveModifiers; }
        }
        private static readonly Dictionary<int, KeyValuePair<string, int[]>> _adjectiveModifiers =
            new Dictionary<int, KeyValuePair<string, int[]>>()
        {
            // Strength-Based Adjectives
            {  0, new KeyValuePair<string, int[]>("Mighty",       new int[] { 5,  0,  0, 10,  0,  0,  2,  0,  0,  0,  0,  1,  2 }) },
            {  1, new KeyValuePair<string, int[]>("Brawny",       new int[] { 3, -2,  0,  8,  0,  0,  1,  0,  0,  0,  0,  1,  1 }) },
            {  2, new KeyValuePair<string, int[]>("Hulking",      new int[] { 6, -3,  0, 15,  0,  0,  3,  0,  0,  0,  0,  2,  3 }) },
            {  3, new KeyValuePair<string, int[]>("Brutish",      new int[] { 7, -4,  0, 18,  0,  0,  4,  0,  0,  0,  0,  3,  4 }) },
            {  4, new KeyValuePair<string, int[]>("Colossal",     new int[] { 8, -5,  0, 20,  0,  0,  5,  0,  0,  0,  0,  4,  5 }) },
            {  5, new KeyValuePair<string, int[]>("Titanic",      new int[] { 10,-6,  0, 25,  0,  0,  6,  0,  0,  0,  0,  5,  6 }) },
            {  6, new KeyValuePair<string, int[]>("Behemoth",     new int[] { 12,-7,  0, 30,  0,  0,  7,  0,  0,  0,  0,  6,  7 }) },
            {  7, new KeyValuePair<string, int[]>("Monstrous",   new int[] { 14, -8,  0, 35,  0,  0,  8,  0,  0,  0,  0,  7,  8 }) },
            {  8, new KeyValuePair<string, int[]>("Gargantuan",  new int[] { 16, -9,  0, 40,  0,  0,  9,  0,  0,  0,  0,  8,  9 }) },
            {  9, new KeyValuePair<string, int[]>("Titanborn",   new int[] { 18,-10,  0, 50,  0,  0, 10,  0,  0,  0,  0,  9, 10 }) },

            // Dexterity-Based
            {  10, new KeyValuePair<string, int[]>("Swift",           new int[] { 0,  5,  0,  0, 10,  0,  0,  2,  0,  0,  0,  1,  2 }) },
            {  11, new KeyValuePair<string, int[]>("Fleet-Footed",    new int[] { 0,  7,  0,  0, 15,  0,  0,  3,  0,  0,  0,  2,  3 }) },
            {  12, new KeyValuePair<string, int[]>("Lightning-Fast",  new int[] { 0,  9,  0,  0, 20,  0,  0,  2,  0,  0,  1,  1,  2 }) },
            {  13, new KeyValuePair<string, int[]>("Evasive",         new int[] { 0,  8,  0,  0, 15,  0,  0,  2,  0,  0,  1,  1,  1 }) },
            {  14, new KeyValuePair<string, int[]>("Ghostly",         new int[] { 0, 10,  5,  0, 25, 10,  0,  2,  2,  2,  5,  2,  2 }) },
            {  15, new KeyValuePair<string, int[]>("Shadowy",         new int[] { 0, 12,  3,  0, 30,  5,  1,  2,  3,  3,  3,  3,  3 }) },
            {  16, new KeyValuePair<string, int[]>("Windborne",       new int[] { 0, 14,  0,  0, 35,  0,  0,  4,  2,  2,  1,  4,  4 }) },
            {  17, new KeyValuePair<string, int[]>("Phantom",         new int[] { 0, 16,  6,  0, 40, 12,  0,  3,  4,  4,  6,  5,  5 }) },
            {  18, new KeyValuePair<string, int[]>("Blinding",        new int[] { 0, 18,  0,  0, 45,  0,  0,  5,  3,  3,  2,  5,  6 }) },
            {  19, new KeyValuePair<string, int[]>("Untouchable",     new int[] { 0, 20,  0,  0, 50,  0,  0,  6,  4,  4,  3,  6,  7 }) },

            // Magic-Aligned
            {  20, new KeyValuePair<string, int[]>("Runic",        new int[] { 0,  0,  7,  0,  0, 20,  0,  0,  6,  0,  5,  2,  3 }) },
            {  21, new KeyValuePair<string, int[]>("Eldritch",     new int[] { 0,  0, 10,  0,  0, 25,  0,  0,  5,  0,  3,  2,  3 }) },
            {  22, new KeyValuePair<string, int[]>("Sorcerous",    new int[] { 0,  0,  9,  0,  0, 15,  0,  0,  4,  0,  3,  1,  2 }) },
            {  23, new KeyValuePair<string, int[]>("Mystic",       new int[] { 0,  0, 12,  0,  0, 30,  0,  2,  6,  0,  6,  3,  4 }) },
            {  24, new KeyValuePair<string, int[]>("Arcane",       new int[] { 0,  0, 14,  0,  0, 35,  0,  3,  7,  1,  7,  4,  5 }) },
            {  25, new KeyValuePair<string, int[]>("Occult",       new int[] { 0,  0, 16,  0,  0, 40,  0,  4,  8,  2,  8,  5,  6 }) },
            {  26, new KeyValuePair<string, int[]>("Necrotic",     new int[] { 0,  0, 18,  0,  0, 45,  0,  3,  5, 10,  9,  6,  7 }) },
            {  27, new KeyValuePair<string, int[]>("Druidic",      new int[] { 0,  0, 17,  5,  5, 40,  3,  5,  8,  5,  7,  5,  5 }) },
            {  28, new KeyValuePair<string, int[]>("Runebound",    new int[] { 0,  0, 20,  0,  0, 50,  0,  2, 10,  3, 12,  7,  8 }) },
            {  29, new KeyValuePair<string, int[]>("Hexed",        new int[] { 0,  0, 15,  0,  0, 35,  0,  5,  6,  8,  5,  4,  4 }) },
            {  30, new KeyValuePair<string, int[]>("Warlock's",    new int[] { 0,  0, 19,  0,  0, 45,  1,  2,  7,  2,  9,  6,  6 }) },
            {  31, new KeyValuePair<string, int[]>("Spellwoven",   new int[] { 0,  0, 21,  0,  0, 55,  2,  6,  8,  3, 10,  8,  9 }) },

            // Tanky & Resilient
            {  32, new KeyValuePair<string, int[]>("Resilient",    new int[] { 0,  0,  0, 20, 10,  5,  8,  3,  4,  2,  1,  0,  0 }) },
            {  33, new KeyValuePair<string, int[]>("Unyielding",   new int[] { 6, -1,  0, 25,  0,  0,  7,  2,  4,  1,  1,  3,  4 }) },
            {  34, new KeyValuePair<string, int[]>("Stalwart",     new int[] { 4,  0,  0, 30,  5,  0, 10,  4,  5,  3,  2,  2,  3 }) },
            {  35, new KeyValuePair<string, int[]>("Adamant",      new int[] { 5, -1,  0, 35, 10,  0, 12,  5,  6,  4,  3,  3,  4 }) },
            {  36, new KeyValuePair<string, int[]>("Ironclad",     new int[] { 8, -2,  0, 40,  5,  0, 15,  6,  8,  5,  4,  4,  5 }) },

            // Damage & Attack-Focused
            {  37, new KeyValuePair<string, int[]>("Fierce",       new int[] { 4,  0,  0,  5,  0,  0,  3,  2,  1,  0,  0,  2,  3 }) },
            {  38, new KeyValuePair<string, int[]>("Savage",       new int[] { 6,  2,  0,  5,  5,  0,  3,  1,  1,  1,  0,  3,  4 }) },
            {  39, new KeyValuePair<string, int[]>("Brutal",       new int[] { 7, -1,  0,  7,  0,  0,  4,  2,  1,  1,  0,  3,  5 }) },
            {  40, new KeyValuePair<string, int[]>("Bloodthirsty", new int[] { 8,  0, -2, 10,  0,  0,  2,  3,  2,  0,  0,  4,  6 }) },
            {  41, new KeyValuePair<string, int[]>("Ruthless",     new int[] { 9,  1, -1,  8,  2,  0,  3,  2,  2,  1,  0,  5,  7 }) },

            // Elemental
            {  42, new KeyValuePair<string, int[]>("Infernal",      new int[] { 0,  0,  0,  5,  0, 10,  0, 10,  0,  5,  0,  2,  3 }) },
            {  43, new KeyValuePair<string, int[]>("Frozen",        new int[] { 0,  0,  0,  5,  0, 10,  0,  0, 10,  5,  0,  2,  3 }) },
            {  44, new KeyValuePair<string, int[]>("Toxic",         new int[] { 0,  0,  0,  5,  0, 10,  0,  0,  0, 10,  5,  2,  3 }) },
            {  45, new KeyValuePair<string, int[]>("Stormforged",   new int[] { 0,  0,  0,  5,  0, 10,  0,  0,  0,  0, 10,  2,  3 }) },
            {  46, new KeyValuePair<string, int[]>("Magma-Touched", new int[] { 2,  0,  0, 10,  0,  5,  0, 12,  2,  3,  0,  3,  4 }) },
            {  47, new KeyValuePair<string, int[]>("Arctic",        new int[] { 0,  0,  2,  5,  0, 15,  0,  0, 12,  6,  0,  2,  3 }) },
            {  48, new KeyValuePair<string, int[]>("Venomous",      new int[] { 0,  2,  0,  5,  5,  5,  0,  0,  0, 12,  6,  3,  4 }) },
            {  49, new KeyValuePair<string, int[]>("Thunderous",    new int[] { 3,  0,  3,  7,  3, 10,  0,  0,  0,  0, 15,  4,  5 }) },
            {  50, new KeyValuePair<string, int[]>("Abyssal",       new int[] { 0,  0,  5,  8,  0, 20,  5,  5,  5,  5,  5,  3,  5 }) },
            {  51, new KeyValuePair<string, int[]>("Gale-Touched",  new int[] { 0,  5,  0,  0, 15,  5,  0,  0,  0,  0,  8,  2,  3 }) },

            // Mythical & Legendary
            {  52, new KeyValuePair<string, int[]>("Celestial",       new int[] { 10, 10, 15, 20, 20, 20,  5,  5,  5,  5,  5,  4,  4 }) },
            {  53, new KeyValuePair<string, int[]>("Wyrm-Touched",    new int[] { 12,  8, 12, 25, 15, 25,  6,  6,  6,  6,  6,  5,  5 }) },
            {  54, new KeyValuePair<string, int[]>("Ancient",         new int[] {  14,  14, 14, 30, 20, 30,  7,  7,  7,  7,  7,  6,  6 }) },
            {  55, new KeyValuePair<string, int[]>("Eternal",         new int[] { 15, 15, 15, 35, 25, 35,  8,  8,  8,  8,  8,  7,  7 }) },
            {  56, new KeyValuePair<string, int[]>("Primordial",      new int[] { 18, 18, 18, 40, 30, 40, 10, 10, 10, 10, 10,  8,  8 }) },
            {  57, new KeyValuePair<string, int[]>("Godforged",       new int[] { 20, 20, 20, 50, 35, 50, 12, 12, 12, 12, 12,  9,  9 }) },
            {  58, new KeyValuePair<string, int[]>("Transcendent",    new int[] { 25, 25, 25, 60, 40, 60, 15, 15, 15, 15, 15, 10, 10 }) },

            // Cursed / Negative
            {  59, new KeyValuePair<string, int[]>("Dull",          new int[] { -2, -2, -2,  -3,  -3,  -3,  -1,  -1,  -1,  -1,  -1,  0,  0 }) },
            {  60, new KeyValuePair<string, int[]>("Frail",         new int[] { -3, -3, -3,  -5,  -5,  -5,  -2,  -2,  -2,  -2,  -2, -1, -1 }) },
            {  61, new KeyValuePair<string, int[]>("Tattered",      new int[] { -4, -3, -2,  -7,  -7,  -7,  -2,  -2,  -2,  -2,  -2, -1, -1 }) },
            {  62, new KeyValuePair<string, int[]>("Withered",      new int[] { -5, -5, -5, -10, -10, -10,  -3,  -3,  -3,  -3,  -3, -2, -2 }) },
            {  63, new KeyValuePair<string, int[]>("Blighted",      new int[] { -8, -3, -5, -15, -15, -15,  -4,  -4,  -4,  -4,  -4, -3, -3 }) },
            {  64, new KeyValuePair<string, int[]>("Accursed",      new int[] {-10, -5, -8, -20, -20, -20,  -5,  -5,  -5,  -5,  -5, -4, -4 }) },
            {  65, new KeyValuePair<string, int[]>("Cracked",       new int[] { -6, -6, -4, -12, -12, -12,  -4,  -4,  -4,  -4,  -4, -3, -3 }) },
            {  66, new KeyValuePair<string, int[]>("Rotting",       new int[] { -7, -4, -6, -14, -14, -14,  -4,  -4,  -4,  -4,  -4, -3, -3 }) },
            {  67, new KeyValuePair<string, int[]>("Decayed",       new int[] { -9, -7, -7, -18, -18, -18,  -5,  -5,  -5,  -5,  -5, -4, -4 }) },
            {  68, new KeyValuePair<string, int[]>("Malformed",     new int[] {-10, -8, -6, -22, -22, -22,  -6,  -6,  -6,  -6,  -6, -5, -5 }) },
            {  69, new KeyValuePair<string, int[]>("Corrupted",     new int[] {-12, -6, -9, -25, -25, -25,  -7,  -7,  -7,  -7,  -7, -5, -5 }) },
            {  70, new KeyValuePair<string, int[]>("Wretched",      new int[] {-13, -9, -10, -28, -28, -28,  -8,  -8,  -8,  -8,  -8, -6, -6 }) },
            {  71, new KeyValuePair<string, int[]>("Cursed",        new int[] {-15,-10,-12, -30, -30, -30,  -9,  -9,  -9,  -9,  -9, -7, -7 }) },
            {  72, new KeyValuePair<string, int[]>("Forsaken",      new int[] {-18,-12,-15, -35, -35, -35, -10, -10, -10, -10, -10, -8, -8 }) },
            {  73, new KeyValuePair<string, int[]>("Doomed",        new int[] {-20,-15,-18, -40, -40, -40, -12, -12, -12, -12, -12, -9, -9 }) },
            {  99, new KeyValuePair<string, int[]>("DEBUG",         new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }) }
        };

        public static int GetRandomAdjective(KoperPetData newPet, BaseCreature oldPet)
        {
            int index = GetRandomNumber();
            
            if (AdjectiveModifiers.ContainsKey(index))
            {
                //Console.WriteLine("Got adjective index");
                return index;
            }
            return 99;
        }

        public static void ApplyAdjectiveModifiers(BaseCreature pet, int index)
        {
            if (!AdjectiveModifiers.ContainsKey(index))
            {
                Console.WriteLine("[ApplyAdjectiveModifiers] ERROR: Invalid index: " + index);
                return;
            }

            int[] modifiers = AdjectiveModifiers[index].Value; // Retrieve the modifier array

            // Apply base stats
            pet.RawStr += modifiers[0];
            pet.RawDex += modifiers[1];
            pet.RawInt += modifiers[2];

            // Apply independent HP, Stamina, and Mana bonuses
            pet.HitsMaxSeed += modifiers[3];
            pet.StamMaxSeed += modifiers[4];
            pet.ManaMaxSeed += modifiers[5];

            // Apply resistances
            pet.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, modifiers[6]));
            pet.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, modifiers[7]));
            pet.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, modifiers[8]));
            pet.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, modifiers[9]));
            pet.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, modifiers[10]));

            // Apply min/max damage bonuses
            pet.DamageMin += modifiers[11];
            pet.DamageMax += modifiers[12];

            //Console.WriteLine("Applied Adjective");
        }

        private static int GetRandomNumber()
        {
            lock(getrandom) // synchronize
            {
                return getrandom.Next(0, AdjectiveModifiers.Count -1 ); // do not include 99, fallback/default stats
            }
        }

        public static void RenamePet(BaseCreature pet, string adjective)
        {
            if (pet == null || pet.ControlMaster == null)
                return;

            string baseName = (pet.Name != null) ? pet.Name : "creature"; // Ensure a default name
            baseName = StripExistingArticle(baseName);
            string newName = FixGrammar(baseName, adjective); // Ensure correct grammar

            pet.Name = newName.ToLower(); // This triggers the Mobile.cs setter
            pet.Delta(MobileDelta.Name); // Forces client update
            //Console.WriteLine("[KoperPetManager] Renaming pet: " + pet.Serial);

        }

        private static string StripExistingArticle(string baseName)
        {
            string[] words = baseName.Split(' ');

            if (words.Length > 1 && (words[0].ToLower() == "a" || words[0].ToLower() == "an"))
            {
                return string.Join(" ", words, 1, words.Length - 1); // Reconstruct name without article
            }

            return baseName; // Return unchanged if no article found
        }


        // Fixes grammar by properly placing "a" or "an" before the adjective
        private static string FixGrammar(string baseName, string adjective)
        {
            string article = NeedsAn(adjective) ? "an" : "a"; // Determine correct article
            return (article + " " + adjective + " " + baseName).ToLower();
        }

        // Determines if an adjective should use "an" instead of "a"
        private static bool NeedsAn(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            char firstLetter = Char.ToLower(word[0]);
            return "aeiou".IndexOf(firstLetter) >= 0; // Uses "an" if the first letter is a vowel
        }

        public static string GetPedigreeName(int pedigree)
        {
            switch (pedigree)
            {
                case 0: return "Wild-Born";
                case 1: return "Lesser Bred";
                case 2: return "Well-Bred";
                case 3: return "Noble Line";
                case 4: return "Purebred";
                case 5: return "Ascendant Bloodline";
                default: return "Unknown Lineage"; // Fallback in case of unexpected values
            }
        }

    }

}
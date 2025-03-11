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

        public static string GetAdjectiveDescription(int value)
        {
            string description;

            if (AdjectiveDescriptions.TryGetValue(value, out description))
            {
                return description;
            }
            return "No description available for this trait.";
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

        private static readonly Dictionary<int, string> AdjectiveDescriptions = new Dictionary<int, string>()
        {
            // Strength based
            { 0,  "Forged in the fires of valor, this creature possesses unyielding strength, able to topple foes with sheer force." },
            { 1,  "Broad of shoulder and thick of limb, this beast is built for power, though at the cost of some agility." },
            { 2,  "A massive frame, thick as the walls of Blackthorn’s keep, grants this creature incredible durability, yet it moves ponderously." },
            { 3,  "Savage and untamed, this brute crashes through enemies with unrelenting might, though its reflexes suffer for its bulk." },
            { 4,  "Resembling the legendary colossi of ancient Sosaria, this beast towers over others, a true juggernaut of muscle and sinew." },
            { 5,  "Carrying the strength of a forgotten Titan, this creature moves mountains with its blows but lacks the nimbleness of lesser beings." },
            { 6,  "As large as the Behemoths of old, this beast's might is matched only by its hunger for battle, leaving destruction in its wake." },
            { 7,  "Drenched in the blood of foes, this monstrous titan wields unmatched strength, though its agility is dulled by sheer mass." },
            { 8,  "Born of primordial chaos, this gargantuan creature shakes the ground with each step, an unstoppable force of raw power." },
            { 9,  "Carrying the legacy of the Titans themselves, this being is as close to a living legend as the world will ever see." },
            // Dex based
            { 10, "This creature moves with remarkable swiftness, darting through the battlefield like a trained fencer avoiding a mortal strike." },
            { 11, "Graceful and fleet-footed, it glides across terrain as though the very winds of Britannia guide its steps." },
            { 12, "Lightning-fast, this creature’s reflexes blur before the eye, its strikes and evasions as sudden as a tempest’s fury." },
            { 13, "Elusive and evasive, it weaves through combat with unnatural grace, making even the most seasoned warriors struggle to land a blow." },
            { 14, "Ghostly in movement, this being flickers between positions as though stepping through the Veil, always one step ahead of its foes." },
            { 15, "A creature of shadow, it vanishes into dim light and reappears with uncanny speed, its presence felt only when it strikes." },
            { 16, "Born of the wind, this beast moves as if weightless, its form untethered by the laws of men and nature alike." },
            { 17, "A phantom of the battlefield, it drifts and lunges with eerie precision, its touch cold as a wraith’s whisper." },
            { 18, "So fast it seems to shimmer, its blinding movements confound the eye, making it appear as if it exists in two places at once." },
            { 19, "Untouchable, it dodges with supernatural ease, as though Fate itself denies its enemies the chance to land a blow." },
            // Int Based
            { 20, "This creature hums with runic energy, its body marked with glowing sigils of forgotten languages, channeling arcane forces through its very being." },
            { 21, "Eldritch power flows through its form, whispering secrets of the Void. It exudes an eerie aura, unsettling even the most seasoned mages." },
            { 22, "Infused with sorcerous might, it draws unseen energies from the ether, bending elemental forces to its will with each movement." },
            { 23, "A mystic force surrounds this creature, its mind in tune with celestial harmonies, its actions guided by unseen cosmic currents." },
            { 24, "Arcane symbols dance in its wake, the very fabric of magic bending slightly around its presence as if reality itself takes notice." },
            { 25, "This beast carries the mark of the occult, its gaze hinting at forbidden knowledge. Strange omens seem to follow wherever it treads." },
            { 26, "A necrotic chill emanates from its form, the dark power of undeath coursing through its veins, bound by twisted sorcery." },
            { 27, "Steeped in druidic power, it resonates with nature’s balance. Vines and leaves seem to stir as it passes, and the winds carry its whispers." },
            { 28, "Ancient runes pulse upon its hide, binding it to forgotten oaths. Its very existence is entwined with the ley lines of the world." },
            { 29, "Hexed by unseen forces, this creature carries a lingering curse. Dark magic clings to it, warping reality in small but noticeable ways." },
            { 30, "A warlock’s familiar, this being pulses with raw infernal magic, its essence bound to long-forgotten pacts with entities beyond mortal ken." },
            { 31, "Spellwoven energies dance through its body, its form stitched together by strands of pure magic, granting it an uncanny presence." },
            // Tanky
            { 32, "Forged by hardship, this creature's hide is as tough as orcish battle plate. It weathers blows that would shatter lesser beasts." },
            { 33, "An unyielding force of nature, this beast does not falter, standing its ground even in the face of overwhelming odds." },
            { 34, "Stalwart and steadfast, this creature moves with the discipline of a trained guardian, resisting harm with unwavering resolve." },
            { 35, "Adamant in both body and spirit, its form is dense and unbreakable, shrugging off wounds that would cripple most creatures." },
            { 36, "Ironclad and impervious, its armored flesh gleams like a knight’s polished breastplate, each step shaking the earth beneath it." },
            // DPS
            { 37, "Fierce and untamed, this creature fights with an unrelenting spirit, its fangs and claws striking with primal fury." },
            { 38, "A savage hunter of the wilds, it tears into foes with a relentless aggression, its instincts honed for battle." },
            { 39, "Brutal and merciless, each strike is delivered with the force of a war hammer, breaking bones and rending flesh with ease." },
            { 40, "Bloodthirsty and insatiable, this beast revels in combat, its attacks growing ever more frenzied as it tastes victory." },
            { 41, "Ruthless in the pursuit of dominance, it strikes with calculated precision, seeking the quickest path to a foe’s demise." },
            // Elemental
            { 42, "Wreathed in unholy flame, this creature bears the mark of the Abyss, its essence burning with demonic fire." },
            { 43, "A being of endless winter, its presence chills the air, and its touch saps warmth from the living." },
            { 44, "Reeking of decay and venom, this beast’s body seeps with lethal toxins, making every wound fester." },
            { 45, "Crackling with arcane electricity, this creature surges with the power of Tempest, its strikes like bolts from the heavens." },
            { 46, "Born from molten depths, its body pulses with liquid fire, searing all who dare approach its wrath." },
            { 47, "As if sculpted from permafrost, its icy form is unyielding, and its breath carries the sting of glacial winds." },
            { 48, "Coiled with a serpent’s cunning, this creature’s venom is a slow death, paralyzing and rotting its prey from within." },
            { 49, "A living storm, its fury crashes like rolling thunder, each attack charged with the raw power of the skies." },
            { 50, "Abyssal energies radiate from its form, its dark influence warping the very air around it, whispering secrets of the void." },
            { 51, "Swept by the eternal winds, it moves with uncanny grace, its speed a blur that no mortal eye can follow." },
            // Mythical/Lgendary
            { 52, "Bathed in divine radiance, this creature moves with celestial grace, a living avatar of the virtues themselves." },
            { 53, "Bearing the essence of dragons, its scales shimmer with arcane power, and its breath carries the whispers of wyrmkin." },
            { 54, "Older than the histories of men, this being has walked the ages, its presence alone an echo of forgotten eras." },
            { 55, "Bound by neither time nor decay, this creature endures eternally, its body and soul untouched by the ravages of fate." },
            { 56, "A remnant of the world's first dawn, its primal might is unmatched, its power woven into the very fabric of Sosaria." },
            { 57, "Forged by the hands of gods, its form is flawless, radiating the unyielding strength of celestial craftsmanship." },
            { 58, "Beyond all earthly limits, this being exists on a higher plane, a force of nature unbound by mortal constraints." },
            // Cursed/Negative
            { 59, "This creature lacks vigor and spark, its dull eyes betraying an absence of strength or spirit." },
            { 60, "Gaunt and feeble, it moves with great effort, each motion a struggle against its own frailty." },
            { 61, "Ravaged by time or torment, its tattered form bears the scars of past suffering and neglect." },
            { 62, "A husk of its former self, this withered beast seems barely held together by will alone." },
            { 63, "Plagued by unseen afflictions, its blighted flesh festers with unnatural sickness." },
            { 64, "Dark forces cling to this beast, its accursed form radiating an aura of misfortune." },
            { 65, "Its hide is cracked and brittle, flaking away as if unable to contain its own failing essence." },
            { 66, "A stench of decay follows this rotting creature, its flesh sloughing with every labored step." },
            { 67, "Barely more than bone and sinew, this decayed being teeters between life and oblivion." },
            { 68, "Its malformed body is twisted and unnatural, a mockery of what it was meant to be." },
            { 69, "Something sinister lurks within its corrupted soul, its very presence unsettling to behold." },
            { 70, "Misshapen and miserable, this wretched beast suffers under an invisible, ceaseless agony." },
            { 71, "A dark omen follows this cursed creature, as though its very existence invites disaster." },
            { 72, "Abandoned by fate, this forsaken beast is shunned even by the natural order." },
            { 73, "Doomed from birth or by misdeed, its mere presence chills the bones of those who look upon it." },
            { 99, "DEBUG: This should not be visible. If you see this, something is broken." }
        };

        public static string GetAdjectiveName(KoperPetData petData)
        {
            if (petData == null)
                return "NULL";

            return AdjectiveModifiers[petData.Adjective].Key;
        }
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
            lock (getrandom) // synchronize
            {
                return getrandom.Next(0, AdjectiveModifiers.Count - 1); // do not include 99, fallback/default stats
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

        public static List<string> SplitToLines(string text, int maxLineLength)
        {
            List<string> lines = new List<string>();

            while (text.Length > maxLineLength)
            {
                int splitIndex = text.LastIndexOf(' ', maxLineLength); // Find last space within limit

                if (splitIndex == -1) // If no space found, take the whole line
                    splitIndex = maxLineLength;

                lines.Add(text.Substring(0, splitIndex).Trim()); // Add the trimmed valid line
                text = text.Substring(splitIndex + 1); // Move past the space, avoiding extra spaces
            }

            if (text.Length > 0) // Add remaining text
                lines.Add(text.Trim());

            return lines;
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
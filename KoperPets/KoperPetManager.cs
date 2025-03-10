//TODO: refactor level when show to player as a seperate progression, maybe tiers?
//TODO: add peddigree, ie wild for wild tamed creatures, 1st, 2nd gen, purebred, etc.
//TODO: do we make leveling apply the adjective modifiers each level? cap level at 5, then create an maxlevel for each stage of pedigree?

/*
Add this to to the end of the taming skill method where you see comments about successfully taming, and the owner is assigned:

							#region KoperPets
							Custom.KoperPetManager.RegisterPet(m_Creature);
							#endregion

Add this to public override void   OnDamage( int amount, Mobile from, bool willKill ) in basecreature.cs
at the bottom, after 			       base.OnDamage( amount, from, willKill );

			#region KoperPets
    		if (from == null)
        		return;

			BaseCreature petAttacker = from as BaseCreature;
			if (petAttacker != null && petAttacker.Controlled)
			{
    			PlayerMobile owner = petAttacker.ControlMaster as PlayerMobile;
    			if (owner != null)
    			{
        			// Pet successfully hit an enemy, award XP
        			Server.Custom.KoperPets.KoperPetManager.AwardXP(petAttacker, this);
        			Server.Custom.KoperPets.PetTamingSkillGain.TryTamingGain(petAttacker, this);
    			}
			}
			#endregion
*/

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Mobiles;
using Server.Misc;
using Server.Items;

namespace Server.Custom.KoperPets
{
    public static class KoperPetManager
    {
        public static readonly string saveFilePath = Path.Combine(Core.BaseDirectory, "Saves/Mobiles/KoperPets.bin");
        private static Dictionary<Serial, KoperPetData> petDataCache = new Dictionary<Serial, KoperPetData>();
        private static bool enableKoper = true;

        //private static Dictionary<PlayerMobile, DateTime> _breedingCooldown = new Dictionary<PlayerMobile, DateTime>();
        // convert to MySettings config option

        // AdjectiveModifiers Dictionary Structure:
        // Each adjective applies a set of modifiers to a pet's stats.
        // The array values correspond to the following attributes:
        //
        //  0 - Strength Bonus (RawStr)
        //  1 - Dexterity Bonus (RawDex)
        //  2 - Intelligence Bonus (RawInt)
        //  3 - Max Health Bonus (HitsMaxSeed)
        //  4 - Max Stamina Bonus (StamMaxSeed)
        //  5 - Max Mana Bonus (ManaMaxSeed)
        //  6 - Min Damage Bonus
        //  7 - Max Damage Bonus
        //  8 - Physical Resistance Bonus
        //  9 - Fire Resistance Bonus
        // 10 - Cold Resistance Bonus
        // 11 - Poison Resistance Bonus
        // 12 - Energy Resistance Bonus

        public static void RegisterPet(BaseCreature pet, KoperPetData preAssignedData = null)
        {
            if (pet == null || !pet.Controlled || pet.ControlMaster == null)
                return;

            if (petDataCache.ContainsKey(pet.Serial))
            {
                Console.WriteLine("[KoperPetManager] Pet already exists in cache: " + pet.Serial);
                return;
            }

            Console.WriteLine("[KoperPetManager] Registering new pet: " + pet.Serial);

            // If bred, use provided data; otherwise, create new data
            KoperPetData newPetData = preAssignedData ?? new KoperPetData(pet.Serial);

            if (preAssignedData == null) // Tamed Pet Case
            {
                newPetData.Adjective = KoperPetNaming.GetRandomAdjective(newPetData, pet);
                newPetData.MaxLevel = 5;
            }

            // Explicitly Set MaxLevel if PreAssigned
            if (preAssignedData != null && preAssignedData.MaxLevel > 0)
            {
                newPetData.MaxLevel = preAssignedData.MaxLevel;
                Console.WriteLine("Maxlevel = ", preAssignedData.MaxLevel);
            }

            // Store in cache
            petDataCache[pet.Serial] = newPetData;

            // Apply inherited or generated stats
            KoperPetNaming.ApplyAdjectiveModifiers(pet, newPetData.Adjective);
            KoperPetNaming.RenamePet(pet, KoperPetNaming.AdjectiveModifiers[newPetData.Adjective].Key);
            newPetData.Gender = SetGender(pet);

            Console.WriteLine("[KoperPetManager] Pet registered successfully: " + pet.Serial);
        }

        public static void SaveAllPets()
        {
            Console.WriteLine("[KoperPetManager] Saving all pets...");

            if (petDataCache.Count == 0)
            {
                Console.WriteLine("[KoperPetManager] No pets in cache to save.");
                return;
            }

            List<Serial> toRemove = new List<Serial>();

            using (BinaryWriter writer = new BinaryWriter(File.Open(saveFilePath, FileMode.Create)))
            {
                int validPetCount = 0;

                foreach (KeyValuePair<Serial, KoperPetData> entry in petDataCache)
                {
                    BaseCreature pet = World.FindMobile(entry.Key) as BaseCreature;

                    if (pet == null || pet.Deleted || !pet.Alive || pet.Hits <= 0) // Remove dead/deleted pets
                    {
                        Console.WriteLine("[KoperPetManager] Removing dead or missing pet: " + entry.Key);
                        toRemove.Add(entry.Key);
                        continue;
                    }

                    validPetCount++; // Count only active pets
                }

                writer.Write(validPetCount); // Save actual number of valid pets

                foreach (KeyValuePair<Serial, KoperPetData> entry in petDataCache)
                {
                    if (toRemove.Contains(entry.Key))
                        continue; // Skip saving removed pets

                    KoperPetData pet = entry.Value;

                    Console.WriteLine("[KoperPetManager] Writing pet to save file: " + pet.Serial);

                    writer.Write(pet.Serial.Value);
                    writer.Write(pet.Experience);
                    writer.Write(pet.Level);
                    writer.Write(pet.MaxLevel);
                    writer.Write(pet.Traits);
                    writer.Write(pet.Gender);
                    writer.Write(pet.Adjective);
                    writer.Write(pet.Pedigree);
                    writer.Write(pet.LastBreedingTime.ToBinary());
                }

                writer.Flush(); // Ensure all data is written before closing
            }

            // Remove stale pets from cache AFTER saving
            foreach (Serial serial in toRemove)
            {
                petDataCache.Remove(serial);
            }

            Console.WriteLine("[KoperPetManager] Save complete. Removed {0} inactive pets.", toRemove.Count);
        }

        public static void LoadAllPets()
        {
            if (!File.Exists(saveFilePath))
            {
                Console.WriteLine("[KoperPetManager] No save file found. Creating a new one...");
                SaveAllPets();
                return;
            }

            Console.WriteLine("[KoperPetManager] Loading pets...");

            petDataCache.Clear();

            using (BinaryReader reader = new BinaryReader(File.Open(saveFilePath, FileMode.Open)))
            {
                int petCount = reader.ReadInt32(); // Read total pet count

                for (int i = 0; i < petCount; i++)
                {
                    Serial serial = (Serial)reader.ReadInt32();
                    BaseCreature pet = World.FindMobile(serial) as BaseCreature;

                    // **Skip pets that no longer exist or are dead, but confirm bonded logic
                    if ( pet == null || pet.Deleted || !pet.Alive || pet.Hits <= 0)
                    {
                            Console.WriteLine("[KoperPetManager] Removing dead or missing pet: " + serial);
                            continue;
                    }

                    KoperPetData petData = new KoperPetData(serial);
                    petData.Experience = reader.ReadInt32();
                    petData.Level = reader.ReadInt32();
                    petData.MaxLevel = reader.ReadInt32();
                    petData.Traits = reader.ReadInt32();
                    petData.Gender = reader.ReadInt32(); // Gender is now an int (0 = Female, 1 = Male)
                    petData.Adjective = reader.ReadInt32(); // Load as int index
                    petData.Pedigree = reader.ReadInt32();
                    petData.LastBreedingTime = DateTime.FromBinary(reader.ReadInt64());
                    

                    petDataCache[serial] = petData;
                    Console.WriteLine("[KoperPetManager] Loaded pet into cache: " + serial);
                }
            }

            Console.WriteLine("[KoperPetManager] Pet loading complete.");
        }

        public static int GetExperience(BaseCreature pet)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                return petDataCache[pet.Serial].Experience;
            }
            return 0;
        }

        public static int GetLevel(BaseCreature pet)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                return petDataCache[pet.Serial].Level;
            }
            return 1;
        }

        public static int GetMaxLevel(BaseCreature pet)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                return petDataCache[pet.Serial].MaxLevel;
            }
            return 5;
        }

        public static int GetTraits(BaseCreature pet)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                return petDataCache[pet.Serial].Traits;
            }
            return 0;
        }

        public static void SetTraits(BaseCreature pet, int traits)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                petDataCache[pet.Serial].Traits = traits;
            }
        }

        private static int SetGender(BaseCreature pet)
        {
            return (pet.Serial.Value % 2 == 0) ? 0 : 1;
        }

        public static string GetGender(BaseCreature pet)
        {
            if (petDataCache[pet.Serial].Gender == 1)
            {
                return "Male";
            }
            else if (petDataCache[pet.Serial].Gender == 0)
            {
                return "Female";
            }
            else return "Default";
        }

        public static int GetPedigree(BaseCreature pet)
        {
            return petDataCache[pet.Serial].Pedigree;
        }

        public static int SetPedigree(BaseCreature pet, int p)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                petDataCache[pet.Serial].Pedigree = p;
            }
            
            return 0;
        }

        public static KoperPetData GetPetData(BaseCreature pet)
        {
            if (petDataCache.ContainsKey(pet.Serial))
            {
                return petDataCache[pet.Serial];
            }
            return null;
        }

        public static void AwardXP(BaseCreature pet, Mobile attacked)
        {
            if (pet == null || attacked == null)
                return;

            KoperPetData petData = GetPetData(pet);
            if (petData == null)
                return;

            int baseXP = Math.Max(1, attacked.HitsMax / 10); // Base XP from HP
            int karmaBonus = Math.Max(1, attacked.Karma / 100); // Bonus XP from Karma
            int fameBonus = Math.Max(1, attacked.Fame / 100); // Bonus XP from Fame

            int totalXP = Math.Max(5, baseXP + karmaBonus + fameBonus);

            petData.Experience += totalXP;

            Console.WriteLine(string.Format("[KoperPetManager] {0} gained {1} XP for attacking {2}.", pet.Name, totalXP, attacked.Name));

            CheckLevelUp(pet, petData);
        }

        public static void GainExperience(BaseCreature pet, int amount)
        {
            if (!petDataCache.ContainsKey(pet.Serial))
            {
                RegisterPet(pet); // Ensure pet is registered before modifying data
            }

            KoperPetData data = petDataCache[pet.Serial];
            data.Experience += amount;

            // Check if pet should level up
            while (data.Experience >= data.Level * 100 && data.Level < data.MaxLevel)
            {
                data.Experience -= data.Level * 100;
                data.Level++;
                data.Traits+=3;
                pet.Say("I have leveled up to level " + data.Level + "!");
            }

            petDataCache[pet.Serial] = data; // Ensure updated data is saved
        }

        private static void CheckLevelUp(BaseCreature pet, KoperPetData petData)
        {
            if (pet == null || petData == null)
                return;

            int requiredXP = petData.Level * (petData.Pedigree + 1) * 100; // XP needed to level up (a level 1, pedigree 0, would require 100. A level 1 & 5 pedigree would require 500)

            while (petData.Experience >= requiredXP && petData.Level < petData.MaxLevel)
            {
                petData.Experience -= requiredXP; // Subtract required XP
                petData.Level++; // Increase Level
                petData.Traits++; // Award 1 Trait Point

                Console.WriteLine(string.Format("[KoperPetManager] {0} leveled up to {1}!", pet.Name, petData.Level));

                requiredXP = petData.Level * 100; // Update XP requirement for next level
            }

            if (petData.Level >= petData.MaxLevel)
            {
                petData.Experience = requiredXP - 1; // Prevents gaining more XP
                Console.WriteLine(string.Format("[KoperPetManager] {0} has reached max level!", pet.Name));

            }
        }
    }

    public class KoperPetData
    {
        public Serial Serial { get; private set; }
        public int Adjective { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public int Traits { get; set; }
        public int Gender { get; set; }
        public int Pedigree { get; set; }
        public DateTime LastBreedingTime {get; set;}

        public KoperPetData(Serial serial)
        {
            Serial = serial;
            Adjective = -1;
            Gender = -1;
            Experience = 0;
            Level = 1;
            MaxLevel = 5;
            Traits = 0;
            Pedigree = 0;
            LastBreedingTime = DateTime.UtcNow;
        }
    }
}
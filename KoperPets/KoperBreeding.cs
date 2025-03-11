/*

Example Outcomes - Pedigree
Parent 1	Parent 2	Child Pedigree
  0 (wild)	  0 (wild)	     1
  1	          1	             2
  2           0              2 (not increasing)
  2 	      3           	 3
  4	          5	             5 (cap)
  4           5              5
  0	          4	             4 (not increasing)

Example Outcomes - Parent adjectives
Parent 1	    Parent 2	        Stronger Parent     	    Child Adjective
Hulking (2)	    Brutish (3)	        Brutish (3)	                Brutish (3) (70%) or Colossal (4) (30%)
Titanic (5)	    Colossal (4)	    Titanic (5)	                Titanic (5) (70%) or Behemoth (6) (30%)
Brawny (1)	    Monstrous (7)	    Brawny (1)                  (Different category)	Brawny (1)
Infernal (42)	Frozen (43)	        Frozen (43)	                Frozen (43) (70%) or Stormforged (45) (30%)
Brutish (3)	    Brutish (3)	        Brutish (3)	                Titanborn (9) (5% Mythical chance)
Swift (10)	    Lightning-Fast (12)	Lightning-Fast (12)	        Tattered (61) (2% Cursed chance)
*/

using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Custom.KoperPets
{
    public static class KoperBreeding
    {
        private static readonly Random random = new Random();
        private static int breedingScalar = 20;
        private static TimeSpan breedingCooldown = TimeSpan.FromMinutes(breedingScalar);
        private static int GetCategoryStart(int adjective)
        {
            if (adjective >= 0 && adjective <= 9) return 0;     // Strength
            if (adjective >= 10 && adjective <= 19) return 10;  // Dexterity
            if (adjective >= 20 && adjective <= 31) return 20;  // Magic
            if (adjective >= 32 && adjective <= 36) return 32;  // Tanky
            if (adjective >= 37 && adjective <= 41) return 37;  // Attack
            if (adjective >= 42 && adjective <= 51) return 42;  // Elemental
            if (adjective >= 52 && adjective <= 58) return 52;  // Mythical
            if (adjective >= 59 && adjective <= 73) return 59;  // Cursed
            return 0; // Default fallback
        }

        private static int GetCategoryEnd(int adjective)
        {
            if (adjective >= 0 && adjective <= 9) return 9;     // Strength
            if (adjective >= 10 && adjective <= 19) return 19;  // Dexterity
            if (adjective >= 20 && adjective <= 31) return 31;  // Magic
            if (adjective >= 32 && adjective <= 36) return 36;  // Tanky
            if (adjective >= 37 && adjective <= 41) return 41;  // Attack
            if (adjective >= 42 && adjective <= 51) return 51;  // Elemental
            if (adjective >= 52 && adjective <= 58) return 58;  // Mythical
            if (adjective >= 59 && adjective <= 73) return 73;  // Cursed
            return 9; // Default fallback
        }

        private static int GetRandomCursedAdjective()
        {
            int[] cursedAdjectives = { 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73 };
            return cursedAdjectives[random.Next(cursedAdjectives.Length)];
        }

        private static int GetRandomMythicalAdjective()
        {
            int[] mythicalAdjectives = { 52, 53, 54, 55, 56, 57, 58 };
            return mythicalAdjectives[random.Next(mythicalAdjectives.Length)];
        }

        private static int InheritBetterAdjective(int adj1, int adj2)
        {
            // Ensure both adjectives are valid
            if (!KoperPetNaming.AdjectiveModifiers.ContainsKey(adj1) || !KoperPetNaming.AdjectiveModifiers.ContainsKey(adj2))
            {
                Console.WriteLine("[KoperBreeding] ERROR: Invalid parent adjectives.");
                return adj1; // Fallback to one of the parents
            }
            // Get parent category ranges
            int categoryStart1 = GetCategoryStart(adj1);
            int categoryStart2 = GetCategoryStart(adj2);

            int relativeRank1 = adj1 - categoryStart1;  // Rank within category
            int relativeRank2 = adj2 - categoryStart2;

            // Choose the better parent *relative to their category*
            int selectedAdjective = (relativeRank1 >= relativeRank2) ? adj1 : adj2;

            // 2% chance to inherit a cursed trait
            if (random.NextDouble() < 0.02)
            {
                Console.WriteLine("[KoperBreeding] BAD LUCK! The offspring has inherited a cursed trait!");
                return GetRandomCursedAdjective();
            }

            // 30% chance to upgrade within the same category
            int categoryEnd = GetCategoryEnd(selectedAdjective);
            if (random.NextDouble() < 0.3 && selectedAdjective < categoryEnd)
            {
                Console.WriteLine("[KoperBreeding] Offspring inherited an upgraded trait!");
                return selectedAdjective + 1;
            }

            // 5% chance to ascend to Mythical (Only if both parents are at least tier 3 in their category)
            if (relativeRank1 >= 3 && relativeRank2 >= 3 && random.NextDouble() < 0.05)
            {
                Console.WriteLine("[KoperBreeding] The offspring has ascended to a Mythical adjective!");
                return GetRandomMythicalAdjective();
            }

            return selectedAdjective; // Default inheritance
        }

        public static void BreedPets(PlayerMobile breeder, BaseCreature pet1, BaseCreature pet2)
        {
            if (breeder == null || pet1 == null || pet2 == null)
            {
                Console.WriteLine("[KoperBreeding] ERROR: Invalid breeder or pets.");
                return;
            }

            KoperPetData data1 = KoperPetManager.GetPetData(pet1);
            KoperPetData data2 = KoperPetManager.GetPetData(pet2);

            if (data1 == null || data2 == null)
            {
                Console.WriteLine("[KoperBreeding] ERROR: One or both pets are not in the KoperPet system.");
                return;
            }

            if (data1.Gender == data2.Gender)
            {
                Console.WriteLine("[KoperBreeding] ERROR: Both pets are the same gender.");
                return;
            }

            if (pet1.GetType() != pet2.GetType())
            {
                Console.WriteLine("[KoperBreeding] ERROR: Pets must be the same species to breed.");
                return;
            }

            if (!TryBreed(pet1, pet2))
            {
                Console.WriteLine("[KoperBreeding] Failed breeding time check");
                return;
            }

            // **Determine offspring traits**
            int childPedigree = DeterminePedigree(data1.Pedigree, data2.Pedigree);
            int childAdjective = InheritBetterAdjective(data1.Adjective, data2.Adjective);

            // **Spawn offspring**
            BaseCreature offspring = SpawnOffspring(pet1, breeder.Location);

            if (offspring != null)
            {
                offspring.Owners.Add( breeder );
				offspring.SetControlMaster( breeder );
				offspring.Fame = 0;
				offspring.Karma = 0;
				offspring.RangeHome = -1;
				offspring.Home = new Point3D(0, 0, 0);
				offspring.FightMode = FightMode.Aggressor;
				offspring.IsBonded = false;
                offspring.MinTameSkill = 0; // Prevents untaming

                // **Pre-assign inherited values**
                KoperPetData offspringData = new KoperPetData(offspring.Serial)
                {
                    Pedigree = childPedigree,
                    Adjective = childAdjective,
                    MaxLevel = ((childPedigree + 1 )* 5),
                };

                ApplyInheritedStats(offspring, pet1, pet2); // Get inhererited stats before registering
                // **Register offspring with inherited data**
                KoperPetManager.RegisterPet(offspring, offspringData);

                Console.WriteLine("[KoperBreeding] A new pet has been born!");
            }
        }

        private static int DeterminePedigree(int pedigree1, int pedigree2)
        {
            // Ensure pedigree values are valid (0-5)
            pedigree1 = Math.Max(0, Math.Min(5, pedigree1));
            pedigree2 = Math.Max(0, Math.Min(5, pedigree2));

            // Case 1: Two Wild-Born (0,0) → First Generation (1)
            if (pedigree1 == 0 && pedigree2 == 0)
                return 1;

            // Case 2: One Parent is Wild-Born (0, X) → Inherit Higher Pedigree (NO Increase)
            if (pedigree1 == 0 || pedigree2 == 0)
                return Math.Max(pedigree1, pedigree2);

            // Case 3: Two of the Same Pedigree (X, X) → Increase by 1 (Max 5)
            if (pedigree1 == pedigree2)
                return Math.Min(5, pedigree1 + 1);

            // Case 4: Different Pedigrees (≥1) → Child Inherits One Higher than Lower Parent (Max 5)
            return Math.Min(5, Math.Min(pedigree1, pedigree2) + 1);
        }

        private static void ApplyInheritedStats(BaseCreature offspring, BaseCreature parent1, BaseCreature parent2)
        {
            if (offspring == null || parent1 == null || parent2 == null)
                return;

            // **Averaging Parent Stats Before Adjective Bonus**
            offspring.RawStr = (parent1.RawStr + parent2.RawStr) / 2;
            offspring.RawDex = (parent1.RawDex + parent2.RawDex) / 2;
            offspring.RawInt = (parent1.RawInt + parent2.RawInt) / 2;

            offspring.HitsMaxSeed = (parent1.HitsMaxSeed + parent2.HitsMaxSeed) / 2;
            offspring.StamMaxSeed = (parent1.StamMaxSeed + parent2.StamMaxSeed) / 2;
            offspring.ManaMaxSeed = (parent1.ManaMaxSeed + parent2.ManaMaxSeed) / 2;

            // **Averaging Resistances**
            offspring.SetResistance(ResistanceType.Physical, (parent1.PhysicalResistance + parent2.PhysicalResistance) / 2);
            offspring.SetResistance(ResistanceType.Fire, (parent1.FireResistance + parent2.FireResistance) / 2);
            offspring.SetResistance(ResistanceType.Cold, (parent1.ColdResistance + parent2.ColdResistance) / 2);
            offspring.SetResistance(ResistanceType.Poison, (parent1.PoisonResistance + parent2.PoisonResistance) / 2);
            offspring.SetResistance(ResistanceType.Energy, (parent1.EnergyResistance + parent2.EnergyResistance) / 2);

            // **Averaging Min/Max Damage**
            offspring.DamageMin = (parent1.DamageMin + parent2.DamageMin) / 2;
            offspring.DamageMax = (parent1.DamageMax + parent2.DamageMax) / 2;

            Console.WriteLine("[KoperBreeding] Offspring inherited averaged base stats.");
        }
        public static bool CanBreed(BaseCreature pet)
        {
            if (pet == null)
                return false;

            KoperPetData petData = KoperPetManager.GetPetData(pet);

            if (petData == null) // Ensure pet data exists
                return false;

            if (DateTime.UtcNow - petData.LastBreedingTime < breedingCooldown) // Uses updated cooldown
                return false; // Cooldown active

            return true; // Breeding allowed
        }
        public static bool TryBreed(BaseCreature parent1, BaseCreature parent2) // FIXME
        {
            if (!CanBreed(parent1) || !CanBreed(parent2))
            {
                Console.WriteLine("[KoperPetManager] One or both pets are on breeding cooldown.");
                return false;
            }

            if (parent1 == null || parent2 == null)
            {
                Console.WriteLine("[KoperPetManager] Invalid breeding pair.");
                return false;
            }

            RegisterBreedingAttempt(parent1);
            RegisterBreedingAttempt(parent2);

            Console.WriteLine("[KoperPetManager] Breeding time check successful! Cooldown applied to both parents.");
            return true;
        }

        public static void RegisterBreedingAttempt(BaseCreature pet)
        {
            if (pet == null)
                return;

            KoperPetData petData = KoperPetManager.GetPetData(pet);

            petData.LastBreedingTime = DateTime.UtcNow;

            Console.WriteLine("[KoperPetManager] breeding cooldown started for " + pet.Name);
        }

        private static BaseCreature SpawnOffspring(BaseCreature parent, Point3D location)
        {
            try
            {
                BaseCreature offspring = Activator.CreateInstance(parent.GetType()) as BaseCreature;
                if (offspring == null)
                    return null;

                offspring.MoveToWorld(new Point3D(location.X + Utility.RandomMinMax(-1, 1), 
                                          location.Y + Utility.RandomMinMax(-1, 1), 
                                          location.Z), parent.Map);
                return offspring;
                }
            catch (Exception ex)
            {
                Console.WriteLine("[KoperBreeding] ERROR: Failed to spawn offspring: " + ex.Message);
                return null;
            }
        }
    }
}

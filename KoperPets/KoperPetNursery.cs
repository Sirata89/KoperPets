using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Custom.KoperPets;
using Server.Misc;
//using System.Formats.Asn1;

namespace Server.Custom.KoperPets
{
    public static class KoperPetNursery
    {
        private static readonly string SaveFilePath = Path.Combine(Core.BaseDirectory, "Saves/Mobiles/KoperNursery.bin");
        private static Dictionary<Mobile, List<KoperStoredPet>> _nurseryPets = new Dictionary<Mobile, List<KoperStoredPet>>();

        private const int MaxStableSlots = 5; // Custom stable slot limit

        public static void StorePet(PlayerMobile owner, BaseCreature pet)
        {
            if (owner == null || pet == null || !pet.Controlled || pet.ControlMaster != owner)
            {
                owner.SendMessage("That is not a valid pet.");
                return;
            }

            if (!_nurseryPets.ContainsKey(owner))
                _nurseryPets[owner] = new List<KoperStoredPet>();

            if (_nurseryPets[owner].Count >= MaxStableSlots)
            {
                owner.SendMessage("You have reached your stable limit.");
                return;
            }

            // Store pet data before deleting
            KoperPetData petData = KoperPetManager.GetPetData(pet);
            KoperStoredPet storedPet = new KoperStoredPet(pet, petData);
            _nurseryPets[owner].Add(storedPet);

            // Delete the original pet
            pet.Delete();

            owner.SendMessage(string.Format("You have nursery {0}.", pet.Name));
        }

        public static void RetrievePet(PlayerMobile owner, int petIndex)
        {
            if (owner == null || !_nurseryPets.ContainsKey(owner) || _nurseryPets[owner].Count <= petIndex)
            {
                owner.SendMessage("Invalid pet selection.");
                return;
            }

            KoperStoredPet storedPet = _nurseryPets[owner][petIndex];

            if (owner.Followers + storedPet.ControlSlots > owner.FollowersMax)
            {
                owner.SendMessage("You do not have enough control slots to retrieve this pet.");
                return;
            }

            // Ensure the pet type is valid
            if (storedPet.PetType == null)
            {
                owner.SendMessage("Error: Pet type could not be determined.");
                return;
            }

            // Use reflection to recreate the pet
            BaseCreature pet = Activator.CreateInstance(storedPet.PetType) as BaseCreature;
            if (pet == null)
            {
                owner.SendMessage("Error: Failed to recreate pet.");
                return;
            }

            // Restore stats
            pet.Name = storedPet.PetName;
            pet.ControlSlots = storedPet.ControlSlots;
            pet.HitsMaxSeed = storedPet.Hits;
            pet.StamMaxSeed = storedPet.Stam;
            pet.ManaMaxSeed = storedPet.Mana;
            pet.RawStr = storedPet.Str;
            pet.RawDex = storedPet.Dex;
            pet.RawInt = storedPet.Int;
            pet.DamageMin = storedPet.MinDamage;
            pet.DamageMax = storedPet.MaxDamage;
            pet.Fame = storedPet.Fame;
            pet.Karma = storedPet.Karma;
            pet.Loyalty = storedPet.Loyalty;
            pet.IsBonded = storedPet.Bonded;

            // Restore Skills
            for (int i = 0; i < pet.Skills.Length; i++)
            {
                Skill skill = pet.Skills[i];

                if (storedPet.Skills.ContainsKey(skill.SkillName))
                {
                    skill.Base = storedPet.Skills[skill.SkillName].Item1; // Restore Base
                    skill.Cap = storedPet.Skills[skill.SkillName].Item2;  // Restore Cap
                }
            }

            pet.SetResistance(ResistanceType.Physical, storedPet.PhysicalResist);
            pet.SetResistance(ResistanceType.Fire, storedPet.FireResist);
            pet.SetResistance(ResistanceType.Cold, storedPet.ColdResist);
            pet.SetResistance(ResistanceType.Poison, storedPet.PoisonResist);
            pet.SetResistance(ResistanceType.Energy, storedPet.EnergyResist);

            // Restore custom pet data
            KoperPetManager.TransferPetData(storedPet.StoredData, pet);

            // Restore ownership
            pet.ControlMaster = owner;
            pet.Controlled = true;
            pet.MoveToWorld(owner.Location, owner.Map);

            _nurseryPets[owner].RemoveAt(petIndex);
            owner.SendMessage(string.Format("You have retrieved {0}.", pet.Name));
        }


        public static void ListStable(PlayerMobile owner)
        {
            if (!_nurseryPets.ContainsKey(owner) || _nurseryPets[owner].Count == 0)
            {
                owner.SendMessage("You have no pets in storage.");
                return;
            }

            owner.SendMessage("Stored Pets:");
            for (int i = 0; i < _nurseryPets[owner].Count; i++)
            {
                owner.SendMessage(string.Format("{0}. {1}", i + 1, _nurseryPets[owner][i].PetName));
            }
        }

        public static void SaveNurseryData()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(SaveFilePath, FileMode.Create)))
            {
                writer.Write(_nurseryPets.Count);

                foreach (KeyValuePair<Mobile, List<KoperStoredPet>> entry in _nurseryPets)
                {
                    writer.Write(entry.Key.Serial.Value);
                    writer.Write(entry.Value.Count);

                    foreach (KoperStoredPet pet in entry.Value)
                    {
                        pet.Serialize(writer);
                    }
                }
            }
        }

        public static void LoadNurseryData()
        {
            if (!File.Exists(SaveFilePath))
                return;

            using (BinaryReader reader = new BinaryReader(File.Open(SaveFilePath, FileMode.Open)))
            {
                int ownerCount = reader.ReadInt32();
                _nurseryPets.Clear();

                for (int i = 0; i < ownerCount; i++)
                {
                    Serial ownerSerial = (Serial)reader.ReadInt32();
                    Mobile owner = World.FindMobile(ownerSerial);
                    int petCount = reader.ReadInt32();

                    List<KoperStoredPet> petList = new List<KoperStoredPet>();

                    for (int j = 0; j < petCount; j++)
                    {
                        petList.Add(new KoperStoredPet(reader));
                    }

                    if (owner != null)
                        _nurseryPets[owner] = petList;
                }
            }
        }
    }

    public class KoperStoredPet
    {
        public Serial Serial { get; private set; }
        public string PetName { get; private set; }
        public Type PetType { get; private set; } // Store the actual pet type
        public int ControlSlots { get; private set; }
        public int Hits, Stam, Mana;
        public int Str, Dex, Int;
        public int MinDamage, MaxDamage;
        public int Fame, Karma, Loyalty;
        public bool Bonded;
        public int PhysicalResist, FireResist, ColdResist, PoisonResist, EnergyResist;
        public Dictionary<SkillName, Tuple<double, double>> Skills { get; private set; } // Store Skills
        public KoperPetData StoredData { get; private set; }

        public KoperStoredPet(BaseCreature pet, KoperPetData petData)
        {
            Serial = pet.Serial;
            PetName = pet.Name;
            PetType = pet.GetType(); // Store the actual creature type
            ControlSlots = pet.ControlSlots;
            Hits = pet.HitsMax;
            Stam = pet.StamMax;
            Mana = pet.ManaMax;
            Str = pet.RawStr;
            Dex = pet.RawDex;
            Int = pet.RawInt;
            MinDamage = pet.DamageMin;
            MaxDamage = pet.DamageMax;
            PhysicalResist = pet.PhysicalResistance;
            FireResist = pet.FireResistance;
            ColdResist = pet.ColdResistance;
            PoisonResist = pet.PoisonResistance;
            EnergyResist = pet.EnergyResistance;
            Fame = pet.Fame;
            Karma = pet.Karma;
            Loyalty = pet.Loyalty;
            Bonded = pet.IsBonded;
            StoredData = petData;

            // Save Skills
            Skills = new Dictionary<SkillName, Tuple<double, double>>();
            for (int i = 0; i < pet.Skills.Length; i++)
            {
                Skill skill = pet.Skills[i];
                Skills[skill.SkillName] = new Tuple<double, double>(skill.Base, skill.Cap);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Serial.Value);
            writer.Write(PetName);
            writer.Write(PetType.FullName); // Save full type name as string
            writer.Write(ControlSlots);
            writer.Write(Hits);
            writer.Write(Stam);
            writer.Write(Mana);
            writer.Write(Str);
            writer.Write(Dex);
            writer.Write(Int);
            writer.Write(MinDamage);
            writer.Write(MaxDamage);
            writer.Write(PhysicalResist);
            writer.Write(FireResist);
            writer.Write(ColdResist);
            writer.Write(PoisonResist);
            writer.Write(EnergyResist);
            writer.Write(Fame);
            writer.Write(Karma);
            writer.Write(Loyalty);
            writer.Write(Bonded);

            // Save Skills
            writer.Write(Skills.Count);
            foreach (var skill in Skills)
            {
                writer.Write((int)skill.Key);
                writer.Write(skill.Value.Item1); // Base Skill Value
                writer.Write(skill.Value.Item2); // Skill Cap
            }



            writer.Write(StoredData != null);
            if (StoredData != null)
            {
                writer.Write(StoredData.Experience);
                writer.Write(StoredData.Level);
                writer.Write(StoredData.MaxLevel);
                writer.Write(StoredData.Traits);
                writer.Write(StoredData.Gender);
                writer.Write(StoredData.Adjective);
                writer.Write(StoredData.Pedigree);
                writer.Write(StoredData.LastBreedingTime.ToBinary());
            }
        }

        public KoperStoredPet(BinaryReader reader)
        {
            Serial = (Serial)reader.ReadInt32();
            PetName = reader.ReadString();
            string typeName = reader.ReadString();
            PetType = ScriptCompiler.FindTypeByFullName(typeName); // Restore creature type
            ControlSlots = reader.ReadInt32();
            Hits = reader.ReadInt32();
            Stam = reader.ReadInt32();
            Mana = reader.ReadInt32();
            Str = reader.ReadInt32();
            Dex = reader.ReadInt32();
            Int = reader.ReadInt32();
            MinDamage = reader.ReadInt32();
            MaxDamage = reader.ReadInt32();
            PhysicalResist = reader.ReadInt32();
            FireResist = reader.ReadInt32();
            ColdResist = reader.ReadInt32();
            PoisonResist = reader.ReadInt32();
            EnergyResist = reader.ReadInt32();
            Fame = reader.ReadInt32();
            Karma = reader.ReadInt32();
            Loyalty = reader.ReadInt32();
            Bonded = reader.ReadBoolean();


            Skills = new Dictionary<SkillName, Tuple<double, double>>();
            int skillCount = reader.ReadInt32();
            for (int i = 0; i < skillCount; i++)
            {
                SkillName skillName = (SkillName)reader.ReadInt32();
                double baseValue = reader.ReadDouble();
                double capValue = reader.ReadDouble();
                Skills[skillName] = new Tuple<double, double>(baseValue, capValue);
            }


            if (reader.ReadBoolean())
            {
                StoredData = new KoperPetData(Serial)
                {
                    Experience = reader.ReadInt32(),
                    Level = reader.ReadInt32(),
                    MaxLevel = reader.ReadInt32(),
                    Traits = reader.ReadInt32(),
                    Gender = reader.ReadInt32(),
                    Adjective = reader.ReadInt32(),
                    Pedigree = reader.ReadInt32(),
                    LastBreedingTime = DateTime.FromBinary(reader.ReadInt64())
                };
            }
        }
    }

}

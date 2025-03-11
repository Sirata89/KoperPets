using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Custom.KoperPets
{
    public static class KoperPetNursery
    {
        private static readonly string SaveFilePath = Path.Combine(Core.BaseDirectory, "Saves/Mobiles/KoperNursery.bin");
        private static Dictionary<Mobile, List<KoperStoredPet>> _stabledPets = new Dictionary<Mobile, List<KoperStoredPet>>();

        private const int MaxStableSlots = 5; // Custom stable slot limit

        public static void StorePet(PlayerMobile owner, BaseCreature pet)
        {
            if (owner == null || pet == null || !pet.Controlled || pet.ControlMaster != owner)
            {
                owner.SendMessage("That is not a valid pet.");
                return;
            }

            if (!_stabledPets.ContainsKey(owner))
                _stabledPets[owner] = new List<KoperStoredPet>();

            if (_stabledPets[owner].Count >= MaxStableSlots)
            {
                owner.SendMessage("You have reached your stable limit.");
                return;
            }

            // Store pet data before removing control
            KoperPetData petData = KoperPetManager.GetPetData(pet);
            KoperStoredPet storedPet = new KoperStoredPet(pet, petData);
            _stabledPets[owner].Add(storedPet);

            // **Remove pet from world properly**
            pet.ControlMaster = null;        // Unassign owner
            pet.Controlled = false;          // Remove from control list
            pet.Internalize();               // Move to storage

            owner.SendMessage("You have stabled " + pet.Name);
        }


        //  Retrieves a pet from storage, restoring its stats.
        public static void RetrievePet(PlayerMobile owner, int petIndex)
        {
            if (owner == null || !_stabledPets.ContainsKey(owner) || _stabledPets[owner].Count <= petIndex)
            {
                owner.SendMessage("Invalid pet selection.");
                return;
            }

            KoperStoredPet storedPet = _stabledPets[owner][petIndex];

            // **Restore pet using its original serial**
            BaseCreature pet = World.FindMobile(storedPet.Serial) as BaseCreature;
            if (pet == null)
            {
                owner.SendMessage("Error retrieving pet. It may no longer exist.");
                return;
            }

            // **Check Control Slots**
            if (owner.Followers + pet.ControlSlots > owner.FollowersMax)
            {
                owner.SendMessage("You do not have enough control slots to retrieve this pet.");
                return;
            }

            // **Restore pet control**
            pet.ControlMaster = owner;
            pet.Controlled = true;
            pet.MoveToWorld(owner.Location, owner.Map);

            // Restore KoperPetData
            if (KoperPetManager.GetPetData(pet) == null)
            {
                KoperPetManager.RegisterPet(pet, storedPet.StoredData);
            }

            _stabledPets[owner].RemoveAt(petIndex);
            owner.SendMessage("You have retrieved " + pet.Name);
        }

        // Lists stored pets in a player's stable.
        public static void ListStable(PlayerMobile owner)
        {
            if (!_stabledPets.ContainsKey(owner) || _stabledPets[owner].Count == 0)
            {
                owner.SendMessage("You have no pets in storage.");
                return;
            }

            owner.SendMessage("Stored Pets:");
            for (int i = 0; i < _stabledPets[owner].Count; i++)
            {
                owner.SendMessage(string.Format("{0}. {1}", i + 1, _stabledPets[owner][i].PetName));
            }
        }

        // Saves all stored pets to a binary file.
        public static void SaveStableData()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(SaveFilePath, FileMode.Create)))
            {
                writer.Write(_stabledPets.Count);

                foreach (KeyValuePair<Mobile, List<KoperStoredPet>> entry in _stabledPets)
                {
                    writer.Write(entry.Key.Serial.Value);
                    writer.Write(entry.Value.Count);

                    foreach (KoperStoredPet pet in entry.Value)
                    {
                        writer.Write(pet.Serial.Value);
                        writer.Write(pet.PetName);
                        //writer.Write(pet.ControlSlots);
                        //writer.Write(pet.StoredData.SerializeData());
                    }
                }
            }
        }

        // Loads stored pets from a binary file.
        public static void LoadStableData()
        {
            if (!File.Exists(SaveFilePath))
                return;

            using (BinaryReader reader = new BinaryReader(File.Open(SaveFilePath, FileMode.Open)))
            {
                int ownerCount = reader.ReadInt32();
                _stabledPets.Clear();

                for (int i = 0; i < ownerCount; i++)
                {
                    Serial ownerSerial = (Serial)reader.ReadInt32();
                    Mobile owner = World.FindMobile(ownerSerial);
                    int petCount = reader.ReadInt32();

                    List<KoperStoredPet> petList = new List<KoperStoredPet>();

                    for (int j = 0; j < petCount; j++)
                    {
                        Serial petSerial = (Serial)reader.ReadInt32();
                        string petName = reader.ReadString();
                        //int controlSlots = reader.ReadInt32();
                        //KoperPetData petData = KoperPetData.DeserializeData(reader.ReadString());

                        petList.Add(new KoperStoredPet(petSerial, petName)); // , petdata)
                    }

                    if (owner != null)
                        _stabledPets[owner] = petList;
                }
            }
        }

        // Command to stable pets.
        /* private static void StablePet_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if (player != null)
            {
                player.SendMessage("Target the pet you wish to stable.");
                player.Target = new StorePet();
            }
        }

        // Command to retrieve pets.
        private static void RetrievePet_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if (player != null)
            {
                player.SendMessage("Enter the pet index to retrieve:");
                player.Prompt = new RetrievePet();
            }
        }

        // Command to list stored pets.
        private static void ListStable_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if (player != null)
            {
                ListStable(player);
            }
        }*/
    }

    public class KoperStoredPet
    {
        public Serial Serial { get; private set; }
        public string PetName { get; private set; }
        //public int ControlSlots { get; private set; }
        public KoperPetData StoredData { get; private set; }

        public KoperStoredPet(BaseCreature pet, KoperPetData petData)
        {
            Serial = pet.Serial;
            PetName = pet.Name;
            //ControlSlots = pet.ControlSlots;
            //StoredData = petData;
        }

        public KoperStoredPet(Serial serial, string name) //, int slots , KoperPetData data
        {
            Serial = serial;
            PetName = name;
            //ControlSlots = slots;
            //StoredData = data;
        }
    }
}

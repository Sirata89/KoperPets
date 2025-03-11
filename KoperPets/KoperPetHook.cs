using System;
using System.IO;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom.KoperPets
{
    public class KoperPetTamingHook
    {
        public static void Initialize()
        {
            Console.WriteLine("KoperPetHook initialized"); // DEBUG
            EventSink.WorldLoad += OnServerStart;
            EventSink.WorldSave += new WorldSaveEventHandler(OnWorldSave);
            //EventSink.Speech += new SpeechEventHandler(OnPetCommand); // New: Hook into speech for first command
            EventSink.Shutdown += OnServerShutdown;

            OnServerStart();
        }
        private static void OnServerStart()
        {
            Console.WriteLine("On Server Start"); // DEBUG

            if (File.Exists(KoperPetManager.saveFilePath))
            {
                Console.WriteLine("[KoperPetManager] OnServerStartup() Save file found, aborting registering new pets");
                KoperPetManager.LoadAllPets();
                KoperPetNursery.LoadStableData(); // DEBUG
                return;
            }

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && pet.ControlMaster is PlayerMobile)
                    {
                        if (KoperPetManager.GetPetData(pet) == null)
                        {
                            KoperPetManager.RegisterPet(pet);
                        }
                    }
                }
            }
        }

        // Save pets when shutting down
        private static void OnServerShutdown(ShutdownEventArgs e)
        {
            Console.WriteLine("[KoperPetManager] Server shutting down, saving all pets...");
            KoperPetManager.SaveAllPets();
            KoperPetNursery.SaveStableData();
            Console.WriteLine("[KoperPetManager] Finished saving");
        }

        private static void OnWorldSave(WorldSaveEventArgs e) {
            KoperPetManager.SaveAllPets();
            KoperPetNursery.SaveStableData();
        }
        
        // Fallback to ensure pets get registerd if all other methods fail
        /*private static void OnPetCommand(SpeechEventArgs e)
        {
            Mobile m = e.Mobile;
            if (m is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)m;
                foreach (Mobile follower in player.AllFollowers)
                {
                    if (follower is BaseCreature)
                    {
                        BaseCreature pet = (BaseCreature)follower;

                        if (pet.Controlled && pet.ControlMaster == player)
                        {
                            KoperPetData data = KoperPetManager.GetPetData(pet);

                            if (data == null)
                            {
                                KoperPetManager.RegisterPet(pet);
                            }
                        }
                    }
                }
            }
        }*/
    }
}




//TODO: make the gump show a picture of the pet, move the pet name to top of the gump, move breeding info to a second page 
//FIXME: add caps for resistance, at 90 for each
//TODO: Fix dmg other stat scalling with stat increase, 1 str should give +2 hits, +min/max dmg.
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.KoperPets;

namespace Server.Custom.KoperPets
{
    public class KoperPetGump : Gump
    {
        private PlayerMobile m_Player;
        private BaseCreature m_Pet;
        private KoperPetData m_PetData;
        int yOffset = 162; // Start position

        public static int GetCenteredX(string text, int gumpWidth)
        {
            int estimatedTextWidth = text.Length * 6; // Approx. 6px per character
            return (gumpWidth / 2) - (estimatedTextWidth / 2);
        }

        public KoperPetGump(PlayerMobile player, BaseCreature pet) : base(50, 50)
        {

            if (player == null || pet == null || !pet.Controlled || pet.ControlMaster != player)
            {
                player.SendMessage("Invalid pet selection.");
                return;
            }

            m_Pet = pet;
            m_PetData = KoperPetManager.GetPetData(pet);
            string adjectiveText = KoperPetNaming.GetAdjectiveDescription(m_PetData.Adjective);
            List<string> splitText = KoperPetNaming.SplitToLines(adjectiveText, 30);
            int gumpWidth = 550;
            string pedigreeText = KoperPetNaming.GetPedigreeName(m_PetData.Pedigree) + " - " + KoperPetManager.GetPedigree(m_Pet).ToString();
            string petGender = KoperPetManager.GetGender(m_PetData);
            string petLevel = "Level: " + m_PetData.Level.ToString();

            // Define cooldown duration (Example: 12 hours)
            TimeSpan breedingCooldown = TimeSpan.FromHours(12);

            // Calculate remaining cooldown
            TimeSpan timeRemaining = (m_PetData.LastBreedingTime + breedingCooldown) - DateTime.UtcNow;
            string cooldownText;

            // Ensure timeRemaining isn't negative
            if (timeRemaining.TotalSeconds > 0)
            {
                if (timeRemaining.TotalHours >= 1)
                    cooldownText = string.Format("{0} Hours {1} Minutes", (int)timeRemaining.TotalHours, timeRemaining.Minutes);
                else
                    cooldownText = string.Format("{0} Minutes", (int)timeRemaining.TotalMinutes);
            }
            else
            {
                cooldownText = "Ready to breed!";
            }


            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(3, 6, 7034);
            this.AddLabel(GetCenteredX("Lineage Tracking", gumpWidth), 17, 1153, @"Lineage Tracking"); //220
            this.AddLabel(GetCenteredX(m_Pet.Name, gumpWidth), 40, 1153, m_Pet.Name);
            this.AddImage(173, 20, 57);
            this.AddImage(346, 20, 59);
            this.AddLabel(GetCenteredX(petLevel, gumpWidth), 77, 1153, petLevel);
            this.AddLabel(409, 35, 1153, string.Format("Lineage Points: {0}", m_PetData.Traits));
            this.AddLabel(GetCenteredX(pedigreeText, gumpWidth), 98, 1153, pedigreeText);
            this.AddImage(225, 40, 93);
            this.AddLabel(38, 65, 1153, @"Hit Points");
            this.AddLabel(40, 88, 1153, string.Format("{0}/{1}", m_Pet.Hits, m_Pet.HitsMax));
            this.AddLabel(42, 124, 1153, @"Stamina");
            this.AddLabel(40, 144, 1153, string.Format("{0}/{1}", m_Pet.Stam, m_Pet.StamMax));
            this.AddLabel(48, 182, 1153, @"Mana");
            this.AddLabel(37, 204, 1153, string.Format("{0}/{1}", m_Pet.Mana, m_Pet.ManaMax));
            this.AddLabel(GetCenteredX(KoperPetNaming.GetAdjectiveName(m_PetData), gumpWidth), 119, 1153, KoperPetNaming.GetAdjectiveName(m_PetData));
            foreach (string line in splitText)
            {
                this.AddLabel(GetCenteredX(line, gumpWidth), yOffset, 1153, line);
                yOffset += 20; // Move down for next line
            }
            this.AddLabel(25, 255, 1153, @"Strength");
            this.AddLabel(25, 280, 1153, @"Dexterity");
            this.AddLabel(24, 305, 1153, @"Intelligence");
            this.AddLabel(105, 255, 1153, m_Pet.RawStr.ToString());
            this.AddLabel(105, 280, 1153, m_Pet.RawDex.ToString());
            this.AddLabel(105, 305, 1153, m_Pet.RawInt.ToString());
            this.AddButton(249, 356, 27, 27, 1, GumpButtonType.Page, 0);
            this.AddButton(453, 375, 247, 248, 2, GumpButtonType.Reply, 0);
            this.AddLabel(433, 65, 1153, @"Damage");
            this.AddLabel(437, 88, 1153, string.Format("{0}-{1}", m_Pet.DamageMin, m_Pet.DamageMax));
            this.AddLabel(423, 125, 1153, @"Resistances");
            this.AddLabel(252, 331, 1153, @"Breed");
            this.AddLabel(298, 378, 1153, @"Cooldown:");
            this.AddLabel(368, 379, 1153, cooldownText); // TODO add dynamic cooldown calc
            this.AddLabel(415, 152, 1153, string.Format("Physical:  {0}", m_Pet.PhysicalResistance));
            this.AddLabel(415, 176, 1153, string.Format("Fire:     {0}", m_Pet.FireResistance));
            this.AddLabel(415, 199, 1153, string.Format("Cold:     {0}", m_Pet.ColdResistance));
            this.AddLabel(415, 221, 1153, string.Format("Poison:   {0}", m_Pet.PoisonResistance));
            this.AddLabel(415, 245, 1153, string.Format("Energy:   {0}", m_Pet.EnergyResistance));
            this.AddLabel(25, 35, 1153, @"Exp:");
            this.AddLabel(60, 35, 1153, string.Format("{0}/{1}", m_PetData.Experience, KoperPetManager.GetXPNeeded(m_PetData)));
            this.AddLabel(GetCenteredX(petGender, gumpWidth), 138, 1153, petGender);
            this.AddLabel(26, 338, 1153, string.Format("MaxLevel: {0}", m_PetData.MaxLevel));

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {

        }
    }
}
//TODO: make the gump show a picture of the pet, move the pet name to top of the gump, move breeding info to a second page 
//FIXME: add caps for resistance, at 90 for each
//TODO: Fix dmg other stat scalling with stat increase, 1 str should give +2 hits, +min/max dmg.

/*
using System;
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

        public KoperPetGump(PlayerMobile player, BaseCreature pet) : base(50, 50)
        {
            if (player == null || pet == null || !pet.Controlled || pet.ControlMaster != player || (m_PetData = KoperPetManager.GetPetData(pet)) == null)
                return;

            m_Player = player;
            m_Pet = pet;

            int traitPoints = m_PetData.Traits;
            bool canSpendPoints = (traitPoints > 0);

            AddPage(0);
            AddBackground(0, 0, 450, 400, 9250); // Slightly taller for new breeding info

            // **General Info (Left Side)**
            AddLabel(180, 10, 1152, "Pet Status");
            AddLabel(20, 40, 1153, "Name:");
            AddLabel(100, 40, 1153, pet.Name ?? "Unnamed");

            AddLabel(20, 60, 1153, "Gender:");
            AddLabel(100, 60, 1153, KoperPetManager.GetGender(pet));

            AddLabel(20, 80, 1153, "Level:");
            AddLabel(100, 80, 1153, m_PetData.Level.ToString());

            AddLabel(20, 100, 1153, "XP:");
            AddLabel(100, 100, 1153, string.Format("{0}/{1}", m_PetData.Experience, m_PetData.Level * 100));

            AddLabel(20, 120, 1153, "Max Level:");
            AddLabel(120, 120, 1153, m_PetData.MaxLevel.ToString());

            AddLabel(20, 140, 1153, "Trait Points:");
            AddLabel(120, 140, 1153, traitPoints.ToString());

            AddLabel(20, 160, 1153, "Pedigree:");
            AddLabel(120, 160, 1153, KoperPetManager.GetPedigreeName(m_PetData.Pedigree));

            AddLabel(20, 180, 1153, "Adjective:");
            AddLabel(120, 180, 1153, KoperPetManager.AdjectiveModifiers[m_PetData.Adjective].Key);

            // **Attributes (Right Side)**
            AddLabel(230, 40, 1152, "Attributes");
            AddLabel(230, 60, 1153, "Strength:");
            AddLabel(330, 60, 1153, pet.Str.ToString());
            if (canSpendPoints) AddButton(370, 60, 5601, 5605, 101, GumpButtonType.Reply, 0);

            AddLabel(230, 80, 1153, "Dexterity:");
            AddLabel(330, 80, 1153, pet.Dex.ToString());
            if (canSpendPoints) AddButton(370, 80, 5601, 5605, 102, GumpButtonType.Reply, 0);

            AddLabel(230, 100, 1153, "Intelligence:");
            AddLabel(330, 100, 1153, pet.Int.ToString());
            if (canSpendPoints) AddButton(370, 100, 5601, 5605, 103, GumpButtonType.Reply, 0);

            // **Resistances**
            AddLabel(230, 130, 1152, "Resistances");
            AddLabel(230, 150, 1153, "Physical:");
            AddLabel(330, 150, 1153, pet.PhysicalResistance.ToString());
            if (canSpendPoints) AddButton(370, 150, 5601, 5605, 104, GumpButtonType.Reply, 0);

            AddLabel(230, 170, 1153, "Fire:");
            AddLabel(330, 170, 1153, pet.FireResistance.ToString());
            if (canSpendPoints) AddButton(370, 170, 5601, 5605, 105, GumpButtonType.Reply, 0);

            AddLabel(230, 190, 1153, "Cold:");
            AddLabel(330, 190, 1153, pet.ColdResistance.ToString());
            if (canSpendPoints) AddButton(370, 190, 5601, 5605, 106, GumpButtonType.Reply, 0);

            AddLabel(230, 210, 1153, "Poison:");
            AddLabel(330, 210, 1153, pet.PoisonResistance.ToString());
            if (canSpendPoints) AddButton(370, 210, 5601, 5605, 107, GumpButtonType.Reply, 0);

            AddLabel(230, 230, 1153, "Energy:");
            AddLabel(330, 230, 1153, pet.EnergyResistance.ToString());
            if (canSpendPoints) AddButton(370, 230, 5601, 5605, 108, GumpButtonType.Reply, 0);

            // **Combat Stats**
            AddLabel(20, 220, 1152, "Combat Stats");
            AddLabel(20, 240, 1153, "Hit Points:");
            AddLabel(120, 240, 1153, string.Format("{0}/{1}", pet.Hits, pet.HitsMax));

            AddLabel(20, 260, 1153, "Stamina:");
            AddLabel(120, 260, 1153, string.Format("{0}/{1}", pet.Stam, pet.StamMax));

            AddLabel(20, 280, 1153, "Mana:");
            AddLabel(120, 280, 1153, string.Format("{0}/{1}", pet.Mana, pet.ManaMax));

            AddLabel(20, 300, 1153, "Damage:");
            AddLabel(120, 300, 1153, string.Format("{0}-{1}", pet.DamageMin, pet.DamageMax));

            // **Breeding Info**
            AddLabel(230, 270, 1152, "Breeding Info");
            AddLabel(230, 290, 1153, "Cooldown:"); 
            AddLabel(320, 290, 1153, "FIXME"); // TODO: Implement cooldown system

            // **Close Button**
            AddButton(50, 350, 247, 248, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1) // Close button
            {
                sender.Mobile.CloseGump(typeof(KoperPetGump));
            }
            else if (info.ButtonID >= 101 && info.ButtonID <= 108) // Stat increase buttons
            {
                if (m_PetData.Traits > 0)
                {
                    switch (info.ButtonID)
                    {
                        case 101: m_Pet.RawStr += 1; break;
                        case 102: m_Pet.RawDex += 1; break;
                        case 103: m_Pet.RawInt += 1; break;
                        case 104: m_Pet.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, 5)); break;
                        case 105: m_Pet.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, 5)); break;
                        case 106: m_Pet.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, 5)); break;
                        case 107: m_Pet.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, 5)); break;
                        case 108: m_Pet.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, 5)); break;
                    }

                    m_PetData.Traits--;
                    sender.Mobile.SendGump(new KoperPetGump(m_Player, m_Pet));
                }
                else
                {
                    sender.Mobile.SendMessage("You have no Trait Points left.");
                }
            }
        }
    }
}
*/
using System;
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

        public KoperPetGump(PlayerMobile player, BaseCreature pet) : base(50, 50)
        {
            this.Closable=true;
            this.Disposable=true;
            this.Dragable=true;
            this.Resizable=false;
            this.AddPage(0);
            this.AddBackground(73, 40, 368, 366, 9270);
            this.AddImage(178, 125, 1275);
            this.AddLabel(208, 45, 0, @"Lineage Tracking");
            this.AddLabel(199, 74, 0, @"A Celestial Ferrett");
            this.AddImage(176, 48, 57);
            this.AddImage(316, 48, 59);
            this.AddLabel(82, 116, 0, @"Level: 1");
            this.AddLabel(82, 141, 0, @"Lineage: 0");
            this.AddLabel(82, 167, 0, @"Wild-Born (0)");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {

        }
    }
}
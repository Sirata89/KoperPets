using Server;
using Server.Commands;
using Server.Custom.KoperPets;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Custom.KoperPets
{
public class KoperPetCommands
{
    public static void Initialize()
    {
        CommandSystem.Register("GainXP", AccessLevel.Player, new CommandEventHandler(GainXP_OnCommand));
        CommandSystem.Register("CheckPet", AccessLevel.Player, new CommandEventHandler(CheckPet_OnCommand));
        CommandSystem.Register("PetGump", AccessLevel.Player, new CommandEventHandler(PetGump_OnCommand));
        CommandSystem.Register("BreedPet", AccessLevel.Player, new CommandEventHandler(BreedPet_OnCommand));
    }

    [Usage("GainXP <amount>")]
    [Description("Click on your pet to give it experience.")]
    public static void GainXP_OnCommand(CommandEventArgs e)
    {
        PlayerMobile player = e.Mobile as PlayerMobile;
        if (player != null)
        {
            int xp = e.GetInt32(0);
            player.SendMessage("Target your pet to grant experience.");
            player.Target = new GainXPTarget(xp);
        }
    }

    [Usage("CheckPet")]
    [Description("Click on your pet to check its stats.")]
    public static void CheckPet_OnCommand(CommandEventArgs e)
    {
        PlayerMobile player = e.Mobile as PlayerMobile;
        if (player != null)
        {
            player.SendMessage("Target your pet to check its stats.");
            player.Target = new CheckPetTarget();
        }
    }

    /// <summary>
    /// Target handler for gaining experience.
    /// </summary>
    private class GainXPTarget : Target
    {
        private int xpAmount;

        public GainXPTarget(int xp) : base(10, false, TargetFlags.None)
        {
            xpAmount = xp;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            PlayerMobile player = from as PlayerMobile;
            BaseCreature pet = targeted as BaseCreature;

            if (player == null || pet == null || !pet.Controlled || pet.ControlMaster != player)
            {
                player.SendMessage("That is not a valid pet.");
                return;
            }

            KoperPetManager.GainExperience(pet, xpAmount);
            player.SendMessage(string.Format("{0} gained {1} experience!", pet.Name ?? "Your pet", xpAmount));
        }
    }

    [Usage("PetGump")]
    [Description("Click on your pet to view its status.")]
    public static void PetGump_OnCommand(CommandEventArgs e)
    {
        PlayerMobile player = e.Mobile as PlayerMobile;
        if (player != null)
        {
            player.SendMessage("Target your pet to view its stats.");
            player.Target = new PetGumpTarget();
        }
    }

private class PetGumpTarget : Target
{
    public PetGumpTarget() : base(10, false, TargetFlags.None) { }

    protected override void OnTarget(Mobile from, object targeted)
    {
        PlayerMobile player = from as PlayerMobile;
        BaseCreature pet = targeted as BaseCreature;

        if (player == null || pet == null || !pet.Controlled || pet.ControlMaster != player)
        {
            player.SendMessage("That is not a valid pet.");
            return;
        }

        player.SendGump(new KoperPetGump(player, pet));
    }
}


        /// <summary>
        /// Target handler for checking pet stats.
        /// </summary>
    private class CheckPetTarget : Target
    {
        public CheckPetTarget() : base(10, false, TargetFlags.None) // 10 tile range
        {
        }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile player = from as PlayerMobile;
                BaseCreature pet = targeted as BaseCreature;

                if (player == null || pet == null || !pet.Controlled || pet.ControlMaster != player)
                {
                    player.SendMessage("That is not a valid pet.");
                    return;
                }

                KoperPetManager.RegisterPet(pet); // Ensure pet data exists

                player.SendMessage(string.Format("{0} - {6} - Level: {1}, XP: {2}/{3}, Max Level: {4}, Traits points: {5}",
                pet.Name ?? "Your pet",
                KoperPetManager.GetLevel(pet),
                KoperPetManager.GetExperience(pet),
                KoperPetManager.GetLevel(pet) * 100,
                KoperPetManager.GetMaxLevel(pet),
                KoperPetManager.GetTraits(pet),
                KoperPetManager.GetGender(pet)));
            }
        }

    
        [Usage("BreedPet")]
        [Description("Select two pets to breed.")]
        public static void BreedPet_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            if (player != null)
            {
                player.SendMessage("Target the first pet you want to breed.");
                player.Target = new BreedPetTarget(null);
            }
        }

private class BreedPetTarget : Target
{
    private BaseCreature firstPet;

    public BreedPetTarget(BaseCreature pet) : base(10, false, TargetFlags.None)
    {
        firstPet = pet;
    }

    protected override void OnTarget(Mobile from, object targeted)
    {
        PlayerMobile player = from as PlayerMobile;
        BaseCreature pet = targeted as BaseCreature;

        if (player == null || pet == null || !pet.Controlled || pet.ControlMaster != player)
        {
            player.SendMessage("That is not a valid pet.");
            return;
        }

        // First pet selection
        if (firstPet == null)
        {
            player.SendMessage("Now target the second pet to breed.");
            player.Target = new BreedPetTarget(pet); // Re-assign targeting
        }
        else
        {
            // Second pet selected, attempt breeding
            if (pet == firstPet)
            {
                player.SendMessage("You must select two different pets.");
                return;
            }

            KoperBreeding.BreedPets(player, firstPet, pet);
        }
    }
}

    }
}
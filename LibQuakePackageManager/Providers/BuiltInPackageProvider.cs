using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Providers
{
    public class BuiltInPackageProvider : IProvider<Package>
    {
        #region Properties
        public List<Package> Items { get; }
        #endregion

        #region Constructors
        public BuiltInPackageProvider()
        {
            Items = new List<Package>()
            {
                new Package
                (
                    "id1",
                    new Dictionary<string, string>()
                    {
                        { "Title", "Quake" },
                        { "Author", "id Software" },
                        { "Description", "Rage through 32 single player levels and 6 deathmatch levels of sheer terror and fully immersive sound and lighting. Arm yourself against the cannibalistic Ogre, fiendish Vore and indestructible Shambler using lethal nails, fierce Thunderbolts and abominable Rocket and Grenade Launchers." },
                        { "Release Date", "22.06.96" }
                    }
                ),
                new Package
                (
                    "hipnotic",
                    new Dictionary<string, string>()
                    {
                        { "Title", "Quake Mission Pack No. 1: Scourge of Armagon" },
                        { "Author", "Hipnotic Interactive" },
                        { "Description", "After defeating Shub-Niggurath, you arrive back at your home base on Earth, but it's not all blue skies and butterflies. Apparently, not all QUAKE forces have been subdued. Chaos ensues as you blast your way through the QUAKE minions and toward the infested gateway. Your only choice is to find the source of evil and shut it down. Without any hesitation, and with more guts than common sense, you leap into a portal of unknown destination." },
                        { "Release Date", "05.03.97" }
                    }
                ),
                new Package
                (
                    "rogue",
                    new Dictionary<string, string>()
                    {
                        { "Title", "Quake Mission Pack No. 2: Dissolution of Eternity" },
                        { "Author", "Rogue Entertainment" },
                        { "Description", "TWO NEW EPISODES. SIXTEEN NEW LEVELS. ONE WAY OUT.\nYour journey has led you down a path of no return. The acrid smell of death fills the air.And you know the road ahead may lead to your grave. But Quake, with his insidious, apocalyptic plans, must be crushed. If you fail, evil will shroud the universe for all eternity." },
                        { "Release Date", "19.03.97" }
                    }
                )
            };
        }
        #endregion

        #region Methods
        public Package GetItem(string id)
        {
            try
            {
                return Items.First(x => x.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task RefreshAsync()
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}

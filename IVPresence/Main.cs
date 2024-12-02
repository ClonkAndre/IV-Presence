using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Linq;

using DiscordRPC;
using Newtonsoft.Json;
using CCL.GTAIV;
using IVPresence.Classes;
using IVPresence.Classes.Json;

using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;

namespace IVPresence
{
    public class Main : Script
    {

        #region Variables
        public bool MenuOpened;

        // Player
        private uint playerId;
        private int playerIndex;
        private int playerHandle;
        private Vector3 playerPos;
        private uint playerHealth;
        private uint playerArmour;
        private int playerInterior;
        private bool playerDead;
        private bool playerIsInAir;
        private bool playerSwimming;
        private bool playerInAnyCar;
        private bool playerPerformingWheelie;
        private bool playerPerformingStoppie;
        private uint currentScore;
        private uint currentWantedLevel;
        private string currentRadioStationName;
        private string currentZoneRawName;
        private string currentZoneDisplayName;
        private int currentPlayerWeapon;

        private CauseOfDeath causeOfDeath;

        // Pools
        private IVPool interiorPool;

        // Discord
        private DiscordRpcClient discordRpcClient;
        private DiscordLogger discordLogger;
        private RichPresence discordRichPresence;
        private bool isDiscordReady;

        private RichPresence customRichPresence;
        private Guid idOfScriptWhichSetCustomPresence;

        // Timer
        private Stopwatch wheelieStoppieWatch;
        private Guid wantedBlinkTimerId;
        private Guid textSwitchingTimerId;

        // Lists
        private static Dictionary<string, string> zoneToIslandDict = new Dictionary<string, string>()
        {
            // Alderney
            { "WESDY", "Alderney" },
            { "LEFWO", "Alderney" },
            { "ALDCI", "Alderney" },
            { "BERCH", "Alderney" },
            { "NORMY", "Alderney" },
            { "ACTRR", "Alderney" },
            { "PORTU", "Alderney" },
            { "TUDOR", "Alderney" },
            { "ACTIP", "Alderney" },
            { "ALSCF", "Alderney" },

            // Algonquin
            { "NORWO", "Algonquin" },
            { "EAHOL", "Algonquin" },
            { "NOHOL", "Algonquin" },
            { "VASIH", "Algonquin" },
            { "LANCA", "Algonquin" },
            { "MIDPE", "Algonquin" },
            { "MIDPA", "Algonquin" },
            { "MIDPW", "Algonquin" },
            { "PUGAT", "Algonquin" },
            { "HATGA", "Algonquin" },
            { "LANCE", "Algonquin" },
            { "STARJ", "Algonquin" },
            { "WESMI", "Algonquin" },
            { "TMEQU", "Algonquin" },
            { "THTRI", "Algonquin" },
            { "EASON", "Algonquin" },
            { "THPRES", "Algonquin" },
            { "FISSN", "Algonquin" },
            { "FISSO", "Algonquin" },
            { "LOWEA", "Algonquin" },
            { "LITAL", "Algonquin" },
            { "SUFFO", "Algonquin" },
            { "CASGC", "Algonquin" },
            { "CITH" , "Algonquin" },
            { "CHITO", "Algonquin" },
            { "THXCH", "Algonquin" },
            { "CASGR", "Algonquin" },

            // Bohan
            { "BOULE", "Bohan" },
            { "NRTGA", "Bohan" },
            { "LTBAY", "Bohan" },
            { "FORSI", "Bohan" },
            { "INSTI", "Bohan" },
            { "STHBO", "Bohan" },
            { "CHAPO", "Bohan" },

            // Dukes
            { "STEIN", "Dukes" },
            { "MEADP", "Dukes" },
            { "FRANI", "Dukes" },
            { "WILLI", "Dukes" },
            { "MEADH", "Dukes" },
            { "EISLC", "Dukes" },
            { "BOAB" , "Dukes" },
            { "CERHE", "Dukes" },
            { "BEECW", "Dukes" },

            // Broker
            { "SCHOL", "Broker" },
            { "DOWTW", "Broker" },
            { "ROTTH", "Broker" },
            { "ESHOO", "Broker" },
            { "OUTL", "Broker" },
            { "SUTHS", "Broker" },
            { "HOBEH", "Broker" },
            { "FIREP", "Broker" },
            { "FIISL", "Broker" },
            { "BEGGA", "Broker" },

            // Happiness Island
            { "HAPIN", "Happiness Island" },

            // Charge Island
            { "CHISL", "Charge Island" },

            // Colony Island
            { "COISL", "Colony Island" },

            // Bridges, tunnels etc TODO
            { "BRALG", "Liberty City" },
            { "BRBRO", "Liberty City" },
            { "BREBB", "Liberty City" },
            { "BRDBB", "Liberty City" },
            { "NOWOB", "Liberty City" },
            { "HIBRG", "Liberty City" },
            { "LEAPE", "Liberty City" },
            { "BOTU", "Liberty City" },

            // Liberty City
            { "LIBERTY", "Liberty City" }
        };
        private static Dictionary<string, string> zoneToIslandDictDCReady = new Dictionary<string, string>()
        {
            // Alderney
            {"WESDY", "alderney"},
            {"LEFWO", "alderney"},
            {"ALDCI", "alderney"},
            {"BERCH", "alderney"},
            {"NORMY", "alderney"},
            {"ACTRR", "alderney"},
            {"PORTU", "alderney"},
            {"TUDOR", "alderney"},
            {"ACTIP", "alderney"},
            {"ALSCF", "alderney"},

            // Algonquin
            {"CASGR", "algonquin"},
            {"THXCH", "algonquin"},
            {"FISSO", "algonquin"},
            {"CHITO", "algonquin"},
            {"CITH", "algonquin"},
            {"CASGC", "algonquin"},
            {"SUFFO", "algonquin"},
            {"LITAL", "algonquin"},
            {"LOWEA", "algonquin"},
            {"FISSN", "algonquin"},
            {"THPRES", "algonquin"},
            {"EASON", "algonquin"},
            {"THTRI", "algonquin"},
            {"TMEQU", "algonquin"},
            {"WESMI", "algonquin"},
            {"STARJ", "algonquin"},
            {"LANCE", "algonquin"},
            {"HATGA", "algonquin"},
            {"PUGAT", "algonquin"},
            {"MIDPW", "algonquin"},
            {"MIDPA", "algonquin"},
            {"MIDPE", "algonquin"},
            {"LANCA", "algonquin"},
            {"VASIH", "algonquin"},
            {"NOHOL", "algonquin"},
            {"EAHOL", "algonquin"},
            {"NORWO", "algonquin"},

            // Bohan
            {"STHBO", "bohan"},
            {"CHAPO", "bohan"},
            {"FORSI", "bohan"},
            {"BOULE", "bohan"},
            {"NRTGA", "bohan"},
            {"INSTI", "bohan"},
            {"LTBAY", "bohan"},

            // Broker/Dukes
            {"STEIN", "broker_dukes"},
            {"MEADP", "broker_dukes"},
            {"FRANI", "broker_dukes"},
            {"WILLI", "broker_dukes"},
            {"MEADH", "broker_dukes"},
            {"EISLC", "broker_dukes"},
            {"BOAB", "broker_dukes"},
            {"CERHE", "broker_dukes"},
            {"BEECW", "broker_dukes"},
            {"SCHOL", "broker_dukes"},
            {"DOWTW", "broker_dukes"},
            {"ROTTH", "broker_dukes"},
            {"ESHOO", "broker_dukes"},
            {"OUTL", "broker_dukes"},
            {"SUTHS", "broker_dukes"},
            {"HOBEH", "broker_dukes"},
            {"FIREP", "broker_dukes"},
            {"FIISL", "broker_dukes"},
            {"BEGGA", "broker_dukes"},

            // Charge Island
            {"CHISL", "chisl"},

            // Colony Island
            {"COISL", "coisl"},

            // Happiness Island
            {"HAPIN", "hapin"},

            // Bridges, tunnels etc TODO
            {"BRALG", "bralg"},
            {"BRBRO", "brbro"},
            {"BREBB", "brebb"},
            {"BRDBB", "brdbb"},
            {"NOWOB", "nowob"},
            {"HIBRG", "hibrg"},
            {"LEAPE", "leape"},
            {"BOTU", "botu"},

            // Liberty City
            {"LIBERTY", "liberty"}
        };
        private List<RadioInfo> radioStationInfos;
        private List<EpisodeInfo> episodeInfos;
        private List<PredefinedLocations> predefinedLocations;
        private List<CustomInterior> customInteriors;

        // States
        private int wantedBlinkState;
        private object wantedBlinkStateLockObj = new object();
        private TextState currentTextState;
        private object textSwitchStateLockObj = new object();

        private bool isAltTabbed;
        private bool checkedForCauseOfDeath;

        // Eggs
        private int blackoutTextToShow;
        private int honkTextToShow;
        private int deathTextToShow;

        // DEBUG
#if DEBUG
        public bool DebuggingWindowOpen = true;
#endif

        private string currentPredefinedLocation;

        #endregion

        #region Constructor
        public Main()
        {
            // Lists
            radioStationInfos = new List<RadioInfo>();
            episodeInfos = new List<EpisodeInfo>();
            predefinedLocations = new List<PredefinedLocations>();
            customInteriors = new List<CustomInterior>();
            //predefinedLocations = new List<PredefinedLocations>()
            //{
            //    new PredefinedLocations("SwingSet", new Vector3(1348.225f, -250.883f, 24.17826f), 10.0f),
            //    new PredefinedLocations("OnTopOfRotterdamHillTower", new Vector3(-313.4718f, -77.74622f, 459.6857f), new Vector3(-254.4818f, -123.194f, 310.5f)),
            //    new PredefinedLocations("InsideStatueOfLiberty", new Vector3(-605.0084f, -752.2288f, 73.98586f), new Vector3(-612.1718f, -751.1531f, 46.31163f)),
            //    new PredefinedLocations("AidenDeathLocation", new Vector3(-1546.964f, 1237.855f, 11.96186f), 10.0f),
            //    new PredefinedLocations("VladDeathLocation", new Vector3(779.5123f, 235.4688f, 4.506213f), 10.0f)
            //};

            // Other
            wheelieStoppieWatch = new Stopwatch();

            // IV-SDK .NET stuff
            RAGE.OnWindowFocusChanged += RAGE_OnWindowFocusChanged;
            Uninitialize += Main_Uninitialize;
            Initialized += Main_Initialized;
            ScriptCommandReceived += Main_ScriptCommandReceived;
            Drawing += Main_Drawing;
            OnImGuiRendering += Main_OnImGuiRendering;
            Tick += Main_Tick;
        }
        #endregion

        #region Methods
        private void LoadEpisodeInfo()
        {
            try
            {
                string path = string.Format("{0}\\EpisodeInfo.json", ScriptResourceFolder);

                if (!File.Exists(path))
                {
                    Logging.LogError("Failed to load and add episode info. Details: The 'EpisodeInfo.json' file was not found.");
                    return;
                }

                episodeInfos.Clear();
                episodeInfos = JsonConvert.DeserializeObject<List<EpisodeInfo>>(File.ReadAllText(path));

                Logging.Log("Loaded {0} episode info(s).", episodeInfos.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to load and add episode info. Details: {0}", ex);
            }
        }
        private void LoadPredefinedLocations()
        {
            try
            {
                string path = string.Format("{0}\\PredefinedLocations.json", ScriptResourceFolder);

                if (!File.Exists(path))
                {
                    Logging.LogError("Failed to load and add predefined locations. Details: The 'PredefinedLocations.json' file was not found.");
                    return;
                }

                predefinedLocations.Clear();
                predefinedLocations = JsonConvert.DeserializeObject<List<PredefinedLocations>>(File.ReadAllText(path));

                Logging.Log("Loaded {0} predefined locations(s).", episodeInfos.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to load and add predefined locations. Details: {0}", ex);
            }
        }
        private void LoadRadioStationInfo()
        {
            try
            {
                string path = string.Format("{0}\\RadioStationInfo.json", ScriptResourceFolder);

                if (!File.Exists(path))
                {
                    Logging.LogError("Failed to load and add radio stations. Details: The 'RadioStationInfo.json' file was not found.");
                    return;
                }

                radioStationInfos.Clear();
                radioStationInfos = JsonConvert.DeserializeObject<List<RadioInfo>>(File.ReadAllText(path));

                Logging.Log("Loaded {0} radio station(s).", episodeInfos.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to load and add radio stations. Details: {0}", ex);
            }
        }
        private void LoadCustomInteriors()
        {
            try
            {
                string path = string.Format("{0}\\CustomInteriors.json", ScriptResourceFolder);

                if (!File.Exists(path))
                {
                    Logging.LogError("Failed to load and add custom interiors. Details: The 'CustomInteriors.json' file was not found.");
                    return;
                }

                customInteriors.Clear();
                customInteriors = JsonConvert.DeserializeObject<List<CustomInterior>>(File.ReadAllText(path));

                Logging.Log("Loaded {0} custom interior(s).", episodeInfos.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to load and add custom interiors. Details: {0}", ex);
            }
        }
        private void SaveEpisodeInfo()
        {
            try
            {
                string path = string.Format("{0}\\EpisodeInfo.json", ScriptResourceFolder);

                File.WriteAllText(path, JsonConvert.SerializeObject(episodeInfos));

                Logging.Log("Loaded {0} episode info(s).", episodeInfos.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to save episode infos. Details: {0}", ex);
            }
        }
        private void SavePredefinedLocations()
        {
            try
            {
                string path = string.Format("{0}\\PredefinedLocations.json", ScriptResourceFolder);

                File.WriteAllText(path, JsonConvert.SerializeObject(predefinedLocations));

                Logging.Log("Loaded {0} predefined location(s).", predefinedLocations.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to save predefined locations. Details: {0}", ex);
            }
        }
        private void SaveRadioStationInfo()
        {
            try
            {
                string path = string.Format("{0}\\RadioStationInfo.json", ScriptResourceFolder);

                File.WriteAllText(path, JsonConvert.SerializeObject(radioStationInfos));

                Logging.Log("Loaded {0} radio station(s).", radioStationInfos.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to save radio stations. Details: {0}", ex);
            }
        }
        private void SaveCustomInteriors()
        {
            try
            {
                string path = string.Format("{0}\\CustomInteriors.json", ScriptResourceFolder);

                File.WriteAllText(path, JsonConvert.SerializeObject(customInteriors));

                Logging.Log("Loaded {0} custom interior(s).", customInteriors.Count);
            }
            catch (Exception ex)
            {
                Logging.LogError("Failed to save custom interiors. Details: {0}", ex);
            }
        }

        private void Prepare()
        {
            isDiscordReady = true;
            discordRpcClient.UpdateStartTime();

            // Start blink timer
            wantedBlinkTimerId = StartNewTimer(3000, () =>
            {
                lock (wantedBlinkStateLockObj)
                {
                    wantedBlinkState++;

                    if (wantedBlinkState > 1)
                        wantedBlinkState = 0;
                }
            });
            textSwitchingTimerId = StartNewTimer(5000, () =>
            {
                lock (textSwitchStateLockObj)
                {
                    currentTextState = (TextState)((int)currentTextState + 1);

                    if (currentTextState == TextState.MAX)
                        currentTextState = TextState.Default;
                }
            });
        }

        private void CheckCauseOfDeath()
        {
            if (checkedForCauseOfDeath)
                return;

            // Check if player died while fleeing from the cops
            if (currentWantedLevel != 0 && playerInAnyCar)
            {
                causeOfDeath = CauseOfDeath.FleeingFromCops;
                checkedForCauseOfDeath = true;
                return;
            }

            // Check if player died while fighting the cops
            if (currentWantedLevel != 0 && currentPlayerWeapon != 0)
            {
                causeOfDeath = CauseOfDeath.FightingCops;
                checkedForCauseOfDeath = true;
                return;
            }

            // Check if player died through a HEADSHOT
            if (WAS_PED_KILLED_BY_HEADSHOT(playerHandle))
            {
                causeOfDeath = CauseOfDeath.Headshot;
                checkedForCauseOfDeath = true;
                return;
            }

            // Check if player died through an fire
            if (IS_CHAR_ON_FIRE(playerHandle))
            {
                causeOfDeath = CauseOfDeath.Fire;
                checkedForCauseOfDeath = true;
                return;
            }

            // Check if player died through an explosion
            // Loop through explosion types and see if there is an explosion with this type in sphere
            for (int i = 0; i < 24; i++)
            {
                // Ignore some explosion types
                switch (i)
                {
                    case 1:
                    case 3:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        continue;
                }

                if (IS_EXPLOSION_IN_SPHERE(i, playerPos, 10f))
                {
                    // Get explosion which killed player
                    switch ((eExplosion)i)
                    {
                        case eExplosion.EXPLOSION_CAR:
                            causeOfDeath = CauseOfDeath.ExplodingCar;
                            break;
                        case eExplosion.EXPLOSION_BIKE:
                            causeOfDeath = CauseOfDeath.ExplodingBike;
                            break;
                        case eExplosion.EXPLOSION_TRUCK:
                            causeOfDeath = CauseOfDeath.ExplodingTruck;
                            break;
                        case eExplosion.EXPLOSION_PETROL_PUMP:
                            causeOfDeath = CauseOfDeath.ExplodingPetrolPump;
                            break;
                        default:

                            if (IVNetwork.IsNetworkGameRunning() && GENERATE_RANDOM_INT_IN_RANGE(0, 100) < 15)
                                causeOfDeath = CauseOfDeath.ExplosionNoobtubed;
                            else
                                causeOfDeath = CauseOfDeath.Explosion;

                            break;
                    }

                    
                    checkedForCauseOfDeath = true;
                    return;
                }
            }

            // Check if player died through falling damage
            if (playerIsInAir)
            {
                causeOfDeath = CauseOfDeath.FallingDamage;
                checkedForCauseOfDeath = true;
                return;
            }

            // No cause found
            causeOfDeath = CauseOfDeath.Unknown;
            checkedForCauseOfDeath = true;
        }

        private void SetLargeImageStuff()
        {
            // Get name of current zone player is in
            string zoneName = "UNKNOWN";
            if (!zoneToIslandDict.TryGetValue(GET_NAME_OF_ZONE(playerPos), out zoneName))
            {
                // TODO: Maybe log or something
            }

            // Interiors: They have the highest priority
            if (IS_INTERIOR_SCENE())
            {
                short interiorModelIndex = GetCurrentInteriorModelIndex();

                if (interiorModelIndex != -1)
                {
                    // Check for custom interior: This has the highest priority
                    if (FindCustomInterior(interiorModelIndex, out CustomInterior customInterior))
                    {
                        // Set default location incase any of the custom interior details are not specified
                        SetDefaultLocation(zoneName);

                        bool hasLargeImageKey = !string.IsNullOrWhiteSpace(customInterior.LargeImageKey);
                        bool hasDisplayName =   !string.IsNullOrWhiteSpace(customInterior.DisplayName);

                        // Check for custom large image key - If not specified, uses default area image
                        if (hasLargeImageKey)
                            discordRichPresence.Assets.LargeImageKey = customInterior.LargeImageKey;

                        if (hasDisplayName)
                            discordRichPresence.Assets.LargeImageText = string.Format("{0} in {1}", customInterior.DisplayName, zoneName);
                        else
                            discordRichPresence.Assets.LargeImageText = string.Format("Building in {0}", zoneName);

                        return;
                    }
                    else
                    {
                        // Check which default interior the player is currently in
                        switch (interiorModelIndex)
                        {
                            // General
                            case 4222: // Strip Club Bohan
                            case 5071:
                            case 4654:
                                discordRichPresence.Assets.LargeImageKey = "sc_bohan";
                                discordRichPresence.Assets.LargeImageText = string.Format("Strip Club in {0}", zoneName);
                                return;
                            case 5164: // Strip Club Alderney
                            case 6013:
                            case 5596:
                                discordRichPresence.Assets.LargeImageKey = "sc_alderney";
                                discordRichPresence.Assets.LargeImageText = string.Format("Strip Club in {0}", zoneName);
                                return;
                            case 6422: // Burger Shot
                            case 7126:
                            case 6854:
                                discordRichPresence.Assets.LargeImageKey = "burger_shot";
                                discordRichPresence.Assets.LargeImageText = string.Format("Burger Shot in {0}", zoneName);
                                return;
                            case 6423: // Cluckin Bell
                            case 7127:
                            case 6855:
                                discordRichPresence.Assets.LargeImageKey = "cluckin_bell";
                                discordRichPresence.Assets.LargeImageText = string.Format("Cluckin' Bell in {0}", zoneName);
                                return;
                            case 4385: // Hospital
                            case 5234:
                            case 4817:
                                discordRichPresence.Assets.LargeImageKey = "hospital";
                                discordRichPresence.Assets.LargeImageText = string.Format("Hospital in {0}", zoneName);
                                return;
                            case 5585: // Memory Lanes (Bowling Alley)
                            case 29291:
                            case 27944:
                                discordRichPresence.Assets.LargeImageKey = "memory_lanes";
                                discordRichPresence.Assets.LargeImageText = string.Format("Memory Lanes in {0}", zoneName);
                                return;
                            case 4221: // Steinway Beer Garden (Darts)
                            case 5070:
                            case 4653:
                                discordRichPresence.Assets.LargeImageKey = "steinway_beer_garden";
                                discordRichPresence.Assets.LargeImageText = string.Format("Steinway Beer Garden in {0}", zoneName);
                                return;
                            case 4386: // Homebrew Cafe (Billiard)
                            case 5235:
                            case 4818:
                                discordRichPresence.Assets.LargeImageKey = "homebrew_cafe";
                                discordRichPresence.Assets.LargeImageText = string.Format("Homebrew Cafe in {0}", zoneName);
                                return;
                            case 5067: // Perestroika (Cabaret)
                            case 5916:
                            case 5499:
                                discordRichPresence.Assets.LargeImageKey = "cabaret";
                                discordRichPresence.Assets.LargeImageText = string.Format("Cabaret Perestroika in {0}", zoneName);
                                return;
                            case 6144: // Internet Cafe (tw@)
                            case 6848:
                            case 6576:
                                discordRichPresence.Assets.LargeImageKey = "internet_cafe";
                                discordRichPresence.Assets.LargeImageText = string.Format("tw@ in {0}", zoneName);
                                return;
                            case 6262: // 69th Street Diner
                                discordRichPresence.Assets.LargeImageKey = "69th_street_diner";
                                discordRichPresence.Assets.LargeImageText = string.Format("69th Street Diner in {0}", zoneName);
                                return;
                            case 6821: // Gun Shop
                            case 6822:
                            case 6118:
                            case 6117:
                            case 6550:
                            case 6549:
                                discordRichPresence.Assets.LargeImageKey = "gun_shop";
                                discordRichPresence.Assets.LargeImageText = string.Format("Gun Shop in {0}", zoneName);
                                return;
                            case 6426: // Bohan Sprunk Warehouse
                                discordRichPresence.Assets.LargeImageKey = "bohan_sprunk_warehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Sprunk Warehouse in {0}", zoneName);
                                return;
                            case 5584: // Abandoned Sprunk Factory
                                discordRichPresence.Assets.LargeImageKey = "abandoned_sprunk_factory";
                                discordRichPresence.Assets.LargeImageText = string.Format("Abandoned Sprunk Factory in {0}", zoneName);
                                return;
                            case 5919: // Playboy X's Penthouse
                            case 6768:
                            case 6351:
                                discordRichPresence.Assets.LargeImageKey = "pbx_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Playboy X's Penthouse in {0}", zoneName);
                                return;

                            // GTA IV
                            case 4139: // Broker Safehouse
                                discordRichPresence.Assets.LargeImageKey = "gtaiv_broker_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Safehouse in {0}", zoneName);
                                return;
                            case 5681: // South Bohan Apartment
                                discordRichPresence.Assets.LargeImageKey = "gtaiv_south_bohan_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Apartment in {0}", zoneName);
                                return;
                            case 5684: // Middle Park East Safehouse
                                discordRichPresence.Assets.LargeImageKey = "gtaiv_middle_park_east_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Safehouse in {0}", zoneName);
                                return;
                            case 5678: // Alderney Safehouse
                                discordRichPresence.Assets.LargeImageKey = "gtaiv_alderney_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Safehouse in {0}", zoneName);
                                return;

                            // TLaD
                            case 27827: // Clubhouse
                                discordRichPresence.Assets.LargeImageKey = "tlad_the_lost_clubhouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Clubhouse in {0}", zoneName);
                                return;
                            case 28024: // Brian's Safehouse
                                discordRichPresence.Assets.LargeImageKey = "tlad_brians_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Safehouse in {0}", zoneName);
                                return;
                            case 29316: // Luis Safehouse
                                discordRichPresence.Assets.LargeImageKey = "tbogt_luis_safehouse";
                                discordRichPresence.Assets.LargeImageText = string.Format("Safehouse in {0}", zoneName);
                                return;

                            // TBoGT
                            case 28931: // Maisonette 9
                                discordRichPresence.Assets.LargeImageKey = "maisonette";
                                discordRichPresence.Assets.LargeImageText = string.Format("Maisonette in {0}", zoneName);
                                return;
                            case 29026: // Hercules
                                discordRichPresence.Assets.LargeImageKey = "hercules";
                                discordRichPresence.Assets.LargeImageText = string.Format("Hercules in {0}", zoneName);
                                return;
                            case 28578: // Bahama Mamas
                                discordRichPresence.Assets.LargeImageKey = "bahama_mamas";
                                discordRichPresence.Assets.LargeImageText = string.Format("Bahama Mamas in {0}", zoneName);
                                return;
                        }
                    }
                }

                discordRichPresence.Assets.LargeImageText = string.Format("Building in {0}", zoneName);
                return;
            }

            // Predefined locations: They have priority over the default locations
            for (int i = 0; i < predefinedLocations.Count; i++)
            {
                PredefinedLocations predefinedLocation = predefinedLocations[i];

                if (predefinedLocation.CustomRichPresence == null)
                    continue;

                if (predefinedLocation.RangeTrigger != null)
                {
                    // Check if player is within the desired range
                    if (Vector3.Distance(playerPos, predefinedLocation.RangeTrigger.Position) < predefinedLocation.RangeTrigger.Range)
                    {
                        currentPredefinedLocation = predefinedLocation.ID;

                        // Set custom stuff
                        if (!string.IsNullOrWhiteSpace(predefinedLocation.CustomRichPresence.LargeImageKey))
                            discordRichPresence.Assets.LargeImageKey = predefinedLocation.CustomRichPresence.LargeImageKey;
                        if (!string.IsNullOrWhiteSpace(predefinedLocation.CustomRichPresence.LargeImageText))
                            discordRichPresence.Assets.LargeImageText = predefinedLocation.CustomRichPresence.LargeImageText;

                        return;
                    }
                }
                //else if (predefinedLocation.BoxTrigger != null)
                //{
                //    // Check if player is within the desired box
                //    if (IsCharInArea3D(playerHandle, predefinedLocation.BoxTrigger.Pos1, predefinedLocation.BoxTrigger.Pos2))
                //    {
                //        currentPredefinedLocation = predefinedLocation.ID;

                //        // Set custom stuff
                //        if (!string.IsNullOrWhiteSpace(predefinedLocation.CustomRichPresence.LargeImageKey))
                //            discordRichPresence.Assets.LargeImageKey = predefinedLocation.CustomRichPresence.LargeImageKey;
                //        if (!string.IsNullOrWhiteSpace(predefinedLocation.CustomRichPresence.LargeImageText))
                //            discordRichPresence.Assets.LargeImageText = predefinedLocation.CustomRichPresence.LargeImageText;

                //        return;
                //    }
                //}
            }

            currentPredefinedLocation = "";

            // Default locations: Lowest priority
            SetDefaultLocation(zoneName);
        }
        private void SetDefaultLocation(string zoneLocation)
        {
            if (zoneToIslandDictDCReady.TryGetValue(currentZoneRawName, out string largeImageKey))
            {
                discordRichPresence.Assets.LargeImageKey = largeImageKey;
            }

            discordRichPresence.Assets.LargeImageText = string.Format("{0} in {1}", currentZoneDisplayName, zoneLocation);
        }
        private void SetDefaultInteriorDetails(short modelIndex)
        {
            // Check default interiors
            switch (modelIndex)
            {
                case 4222: // Strip Club Bohan
                case 5071:
                case 4654:
                case 5164: // Strip Club Alderney
                case 6013:
                case 5596:
                    discordRichPresence.Details = "Enjoying the Strip Club";
                    return;
                case 6422: // Burger Shot
                case 7126:
                case 6854:
                    discordRichPresence.Details = "In Burger Shot";
                    return;
                case 6423: // Cluckin Bell
                case 7127:
                case 6855:
                    discordRichPresence.Details = "In Cluckin' Bell";
                    return;
                case 4385: // Hospital
                case 5234:
                case 4817:
                    discordRichPresence.Details = "In Hospital";
                    return;
                case 5585: // Memory Lanes (Bowling Alley)
                case 29291:
                case 27944:
                    discordRichPresence.Details = "In Bowling Alley";
                    return;
                case 4221: // Steinway Beer Garden (Darts)
                case 5070:
                case 4653:
                    discordRichPresence.Details = "In Steinway's Beer Garden";
                    return;
                case 4386: // Homebrew Cafe (Billiard)
                case 5235:
                case 4818:
                    discordRichPresence.Details = "In Homebrew's Cafe";
                    return;
                case 5067: // Perestroika (Cabaret)
                case 5916:
                case 5499:
                    discordRichPresence.Details = "In Cabaret Perestroika";
                    return;
                case 6144: // Internet Cafe (tw@)
                case 6848:
                case 6576:
                    discordRichPresence.Details = "In tw@";
                    return;
                case 6262: // 69th Street Diner
                    discordRichPresence.Details = "In 69th Street Diner";
                    return;
                case 6821: // Gun Shop
                case 6822:
                case 6118:
                case 6117:
                case 6550:
                case 6549:
                    discordRichPresence.Details = "In Gun Shop";
                    return;

                case 4139: // GTA IV Broker Safehouse
                    discordRichPresence.Details = "In Broker Safehouse";
                    return;
                case 5681: // GTA IV South Bohan Apartment
                    discordRichPresence.Details = "In South Bohan Apartment";
                    return;
                case 5684: // GTA IV Middle Park East Safehouse
                    discordRichPresence.Details = "In Middle Park East Safehouse";
                    return;
                case 5678: // GTA IV Alderney Safehouse
                    discordRichPresence.Details = "In Alderney Safehouse";
                    return;
                case 5919: // Playboy X's Penthouse
                case 6768:
                case 6351:
                    discordRichPresence.Details = "In Playboy X's Penthouse";
                    return;
                case 6426: // Bohan Sprunk Warehouse
                    discordRichPresence.Details = "In Bohan's Sprunk Warehouse";
                    return;
                case 5584: // Abandoned Sprunk Factory
                    discordRichPresence.Details = "In Abandoned Sprunk Factory";
                    return;

                case 27827: // TLaD Clubhouse
                    discordRichPresence.Details = "In Lost MC Clubhouse";
                    return;
                case 28024: // TLaD Brian's Safehouse
                    discordRichPresence.Details = "In Brian's Safehouse";
                    return;
                case 29316: // TBoGT Luis Safehouse
                    discordRichPresence.Details = "In Luis Safehouse";
                    return;

                case 28931: // TBoGT Maisonette 9
                    discordRichPresence.Details = "Hanging out in Maisonette 9";
                    return;
                case 29026: // TBoGT Hercules
                    discordRichPresence.Details = "Hanging out in Hercules";
                    return;
                case 28578: // TBoGT Bahama Mamas
                    discordRichPresence.Details = "Hanging out in Bahama Mamas";
                    return;
            }

            // Generic building text
            discordRichPresence.Details = "Inside a Building";

            if (IsPlayerDoingAMission(out string missionName))
            {
                switch ((int)currentTextState % 3)
                {
                    case 0: // Building location
                        discordRichPresence.State = string.Format("In {0}", currentZoneDisplayName);
                        break;
                    case 1: // Doing a mission or watching cutscene
                        discordRichPresence.State = HAS_CUTSCENE_FINISHED() == false ? "Watching a cutscene" : "Doing a mission";
                        break;
                    case 2: // Mission name
                        discordRichPresence.State = missionName;
                        break;
                }
            }
            else
            {
                discordRichPresence.State = string.Format("In {0}", currentZoneDisplayName);
            }
        }

        private void SetSingleplayerStuff()
        {
            // Get current episode info
            EpisodeInfo episodeInfo = GetEpisodeInfo(GET_CURRENT_EPISODE());
            string playerIconKey = episodeInfo.ProtagonistImageKey;

            // Check dead state
            if (episodeInfo.IsBaseEpisode)
            {
                if (playerDead)
                {
                    playerIconKey = playerIconKey + "_red";
                }
                else
                {
                    // Make player icon blink if wanted
                    if (currentWantedLevel > 0)
                        playerIconKey = playerIconKey + (wantedBlinkState == 0 ? "_red" : "_blue");
                }
            }

            // Set small image text - Show different kinds of information based on the currentTextState
            switch ((int)currentTextState % (currentPlayerWeapon == 0 ? 3 : 4))
            {
                case 0: // Protagonist Name
                    discordRichPresence.Assets.SmallImageText = episodeInfo.ProtagonistName;
                    break;
                case 1: // Health, Armour
                    discordRichPresence.Assets.SmallImageText = string.Format("Health: {0}, Armour: {1}", playerHealth, playerArmour);
                    break;
                case 2: // Money
                    discordRichPresence.Assets.SmallImageText = string.Format("Money: ${0:N}", currentScore);
                    break;
                case 3: // Equipped Weapon
                    discordRichPresence.Assets.SmallImageText = string.Format("Equipped Weapon: {0}", NativeGame.GetCommonWeaponName((eWeaponType)currentPlayerWeapon));
                    break;

            }

            // Update
            discordRichPresence.Assets.SmallImageKey = playerIconKey;
        }
        private void SetNetworkStuff()
        {
            if (!discordRichPresence.HasParty())
                discordRichPresence.Party = new Party();

            // Get player stuff
            string playerName = GET_PLAYER_NAME(playerIndex);

            // Set small image stuff
            discordRichPresence.Assets.SmallImageKey = IS_CHAR_DEAD(playerHandle) ? "mp_player_red" : "mp_player";

            switch ((NetworkGameMode)NETWORK_GET_GAME_MODE())
            {
                // Game modes with a score
                case NetworkGameMode.Deathmatch:
                case NetworkGameMode.TLaD_Deathmatch:
                case NetworkGameMode.TBoGT_Deathmatch:
                case NetworkGameMode.MafiyaWork:
                case NetworkGameMode.CarJackCity:
                case NetworkGameMode.GTARace:
                case NetworkGameMode.TBoGT_GTARace:
                case NetworkGameMode.TurfWar:
                case NetworkGameMode.TLaD_ClubBusiness:

                    switch ((int)currentTextState % 3)
                    {
                        case 0: // Player name
                            discordRichPresence.Assets.SmallImageText = playerName;
                            break;
                        case 1: // Current Score
                            discordRichPresence.Assets.SmallImageText = string.Format("Current Score: {0}", currentScore);
                            break;
                        case 2: // Health, Armour
                            discordRichPresence.Assets.SmallImageText = string.Format("Health: {0}, Armour: {1}", playerHealth, playerArmour);
                            break;
                    }

                    break;

                // Game modes with a score and a team
                case NetworkGameMode.TeamDeathmatch:
                case NetworkGameMode.TLaD_TeamDeathmatch:
                case NetworkGameMode.TBoGT_TeamDeathmatch:
                case NetworkGameMode.TeamMafiyaWork:
                case NetworkGameMode.TeamCarJackCity:
                case NetworkGameMode.CopsAndCrooks:
                case NetworkGameMode.TLaD_ChopperVsChopper:
                case NetworkGameMode.TLaD_WitnessProtection:

                    switch ((int)currentTextState % 4)
                    {
                        case 0: // Player name
                            discordRichPresence.Assets.SmallImageText = playerName;
                            break;
                        case 1: // Current Team
                            discordRichPresence.Assets.SmallImageText = string.Format("Current Team: {0}", GET_PLAYER_TEAM((int)GET_PLAYER_ID()));
                            break;
                        case 2: // Current Score
                            discordRichPresence.Assets.SmallImageText = string.Format("Current Score: {0}", currentScore);
                            break;
                        case 3: // Health, Armour
                            discordRichPresence.Assets.SmallImageText = string.Format("Health: {0}, Armour: {1}", playerHealth, playerArmour);
                            break;
                    }

                    break;

                // "Normal" game modes
                case NetworkGameMode.FreeMode:
                case NetworkGameMode.PartyMode:
                case NetworkGameMode.Race:
                case NetworkGameMode.TLaD_Race:
                case NetworkGameMode.TBoGT_Race:
                case NetworkGameMode.DealBreaker:
                case NetworkGameMode.HangmansNoose:
                case NetworkGameMode.BombDaBaseII:
                case NetworkGameMode.TLaD_LoneWolfBiker:
                case NetworkGameMode.TLaD_OwnTheCity:

                    switch ((int)currentTextState % 2)
                    {
                        case 0: // Player name
                            discordRichPresence.Assets.SmallImageText = playerName;
                            break;
                        case 1: // Health, Armour
                            discordRichPresence.Assets.SmallImageText = string.Format("Health: {0}, Armour: {1}", playerHealth, playerArmour);
                            break;
                    }

                    break;
            }

            // Set party stuff
            discordRichPresence.Party.Size = (int)GET_NUMBER_OF_PLAYERS();
            discordRichPresence.Party.Max = (int)NETWORK_GET_MAX_SLOTS();
        }
        private void SetDetailsAndState()
        {
            if (IS_PAUSE_MENU_ACTIVE())
            {
                discordRichPresence.Details = "In pause menu";
                discordRichPresence.State = null;
                return;
            }

            // Handle player death
            if (playerDead)
            {

                if (IVNetwork.IsNetworkSession())
                {
                    string networkKillerName = GetNameOfNetworkKillerOfPlayer();

                    if (string.IsNullOrWhiteSpace(networkKillerName))
                    {
                        discordRichPresence.Details = "About to respawn";
                    }
                    else
                    {
                        switch (deathTextToShow)
                        {
                            case 0:
                                discordRichPresence.Details = string.Format("Was annihilated by {0}", networkKillerName);
                                break;
                            case 1:
                                discordRichPresence.Details = string.Format("Was erased by {0}", networkKillerName);
                                break;
                            case 2:
                                discordRichPresence.Details = string.Format("Was outplayed by {0}", networkKillerName);
                                break;
                            case 3:
                                discordRichPresence.Details = string.Format("Was obliterated by {0}", networkKillerName);
                                break;
                            case 4:
                                discordRichPresence.Details = string.Format("Was silenced by {0}", networkKillerName);
                                break;
                            case 5:
                                discordRichPresence.Details = string.Format("Got crushed by {0}", networkKillerName);
                                break;

                            default:
                                discordRichPresence.Details = string.Format("Was killed by {0}", networkKillerName);
                                break;
                        }
                        
                        if ((int)currentTextState % 2 == 1)
                        {
                            discordRichPresence.Details = "About to respawn";
                        }
                    }

                    discordRichPresence.State = GetDeathCauseDisplayText();
                }
                else
                {
                    discordRichPresence.Details = "About to respawn at hospital";
                    discordRichPresence.State = GetDeathCauseDisplayText();
                }

                return;
            }
            else
            {
                deathTextToShow = GENERATE_RANDOM_INT_IN_RANGE(0, 20);
            }

            // Custom presence: This has the highest priority
            if (ModSettings.AllowOtherModsToSetPresence)
            {
                if (idOfScriptWhichSetCustomPresence != Guid.Empty)
                {
                    // Check if the script which set the custom presence is still running
                    if (IsScriptRunning(idOfScriptWhichSetCustomPresence))
                    {
                        // Set the custom presence
                        discordRichPresence.Details = customRichPresence.Details;
                        discordRichPresence.State = customRichPresence.State;

                        return;
                    }
                    else
                    {
                        // Script is no longer running so we reset stuff
                        idOfScriptWhichSetCustomPresence = Guid.Empty;
                        customRichPresence = null;
                    }
                }
            }

            // Other mod stuff: This has the second highest priority
            if (ModSettings.ShowPresenceOfOtherMods)
            {
                if (SendScriptCommand("SimpleHotTub", "GET_IS_IN_HOT_TUB", null, out object result))
                {
                    if (Convert.ToBoolean(result))
                    {
                        discordRichPresence.Details = "Currently relaxing in a Hot Tub";

                        bool commandSentSuccessful =    SendScriptCommand("SimpleHotTub", "GET_TOTAL_TIME_IN_HOT_TUB", null, out object timeResult);
                        bool commandSentSuccessful2 =   SendScriptCommand("SimpleHotTub", "GET_AMOUNT_OF_PEOPLE_IN_HOT_TUB", null, out object peopleInHotTubResult);

                        if (!commandSentSuccessful && !commandSentSuccessful2)
                        {
                            discordRichPresence.State = null;
                            return;
                        }

                        // Get total time in Hot Tub
                        int modBy = 0;

                        if (commandSentSuccessful)
                            modBy++;
                        if (commandSentSuccessful2)
                            modBy++;

                        switch ((int)currentTextState % modBy)
                        {
                            case 0:
                                int peopleInHotTub = Convert.ToInt32(peopleInHotTubResult) - 1;

                                if (peopleInHotTub == 0)
                                    discordRichPresence.State = "Relaxing all by himself";
                                else
                                    discordRichPresence.State = string.Format("Accompanied by {0} other(s)", peopleInHotTub);
                                
                                break;
                            case 1:
                                discordRichPresence.State = string.Format("{0} seconds in", TimeSpan.FromTicks(Convert.ToInt64(timeResult)).Seconds);
                                break;
                        }

                        return;
                    }
                }
                if (SendScriptCommand("SimpleSpeedometer", "get_current_gas_station", null, out result)) // Temporary solution to check if the player is refilling their car
                {
                    int gasStationIndex = Convert.ToInt32(result);

                    if (gasStationIndex != 0)
                    {
                        discordRichPresence.Details = "Refilling their car";
                        discordRichPresence.State = string.Format("At {0}", (GasStation)gasStationIndex);
                        return;
                    }
                }
                if (SendScriptCommand("ProjectThunderIV", "is_blackout_active", null, out result))
                {
                    if (Convert.ToBoolean(result))
                    {
                        switch (blackoutTextToShow)
                        {
                            case 0:
                                discordRichPresence.Details = "Currently blinded by the darkness";
                                break;
                            case 1:
                                discordRichPresence.Details = "We Watch_Dogs now boys";
                                break;
                            case 2:
                                discordRichPresence.Details = "Welcome to LC, Aiden Pearce";
                                break;
                            case 3:
                                discordRichPresence.Details = "It's 1977 in Liberty City again";
                                break;
                            default:
                                discordRichPresence.Details = "Currently witnessing a Blackout";
                                break;
                        }
                    }
                    else
                    {
                        blackoutTextToShow = GENERATE_RANDOM_INT_IN_RANGE(0, 20);
                    }

                    return;
                }
            }

            // Credits
            if (!ARE_CREDITS_FINISHED())
            {
                discordRichPresence.Details = "Watching end credits";

                // Go through some stats
                if (ModSettings.ShowStatistics)
                {
                    switch (currentTextState)
                    {
                        case TextState.Default:
                            discordRichPresence.State = string.Format("{0} kills by headshots", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_KILLS_BY_HEADSHOTS));
                            break;
                        case TextState.State1:
                            discordRichPresence.State = string.Format("{0} deaths", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_PLAYER_SHOT_TO_DEATH));
                            break;
                        case TextState.State2:
                            discordRichPresence.State = string.Format("{0} addiction level", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_ADDICTION_LEVEL));
                            break;
                        case TextState.State3:
                            discordRichPresence.State = string.Format("Cheated {0} times", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_TIMES_CHEATED));
                            break;
                        case TextState.State4:
                            discordRichPresence.State = string.Format("{0} cars stolen", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_CARS_STOLEN));
                            break;
                        case TextState.State5:
                            discordRichPresence.State = string.Format("{0} bullets fired", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_BULLETS_FIRED));
                            break;
                        case TextState.State6:
                            discordRichPresence.State = string.Format("{0} failed missions", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_MISSIONS_FAILED));
                            break;
                        case TextState.State7:
                            discordRichPresence.State = string.Format("{0} prostitute visits", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_PROSTITUTE_VISITS));
                            break;
                        case TextState.State8:
                            discordRichPresence.State = string.Format("{0} completed stunt jumps", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_STUNT_JUMPS_COMPLETED));
                            break;
                        case TextState.State9:
                            discordRichPresence.State = string.Format("Got drunk {0} times", NativeGame.GetIntegerStatistic(eIntStatistic.STAT_TIMES_GOT_DRUNK));
                            break;
                        case TextState.State10:
                            discordRichPresence.State = string.Format("Total progress: {0}%", NativeGame.GetFloatStatistic(eFloatStatistic.STAT_TOTAL_PROGRESS));
                            break;
                    }
                }
                else
                {
                    discordRichPresence.State = null;
                }

                return;
            }

            // Swimming
            if (playerSwimming)
            {
                discordRichPresence.Details = string.Format("Swimming through {0}", currentZoneDisplayName);

                // On Mission
                if (IsPlayerDoingAMission(out string missionName))
                {
                    switch ((int)currentTextState % 2)
                    {
                        case 0: // Doing a mission
                            discordRichPresence.State = "Doing a mission";
                            break;
                        case 1: // Mission name
                            discordRichPresence.State = missionName;
                            break;
                    }
                }
                else
                {
                    discordRichPresence.State = null;
                }

                return;
            }

            // Vehicle
            if (playerInAnyCar)
            {
                GET_CAR_CHAR_IS_USING(playerHandle, out int playerVehicle);
                GET_CAR_MODEL(playerVehicle, out uint vehicleModel);

                // Check vehicle type
                if (IS_THIS_MODEL_A_BIKE(vehicleModel))
                    discordRichPresence.Details = string.Format("Motorcycling through {0}", currentZoneDisplayName);
                else if (IS_THIS_MODEL_A_BOAT(vehicleModel))
                    discordRichPresence.Details = string.Format("Boating through {0}", currentZoneDisplayName);
                else if (IS_THIS_MODEL_A_CAR(vehicleModel))
                    discordRichPresence.Details = string.Format("Driving through {0}", currentZoneDisplayName);
                else if (IS_THIS_MODEL_A_HELI(vehicleModel) || IS_THIS_MODEL_A_PLANE(vehicleModel))
                    discordRichPresence.Details = string.Format("Flying over {0}", currentZoneDisplayName);
                else if (IS_THIS_MODEL_A_TRAIN(vehicleModel))
                    discordRichPresence.Details = string.Format("Taking the train through {0}", currentZoneDisplayName);

                // Get current radio station
                string radioStationDisplayName = GetCurrentRadioStationDisplayName(false);

                // Get vehicle display name
                string vehicleDisplayName = GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(vehicleModel);

                if (DOES_TEXT_LABEL_EXIST(vehicleDisplayName))
                    vehicleDisplayName = GET_STRING_FROM_TEXT_FILE(vehicleDisplayName);

                bool isRomansCab = vehicleDisplayName == "Roman's Taxi";

                // Get the speed of the vehicle and convert to MPH
                GET_CAR_SPEED(playerVehicle, out float currentVehicleSpeedRaw);
                currentVehicleSpeedRaw = currentVehicleSpeedRaw * 2.23694f;
                int currentVehicleSpeed = (int)Math.Round(currentVehicleSpeedRaw);

                // Set rich presence
                if (currentVehicleSpeed == 0)
                    discordRichPresence.State = string.Format("{0} {1}", isRomansCab ? "With" : "With a", vehicleDisplayName);
                else
                    discordRichPresence.State = string.Format("Going {0} MPH {1} {2}", currentVehicleSpeed, isRomansCab ? "in" : "in a", vehicleDisplayName);
                
                if (radioStationDisplayName != null)
                {
                    if ((int)currentTextState % 2 == 1)
                        discordRichPresence.State = radioStationDisplayName;
                }

                // On Mission
                bool doingMission = IsPlayerDoingAMission(out string missionName);
                if (doingMission)
                {
                    switch ((int)currentTextState % 4)
                    {
                        case 2: // Doing a mission or watching cutscene
                            discordRichPresence.State = HAS_CUTSCENE_FINISHED() == false ? "Watching a cutscene" : "Doing a mission";
                            break;
                        case 3: // Mission name
                            discordRichPresence.State = missionName;
                            break;
                    }
                }

                // Horn
                if (IS_PLAYER_PRESSING_HORN(playerIndex))
                {
                    int index = 2;

                    if (radioStationDisplayName != null)
                        index++;
                    if (doingMission)
                        index = index + 2;

                    if ((int)currentTextState % index == 0)
                    {
                        switch (honkTextToShow)
                        {
                            case 0:
                                discordRichPresence.State = "Beeping at someone";
                                break;
                            case 1:
                                discordRichPresence.State = "Yoinking the horn";
                                break;
                            case 2:
                                discordRichPresence.State = "Being a New Yorker";
                                break;
                            default:
                                discordRichPresence.State = "Honking the horn";
                                break;
                        }
                    }
                    else
                    {
                        honkTextToShow = GENERATE_RANDOM_INT_IN_RANGE(0, 20);
                    }
                }

                // Some priority stuff
                if (IS_PLAYER_PERFORMING_WHEELIE((int)playerId))
                {
                    discordRichPresence.State = "Performing a wheelie!";

                    if ((int)currentTextState % 2 == 1)
                        discordRichPresence.State = string.Format("For {0} seconds", wheelieStoppieWatch.Elapsed.Seconds);
                }
                else if (IS_PLAYER_PERFORMING_STOPPIE((int)playerId))
                {
                    discordRichPresence.State = "Performing a stoppie!";

                    if ((int)currentTextState % 2 == 1)
                        discordRichPresence.State = string.Format("For {0} seconds", wheelieStoppieWatch.Elapsed.Seconds);
                }
            }
            // On Foot
            else
            {
                // Interiors: They have the highest priority
                if (IS_INTERIOR_SCENE())
                {
                    short interiorModelIndex = GetCurrentInteriorModelIndex();

                    if (interiorModelIndex != -1)
                    {
                        // Check for custom interior: This has the highest priority
                        if (FindCustomInterior(interiorModelIndex, out CustomInterior customInterior))
                        {
                            // Set default details
                            SetDefaultInteriorDetails(interiorModelIndex);

                            // Then override detail with custom one if specified
                            if (!string.IsNullOrWhiteSpace(customInterior.CustomDetailPresence))
                                discordRichPresence.Details = customInterior.CustomDetailPresence;
                        }
                        else
                        {
                            SetDefaultInteriorDetails(interiorModelIndex);
                        }
                        
                        return;
                    }
                }

                // Other stuff
                GET_CHAR_SPEED(playerHandle, out float currentCharSpeed);
                bool isDucking = IS_CHAR_DUCKING(playerHandle);

                if (currentCharSpeed < 1.0f)
                    discordRichPresence.Details = string.Format("{0} in {1}", isDucking ? "Crouching" : "Standing", currentZoneDisplayName);
                else if (currentCharSpeed >= 1.90f)
                    discordRichPresence.Details = string.Format("Running through {0}", currentZoneDisplayName);
                else
                    discordRichPresence.Details = string.Format("{0} through {1}", isDucking ? "Sneaking" : "Walking", currentZoneDisplayName);

                // On Mission
                if (IsPlayerDoingAMission(out string missionName2))
                {
                    switch ((int)currentTextState % 2)
                    {
                        case 0: // Doing a mission or watching cutscene
                            discordRichPresence.State = HAS_CUTSCENE_FINISHED() == false ? "Watching a cutscene" : "Doing a mission";
                            break;
                        case 1: // Mission name
                            discordRichPresence.State = missionName2;
                            break;
                    }
                }
                else
                {
                    discordRichPresence.State = null;
                }
            }
        }
        #endregion

        #region Functions
        private bool IsCharInArea3D(int handle, Vector3 pos1, Vector3 pos2)
        {
            return IS_CHAR_IN_AREA_3D(handle, pos2.X, pos2.Y, pos2.Z, pos1.X, pos1.Y, pos1.Z, false);
        }
        private EpisodeInfo GetEpisodeInfo(uint id)
        {
            return episodeInfos.Where(x => x.ID == id).FirstOrDefault();
        }
        private bool FindCustomInterior(short modelIndex, out CustomInterior customInterior)
        {
            CustomInterior interior = customInteriors.Where(x => (short)x.ModelIndex == modelIndex).FirstOrDefault();

            if (interior == null)
            {
                customInterior = null;
                return false;
            }

            customInterior = interior;
            return true;
        }
        private string GetDeathCauseDisplayText()
        {
            switch (causeOfDeath)
            {
                case CauseOfDeath.FallingDamage:         return "Died by falling damage";
                case CauseOfDeath.FleeingFromCops:       return "Died while fleeing from cops";
                case CauseOfDeath.FightingCops:          return "Died while fighting cops";
                case CauseOfDeath.Headshot:              return "Died by headshot";
                case CauseOfDeath.Fire:                  return "Burned to death";
                case CauseOfDeath.Explosion:             return "Died by an explosion";
                case CauseOfDeath.ExplosionNoobtubed:    return "Got noobtubed";
                case CauseOfDeath.ExplodingCar:          return "Died by an exploding Vehicle";
                case CauseOfDeath.ExplodingBike:         return "Died by an exploding Bike";
                case CauseOfDeath.ExplodingTruck:        return "Died by an exploding Truck";
                case CauseOfDeath.ExplodingPetrolPump:   return "Died by an exploding Petrol Pump";
            }

            return null;
        }
        private string GetCurrentRadioStationDisplayName(bool onlyGetDisplayName)
        {
            if (!string.IsNullOrEmpty(currentRadioStationName))
            {
                RadioInfo foundRadioInfo = radioStationInfos.Where(x => x.RawName == currentRadioStationName).FirstOrDefault();

                if (foundRadioInfo == null)
                    return null;

                if (onlyGetDisplayName)
                    return foundRadioInfo.DisplayName;
                else
                    return "Listening to " + foundRadioInfo.DisplayName;
            }

            return null;
        }
        private string GetNameOfNetworkKillerOfPlayer()
        {
            if (!IVNetwork.IsNetworkGameRunning())
                return null;

            string name = null;
            int networkKillerOfPlayer = FIND_NETWORK_KILLER_OF_PLAYER((int)playerId);

            if (networkKillerOfPlayer != (int)playerId)
            {
                // Check if local player really is dead and then get the name of the network killer
                if (IS_NETWORK_PLAYER_ACTIVE(networkKillerOfPlayer))
                {
                    if (IS_PLAYER_DEAD((int)playerId))
                        name = GET_PLAYER_NAME(networkKillerOfPlayer);
                }
            }

            return name;
        }
        private bool IsPlayerDoingAMission(out string name)
        {
            if (!IVTheScripts.IsPlayerOnAMission())
            {
                name = null;
                return false;
            }

            string missionName = GET_STRING_FROM_TEXT_FILE(IVTheScripts.GetGlobalString(9926));

            if (missionName == "NULL")
                missionName = "Probably on a hangout";

            name = missionName;

            return true;
        }
        private short GetCurrentInteriorModelIndex()
        {
            if (playerInterior == 0)
                return -1;

            UIntPtr ptr = interiorPool.GetAt((uint)playerInterior);

            if (ptr == UIntPtr.Zero)
                return -1;

            IVEntity interiorEntity = IVInteriorInst.FromUIntPtr(ptr);

            return interiorEntity.ModelIndex;
        }
        #endregion

        #region Events
        private void DiscordRpcClient_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
        {
            Prepare();
        }
        private void DiscordRpcClient_OnConnectionFailed(object sender, DiscordRPC.Message.ConnectionFailedMessage args)
        {
            Logging.LogWarning("[Discord] Failed to connect!");
            isDiscordReady = false;
        }
        private void DiscordRpcClient_OnError(object sender, DiscordRPC.Message.ErrorMessage args)
        {
            if (ModSettings.LogErrorsToConsole)
                Logging.LogError("[Discord] An error occured! Code: {0}, Message: {1}", args.Code, args.Message);
            //isDiscordReady = false;
        }

        private void RAGE_OnWindowFocusChanged(bool focused)
        {
            if (!isDiscordReady)
                return;

            isAltTabbed = !focused;

            if (focused)
                return;

            discordRichPresence.Details = "Alt-tabbed";
            discordRichPresence.State = null;

            // Update rich presence
            discordRpcClient.SetPresence(discordRichPresence);
        }
        #endregion

        private void Main_Uninitialize(object sender, EventArgs e)
        {
            isDiscordReady = false;

            // Unsubscribe from events
            RAGE.OnWindowFocusChanged -= RAGE_OnWindowFocusChanged;

            // Stop timers
            if (wantedBlinkTimerId != Guid.Empty)
            {
                AbortTaskOrTimer(wantedBlinkTimerId);
                wantedBlinkTimerId = Guid.Empty;
            }
            if (textSwitchingTimerId != Guid.Empty)
            {
                AbortTaskOrTimer(textSwitchingTimerId);
                textSwitchingTimerId = Guid.Empty;
            }

            // Cleanup
            if (discordRpcClient != null)
            {
                discordRpcClient.Dispose();
                discordRpcClient = null;
            }
            if (discordRichPresence != null)
            {
                discordRichPresence = null;
            }
            if (discordLogger != null)
            {
                discordLogger = null;
            }
            if (interiorPool != null)
            {
                interiorPool = null;
            }
            if (wheelieStoppieWatch != null)
            {
                wheelieStoppieWatch.Stop();
                wheelieStoppieWatch = null;
            }
        }
        private void Main_Initialized(object sender, EventArgs e)
        {
            // Load stuff
            ModSettings.Load(Settings);
            LoadEpisodeInfo();
            LoadPredefinedLocations();
            LoadRadioStationInfo();
            LoadCustomInteriors();

            // Init Discord logger
            discordLogger = new DiscordLogger();

#if DEBUG
            discordLogger.Level = DiscordRPC.Logging.LogLevel.Trace;
#else
            discordLogger.Level = DiscordRPC.Logging.LogLevel.None;
#endif

            // Init Discord client
            discordRpcClient = new DiscordRpcClient("888450942394056704", -1, discordLogger, false, null);
            discordRpcClient.OnReady += DiscordRpcClient_OnReady;
            discordRpcClient.OnConnectionFailed += DiscordRpcClient_OnConnectionFailed;
            discordRpcClient.OnError += DiscordRpcClient_OnError;
            discordRpcClient.SkipIdenticalPresence = true;
            discordRpcClient.Initialize();

            // Create rich presence object
            discordRichPresence = new RichPresence();
        }

        private object Main_ScriptCommandReceived(Script fromScript, object[] args, string command)
        {
            if (!isDiscordReady)
                return null;
            if (discordRpcClient == null)
                return null;
            if (!discordRpcClient.IsInitialized)
                return null;
            if (discordRpcClient.CurrentUser == null)
                return null;

            switch (command)
            {
                case "SET_CUSTOM_PRESENCE":

                    if (fromScript == null)
                        return false;
                    if (!ModSettings.AllowOtherModsToSetPresence)
                        return false;

                    string details = Convert.ToString(args[0]);
                    string state = Convert.ToString(args[1]);

                    idOfScriptWhichSetCustomPresence = fromScript.ID;

                    customRichPresence = new RichPresence()
                    {
                        Details = details,
                        State = state,
                    };

                    return true;
                case "CLEAR_CUSTOM_PRESENCE":
                    idOfScriptWhichSetCustomPresence = Guid.Empty;
                    customRichPresence = null;
                    break;
                case "WAS_CUSTOM_PRESENCE_SET": return idOfScriptWhichSetCustomPresence != Guid.Empty && customRichPresence != null;

                case "GET_CURRENT_DISCORD_USER_NAME":           return discordRpcClient.CurrentUser.Username;
                case "GET_CURRENT_DISCORD_USER_DISPLAY_NAME":   return discordRpcClient.CurrentUser.DisplayName;
                case "GET_CURRENT_DISCORD_USER_AVATAR_URL":
                    {
                        User.AvatarFormat format = (User.AvatarFormat)Convert.ToInt32(args[0]);
                        User.AvatarSize size = (User.AvatarSize)Convert.ToInt32(args[1]);

                        return discordRpcClient.CurrentUser.GetAvatarURL(format, size);
                    }
            }

            return null;
        }

        private void Main_Drawing(object sender, EventArgs e)
        {
            discordRpcClient.Invoke();
        }
        private void Main_OnImGuiRendering(IntPtr devicePtr, ImGuiIV_DrawingContext ctx)
        {
            DebugWindow();
            MainWindow();
        }
        private void DebugWindow()
        {
#if DEBUG
            if (!DebuggingWindowOpen)
                return;

            if (ImGuiIV.Begin("IV-Presence Debug", ref DebuggingWindowOpen, eImGuiWindowFlags.None, eImGuiWindowFlagsEx.NoMouseEnable))
            {
                ImGuiIV.TextDisabled("States");
                ImGuiIV.TextUnformatted("wantedBlinkState: {0}", wantedBlinkState);
                ImGuiIV.TextUnformatted("currentTextState: {0} (Mod by 2: {1})", currentTextState, (int)currentTextState % 2);

                ImGuiIV.Spacing(2);
                ImGuiIV.TextDisabled("Player");
                ImGuiIV.TextUnformatted("IsInAir: {0}", playerIsInAir);
                ImGuiIV.TextUnformatted("IsSwimming: {0}", playerSwimming);
                ImGuiIV.TextUnformatted("CauseOfDeath: {0}", causeOfDeath);
                ImGuiIV.TextUnformatted("Current Zone: {0}", currentZoneDisplayName);
                ImGuiIV.TextUnformatted("Current Zone Raw: {0}", currentZoneRawName);
                ImGuiIV.TextUnformatted("Current Predefined Loc: {0}", currentPredefinedLocation);
                ImGuiIV.TextUnformatted("Current Interior ModelIndex: {0} (Raw: {1})", GetCurrentInteriorModelIndex(), playerInterior);

                ImGuiIV.Spacing(2);
                ImGuiIV.TextDisabled("Radio");
                ImGuiIV.TextUnformatted("currentRadioStationName: {0}", currentRadioStationName);
            }
            ImGuiIV.End();
#endif
        }
        private void MainWindow()
        {
            if (!MenuOpened)
                return;

            if (ImGuiIV.Begin("IV-Presence", ref MenuOpened))
            {
                if (ImGuiIV.BeginTabBar("##IVPresenceTabBar"))
                {

                    SettingsTab();
                    EpisodeInfoTab();
                    RadioStationsTab();
                    PredefinedLocationsTab();
                    CustomInteriorsTab();

                    ImGuiIV.EndTabBar();
                }
            }
            ImGuiIV.End();
        }
        private void SettingsTab()
        {
            if (ImGuiIV.BeginTabItem("Settings##IVPresence"))
            {
                if (ImGuiIV.Button("Reload Settings"))
                {
                    if (Settings.Load())
                    {
                        ModSettings.Load(Settings);
                        Logging.Log("Settings file of IV-Presence was reloaded!");
                    }
                    else
                    {
                        Logging.LogWarning("Could not reload the settings file of IV-Presence! File might not exists.");
                    }
                }

                ImGuiIV.Spacing();
                ImGuiIV.SeparatorText("The Settings");

                ImGuiIV.TextUnformatted("General");
                ImGuiIV.CheckBox("LogErrorsToConsole", ref ModSettings.LogErrorsToConsole);
                ImGuiIV.CheckBox("AllowOtherModsToSetPresence", ref ModSettings.AllowOtherModsToSetPresence);
                ImGuiIV.CheckBox("ShowPresenceOfOtherMods", ref ModSettings.ShowPresenceOfOtherMods);

                ImGuiIV.Spacing(2);
                ImGuiIV.TextUnformatted("Credits");
                ImGuiIV.CheckBox("ShowStatistics", ref ModSettings.ShowStatistics);

                ImGuiIV.Spacing(2);
                ImGuiIV.TextUnformatted("Network");
                ImGuiIV.CheckBox("ShowNetworkKillerName", ref ModSettings.ShowNetworkKillerName);

                ImGuiIV.EndTabItem();
            }
        }
        private void EpisodeInfoTab()
        {
            if (ImGuiIV.BeginTabItem("Episode Info"))
            {
                ImGuiIV.SeparatorText("Control");
                if (ImGuiIV.Button("Save"))
                {
                    SaveEpisodeInfo();
                }
                ImGuiIV.SameLine();
                if (ImGuiIV.Button("Load"))
                {
                    LoadEpisodeInfo();
                }

                ImGuiIV.Spacing(3);
                ImGuiIV.SeparatorText("Items");
                if (ImGuiIV.Button("Add new"))
                {
                    episodeInfos.Add(new EpisodeInfo());
                }
                ImGuiIV.TextDisabled("Loaded {0} episode info items", episodeInfos.Count);

                for (int i = 0; i < episodeInfos.Count; i++)
                {
                    EpisodeInfo episodeInfo = episodeInfos[i];

                    if (ImGuiIV.TreeNode(string.Format("{0}##IVPresenceEpisodeInfoNode", i)))
                    {
                        ImGuiIV.SeparatorText("Control");
                        if (ImGuiIV.Button("Delete"))
                        {
                            episodeInfos.RemoveAt(i);
                            i--;
                            ImGuiIV.TreePop();
                            continue;
                        }

                        ImGuiIV.SeparatorText("Details");
                        ImGuiIV.SliderInt("ID", ref episodeInfo.ID, 0, 255);
                        ImGuiIV.CheckBox("IsBaseEpisode", ref episodeInfo.IsBaseEpisode);
                        ImGuiIV.InputText("ProtagonistName", ref episodeInfo.ProtagonistName);
                        ImGuiIV.InputText("ProtagonistImageKey", ref episodeInfo.ProtagonistImageKey);

                        ImGuiIV.TreePop();
                    }
                }

                ImGuiIV.EndTabItem();
            }
        }
        private void RadioStationsTab()
        {
            if (ImGuiIV.BeginTabItem("Radio Stations"))
            {
                ImGuiIV.SeparatorText("Control");
                if (ImGuiIV.Button("Save"))
                {
                    SaveRadioStationInfo();
                }
                ImGuiIV.SameLine();
                if (ImGuiIV.Button("Load"))
                {
                    LoadRadioStationInfo();
                }

                ImGuiIV.Spacing(3);
                ImGuiIV.SeparatorText("Items");
                if (ImGuiIV.Button("Create new"))
                {
                    radioStationInfos.Add(new RadioInfo());
                }
                ImGuiIV.TextDisabled("Loaded {0} radio station items", radioStationInfos.Count);

                for (int i = 0; i < radioStationInfos.Count; i++)
                {
                    RadioInfo radioInfo = radioStationInfos[i];

                    if (ImGuiIV.TreeNode(string.Format("{0}##IVPresenceRadioStationsNode", i)))
                    {
                        ImGuiIV.SeparatorText("Control");
                        if (ImGuiIV.Button("Delete"))
                        {
                            radioStationInfos.RemoveAt(i);
                            i--;
                            ImGuiIV.TreePop();
                            continue;
                        }

                        ImGuiIV.SeparatorText("Details");
                        ImGuiIV.InputText("RawName", ref radioInfo.RawName);
                        ImGuiIV.InputText("DisplayName", ref radioInfo.DisplayName);

                        ImGuiIV.TreePop();
                    }
                }

                ImGuiIV.EndTabItem();
            }
        }
        private void PredefinedLocationsTab()
        {
            if (ImGuiIV.BeginTabItem("Predefined Locations"))
            {
                ImGuiIV.SeparatorText("Control");
                if (ImGuiIV.Button("Save"))
                {
                    SavePredefinedLocations();
                }
                ImGuiIV.SameLine();
                if (ImGuiIV.Button("Load"))
                {
                    LoadPredefinedLocations();
                }

                ImGuiIV.Spacing(3);
                ImGuiIV.SeparatorText("Items");
                if (ImGuiIV.Button("Add new"))
                {
                    predefinedLocations.Add(new PredefinedLocations());
                }
                ImGuiIV.TextDisabled("Loaded {0} predefined locations items", predefinedLocations.Count);

                for (int i = 0; i < predefinedLocations.Count; i++)
                {
                    PredefinedLocations predefinedLocation = predefinedLocations[i];

                    if (ImGuiIV.TreeNode(string.Format("{0}##IVPresencePredefinedLocationsNode", i)))
                    {
                        ImGuiIV.SeparatorText("Control");
                        if (ImGuiIV.Button("Delete"))
                        {
                            predefinedLocations.RemoveAt(i);
                            i--;
                            ImGuiIV.TreePop();
                            continue;
                        }

                        ImGuiIV.SeparatorText("Details");
                        ImGuiIV.InputText("ID", ref predefinedLocation.ID);

                        if (predefinedLocation.RangeTrigger != null)
                        {
                            if (ImGuiIV.TreeNode(string.Format("RangeTrigger##IVPresencePredefinedLocationsNode")))
                            {
                                if (ImGuiIV.Button("Delete"))
                                {
                                    predefinedLocation.RangeTrigger = null;
                                }
                                else
                                {
                                    if (ImGuiIV.Button("Set to current pos"))
                                    {
                                        predefinedLocation.RangeTrigger.Position = playerPos;
                                    }
                                    ImGuiIV.SameLine();
                                    ImGuiIV.DragFloat3("Position", ref predefinedLocation.RangeTrigger.Position);
                                    ImGuiIV.DragFloat("Range", ref predefinedLocation.RangeTrigger.Range);
                                }

                                ImGuiIV.TreePop();
                            }
                        }
                        else
                        {
                            if (ImGuiIV.Button("Add range trigger"))
                            {
                                predefinedLocation.RangeTrigger = new TriggerRange();
                            }
                        }
                        if (predefinedLocation.CustomRichPresence != null)
                        {
                            if (ImGuiIV.TreeNode(string.Format("CustomRichPresence##IVPresencePredefinedLocationsNode")))
                            {
                                if (ImGuiIV.Button("Delete"))
                                {
                                    predefinedLocation.CustomRichPresence = null;
                                }
                                else
                                {
                                    ImGuiIV.InputText("LargeImageKey", ref predefinedLocation.CustomRichPresence.LargeImageKey);
                                    ImGuiIV.InputText("LargeImageText", ref predefinedLocation.CustomRichPresence.LargeImageText);
                                }

                                ImGuiIV.TreePop();
                            }
                        }
                        else
                        {
                            if (ImGuiIV.Button("Add custom rich presence"))
                            {
                                predefinedLocation.CustomRichPresence = new CustomRichPresence();
                            }
                        }

                        ImGuiIV.TreePop();
                    }
                }

                ImGuiIV.EndTabItem();
            }
        }
        private void CustomInteriorsTab()
        {
            if (ImGuiIV.BeginTabItem("Custom Interiors"))
            {
                ImGuiIV.SeparatorText("Control");
                if (ImGuiIV.Button("Save"))
                {
                    SaveCustomInteriors();
                }
                ImGuiIV.SameLine();
                if (ImGuiIV.Button("Load"))
                {
                    LoadCustomInteriors();
                }

                ImGuiIV.Spacing(3);
                ImGuiIV.SeparatorText("Items");
                if (ImGuiIV.Button("Add new"))
                {
                    customInteriors.Add(new CustomInterior());
                }
                ImGuiIV.TextDisabled("Loaded {0} custom interior items", customInteriors.Count);

                for (int i = 0; i < customInteriors.Count; i++)
                {
                    CustomInterior customInterior = customInteriors[i];

                    if (ImGuiIV.TreeNode(string.Format("{0}##IVPresenceCustomInteriors", i)))
                    {
                        ImGuiIV.SeparatorText("Control");
                        if (ImGuiIV.Button("Delete"))
                        {
                            customInteriors.RemoveAt(i);
                            i--;
                            ImGuiIV.TreePop();
                            continue;
                        }

                        ImGuiIV.SeparatorText("Details");
                        if (ImGuiIV.Button("Set to current interior"))
                        {
                            if (playerInterior != 0)
                            {
                                UIntPtr ptr = interiorPool.GetAt((uint)playerInterior);

                                if (ptr != UIntPtr.Zero)
                                {
                                    IVEntity ent = IVInteriorInst.FromUIntPtr(ptr);
                                    customInterior.ModelIndex = ent.ModelIndex;
                                }
                            }
                        }
                        ImGuiIV.SameLine();
                        ImGuiIV.SliderInt("ModelIndex", ref customInterior.ModelIndex, 0, short.MaxValue);
                        ImGuiIV.InputText("LargeImageKey", ref customInterior.LargeImageKey);
                        ImGuiIV.InputText("DisplayName", ref customInterior.DisplayName);
                        ImGuiIV.InputText("CustomDetailPresence", ref customInterior.CustomDetailPresence);

                        ImGuiIV.TreePop();
                    }
                }

                ImGuiIV.EndTabItem();
            }
        }

        private void Main_Tick(object sender, EventArgs e)
        {
            if (!isDiscordReady)
                return;
            if (isAltTabbed)
                return;

            // Get pools
            if (interiorPool == null)
                interiorPool = IVPools.GetInteriorInstPool();

            // Get player stuff
            playerId = GET_PLAYER_ID();
            playerIndex = CONVERT_INT_TO_PLAYERINDEX(playerId);
            GET_PLAYER_CHAR(playerIndex, out playerHandle);
            GET_CHAR_COORDINATES(playerHandle, out playerPos);
            GET_CHAR_HEALTH(playerHandle, out playerHealth);
            GET_CHAR_ARMOUR(playerHandle, out playerArmour);
            GET_INTERIOR_FROM_CHAR(playerHandle, out playerInterior);
            playerDead = IS_CHAR_DEAD(playerHandle);
            playerIsInAir = IS_CHAR_IN_AIR(playerHandle);
            playerSwimming = IS_CHAR_SWIMMING(playerHandle);
            playerInAnyCar = IS_CHAR_IN_ANY_CAR(playerHandle);
            playerPerformingWheelie = IS_PLAYER_PERFORMING_WHEELIE((int)playerId);
            playerPerformingStoppie = IS_PLAYER_PERFORMING_STOPPIE((int)playerId);
            STORE_SCORE(playerIndex, out currentScore);
            STORE_WANTED_LEVEL(playerIndex, out currentWantedLevel);
            currentRadioStationName = GET_PLAYER_RADIO_STATION_NAME();
            currentZoneRawName = GET_NAME_OF_ZONE(playerPos);
            currentZoneDisplayName = GET_STRING_FROM_TEXT_FILE(currentZoneRawName);

            // Check if performing wheelie/stoppie and measure time
            bool holdingWheelieKey = false;
            bool holdingStoppieKey = false;

            if (IS_USING_CONTROLLER())
            {
                holdingWheelieKey = ImGuiIV.IsKeyDown(eImGuiKey.ImGuiKey_GamepadLStickDown);
                holdingStoppieKey = ImGuiIV.IsKeyDown(eImGuiKey.ImGuiKey_GamepadLStickUp);
            }
            else
            {
                holdingWheelieKey = ImGuiIV.IsKeyDown(eImGuiKey.ImGuiKey_LeftCtrl);
                holdingStoppieKey = ImGuiIV.IsKeyDown(eImGuiKey.ImGuiKey_LeftShift);
            }

            if ((playerPerformingWheelie && holdingWheelieKey) || (playerPerformingStoppie && holdingStoppieKey))
            {
                // Start time measurement
                if (!wheelieStoppieWatch.IsRunning)
                    wheelieStoppieWatch.Restart();
            }
            else
            {
                // Stop time measurement
                wheelieStoppieWatch.Stop();
            }

            // Check cause of death
            if (playerDead)
            {
                CheckCauseOfDeath();
            }
            else
            {
                checkedForCauseOfDeath = false;
                causeOfDeath = CauseOfDeath.Unknown;
            }

            // We get the current weapon after we checked the cause of death because the player drops the weapon on death so we will never get the "FightingCops" death cause
            GET_CURRENT_CHAR_WEAPON(playerHandle, out currentPlayerWeapon);

            // Set rich presence
            if (!discordRichPresence.HasAssets())
                discordRichPresence.Assets = new Assets();

            SetLargeImageStuff();

            if (!IVNetwork.IsNetworkSession())
                SetSingleplayerStuff();
            else
                SetNetworkStuff();

            SetDetailsAndState();

            // WORKS!!!
            //discordRichPresence.Assets.LargeImageKey = "https://media.istockphoto.com/id/157030584/vector/thumb-up-emoticon.jpg?s=612x612&w=0&k=20&c=GGl4NM_6_BzvJxLSl7uCDF4Vlo_zHGZVmmqOBIewgKg=";

            // Update rich presence
            discordRpcClient.SetPresence(discordRichPresence);
        }

    }
}

// IV-Presence by ItsClonkAndre
// Version 1.4.1

#define IVPresenceExport _declspec(dllexport)
#include "IVSDK.cpp"
#include "discord_rpc.h"
#include <map>
#include <ctime>

#pragma region Structs
struct CustomRichPresence {
    bool isAvailable;
    const char* details;
    const char* state;
    const char* largeImageKey;
    const char* largeImageText;
    int partySize;
    int partyMax;
};
#pragma endregion

#pragma region Variables_And_Maps
// Discord Stuff
static const char* APPLICATION_ID = "888450942394056704";

DiscordRichPresence dPresence;
CustomRichPresence dCustomPresence;

// GTA Stuff
bool isDiscordReady, isPauseMenuActive, isDead, isDoingAMission, isSwimming;
bool isNetworkSession, isNetworkSessionRunning, isNetworkGameRunning;
float pX, pY, pZ, charSpeed;
unsigned int tempTime, scriptInterval, currentEpisode, wantedLevel, health, armor, mpScore;
u32 mpGameMode, mpPlayerCount, mpMaxPlayerCount, mpTeam;

Scripting::Player player;
Scripting::Ped playerPed;
Scripting::Interior currentInteriorRaw;
std::time_t elapsedTime;

char currentLocationBuffer[256];
std::string currentZone, currentZoneForDC, currentZoneLocation, currentRadioStation;

// Maps
std::map<std::string, std::string> GTAZoneNames{
    {"WESDY", "Westdyke"},
    {"LEFWO", "Leftwood"},
    {"ALDCI", "Alderney City"},
    {"BERCH", "Berchem"},
    {"NORMY", "Normandy"},
    {"ACTRR", "Acter"},
    {"PORTU", "Port Tudor"},
    {"TUDOR", "Tudor"},
    {"ACTIP", "Acter Industrial Park"},
    {"ALSCF", "Alderney State Correctional Facility"},
    {"HAPIN", "Happiness Island"},
    {"CASGR", "Castle Gardens"},
    {"THXCH", "The Exchange"},
    {"FISSO", "Fishmarket South"},
    {"CHITO", "Chinatown"},
    {"CITH", "City Hall"},
    {"CASGC", "Castle Garden City"},
    {"SUFFO", "Suffolk"},
    {"LITAL", "Little Italy"},
    {"LOWEA", "Lower Easton"},
    {"FISSN", "Fishmarket North"},
    {"THPRES", "Presidents City"},
    {"EASON", "Easton"},
    {"THTRI", "The Triangle"},
    {"TMEQU", "The Meat Quarter"},
    {"WESMI", "Westminster"},
    {"STARJ", "Star Junction"},
    {"LANCE", "Lancet"},
    {"HATGA", "Hatton Gardens"},
    {"PUGAT", "Purgatory"},
    {"MIDPW", "Middle Park West"},
    {"MIDPA", "Middle Park"},
    {"MIDPE", "Middle Park East"},
    {"LANCA", "Lancaster"},
    {"VASIH", "Varsity Heights"},
    {"NOHOL", "North Holland"},
    {"EAHOL", "East Holland"},
    {"NORWO", "Northwood"},
    {"CHISL", "Charge Island"},
    {"COISL", "Colony Island"},
    {"STHBO", "South Bohan"},
    {"CHAPO", "Chase Point"},
    {"FORSI", "Fortside"},
    {"BOULE", "Boulevard"},
    {"NRTGA", "Northern Gardens"},
    {"INSTI", "Industrial"},
    {"LTBAY", "Little Bay"},
    {"STEIN", "Steinway"},
    {"MEADP", "Meadows Park"},
    {"FRANI", "Francis International Airport"},
    {"WILLI", "Willis"},
    {"MEADH", "Meadow Hills"},
    {"EISLC", "East Island City"},
    {"BOAB", "Boabo"},
    {"CERHE", "Cerveza Heights"},
    {"BEECW", "Beachwood City"},
    {"SCHOL", "Schottler"},
    {"DOWTW", "Downtown"},
    {"ROTTH", "Rotterdam Hill"},
    {"ESHOO", "East Hook"},
    {"OUTL", "Outlook"},
    {"SUTHS", "South Slopes"},
    {"HOBEH", "Hove Beach"},
    {"FIREP", "Firefly Projects"},
    {"FIISL", "Firefly Island"},
    {"BEGGA", "Beachgate"},
    {"BRALG", "Algonquin Bridge"},
    {"BRBRO", "Broker Bridge"},
    {"BREBB", "East Borough Bridge"},
    {"BRDBB", "Dukes Bay Bridge"},
    {"NOWOB", "Northwood Heights Bridge"},
    {"HIBRG", "Hickey Bridge"},
    {"LEAPE", "Leaper's Bridge"},
    {"BOTU", "Booth Tunnel"},
    {"LIBERTY", "Liberty City"}
};
std::map<std::string, std::string> GTAZoneNamesDCReady{
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
    {"HAPIN", "hapin"},
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
    {"CHISL", "chisl"},
    {"COISL", "coisl"},
    {"STHBO", "bohan"},
    {"CHAPO", "bohan"},
    {"FORSI", "bohan"},
    {"BOULE", "bohan"},
    {"NRTGA", "bohan"},
    {"INSTI", "bohan"},
    {"LTBAY", "bohan"},
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
    {"BRALG", "bralg"},
    {"BRBRO", "brbro"},
    {"BREBB", "brebb"},
    {"BRDBB", "brdbb"},
    {"NOWOB", "nowob"},
    {"HIBRG", "hibrg"},
    {"LEAPE", "leape"},
    {"BOTU", "botu"},
    {"LIBERTY", "liberty"}
};
std::map<std::string, std::string> GTAZoneNamesLocations{
    {"WESDY", "Alderney"},
    {"LEFWO", "Alderney"},
    {"ALDCI", "Alderney"},
    {"BERCH", "Alderney"},
    {"NORMY", "Alderney"},
    {"ACTRR", "Alderney"},
    {"PORTU", "Alderney"},
    {"TUDOR", "Alderney"},
    {"ACTIP", "Alderney"},
    {"ALSCF", "Alderney"},
    {"HAPIN", "Happiness Island"},
    {"CASGR", "Algonquin"},
    {"THXCH", "Algonquin"},
    {"FISSO", "Algonquin"},
    {"CHITO", "Algonquin"},
    {"CITH", "Algonquin"},
    {"CASGC", "Algonquin"},
    {"SUFFO", "Algonquin"},
    {"LITAL", "Algonquin"},
    {"LOWEA", "Algonquin"},
    {"FISSN", "Algonquin"},
    {"THPRES", "Algonquin"},
    {"EASON", "Algonquin"},
    {"THTRI", "Algonquin"},
    {"TMEQU", "Algonquin"},
    {"WESMI", "Algonquin"},
    {"STARJ", "Algonquin"},
    {"LANCE", "Algonquin"},
    {"HATGA", "Algonquin"},
    {"PUGAT", "Algonquin"},
    {"MIDPW", "Algonquin"},
    {"MIDPA", "Algonquin"},
    {"MIDPE", "Algonquin"},
    {"LANCA", "Algonquin"},
    {"VASIH", "Algonquin"},
    {"NOHOL", "Algonquin"},
    {"EAHOL", "Algonquin"},
    {"NORWO", "Algonquin"},
    {"CHISL", "Charge Island"},
    {"COISL", "Colony Island"},
    {"STHBO", "Bohan"},
    {"CHAPO", "Bohan"},
    {"FORSI", "Bohan"},
    {"BOULE", "Bohan"},
    {"NRTGA", "Bohan"},
    {"INSTI", "Bohan"},
    {"LTBAY", "Bohan"},
    {"STEIN", "Broker/Dukes"},
    {"MEADP", "Broker/Dukes"},
    {"FRANI", "Broker/Dukes"},
    {"WILLI", "Broker/Dukes"},
    {"MEADH", "Broker/Dukes"},
    {"EISLC", "Broker/Dukes"},
    {"BOAB", "Broker/Dukes"},
    {"CERHE", "Broker/Dukes"},
    {"BEECW", "Broker/Dukes"},
    {"SCHOL", "Broker/Dukes"},
    {"DOWTW", "Broker/Dukes"},
    {"ROTTH", "Broker/Dukes"},
    {"ESHOO", "Broker/Dukes"},
    {"OUTL", "Broker/Dukes"},
    {"SUTHS", "Broker/Dukes"},
    {"HOBEH", "Broker/Dukes"},
    {"FIREP", "Broker/Dukes"},
    {"FIISL", "Broker/Dukes"},
    {"BEGGA", "Broker/Dukes"},
    {"BRALG", "Liberty City"},
    {"BRBRO", "Liberty City"},
    {"BREBB", "Liberty City"},
    {"BRDBB", "Liberty City"},
    {"NOWOB", "Liberty City"},
    {"HIBRG", "Liberty City"},
    {"LEAPE", "Liberty City"},
    {"BOTU", "Liberty City"},
    {"LIBERTY", "Liberty City"}
};
std::map<std::string, std::string> GTARadioStations{
    {"THE_VIBE", "The Vibe"},
    {"LIBERTY_ROCK", "Liberty Rock Radio"},
    {"JAZZ_NATION", "Jazz Nation Radio"},
    {"BOBBY_KONDERS", "Massive B"},
    {"SELFREALIZATION", "Self Realization"},
    {"K109_THE_STUDIO", "K109 The Studio"},
    {"VCFM", "Vice City FM"},
    {"WKTT", "WKTT"},
    {"HARDCORE", "Liberty City Hardcore"},
    {"CLASSICAL_AMBIENT", "The Journey"},
    {"FUSION_FM", "Fusion FM"},
    {"BEAT_95", "The Beat"},
    {"RAMJAMFM", "Ram Jam FM"},
    {"DANCE_ROCK", "Radio Broker"},
    {"VLADIVOSTOK", "Vladivostok FM"},
    {"PLR", "Public Liberty Radio"},
    {"SAN_JUAN_SOUNDS", "San Juan Sounds"},
    {"DANCE_MIX", "Electro Choc"},
    {"NY_CLASSICS", "The Classics"},
    {"AFRO_BEAT", "International Funk"},
    {"BABYLON", "Tuff Gong"},
    {"INDEPENDENT", "Inpedendence FM"},
    {"INTEGRITY", "Integrity 2.0"},
    {"NONE", "None"}
};
#pragma endregion

#pragma region Extern C
extern "C" {
    IVPresenceExport void SetCustomData(CustomRichPresence crp) {
        memset(&dCustomPresence, 0, sizeof(dCustomPresence));
        dCustomPresence = crp;
    }
    IVPresenceExport void ClearCustomData() {
        memset(&dCustomPresence, 0, sizeof(dCustomPresence));
    }
}
#pragma endregion

#pragma region Discord
// Initialization
inline void DiscordInit()
{
    try {
        DiscordEventHandlers handlers;
        memset(&handlers, 0, sizeof(handlers));
        Discord_Initialize(APPLICATION_ID, &handlers, 1, "12210");
        isDiscordReady = true;
    }
    catch (const std::exception&) { }
}
#pragma endregion

#pragma region Methods
inline void RPC_SetDefaultImageKeyAndText() {
    if (currentZoneForDC == "alderney") {
        dPresence.largeImageKey = "alderney";

        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "%s in %s", currentZone.c_str(), currentZoneLocation.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }
    else if (currentZoneForDC == "algonquin") {
        dPresence.largeImageKey = "algonquin";

        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "%s in %s", currentZone.c_str(), currentZoneLocation.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }
    else if (currentZoneForDC == "bohan") {
        dPresence.largeImageKey = "bohan";

        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "%s in %s", currentZone.c_str(), currentZoneLocation.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }
    else if (currentZoneForDC == "broker_dukes") {
        dPresence.largeImageKey = "broker_dukes";

        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "%s in %s", currentZone.c_str(), currentZoneLocation.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }
    else if (currentZoneForDC == "liberty") {
        switch (currentEpisode) {
        case 0: dPresence.largeImageKey = "gta_iv"; break;
        case 1: dPresence.largeImageKey = "gta_iv_tlad"; break;
        case 2: dPresence.largeImageKey = "gta_iv_tbogt"; break;
        }

        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "%s in %s", currentZone.c_str(), currentZoneLocation.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }
    else {
        dPresence.largeImageKey = currentZoneForDC.c_str();

        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "%s in %s", currentZone.c_str(), currentZoneLocation.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }

    if (Scripting::IS_INTERIOR_SCENE()) {
        snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Building in %s", currentZone.c_str());
        dPresence.largeImageText = currentLocationBuffer;
    }
}
inline void RPC_SetDefaultWalkingText() {
    if (Scripting::IS_INTERIOR_SCENE()) {
        dPresence.details = "In a Building";
    }
    else {
        if (isSwimming) {
            // Doesn't work for whatever reason
            char locationBuffer[256];
            snprintf(locationBuffer, sizeof(locationBuffer), "Swimming through %s", currentZone.c_str());
            dPresence.details = locationBuffer;
        }
        else {
            if (Scripting::IS_CHAR_IN_AREA_3D(playerPed, 1357.270, -241.905, 23.232, 1339.550, -259.176, 27.232, false)) { // Swingset (Not perfect yet)
                dPresence.details = "At swingset";
            }
            else {
                if (charSpeed <= 1.90) {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Walking through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
                else {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Running through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
            }
        }
    }
}
#pragma endregion

// Runs every frame while in-game
void ScriptLoop()
{
    if (!isDiscordReady) {
        DiscordInit();
        elapsedTime = std::time(nullptr);
    }

#pragma region GTA_Stuff
    player = Scripting::CONVERT_INT_TO_PLAYERINDEX(Scripting::GET_PLAYER_ID());
    Scripting::GET_PLAYER_CHAR(player, &playerPed);

    Scripting::STORE_WANTED_LEVEL(player, &wantedLevel);
    Scripting::GET_CHAR_COORDINATES(playerPed, &pX, &pY, &pZ);
    Scripting::GET_CHAR_HEALTH(playerPed, &health);
    Scripting::GET_CHAR_ARMOUR(playerPed, &armor);
    Scripting::GET_CHAR_SPEED(playerPed, &charSpeed);
    Scripting::GET_INTERIOR_FROM_CHAR(playerPed, &currentInteriorRaw);
    isSwimming = Scripting::IS_CHAR_SWIMMING(playerPed);

    currentEpisode = Scripting::GET_CURRENT_EPISODE();
    isDead = Scripting::IS_CHAR_DEAD(playerPed);
    isDoingAMission = Scripting::GET_MISSION_FLAG();
    isPauseMenuActive = Scripting::IS_PAUSE_MENU_ACTIVE();

    // Network Stuff
    isNetworkSession = Scripting::IS_NETWORK_SESSION();
    isNetworkSessionRunning = Scripting::NETWORK_IS_SESSION_STARTED();
    isNetworkGameRunning = Scripting::IS_NETWORK_GAME_RUNNING();
    mpGameMode = Scripting::NETWORK_GET_GAME_MODE();
    mpPlayerCount = Scripting::GET_NUMBER_OF_PLAYERS();
    mpMaxPlayerCount = Scripting::NETWORK_GET_MAX_SLOTS();
    mpTeam = Scripting::GET_PLAYER_TEAM(player);
    Scripting::STORE_SCORE(player, &mpScore);
#pragma endregion

    if (wantedLevel != 0) {
        tempTime++;
    }
    else {
        scriptInterval++;
        if (scriptInterval % 75 == 0) {
            scriptInterval = 0;
        }
        else {
            return;
        }
    }

    if (isDiscordReady) {
        char* asctimeTemp = std::asctime(std::localtime(&elapsedTime));

        // Get interior
        auto interiorPool = CPools::ms_pInteriorInstPool;
        CInteriorInst* currentInterior = (CInteriorInst*)interiorPool->GetAt(currentInteriorRaw);

        char nameHealthArmorBuffer[256];
        const char* rawZoneName = Scripting::GET_NAME_OF_ZONE(pX, pY, pZ);
        currentZone = GTAZoneNames.find(rawZoneName)->second;
        currentZoneForDC = GTAZoneNamesDCReady.find(rawZoneName)->second;
        currentZoneLocation = GTAZoneNamesLocations.find(rawZoneName)->second;

        // Get current radio station
        CRadioStation* radioStationRaw = CAERadioTrackManager::GetRadioStation(Scripting::GET_PLAYER_RADIO_STATION_INDEX());
        if (radioStationRaw != nullptr) currentRadioStation = GTARadioStations.find(radioStationRaw->m_sName)->second;

        // Discord Rich Presence
        memset(&dPresence, 0, sizeof(dPresence));
        dPresence.instance = 0;
        dPresence.startTimestamp = elapsedTime;

        // Set party size if is network session
        if (isNetworkSession) {
            dPresence.partyMax = mpMaxPlayerCount;
            dPresence.partySize = mpPlayerCount;
        }

#pragma region Large_Image_Stuff
        if (currentInterior != nullptr) {
            switch (currentInterior->m_nModelIndex) {
                case 4222: // Strip Club Bohan
                case 5071:
                case 4654:
                    dPresence.largeImageKey = "sc_bohan";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Strip Club in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5164: // Strip Club Alderney
                case 6013:
                case 5596:
                    dPresence.largeImageKey = "sc_alderney";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Strip Club in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 6422: // Burger Shot
                case 7126:
                case 6854:
                    dPresence.largeImageKey = "burger_shot";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Burger Shot in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 6423: // Cluckin Bell
                case 7127:
                case 6855:
                    dPresence.largeImageKey = "cluckin_bell";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Cluckin' Bell in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 4385: // Hospital
                case 5234:
                case 4817:
                    dPresence.largeImageKey = "hospital";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Hospital in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5585: // Memory Lanes (Bowling Alley)
                case 29291:
                case 27944:
                    dPresence.largeImageKey = "memory_lanes";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Memory Lanes in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 4221: // Steinway Beer Garden (Darts)
                case 5070:
                case 4653:
                    dPresence.largeImageKey = "steinway_beer_garden";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Steinway Beer Garden in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 4386: // Homebrew Cafe (Billiard)
                case 5235:
                case 4818:
                    dPresence.largeImageKey = "homebrew_cafe";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Homebrew Cafe in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5067: // Perestroika (Cabaret)
                case 5916:
                case 5499:
                    dPresence.largeImageKey = "cabaret";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Perestroika (Cabaret) in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 6144: // Internet Cafe (tw@)
                case 6848:
                case 6576:
                    dPresence.largeImageKey = "internet_cafe";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "tw@ in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 6262: // 69th Street Diner
                    dPresence.largeImageKey = "69th_street_diner";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "69th Street Diner in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 6821: // Gun Shop
                case 6822:
                case 6118:
                case 6117:
                case 6550:
                case 6549:
                    dPresence.largeImageKey = "gun_shop";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Gun Shop in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;

                case 4139: // GTA IV Broker Safehouse
                    dPresence.largeImageKey = "gtaiv_broker_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Safehouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5681: // GTA IV South Bohan Apartment
                    dPresence.largeImageKey = "gtaiv_south_bohan_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Apartment in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5684: // GTA IV Middle Park East Safehouse
                    dPresence.largeImageKey = "gtaiv_middle_park_east_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Safehouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5678: // GTA IV Alderney Safehouse
                    dPresence.largeImageKey = "gtaiv_alderney_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Safehouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5919: // Playboy X's Penthouse
                case 6768:
                case 6351:
                    dPresence.largeImageKey = "pbx_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Playboy X's Penthouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 6426: // Bohan Sprunk Warehouse
                    dPresence.largeImageKey = "bohan_sprunk_warehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Bohan's Sprunk Warehouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 5584: // Abandoned Sprunk Factory
                    dPresence.largeImageKey = "abandoned_sprunk_factory";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Abandoned Sprunk Factory in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;

                case 27827: // TLaD Clubhouse
                    dPresence.largeImageKey = "tlad_the_lost_clubhouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Clubhouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 28024: // TLaD Brian's Safehouse
                    dPresence.largeImageKey = "tlad_brians_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Safehouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 29316: // TBoGT Luis Safehouse
                    dPresence.largeImageKey = "tbogt_luis_safehouse";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Safehouse in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;

                case 28931: // TBoGT Maisonette 9
                    dPresence.largeImageKey = "maisonette";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Maisonette in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 29026: // TBoGT Hercules
                    dPresence.largeImageKey = "hercules";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Hercules in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;
                case 28578: // TBoGT Bahama Mamas
                    dPresence.largeImageKey = "bahama_mamas";

                    snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Bahama Mamas in %s", currentZoneLocation.c_str());
                    dPresence.largeImageText = currentLocationBuffer;
                    break;

                default: 
                    RPC_SetDefaultImageKeyAndText();
                    break;
            }
        }
        else {
            if (Scripting::IS_CHAR_IN_AREA_3D(playerPed, 1357.270, -241.905, 23.232, 1339.550, -259.176, 27.232, false)) { // Swingset (Not perfect yet)
                dPresence.largeImageKey = "swingset";

                snprintf(currentLocationBuffer, sizeof(currentLocationBuffer), "Swingset in %s", currentZoneLocation.c_str());
                dPresence.largeImageText = currentLocationBuffer;
            }
            else {
                RPC_SetDefaultImageKeyAndText();
            }
        }
#pragma endregion

#pragma region Small_Image_Stuff
        if (isNetworkSession) { // Multiplayer
            if (!isDead) {
                dPresence.smallImageKey = "mp_player";
            }
            else {
                dPresence.smallImageKey = "mp_player_red";
            }

            switch (mpGameMode) {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 10:
                case 12:
                case 15:
                case 26:
                case 27:
                    snprintf(nameHealthArmorBuffer, sizeof(nameHealthArmorBuffer), "%s - Health: %s, Armor: %s, Score: %s", Scripting::GET_PLAYER_NAME(player), std::to_string(health).c_str(), std::to_string(armor).c_str(), std::to_string(mpScore).c_str());
                    dPresence.smallImageText = nameHealthArmorBuffer;
                    break;
                default:
                    snprintf(nameHealthArmorBuffer, sizeof(nameHealthArmorBuffer), "%s - Health: %s, Armor: %s", Scripting::GET_PLAYER_NAME(player), std::to_string(health).c_str(), std::to_string(armor).c_str());
                    dPresence.smallImageText = nameHealthArmorBuffer;
                    break;
            }
        }
        else { // Singleplayer
            switch (currentEpisode) {
                case 0: // GTA IV
                    if (!isDead) {
                        if (wantedLevel > 0) {
                            if (tempTime <= 35) {
                                dPresence.smallImageKey = "niko_icon_red";
                            }
                            if (tempTime >= 35) {
                                if (tempTime >= 55) {
                                    tempTime = 0;
                                }
                                dPresence.smallImageKey = "niko_icon_blue";
                            }
                        }
                        else {
                            dPresence.smallImageKey = "niko_icon";
                        }
                    }
                    else {
                        dPresence.smallImageKey = "niko_icon_red";
                    }

                    snprintf(nameHealthArmorBuffer, sizeof(nameHealthArmorBuffer), "%s - Health: %s, Armor: %s", "Niko Bellic", std::to_string(health).c_str(), std::to_string(armor).c_str());
                    dPresence.smallImageText = nameHealthArmorBuffer;
                    break;
                case 1: // TLAD
                    if (!isDead) {
                        if (wantedLevel > 0) {
                            if (tempTime <= 35) {
                                dPresence.smallImageKey = "johnny_icon_red";
                            }
                            if (tempTime >= 35) {
                                if (tempTime >= 55) {
                                    tempTime = 0;
                                }
                                dPresence.smallImageKey = "johnny_icon_blue";
                            }
                        }
                        else {
                            dPresence.smallImageKey = "johnny_icon";
                        }
                    }
                    else {
                        dPresence.smallImageKey = "johnny_icon_red";
                    }

                    snprintf(nameHealthArmorBuffer, sizeof(nameHealthArmorBuffer), "%s - Health: %s, Armor: %s", "Jonathan Klebitz", std::to_string(health).c_str(), std::to_string(armor).c_str());
                    dPresence.smallImageText = nameHealthArmorBuffer;
                    break;
                case 2: // TBOGT
                    if (!isDead) {
                        if (wantedLevel > 0) {
                            if (tempTime % 50 == 0) {
                                if (tempTime <= 35) {
                                    dPresence.smallImageKey = "luis_icon_red";
                                }
                                if (tempTime >= 35) {
                                    if (tempTime >= 55) {
                                        tempTime = 0;
                                    }
                                    dPresence.smallImageKey = "luis_icon_blue";
                                }
                            }
                        }
                        else {
                            dPresence.smallImageKey = "luis_icon";
                        }
                    }
                    else {
                        dPresence.smallImageKey = "luis_icon_red";
                    }

                    snprintf(nameHealthArmorBuffer, sizeof(nameHealthArmorBuffer), "%s - Health: %s, Armor: %s", "Luis Fernando Lopez", std::to_string(health).c_str(), std::to_string(armor).c_str());
                    dPresence.smallImageText = nameHealthArmorBuffer;
                    break;
            }
        }
#pragma endregion

#pragma region Details_And_State

        if (isPauseMenuActive) goto DC_UPDATE_RICH_PRESENCE;

        if (isDead) {
            if (isNetworkSession) { // Multiplayer
                dPresence.details = "Waiting to be respawned";
            }
            else { // Singleplayer
                dPresence.details = "Waiting to be respawned at hospital";
            }
        }
        else {
            if (Scripting::IS_CHAR_IN_ANY_CAR(playerPed)) {
                Scripting::Vehicle currentVehicle;
                Scripting::GET_CAR_CHAR_IS_USING(playerPed, &currentVehicle);

                unsigned int vehicleModel;
                Scripting::GET_CAR_MODEL(currentVehicle, &vehicleModel);

                if (Scripting::IS_THIS_MODEL_A_BIKE(vehicleModel)) {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Motorcycling through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
                else if (Scripting::IS_THIS_MODEL_A_BOAT(vehicleModel)) {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Boating through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
                else if (Scripting::IS_THIS_MODEL_A_CAR(vehicleModel)) {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Driving through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
                else if (Scripting::IS_THIS_MODEL_A_HELI(vehicleModel) || Scripting::IS_THIS_MODEL_A_PLANE(vehicleModel)) {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Flying over %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
                else if (Scripting::IS_THIS_MODEL_A_TRAIN(vehicleModel)) {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Taking the train through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
                else {
                    RPC_SetDefaultWalkingText();
                }

                if (radioStationRaw != nullptr) {
                    char radioBuffer[256];
                    snprintf(radioBuffer, sizeof(radioBuffer), "Listening to %s", currentRadioStation.c_str());
                    dPresence.state = radioBuffer;
                }
            }
            else {
                if (currentInterior != nullptr) {
                    switch (currentInterior->m_nModelIndex) {
                        case 4222: // Strip Club Bohan
                        case 5071:
                        case 4654:
                        case 5164: // Strip Club Alderney
                        case 6013:
                        case 5596:
                            dPresence.details = "Enjoys his time in the Strip Club";
                            break;
                        case 6422: // Burger Shot
                        case 7126:
                        case 6854:
                            dPresence.details = "In Burger Shot";
                            break;
                        case 6423: // Cluckin Bell
                        case 7127:
                        case 6855:
                            dPresence.details = "In Cluckin' Bell";
                            break;
                        case 4385: // Hospital
                        case 5234:
                        case 4817:
                            dPresence.details = "In Hospital";
                            break;
                        case 5585: // Memory Lanes (Bowling Alley)
                        case 29291:
                        case 27944:
                            dPresence.details = "In Bowling Alley";
                            break;
                        case 4221: // Steinway Beer Garden (Darts)
                        case 5070:
                        case 4653:
                            dPresence.details = "In Steinway's Beer Garden";
                            break;
                        case 4386: // Homebrew Cafe (Billiard)
                        case 5235:
                        case 4818:
                            dPresence.details = "In Homebrew's Cafe";
                            break;
                        case 5067: // Perestroika (Cabaret)
                        case 5916:
                        case 5499:
                            dPresence.details = "In Perestroika (Cabaret)";
                            break;
                        case 6144: // Internet Cafe (tw@)
                        case 6848:
                        case 6576:
                            dPresence.details = "In tw@";
                            break;
                        case 6262: // 69th Street Diner
                            dPresence.details = "In 69th Street Diner";
                            break;
                        case 6821: // Gun Shop
                        case 6822:
                        case 6118:
                        case 6117:
                        case 6550:
                        case 6549:
                            dPresence.details = "In Gun Shop";
                            break;

                        case 4139: // GTA IV Broker Safehouse
                            dPresence.details = "In Broker Safehouse";
                            break;
                        case 5681: // GTA IV South Bohan Apartment
                            dPresence.details = "In South Bohan Apartment";
                            break;
                        case 5684: // GTA IV Middle Park East Safehouse
                            dPresence.details = "In Middle Park East Safehouse";
                            break;
                        case 5678: // GTA IV Alderney Safehouse
                            dPresence.details = "In Alderney Safehouse";
                            break;
                        case 5919: // Playboy X's Penthouse
                        case 6768:
                        case 6351:
                            dPresence.details = "In Playboy X's Penthouse";
                            break;
                        case 6426: // Bohan Sprunk Warehouse
                            dPresence.details = "In Bohan's Sprunk Warehouse";
                            break;
                        case 5584: // Abandoned Sprunk Factory
                            dPresence.details = "In Abandoned Sprunk Factory";
                            break;

                        case 27827: // TLaD Clubhouse
                            dPresence.details = "In Lost MC Clubhouse";
                            break;
                        case 28024: // TLaD Brian's Safehouse
                            dPresence.details = "In Brian's Safehouse";
                            break;
                        case 29316: // TBoGT Luis Safehouse
                            dPresence.details = "In Luis Safehouse";
                            break;

                        case 28931: // TBoGT Maisonette 9
                            dPresence.details = "Hanging out in Maisonette 9";
                            break;
                        case 29026: // TBoGT Hercules
                            dPresence.details = "Hanging out in Hercules";
                            break;
                        case 28578: // TBoGT Bahama Mamas
                            dPresence.details = "Hanging out in Bahama Mamas";
                            break;
                        default:
                            RPC_SetDefaultWalkingText();
                            break;
                    }
                }
                else {
                    RPC_SetDefaultWalkingText();
                }
            }

            // State
            if (isNetworkSession) { // Multiplayer
                switch (mpGameMode) {
                    case 0:
                    case 24:
                    case 26:
                        dPresence.state = "Multiplayer: Deathmatch";
                        break;
                    case 1:
                    case 21:
                    case 27:
                        dPresence.state = "Multiplayer: Team Deathmatch";
                        break;
                    case 2:
                        dPresence.state = "Multiplayer: Mafiya Work";
                        break;
                    case 3:
                        dPresence.state = "Multiplayer: Team Mafiya Work";
                        break;
                    case 4:
                        dPresence.state = "Multiplayer: Team Car Jack City";
                        break;
                    case 5:
                        dPresence.state = "Multiplayer: Car Jack City";
                        break;
                    case 6:
                    case 20:
                    case 28:
                        dPresence.state = "Multiplayer: Race";
                        break;
                    case 7:
                    case 29:
                        dPresence.state = "Multiplayer: GTA Race";
                        break;
                    case 8:
                        dPresence.state = "Multiplayer: Party Mode";
                        break;
                    case 10:
                        dPresence.state = "Multiplayer: Cops And Crooks";
                        break;
                    case 12:
                        dPresence.state = "Multiplayer: Turf War";
                        break;
                    case 13:
                        dPresence.state = "Multiplayer: Deal Breaker";
                        break;
                    case 14:
                        dPresence.state = "Multiplayer: Hangmans Noose";
                        break;
                    case 15:
                        dPresence.state = "Multiplayer: Bomb Da Base II";
                        break;
                    case 16:
                        dPresence.state = "Multiplayer: Free Mode";
                        break;

                        // TLAD
                    case 17:
                        dPresence.state = "Multiplayer: Chopper vs Chopper";
                        break;
                    case 18:
                        dPresence.state = "Multiplayer: Witness Protection";
                        break;
                    case 19:
                        dPresence.state = "Multiplayer: Club Business";
                        break;
                    case 22:
                        dPresence.state = "Multiplayer: Own The City";
                        break;
                    case 23:
                        dPresence.state = "Multiplayer: Lone Wolf Biker";
                        break;

                    default:
                        dPresence.state = "Multiplayer: Mysterious mode";
                        break;
                }
            }
            else { // Singleplayer
                if (!Scripting::ARE_CREDITS_FINISHED()) {
                    dPresence.details = "Is watching end credits";
                }
                else {
                    if (wantedLevel != 0) {
                        if (Scripting::PLAYER_HAS_FLASHING_STARS_ABOUT_TO_DROP(player)) {
                            if (wantedLevel > 1) {
                                char wantedLevelBuffer[256];
                                snprintf(wantedLevelBuffer, sizeof(wantedLevelBuffer), "Losing %s stars", std::to_string(wantedLevel).c_str());
                                dPresence.state = wantedLevelBuffer;
                            }
                            else {
                                char wantedLevelBuffer[256];
                                snprintf(wantedLevelBuffer, sizeof(wantedLevelBuffer), "Losing %s star", std::to_string(wantedLevel).c_str());
                                dPresence.state = wantedLevelBuffer;
                            }
                        }
                        else {
                            if (Scripting::PLAYER_HAS_GREYED_OUT_STARS(player)) {
                                if (wantedLevel > 1) {
                                    char wantedLevelBuffer[256];
                                    snprintf(wantedLevelBuffer, sizeof(wantedLevelBuffer), "Hiding from police with %s stars", std::to_string(wantedLevel).c_str());
                                    dPresence.state = wantedLevelBuffer;
                                }
                                else {
                                    char wantedLevelBuffer[256];
                                    snprintf(wantedLevelBuffer, sizeof(wantedLevelBuffer), "Hiding from police with %s star", std::to_string(wantedLevel).c_str());
                                    dPresence.state = wantedLevelBuffer;
                                }
                            }
                            else {
                                if (wantedLevel > 1) {
                                    char wantedLevelBuffer[256];
                                    snprintf(wantedLevelBuffer, sizeof(wantedLevelBuffer), "Wanted by police with %s stars", std::to_string(wantedLevel).c_str());
                                    dPresence.state = wantedLevelBuffer;
                                }
                                else {
                                    char wantedLevelBuffer[256];
                                    snprintf(wantedLevelBuffer, sizeof(wantedLevelBuffer), "Wanted by police with %s star", std::to_string(wantedLevel).c_str());
                                    dPresence.state = wantedLevelBuffer;
                                }
                            }
                        }
                    }
                    else {
                        if (isDoingAMission) {
                            dPresence.state = "Currently on a mission";
                        }
                        else {
                            if (radioStationRaw == nullptr) {
                                if (Scripting::IS_CHAR_IN_ANY_CAR(playerPed)) {
                                    Scripting::Vehicle currentVehicle;
                                    Scripting::GET_CAR_CHAR_IS_USING(playerPed, &currentVehicle);

                                    unsigned int vehicleModel;
                                    Scripting::GET_CAR_MODEL(currentVehicle, &vehicleModel);

                                    if (!Scripting::IS_THIS_MODEL_A_TRAIN(vehicleModel)) {

                                        const char* vehicleName = Scripting::GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(vehicleModel);

                                        if (Scripting::DOES_TEXT_LABEL_EXIST(vehicleName)) {
                                            vehicleName = Scripting::GET_STRING_FROM_TEXT_FILE(vehicleName);
                                        }

                                        float carSpeed;
                                        Scripting::GET_CAR_SPEED(currentVehicle, &carSpeed);
                                        carSpeed = carSpeed / 0.27777;

                                        char vehicleInfosBuffer[256];
                                        snprintf(vehicleInfosBuffer, sizeof(vehicleInfosBuffer), "With a %s at %.0f km/h", vehicleName, carSpeed);
                                        dPresence.state = vehicleInfosBuffer;

                                        if (Scripting::IS_THIS_MODEL_A_BIKE(vehicleModel)) {
                                            if (Scripting::IS_PLAYER_PERFORMING_WHEELIE(player)) {
                                                dPresence.state = "Performing a wheelie!";
                                            }
                                            if (Scripting::IS_PLAYER_PERFORMING_STOPPIE(player)) {
                                                dPresence.state = "Performing a stoppie!";
                                            }
                                        }
                                    }
                                }
                                else {
                                    // TODO: Make functional
                                    //if (Scripting::IS_CHAR_PLAYING_ANIM(playerPed, "amb@burgercart", "buy_burger_plyr")) { // Player buys burger
                                    //    dPresence.state = "Is buying a burger";
                                    //}
                                    //else if (Scripting::IS_CHAR_PLAYING_ANIM(playerPed, "amb@burgercart", "eat_burger_plyr")) { // Player eats burger
                                    //    dPresence.state = "Is eating a burger";
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }
#pragma endregion

DC_UPDATE_RICH_PRESENCE:
        if (dCustomPresence.isAvailable) { // If custom data is available then set custom data
            if (isPauseMenuActive) {
                dPresence.details = "In pause menu";
                dPresence.state = "";
            }
            else {
                dPresence.partySize = dCustomPresence.partySize;
                dPresence.partyMax = dCustomPresence.partyMax;

                if (dCustomPresence.details != "") dPresence.details = dCustomPresence.details;
                if (dCustomPresence.state != "") dPresence.state = dCustomPresence.state;
                if (dCustomPresence.largeImageText != "") dPresence.largeImageText = dCustomPresence.largeImageText;
                if (dCustomPresence.largeImageKey != "") dPresence.largeImageKey = dCustomPresence.largeImageKey;
            }

            Discord_UpdatePresence(&dPresence);
        }
        else { // Set default data
            if (isPauseMenuActive) dPresence.details = "In pause menu";
            Discord_UpdatePresence(&dPresence);
        }


#ifdef DISCORD_DISABLE_IO_THREAD
        Discord_UpdateConnection();
#endif
        Discord_RunCallbacks();

    }
}

// Basically just DllMain but fancier and with the SDK initialized
void plugin::gameStartupEvent()
{
    plugin::processScriptsEvent::Add(ScriptLoop);
}

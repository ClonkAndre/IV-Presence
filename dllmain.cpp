// IV-Presence by ItsClonkAndre
// Version 1.0

#include "IVSDK.cpp"
#include "discord_rpc.h"
#include <map>
#include <ctime>

#pragma region  Variables
static const char* APPLICATION_ID = "888450942394056704";

DiscordRichPresence dPresence;

bool isDiscordReady, isDead, isDoingAMission;
float pX, pY, pZ;
unsigned int tempTime, scriptInterval, currentEpisode, wantedLevel, health, armor;

Scripting::Interior currentInteriorRaw;
std::time_t elapsedTime;

char currentLocationBuffer[256];
std::string currentZone, currentZoneForDC, currentZoneLocation, currentRadioStation;

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
std::map<int, std::string> GTARadioStations{
    {0, "The Vibe"},
    {1, "Liberty Rock Radio"},
    {2, "Jazz Nation Radio"},
    {3, "Massive B"},
    {4, "K109 The Studio"},
    {5, "WKTT"},
    {6, "Liberty City Hardcore"},
    {7, "The Journey"},
    {8, "Fusion FM"},
    {9, "The Beat"},
    {10, "Radio Broker"},
    {11, "Vladivostok FM"},
    {12, "Public Liberty Radio"},
    {13, "San Juan Sounds"},
    {14, "Electro Choc"},
    {15, "The Classics"},
    {16, "International Funk"},
    {17, "Tuff Gong"},
    {18, "Inpedendence FM"},
    {19, "Integrity 2.0"},
    {254, "None"},
    {255, "None"}
};
#pragma endregion

#pragma region Discord
static void DiscordInit()
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
}
#pragma endregion

// Runs every frame while in-game
void plugin::processScriptsEvent()
{
    if (!isDiscordReady) {
        DiscordInit();
        elapsedTime = std::time(nullptr);
    }

    Scripting::Player player = Scripting::CONVERT_INT_TO_PLAYERINDEX(Scripting::GET_PLAYER_ID());
    Scripting::Ped playerPed;
    Scripting::GET_PLAYER_CHAR(player, &playerPed);

    Scripting::STORE_WANTED_LEVEL(player, &wantedLevel);
    Scripting::GET_CHAR_COORDINATES(playerPed, &pX, &pY, &pZ);
    Scripting::GET_CHAR_HEALTH(playerPed, &health);
    Scripting::GET_CHAR_ARMOUR(playerPed, &armor);
    Scripting::GET_INTERIOR_FROM_CHAR(playerPed, &currentInteriorRaw);

    currentEpisode = Scripting::GET_CURRENT_EPISODE();
    isDead = Scripting::IS_CHAR_DEAD(playerPed);
    isDoingAMission = Scripting::GET_MISSION_FLAG();

    if (wantedLevel != 0) {
        tempTime++;
    }
    else {
        scriptInterval++;
        if (scriptInterval % 50 == 0) {
            scriptInterval = 0;
        }
        else {
            return;
        }
    }

    if (isDiscordReady) {
        Discord_RunCallbacks();

        std::asctime(std::localtime(&elapsedTime));

        auto interiorPool = CPools::ms_pInteriorInstPool;
        CInteriorInst* currentInterior = (CInteriorInst*)interiorPool->GetAt(currentInteriorRaw);

        char nameHealthArmorBuffer[256];
        char* rawZoneName = Scripting::GET_NAME_OF_ZONE(pX, pY, pZ);
        currentZone = GTAZoneNames.find(rawZoneName)->second;
        currentZoneForDC = GTAZoneNamesDCReady.find(rawZoneName)->second;
        currentZoneLocation = GTAZoneNamesLocations.find(rawZoneName)->second;
        currentRadioStation = GTARadioStations.find(Scripting::GET_PLAYER_RADIO_STATION_INDEX())->second;

        // Discord Rich Presence
        memset(&dPresence, 0, sizeof(dPresence));
        dPresence.instance = 0;
        dPresence.startTimestamp = elapsedTime;

#pragma region Image Stuff
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
            RPC_SetDefaultImageKeyAndText();
        }

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
#pragma endregion

#pragma region Details

        if (Scripting::IS_PAUSE_MENU_ACTIVE()) {
            dPresence.details = "In pause menu";
            goto DC_UPDATE_RICH_PRESENCE;
        }

        if (!isDead) {
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
                else if (Scripting::IS_THIS_MODEL_A_HELI(vehicleModel)) {
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
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Walking through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }

                if (currentRadioStation != "None") {
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
                            char locationBuffer[256];
                            snprintf(locationBuffer, sizeof(locationBuffer), "Walking through %s", currentZone.c_str());
                            dPresence.details = locationBuffer;
                            break;
                    }
                }
                else {
                    char locationBuffer[256];
                    snprintf(locationBuffer, sizeof(locationBuffer), "Walking through %s", currentZone.c_str());
                    dPresence.details = locationBuffer;
                }
            }

            // State
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
                    if (currentRadioStation == "None") {
                        if (Scripting::IS_CHAR_IN_ANY_CAR(playerPed)) {
                            Scripting::Vehicle currentVehicle;
                            Scripting::GET_CAR_CHAR_IS_USING(playerPed, &currentVehicle);

                            unsigned int vehicleModel;
                            Scripting::GET_CAR_MODEL(currentVehicle, &vehicleModel);

                            if (!Scripting::IS_THIS_MODEL_A_TRAIN(vehicleModel)) {
                                const char* carName = Scripting::GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(vehicleModel);

                                float carSpeed;
                                Scripting::GET_CAR_SPEED(currentVehicle, &carSpeed);
                                carSpeed = carSpeed / 0.27777;

                                char vehicleInfosBuffer[256];
                                snprintf(vehicleInfosBuffer, sizeof(vehicleInfosBuffer), "With a %s at %.0f km/h", carName, carSpeed);
                                dPresence.state = vehicleInfosBuffer;
                            }
                        }
                    }
                }
            }
        }
        else {
            dPresence.details = "Waiting to be respawned at hospital";
        }
#pragma endregion

DC_UPDATE_RICH_PRESENCE:
        Discord_UpdatePresence(&dPresence);
    }
}

// Basically just DllMain but fancier and with the SDK initialized
void plugin::gameStartupEvent()
{

}

// Right after gta.dat loads, put any extra loading related things here
void plugin::gameLoadEvent()
{

}

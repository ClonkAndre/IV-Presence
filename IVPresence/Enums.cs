namespace IVPresence
{

    internal enum TextState
    {
        Default = 0,
        State1 = 1,
        State2 = 2,
        State3 = 3,
        State4 = 4,
        State5 = 5,
        State6 = 6,
        State7 = 7,
        State8 = 8,
        State9 = 9,
        State10 = 10,
        MAX
    }

    internal enum CauseOfDeath
    {
        Unknown,
        FallingDamage,
        FleeingFromCops,
        FightingCops,
        Headshot,
        Fire,
        Explosion,
        ExplosionNoobtubed,
        ExplodingCar,
        ExplodingBike,
        ExplodingTruck,
        ExplodingPetrolPump,
    }

    internal enum GasStation
    {
        None,
        Ron,
        GlobeOil,
        Terroil
    }

}

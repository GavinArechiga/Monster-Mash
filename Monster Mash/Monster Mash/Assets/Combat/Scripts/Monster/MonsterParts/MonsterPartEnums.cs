public enum MonsterPartLimb
{
    None,
    Torso,
    Head,
    Arm,
    Leg,
    Eye,
    Mouth,
    Wing,
    Tail,
    Decor
}

public enum MonsterPartAnimTrigger
{
    n_Attk,
    h_Attk,
    release,
    jump,
    land,
    hit,
    brace,
    unbrace,
}

public enum MonsterPartAnimBool
{
    isWalking,
    isRunning
}

public enum MonsterPartAnimFloat
{
    torsoAttkDirX,
    torsoAttkDirY,
    legWalkOffset,
    legRunOffset,
    legBlendDir
}

public enum MonsterPartAnimStateTags
{
    attack,
    hit
}
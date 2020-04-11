//r- right
//L-LEFT
//D-DOWN
//U-UP

public enum RoadTypeList
{
    NO_CONNECTION,

    //for one direction
    R_ONLY_CONNECTION,
    L_ONLY_CONNECTION,
    U_ONLY_CONNECTION,
    D_ONLY_CONNECTION,

    //goes to 2 different direction
    R_L_ONLY_CONNECTION,
    U_D_ONLY_CONNECTION,


    RLUP_ONLY_CONNECTION,
}

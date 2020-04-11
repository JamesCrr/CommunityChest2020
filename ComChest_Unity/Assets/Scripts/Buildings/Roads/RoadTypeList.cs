//r- right
//L-LEFT
//D-DOWN
//U-UP
//DIA - diagonal

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
    U_R_ONLY_CONNECTION,
    U_L_ONLY_CONNECTION,
    D_R_ONLY_CONNECTION,
    D_L_ONLY_CONNECTION,

    //diagonal of it theres a block, remove the corners in betweem
    U_R_DIA_ONLY_CONNECTION,
    U_L_DIA_ONLY_CONNECTION,
    D_R_DIA_ONLY_CONNECTION,
    D_L_DIA_ONLY_CONNECTION,

    //goes to 3 different direction
    U_D_L_ONLY_CONNECTION,
    U_D_R_ONLY_CONNECTION,
    U_R_L_ONLY_CONNECTION,
    D_R_L_ONLY_CONNECTION,



    //goes to 4 different direction


    RLUP_ONLY_CONNECTION,
}

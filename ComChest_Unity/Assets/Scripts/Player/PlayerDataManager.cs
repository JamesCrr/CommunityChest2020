
public class PlayerDataManager : SingletonBase<PlayerDataManager>
{
    int m_TotalMoney = 0;
    int m_TotalPopulation = 0;
    int m_TotalHappiness = 0;
    int m_TotalElectricity = 0;
    int m_TotalWater = 0;

    int m_RequiredWater = 0;
    int m_RequiredElectricity = 0;
    

    // Start is called before the first frame update
    public override void Awake()
    {
        //TODO init from save file
    }

    public int GetSetTotalMoney
    {
        get { return m_TotalMoney; }
        set { m_TotalMoney = value; }
    }

    public int GetSetTotalPopulation
    {
        get { return m_TotalPopulation; }
        set { m_TotalPopulation = value; }
    }

    public int GetSetTotalHappiness
    {
        get { return m_TotalHappiness; }
        set { m_TotalHappiness = value; }
    }

    public int GetSetTotalElectricity
    {
        get { return m_TotalElectricity; }
        set { m_TotalElectricity = value; }
    }

    public int GetSetTotalWater
    {
        get { return m_TotalWater; }
        set { m_TotalWater = value; }
    }

}

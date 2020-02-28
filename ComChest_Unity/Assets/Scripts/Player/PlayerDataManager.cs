
public class PlayerDataManager : SingletonBase<PlayerDataManager>
{
    int m_TotalMoney = 0;
    int m_TotalPopulation = 0;

    // Start is called before the first frame update
    public override void Awake()
    {
        //TODO init from save file
    }
}

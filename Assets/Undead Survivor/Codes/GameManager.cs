using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 정적 변수는 유니티에서 인스펙터 창에 노출되지 않는다. 따라서 드래그앤드롭으로 할당할 수 없다.
    public static GameManager instance;

    // 유니티에서 이 변수에 오브젝트를 드래그앤드롭 하면, PoolManager 컴포넌트가 있는지 확인하고 있다면 그 컴포넌트의 참조가 pool 변수에 저장되는 방식임. 이 방식은 Awake 호출 전에 Unity에서 수행함.
    public float gameTime;
    public float maxGameTime = 2*10f;
    
    public PoolManager pool;
    public Player player;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        if(gameTime>maxGameTime)
        {
            gameTime=maxGameTime;
        }
    }
}

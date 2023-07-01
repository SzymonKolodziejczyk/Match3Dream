using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;

    public GameObject prefab;

    private ProjectilePool pool;

    private MatchablePool pool2;

    private void Start()
    {
        Instantiate(prefab);

        player = Player.Instance;

        print(player.health);

        pool = (ProjectilePool) ProjectilePool.Instance;

        pool.PoolObjects(4);
        
        StartCoroutine(ProjectilePoolTest());

        pool2 = (MatchablePool) MatchablePool.Instance;

        pool2.PoolObjects(10);

        StartCoroutine(MatchablePoolTest());
    }

    private IEnumerator ProjectilePoolTest()
    {
        List<Projectile> projectileList = new List<Projectile>();
        Projectile projectile;

        for(int i = 0; i != 7; ++i)
        {
            projectile = pool.GetPooledObject();
            projectileList.Add(projectile);
            projectile.Randomize();
            projectile.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
        }
        for (int i = 0; i != 4; ++i)
        {
            pool.ReturnObjectToPool(projectileList[i]);

            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator MatchablePoolTest()
    {
        Matchable m = pool2.GetPooledObject();

        m.gameObject.SetActive(true);

        Vector3 randomPosition;

        for(int i = 0; i != 7; ++i)
        {
            randomPosition = new Vector3(Random.Range(-6f, 6f), Random.Range(-4f, 4f));
            yield return StartCoroutine(m.MoveToPosition(randomPosition));
        }
        yield return null;
    }
}

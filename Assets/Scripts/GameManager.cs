using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;

    public GameObject prefab;

    private ProjectilePool pool;

    private void Start()
    {
        Instantiate(prefab);

        player = Player.Instance;

        print(player.health);

        pool = (ProjectilePool) ProjectilePool.Instance;

        pool.PoolObjects(4);
        
        StartCoroutine(Demo());
    }

    private IEnumerator Demo()
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
}

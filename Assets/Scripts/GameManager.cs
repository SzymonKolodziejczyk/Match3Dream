using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    [SerializeField] private Vector2Int dimensions;
    [SerializeField] private Text gridOutput;

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;

        pool.PoolObjects(10);

        grid.InitializeGrid(dimensions);

        StartCoroutine(Demo());
    }
    private IEnumerator Demo()
    {
        gridOutput.text = grid.ToString();
        yield return new WaitForSeconds(2);

        Matchable m1 = pool.GetPooledObject();
        m1.gameObject.SetActive(true);
        m1.gameObject.name = "a";

        Matchable m2 = pool.GetPooledObject();
        m2.gameObject.SetActive(true);
        m2.gameObject.name = "b";

        grid.PutItemAt(m1,0,1);
        grid.PutItemAt(m2,2,3);

        gridOutput.text = grid.ToString();
        yield return new WaitForSeconds(2);

        grid.SwapItemsAt(0,1,2,3);
        gridOutput.text = grid.ToString();
        yield return new WaitForSeconds(2);

        grid.RemoveItemAt(0,1);
        grid.RemoveItemAt(2,3);
        gridOutput.text = grid.ToString();
        yield return new WaitForSeconds(2);

        pool.ReturnObjectToPool(m1);
        pool.ReturnObjectToPool(m2);
    }
}

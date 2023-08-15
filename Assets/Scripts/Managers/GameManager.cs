using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
*
* This class will set up the scene and initialize objects
*
* This class inherits from Singleton so any other script can access it easily through GameManager.Instance
*
*/

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    // the dimension of the matchable grid, set in the inspector
    [SerializeField] private Vector2Int dimensions = Vector2Int.one;

    // a UI Text object for displaying the contents of the grid data
    // for testing and debugging purposes only
    [SerializeField] private Text gridOutput;

    private void Start()
    {
        // get references to other important game objects
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;

        StartCoroutine(Setup());
    }
    private IEnumerator Setup()
    {
        // Put loading screen here in the future


        // pool the matchables
        pool.PoolObjects(dimensions.x * dimensions.y * 2);

        // create the grid
        grid.InitializeGrid(dimensions);

        yield return null;

        StartCoroutine(grid.PopulateGrid());

        /*

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
        */
    }
}

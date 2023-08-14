using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{

    private MatchablePool pool;
    [SerializeField] private Vector3 offscreenOffset;

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
    }

    public IEnumerator PopulateGrid(bool allowMatches = false)
    {
        Matchable newMatchable;
        Vector3 onscreenPosition;

        for(int y = 0; y != Dimensions.y; ++y)
            for(int x = 0; x != Dimensions.x; ++x)
            {
                // get a matchable from the pool
                newMatchable = pool.GetRandomMatchable();

                // position the matchable on screen
                //  newMatchable.transform.position = transform.position + new Vector3(x,y);

                onscreenPosition = transform.position + new Vector3(x, y);
                newMatchable.transform.position = onscreenPosition + offscreenOffset;

                // activate the matchable
                newMatchable.gameObject.SetActive(true);

                // place the matchable in the grid
                PutItemAt(newMatchable, x, y);

                int type = newMatchable.Type;

                while(!allowMatches & IsPartOfAMatch(newMatchable))
                {
                    //change the matchable's type until it isn't a match anymore
                    if(pool.NextType(newMatchable) == type)
                    {
                        Debug.LogWarning("Failed to find a matchable type that didn't match at (" + x + ", " + y + ")");
                        Debug.Break();
                        break;
                    }
                }

                //MoveMatchable to the appropriate screen position
                StartCoroutine(newMatchable.MoveToPosition(onscreenPosition));

                yield return new WaitForSeconds(0.1f);
            }
    }
    /*
    //TODO
    */
    private bool IsPartOfAMatch(Matchable matchable)
    {
        return false;
    }
}

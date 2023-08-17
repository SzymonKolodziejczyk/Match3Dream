using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{
    //the pool of Matchables with which to populate the grid
    private MatchablePool pool;
    private ScoreManager score;
    
    // A distance offscreen where the matchables will be spawned
    [SerializeField] private Vector3 offscreenOffset;

    // Get a reference to the pool on start
    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        score = ScoreManager.Instance;
    }

    //Populate the grid with matchables from the pool
    public IEnumerator PopulateGrid(bool allowMatches = false)
    {
        Matchable newMatchable;
        Vector3 onscreenPosition;

        for(int y = 0; y != Dimensions.y; ++y)
            for(int x = 0; x != Dimensions.x; ++x)
                if(IsEmpty(x, y))

                {
                    // get a matchable from the pool
                    newMatchable = pool.GetRandomMatchable();

                    // position the matchable on screen
                    // newMatchable.transform.position = transform.position + new Vector3(x,y);

                    onscreenPosition = transform.position + new Vector3(x, y);
                    newMatchable.transform.position = onscreenPosition + offscreenOffset;

                    // activate the matchable
                    newMatchable.gameObject.SetActive(true);

                    // tell me where it is on the grid
                    newMatchable.position = new Vector2Int(x,y);

                    // place the matchable in the grid
                    PutItemAt(newMatchable, x, y);

                    int initialType = newMatchable.Type;

                    while(!allowMatches & IsPartOfAMatch(newMatchable))
                    {
                        //Debug.Break(); //Checking how matching works
                        //yield return null;

                        //change the matchable's type until it isn't a match anymore
                        if(pool.NextType(newMatchable) == initialType)
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
        yield return null;
        
    }
    //Check if the matchable beign populated is part of a match or not
    private bool IsPartOfAMatch(Matchable toMatch)
    {
        int horizontalMatches   = 0,
            verticalMatches     = 0;

        //look to the left, now to the right
        horizontalMatches += CountMatchesInDirection(toMatch, Vector2Int.left);
        horizontalMatches += CountMatchesInDirection(toMatch, Vector2Int.right);

        if(horizontalMatches > 1)
            return true;

        //shake your up, now down
        verticalMatches += CountMatchesInDirection(toMatch, Vector2Int.up);
        verticalMatches += CountMatchesInDirection(toMatch, Vector2Int.down);

        if(verticalMatches > 1)
            return true;
        
        return false;
    }

    //Check if the number of matches on the grid starts from the matchable to match moving in indicated direction
    private int CountMatchesInDirection(Matchable toMatch, Vector2Int direction)
    {
        int matches = 0;
        Vector2Int position = toMatch.position + direction;

        while(CheckBounds(position) && !IsEmpty(position) && GetItemAt(position).Type == toMatch.Type)
        {
            ++matches;
            position += direction;
        }
        return matches;
    }

    public IEnumerator TrySwap(Matchable[] toBeSwapped)
    {
        // Make a local copy of swapping things for Cursor to not overwrite
        Matchable[] copies = new Matchable[2];
        copies[0] = toBeSwapped[0];
        copies[1] = toBeSwapped[1];
        //  yield until matchables animate swapping
        yield return StartCoroutine(Swap(copies));

        // check for a valid match
        Match[] matches = new Match[2];

        matches[0] = GetMatch(copies[0]);
        matches[1] = GetMatch(copies[1]);

        if(matches[0] != null)
            //resolve match
            //print(matches[0]);
            StartCoroutine(score.ResolveMatch(matches[0]));

        if(matches[1] != null)
            //resolve match
            //print(matches[1]);
            StartCoroutine(score.ResolveMatch(matches[1]));

        // If no matches, swap them back
        if(matches[0] == null && matches[1] == null)
            StartCoroutine(Swap(copies));

        else 
        {
            CollapseGrid();
            StartCoroutine(PopulateGrid(true));
        }
    }

    private Match GetMatch(Matchable toMatch)
    {
        Match match = new Match(toMatch);

        Match   horizontalMatch,
                verticalMatch;

        //Get horizontal matches from left to right
        horizontalMatch = GetMatchesInDirection(toMatch, Vector2Int.left);
        horizontalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.right));

        if(horizontalMatch.Count > 1)
            match.Merge(horizontalMatch);

        //Get vertical matches from up to down
        verticalMatch = GetMatchesInDirection(toMatch, Vector2Int.up);
        verticalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.down));

        if(verticalMatch.Count > 1)
            match.Merge(verticalMatch);

        if(match.Count == 1)
            return null;

        return match;
    }

    // Add each matching matchable in the direction to a match and return it
    private Match GetMatchesInDirection(Matchable toMatch, Vector2Int direction)
    {
        Match match = new Match();
        Vector2Int position = toMatch.position + direction;
        Matchable next;

        while(CheckBounds(position) && !IsEmpty(position))
        {
            next = GetItemAt(position);

            if(next.Type == toMatch.Type && next.Idle)
            {
                match.AddMatchable(next);
                position += direction;
            }
            else
                break;
        }
        return match;
    }

    private IEnumerator Swap(Matchable[] toBeSwapped)
    {
        //  swap in the grid data structure
        SwapItemsAt(toBeSwapped[0].position, toBeSwapped[1].position);

        //  tell the matchables their new positions
        Vector2Int temp = toBeSwapped[0].position;
        toBeSwapped[0].position = toBeSwapped[1].position;
        toBeSwapped[1].position = temp;

        //  get the world position of both
        Vector3[] worldPosition = new Vector3[2];
        worldPosition[0] = toBeSwapped[0].transform.position;
        worldPosition[1] = toBeSwapped[1].transform.position;

        //  move them to new position on screen
        StartCoroutine(toBeSwapped[0].MoveToPosition(worldPosition[1]));
        yield return StartCoroutine(toBeSwapped[1].MoveToPosition(worldPosition[0]));
    }
    private void CollapseGrid()
    {

        //  Go through each column left to right, search from bottom up to find an empty space, then look above the empty space, 
        //  and up through the rest of the column, until you find a non empty space. Move the matchable at the non empty space 
        //  into the empty space, then continue looking for empty spaces.
        for(int x = 0; x != Dimensions.x; ++x)
            for(int yEmpty = 0; yEmpty != Dimensions.y - 1; ++yEmpty)
                if(IsEmpty(x, yEmpty))
                    for(int yNotEmpty = yEmpty + 1; yNotEmpty != Dimensions.y; ++yNotEmpty)
                        if(!IsEmpty(x, yNotEmpty) && GetItemAt(x, yNotEmpty).Idle)
                            {
                                // Move the matchable from NotEmpty to Empty
                                MoveMatchableToPosition(GetItemAt(x, yNotEmpty), x, yEmpty);
                                break;
                            }
    }
    private void MoveMatchableToPosition(Matchable toMove, int x, int y)
    {
        //  move the matchable to its new position in the grid
        MoveItemTo(toMove.position, new Vector2Int(x, y));

        //  update the matchable's internal grid position
        toMove.position = new Vector2Int(x, y);

        //  start animation to move it on screen
        StartCoroutine(toMove.MoveToPosition(transform.position + new Vector3(x, y)));
    }
}

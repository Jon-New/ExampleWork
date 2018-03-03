using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/*
    This is a networked voting system that can be used by any other system in the game.
    It can handle 2-4 players and different criteria.
    
    fromrustcg.com
    razburygames.com
*/

public class VoteManager : NetworkBehaviour {

    Vote[] voteArray;

    object source;
    string functionName;

    VoteType voteType;
    float voteTimer;
    bool isActiveVote;

    void Update () //Runs the timer for a vote if needed, ends vote when time runs out
    {
        if (isActiveVote)
        {
            if (voteType == VoteType.Majority_Timed || voteType == VoteType.Unanimous_Timed)
            {
                if (voteTimer > 0)
                    voteTimer -= Time.deltaTime;
                else
                    EndVote();
            }
        }
    }

    [Server]
    public void BeginVote (object source, string functionName, string voteName, VoteType vType, float voteTime) //Is called by an outside script and passed information.  Starts vote on the server.
    {
        this.source = source;
        this.functionName = functionName;
        voteType = vType;
        voteTimer = voteTime;
        isActiveVote = true;
        BeginVote(voteName);
    }

    [Server]
    void BeginVote (string voteName) //Gets important information about the vote (number of players and names) 
    {
        int playerNum = FindObjectOfType<NetworkLobbyManager>().numPlayers;
	    voteArray = new Vote[playerNum];

        var names = FindObjectsOfType<Player>();
        string [] nameStrings = new string [names.Length];

        foreach (Player p in names)
            nameStrings[p.playerID] = p.playerName;

        EventBus.ServerInstance.Broadcast(EventConstants.VoteStarted, this, new VoteStartArgs(playerNum, voteName, nameStrings, voteType, voteTimer));
        RpcStartVote(playerNum, voteName, nameStrings, voteType, voteTimer);
    }

    [ClientRpc]
    void RpcStartVote (int i, string s, string[] a, VoteType vT, float vTime) //Tells the clients that a vote has started and passes all relevent information
    {
        Debug.Log("client rpc start vote");
        EventBus.ClientInstance.Broadcast(EventConstants.VoteStarted, this, new VoteStartArgs(i, s, a, vT, vTime));
    }


    [Server]
    void EndVote () //Ends the vote on the server and invokes the method passed to it by the source at the start of the vote
    {
        EventBus.ServerInstance.Broadcast(EventConstants.VoteEnded, this, EventArgs.Empty);

        isActiveVote = false;

        Type sourceType = source.GetType();
        MethodInfo info = sourceType.GetMethod(functionName);
        Vote [][] parameters = new Vote [][] {voteArray};

        Debug.Log("Before the Invoke");

        info.Invoke(source, parameters);

        Debug.Log("After the Invoke");

        RpcEndVote();
    }

    [ClientRpc]
    void RpcEndVote () //Tells clients that the vote has ended
    {
        EventBus.ClientInstance.Broadcast(EventConstants.VoteEnded, this, EventArgs.Empty);
    }
	

    [Server]
    public void UpdateVoteTally (Vote vote, int playerID) //Called whenver a player casts a vote; updates the server's 
    {
        voteArray[playerID] = vote;

        RpcUpdateVoteScreen(vote, playerID);

        CheckIfAllVoted();
    }

    [Server]
    void CheckIfAllVoted() //Checks to see if all votes in the array have been changed
    {
        foreach (Vote v in voteArray)
        {
            if (v == Vote.NotYetVoted)
                return;
        }

        EndVote();
    }

    [Server]
    public bool CheckVotes (Vote [] vArray, VoteType vType) //Checks to see if the vote was successful
    {
        int yesVotes = 0;
        int noVotes = 0;
        int abstainVotes = 0;

        foreach (Vote v in vArray)
        {
            if (v == Vote.Yes)
                yesVotes++;
            else if (v == Vote.No)
                noVotes++;
            else
                abstainVotes++;
        }

        if (vType == VoteType.Majority || vType == VoteType.Majority_Timed)
        {
            if (yesVotes/vArray.Length > .5f)
                return true;
            else
                return false;
        }
        else if (vType == VoteType.Unanimous || vType == VoteType.Unanimous_Timed)
        {
            if (yesVotes == vArray.Length)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    [ClientRpc]
    void RpcUpdateVoteScreen (Vote vote, int player) //Tells the clients to update their UI with new votes
    {
        FindObjectOfType<VoteScreen>().UpdateVotes(vote, player);
    }
}

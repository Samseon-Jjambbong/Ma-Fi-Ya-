using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreConfig", menuName = "ScriptableObjects/ScoreConfig", order = 1)]
public class ScoreConfig : ScriptableObject
{
    public List<RankScore> rankScores;

    [Serializable]
    public class RankScore
    {
        public int rank;
        public int score;
    }

    public int defaultScore;

    public int GetScoreFromRank(int rank)
    {
        var rankScore = rankScores.FirstOrDefault(rs => rs.rank == rank);
        return rankScore != null ? rankScore.score : defaultScore; // 
    }
}
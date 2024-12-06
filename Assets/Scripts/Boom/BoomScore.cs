using Boom.Utility;
using Boom.Values;
using Boom;
using Candid;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Candid.World.Models;
using Newtonsoft.Json;

namespace AstroAssault
{
    public class BoomScore : MonoBehaviour
    {
        #region FIELDS
        //This is the button that triggers the action "set_username"

        //This is the a coroutine cache from displaying the logs. It is used to stop it when required.
        private Coroutine logCoroutine;

        //The action ID
        [SerializeField] string actionId = "set_max_score";

        #endregion


        #region ACTION 

        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        public void ActionButtonClickHandler(int score)
        {
            //Forget() is included as we dont care awaiting for the result
            MaxScoreCheck(score);
        }

        private void MaxScoreCheck(int score)
        {
            var principal = UserUtil.GetPrincipal();

            EntityUtil.TryGetFieldAsText(principal, "score", "maxscore", out var outVal, "None");

            if (outVal == "None" || outVal == null)
            {
                //MainMenuGameManager.mMgameManager.boomLeaderboardPP.SetLeaderboardEntry("set_leaderboard_3", score.ToString(), MainMenuGameManager.mMgameManager.playerData.username);
                ExecuteAction(score).Forget();
            }
            else
            {
                int oldScore = int.Parse(outVal);

                if (score > oldScore)
                {
                    //MainMenuGameManager.mMgameManager.boomLeaderboardPP.SetLeaderboardEntry("set_leaderboard_3", score.ToString(), MainMenuGameManager.mMgameManager.playerData.username);
                    ExecuteAction(score).Forget();
                }
                else return;
            }
        }

        public async UniTaskVoid ExecuteAction(int score)
        {
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Set up arguments

            var newMaxScore = score.ToString();

            //if (string.IsNullOrEmpty(newUsername)) return;

            List<Field> fields = new()
            {
                new("maxscore", newMaxScore),
                //new("animal", newUsername),
            };


            //SECTION B: Action execution

            //Here we execute the action by passing the actionId we wantto execute.
            //actionLogText.text = $"Processing Action of id: \"{actionId}\" with arguments:\n{JsonConvert.SerializeObject(fields)}";
            var actionResult = await ActionUtil.ProcessAction(actionId, fields);

            //SECTION C: Error handling

            //Here we handle the errors
            bool isError = actionResult.IsErr;

            if (isError)
            {
                string errorMessage = actionResult.AsErr().content;

                Debug.LogError(errorMessage);
                //logCoroutine = StartCoroutine(DisplayTempLog(errorMessage));

                return;
            }
            else
            {
                //GameManager.gameManager.mainMenu._playerData.maxScoreAA = score;
                GameManager.gameManager.mainMenu.ScoreSafeAA(score.ToString());
            }

            //SECTION D: At this point the action was successful, therefore we print the username change

            //logCoroutine = StartCoroutine(DisplayTempLog($"You have changed your username to: {newUsername}"));
        }

        #endregion
    }
}

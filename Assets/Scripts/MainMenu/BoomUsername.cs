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
using EdjCase.ICP.Candid.Models;

namespace AstroAssault
{
    public class BoomUsername : MonoBehaviour
    {
        #region FIELDS

        //This is used to input the user's username
        [SerializeField] TMP_InputField userNameInputField;
        //This is the button that triggers the action "set_username"
        [SerializeField] Button actionButton;

        //This is the a coroutine cache from displaying the logs. It is used to stop it when required.
        private Coroutine logCoroutine;

        //The action ID
        [SerializeField] string actionId = "set_username";
        [SerializeField] MainMenu mainMenu;

        #endregion


        #region MONO
        private void Awake()
        {

            AutoNameLogin();
            //This is to clear out any unwanted listener
            actionButton.onClick.RemoveAllListeners();

            //Register to action button click
            actionButton.onClick.AddListener(ActionButtonClickHandler);
        }

        private void OnDestroy()
        {
            //Unregister to action button click
            actionButton.onClick.RemoveListener(ActionButtonClickHandler);

            //We unregister from MainDataTypes.LoginData change event
            //UserUtil.RemoveListenerMainDataChange<MainDataTypes.LoginData>(LoginDataChangeHandler);
        }

        private void OnEnable()
        {
            //actionLogText.text = "...";
        }
        #endregion


        #region ACTION 

        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        public void ActionButtonClickHandler()
        {
            //Forget() is included as we dont care awaiting for the result
            ExecuteAction().Forget();
        }

        public async UniTaskVoid ExecuteAction()
        {
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Set up arguments

            var newUsername = userNameInputField.text;

            if (string.IsNullOrEmpty(newUsername)) return;

            List<Field> fields = new()
            {
                new("username", newUsername),
                //new("animal", newUsername),
            };

            //SECTION B: Action execution

            //Here we execute the action by passing the actionId we wantto execute.
            //actionLogText.text = $"Processing Action of id: \"{actionId}\" with arguments:\n{JsonConvert.SerializeObject(fields)}";
            var actionResult = await ActionUtil.ProcessAction(actionId, fields);

            mainMenu.ScoreUpdateAA();

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
                mainMenu.NameSafe(newUsername);
            }
        }

        #endregion


        #region USER DATA

        /*private void LoginDataChangeHandler(MainDataTypes.LoginData data)
        {

            Debug.Log("Does it ever enters here????");
            //If user is not logged in, return
            if (data.state != MainDataTypes.LoginData.State.LoggedIn) return;

            //Temporally store the user principal
            var principal = data.principal;

            //Try get the field "username" from the user's entity "user_profile"
            //If the entity or entity's field could not be found, then it will default the value to "None"
            EntityUtil.TryGetFieldAsText(principal, "user_profile", "username", out var outVal, "None");

            //Update InputField
            //UpdateInputFieldContent(outVal);
            UpdateUsername(outVal);
        }*/

        private void AutoNameLogin()
        {
            mainMenu.ScoreUpdateAA();

            var principal = UserUtil.GetPrincipal();

            EntityUtil.TryGetFieldAsText(principal, "user_profile", "username", out var outVal, "None");

            UpdateUsername(outVal);
        }

        /*private void UpdateInputFieldContent(string value)
        {
            userNameInputField.SetTextWithoutNotify(value);
        }*/

        private void UpdateUsername(string value)
        {

            if (value == "None" || value == null)
            {
                return;
            }
            else
            {
                mainMenu.NameSafe(value);
            }
        }

        /*private void UpdateScore(string score)
        {
            if (score == "None" || score == null)
            {
                mainMenu.ScoreSafePO("0");
            }
            else
            {
                mainMenu.ScoreSafePO(score);
            }
        }*/

        #endregion
    }
}

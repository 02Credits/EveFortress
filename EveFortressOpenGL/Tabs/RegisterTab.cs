using EveFortressModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class RegisterTab : UITab
    {
        public override string Title
        {
            get { return "Register"; }
        }

        Label usernameError;
        TextInput usernameInput;
        Label passwordError;
        TextInput passwordInput;
        Button registerButton;
        Button backButton;

        public RegisterTab(string username, string password)
        {
            new Label(this, 1, 1, "UserName:");
            usernameError = new Label(this, 11, 1, "", Color.Red);
            usernameInput = new TextInput(this, 2, 3, 20, (s) => passwordInput.Focus());
            usernameInput.Text = username;
            new Label(this, 1, 5, "Password:");
            passwordError = new Label(this, 11, 5, "", Color.Red);
            passwordInput = new TextInput(this, 2, 7, 20, (s) => registerButton.OnClicked(), password: true);
            passwordInput.Text = password;
            registerButton = new Button(this, 1, 9, AttemptRegister, "Register");
            backButton = new Button(this, 1, 11, () => ParentSection.ReplaceTab(new LoginTab()), "Back");
            usernameInput.Focus();
        }

        private async void AttemptRegister()
        {
            usernameError.Text = "";
            passwordError.Text = "";
            var loginInformation = await Game.ServerMethods.RegisterUser(new LoginInformation(usernameInput.Text, passwordInput.Text));
            HandleRegisterResponse(loginInformation);
        }
        
        private void HandleRegisterResponse(LoginInformation info)
        {
            switch (info.LoginResponse)
            {
                case LoginResponse.PasswordWrong: 
                case LoginResponse.AlreadyLoggedIn:
                    usernameError.Text = "That username is taken";
                    usernameInput.Focus();
                    break;
                case LoginResponse.UsernameEmpty:
                    usernameError.Text = "Needs non-empty username";
                    usernameInput.Focus();
                    break;
                case LoginResponse.PasswordEmpty:
                    passwordError.Text = "Needs non-empty password";
                    passwordInput.Focus();
                    break;
                case LoginResponse.Success:
                    ParentSection.ReplaceTab(new NewTab());
                    break;
                default:
                    usernameError.Text = "Unknown error has occured";
                    break;
            }
        }
    }
}

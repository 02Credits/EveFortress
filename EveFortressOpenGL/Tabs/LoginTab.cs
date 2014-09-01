using EveFortressModel;
using Microsoft.Xna.Framework;

namespace EveFortressClient
{
    public class LoginTab : UITab
    {
        public override string Title
        {
            get { return "Login Tab"; }
        }

        private TextInput usernameInput;
        private Label usernameError;
        private TextInput passwordInput;
        private Label passwordError;
        private Button registerButton;
        private Button loginButton;

        public LoginTab()
        {
            new Label(this, 1, 1, "UserName:");
            usernameError = new Label(this, 11, 1, "", Color.Red);
            usernameInput = new TextInput(this, 2, 3, 20, (s) => passwordInput.Focus());
            new Label(this, 1, 5, "Password:");
            passwordError = new Label(this, 11, 5, "", Color.Red);
            passwordInput = new TextInput(this, 2, 7, 20, (s) => loginButton.OnClicked(), password: true);
            loginButton = new Button(this, 1, 9, AttemptLogin, "Login");
            registerButton = new Button(this, 1, 11,
                () => ParentSection.ReplaceTab(new RegisterTab(usernameInput.Text, passwordInput.Text)), "Register");
            usernameInput.Focus();
        }

        private async void AttemptLogin()
        {
            usernameError.Text = "";
            passwordError.Text = "";
            var loginInformation = await Game.ServerMethods.Login(new LoginInformation(usernameInput.Text, passwordInput.Text));
            HandleLoginResponse(loginInformation);
        }

        private void HandleLoginResponse(LoginInformation loginInformation)
        {
            switch (loginInformation.LoginResponse)
            {
                case LoginResponse.AlreadyLoggedIn:
                    usernameError.Text = "That user is already logged in...";
                    break;

                case LoginResponse.PasswordWrong:
                    passwordError.Text = "Wrong password";
                    break;

                case LoginResponse.UserDoesNotExist:
                    usernameError.Text = "Unknown login. New user?";
                    break;

                case LoginResponse.UsernameEmpty:
                    usernameError.Text = "Needs non-empty username";
                    ActiveElement = usernameInput;
                    break;

                case LoginResponse.PasswordEmpty:
                    passwordError.Text = "Needs non-empty password";
                    ActiveElement = passwordInput;
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
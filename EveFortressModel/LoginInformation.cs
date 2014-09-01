using ProtoBuf;

namespace EveFortressModel
{
    [ProtoContract]
    public class LoginInformation
    {
        [ProtoMember(1)]
        public string UserName { get; set; }

        [ProtoMember(2)]
        public string Password { get; set; }

        [ProtoMember(3)]
        public LoginResponse LoginResponse { get; set; }

        public LoginInformation()
        {
        }

        public LoginInformation(string username, string password, LoginResponse loginResponse = LoginResponse.Unknown)
        {
            UserName = username;
            Password = password;
            LoginResponse = loginResponse;
        }
    }
}
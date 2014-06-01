using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    public enum LoginResponse
    {
        Unknown,
        Success,
        PasswordWrong,
        AlreadyLoggedIn,
        UserDoesNotExist,
        UsernameEmpty,
        PasswordEmpty,
    }
}

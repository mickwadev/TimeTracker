using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.SupabaseDB;

public enum UserState
{
    CLIENT_NOT_INITIALIZED,
    CLIENT_INITIALIZED,
    USER_SIGNUP_FAILED,
    USER_SIGNIN_FAILED,
    USER_SIGNEDIN
}

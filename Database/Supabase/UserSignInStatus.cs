using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace Database.SupabaseDB;

public class UserSignInStatus
{
    public bool OK;
    public string Info;
    public LoggedUser? User;
    public string SecureJsonSupabaseSessionKey;
}

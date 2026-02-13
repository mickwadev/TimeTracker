using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.SupabaseDB;

public class LoggedUser
{
    public string UserName { get; set; }
    public string UserId { get; set; }

    public string AccessToken;
}

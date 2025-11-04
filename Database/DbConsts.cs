using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database;

internal class DbConsts
{ 
    public const string dbDateFormat = "yyyy-MM-dd HH:mm:ss"; // this format is required for sqlite date functions to work.
    public const string NotSetUser = "user_not_set";
    public const string ActivityNotSet = "at_not_set";
    public const long NoBackupID = -1;
}

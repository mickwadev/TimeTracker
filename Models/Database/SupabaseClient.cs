using Newtonsoft.Json;
using Supabase;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Data;

namespace TimeTracker.Models.Database
{
    public class SupabaseClient : IDbBackup
    {
        private Supabase.Client _client = null;
        const string fakeEmailPart = "@sofakeemail.com";
        const string secureJsonSupabaseSessionKey = "supabase_session";

        private LoggedUser _loggedUser = null;

        public string GetLoggedUserName => _loggedUser?.UserName ?? DbConsts.NotSetUser;
        
        public async Task InitSupabaseClient()
        {
            var url = "https://baeovtminnahkokhrxnh.supabase.co";   // Project URL
            var anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJhZW92dG1pbm5haGtva2hyeG5oIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA0NjkxNTUsImV4cCI6MjA3NjA0NTE1NX0.QN5dK5Yi9aoJcm0tEjsewIEVQZttTwkajG5mDuGzDts";               // anon key (not service role)

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false,
                AutoRefreshToken = true
            };

            _client = new Supabase.Client(url, anonKey, options);
            var d = await _client.InitializeAsync();
            Trace.WriteLine(d.Auth.CurrentUser?.Email ?? " [No curent user]"); // to się zgadza, bo jest tylko client a nie użytkownik.
        }

        public async Task<UserSignInStatus> SignUpUser(string newUserEmail, string displayName, string newUserPassword)
        {
            UserSignInStatus status = new();
            if (newUserEmail.Contains("@") is false)
            {
                newUserEmail += fakeEmailPart;
            }
            Supabase.Gotrue.Session signUpResult = null;
            try
            {
                signUpResult = await _client.Auth.SignUp(newUserEmail, newUserPassword);
            }
            catch (Exception ex)
            {
                // co może pójść nie tak?
                // {"code":422,"error_code":"weak_password","msg":"Password should be at least 6 characters.","weak_password":{"reasons":["length"]}}
                // {"code":400,"error_code":"validation_failed","msg":"Unable to validate email address: invalid format"}
                // {"code":422,"error_code":"user_already_exists","msg":"User already registered"}

                // jesli jest włączona captcha:
                // {"code":500,"error_code":"unexpected_failure","msg":"captcha verification process failed","error_id":"99015d0de0e7289c-WAW"}
                Trace.WriteLine("Signup failed: "+ex.ToString());
                status.OK = false;
                status.Info = $"Sign up new user failed: {ex.Message}";
                return status;
            }
            Supabase.Gotrue.User user = signUpResult?.User;
            if (user is null)
            {
                Trace.WriteLine("User after sign up is null. Auto sign in...");
            }
            var session = await _client.Auth.SignIn(newUserEmail, newUserPassword);
            user = session?.User;

            Trace.WriteLine(user.Email);
            // Now save this session:
            var sessionJson = JsonConvert.SerializeObject(session);
            Trace.WriteLine($"Session json: {sessionJson}");
            await SecureStorage.SetAsync(secureJsonSupabaseSessionKey, sessionJson);

            var attribs = new Supabase.Gotrue.UserAttributes()
            {
                Data = new Dictionary<string, object>
                {
                    ["display_name"] = displayName
                }
            };

            Supabase.Gotrue.User updated = await _client.Auth.Update(attribs);
            Trace.WriteLine(updated.UserMetadata["display_name"]?.ToString());
            status.OK = true;
            status.Info = "Sign up and sign in {} successful...";
            status.User = new LoggedUser()
            {
                UserName = updated.Email.Split("@").First(),
                UserId = _client.Auth.CurrentSession.User.Id
            };
            return status;
        }

        public async Task<UserSignInStatus> SignIn(string email, string password)
        {
            UserSignInStatus status = new();
            if (email.Contains("@") is false)
            {
                email += fakeEmailPart;
            }
            Supabase.Gotrue.Session session = null;
            try
            {
                // to działa dla użytkownika stworzonego tam na stronie i nie tylko, tworzonych tutaj też działa.
                session = await _client.Auth.SignIn(email, password);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"SignIn exception: {ex.Message}");
                status.OK = false;
                status.Info = $"Login failed: {ex.Message}";
                return status;
            }
            if (session?.User is null)
            {
                Trace.WriteLine("Login failed...");
                status.OK = false;
                status.Info = "Login failed...";
                return status;
            }

            Trace.WriteLine($"Hello {session.User.UserMetadata["display_name"] ?? " [NoName] "}");

            // Now save this session:
            var sessionJson = JsonConvert.SerializeObject(session);
            Trace.WriteLine($"Session json: {sessionJson}");
            await SecureStorage.SetAsync(secureJsonSupabaseSessionKey, sessionJson);
            //_client.Auth.CurrentSession.User.Id

            status.Info = "SignIn OK";
            status.OK = true;
            status.User  = new LoggedUser()
            {
                UserName = session.User.Email.Split("@").First(),
                UserId =_client.Auth.CurrentSession.User.Id
            };

            return status;
        }

        public async Task<WorkTime> BackupSingleData(WorkTime wt)
        {
            Supabase.Postgrest.QueryOptions options = new Supabase.Postgrest.QueryOptions()
            {
                Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation
            };
            Supabase.Postgrest.Responses.ModeledResponse<WorkTime> response = null;
            try
            {
                response = await _client.From<WorkTime>().Insert(wt , options);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return null;
            }

           // Trace.WriteLine($"Added: {response.Models.Count}");
         //   foreach (var model in response.Models)
         //   {
              //  Trace.WriteLine($"{model.ID}");
           // }
            return response.Model;
        }

        public async Task PutTestData()
        {
            WorkTime wt = new WorkTime()
            {
                Title = $"[1] Test Data from {_client.Auth.CurrentUser.Email}",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + TimeSpan.FromSeconds(1)
            };

            WorkTime wt2 = new WorkTime()
            {
                Title = $"[2] Test Data from {_client.Auth.CurrentUser.Email}",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + TimeSpan.FromSeconds(1)
            };
            Supabase.Postgrest.QueryOptions options = new Supabase.Postgrest.QueryOptions()
            {
                Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation
            };
            Supabase.Postgrest.Responses.ModeledResponse<WorkTime> response = null;
            try
            {
                response = await _client.From<WorkTime>().Insert(new List<WorkTime>() { wt, wt2 }, options);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return;
            }

            Trace.WriteLine($"Added: {response.Models.Count}");
            foreach (var model in response.Models)
            {
                Trace.WriteLine($"{model.ID}");
            }
        }

        public async Task<List<WorkTime>> GetData()
        {
            Supabase.Postgrest.Responses.ModeledResponse<WorkTime> response = null;
            try
            {
                // this Select is not mandatory
                response = await _client.From<WorkTime>().Select("*").Get();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return null; // todo use  null object pattern here
            }

            Trace.WriteLine($"Received: {response.Models.Count}");
            foreach (var model in response.Models)
            {
                Trace.WriteLine($"{model.ID} {model.Title}");
            }

            return response.Models;
        }

        public async Task<UserSignInStatus> RestoreSession()
        {
            UserSignInStatus status = new();
            string sessionJson = await SecureStorage.GetAsync(secureJsonSupabaseSessionKey);
            if (string.IsNullOrEmpty(sessionJson))
            {
                Trace.WriteLine("Failed to restore session...");
                status.Info = "No previous session stored...";
                status.OK = false;
                return status;
            }
            Trace.WriteLine("Found previous session.");
            var savedSession = JsonConvert.DeserializeObject<Supabase.Gotrue.Session>(sessionJson);
            
            Session restoredSession = await _client.Auth.SetSession(savedSession.AccessToken, savedSession.AccessToken);
            var userName = savedSession.User.Email.Split("@").First();
            Trace.WriteLine($"Restored session for: {restoredSession.User.Email} to {restoredSession.ExpiresIn}");
            Trace.WriteLine($"ID: {_client.Auth.CurrentSession.User.Id}");
            status.Info = $"Session restored for user \"{userName}\": OK (expires: {restoredSession.ExpiresIn})";
            status.OK = true;
            status.User = new LoggedUser()
            {
                UserName = userName,
                UserId = _client.Auth.CurrentSession.User.Id
            };

            return status;
        }

        public async Task SignOutUser()
        {
            Trace.WriteLine($"Session logout: {_client.Auth.CurrentUser.Email}");
            bool successfulyRemoved =SecureStorage.Remove(secureJsonSupabaseSessionKey);

            try
            {
                await _client.Auth.SignOut();
            }
            catch (Exception ex)
            {
                // This happens when I logout on other device first: {"code":403,"error_code":"session_not_found","msg":"Session from session_id claim in JWT does not exist"}
                Trace.WriteLine($"Logout issue: {ex.Message}");
            }
            Trace.WriteLine($"CurrentUser is null: {_client.Auth.CurrentUser is null}"); // true
        }
    }
}

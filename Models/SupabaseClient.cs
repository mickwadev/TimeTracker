using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Models.db
{
    internal class SupabaseClient : IDbBackup
    {
        private Client client;

        public async Task InitSupabaseClient()
        {
            var url = "https://baeovtminnahkokhrxnh.supabase.co";   // Project URL
            var anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJhZW92dG1pbm5haGtva2hyeG5oIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA0NjkxNTUsImV4cCI6MjA3NjA0NTE1NX0.QN5dK5Yi9aoJcm0tEjsewIEVQZttTwkajG5mDuGzDts";               // anon key (not service role)

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false
            };

            client = new Client(url, anonKey, options);
            await client.InitializeAsync();
        }
    }
}

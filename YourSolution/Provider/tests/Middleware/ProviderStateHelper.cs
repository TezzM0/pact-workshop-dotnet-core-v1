using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tests.Middleware
{
    public static class ProviderStateHelper
    {
        public static void RemoveAllData()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../../data");
            var deletePath = Path.Combine(path, "somedata.txt");

            if (File.Exists(deletePath))
            {
                File.Delete(deletePath);
            }
        }

        public static void AddData()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../../data");
            var writePath = Path.Combine(path, "somedata.txt");

            if (!File.Exists(writePath))
            {
                File.Create(writePath);
            }
        }
    }
}

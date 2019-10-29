using System.IO;

namespace provider
{
    public class Data
    {
        public Data()
        {
        }

        public bool DataIsMissing()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"../../data");
            string pathWithFile = Path.Combine(path, "somedata.txt");

            return !System.IO.File.Exists(pathWithFile);
        }
    }
}
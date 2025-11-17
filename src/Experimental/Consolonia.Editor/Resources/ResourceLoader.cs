using System.Reflection;

namespace ConsoloniaEdit.Demo.Resources
{
    internal class ResourceLoader
    {
        const string SampleFilesPrefix = "Consolonia.Editor.Resources.SampleFiles.";

        internal static string LoadSampleFile(string fileName)
        {
            Stream? stream = typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(SampleFilesPrefix + fileName);

            if (stream == null)
                return string.Empty;

            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
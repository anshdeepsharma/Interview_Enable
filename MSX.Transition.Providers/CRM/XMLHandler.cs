namespace MSX.Transition.Providers.CRM
{
    public static class XMLHandler
    {
        public static string ReadXMLFile(string relativeFilePath)
        {
            string result = string.Empty;
            using (var streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + relativeFilePath))
            {
                result = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return result;
        }
    }
}

using System.Xml.Linq;

namespace Order_Manage.XML
{
    public class QueryLoader
    {
        private readonly string _filePath;

        public QueryLoader(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("XML file not found.", filePath);

            _filePath = filePath;
        }

        public Dictionary<string, string> Read_Xml()
        {
            var queries = new Dictionary<string, string>();
            var doc = XDocument.Load(_filePath);

            foreach (var section in doc.Root?.Elements()!)
            {
                foreach (var query in section.Elements())
                {
                    var key = query.Name.LocalName;
                    var value = query.Value.Trim();
                    queries.TryAdd(key, value);
                }
            }

            return queries;
        }
    }
}

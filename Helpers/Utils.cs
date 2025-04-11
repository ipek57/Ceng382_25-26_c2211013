using Newtonsoft.Json;
using System.Reflection;

namespace Week5Project.Helpers
{
    public class Utils
    {
        private static readonly Utils _instance = new Utils();
        public static Utils Instance => _instance;

        private Utils() { }

        public string ExportToJson<T>(List<T> data, List<string>? selectedProperties = null)
        {
            if (selectedProperties == null || selectedProperties.Count == 0)
            {
                return JsonConvert.SerializeObject(data, Formatting.Indented);
            }

            var filteredData = new List<Dictionary<string, object>>();

            foreach (var item in data)
            {
                var dict = new Dictionary<string, object>();
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    if (selectedProperties.Contains(prop.Name))
                    {
                        dict[prop.Name] = prop.GetValue(item);
                    }
                }
                filteredData.Add(dict);
            }

            return JsonConvert.SerializeObject(filteredData, Formatting.Indented);
        }
    }
}

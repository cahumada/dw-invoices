
namespace iaas.app.dw.invoices.Domain.Entities
{
    public static class GenericFunctions
    {

        public static string ConvertListToString<T>(List<T> data)
        {
            //Get headers into class
            List<string> headers = typeof(T).GetProperties().Select(x => x.Name).ToList();
            string fileString = string.Join(",", headers) + Environment.NewLine;

            //Get data into list
            foreach (var line in data)
            {
                List<object> values = typeof(T).GetProperties().Select(prop => prop.GetValue(line)).ToList();
                string valuesStr = string.Join(",", values);
                fileString += $"{valuesStr}{Environment.NewLine}";
            }

            return fileString;
        }

        public static string ConvertMemoryStreamToBase64(MemoryStream memoryStream, string extension)
        {
            memoryStream.Position = 0;

            //Create base64 from file
            byte[] bytes = memoryStream.ToArray();
            string base64String = Convert.ToBase64String(bytes);
            string base64FileString = $"data:text/{extension};base64,{base64String}";
            return base64FileString;
        }
    }
}

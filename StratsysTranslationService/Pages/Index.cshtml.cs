using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Text;
using System.Net.Http.Headers;

namespace RazorPagesContacts.Pages
{
    public class IndexModel : PageModel
    {
        static string host = "https://api.cognitive.microsofttranslator.com";
        static string path = "/translate?api-version=3.0";
        // Translate to German and Italian.
        static string params_ = "&to=no";
        static string uri = host + path + params_;
        static string key = "30939bcfac5d454e8a2d42d9c2fdd308";

        public async Task OnGetAsync()
        {
        }
        public async Task OnPostAsync()
        {

            // NOTE: Replace this example key with a valid subscription key.
            await Translate(Customer.From);
        }
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public string Message { get; private set; } = "";
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {

            return RedirectToPage();
        }
        public async Task<IActionResult> Translate(string text)
        {
            System.Object[] body = new System.Object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                client.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody));
                var resultstring = JsonConvert.DeserializeObject(responseBody);
                //var result2 = JsonConvert.DeserializeObject<Rootobject>(resultstring.toString());

                //Message = result.Property1[0].translations[0].text;
                //Customer.To = result.Property1[0].translations[0].text;
                return RedirectToPage();
            }
        }


        public class Rootobject
        {
            public Class1[] Property1 { get; set; }
        }

        public class Class1
        {
            public Detectedlanguage detectedLanguage { get; set; }
            public Translation[] translations { get; set; }
        }

        public class Detectedlanguage
        {
            public string language { get; set; }
            public float score { get; set; }
        }

        public class Translation
        {
            public string text { get; set; }
            public string to { get; set; }
        }

    }


}
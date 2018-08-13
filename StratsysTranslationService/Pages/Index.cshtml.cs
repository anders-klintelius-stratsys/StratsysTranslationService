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
using HtmlAgilityPack;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StratsysTranslationService;

namespace RazorPagesContacts.Pages
{
    public class IndexModel : PageModel
    {
        static string host = "https://api.cognitive.microsofttranslator.com";

        static string path = "/translate?api-version=3.0";

        // Translate to German and Italian.
        static string params_ = "&to=no&to=en";
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

        [BindProperty] public Customer Customer { get; set; }
        [BindProperty] public string Message { get; private set; } = "";
        public string Message2 { get; private set; } = "";

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {

            return RedirectToPage();
        }

        public async Task<IActionResult> Translate(string text)
        {
            Startup.Progress = 1;
            var split = new List<String>();
            var translated = "";
            var translated2 = "";
            try
            {
                while (text.Length > 0)
                {
                    Startup.Progress = Startup.Progress + 1;
                    int e;
                    int s;
                    if (text.IndexOf("<") == 0)
                    {
                        s = text.IndexOf(char.Parse("<"));
                        e = text.IndexOf(char.Parse(">")) + 1;
                        var t = text.Substring(s, e - s);
                        //split.Add(t);
                        translated = translated + t;
                        translated2 = translated2 + t;

                        text = text.Remove(0, e);
                    }
                    else
                    {
                        if (text.IndexOf(char.Parse("<")) > 0)
                        {
                            s = text.IndexOf(char.Parse("<"));
                        }
                        else
                        {
                            s = text.Length;
                        }
                        var t = await TranslateString(text.Substring(0, s));
                        //split.Add(t);
                        translated = translated + t[0];
                        translated2 = translated2 + t[1];
                        text = text.Remove(0, s);
                    }
                }
            }
            catch(Exception ex)
            {

            }


            Message = translated;
            Message2 = translated2;
            return RedirectToPage();
        }
        [HttpPost]
        public ActionResult Progress()
        {
            return this.Content(Startup.Progress.ToString());
        }
        public async Task<List<string>> TranslateString(string text)
        {
            System.Object[] body = new System.Object[] {new {Text = text}};
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

                var result2 = JsonConvert.DeserializeObject<RootObject>(resultstring.ToString()
                    .TrimStart(char.Parse("[")).TrimEnd(char.Parse("]")));

                var rlist =  new List<String>();
                if (result2.translations!=null)
                { 
                rlist.Add(result2.translations[0].text);
                    rlist.Add(result2.translations[1].text);
                return rlist;
                }
                else
                {
                    rlist.Add(text);
                    rlist.Add(text);
                    return rlist;
                }
            }

        }

        public class DetectedLanguage
        {
            public string language { get; set; }
            public double score { get; set; }
        }

        public class Translation
        {
            public string text { get; set; }
            public string to { get; set; }
        }

        public class RootObject
        {
            public DetectedLanguage detectedLanguage { get; set; }
            public List<Translation> translations { get; set; }
        }

    }
}



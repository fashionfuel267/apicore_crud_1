using System.Net.Http;
using Consume_WEBAPICORE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;

namespace Consume_WEBAPICORE.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public OrdersController(IHttpClientFactory factory)
        {
            this._factory = factory;
        }
        public async Task<IActionResult> Index()
        {
            HttpClient client = _factory.CreateClient();
            client.BaseAddress =
            new Uri("https://localhost:7178/api/");
            client.DefaultRequestHeaders.Add(
            HeaderNames.UserAgent, "ExchangeRateViewer");
            var response = await client.GetAsync("sales");
            response.EnsureSuccessStatusCode();
            var result= await response.Content.ReadFromJsonAsync<IEnumerable< OrderVM>>();
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ProductId = new SelectList(await GetProduct(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(OrderVM order)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            string orderString = JsonConvert.SerializeObject(order, jsonSerializerSettings);
            var content = new MultipartFormDataContent();
           // content.Add(new StringContent(orderString, "Orderdata"));
            content.Add(new StringContent(orderString), "orderdata");
            //content.Add(new StringContent(entity.ContactNumber), "ContactNumber");
            if (order.Image != null)
            {
                var imageContent = new StreamContent(order.Image.OpenReadStream());
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue (order.Image.ContentType);
                content.Add(imageContent, "LOGOFile", order.Image.FileName);

            }
            HttpClient client = _factory.CreateClient();
            client.BaseAddress =
            new Uri("https://localhost:7178/api/");
            client.DefaultRequestHeaders.Add(
            HeaderNames.UserAgent, "ExchangeRateViewer");
            var response = await client.PostAsync("sales",content);
            
             
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("index");
            }


            ViewBag.ProductId = new SelectList(await GetProduct(), "Id", "Name");
            return View();
        }
        private async Task< IEnumerable<ProductVM>> GetProduct()
        {
            HttpClient client = _factory.CreateClient();
            client.BaseAddress =
            new Uri("https://localhost:7178/api/");
            client.DefaultRequestHeaders.Add(
            HeaderNames.UserAgent, "ExchangeRateViewer");
            var response = await client.GetAsync("Products");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ProductVM>>();
            return result?? Enumerable.Empty<ProductVM>();
        }
    }
}

using System.Net.Http;
using System.Reflection;
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
            var model = new OrderVM()
            {
                Details = { new DetailsVM{
                 OrderId=0,
                 ProductId=0,
                     Price=0,
                } }
            };
             
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(OrderVM order, string act = "")
        {

            if (act == "add")
            {
                order.Details.Add(new DetailsVM());
            }
            if (act.StartsWith("remove"))
            {

                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                order.Details.RemoveAt(index);
            }
            if (act == "Save")
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
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(order.Image.ContentType);
                    content.Add(imageContent, "LOGOFile", order.Image.FileName);

                }
                HttpClient client = _factory.CreateClient();
                client.BaseAddress =
                new Uri("https://localhost:7178/api/");
                client.DefaultRequestHeaders.Add(
                HeaderNames.UserAgent, "ExchangeRateViewer");
                var response = await client.PostAsync("sales", content);


                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
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

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.ProductId = new SelectList(await GetProduct(), "Id", "Name");
            if (id == null) {

                return BadRequest();
            }
            HttpClient client = _factory.CreateClient();
            client.BaseAddress =
            new Uri("https://localhost:7178/api/");
            client.DefaultRequestHeaders.Add(
            HeaderNames.UserAgent, "ExchangeRateViewer");
            var response = await client.GetAsync("Sales/" + id.Value);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OrderVM>();
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(OrderVM order,string act = "")
        {
            if (act == "add")
            {
                order.Details.Add(new DetailsVM());
            }
            if (act.StartsWith("remove"))
            {

                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                order.Details.RemoveAt(index);
            }
            if (act == "Update")
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
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(order.Image.ContentType);
                    content.Add(imageContent, "LOGOFile", order.Image.FileName);

                }
                HttpClient client = _factory.CreateClient();
                client.BaseAddress =
                new Uri("https://localhost:7178/api/");
                client.DefaultRequestHeaders.Add(
                HeaderNames.UserAgent, "ExchangeRateViewer");
                var response = await client.PutAsync("sales", content);


                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
            }
            ViewBag.ProductId = new SelectList(await GetProduct(), "Id", "Name");
            return View(order);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Dispatcher;
using Jumeirah.ContentService.DelegatingHandlers;

namespace Jumeirah.ContentService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Pipelines
            HttpMessageHandler siteLanguagePipeline =
                HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(GlobalConfiguration.Configuration),
                    new[] { new SiteLanguageDispatcher() });

            HttpMessageHandler cityPipeline =
                HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(GlobalConfiguration.Configuration),
                    new DelegatingHandler[] { new SiteLanguageDispatcher(), new CityDispatcher() });

            HttpMessageHandler hotelPipeline =
                HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(GlobalConfiguration.Configuration),
                    new DelegatingHandler[] { new SiteLanguageDispatcher(), new CityDispatcher(), new HotelDispatcher() });

            routes.MapHttpRoute(
                name: "CityApiRoute",
                routeTemplate: "content/{site}/{language}/cities/{id}",
                defaults: new { controller = "City", id = RouteParameter.Optional },
                constraints: null,
                handler: siteLanguagePipeline
            );

            routes.MapHttpRoute(
                name: "HotelApiRoute",
                routeTemplate: "content/{site}/{language}/cities/{cityName}/hotels/{id}",
                defaults: new { controller = "Hotel", id = RouteParameter.Optional },
                constraints: null,
                handler: cityPipeline
            );

            routes.MapHttpRoute(
                name: "SpaApiRoute",
                routeTemplate: "content/{site}/{language}/cities/{cityName}/hotels/{hotelName}/spas/{id}",
                defaults: new { controller = "Spa", id = RouteParameter.Optional },
                constraints: null,
                handler: hotelPipeline
            );

            routes.MapHttpRoute(
                name: "SpaApiRoute1",
                routeTemplate: "content/{site}/{language}/cities/{cityName}/spas/{id}",
                defaults: new { controller = "Spa", id = RouteParameter.Optional },
                constraints: null,
                handler: cityPipeline
            );
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;

namespace Jumeirah.ContentService.DelegatingHandlers
{
    public class HotelDispatcher : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IHttpRouteData routeData = request.GetRouteData();
            var hotelName = routeData.Values["hotelName"].ToString();

            // TODO: Check if the site and language nodes exist here

            //if (notExist)
            //    return Task.FromResult(
            //        request.CreateResponse(HttpStatusCode.NotFound));
            //}

            return base.SendAsync(request, cancellationToken);
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using WebsocketClient.Models;

namespace WebsocketServer
{
    /// <summary>
    /// Summary description for ws
    /// </summary>
    public class WebsocketServer : IHttpHandler
    {
        public WebsocketServer()
        {

        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(new SnakeGameWebSocketHandler());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        
    }
}